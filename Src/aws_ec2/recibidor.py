# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025

from flask import Flask, request
import os
import subprocess
import signal

# Script para generar un servidor web que recibe scripts de ataque
# Se subirá a la EC2 de AWS para que los usuarios puedan subir scripts de ataque
# Es importante convertirlo en un servicio de sistema para que se inicie automáticamente al arrancar la EC2


import logging
from datetime import datetime

# Configuración del logger
LOG_FILE = "/lalalab/ciberataque/servidor.log"
logging.basicConfig(
    filename=LOG_FILE, level=logging.INFO, format="%(asctime)s [%(levelname)s] %(message)s", datefmt="%Y-%m-%d %H:%M:%S"
)

# Asegurarse de que el directorio de subida existe
UPLOAD_FOLDER = "/lalalab/ciberataque/"
os.makedirs(UPLOAD_FOLDER, exist_ok=True)

# Nombre del script y su ruta
SCRIPT_NAME = "ataque_actual.py"
SCRIPT_PATH = os.path.join(UPLOAD_FOLDER, SCRIPT_NAME)

# Archivo para guardar el estado del flag
FLAG_FILE = os.path.join(UPLOAD_FOLDER, "flag.txt")

process = None
# Crear la aplicación Flask
app = Flask(__name__)


@app.route("/upload", methods=["POST"])
def upload_script():
    """    Endpoint para subir un script de ataque.
    El script debe ser un archivo .py y se guardará en el directorio de subida.
    """
    if "file" not in request.files:
        return "No file part", 400
    file = request.files["file"]

    if file.filename.endswith(".py"):
        file.save(SCRIPT_PATH)
        logging.info("Script recibido: %s", file.filename)
        return "Script cargado correctamente", 200
    return "Invalid file type", 400


@app.route("/control", methods=["POST"])
def control_script():
    """    Endpoint para controlar la ejecución del script de ataque.
    Permite iniciar o detener el script basado en un flag enviado en el cuerpo de la solicitud.
    El flag debe ser un JSON con el campo "ejecutar" (booleano).
    """
    global process
    data = request.get_json()
    print("Datos recibidos:", data)

    if not data or "ejecutar" not in data:
        return "Falta el parámetro 'ejecutar'", 400

    ejecutar = data["ejecutar"]

    # Guardar el flag en un archivo
    with open(FLAG_FILE, "w") as f:
        f.write("1" if ejecutar else "0")

    logging.info("Flag actualizado: ejecutar = %s", ejecutar)
    print("Flag ejecutar:", ejecutar)

    if ejecutar:
        if process is None:
            process = subprocess.Popen(["python3", SCRIPT_PATH])
            logging.info("Script lanzado")
            return "Script ejecutado", 200
        else:
            logging.info("Script ya estaba en ejecución")
            return "El script ya está en ejecución", 200
    else:
        if process:
            os.kill(process.pid, signal.SIGTERM)
            process = None
            logging.info("Script detenido")
            return "Script detenido", 200
        else:
            logging.info("No había script en ejecución")
            return "No hay script en ejecución", 200


# Inicio el servidor en el puerto 5001, previamente abierto en AWS
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5001)
