#include <FanPWM.h>

// Choose an LEDC-capable GPIO on your ESP32-C3 board.
#define FAN_PIN 5

FanPWM fan(FAN_PIN, 25000, 8); // 25 kHz, 8-bit

void setup() {
  Serial.begin(115200);
  while (!Serial) { /* no blocking delay */ }
  if (!fan.begin()) {
    Serial.println(F("{\"error\":\"ledcAttach failed\"}"));
  } else {
    fan.setSpeed(FanSpeed::SLOW);
    Serial.println(F("Type: 0=OFF, 1=SLOW, 2=MEDIUM, 3=FAST, pNN=set percent, status=report"));
  }
}

void loop() {
  if (Serial.available()) {
    String cmd = Serial.readStringUntil('\n');
    cmd.trim();
    if (cmd == "0") fan.setSpeed(FanSpeed::OFF);
    else if (cmd == "1") fan.setSpeed(FanSpeed::SLOW);
    else if (cmd == "2") fan.setSpeed(FanSpeed::MEDIUM);
    else if (cmd == "3") fan.setSpeed(FanSpeed::FAST);
    else if (cmd.startsWith("p")) {
      int pct = cmd.substring(1).toInt();
      fan.setPercent((uint8_t)constrain(pct, 0, 100));
    } else if (cmd == "status" || cmd == "s") {
      fan.printStatus(Serial);
    } else {
      Serial.println(F("{\"hint\":\"0|1|2|3|pNN|status\"}"));
    }
  }

  // Non-blocking main loop; add your app logic here.
}
