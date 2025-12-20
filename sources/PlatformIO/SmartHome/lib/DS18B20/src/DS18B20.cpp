#include "DS18B20.h"

DS18B20::DS18B20(uint8_t pin, uint8_t resolutionBits, uint32_t samplePeriodMs)
: _ow(pin), _sensors(&_ow), _resolution(resolutionBits), _periodMs(samplePeriodMs) {}

bool DS18B20::begin() {
  _sensors.begin();
  _sensors.setWaitForConversion(false);      // non-blocking mode (we handle timing)
  int count = _sensors.getDeviceCount();
  if (count <= 0) { _state = ERROR; return false; }
  _hasAddr = _sensors.getAddress(_addr, 0);
  if (!_hasAddr) { _state = ERROR; return false; }
  setResolution(_resolution);
  _state = IDLE;
  _tReq = millis();
  return true;
}

void DS18B20::setResolution(uint8_t bits) {
  if (bits < 9) bits = 9;
  if (bits > 12) bits = 12;
  _resolution = bits;
  if (_hasAddr) _sensors.setResolution(_addr, _resolution);
  else          _sensors.setResolution(_resolution);
}

uint16_t DS18B20::convTimeMs(uint8_t bits) const {
  // DS18B20 datasheet max conversion times
  switch (bits) {
    case 9:  return 94;
    case 10: return 188;
    case 11: return 375;
    default: return 750; // 12-bit
  }
}

void DS18B20::startConversion_() {
  _sensors.requestTemperatures();            // returns immediately (non-blocking)
  uint16_t t = convTimeMs(_resolution);
  // add small margin against bus/JIT jitter
  _tReq = millis();
  _tReady = _tReq + t + 5;
  _state = CONVERTING;
}

void DS18B20::requestNow() {
  if (_state == CONVERTING) return;          // already running; ignore
  startConversion_();
}

void DS18B20::poll() {
  if (_state == ERROR) return;

  uint32_t now = millis();
  if (_state == IDLE) {
    if (_periodMs && (int32_t)(now - _tReq) >= (int32_t)_periodMs) startConversion_();
    return;
  }

  if (_state == CONVERTING) {
    // Option A: time-based readiness (portable and zero-bus overhead)
    if ((int32_t)(now - _tReady) >= 0) {
      float c = _hasAddr ? _sensors.getTempC(_addr) : _sensors.getTempCByIndex(0);
      if (c == DEVICE_DISCONNECTED_C || c == 85.0f) {
        _errCount++;
        _state = IDLE;
        _tReq = now;     // schedule next window
        return;
      }
      _lastC = c;
      _fresh = true;
      _okCount++;
      _state = READY;
      _tReq = now;
    }
    return;
  }

  if (_state == READY) {
    // Hold READY until sample period elapses or user reads the value; then schedule next
    if (_periodMs && (int32_t)(now - _tReq) >= (int32_t)_periodMs) {
      startConversion_();
    }
  }
}

bool DS18B20::available() const { return _fresh; }

float DS18B20::readC() {
  _fresh = false;
  return _lastC;
}

float DS18B20::readF() { return isnan(_lastC) ? NAN : _lastC * 1.8f + 32.0f; }

String DS18B20::status() const {
  String s = "{";
  s += "\"state\":" + String(_state);
  s += ",\"hasDevice\":" + String(_hasAddr ? "true" : "false");
  s += ",\"resolutionBits\":" + String(_resolution);
  s += ",\"lastC\":" + String(_lastC, 3);
  s += ",\"ok\":" + String(_okCount);
  s += ",\"err\":" + String(_errCount);
  s += ",\"periodMs\":" + String(_periodMs);
  s += ",\"tReq\":" + String(_tReq);
  s += ",\"tReady\":" + String(_tReady);
  s += "}";
  return s;
}
