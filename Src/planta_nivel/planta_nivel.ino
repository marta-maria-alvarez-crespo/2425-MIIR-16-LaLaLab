#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include "env.h"

// WiFi credenciales
const char WIFI_SSID[] = "";
const char WIFI_PASSWORD[] = "";

// MQTT broker
const char MQTT_HOST[] = "";
const int PORT = 0000;

// MQTT topics
const char PUBLISH_TOPIC[] = "optimizacion/planta_nivel1/parametros";
const char SUBSCRIBE_TOPIC[] = "optimizacion/planta_nivel1/consigna";

// Certificados
BearSSL::X509List cert(cacert);
BearSSL::X509List client_crt(client_cert);
BearSSL::PrivateKey key(privkey);

WiFiClientSecure net;
PubSubClient client(net);

// Pines PWM
const int VALVULA_PIN = D3;
const int BOMBA_PIN = D4;

// Intervalo de publicación
const long TIEMPO_MUESTREO = 1000;
unsigned long lastMillis = 0;

void configTiempoCert() {
  // Configuración del tiempo para la validacion de los certificados
  configTime(0, 0, "pool.ntp.org", "time.nist.gov");
  time_t now = time(nullptr);
  while (now < 1510592825) {
    delay(500);
    now = time(nullptr);
  }
}

void messageReceived(char *topic, byte *payload, unsigned int length) {
// Callback por cada mensaje recibido por MQTT
  StaticJsonDocument<256> doc;
  DeserializationError error = deserializeJson(doc, payload, length);
  if (error) {
    Serial.println("Error al parsear JSON");
    return;
  }

  float valvula = doc["Valvula"];
  const char* uValvula = doc["UValvula"];
  float bomba = doc["Bomba"];
  const char* uBomba = doc["UBomba"];

  if (strcmp(uValvula, "%") == 0) {
    valvula = valvula * 0.1;  // 10% = 1V
  }
  if (strcmp(uBomba, "%") == 0) {
    bomba = bomba * 0.1;
  }

  // Convertir a rango 0–3.3V
  int pwmValvula = map(valvula * 100, 0, 1000, 0, 1023);
  int pwmBomba = map(bomba * 100, 0, 1000, 0, 1023);

  analogWrite(VALVULA_PIN, pwmValvula);
  analogWrite(BOMBA_PIN, pwmBomba);

  Serial.printf("PWM Valvula: %d, PWM Bomba: %d\n", pwmValvula, pwmBomba);
}

void conectarMosquittoEC2() {
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  while (WiFi.status() != WL_CONNECTED) {
    Serial.println("Conectando a WiFi...");
    delay(1000);
  }

  configTiempoCert();

  net.setTrustAnchors(&cert);
  net.setClientRSACert(&client_crt, &key);

  client.setServer(MQTT_HOST, PORT);
  client.setCallback(messageReceived);

  while (!client.connect("ESP8266_1")) {
    delay(1000);
  }

  client.subscribe(SUBSCRIBE_TOPIC);
}

void publicarSensor() {
  float voltaje = analogRead(A0) * (3.3 / 1023.0);
  float voltajeReal = voltaje * (10.0 / 3.3);
  if (voltajeReal > 10.00){
    voltajeReal = 10.00;
  }

  StaticJsonDocument<128> doc;
  doc["Sensor"] = voltajeReal;
  doc["USensor"] = "V";

  char buffer[128];
  serializeJson(doc, buffer);
  client.publish(PUBLISH_TOPIC, buffer);

  Serial.printf("Sensor: %.2f V\n", voltajeReal);
}

void setup() {
  Serial.begin(115200);
  pinMode(VALVULA_PIN, OUTPUT);
  pinMode(BOMBA_PIN, OUTPUT);
  conectarMosquittoEC2();
}

void loop() {
  if (!client.connected()) {
    conectarMosquittoEC2();
  }
  client.loop();

  if (millis() - lastMillis > TIEMPO_MUESTREO) {
    lastMillis = millis();
    publicarSensor();
  }
}
