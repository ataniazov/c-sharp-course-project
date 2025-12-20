#pragma once
#include <Arduino.h>

class RelayModule {
public:
  enum ActiveLevel : uint8_t { ActiveLow = 0, ActiveHigh = 1 };

  RelayModule() : _count(0), _level(ActiveLow), _begun(false) {}

  bool begin(const uint8_t* pins, size_t count,
             ActiveLevel level = ActiveLow, bool initOff = true);

  void loop();                          // service timed pulses
  bool on(size_t ch);                   // immediate ON
  bool off(size_t ch);                  // immediate OFF
  bool toggle(size_t ch);               // toggle
  bool pulse(size_t ch, uint32_t ms);   // ON for ms, then OFF (non-blocking)

  size_t channelCount() const { return _count; }
  bool isOn(size_t ch) const;           // current state
  ActiveLevel activeLevel() const { return _level; }

private:
  struct Channel {
    uint8_t pin = 0xFF;
    bool state = false;           // logical ON/OFF
    uint32_t pulseEnd = 0;        // millis timestamp or 0 if none
  };

  void writeHW(size_t idx, bool on);

  static constexpr size_t MAX_CH = 8;
  Channel _chs[MAX_CH];
  size_t _count;
  ActiveLevel _level;
  bool _begun;
};
