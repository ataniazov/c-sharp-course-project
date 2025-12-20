#pragma once
#include <Arduino.h>

class LEDPWM {
public:
  enum class Level { Off = 0, Low, Medium, High };

  // channel: -1 = auto
  LEDPWM(uint8_t pin, uint32_t freqHz = 5000, uint8_t resolutionBits = 8, int8_t channel = -1)
    : _pin(pin), _freq(freqHz), _res(resolutionBits), _channel(channel) {}

  bool begin();                    // configure LEDC on the pin
  bool on(Level level);            // turn on with given level
  bool set(Level level);           // change level (non-blocking)
  bool off();                      // turn off
  bool isOn() const { return _duty > 0; }
  Level level() const { return _level; }
  uint32_t duty() const { return _duty; }
  uint32_t maxDuty() const { return (1u << _res) - 1u; }
  uint32_t freq() const { return _freq; }
  uint8_t resolution() const { return _res; }

  String statusString() const;     // human-readable status
  void printStatus(Stream& out = Serial) const { out.println(statusString()); }

private:
  uint8_t  _pin;
  uint32_t _freq;
  uint8_t  _res;
  int8_t   _channel;   // -1 auto
  uint32_t _duty = 0;
  Level    _level = Level::Off;
  bool     _attached = false;

  uint32_t dutyFor(Level level) const; // map level -> duty
  bool writeDuty(uint32_t duty);       // wraps LEDC write by pin
};
