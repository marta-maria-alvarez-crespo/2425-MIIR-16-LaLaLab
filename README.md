# 2425-MIIR-16-LaLaLab
Trabajo Fin de Máster "Diseño, e implementación de un sistema IoT con modelo 3D para monitorización en tiempo real de plantas de laboratorio" - Máster en Informática Industrial y Robótica, EPEF, UDC.

## Estructura del repositorio


```plaintext
2425_MIIR_16
├── Src/
│   ├── aws_ec2/
│   │   ├── ataque_actual.py
│   │   ├── db_logger.py
│   │   ├── man-in-the-middle.py
│   │   ├── recibidor.py
│   │   ├── replay_attack.py
│   │   ├── sinffer.py
│   │   └── topic_hijacking.py
│   ├── control_auto/
│   │   └── control_auto.py
│   ├── iot_core/
│   │   └── lambda_function.py
│   ├── modelos_openscad/
│   │   ├── electronica.scad
│   │   ├── estructura_planta.scad
│   │   ├── main-scad
│   │   ├── motor.scad
│   │   └── variador.scad
│   ├── planta_nivel/
│   │   └── planta_nivel.ino
│   └── servicios_locales/
│       ├── consultas_database.py
│       └── servidor_local.py
└── Assets/
    └── Scripts/
        ├── Etiquetas/
        │   └── Etiquetas.cs
        ├── FuncionTransferencia/
        │   ├── BotonCargarFT.cs
        │   ├── CargadorFuncionTransferencia.cs
        │   ├── LanzadorPython.cs
        │   └── SimuladorPlanta.cs
        ├── MQTT/
        │   ├── MqttConsignaSender.cs
        │   ├── MQTTPublisher.cs
        │   ├── MqttSubscriberFromJson.cs
        │   └── salidaSegura.cs
        ├── Objeto Planta/
        │   ├── GestorPlantas.cs
        │   ├── PlantaNivel.cs
        │   ├── PythonInputListener.cs
        │   └── UnityMainThreadDispatcher.cs
        └── UI/
        │   ├── Botones/
        │   │   ├── CustomMenu.cs
        │   │   ├── CustomToggleUI.cs
        │   │   ├── DropdownFixer.cs
        │   │   ├── SliderToggle.cs
        │   │   └── SimpleButton.cs
        │   ├── Cerrar/
        │   │   ├── Cerrar.cs
        │   │   └── Minimizar.cs
        │   ├── Grafico/
        │   │   ├── Graficador.cs
        │   │   ├── GraficadorHistorico.cs
        │   │   ├── GraficaTiempoReal.cs
        │   │   ├── ModoGraficas.cs
        │   │   ├── ModoGraficasManager.cs
        │   │   ├── MultiplesGraphs.cs
        │   │   ├── PanelSwitch.cs
        │   │   ├── SelectorRangos.cs
        │   │   └── UIMultiLineGraphHistorico.cs
        │   └── VisualizacionPlantas/
        │   │   ├── AperturaGrifo.cs
        │   │   ├── Console.cs
        │   │   ├── ControladorNivelVisual.cs
        │   │   ├── RotadorValvula.cs
        │   │   └── SensorFlotador.cs
        │   ├── ButtonSelector.cs
        │   ├── ControladorScriptRemoto.cs
        │   ├── PantallaCarga.cs
        │   ├── SliderSelector.cs
        │   └── ToggleManualAuto.cs
        └── OrbitCamera.cs
```


### Carpeta Src
En esta carpeta se almacenan los códigos que se han implementado en los diferentes sistemas y servicios que no están directamente relacionados con el funcionamiento de la aplicación de Unity:
En esta carpeta se almacenan los códigos que se han implementado en los diferentes sistemas y servicios que no están directamente relacionados con el funcionamiento de la aplicación de Unity.

- **`aws_ec2/`**: Scripts implementados en los servicios de la EC2 y plantilla y ejemplos para la realización de ciberataques.
- **`control_auto/`**: Plantilla de muestra para el control automático de las plantas.
- **`iot_core/`**: Funciones Lambda para integración con AWS IoT Core.
- **`modelos_openscad/`**: Modelado CAD de los componentes de la planta.
- **`planta_nivel/`**: Código para los microcontroladores de las plantas.
- **`servicios_locales/`**: Servicios de backend ejecutados localmente para realizar la simulación offline.

### Carpeta Scripts
Todos aquellos scripts necesarios para el correcto funcionamiento de la aplicación, así como su interacción directa con el resto del entorno.
- **`Etiquetas/`**: Gestión de las etiquetas que permiten visualizar el estado de la planta en la UI.
- **`FuncionTransferencia/`**: Carga y simulación de las funciones de transferencia.
- **`MQTT/`**: Comunicación con el sistema IoT mediante MQTT.
- **`Objeto Planta/`**: Lógica de simulación de la planta en Unity.
- **`UI/`**: Interfaz de usuario, incluyendo botones, gráficos y visualización...

**Nota**: Todas las IPs, direcciones, contraseñas... han sido eliminadas de este repositorio, así como programas específicos para la generación de certificados válidos.

