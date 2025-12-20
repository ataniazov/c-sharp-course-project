#pragma once
#include <Arduino.h>
#include <Wire.h>

class TSL2561 {
public:
  enum IntegrationTime : uint8_t { IT_13MS = 0, IT_101MS = 1, IT_402MS = 2 };
  enum Gain : uint8_t { GAIN_1X = 0, GAIN_16X = 1 };
  enum Package : uint8_t { PACKAGE_T = 0, PACKAGE_CS = 1 }; // Most modules use PACKAGE_T

  struct Status {
    bool initialized = false;
    bool measuring = false;
    bool dataReady = false;
    bool saturated = false;
    int lastErr = 0; // 0 OK; otherwise Wire error
    uint8_t i2cAddr = 0x39;
    IntegrationTime it = IT_402MS;
    Gain gain = GAIN_1X;
    Package pkg = PACKAGE_T;
    uint32_t lastStartMs = 0;
    uint32_t lastReadyMs = 0;
    uint16_t rawCh0 = 0;
    uint16_t rawCh1 = 0;
    float lux = NAN;
  };

  // Construct with optional I2C address (0x29, 0x39 default, or 0x49)
  explicit TSL2561(uint8_t addr = 0x39) : _addr(addr) {}

  bool begin(TwoWire &wire = Wire, Package pkg = PACKAGE_T);
  void configure(IntegrationTime it, Gain gain);

  // Start a measurement if idle. Returns true on start, false if busy or error.
  bool request();

  // Call frequently from loop(). Returns true when a new sample was captured.
  bool poll();

  // True when a new sample is available since last read.
  bool available() const { return _st.dataReady; }

  // Copy out latest sample and clear the ready flag. Returns true on success.
  bool readLux(float &outLux);

  // Read-only status snapshot.
  Status getStatus() const { return _st; }

private:
  // I2C register map constants
  static constexpr uint8_t CMD = 0x80;
  static constexpr uint8_t CMD_WORD = 0x20; // not used; we read bytes
  static constexpr uint8_t REG_CONTROL = 0x00;
  static constexpr uint8_t REG_TIMING  = 0x01;
  static constexpr uint8_t REG_ID      = 0x0A;
  static constexpr uint8_t REG_DATA0_L = 0x0C;
  static constexpr uint8_t REG_DATA0_H = 0x0D;
  static constexpr uint8_t REG_DATA1_L = 0x0E;
  static constexpr uint8_t REG_DATA1_H = 0x0F;

  static constexpr uint8_t CONTROL_POWERON  = 0x03;
  static constexpr uint8_t CONTROL_POWEROFF = 0x00;

  // Fixed-point lux algorithm constants (DN40/DN29)
  static constexpr uint8_t  LUX_SCALE   = 14; // scale by 2^14
  static constexpr uint8_t  RATIO_SCALE = 9;  // scale ratio by 2^9
  static constexpr uint8_t  CH_SCALE    = 10; // scale channel values by 2^10
  static constexpr uint16_t CHSCALE_TINT0 = 0x7517; // 13.7ms
  static constexpr uint16_t CHSCALE_TINT1 = 0x0FE7; // 101ms

  // T package segmented approximation coefficients
  static constexpr uint16_t K1T = 0x0040; // 0.125 * 2^RATIO_SCALE
  static constexpr uint16_t B1T = 0x01f2; // 0.0304 * 2^LUX_SCALE
  static constexpr uint16_t M1T = 0x01be; // 0.0272 * 2^LUX_SCALE
  static constexpr uint16_t K2T = 0x0080; // 0.250
  static constexpr uint16_t B2T = 0x0214; // 0.0325
  static constexpr uint16_t M2T = 0x02d1; // 0.0440
  static constexpr uint16_t K3T = 0x00c0; // 0.375
  static constexpr uint16_t B3T = 0x023f; // 0.0351
  static constexpr uint16_t M3T = 0x037b; // 0.0544
  static constexpr uint16_t K4T = 0x0100; // 0.50
  static constexpr uint16_t B4T = 0x0270; // 0.0381
  static constexpr uint16_t M4T = 0x03fe; // 0.0624
  static constexpr uint16_t K5T = 0x0138; // 0.61
  static constexpr uint16_t B5T = 0x016f; // 0.0224
  static constexpr uint16_t M5T = 0x01fc; // 0.0310
  static constexpr uint16_t K6T = 0x019a; // 0.80
  static constexpr uint16_t B6T = 0x00d2; // 0.0128
  static constexpr uint16_t M6T = 0x00fb; // 0.0153
  static constexpr uint16_t K7T = 0x029a; // 1.3
  static constexpr uint16_t B7T = 0x0018; // 0.00146
  static constexpr uint16_t M7T = 0x0012; // 0.00112
  static constexpr uint16_t K8T = 0x029a; // 1.3
  static constexpr uint16_t B8T = 0x0000; // 0.000
  static constexpr uint16_t M8T = 0x0000; // 0.000

  // CS package coefficients
  static constexpr uint16_t K1C = 0x0043; // 0.130
  static constexpr uint16_t B1C = 0x0204; // 0.0315
  static constexpr uint16_t M1C = 0x01ad; // 0.0262
  static constexpr uint16_t K2C = 0x0085; // 0.260
  static constexpr uint16_t B2C = 0x0228; // 0.0337
  static constexpr uint16_t M2C = 0x02c1; // 0.0430
  static constexpr uint16_t K3C = 0x00c8; // 0.390
  static constexpr uint16_t B3C = 0x0253; // 0.0363
  static constexpr uint16_t M3C = 0x0363; // 0.0529
  static constexpr uint16_t K4C = 0x010a; // 0.520
  static constexpr uint16_t B4C = 0x0282; // 0.0392
  static constexpr uint16_t M4C = 0x03df; // 0.0605
  static constexpr uint16_t K5C = 0x014d; // 0.65
  static constexpr uint16_t B5C = 0x0177; // 0.0229
  static constexpr uint16_t M5C = 0x01dd; // 0.0291
  static constexpr uint16_t K6C = 0x019a; // 0.80
  static constexpr uint16_t B6C = 0x0101; // 0.0157
  static constexpr uint16_t M6C = 0x0127; // 0.0180
  static constexpr uint16_t K7C = 0x029a; // 1.3
  static constexpr uint16_t B7C = 0x0037; // 0.00338
  static constexpr uint16_t M7C = 0x002b; // 0.00260
  static constexpr uint16_t K8C = 0x029a; // 1.3
  static constexpr uint16_t B8C = 0x0000;
  static constexpr uint16_t M8C = 0x0000;

  enum State : uint8_t { IDLE=0, INTEGRATING };

  // helpers
  bool write8(uint8_t reg, uint8_t val);
  bool read8(uint8_t reg, uint8_t &val);
  bool read16(uint8_t reg, uint16_t &val);
  void power(bool on);
  uint32_t currentIntegrationWindowMs() const;
  uint32_t nowMs() const { return millis(); }
  uint32_t computeLux(uint16_t ch0, uint16_t ch1) const;

  TwoWire* _wire = nullptr;
  uint8_t _addr;
  State _state = IDLE;
  Status _st;
};