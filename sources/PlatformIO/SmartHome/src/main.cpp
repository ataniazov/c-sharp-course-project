// ================================================================================

#include <Arduino.h>

#include <WiFi.h>
#include <PsychicMqttClient.h>

#include <ArduinoJson.h>

#include "config.h"
#include "photometry.h"

#include <RainDrop.h>
#include <WindowServo.h>

#include <RelayModule.h>
#include <DS18B20.h>
#include <FanPWM.h>

#include <TSL2561.h>
#include <LEDPWM.h>

// =============================== Rain Drop Sensor ===============================

RainDrop rain(RAINDROP_SENSOR_AO_PIN, RAINDROP_SENSOR_DO_PIN);

void rain_sensor_setup()
{
    // Calibrate once your board is assembled: read raw when dry and when wet, then update.
    //rain.begin(/*dryCal=*/3700, /*wetCal=*/500, /*samplePeriodMs=*/100);
    rain.begin(/*dryCal=*/3700, /*wetCal=*/700, /*samplePeriodMs=*/100);
}

// ================================= Window Servo =================================

WindowServo window_servo;
WindowServo::Speed window_servo_speed = WindowServo::Speed::FAST;
WindowServo::State window_servo_state = WindowServo::State::CLOSED;
static bool window_state_changed = false;

void window_servo_setup()
{
    if (!window_servo.begin(SERVO_PIN, SERVO_LEDC_CH))
    {
        while (1)
        {
            Serial.println("WindowServo: LEDC init failed");
            delay(1000);
        }
    }
    window_servo.setAngles(0, 90);
    window_servo_speed = WindowServo::Speed::FAST;
    window_servo_state = WindowServo::State::CLOSED;
    window_state_changed = true;
}

void window_servo_update()
{
    if (window_state_changed)
    {
        window_servo.setSpeed(window_servo_speed);
        window_servo.state(window_servo_state);
        window_state_changed = false;
    }
}

// Case-insensitive compare without allocating
static bool ieq(const char *a, const char *b)
{
    if (!a || !b)
    {
        return false;
    }

    while (*a && *b)
    {
        char ca = ((*a >= 'A' && *a <= 'Z') ? char(*a + 32) : *a);
        char cb = ((*b >= 'A' && *b <= 'Z') ? char(*b + 32) : *b);
        if (ca != cb)
        {
            return false;
        }
        ++a;
        ++b;
    }
    return ((*a == 0) && (*b == 0));
}

static bool mapWindowServoSpeed(const char *s, WindowServo::Speed &out)
{
    if (ieq(s, "slow") || ieq(s, "low"))
    {
        out = WindowServo::Speed::SLOW;
        return true;
    }

    if (ieq(s, "med") || ieq(s, "mid") || ieq(s, "medium"))
    {
        out = WindowServo::Speed::MEDIUM;
        return true;
    }

    if (ieq(s, "fast") || ieq(s, "high"))
    {
        out = WindowServo::Speed::FAST;
        return true;
    }

    return false;
}

static bool mapWindowServoState(const char *s, WindowServo::State &out)
{
    if (ieq(s, "open"))
    {
        out = WindowServo::State::OPEN;
        return true;
    }

    if (ieq(s, "close") || ieq(s, "closed"))
    {
        out = WindowServo::State::CLOSED;
        return true;
    }

    if (ieq(s, "idle") || ieq(s, "stop"))
    {
        out = WindowServo::State::IDLE;
        return true;
    }

    return false;
}

// Optional wrapper if your MQTT callback doesn't give payload length
static inline void onWindowTopic_len(const char *topic,
                                     const char *payload, size_t payload_len,
                                     int retain, int qos, bool dup);

void onWindowTopic(const char *topic, const char *payload, int retain, int qos, bool dup)
{
    // Fallback if your MQTT client doesn't expose 'length'
    size_t len = payload ? strlen(payload) : 0;
    onWindowTopic_len(topic, payload, len, retain, qos, dup);
}

static inline void onWindowTopic_len(const char *topic,
                                     const char *payload, size_t payload_len,
                                     int /*retain*/, int /*qos*/, bool /*dup*/)
{
    Serial.printf("%s: %.*s\r\n", topic, (int)payload_len, payload ? payload : "(null)");
    if (!payload || payload_len == 0)
    {
        Serial.println("WindowTopic invalid payload: null/empty");
        return;
    }

    DeserializationError err;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonDocument doc; // v7 dynamic
#else
    StaticJsonDocument<256> doc; // v6 fixed-capacity is fine for two short strings
#endif

    // Always pass the payload length to be robust with MQTT buffers
    err = deserializeJson(doc, payload, payload_len);
    if (err)
    {
        Serial.print("WindowTopic JSON parse error: ");
        Serial.println(err.c_str());
        return;
    }

    // Extract strings safely across v6/v7
    const char *speedRaw = nullptr;
    const char *stateRaw = nullptr;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    JsonString sp = obj["speed"];
    JsonString st = obj["state"];
    speedRaw = sp.c_str();
    stateRaw = st.c_str();
#else
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    speedRaw = obj["speed"].as<const char *>();
    stateRaw = obj["state"].as<const char *>();
#endif

    if (!speedRaw || !stateRaw)
    {
        // Dump what was actually parsed to help debugging
        Serial.print("WindowTopic Parsed doc: ");
        serializeJson(doc, Serial);
        Serial.println();
        Serial.println("Missing 'speed' or 'state'");
        return;
    }

    WindowServo::Speed spd;
    WindowServo::State stt;
    if (!mapWindowServoSpeed(speedRaw, spd))
    {
        Serial.println("WindowTopic: Invalid speed");
        return;
    }
    if (!mapWindowServoState(stateRaw, stt))
    {
        Serial.println("WindowTopic: Invalid state");
        return;
    }

    window_servo_speed = spd;
    window_servo_state = stt;
    window_state_changed = true;
}

// ================================= Relay Module =================================

RelayModule relay_module;
static const uint8_t RELAY_MODULE_CH = 0;
static const uint8_t RELAY_MODULE_PINS[] = {RELAY_MODULE_PIN};
static bool relay_module_state_changed = false;

enum RelayModuleState
{
    RELAY_MODULE_OFF = 0,
    RELAY_MODULE_ON = 1
};

RelayModuleState relay_module_state = RELAY_MODULE_OFF;

void relay_module_setup()
{
    relay_module.begin(RELAY_MODULE_PINS, sizeof(RELAY_MODULE_PINS), RelayModule::ActiveLow, /*initOff=*/true);
    relay_module_state = RELAY_MODULE_OFF;
    relay_module_state_changed = true;
}

void relay_module_update()
{
    if (relay_module_state_changed)
    {
        if (relay_module_state == RELAY_MODULE_ON)
        {
            relay_module.on(RELAY_MODULE_CH);
        }
        else if (relay_module_state == RELAY_MODULE_OFF)
        {
            relay_module.off(RELAY_MODULE_CH);
        }
    }

    relay_module_state_changed = false;
}

static bool mapRelayModuleState(const char *s, RelayModuleState &out)
{
    if (ieq(s, "on"))
    {
        out = RELAY_MODULE_ON;
        return true;
    }

    if (ieq(s, "off"))
    {
        out = RELAY_MODULE_OFF;
        return true;
    }

    return false;
}

// Optional wrapper if your MQTT callback doesn't give payload length
static inline void onRelayTopic_len(const char *topic,
                                    const char *payload, size_t payload_len,
                                    int retain, int qos, bool dup);

void onRelayTopic(const char *topic, const char *payload, int retain, int qos, bool dup)
{
    // Fallback if your MQTT client doesn't expose 'length'
    size_t len = payload ? strlen(payload) : 0;
    onRelayTopic_len(topic, payload, len, retain, qos, dup);
}

static inline void onRelayTopic_len(const char *topic,
                                    const char *payload, size_t payload_len,
                                    int /*retain*/, int /*qos*/, bool /*dup*/)
{
    Serial.printf("%s: %.*s\r\n", topic, (int)payload_len, payload ? payload : "(null)");
    if (!payload || payload_len == 0)
    {
        Serial.println("RelayTopic invalid payload: null/empty");
        return;
    }

    DeserializationError err;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonDocument doc; // v7 dynamic
#else
    StaticJsonDocument<256> doc; // v6 fixed-capacity is fine for two short strings
#endif

    // Always pass the payload length to be robust with MQTT buffers
    err = deserializeJson(doc, payload, payload_len);
    if (err)
    {
        Serial.print("RelayTopic JSON parse error: ");
        Serial.println(err.c_str());
        return;
    }

    // Extract strings safely across v6/v7
    const char *StateRaw = nullptr;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    JsonString rs = obj["state"];
    StateRaw = rs.c_str();
#else
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    StateRaw = obj["state"].as<const char *>();
#endif

    if (!StateRaw)
    {
        // Dump what was actually parsed to help debugging
        Serial.print("RelayTopic Parsed doc: ");
        serializeJson(doc, Serial);
        Serial.println();
        Serial.println("Missing 'state'");
        return;
    }

    if (!mapRelayModuleState(StateRaw, relay_module_state))
    {
        Serial.println("RelayTopic: Invalid state");
        return;
    }

    relay_module_state_changed = true;
}

// ============================== Temperature Sensor ==============================

DS18B20 temp(/*pin=*/TEMPERATURE_SENSOR_PIN, /*bits=*/11, /*ms=*/375);

void temp_sensor_setup()
{
    if (!temp.begin())
    {
        Serial.println("DS18B20 init failed.");
    }
    // else
    // {
    //     Serial.println("DS18B20 init OK.");
    // }
}

// ==================================== FanPWM ====================================

FanPWM fan(FAN_PWM_PIN, 25000, 10, FAN_PWM_LEDC_CH); // 25 kHz, 10-bit resolution
FanSpeed fan_speed = FanSpeed::OFF;
static bool fan_speed_changed = false;

void fan_setup()
{
    if (!fan.begin())
    {
        while (1)
        {
            Serial.println("Fan: LEDC init failed");
            delay(1000);
        }
    }

    fan_speed = FanSpeed::OFF;
    fan_speed_changed = true;
}

void fan_update()
{
    if (fan_speed_changed)
    {
        fan.setSpeed(fan_speed);
        fan_speed_changed = false;
    }
}

static bool mapFanSpeed(const char *s, FanSpeed &out)
{
    if (ieq(s, "off") || ieq(s, "stop"))
    {
        out = FanSpeed::OFF;
        return true;
    }

    if (ieq(s, "slow") || ieq(s, "low"))
    {
        out = FanSpeed::SLOW;
        return true;
    }

    if (ieq(s, "medium") || ieq(s, "med") || ieq(s, "mid"))
    {
        out = FanSpeed::MEDIUM;
        return true;
    }

    if (ieq(s, "fast") || ieq(s, "high"))
    {
        out = FanSpeed::FAST;
        return true;
    }

    return false;
}

// Optional wrapper if your MQTT callback doesn't give payload length
static inline void onFanTopic_len(const char *topic,
                                  const char *payload, size_t payload_len,
                                  int retain, int qos, bool dup);

void onFanTopic(const char *topic, const char *payload, int retain, int qos, bool dup)
{
    // Fallback if your MQTT client doesn't expose 'length'
    size_t len = payload ? strlen(payload) : 0;
    onFanTopic_len(topic, payload, len, retain, qos, dup);
}

static inline void onFanTopic_len(const char *topic,
                                  const char *payload, size_t payload_len,
                                  int /*retain*/, int /*qos*/, bool /*dup*/)
{
    Serial.printf("%s: %.*s\r\n", topic, (int)payload_len, payload ? payload : "(null)");
    if (!payload || payload_len == 0)
    {
        Serial.println("FanTopic invalid payload: null/empty");
        return;
    }

    DeserializationError err;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonDocument doc; // v7 dynamic
#else
    StaticJsonDocument<256> doc; // v6 fixed-capacity is fine for two short strings
#endif

    // Always pass the payload length to be robust with MQTT buffers
    err = deserializeJson(doc, payload, payload_len);
    if (err)
    {
        Serial.print("FanTopic JSON parse error: ");
        Serial.println(err.c_str());
        return;
    }

    // Extract strings safely across v6/v7
    const char *speedRaw = nullptr;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    JsonString sp = obj["speed"];
    speedRaw = sp.c_str();
#else
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    speedRaw = obj["speed"].as<const char *>();
#endif

    if (!speedRaw)
    {
        // Dump what was actually parsed to help debugging
        Serial.print("FanTopic Parsed doc: ");
        serializeJson(doc, Serial);
        Serial.println();
        Serial.println("Missing 'speed'");
        return;
    }

    FanSpeed spd;
    if (!mapFanSpeed(speedRaw, spd))
    {
        Serial.println("FanTopic: Invalid speed");
        return;
    }

    fan_speed = spd;
    fan_speed_changed = true;
}

// ==================================== LEDPWM ====================================

LEDPWM led(POWER_LED_PWM_PIN, 5000, 8, POWER_LED_PWM_LEDC_CH); // 5 kHz, 8-bit, channel 2 (v3 auto-assigns if channel=-1)

LEDPWM::Level led_level = LEDPWM::Level::Off;
static bool led_level_changed = false;

void led_setup()
{
    if (!led.begin())
    {
        while (1)
        {
            Serial.println("Power LED: LEDC init failed");
            delay(1000);
        }
    }

    led_level = LEDPWM::Level::Off;
    led_level_changed = true;
}

void led_update()
{
    if (led_level_changed)
    {
        led.set(led_level);
        led_level_changed = false;
    }
}

static bool mapLedLevel(const char *s, LEDPWM::Level &out)
{
    if (ieq(s, "off") || ieq(s, "stop"))
    {
        out = LEDPWM::Level::Off;
        return true;
    }

    if (ieq(s, "low"))
    {
        out = LEDPWM::Level::Low;
        return true;
    }

    if (ieq(s, "medium") || ieq(s, "med") || ieq(s, "mid"))
    {
        out = LEDPWM::Level::Medium;
        return true;
    }

    if (ieq(s, "high"))
    {
        out = LEDPWM::Level::High;
        return true;
    }

    return false;
}

// Optional wrapper if your MQTT callback doesn't give payload length
static inline void onLedTopic_len(const char *topic,
                                  const char *payload, size_t payload_len,
                                  int retain, int qos, bool dup);

void onLedTopic(const char *topic, const char *payload, int retain, int qos, bool dup)
{
    // Fallback if your MQTT client doesn't expose 'length'
    size_t len = payload ? strlen(payload) : 0;
    onLedTopic_len(topic, payload, len, retain, qos, dup);
}

static inline void onLedTopic_len(const char *topic,
                                  const char *payload, size_t payload_len,
                                  int /*retain*/, int /*qos*/, bool /*dup*/)
{
    Serial.printf("%s: %.*s\r\n", topic, (int)payload_len, payload ? payload : "(null)");
    if (!payload || payload_len == 0)
    {
        Serial.println("LedTopic invalid payload: null/empty");
        return;
    }

    DeserializationError err;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonDocument doc; // v7 dynamic
#else
    StaticJsonDocument<256> doc; // v6 fixed-capacity is fine for two short strings
#endif

    // Always pass the payload length to be robust with MQTT buffers
    err = deserializeJson(doc, payload, payload_len);
    if (err)
    {
        Serial.print("LedTopic JSON parse error: ");
        Serial.println(err.c_str());
        return;
    }

    // Extract strings safely across v6/v7
    const char *levelRaw = nullptr;
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    JsonString sp = obj["level"];
    levelRaw = sp.c_str();
#else
    JsonObjectConst obj = doc.as<JsonObjectConst>();
    levelRaw = obj["level"].as<const char *>();
#endif

    if (!levelRaw)
    {
        // Dump what was actually parsed to help debugging
        Serial.print("LedTopic Parsed doc: ");
        serializeJson(doc, Serial);
        Serial.println();
        Serial.println("Missing 'level'");
        return;
    }

    LEDPWM::Level lvl;
    if (!mapLedLevel(levelRaw, lvl))
    {
        Serial.println("LedTopic: Invalid level");
        return;
    }

    led_level = lvl;
    led_level_changed = true;
}

// ================================= Light sensor =================================

TSL2561 light(0x39); // Addr: 0x29, 0x39 (default), or 0x49

static float light_sensor_lux = 0.0f;

void light_sensor_setup()
{
    Wire.begin(LIGHT_SENSOR_I2C_SDA_PIN, LIGHT_SENSOR_I2C_SCL_PIN);

    if (!light.begin(Wire, TSL2561::PACKAGE_T))
    {
        Serial.println("TSL2561 init failed");
    }

    light.configure(TSL2561::IT_101MS, TSL2561::GAIN_1X);
    light.request();
}

void light_sensor_update()
{
    static uint8_t sensor_restart_count = 0;
    static uint8_t data_read_fail_count = 0;
    static bool sensor_failed = false;

    if (!sensor_failed)
    {
        bool isReady = false;

        // Drive the measurement state machine
        if (light.poll())
        {
            if (light.readLux(light_sensor_lux))
            {
                // Immediately queue next reading without blocking
                light.request();
                isReady = true;
            }
        }

        if (!isReady)
        {
            data_read_fail_count++;

            if (data_read_fail_count > 5)
            {
                light_sensor_lux = 0.0f;
                data_read_fail_count = 0;

                if (sensor_restart_count > 10)
                {
                    Serial.println("Light sensor failed repeatedly. Disabling further attempts.");
                    sensor_failed = true;
                }

                light_sensor_setup(); // Re-init after repeated failures
                sensor_restart_count++;
            }
        }
        else
        {
            sensor_restart_count = 0;
            data_read_fail_count = 0;
        }
    }
}

// ================================= MQTT Client ==================================

/**
 * Create a PsychicMqttClient object
 */
PsychicMqttClient mqttClient;

String topicBase = String(MQTT_BASE_TOPIC) + "/" + String(MQTT_CLIENT_ID);
String topicStatus = topicBase + "/status";
String topicState = topicBase + "/state";

// String topicWeather = topicBase + "/weather";
String topicWindow = topicBase + "/window";

String topicHeater = topicBase + "/heater";
// String topicTemp = topicBase + "/temp";
String topicFan = topicBase + "/fan";

// String topicLight = topicBase + "/light";
String topicLamp = topicBase + "/lamp";

void mqttPublishMessage(const char *topic, const char *payload, int qos = 1, bool retain = false, bool async = true)
{
    int packetIdPub = mqttClient.publish(topic, qos, retain, payload, strlen(payload), async);

    if (packetIdPub < 0)
    {
        Serial.printf("Error: Failed to publish message to topic \"%s\"\r\n", topic);
    }
    // else
    // {
    //     Serial.printf("Publishing message to topic \"%s\" at QoS %d, packetId: %d\r\n", topic, qos, packetIdPub);
    // }
}

// void mqttSubscribeTopic(const char *topic, int qos = 2)
// {
//     int packetIdSub = mqttClient.subscribe(topic, qos);

//     if (packetIdSub == -1)
//     {
//         Serial.printf("Error: Failed to subscribe to topic \"%s\"\r\n", topic);
//     }
//     else
//     {
//         Serial.printf("Subscribing to topic \"%s\" at QoS %d, packetId: %d\r\n", topic, qos, packetIdSub);
//     }
// }

void onMqttConnect(bool sessionPresent)
{
    Serial.println("Connected to MQTT Broker.");
    // Serial.printf("Session present: %d\r\n", sessionPresent);

    // Publish "online" to the status topic
    mqttPublishMessage(topicStatus.c_str(), "{\"status\":\"online\"}", 1, true);

    // mqttSubscribeTopic(topicWeather.c_str(), 2);
    // mqttSubscribeTopic(topicWindow.c_str(), 2);
    // mqttSubscribeTopic(topicHeater.c_str(), 2);
    // mqttSubscribeTopic(topicTemp.c_str(), 2);
    // mqttSubscribeTopic(topicFan.c_str(), 2);
    // mqttSubscribeTopic(topicLumen.c_str(), 2);
    // mqttSubscribeTopic(topicLamp.c_str(), 2);
}

// void onMqttDisconnect(bool sessionPresent)
// {
//     Serial.println("Disconnected from MQTT.");
// }

// void onMqttSubscribe(uint16_t packetId)
// {
//     Serial.println("Subscribe acknowledged.");
//     Serial.printf("  packetId: %d\r\n", packetId);
// }

// void onMqttUnsubscribe(uint16_t packetId)
// {
//     Serial.println("Unsubscribe acknowledged.");
//     Serial.printf("  packetId: %d\r\n", packetId);
// }

// void onMqttMessage(char *topic, char *payload, int retain, int qos, bool dup)
// {
//     Serial.println("Message received.");
//     Serial.printf("  topic: %s\r\n", topic);
//     Serial.printf("  qos: %d\r\n", qos);
//     Serial.printf("  dup: %d\r\n", dup);
//     Serial.printf("  retain: %d\r\n", retain);
// }

// void onMqttPublish(uint16_t packetId)
// {
//     Serial.println("Publish acknowledged.");
//     Serial.printf("  packetId: %d\r\n", packetId);
// }

// ==================================== Setup =====================================

void setup()
{
    Serial.begin(SERIAL_BAUDRATE);

    WiFi.mode(WIFI_OFF);
    WiFi.setHostname(MQTT_CLIENT_ID);
    WiFi.mode(WIFI_STA);
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

    Serial.printf("Connecting to WiFi %s .", WIFI_SSID);
    while (WiFi.status() != WL_CONNECTED)
    {
        Serial.print(".");
        delay(WIFI_CONNECT_DELAY);
    }

    Serial.println("\r\nConnected to WiFi.");
    Serial.printf("  IP address: %s \r\n", WiFi.localIP().toString().c_str());
    Serial.printf("  Gateway: %s \r\n", WiFi.gatewayIP().toString().c_str());

    String mqttBrokerIP = WiFi.gatewayIP().toString();
    String mqttBroker = String("mqtt://") + mqttBrokerIP + ":1883";
    Serial.printf("MQTT Broker: %s\r\n", mqttBroker.c_str());

    mqttClient.setServer(mqttBroker.c_str());
    mqttClient.setClientId(MQTT_CLIENT_ID);
    mqttClient.setKeepAlive(5);

    mqttClient.onConnect(onMqttConnect);
    // mqttClient.onDisconnect(onMqttDisconnect);
    // mqttClient.onSubscribe(onMqttSubscribe);
    // mqttClient.onUnsubscribe(onMqttUnsubscribe);
    // mqttClient.onMessage(onMqttMessage);
    // mqttClient.onPublish(onMqttPublish);

    // /**
    //  * Using a lambda callback function is a very convenient way to handle the
    //  * received message. The function will be called when a message is received.
    //  *
    //  * The parameters are:
    //  * - topic: The topic of the received message
    //  * - payload: The payload of the received message
    //  * - retain: The retain flag of the received message
    //  * - qos: The qos of the received message
    //  * - dup: The duplicate flag of the received message
    //  *
    //  * It is important not to do any heavy calculations, hardware access, delays or
    //  * blocking code in the callback function.
    //  */
    // mqttClient.onTopic(simpleTopic.c_str(), 0, [&](const char *topic, const char *payload, int retain, int qos, bool dup)
    //                    {
    //     Serial.printf("Received Topic: %s\r\n", topic);
    //     Serial.printf("Received Payload: %s\r\n", payload); });

    // /**
    //  * Using a dedicated callback function is a good way to handle the received message.
    //  */
    // mqttClient.onTopic(longTopic.c_str(), 0, onLongTopic);

    mqttClient.onTopic(topicWindow.c_str(), 1, onWindowTopic);
    mqttClient.onTopic(topicHeater.c_str(), 1, onRelayTopic);
    mqttClient.onTopic(topicFan.c_str(), 1, onFanTopic);
    mqttClient.onTopic(topicLamp.c_str(), 1, onLedTopic);

    mqttClient.setWill(topicStatus.c_str(), 1, true, "{\"status\":\"offline\"}");
    mqttClient.connect();

    Serial.printf("Connecting to MQTT Broker %s .", mqttBroker.c_str());
    while (!mqttClient.connected())
    {
        Serial.print(".");
        delay(MQTT_CONNECT_DELAY);
    }

    Serial.println();

    rain_sensor_setup();
    temp_sensor_setup();
    light_sensor_setup();

    window_servo_setup();
    relay_module_setup();
    fan_setup();
    led_setup();
}

// ======================== Telemetry and State Reporting =========================

static const char *rainToStr(RainDrop::Status s)
{
    switch (s)
    {
    case RainDrop::Status::Dry:
        return "dry";
    case RainDrop::Status::Damp:
        return "damp";
    case RainDrop::Status::Wet:
        return "wet";
    default:
        return "unknown";
    }
}

static const char *windowSpeedToStr(WindowServo::Speed s)
{
    switch (s)
    {
    case WindowServo::Speed::SLOW:
        return "slow";
    case WindowServo::Speed::MEDIUM:
        return "medium";
    case WindowServo::Speed::FAST:
        return "fast";
    default:
        return "unknown";
    }
}

static const char *windowStateToStr(WindowServo::State s)
{
    switch (s)
    {
    case WindowServo::State::CLOSED:
        return "closed";
    case WindowServo::State::OPEN:
        return "open";
    case WindowServo::State::OPENING:
        return "opening";
    case WindowServo::State::CLOSING:
        return "closing";
    case WindowServo::State::IDLE:
        return "idle";
    default:
        return "unknown";
    }
}

static const char *fanSpeedToStr(FanSpeed s)
{
    switch (s)
    {
    case FanSpeed::OFF:
        return "off";
    case FanSpeed::SLOW:
        return "slow";
    case FanSpeed::MEDIUM:
        return "medium";
    case FanSpeed::FAST:
        return "fast";
    default:
        return "unknown";
    }
}

static const char *ledLevelToStr(LEDPWM::Level l)
{
    switch (l)
    {
    case LEDPWM::Level::Off:
        return "off";
    case LEDPWM::Level::Low:
        return "low";
    case LEDPWM::Level::Medium:
        return "medium";
    case LEDPWM::Level::High:
        return "high";
    default:
        return "unknown";
    }
}

// float roundf3(float v)
// {
//     return roundf(v * 1000.0f) / 1000.0f;
// }

static const char *roundf3Str(float val, char *buf, size_t bufsize)
{
    snprintf(buf, bufsize, "%.3f", val);
    return buf;
}

String State()
{
    char buf[12];

    // Cross-version document type
    // v7: JsonDocument has dynamic capacity
    // v6: choose a capacity (256 is ample for 7 short string fields)
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonDocument doc; // v7
#else
    StaticJsonDocument<256> doc; // v6 capacity is sufficient here
#endif

    // Top-level fields
    doc["weather"] = rainToStr(rain.status());

    // Nested "window" object
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonObject window_json = doc["window"].to<JsonObject>(); // ensure object
#else
    JsonObject window_json = doc.createNestedObject("window"); // v6 helper
#endif
    // Use your actual getter or a global variable if different
    window_json["speed"] = windowSpeedToStr(window_servo.speed());
    window_json["state"] = windowStateToStr(window_servo.status());

    doc["heater"] = ((relay_module.isOn(RELAY_MODULE_CH)) ? "on" : "off");

    if (temp.available())
    {
        // Keep numeric by default; encoder may omit trailing zeros by design
        // doc["temp"] = (roundf(temp.readC() * 100.0f) / 100.0f);

        // If you require exactly three decimals in the output text, use serialized():
        doc["temp"] = roundf3Str(temp.readC(), buf, sizeof(buf)); // emits 23.000 without quotes
    }
    else
    {
        // Temperature: null when unavailable
        //doc["temp"] = nullptr;
        doc["temp"] = "0.000";
    }

    doc["fan"] = fanSpeedToStr(fan.speed());

    // Nested "light" object
#if ARDUINOJSON_VERSION_MAJOR >= 7
    JsonObject light_json = doc["light"].to<JsonObject>(); // ensure object
#else
    JsonObject light_json = doc.createNestedObject("light"); // v6 helper
#endif
    // Use your actual getter or a global variable if different
    const float length_m = 0.30;              // 30 cm
    const float width_m = 0.20;               // 20 cm
    const float area_m2 = length_m * width_m; // 30 cm × 20 cm

    const float distance_m = 0.20;      // 20 cm from sensor to emitter
    const float beam_angle_deg = 180.0; // full hemisphere

    light_json["lux"] = roundf3Str(light_sensor_lux, buf, sizeof(buf));
    light_json["lumen_surf"] = roundf3Str(luxToLumens(light_sensor_lux, area_m2, EmitterModel::SURFACE), buf, sizeof(buf));
    light_json["lumen_iso"] = roundf3Str(luxToLumens(light_sensor_lux, distance_m, EmitterModel::ISOTROPIC), buf, sizeof(buf));
    light_json["lumen_lamb"] = roundf3Str(luxToLumens(light_sensor_lux, distance_m, EmitterModel::LAMBERTIAN), buf, sizeof(buf));
    light_json["lumen_angle"] = roundf3Str(luxToLumens(light_sensor_lux, distance_m, EmitterModel::UNIFORM_CONE, beam_angle_deg), buf, sizeof(buf));

    // If you measured that your cavity reflections boost lux by ~1.3×, compensate:
    // double lumens_lamb_corr = luxToLumens(lux, distance_m, EmitterModel::LAMBERTIAN, 0.0, 1.3);

    doc["lamp"] = ledLevelToStr(led.level());

    // Serialize to String, portable across v6 and v7
    String out;              // empty => safe for v6 "append" and v7 "replace"
    serializeJson(doc, out); // produces minified JSON
    return out;
}

// ================================== Main Loop ===================================

void loop()
{
    static uint32_t last_sensor_fast_update = 0;
    static uint32_t last_sensor_slow_update = 0;
    static uint32_t last_mqtt_publish = 0;

    const uint32_t now = millis();

    if (((uint32_t)(now - last_sensor_fast_update)) >= SENSOR_FAST_UPDATE_INTERVAL_MS)
    {
        last_sensor_fast_update = now;

        rain.update();
        light_sensor_update();
    }

    if (((uint32_t)(now - last_sensor_slow_update)) >= SENSOR_SLOW_UPDATE_INTERVAL_MS)
    {
        last_sensor_slow_update = now;

        temp.poll();
    }

    if (((uint32_t)(now - last_mqtt_publish)) >= MQTT_PUBLISH_INTERVAL_MS)
    {
        last_mqtt_publish = now;

        mqttPublishMessage(topicState.c_str(), State().c_str(), 0, false);
    }

    window_servo_update();
    relay_module_update();
    fan_update();
    led_update();
}

// ================================================================================