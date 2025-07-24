# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025




# Ejemplo de un ataque MITM para cargar directamente en el sistema


import random

import random

def procesar_consigna(topic, payload):
    """
    Procesa mensajes que vienen de la app (app_a_bridge).
    Modifica los valores numéricos sumando un valor aleatorio entre 0 y 1,
    y cambia las unidades '%' por 'V' y viceversa.
    """
    try:
        if isinstance(payload, dict):
            modificado = {}
            for clave, valor in payload.items():
                try:
                    if not clave.startswith("U"):
                        numero = float(str(valor).replace(",", "."))
                        nuevo_valor = round(numero + random.uniform(0, 1), 2)
                        modificado[clave] = str(nuevo_valor).replace(".", ",")
                    else:
                        if valor == "%":
                            modificado[clave] = "V"
                        elif valor == "V":
                            modificado[clave] = "%"
                        else:
                            modificado[clave] = valor
                except ValueError:
                    modificado[clave] = valor
            return {
                "payload": modificado,
                "tipo_ataque": "mitm"
            }
    except Exception as e:
        print(f"[!] Error en procesar_consigna: {e}")
        return {
            "payload": payload,
            "tipo_ataque": "error"
        }


def procesar_parametros(topic, payload):
    """
    Procesa mensajes que vienen de algun cliente físico (parametros).
    Coge los valores numéricos y les suma un valor aleatorio entre -1 y 1,
    """
    try:
        if isinstance(payload, dict):
            modificado = {}
            for k, v in payload.items():
                try:
                    valor = float(str(v).replace(",", "."))
                    nuevo_valor = round(valor + random.uniform(-1, 1), 2)
                    modificado[k] = str(nuevo_valor).replace(".", ",")
                except ValueError:
                    modificado[k] = v
            return {
                "payload": modificado,
                "tipo_ataque": "mitm"
            }

    except Exception as e:
        print(f"[!] Error en procesar_parametros: {e}")
        return {"payload": payload, "tipo_ataque": "error"}
