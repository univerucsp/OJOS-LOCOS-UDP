import socket
import pyaudio
import json
from vosk import Model, KaldiRecognizer

# Configuración del cliente UDP
server_ip = "127.0.0.1"  # Dirección IP del servidor Unity
server_port = 65434      # Puerto para enviar mensajes

# Inicializa el modelo de Vosk
model = Model("vosk-model-small-en-us-0.15")
recognizer = KaldiRecognizer(model, 16000)

# Configuración de PyAudio
p = pyaudio.PyAudio()
stream = p.open(format=pyaudio.paInt16, channels=1, rate=16000, input=True, frames_per_buffer=8192)
stream.start_stream()

def recognize_words():
    print("Escuchando... (diga 'play', 'calibrate', 'quit' o 'back')")
    data = stream.read(4096)

    if recognizer.AcceptWaveform(data):
        result = json.loads(recognizer.Result())
        recognized_text = result.get("text", "")
        print(f"Has dicho: '{recognized_text}'")

        # Crear el socket UDP
        udp_client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

        # Verificar si se han reconocido las palabras específicas
        if "play" in recognized_text:
            print("Iniciando el juego...")
            udp_client.sendto(b"iniciar-juego", (server_ip, server_port))
        elif "calibrate" in recognized_text:
            print("Calibrando...")
            udp_client.sendto(b"calibrar", (server_ip, server_port))
        elif "quit" in recognized_text:
            print("Saliendo...")
            udp_client.sendto(b"salir", (server_ip, server_port))
            return False  # Dejar de correr
        elif "back" in recognized_text:
            print("Volviendo...")
            udp_client.sendto(b"volver", (server_ip, server_port))
        else:
            print("Palabra no reconocida.")

        # Cerrar el socket
        udp_client.close()
    else:
        print("No se pudo entender el audio o no se detectaron palabras.")

    return True  # Continúa reconociendo palabras

# Bucle principal
if __name__ == "__main__":
    running = True
    while running:
        running = recognize_words()

# Cierra el stream y PyAudio
stream.stop_stream()
stream.close()
p.terminate()

