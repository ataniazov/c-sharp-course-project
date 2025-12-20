#include <Arduino.h>
#include <RainDrop.h>

// Select an ESP32-C3 ADC1 pin for AO: GPIO0..GPIO4 (board-dependent labeling)
// Example uses GPIO2 for AO and GPIO7 for DO (any regular digital pin is fine).
static constexpr int RAINDROP_AO = 1;   // change to your wiring
static constexpr int RAINDROP_DO = -1;   // or -1 if you don't use DO

RainDrop rain(RAINDROP_AO, RAINDROP_DO);

void setup() {
  Serial.begin(115200);
  // Calibrate once your board is assembled: read raw when dry and when wet, then update.
  rain.begin(/*dryCal=*/3700, /*wetCal=*/500, /*samplePeriodMs=*/50);
}

void loop() {
  // Fully non-blocking: update returns immediately if not time yet.
  rain.update();

  // "Send status when asked": here we print on a key press or command.
  if (Serial.available()) {
    const char c = Serial.read();
    if (c == 's') {
      auto s = rain.last();
      Serial.printf("Status=%s raw=%u wet=%.1f%% DO=%d at %lu ms\n",
                    rain.statusText(), s.raw_adc, s.wet_percent, s.do_level, s.timestamp_ms);
    } else if (c == 'c') {
      // Quick assist to help you calibrate: print current raw
      auto s = rain.last();
      Serial.printf("Current raw ADC=%u; use as dry/wet reference\n", s.raw_adc);
    }
  }

  // Do other work here. No delays required.
}
