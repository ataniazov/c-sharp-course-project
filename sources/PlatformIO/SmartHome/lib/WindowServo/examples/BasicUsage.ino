#include <Arduino.h>
#include <WindowServo.h>

// ESP32-C3 example pin and channel
constexpr uint8_t SERVO_PIN = 4;
constexpr uint8_t SERVO_LEDC_CH = 0;

WindowServo win;

static const __FlashStringHelper* stateToText(WindowServo::State s) {
  switch (s) {
    case WindowServo::State::CLOSED:  return F("CLOSED");
    case WindowServo::State::OPEN:    return F("OPEN");
    case WindowServo::State::OPENING: return F("OPENING");
    case WindowServo::State::CLOSING: return F("CLOSING");
    case WindowServo::State::IDLE:    return F("IDLE");
  }
  return F("?");
}

void setup() {
  Serial.begin(115200);
  if (!win.begin(SERVO_PIN, SERVO_LEDC_CH)) {
    Serial.println("LEDC init failed");
    while (1) {}
  }
  win.setAngles(0, 90);
  win.setSpeed(WindowServo::Speed::FAST);
  Serial.println("Commands: o=open, c=close, s=status, 1/2/3=speed S/M/F");
}

void loop() {
  if (Serial.available()) {
    char ch = (char)Serial.read();
    if (ch == 'o') win.open();
    else if (ch == 'c') win.close();
    else if (ch == '1') win.setSpeed(WindowServo::Speed::SLOW);
    else if (ch == '2') win.setSpeed(WindowServo::Speed::MEDIUM);
    else if (ch == '3') win.setSpeed(WindowServo::Speed::FAST);
    else if (ch == 's') {
      Serial.print("Status=");
      Serial.print(stateToText(win.status()));
      Serial.print(" angle=");
      Serial.println(win.angleDeg(), 1);
    }
  }
  // Non-blocking. All motion handled in timer ISR.
}
