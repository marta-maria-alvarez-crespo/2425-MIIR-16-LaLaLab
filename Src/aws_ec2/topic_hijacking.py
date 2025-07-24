# Autora: Marta María Álvarez Crespo
# Escola Politécnica de Enxeñaría de Ferrol - Universidade da Coruña
# GitHub: github.com/marta-maria-alvarez-crespo/2425-MIIR-16-LaLaLab

# Ultima modificación: 13/06/2025


# Ejemplo de código para realizar un ataque de topic hijacking en el sistema


def procesar_consigna(topic, payload):
    """
    Procesa mensajes que vienen de la app (app_a_bridge).
    """
    try:
        return {
            "payload": payload,
            "nuevo_topic": topic.replace("planta_nivel3", "planta_nivel5"),
            "tipo_ataque": "topic hijacking"
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
    """
    try:
        return {
            "payload": payload,
            "nuevo_topic": topic.replace("optimizacion", "automatizacion"),
            "tipo_ataque": "topic hijacking"
        }
    except Exception as e:
        print(f"[!] Error en procesar_parametros: {e}")
        return {
            "payload": payload,
            "tipo_ataque": "error"
        }