#include <Wire.h>
#include <BH1750.h>

// Many ESP32-C3 boards: SDA=8, SCL=9. Change for your board.
constexpr int SDA_PIN = 7;
constexpr int SCL_PIN = 6;

BH1750 light(Wire, 0x23); // 0x23 default, 0x5C if ADDR tied high

void setup() {
  Serial.begin(115200);
  light.begin(SDA_PIN, SCL_PIN, 400000);
  light.setMTReg(69);                       // default sensitivity
  light.setMode(BH1750::ONE_HIGH_RES);  // 1 lx resolution, ~120 ms, auto power-down
}

void loop() {
  // Kick a measurement whenever we are idle or just consumed data.
  static uint32_t tLast = 0;
  if (millis() - tLast > 10) {              // pace command rate to avoid I2C spam
    if (!light.available() && light.getStatus().state == BH1750::IDLE) {
      light.request();                       // start measurement, non-blocking
    }
    tLast = millis();
  }

  light.poll();                              // advance state machine. No delays.

  // If user asks status over serial: send status
  while (Serial.available()) {
    char c = Serial.read();
    if (c == '?') {
      auto s = light.getStatus();
      Serial.print("state="); Serial.print(s.state);
      Serial.print(" addr=0x"); Serial.print(s.i2cAddr, HEX);
      Serial.print(" cmd=0x"); Serial.print(s.lastCmd, HEX);
      Serial.print(" mt="); Serial.print(s.mtReg);
      Serial.print(" waitMs="); Serial.print(s.waitMs);
      Serial.print(" err="); Serial.print(s.lastI2cErr);
      Serial.print(" raw="); Serial.print(s.lastRaw);
      Serial.print(" lux="); Serial.println(s.lastLux, 2);
    }
  }

  // When new data is ready, read and print
  if (light.available()) {
    float lux = light.readLux();
    Serial.print("Lux: "); Serial.println(lux, 2);
    // ready for next cycle; request again without blocking
    light.setMode(BH1750::ONE_HIGH_RES);
    light.request();
  }
}
