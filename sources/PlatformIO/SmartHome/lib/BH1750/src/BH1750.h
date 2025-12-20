#pragma once
#include <Arduino.h>
#include <Wire.h>

class BH1750 {
public:
  enum Mode : uint8_t {
    CONT_HIGH_RES   = 0x10,
    CONT_HIGH_RES2  = 0x11,
    CONT_LOW_RES    = 0x13,
    ONE_HIGH_RES    = 0x20,
    ONE_HIGH_RES2   = 0x21,
    ONE_LOW_RES     = 0x23
  };
  enum State : uint8_t { POWER_DOWN, IDLE, STARTED, WAITING, READY, ERROR };

  struct Status {
    State state;
    uint8_t i2cAddr;
    uint8_t lastCmd;
    uint8_t mtReg;       // 31..254, default 69
    uint32_t tStartMs;
    uint32_t waitMs;
    int lastI2cErr;      // from Wire.endTransmission(), or <0 on read error
    uint16_t lastRaw;
    float lastLux;
    bool dataReady;
  };

  explicit BH1750(TwoWire& w = Wire, uint8_t i2cAddr = 0x23);

  bool begin(int sda = -1, int scl = -1, uint32_t freq = 400000);
  void setAddress(uint8_t addr);
  void setMode(Mode m);
  void setMTReg(uint8_t mt);             // 31..254, default 69
  bool request();                        // start a measurement (non-blocking)
  void poll();                           // advance state machine
  bool available() const;                // new data ready?
  float readLux();                       // read and clear ready flag
  Status getStatus() const;
  bool powerOn();
  bool powerDown();

private:
  TwoWire* _wire;
  uint8_t _addr;
  Mode _mode;
  uint8_t _mtreg;
  State _state;
  uint32_t _tstart;
  uint32_t _wait_ms;
  bool _hasData;
  uint16_t _raw;
  float _lux;
  int _err;

  bool write8(uint8_t cmd);
  bool readRaw(uint16_t& raw);
  uint32_t modeBaseTimeMs(Mode m) const;
  float convDivisorForMode() const;      // 1.2 * (69/MTreg) with mode adjust
};
