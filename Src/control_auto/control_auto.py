# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025



# Ejemplo de automatización de control de plantas

import time
import random

# IDs de plantas a controlar
plantas = [1]

# Simulación de consignas
for _ in range(10):
    unidadB = random.choice(["V", "%"])
    unidadV = random.choice(["V", "%"])

    bomba = round(random.uniform(0, 10), 2)
    valvula = round(random.uniform(0, 10), 2)

    # Requisito fundamental para el envío: que el print comience con "SEND", formateado como se indica
    print(f"SEND {bomba} {valvula} {unidadB} {unidadV} {','.join(map(str, plantas))}", flush=True)

    time.sleep(2)
