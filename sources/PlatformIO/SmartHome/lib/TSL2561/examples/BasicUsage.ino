#include <Arduino.h>
#include <Wire.h>
#include "TSL2561.h"

// Change as needed for your board wiring
constexpr int SDA_PIN = 7;  // ESP32-C3 typical
constexpr int SCL_PIN = 6;  // ESP32-C3 typical

TSL2561 light(0x39); // Addr: 0x29, 0x39 (default), or 0x49

void setup() {
  Serial.begin(115200);
  Wire.begin(SDA_PIN, SCL_PIN); // Explicit. Avoid hidden defaults.

  if (!light.begin(Wire, TSL2561::PACKAGE_T)) {
    Serial.println("TSL2561 init failed");
    for (;;) delay(1000);
  }

  light.configure(TSL2561::IT_101MS, TSL2561::GAIN_1X);
  light.request();
  Serial.println("Type 's' to print status on demand");
}

void loop() {
  // Drive the measurement state machine
  if (light.poll()) {
    float lux;
    if (light.readLux(lux)) {
      Serial.print("Lux: ");
      Serial.println(lux, 2);
      // Immediately queue next reading without blocking
      light.request();
    }
  }

  // On-demand status dump when user types 's'
  if (Serial.available()) {
    char c = Serial.read();
    if (c == 's' || c == 'S') {
      auto st = light.getStatus();
      Serial.println("--- TSL2561 Status ---");
      Serial.print("initialized="); Serial.println(st.initialized);
      Serial.print("measuring=");   Serial.println(st.measuring);
      Serial.print("dataReady=");   Serial.println(st.dataReady);
      Serial.print("saturated=");   Serial.println(st.saturated);
      Serial.print("lastErr=");     Serial.println(st.lastErr);
      Serial.print("addr=0x");     Serial.println(st.i2cAddr, HEX);
      Serial.print("it=");          Serial.println((int)st.it);
      Serial.print("gain=");        Serial.println((int)st.gain);
      Serial.print("rawCh0=");      Serial.println(st.rawCh0);
      Serial.print("rawCh1=");      Serial.println(st.rawCh1);
      Serial.print("lux=");         Serial.println(st.lux, 2);
      Serial.print("tStart=");      Serial.println(st.lastStartMs);
      Serial.print("tReady=");      Serial.println(st.lastReadyMs);
      Serial.println("----------------------");
    }
  }
}