
#include <Arduino.h>
#include <LEDPWM.h>

// Adjust to your board's LED pin. Example: ESP32-C3-DevKit often uses GPIO8 or GPIO7.
#ifndef LED_PIN
#define LED_PIN 3
#endif

LEDPWM led(LED_PIN, 5000, 8, 0); // 5 kHz, 8-bit, channel 0 (v3 auto-assigns if channel=-1)

void setup() {
  Serial.begin(115200);
  while(!Serial) {}
  led.begin();
  led.on(LEDPWM::Level::Medium);
  Serial.println(F("Type commands: LOW | MEDIUM | HIGH | OFF | STATUS"));
}

static void handleCommand(const String& cmd) {
  if (cmd == "LOW")    led.set(LEDPWM::Level::Low);
  else if (cmd == "MEDIUM") led.set(LEDPWM::Level::Medium);
  else if (cmd == "HIGH")   led.set(LEDPWM::Level::High);
  else if (cmd == "OFF")    led.off();
  else if (cmd == "STATUS") led.printStatus();
}

void loop() {
  static String buf;
  while (Serial.available()) {
    char c = (char)Serial.read();
    if (c == '\n' || c == '\r') {
      buf.trim();
      if (buf.length()) handleCommand(buf);
      buf = "";
    } else {
      buf += (c >= 'a' && c <= 'z') ? (char)(c - 32) : c; // upper-case
    }
  }
  // no delays; library is fully non-blocking
}
