# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 18/07/2025

from fastapi import FastAPI, Form, Query
from fastapi.responses import JSONResponse, StreamingResponse
from sqlalchemy import create_engine, text
import pandas as pd
import io
import json
import uvicorn
import logging

# Inicializar fastapi y la conexión a la base de datos
app = FastAPI()
DB_URL = ""

# Crear el motor de la base de datos
engine = create_engine(DB_URL)

def extraer_planta(topic):
    """Extrae el ID de la planta del topic MQTT.
    """
    try:
        return int(topic.split("planta_nivel")[1].split("/")[0])
    except:
        return None

def parsear_payload(payload, key):
    """Parsea el payload JSON y extrae el valor de la clave especificada.
    Si el payload no es válido o la clave no existe, retorna None."""
    try:
        return json.loads(payload).get(key)
    except:
        return None

@app.post("/api/consulta")
def consulta_ultimos_300(start: str = Form(...), end: str = Form(...)):
    """Consulta los últimos 300 registros de la base de datos entre dos fechas."""
    query = text("""
        SELECT c.uuid, c.timestamp, c.topic, m.payload, m.modificado
        FROM comunicaciones c
        JOIN mensajes m ON c.uuid = m.uuid
        WHERE c.timestamp BETWEEN :start AND :end
        ORDER BY c.timestamp DESC
    """)

    try:
        with engine.connect() as conn:
            df = pd.read_sql(query, conn, params={"start": start, "end": end})
        print(f"Consulta ejecutada. Filas obtenidas: {len(df)}")
        print("Columnas del DataFrame:", df.columns.tolist())
        print("Primeras filas del df:")
        print(df.head(3).to_string())
    except Exception as e:
        import traceback
        print("Error al ejecutar la consulta ")
        traceback.print_exc()
        return JSONResponse(content={"error": "Error en la consulta SQL"}, status_code=500)

    # Procesar el DataFrame
    df["planta_id"] = df["topic"].apply(extraer_planta)
    # Eliminar filas sin planta_id
    df = df.dropna(subset=["planta_id"])

    # Ordenar por uuid, timestamp y modificado
    df = df.sort_values(["uuid", "timestamp", "modificado"], ascending=[True, True, False])
    # Eliminar duplicados, manteniendo el más reciente por uuid y timestamp
    df = df.drop_duplicates(subset=["uuid", "timestamp"], keep="first")

    # Extraer valores de bomba, valvula y sensor del payload
    df["bomba_real"] = df["payload"].apply(lambda p: extraer_valor_con_unidad(p, "Bomba", "UBomba"))
    df["valvula_real"] = df["payload"].apply(lambda p: extraer_valor_con_unidad(p, "Valvula", "UValvula"))
    df["sensor_real"] = df["payload"].apply(lambda p: extraer_valor_con_unidad(p, "Sensor", "USensor"))

    # Ordenar por planta_id y timestamp
    df = df.sort_values(["planta_id", "timestamp"])

    # Agrupar por planta_id y crear la estructura de respuesta
    resultado = {"data": []}
    for planta_id, group in df.groupby("planta_id"):
        puntos = group.sort_values("timestamp")
        data_points = []
        last_bomba = None
        last_valvula = None
        last_sensor = None
        for _, row in puntos.iterrows():
            bomba = row.get("bomba_real")
            valvula = row.get("valvula_real")
            sensor = row.get("sensor_real")

            if pd.notna(bomba):
                last_bomba = bomba
            if pd.notna(valvula):
                last_valvula = valvula
            if pd.notna(sensor):
                last_sensor = sensor

            data_points.append({
                "timestamp": row["timestamp"].isoformat(),
                "sensor_real": last_sensor,
                "bomba_real": last_bomba,
                "valvula_real": last_valvula
            })

        resultado["data"].append({
            "plant_id": str(planta_id),
            "data_points": data_points
        })

    return JSONResponse(content=resultado)


@app.get("/api/descargar_csv")
def descargar_csv(start: str = Query(...), end: str = Query(...)):
    """Descarga los datos de comunicaciones y mensajes en formato CSV entre dos fechas."""
    query = text("""
        SELECT * FROM comunicaciones c
        JOIN mensajes m ON c.uuid = m.uuid
        LEFT JOIN ciberataques a ON m.mensaje_id = a.mensaje_id
        WHERE c.timestamp BETWEEN :start AND :end
    """)
    with engine.connect() as conn:
        df = pd.read_sql(query, conn, params={"start": start, "end": end})

    stream = io.StringIO()
    df.to_csv(stream, index=False)
    stream.seek(0)

    return StreamingResponse(
        stream,
        media_type="text/csv",
        headers={"Content-Disposition": "attachment; filename=datos_rango.csv"}
    )

@app.get("/api/descargar_todo")
def descargar_todo():
    """Descarga todos los datos de comunicaciones y mensajes en formato CSV."""
    query = text("""
        SELECT * FROM comunicaciones c
        JOIN mensajes m ON c.uuid = m.uuid
        LEFT JOIN ciberataques a ON m.mensaje_id = a.mensaje_id
    """)
    with engine.connect() as conn:
        df = pd.read_sql(query, conn)

    stream = io.StringIO()
    df.to_csv(stream, index=False)
    stream.seek(0)

    return StreamingResponse(
        stream,
        media_type="text/csv",
        headers={"Content-Disposition": "attachment; filename=dataset_completo.csv"}
    )

def extraer_valor_con_unidad(payload, clave_valor, clave_unidad):
    """Extrae el valor de una clave específica del payload JSON.
    Si la unidad es "%", divide el valor por 10.
    Si el payload no es válido o la clave no existe, retorna None."""
    try:
        data = json.loads(payload)
        valor = data.get(clave_valor)
        unidad = data.get(clave_unidad)
        if valor is not None and unidad == "%":
            return valor / 10
        return valor
    except:
        return None



if __name__ == "__main__":
    logging.getLogger("uvicorn").handlers.clear()
    uvicorn.run(
        app,
        host="127.0.0.1",
        port=8001,
        log_config=None,
    )
