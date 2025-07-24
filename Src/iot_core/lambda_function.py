# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025

import json
import boto3

# Cliente de IoT Data para publicar mensajes
iot_client = boto3.client('iot-data')

def lambda_handler(event, context):
    """
    Función Lambda que recibe un evento con un campo 'topic' y publica el payload en
    un nuevo topic basado en el sufijo del topic original
    """
    topic = event.get('topic')
    if not topic:
        return {"statusCode": 400, "body": "Falta el campo 'topic'"}

    # Determinar el nuevo topic según el sufijo
    if topic.endswith('/consigna'):
        new_topic = topic.replace('/consigna', '/app_a_bridge')
    elif topic.endswith('/bridge_a_aws'):
        new_topic = topic.replace('/bridge_a_aws', '/parametros')
    else:
        return {"statusCode": 400, "body": "Topic no válido"}

    # Copiar el payload sin el campo 'topic'
    payload = event.copy()
    payload.pop('topic', None)

    try:
        iot_client.publish(
            topic=new_topic,
            qos=1,
            payload=json.dumps(payload)
        )
        return {"statusCode": 200, "body": f"Publicado en {new_topic}"}
    except Exception as e:
        return {"statusCode": 500, "body": str(e)}