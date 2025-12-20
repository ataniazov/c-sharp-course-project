#pragma once
#include <Arduino.h>

class RainDrop {
public:
  enum Status : uint8_t { Dry = 0, Damp = 1, Wet = 2 };

  struct Sample {
    uint16_t raw_adc;        // 0..4095 by default on Arduino-ESP32
    float    wet_percent;    // 0..100 mapped using calibration
    bool     do_level;       // raw digital level from DO pin (true = HIGH)
    uint32_t timestamp_ms;   // millis() of last sample
  };

  // analogPin: any ADC1 pin on ESP32-C3 (GPIO0..GPIO4)
  // digitalPin: optional DO pin from LM393 board, or -1 if unused
  explicit RainDrop(int analogPin, int digitalPin = -1);

  // dryCal ~ AO reading when plate is dry; wetCal ~ AO when fully wet (lower voltage)
  void begin(uint16_t dryCal = 3700, uint16_t wetCal = 500, uint32_t samplePeriodMs = 50);

  // Non-blocking; call from loop(). Reads only when samplePeriodMs elapsed.
  void update();

  // On-demand query: returns last computed status without blocking.
  Status status() const;

  // String helper
  const char* statusText() const;

  // Access last sample
  Sample last() const { return last_; }

  // Tuning
  void setCalibration(uint16_t dryCal, uint16_t wetCal) { dry_ = dryCal; wet_ = wetCal; }
  void setSamplePeriod(uint32_t ms) { period_ = ms; }

private:
  int aPin_, dPin_;
  uint16_t dry_, wet_;
  uint32_t period_, lastTick_;
  Sample last_;

  static float mapWet(uint16_t raw, uint16_t dry, uint16_t wet) {
    if (dry == wet) return 0.f;
    // LM393 boards: wetter -> lower AO voltage -> lower raw ADC.
    float pct = 100.f * (float)((int)dry - (int)raw) / (float)((int)dry - (int)wet);
    if (pct < 0.f) pct = 0.f;
    if (pct > 100.f) pct = 100.f;
    return pct;
  }
};
