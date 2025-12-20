#pragma once
#include <Arduino.h>

class WindowServo {
public:
  enum class Speed : uint8_t { SLOW, MEDIUM, FAST };
  enum class State : uint8_t { CLOSED, OPEN, OPENING, CLOSING, IDLE };

  // Arduino-ESP32 v2.x signature: needs a LEDC channel
  bool begin(uint8_t pin,
             uint8_t ledcChannel = 0,
             uint16_t usMin = 500,
             uint16_t usMax = 2400,
             uint8_t resolutionBits = 14,
             uint32_t pwmHz = 50);

  void setSpeed(Speed s);
  void setAngles(float closeDeg, float openDeg);
  void open();
  void close();
  void stop();
  void state(State st);

  Speed speed() const { return _speed; }
  State status() const { return _state; }
  float angleDeg() const { return _currentDeg; }
  bool isMoving() const { return _moving; }

private:
  static void IRAM_ATTR _onTimerIsr();
  static WindowServo* _self; // single instance ISR pointer

  void _startTimer();
  void _stopTimer();
  void _tick();               // called from ISR
  void _moveTo(float targetDeg);
  uint32_t _dutyFromUs(uint16_t us) const;
  uint16_t _usFromDeg(float deg) const;
  void _writeDutyFromDeg(float deg);

  uint8_t  _pin = 0;
  uint8_t  _ch  = 0;
  uint8_t  _resBits = 14;
  uint32_t _freqHz  = 50;
  uint32_t _maxDuty = 0;
  uint16_t _usMin = 500, _usMax = 2400;

  float _closeDeg = 0.0f, _openDeg = 90.0f;

  volatile float _currentDeg = 0.0f;
  volatile float _targetDeg  = 0.0f;
  volatile bool  _moving = false;
  volatile State _state  = State::CLOSED;

  Speed _speed = Speed::MEDIUM;

  // v2.x hardware timer API
  hw_timer_t* _timer = nullptr;
  static constexpr uint32_t TICK_US = 10000; // 10 ms tick
};
