#include <Arduino.h>
#include <DS18B20.h>

// Set your DS18B20 data pin. Use a 4.7k pull-up to 3V3.
static constexpr uint8_t ONE_WIRE_PIN = 10; // change to your wiring

DS18B20 temp(ONE_WIRE_PIN, 12 /*bits*/, 250 /*ms*/);

void setup() {
  Serial.begin(115200);
  while(!Serial && millis() < 2000) {}
  if (!temp.begin()) {
    Serial.println("DS18B20 init failed. Check wiring.");
  } else {
    Serial.println("DS18B20 init OK.");
  }
}

void loop() {
  temp.poll();  // non-blocking

  if (temp.available()) {
    float c = temp.readC();
    Serial.printf("T=%.3f C\n", c);
  }

  // Simple "ask for status" via serial
  if (Serial.available()) {
    String cmd = Serial.readStringUntil('\n');
    cmd.trim();
    if (cmd.equalsIgnoreCase("status")) {
      Serial.println(temp.status());
    } else if (cmd.equalsIgnoreCase("now")) {
      temp.requestNow();  // force a new conversion without blocking
      Serial.println("{\"requested\":true}");
    }
  }

  // do other work here; loop stays responsive
}
