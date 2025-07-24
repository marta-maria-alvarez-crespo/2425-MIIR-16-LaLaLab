# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025

import psycopg2

DB_CONFIG = {
    "dbname": "",
    "user": "",
    "password": "",
    "host": "",
    "port": 0000
}

def insertar_mensaje(uuid, topic, payload, modificado, fuente, timestamp, tipo_ataque=None, nuevo_topic=None):
    conn = None
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cur = conn.cursor()

        # Insertar en comunicaciones si no existe
        cur.execute("SELECT 1 FROM comunicaciones WHERE uuid = %s", (uuid,))
        if not cur.fetchone():
            cur.execute("""
                INSERT INTO comunicaciones (uuid, timestamp, topic, fuente)
                VALUES (%s, %s, %s, %s)
            """, (uuid, timestamp, topic, fuente))

        # Insertar en mensajes
        cur.execute("""
            INSERT INTO mensajes (uuid, payload, modificado)
            VALUES (%s, %s, %s)
            RETURNING mensaje_id
        """, (uuid, payload, modificado))
        mensaje_id = cur.fetchone()[0]

        # Insertar en ciberataques si aplica
        if modificado and tipo_ataque and isinstance(tipo_ataque, str):
            if tipo_ataque == "topic hijacking" and nuevo_topic:
                cur.execute("""
                    INSERT INTO ciberataques (mensaje_id, tipo_ataque, nuevo_topic)
                    VALUES (%s, %s, %s)
                """, (mensaje_id, tipo_ataque, nuevo_topic))
            else:
                cur.execute("""
                    INSERT INTO ciberataques (mensaje_id, tipo_ataque)
                    VALUES (%s, %s)
                """, (mensaje_id, tipo_ataque))

        conn.commit()
        cur.close()
    except Exception as e:
        print(f"[!] Error al insertar en la base de datos: {e}")
        if conn:
            conn.rollback()
    finally:
        if conn:
            conn.close()
