#include <WiFi.h>
#include <PubSubClient.h>

// WiFi налаштування
const char* ssid = "Wokwi-GUEST";
const char* password = "";

// MQTT налаштування
const char* mqtt_server = "broker.emqx.io";  // Публічний MQTT брокер
const int mqtt_port = 1883;  // Порт для небезпечного MQTT (без SSL)
const char* mqtt_user = "";  
const char* mqtt_password = "";

WiFiClient espClient;
PubSubClient client(espClient);

bool dataSent = false;  // Початково - дані не відправлені
// Функція для підключення до WiFi
void setup_wifi() {
  delay(10);
  Serial.println();
  Serial.print("Підключення до ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi підключено!");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}
// Функція для підключення до MQTT брокера
void reconnect() {
  while (!client.connected()) {
    Serial.print("Підключення до MQTT...");
    if (client.connect("ESP32Client", mqtt_user, mqtt_password)) {
      Serial.println("підключено!");
    } else {
      Serial.print("помилка, rc=");
      Serial.print(client.state());
      Serial.println(" спроба підключення через 5 секунд");
      delay(5000);
    }
  }
}

int generateRandomWeight() {
  return random(50, 120); 
}
// Відправка даних
void sendData() {
  if (dataSent) {
    Serial.println("Дані вже були відправлені. Припиняємо відправку.");
    return;  // Якщо дані вже відправлені, припиняємо відправку
  }

  // Генерація випадкової ваги
  int weight = generateRandomWeight();
  
  // Створення JSON-формату
  String jsonBody = "{"
                    "\"weight\": " + String(weight) + ","
                    "\"measuredAt\": \"" + getCurrentTime() + "\","
                    "\"deviceId\": \"ESP32Device\","
                    "\"isSynced\": true"
                    "}";

  Serial.println("Відправка даних...");
  Serial.println(jsonBody);

  // Публікація даних на тему "weight/measurements"
  if (client.publish("weight/measurements", jsonBody.c_str())) {
    Serial.println("Дані успішно відправлені!");
    dataSent = true;  // Позначаємо, що дані були відправлені
    stopProgram();  // Зупинка програми після успішної відправки
  } else {
    Serial.println("Помилка відправки даних!");
  }
}
// Отримання поточного часу у форматі ISO 8601
String getCurrentTime() {
  unsigned long currentMillis = millis();
  char timeBuffer[30];
  snprintf(timeBuffer, sizeof(timeBuffer), "%04d-%02d-%02dT%02d:%02d:%02d.%03dZ", 
           2024, 12, 9, 15, 8, 19, currentMillis);
  return String(timeBuffer);
}

// Функція для зупинки програми
void stopProgram() {
  Serial.println("Програма зупинена.");
  while (true) {
    // Завершуємо виконання програми, зупиняючи всі цикли
    delay(1000);  // Безкінечно зупиняємо програму
  }
}
void setup() {
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);
}
void loop() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop();
  // Відправка даних лише один раз
  if (!dataSent) {  // Якщо дані ще не були відправлені
    static unsigned long lastMsg = 0;
    unsigned long now = millis();
    if (now - lastMsg > 10000) {  // 10 секунд
      lastMsg = now;
      sendData();
    }
  }
}
// Callback для обробки отриманих повідомлень
void callback(char* topic, byte* payload, unsigned int length) {
  String payloadString = "";
  for (int i = 0; i < length; i++) {
    payloadString += (char)payload[i];
  }
  Serial.println("Отримані дані: " + payloadString);
}
