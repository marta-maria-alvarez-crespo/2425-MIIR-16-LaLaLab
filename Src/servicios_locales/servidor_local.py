# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 18/07/2025

from fastapi import FastAPI, HTTPException
import numpy as np
import os
import ast
import uvicorn
import csv
from datetime import datetime
import logging
from fastapi import Form

# inicializacion de la aplicacion FastAPI
app = FastAPI()

# Inicializacion del diccionario que almacenará las plantas simuladas
plantas = {}

# Directorio donde se guardarán los documentos
documentos_dir = os.path.join(os.path.expanduser("~"), "Documents", "lalalab")
plantas_dir = os.path.join(documentos_dir, "plantas")

# Verifica si el directorio de documentos existe, si no, lo crea
if not os.path.exists(documentos_dir):
    os.makedirs(plantas_dir, exist_ok=True)

# Se crea un archivo CSV para registrar los valores simulados en cada ejecución
timestamp_inicio = datetime.now().strftime("%Y%m%d_%H%M%S")
csv_filename = os.path.join(documentos_dir, f"{timestamp_inicio}_Valores_Simulados.csv")

# Se crea el archivo CSV y se escribe la cabecera
with open(csv_filename, mode="w", newline="", encoding="utf-8") as file:
    writer = csv.writer(file)
    writer.writerow(["timestamp", "planta_id", "consigna", "coef_num", "coef_den", "sensor_simulado"])


class PlantaSimulada:
    """ Clase que simula una planta con una función de transferencia.
    Esta clase carga los coeficientes de la función de transferencia desde un archivo y simula
    el comportamiento de la planta en función de las entradas proporcionadas.
    """
    def __init__(self, planta_id):
        """ Inicializa la planta simulada con su ID y carga la función de transferencia.
        Args:
            planta_id (str): Identificador de la planta.
        """
        self.planta_id = planta_id
        self.num, self.den, self.Ts = self.cargar_funcion_transferencia(planta_id)
        self.entrada = np.zeros(3)
        self.salida = np.zeros(3)
        self.activa = False

    def cargar_funcion_transferencia(self, planta_id):
        """ Carga la función de transferencia de la planta desde un archivo.
        Args:
            planta_id (str): Identificador de la planta.
        Returns:
            tuple: Coeficientes del numerador, denominador y el tiempo de muestreo (Ts).
        Raises:
            FileNotFoundError: Si el archivo de la planta no se encuentra.
        """
        ruta = os.path.join(plantas_dir, f"planta{planta_id}.txt")

        if not os.path.exists(ruta):
            raise FileNotFoundError(f"No se encontró el archivo {ruta}")

        with open(ruta, "r", encoding="utf-8") as f:
            lines = f.readlines()

        num = ast.literal_eval(lines[0].split(":")[1].strip())
        den = ast.literal_eval(lines[1].split(":")[1].strip())
        Ts = float(lines[2].split(":")[1].strip())

        num = np.array(num, dtype=np.float64)
        den = np.array(den, dtype=np.float64)

        return num, den, Ts

    def simular(self, entradas):
        """ Simula el comportamiento de la planta en función de las entradas proporcionadas.
        Args:
            entradas (list): Lista de entradas a la planta, donde la primera entrada es la consigna.
        Returns:
            list: Lista con el valor simulado de la salida de la planta.
        """
        bomba = entradas[0]
        k = 0

        self.entrada[k - 2] = self.entrada[k - 1]
        self.entrada[k - 1] = self.entrada[k]

        if self.activa:
            self.entrada[k] = bomba

        self.salida[k - 2] = self.salida[k - 1]
        self.salida[k - 1] = self.salida[k]

        salida = self.num[0] * self.entrada[k - 2] - self.den[0] * self.salida[k - 1] - self.den[1] * self.salida[k - 2]

        self.salida[k] = salida
        return [self.salida[k]]


@app.post("/simular")
def simular(payload: dict):
    """ Endpoint para simular el comportamiento de una planta.
    Args:
        payload (dict): Diccionario que contiene el ID de la planta y las entradas.
    Returns:
        dict: Diccionario con la salida simulada de la planta.
    Raises:
        HTTPException: Si ocurre un error durante la simulación.
    """
    try:
        planta_id = payload["planta_id"]
        entradas = payload["entradas"]

        if planta_id not in plantas:
            plantas[planta_id] = PlantaSimulada(planta_id)

        planta = plantas[planta_id]
        salida = planta.simular(entradas)

        # Registrar en el CSV
        with open(csv_filename, mode="a", newline="", encoding="utf-8") as file:
            writer = csv.writer(file)
            writer.writerow(
                [
                    datetime.now().isoformat(),
                    planta_id,
                    entradas[0],  # consigna
                    planta.num.tolist(),
                    planta.den.tolist(),
                    salida[0],  # valor del sensor simulado
                ]
            )

        return {"salida": salida}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e)) from e


@app.post("/activar")
def activar(planta_id: str):
    """ Endpoint para activar una planta simulada.
    Args:
        planta_id (str): Identificador de la planta a activar.
    Returns:
        dict: Diccionario con el estado de la planta activada.
    """
    if planta_id not in plantas:
        plantas[planta_id] = PlantaSimulada(planta_id)
    plantas[planta_id].activa = True
    return {"status": f"Planta {planta_id} activada"}


@app.post("/desactivar")
def desactivar(planta_id: str):
    """ Endpoint para desactivar una planta simulada.
    Args:
        planta_id (str): Identificador de la planta a desactivar.
    Returns:
        dict: Diccionario con el estado de la planta desactivada.
    """
    if planta_id not in plantas:
        plantas[planta_id] = PlantaSimulada(planta_id)
    plantas[planta_id].activa = False
    return {"status": f"Planta {planta_id} desactivada"}


@app.post("/recargar")
def recargar(planta_id: str = Form(...)):
    """ Endpoint para recargar y reiniciar una planta simulada.
    Args:
        planta_id (str): Identificador de la planta a recargar.
    Returns:
        dict: Diccionario con el estado de la planta recargada y reiniciada.
    Raises:
        HTTPException: Si ocurre un error durante la recarga.
    """
    try:
        nueva_planta = PlantaSimulada(planta_id)
        nueva_planta.salida = np.zeros(3)
        plantas[planta_id] = nueva_planta

        return {"status": f"Planta {planta_id} recargada y reiniciada a 0"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e)) from e



if __name__ == "__main__":
    logging.getLogger("uvicorn").handlers.clear()
    uvicorn.run(
        app,
        host="127.0.0.1",
        port=8000,
        log_config=None,
    )
