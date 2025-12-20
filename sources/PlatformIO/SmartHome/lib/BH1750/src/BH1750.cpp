#include "BH1750.h"

#define BH1750_CMD_POWER_DOWN 0x00
#define BH1750_CMD_POWER_ON   0x01
#define BH1750_CMD_RESET      0x07

BH1750::BH1750(TwoWire& w, uint8_t i2cAddr)
: _wire(&w), _addr(i2cAddr), _mode(ONE_HIGH_RES), _mtreg(69),
  _state(POWER_DOWN), _tstart(0), _wait_ms(0), _hasData(false),
  _raw(0), _lux(NAN), _err(0) {}

bool BH1750::begin(int sda, int scl, uint32_t freq) {
  if (sda >= 0 && scl >= 0) _wire->begin(sda, scl);
  else                      _wire->begin();
  _wire->setClock(freq);
  _state = POWER_DOWN;
  return powerOn(); // leave powered on, idle
}

void BH1750::setAddress(uint8_t addr) { _addr = addr; }
void BH1750::setMode(Mode m) { _mode = m; }

void BH1750::setMTReg(uint8_t mt) {
  if (mt < 31) mt = 31;
  if (mt > 254) mt = 254;
  _mtreg = mt;
  // Write MTreg high then low bits per datasheet
  uint8_t high = 0x40 | (_mtreg >> 5);   // 01000_MT[7:5]
  uint8_t low  = 0x60 | (_mtreg & 0x1F); // 011_MT[4:0]
  write8(high);
  write8(low);
}

bool BH1750::powerOn()  { bool ok = write8(BH1750_CMD_POWER_ON); if (ok) _state = IDLE; return ok; }
bool BH1750::powerDown(){ bool ok = write8(BH1750_CMD_POWER_DOWN); if (ok) _state = POWER_DOWN; return ok; }

bool BH1750::request() {
  if (!(_state == IDLE || _state == READY)) return false;
  _hasData = false;
  // ensure powered
  if (!write8(BH1750_CMD_POWER_ON)) { _state = ERROR; return false; }
  // start measurement
  if (!write8(static_cast<uint8_t>(_mode))) { _state = ERROR; return false; }
  // schedule read
  _wait_ms = modeBaseTimeMs(_mode);
  // scale by MTreg relative to default 69
  _wait_ms = (_wait_ms * _mtreg + 34) / 69; // round
  // margin (+10%)
  _wait_ms = _wait_ms + (_wait_ms / 10);
  _tstart = millis();
  _state = WAITING;
  return true;
}

void BH1750::poll() {
  if (_state != WAITING) return;
  if ((uint32_t)(millis() - _tstart) < _wait_ms) return;
  uint16_t r = 0;
  if (!readRaw(r)) { _state = ERROR; return; }
  _raw = r;
  // lux conversion per datasheet
  float divisor = convDivisorForMode(); // 1.2*(69/MTreg), adjust mode
  _lux = _raw / divisor;
  _hasData = true;
  _state = READY;
  // auto power down if one-time mode
  if (_mode == ONE_HIGH_RES || _mode == ONE_HIGH_RES2 || _mode == ONE_LOW_RES) {
    write8(BH1750_CMD_POWER_DOWN);
  }
}

bool BH1750::available() const { return _hasData; }

float BH1750::readLux() {
  _hasData = false;
  return _lux;
}

BH1750::Status BH1750::getStatus() const {
  return Status{_state,_addr,(uint8_t)_mode,_mtreg,_tstart,_wait_ms,_err,_raw,_lux,_hasData};
}

bool BH1750::write8(uint8_t cmd) {
  _wire->beginTransmission(_addr);
  _wire->write(cmd);
  _err = _wire->endTransmission();
  return _err == 0;
}

bool BH1750::readRaw(uint16_t& raw) {
  uint8_t n = _wire->requestFrom((int)_addr, 2);
  if (n != 2) { _err = -2; return false; }
  uint8_t msb = _wire->read();
  uint8_t lsb = _wire->read();
  raw = ((uint16_t)msb << 8) | lsb;
  return true;
}

uint32_t BH1750::modeBaseTimeMs(Mode m) const {
  switch (m) {
    case CONT_LOW_RES:
    case ONE_LOW_RES:   return 16;  // typ 16 ms, max ~24 ms
    case CONT_HIGH_RES2:
    case ONE_HIGH_RES2:
    case CONT_HIGH_RES:
    case ONE_HIGH_RES:
    default:            return 120; // typ 120 ms, max ~180 ms first sample
  }
}

float BH1750::convDivisorForMode() const {
  // base divisor from datasheet: H-res: 1.2*(69/MTreg)
  float base = 1.2f * (69.0f / (float)_mtreg);
  switch (_mode) {
    case CONT_HIGH_RES2:
    case ONE_HIGH_RES2: return base * 2.0f; // divide lux by 2 => multiply divisor by 2
    case CONT_LOW_RES:
    case ONE_LOW_RES:   return base / 4.0f; // 4 lx/count => lux = raw * 4 / base => divisor /=4
    default:            return base;
  }
}
