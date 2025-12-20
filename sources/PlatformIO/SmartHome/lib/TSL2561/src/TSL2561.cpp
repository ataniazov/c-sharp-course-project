#include "TSL2561.h"

bool TSL2561::begin(TwoWire &wire, Package pkg) {
  _wire = &wire;
  _st.pkg = pkg;
  // Probe ID register to verify presence
  uint8_t id = 0;
  if (!read8(REG_ID, id)) {
    _st.lastErr = -1;
    _st.initialized = false;
    return false;
  }
  _st.initialized = true;
  _st.lastErr = 0;
  // Default configuration
  configure(IT_402MS, GAIN_1X);
  power(false);
  return true;
}

void TSL2561::configure(IntegrationTime it, Gain gain) {
  _st.it = it;
  _st.gain = gain;
}

bool TSL2561::request() {
  if (!_st.initialized) return false;
  if (_state != IDLE) return false;

  // Apply timing and gain
  uint8_t timing = 0x00;
  switch (_st.it) {
    case IT_13MS:  timing = 0x00; break; // 13.7ms
    case IT_101MS: timing = 0x01; break; // 101ms
    case IT_402MS: timing = 0x02; break; // 402ms
  }
  if (_st.gain == GAIN_16X) timing |= 0x10; // GAIN bit

  power(true);
  if (!write8(REG_TIMING, timing)) {
    _st.lastErr = -2;
    return false;
  }

  _st.measuring = true;
  _st.dataReady = false;
  _st.saturated = false;
  _st.lastStartMs = nowMs();
  _state = INTEGRATING;
  return true;
}

bool TSL2561::poll() {
  if (_state != INTEGRATING) return false;

  uint32_t elapsed = nowMs() - _st.lastStartMs;
  if (elapsed < currentIntegrationWindowMs()) return false;

  // Read results
  uint16_t ch0 = 0, ch1 = 0;
  bool ok = read16(REG_DATA0_L, ch0) && read16(REG_DATA1_L, ch1);
  if (!ok) {
    _st.lastErr = -3;
    power(false);
    _state = IDLE;
    _st.measuring = false;
    return false;
  }

  _st.rawCh0 = ch0;
  _st.rawCh1 = ch1;
  _st.saturated = (ch0 == 0xFFFFu) || (ch1 == 0xFFFFu);

  // Compute lux only if not saturated and ch0 != 0
  if (!_st.saturated && ch0 != 0) {
    uint32_t lx = computeLux(ch0, ch1);
    _st.lux = static_cast<float>(lx) / (1 << LUX_SCALE);
  } else {
    _st.lux = NAN;
  }

  _st.lastReadyMs = nowMs();
  _st.dataReady = true;
  _st.measuring = false;
  power(false);
  _state = IDLE;
  return true;
}

bool TSL2561::readLux(float &outLux) {
  if (!_st.dataReady) return false;
  outLux = _st.lux;
  _st.dataReady = false; // mark as consumed
  return true;
}

uint32_t TSL2561::currentIntegrationWindowMs() const {
  // Add small margin for safe readout
  switch (_st.it) {
    case IT_13MS:  return 15;  // 13.7ms nominal
    case IT_101MS: return 105; // 101ms nominal
    case IT_402MS: return 405; // 402ms nominal
  }
  return 405;
}

void TSL2561::power(bool on) {
  write8(REG_CONTROL, on ? CONTROL_POWERON : CONTROL_POWEROFF);
}

bool TSL2561::write8(uint8_t reg, uint8_t val) {
  _wire->beginTransmission(_addr);
  _wire->write(CMD | reg);
  _wire->write(val);
  int e = _wire->endTransmission();
  _st.lastErr = e;
  return e == 0;
}

bool TSL2561::read8(uint8_t reg, uint8_t &val) {
  _wire->beginTransmission(_addr);
  _wire->write(CMD | reg);
  int e = _wire->endTransmission(false);
  if (e != 0) { _st.lastErr = e; return false; }
  if (_wire->requestFrom((int)_addr, 1) != 1) return false;
  val = _wire->read();
  return true;
}

bool TSL2561::read16(uint8_t reg, uint16_t &val) {
  _wire->beginTransmission(_addr);
  _wire->write(CMD | reg);
  int e = _wire->endTransmission(false);
  if (e != 0) { _st.lastErr = e; return false; }
  if (_wire->requestFrom((int)_addr, 2) != 2) return false;
  uint8_t lo = _wire->read();
  uint8_t hi = _wire->read();
  val = (uint16_t)hi << 8 | lo;
  return true;
}

uint32_t TSL2561::computeLux(uint16_t ch0, uint16_t ch1) const {
  // Scale channels for integration time and gain
  uint32_t chScale;
  switch (_st.it) {
    case IT_13MS:  chScale = CHSCALE_TINT0; break;
    case IT_101MS: chScale = CHSCALE_TINT1; break;
    default:       chScale = (1UL << CH_SCALE); break; // 402ms
  }
  if (_st.gain == GAIN_1X) {
    chScale <<= 4; // scale 1x to 16x
  }

  uint32_t ch0s = (ch0 * chScale) >> CH_SCALE;
  uint32_t ch1s = (ch1 * chScale) >> CH_SCALE;

  // Avoid divide by zero
  uint32_t ratio1 = (ch0s != 0) ? ((ch1s << (RATIO_SCALE + 1)) / ch0s) : 0;
  uint32_t ratio = (ratio1 + 1) >> 1; // round

  // Select coefficients by package and ratio
  uint16_t b = 0, m = 0;
  if (_st.pkg == PACKAGE_T) {
    if (ratio <= K1T) { b = B1T; m = M1T; }
    else if (ratio <= K2T) { b = B2T; m = M2T; }
    else if (ratio <= K3T) { b = B3T; m = M3T; }
    else if (ratio <= K4T) { b = B4T; m = M4T; }
    else if (ratio <= K5T) { b = B5T; m = M5T; }
    else if (ratio <= K6T) { b = B6T; m = M6T; }
    else if (ratio <= K7T) { b = B7T; m = M7T; }
    else { b = B8T; m = M8T; }
  } else {
    if (ratio <= K1C) { b = B1C; m = M1C; }
    else if (ratio <= K2C) { b = B2C; m = M2C; }
    else if (ratio <= K3C) { b = B3C; m = M3C; }
    else if (ratio <= K4C) { b = B4C; m = M4C; }
    else if (ratio <= K5C) { b = B5C; m = M5C; }
    else if (ratio <= K6C) { b = B6C; m = M6C; }
    else if (ratio <= K7C) { b = B7C; m = M7C; }
    else { b = B8C; m = M8C; }
  }

  int32_t temp = ((int32_t)ch0s * b) - ((int32_t)ch1s * m);
  if (temp < 0) temp = 0;
  // round and scale back to LUX_SCALE fixed-point
  temp += 1 << (LUX_SCALE - 1);
  return (uint32_t)temp; // still scaled by 2^LUX_SCALE
}