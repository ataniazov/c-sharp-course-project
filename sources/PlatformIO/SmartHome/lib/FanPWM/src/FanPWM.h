#pragma once
#include <Arduino.h>

enum class FanSpeed : uint8_t {
  OFF = 0,
  SLOW,
  MEDIUM,
  FAST
};

struct FanStatus {
  FanSpeed speed;
  uint8_t percent; // 0..100
};

class FanPWM {
public:
  // For core 2.x you may want an explicit channel (0..5 on ESP32-C3).
  // For core 3.x channel is auto-assigned by ledcAttach, 'channel_' is ignored.
  explicit FanPWM(uint8_t pin, uint32_t freq_hz = 25000, uint8_t resolution_bits = 8, uint8_t channel = 0);

  bool begin();
  void setSpeed(FanSpeed s);         // OFF/SLOW/MEDIUM/FAST
  void setPercent(uint8_t pct);      // 0..100
  FanStatus status() const;
  void printStatus(Stream& out) const;

  uint8_t percent() const { return percent_; }
  FanSpeed speed() const { return level_; }

private:
  uint32_t dutyFromPercent(uint8_t pct) const;
  static const char* speedToStr(FanSpeed s);

  uint8_t pin_;
  uint32_t freq_;
  uint8_t res_bits_;
  uint8_t channel_;   // used on core 2.x
  uint8_t percent_;   // 0..100
  FanSpeed level_;
  bool attached_;
};
