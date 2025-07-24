# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025

import os
import time
from datetime import datetime
import paho.mqtt.client as mqtt
import threading
import json
import uuid
from db_logger import insertar_mensaje
import importlib.util

# Ruta del script de ataque y del archivo de flag
SCRIPT_PATH = "/lalalab/ciberataque/ataque_actual.py"
FLAG_PATH = "/lalalab/ciberataque/flag.txt"


def cargar_script():
    """Carga el script de ataque actual desde la ruta especificada."""
    spec = importlib.util.spec_from_file_location("ataque_actual", SCRIPT_PATH)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def leer_flag():
    """Lee el archivo de flag para determinar si el ataque está habilitado."""
    try:
        with open(FLAG_PATH, "r") as f:
            return f.read().strip() == "1"
    except FileNotFoundError:
        return False


def esperar_con_interrupcion(segundos_totales):
    intervalo = 1  # segundos
    transcurrido = 0
    while transcurrido < segundos_totales:
        if not leer_flag():
            print("[!] Flag desactivada. Interrumpiendo espera.")
            return False
        time.sleep(intervalo)
        transcurrido += intervalo
    return True


def procesar_mensaje(client, topic, payload):
    """Procesa el mensaje recibido, inserta en la base de datos y publica el resultado."""
    mensaje_uuid = str(uuid.uuid4())
    timestamp = datetime.utcnow()
    flag = leer_flag()
    ataque = cargar_script()

    if topic.endswith("app_a_bridge"):
        topic_final = topic.replace("app_a_bridge", "consigna")
        fuente = True
        funcion = ataque.procesar_consigna
    elif topic.endswith("parametros"):
        topic_final = topic
        fuente = False
        funcion = ataque.procesar_parametros
    else:
        print(f"[!] Topic no reconocido: {topic}")
        return

    try:
        data = json.loads(payload)
    except json.JSONDecodeError:
        data = payload

    insertar_mensaje(
        mensaje_uuid, topic_final, json.dumps(data) if isinstance(data, dict) else data, False, fuente, timestamp
    )

    if flag:
        resultado = funcion(topic, data)

        if isinstance(resultado, dict) and "mensajes" in resultado:
            mensajes = resultado["mensajes"]
            tipo_ataque = resultado.get("tipo_ataque", "replay attack")

            try:
                mensajes.sort(key=lambda m: datetime.fromisoformat(m["timestamp"]))
            except Exception as e:
                print(f"[!] Error al ordenar mensajes por timestamp: {e}")
                return

            nuevo_topic = resultado.get("nuevo_topic")

            for i, mensaje in enumerate(mensajes):
                nuevo_uuid = str(uuid.uuid4())
                if i > 0:
                    try:
                        t_actual = datetime.fromisoformat(mensaje["timestamp"])
                        t_anterior = datetime.fromisoformat(mensajes[i - 1]["timestamp"])
                        espera = (t_actual - t_anterior).total_seconds()
                        if espera > 0:
                            if not esperar_con_interrupcion(espera):
                                return
                    except Exception as e:
                        print(f"[!] Error al calcular espera entre mensajes: {e}")

                insertar_mensaje(
                    nuevo_uuid,
                    mensaje["topic"],
                    json.dumps(mensaje["payload"]),
                    True,
                    fuente,
                    timestamp,
                    tipo_ataque=tipo_ataque,
                    nuevo_topic=nuevo_topic,
                )
                publicar(client, mensaje["topic"], json.dumps(mensaje["payload"]))
            return

        if isinstance(resultado, dict) and "payload" in resultado:
            nuevo_payload = resultado["payload"]
            tipo_ataque = resultado.get("tipo_ataque", "unknown")
            nuevo_topic = resultado.get("nuevo_topic")

            if nuevo_payload != data or nuevo_topic:
                topic_publicacion = (
                    nuevo_topic
                    if nuevo_topic
                    else (topic_final if fuente else topic.replace("parametros", "bridge_a_aws"))
                )

                insertar_mensaje(
                    mensaje_uuid,
                    topic_publicacion,
                    json.dumps(nuevo_payload) if isinstance(nuevo_payload, dict) else nuevo_payload,
                    True,
                    fuente,
                    timestamp,
                    tipo_ataque=tipo_ataque,
                    nuevo_topic=nuevo_topic,
                )

                publicar(
                    client,
                    topic_publicacion,
                    json.dumps(nuevo_payload) if isinstance(nuevo_payload, dict) else nuevo_payload,
                )
                return

    publicar(
        client,
        topic_final if fuente else topic.replace("parametros", "bridge_a_aws"),
        json.dumps(data) if isinstance(data, dict) else data,
    )


def publicar(client, topic, payload):
    """Publica el mensaje en el topic especificado."""
    client.publish(topic, payload)


def on_message(client, userdata, msg):
    """Callback que se ejecuta al recibir un mensaje."""
    topic = msg.topic
    payload = msg.payload.decode()
    threading.Thread(target=procesar_mensaje, args=(client, topic, payload)).start()


def main():
    """Función principal para configurar el cliente MQTT y suscribirse a los topics."""
    client = mqtt.Client()
    client.on_message = on_message
    client.tls_set(
        ca_certs="/lalalab/ciberataque/ca.crt",
        certfile="/lalalab/ciberataque/sniffer.crt",
        keyfile="/lalalab/ciberataque/sniffer.key",
    )
    client.tls_insecure_set(True)
    client.connect("localhost", 8883, 60)
    client.subscribe("+/+/app_a_bridge")
    client.subscribe("+/+/parametros")
    client.loop_forever()


if __name__ == "__main__":
    main()
