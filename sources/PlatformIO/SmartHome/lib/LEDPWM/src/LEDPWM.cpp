#include "LEDPWM.h"

// Map levels to perceptible steps (adjust as needed)
uint32_t LEDPWM::dutyFor(Level level) const {
  const uint32_t maxd = maxDuty();
  switch (level) {
    case Level::Low:    return (maxd * 5) / 100;  // ~5%
    case Level::Medium: return (maxd * 30) / 100;  // ~30%
    case Level::High:   return (maxd * 100) / 100;  // ~100%
    case Level::Off:
    default:            return 0;
  }
}

bool LEDPWM::begin() {
#if defined(ESP_ARDUINO_VERSION_MAJOR) && (ESP_ARDUINO_VERSION_MAJOR >= 3)
  // v3.x API: either auto or fixed channel
  _attached = (_channel >= 0)
              ? ledcAttachChannel(_pin, _freq, _res, _channel)
              : ledcAttach(_pin, _freq, _res);
#else
  // v2.x API: need explicit channel
  if (_channel < 0) _channel = 0; // default to channel 0
  ledcSetup(_channel, _freq, _res);
  _attached = true;
  ledcAttachPin(_pin, _channel);
#endif
  if (_attached) { off(); } // start off
  return _attached;
}

bool LEDPWM::writeDuty(uint32_t duty) {
  _duty = duty;
#if defined(ESP_ARDUINO_VERSION_MAJOR) && (ESP_ARDUINO_VERSION_MAJOR >= 3)
  // v3.x: pin-based, returns bool
  return ledcWrite(_pin, duty);
#else
  // v2.x: channel-based, returns void
  ledcWrite(_channel, duty);
  return true;
#endif
}


bool LEDPWM::on(Level level) {
  if (!_attached) return false;
  _level = level;
  return writeDuty(dutyFor(level));
}

bool LEDPWM::set(Level level) { return on(level); }

bool LEDPWM::off() {
  if (!_attached) return false;
  _level = Level::Off;
  return writeDuty(0);
}

String LEDPWM::statusString() const {
  String s = F("LED=");
  s += isOn() ? F("ON") : F("OFF");
  s += F(", level=");
  switch (_level) {
    case Level::Low:    s += F("Low"); break;
    case Level::Medium: s += F("Medium"); break;
    case Level::High:   s += F("High"); break;
    case Level::Off:    s += F("Off"); break;
  }
  s += F(", duty=");
  s += _duty; s += '/'; s += maxDuty();
  s += F(", freq="); s += _freq; s += F("Hz");
  s += F(", res=");  s += _res;  s += F(" bits");
  return s;
}
