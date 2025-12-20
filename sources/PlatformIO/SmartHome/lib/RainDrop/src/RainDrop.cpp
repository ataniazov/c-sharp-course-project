#include "RainDrop.h"

RainDrop::RainDrop(int analogPin, int digitalPin)
: aPin_(analogPin), dPin_(digitalPin),
  dry_(3700), wet_(500), period_(50), lastTick_(0) {
  last_ = {0, 0.f, false, 0};
}

void RainDrop::begin(uint16_t dryCal, uint16_t wetCal, uint32_t samplePeriodMs) {
  dry_ = dryCal; wet_ = wetCal; period_ = samplePeriodMs;
  if (aPin_ >= 0) {
    pinMode(aPin_, INPUT);
    // Optional: analogSetPinAttenuation((adc1_channel_t)aPin_, ADC_11db); // requires ESP-IDF defs
  }
  if (dPin_ >= 0) {
    pinMode(dPin_, INPUT);
  }
  lastTick_ = 0;
  update(); // prime first sample
}

void RainDrop::update() {
  const uint32_t now = millis();
  if ((now - lastTick_) < period_) return;
  lastTick_ = now;

  uint16_t raw = 0;
  if (aPin_ >= 0) raw = analogRead(aPin_);
  bool level = false;
  if (dPin_ >= 0) level = digitalRead(dPin_) == HIGH;

  last_.raw_adc     = raw;
  last_.wet_percent = mapWet(raw, dry_, wet_);
  last_.do_level    = level;
  last_.timestamp_ms = now;
}

RainDrop::Status RainDrop::status() const {
  // Conservative thresholds; adjust in sketch if needed
  if (last_.do_level || last_.wet_percent >= 60.f) return Wet;
  if (last_.wet_percent >= 20.f) return Damp;
  return Dry;
}

const char* RainDrop::statusText() const {
  switch (status()) {
    case Dry:  return "Dry";
    case Damp: return "Damp";
    case Wet:  return "Wet";
  }
  return "Unknown";
}
