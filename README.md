# OJOS-LOCOS-UDP

Este proyecto utiliza un sistema de reconocimiento de voz para controlar un juego en Unity a través de mensajes UDP. La aplicación escucha comandos de voz como "play", "calibrate", "quit" y "back" para interactuar con el juego. El sistema está integrado con Unity y utiliza un script en Python para enviar los comandos reconocidos.

## Integrantes del grupo
- Marco Antonio Guillén Dávila
- Anthony Mamani Mamani
- Mariana Cáceres Urquizo

## Video de demostración
Mira el video de demostración para ver cómo funciona el proyecto: [OJOS-LOCOS-UDP - Resultados](https://youtu.be/kGCL-5tiIUY).

## Instrucciones para la configuración

### 1. Clonar el repositorio
Primero, clona el repositorio desde GitHub:
```bash
git clone https://github.com/univerucsp/OJOS-LOCOS-UDP.git
```

### 2. Agregar el proyecto en Unity
Abre Unity y selecciona Add project from disk para agregar el proyecto clonado.

### 3. Configuración de Python
Antes de ejecutar el programa, asegúrate de instalar las dependencias necesarias para Python:

#### 1. Dirígete a la carpeta Python del proyecto:
```bash
cd OJOS-LOCOS-UDP/Python
```
#### 2. Instala los paquetes necesarios:
```bash
pip install -r requirements.txt
```
#### 3. Instalar módulos adicionales:
```bash
pip install pyaudio vosk
```

### 4. Ejecución del proyecto
1. Ejecuta los siguientes scripts de Python en la carpeta Python:
```bash
python test.py
python audio.py
```
2. Luego, abre Unity y ejecuta el juego para comenzar a interactuar mediante comandos de voz.
3. (Opcional) Si deseas hacer un build del proyecto para otra plataforma, asegúrate de que los scripts de Python test.py y audio.py estén en ejecución antes de iniciar el juego.

