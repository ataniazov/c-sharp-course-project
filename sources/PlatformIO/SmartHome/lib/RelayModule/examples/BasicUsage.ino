#include <RelayModule.h>

// pick GPIOs that suit your board; avoid strapping pins.
// Example for ESP32-C3 dev boards:
static const uint8_t RELAY_PINS[] = { 0, 1 }; // edit as needed

RelayModule relays;

void setup() {
  Serial.begin(115200);
  relays.begin(RELAY_PINS, sizeof(RELAY_PINS), RelayModule::ActiveLow, /*initOff=*/true);
}

void loop() {
  relays.loop(); // required for timed pulses

  // demo: pulse relay 0 for 500 ms every 2 s without delay()
  static uint32_t t0 = 0;
  const uint32_t now = millis();
  if (now - t0 >= 2000) {
    t0 = now;
    relays.pulse(0, 500);
  }

  // query status on demand
  if (Serial.available()) {
    const int c = Serial.read();
    if (c == '0') relays.toggle(0);
    if (c == '1') relays.toggle(1);
    if (c == 's') {
      Serial.printf("CH0=%s CH1=%s\n",
                    relays.isOn(0) ? "ON" : "OFF",
                    relays.isOn(1) ? "ON" : "OFF");
    }
  }
}
