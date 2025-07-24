# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 19/07/2025


# Ejemplo para realizar un replay attack dentro del sistema


import psycopg2
import json

DB_CONFIG = {
    "dbname": "",
    "user": "",
    "password": "",
    "host": "",
    "port": 0000
}

def obtener_mensajes(topic_objetivo, fecha_inicio, fecha_fin):
    """Obtiene mensajes de la base de datos que coinciden con el topic y rango de fechas especificados. """
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()
        cursor.execute("""
            SELECT c.topic, c.timestamp, m.payload
            FROM comunicaciones c
            JOIN mensajes m ON c.uuid = m.uuid
            WHERE c.topic = %s
              AND c.timestamp BETWEEN %s AND %s
              AND c.fuente = true
              AND m.modificado = false
            ORDER BY c.timestamp ASC
        """, (topic_objetivo, fecha_inicio, fecha_fin))
        rows = cursor.fetchall()
        conn.close()
        
        return [
            {
                "topic": row[0],
                "timestamp": row[1].isoformat(),
                "payload": json.loads(row[2])
            }
            for row in rows
        ]

    except Exception as e:
        print(f"[!] Error al obtener mensajes: {e}")
        return []

def procesar_parametros(topic, payload):
    """
    Procesa mensajes que vienen de algun cliente físico (parametros).
    """
    # Consulta a la base de datos para obtener mensajes previos y simula un replay attack simple
    try:
        mensajes = obtener_mensajes(
            "optimizacion/planta_nivel1/parametros",
            "2025-03-08",
            "2025-05-19"
        )
        if mensajes:
            return {
                "tipo_ataque": "replay attack",
                "mensajes": mensajes
            }
        return {"payload": payload, "tipo_ataque": "sin modificación"}
    except Exception as e:
        print(f"[!] Error en procesar_parametros: {e}")
        return {"payload": payload, "tipo_ataque": "error"}

def procesar_consigna(topic, payload):
    """
    Procesa mensajes que vienen de la app (consigna).
    En este caso, se simula un replay attack con topic hijacking
    """
    # Consulta a la base de datos para obtener mensajes previos
    # También se puede importar un CSV o JSON con los mensajes a simular, pero
    # deben tener el formato adecuado para ser procesados...
    try:
        mensajes = obtener_mensajes(
            "optimizacion/planta_nivel4/consigna",
            "2025-07-20",
            "2025-07-22"
        )
        # Además del replay, se hace el topic hijacking
        # para que los mensajes se envíen a planta_nivel3 en lugar de planta_nivel4
        if mensajes:
            for mensaje in mensajes:
                mensaje["topic"] = mensaje["topic"].replace("planta_nivel4", "planta_nivel3")
            return {
            "mensajes": mensajes,
            "tipo_ataque": "topic hijacking"
        }
        return {
            "payload": payload,
            "tipo_ataque": "sin modificación",
            }
    except Exception as e:
        print(f"[!] Error en procesar_app_a_bridge: {e}")
        return {"payload": payload, "tipo_ataque": "error"}
