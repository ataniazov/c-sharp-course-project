#pragma once
#include <Arduino.h>
#include <OneWire.h>
#include <DallasTemperature.h>

class DS18B20 {
public:
  enum State : uint8_t { IDLE=0, CONVERTING=1, READY=2, ERROR=3 };

  // samplePeriodMs: how often to start a new conversion; 0 = manual only
  explicit DS18B20(uint8_t pin, uint8_t resolutionBits = 12, uint32_t samplePeriodMs = 1000);

  bool begin();                         // returns false if no sensor
  void poll();                          // call often from loop()
  void requestNow();                    // trigger a conversion immediately (non-blocking)
  bool available() const;               // true if a fresh value is ready
  float readC();                        // consume last sample (clears available)
  float readF();                        // helper
  State state() const { return _state; }

  void setSamplePeriod(uint32_t ms) { _periodMs = ms; }
  void setResolution(uint8_t bits);     // 9..12
  String status() const;                // human-readable status on demand

private:
  uint16_t convTimeMs(uint8_t bits) const; // per DS18B20 datasheet
  void startConversion_();

  OneWire _ow;
  DallasTemperature _sensors;
  DeviceAddress _addr{};
  bool _hasAddr = false;
  uint8_t _resolution = 12;
  uint32_t _periodMs = 1000;

  State _state = IDLE;
  uint32_t _tReq = 0;
  uint32_t _tReady = 0;

  float _lastC = NAN;
  bool _fresh = false;
  uint32_t _okCount = 0;
  uint32_t _errCount = 0;
};
