// ============= ESP32-C3-32S =============
// ============= BUILT-IN LED =============
#define BUILT_IN_LED_RED_PIN 3
#define BUILT_IN_LED_GREEN_PIN 4
#define BUILT_IN_LED_BLUE_PIN 5
#define BUILT_IN_LED_WARM_PIN 18
#define BUILT_IN_LED_COLD_PIN 19
// ============= BUILT-IN BUTTON ==========
#define BUILT_IN_BUTTON_PIN 9
#define BUILT_IN_BUTTON_ACTIVE LOW
// ================ SERIAL ================
#define SERIAL_BAUDRATE 115200
// ================ Wi-Fi =================
// your network SSID (name)
#define WIFI_SSID "SmartHome"
// your network password
#define WIFI_PASSWORD "SmartHome"
#define WIFI_CONNECT_DELAY 500
// ================= MQTT =================
#define MQTT_BASE_TOPIC "SmartHome"
#define MQTT_CLIENT_ID "esp32c3-32s"
#define MQTT_CONNECT_DELAY 500
// =========== RAINDROP SENSOR ============
#define RAINDROP_SENSOR_AO_PIN 1
#define RAINDROP_SENSOR_DO_PIN -1
// ================ SERVO =================
#define SERVO_PIN 4
#define SERVO_LEDC_CH 1
// ============= RELAY MODULE =============
#define RELAY_MODULE_PIN 5
// ========== TEMPERATURE SENSOR ==========
#define TEMPERATURE_SENSOR_PIN 18
// =============== FAN PWM ================
#define FAN_PWM_PIN 10
#define FAN_PWM_LEDC_CH 2
// ============ POWER LED PWM =============
#define POWER_LED_PWM_PIN 3
#define POWER_LED_PWM_LEDC_CH 3
// ============= LIGHT SENSOR =============
#define LIGHT_SENSOR_I2C_SCL_PIN 6
#define LIGHT_SENSOR_I2C_SDA_PIN 7
// ================ TIMING ================
#define SENSOR_FAST_UPDATE_INTERVAL_MS 100
#define SENSOR_SLOW_UPDATE_INTERVAL_MS 500
#define MQTT_PUBLISH_INTERVAL_MS 1000
// ========================================