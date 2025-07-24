# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025




# Plantilla para realizar ciberataques dentro del sistema Lalalab
# El nombre de las funciones NO ES MODIFICABLE, ya que son llamadas desde el servidor.
# Seguir las instrucciones de la plantilla para realizar los ataques

# Tipos de ataques:
# - no-ataque
# - man-in-the-middle
# - topic hikjacking [Pendiente de implementar]
# - replay attack [Pendiente de implementar]

import random

def procesar_app_a_bridge(topic, payload):
    """
    Procesa mensajes que vienen de la app (app_a_bridge).
    """
    try:
        return {
            "payload": payload,
            "nuevo_topic": topic.replace("app_a_bridge", "topic_modificado"),
            "tipo_ataque": "topic hijacking"
        }
    except Exception as e:
        print(f"[!] Error en procesar_app_a_bridge: {e}")
        return {
            "payload": payload,
            "tipo_ataque": "error"
        }

    except Exception as e:
        print(f"[!] Error en procesar_app_a_bridge: {e}")
        return {"payload": payload, "tipo_ataque": "error"}


def procesar_parametros(topic, payload):
    """
    Procesa mensajes que vienen de algun cliente físico (parametros).
    """
    try:
        if isinstance(payload, dict):
            modificado = {}
            for k, v in payload.items():
                try:
                    valor = float(str(v).replace(",", "."))
                    nuevo_valor = round(valor + random.uniform(-10, 10), 2)
                    modificado[k] = str(nuevo_valor).replace(".", ",")
                except ValueError:
                    modificado[k] = v
            return {
                "payload": modificado,
                "tipo_ataque": "mitm"
            }
        # return {"payload": payload, "tipo_ataque": "sin modificación"}

    except Exception as e:
        print(f"[!] Error en procesar_parametros: {e}")
        return {"payload": payload, "tipo_ataque": "error"}
