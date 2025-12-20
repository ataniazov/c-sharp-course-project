#include "RelayModule.h"

bool RelayModule::begin(const uint8_t* pins, size_t count,
                        ActiveLevel level, bool initOff) {
  if (!pins || count == 0 || count > MAX_CH) return false;
  _count = count;
  _level = level;
  for (size_t i = 0; i < _count; ++i) {
    _chs[i].pin = pins[i];
    _chs[i].state = false;
    _chs[i].pulseEnd = 0;
    pinMode(_chs[i].pin, OUTPUT);
  }
  // initialize hardware without blocking
  for (size_t i = 0; i < _count; ++i) writeHW(i, !initOff ? true : false);
  _begun = true;
  return true;
}

void RelayModule::writeHW(size_t idx, bool on) {
  if (idx >= _count) return;
  _chs[idx].state = on;
  const bool active = (_level == ActiveLow) ? LOW : HIGH;
  const bool inactive = (_level == ActiveLow) ? HIGH : LOW;
  digitalWrite(_chs[idx].pin, on ? active : inactive);
}

void RelayModule::loop() {
  if (!_begun) return;
  const uint32_t now = millis();
  for (size_t i = 0; i < _count; ++i) {
    if (_chs[i].pulseEnd && ((int32_t)(now - _chs[i].pulseEnd) >= 0)) {
      _chs[i].pulseEnd = 0;
      writeHW(i, false);
    }
  }
}

bool RelayModule::on(size_t ch)            { if (ch >= _count) return false; _chs[ch].pulseEnd = 0; writeHW(ch, true);  return true; }
bool RelayModule::off(size_t ch)           { if (ch >= _count) return false; _chs[ch].pulseEnd = 0; writeHW(ch, false); return true; }
bool RelayModule::toggle(size_t ch)        { if (ch >= _count) return false; return _chs[ch].state ? off(ch) : on(ch); }
bool RelayModule::pulse(size_t ch, uint32_t ms) {
  if (ch >= _count) return false;
  writeHW(ch, true);
  _chs[ch].pulseEnd = ms ? millis() + ms : 0;
  return true;
}
bool RelayModule::isOn(size_t ch) const    { if (ch >= _count) return false; return _chs[ch].state; }
