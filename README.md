# OJOS-LOCOS-UDP

Este proyecto desarrolla un videojuego que permite la interacción a través del seguimiento ocular y el reconocimiento de voz, inspirado en el popular juego Osu!. Los jugadores controlan una esfera roja utilizando sus ojos para colisionar con esferas blancas, acumulando puntos. Utilizando tecnologías como GazeTracking y Vosk, el sistema ofrece una experiencia accesible y entretenida sin requerir dispositivos de control tradicionales. Las pruebas realizadas han confirmado la funcionalidad del sistema en diversas condiciones, fomentando la inclusión de usuarios con diferentes habilidades.


## Integrantes del grupo
- Marco Antonio Guillén Dávila
- Anthony Mamani Mamani
- Mariana Cáceres Urquizo

## Video de demostración y resultados
Mira el video de demostración para ver cómo funciona el proyecto y los resultados obtenidos:

[![OJOS-LOCOS-UDP - Resultados](https://img.youtube.com/vi/kGCL-5tiIUY/0.jpg)](https://youtu.be/kGCL-5tiIUY)

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

1. Dirígete a la carpeta Python del proyecto:
```bash
cd OJOS-LOCOS-UDP/Python
```
 2. Instala los paquetes necesarios:
```bash
pip install -r requirements.txt
```
3. Instalar módulos adicionales:
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

