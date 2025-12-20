#include "WindowServo.h"

// Use official version macros to straddle 2.x and 3.x cores.
#ifndef ESP_ARDUINO_VERSION_MAJOR
  // Fallback: treat as 2.x if macros missing
  #define ESP_ARDUINO_VERSION_VAL(major, minor, patch) ((major<<16)|(minor<<8)|(patch))
  #define ESP_ARDUINO_VERSION ESP_ARDUINO_VERSION_VAL(2,0,0)
#endif

// static
WindowServo* WindowServo::_self = nullptr;

// ---- Helpers ----
uint32_t WindowServo::_dutyFromUs(uint16_t us) const {
  const uint32_t periodUs = 1000000UL / _freqHz; // 20000 at 50 Hz
  return (uint32_t)(((uint64_t)us * _maxDuty) / periodUs);
}

uint16_t WindowServo::_usFromDeg(float deg) const {
  if (deg < 0) deg = 0;
  if (deg > 180) deg = 180;
  const float span = float(_usMax - _usMin);
  return (uint16_t)(_usMin + (span * (deg / 180.0f)));
}

void WindowServo::_writeDutyFromDeg(float deg) {
  const uint32_t duty = _dutyFromUs(_usFromDeg(deg));
  ledcWrite(_ch, duty);
}

// ---- Public API ----
bool WindowServo::begin(uint8_t pin, uint8_t ledcChannel, uint16_t usMin, uint16_t usMax,
                        uint8_t resolutionBits, uint32_t pwmHz) {
  _pin = pin;
  _ch  = ledcChannel;
  _usMin = usMin;
  _usMax = usMax;
  _resBits = resolutionBits;
  _freqHz = pwmHz;
  _maxDuty = (1UL << _resBits) - 1;

  // Arduino-ESP32 v2.x LEDC init
  if (!ledcSetup(_ch, _freqHz, _resBits)) {
    return false;
  }
  ledcAttachPin(_pin, _ch);

  _currentDeg = _closeDeg;
  _targetDeg  = _closeDeg;
  _state = State::CLOSED;
  _moving = false;

  _writeDutyFromDeg(_currentDeg);

  // Prepare timer ISR
  _self = this; // one instance for ISR
  _timer = timerBegin(0, 80, true);           // 80 MHz / 80 = 1 MHz tick (1 µs)
  timerAttachInterrupt(_timer, &_onTimerIsr, true);
  timerAlarmWrite(_timer, TICK_US, true);     // 10 ms periodic
  // Do not enable yet; start when a move begins
  return true;
}

void WindowServo::setSpeed(Speed s) { _speed = s; }
void WindowServo::setAngles(float closeDeg, float openDeg) {
  _closeDeg = closeDeg;
  _openDeg  = openDeg;
}

void WindowServo::open()  { _state = State::OPENING;  _moveTo(_openDeg);  }
void WindowServo::close() { _state = State::CLOSING;  _moveTo(_closeDeg); }

void WindowServo::stop() {
  _moving = false;
  _stopTimer();
  _state = State::IDLE;
}

void WindowServo::state(State st) {
  if (st == State::OPEN) {
    open();
  } else if (st == State::CLOSED) {
    close();
  } else if (st == State::IDLE) {
    stop();
  }
}

void WindowServo::_startTimer() { timerAlarmEnable(_timer); }
void WindowServo::_stopTimer()  { timerAlarmDisable(_timer); }

// ---- Motion core (non-blocking via timer ISR stepping) ----
void WindowServo::_moveTo(float targetDeg) {
  _targetDeg = targetDeg;
  _moving = true;
  _startTimer();
}

// Called every 10 ms in ISR context
void WindowServo::_tick() {
  if (!_moving) return;

  float dps = 180.0f;
  if (_speed == Speed::SLOW)  dps = 45.0f;
  if (_speed == Speed::FAST)  dps = 500.0f;

  const float step = dps * (TICK_US / 1e6f); // deg per tick
  float next = _currentDeg;

  if (_targetDeg > _currentDeg) {
    next = _currentDeg + step;
    if (next > _targetDeg) next = _targetDeg;
  } else if (_targetDeg < _currentDeg) {
    next = _currentDeg - step;
    if (next < _targetDeg) next = _targetDeg;
  }

  _currentDeg = next;
  _writeDutyFromDeg(_currentDeg);

  // Stop exactly at target when within one step
  if (fabsf(_targetDeg - _currentDeg) <= step) {
    _currentDeg = _targetDeg;        // snap to exact
    _moving = false;
    _stopTimer();
    if (fabsf(_currentDeg - _openDeg)   < 0.5f) _state = State::OPEN;
    else if (fabsf(_currentDeg - _closeDeg) < 0.5f) _state = State::CLOSED;
    else _state = State::IDLE;
  }
}

// ISR trampoline
void IRAM_ATTR WindowServo::_onTimerIsr() {
  if (_self) _self->_tick();
}
