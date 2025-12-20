#include "FanPWM.h"

// Use official version macros to straddle 2.x and 3.x cores.
#ifndef ESP_ARDUINO_VERSION_MAJOR
  // Fallback: treat as 2.x if macros missing
  #define ESP_ARDUINO_VERSION_VAL(major, minor, patch) ((major<<16)|(minor<<8)|(patch))
  #define ESP_ARDUINO_VERSION ESP_ARDUINO_VERSION_VAL(2,0,0)
#endif

FanPWM::FanPWM(uint8_t pin, uint32_t freq_hz, uint8_t resolution_bits, uint8_t channel)
: pin_(pin),
  freq_(freq_hz),
  res_bits_(resolution_bits),
  channel_(channel),
  percent_(0),
  level_(FanSpeed::OFF),
  attached_(false) {}

bool FanPWM::begin() {
#if ESP_ARDUINO_VERSION >= ESP_ARDUINO_VERSION_VAL(3,0,0)
  attached_ = ledcAttach(pin_, freq_, res_bits_);
#else
  // Core 2.x API
  ledcSetup(channel_, freq_, res_bits_);
  ledcAttachPin(pin_, channel_);
  attached_ = true;
#endif
  if (!attached_) return false;
  setPercent(0);
  return true;
}

uint32_t FanPWM::dutyFromPercent(uint8_t pct) const {
  if (pct > 100) pct = 100;
  const uint32_t maxDuty = (1UL << res_bits_) - 1UL;
  return (maxDuty * pct) / 100UL;
}

void FanPWM::setPercent(uint8_t pct) {
  if (!attached_) return;
  if (pct > 100) pct = 100;
  percent_ = pct;

#if ESP_ARDUINO_VERSION >= ESP_ARDUINO_VERSION_VAL(3,0,0)
  ledcWrite(pin_, dutyFromPercent(percent_));
#else
  ledcWrite(channel_, dutyFromPercent(percent_));
#endif

  // Map to nearest named level for reporting
  if (percent_ == 0)       level_ = FanSpeed::OFF;
  else if (percent_ <= 40) level_ = FanSpeed::SLOW;
  else if (percent_ <= 70) level_ = FanSpeed::MEDIUM;
  else                     level_ = FanSpeed::FAST;
}

void FanPWM::setSpeed(FanSpeed s) {
  level_ = s;
  switch (s) {
    case FanSpeed::OFF:    setPercent(0);   break;
    case FanSpeed::SLOW:   setPercent(40);  break;
    case FanSpeed::MEDIUM: setPercent(46);  break;
    case FanSpeed::FAST:   setPercent(100); break;
  }
}

FanStatus FanPWM::status() const {
  return FanStatus{ level_, percent_ };
}

const char* FanPWM::speedToStr(FanSpeed s) {
  switch (s) {
    case FanSpeed::OFF:    return "OFF";
    case FanSpeed::SLOW:   return "SLOW";
    case FanSpeed::MEDIUM: return "MEDIUM";
    case FanSpeed::FAST:   return "FAST";
    default:               return "UNKNOWN";
  }
}

void FanPWM::printStatus(Stream& out) const {
  out.print(F("{\"speed\":\""));
  out.print(speedToStr(level_));
  out.print(F("\",\"percent\":"));
  out.print(percent_);
  out.println(F("}"));
}
