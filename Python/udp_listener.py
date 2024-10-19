import socket

# Configuración del socket
HOST = '127.0.0.1'  # Dirección IP local
PORT = 65432        # Puerto de escucha

# Crear el socket UDP
with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
    s.bind((HOST, PORT))
    print(f"Escuchando en {HOST}:{PORT}...")

    while True:
        # Esperar por un mensaje
        data, addr = s.recvfrom(1024)  # Tamaño del buffer de recepción
        message = data.decode('utf-8')  # Decodificar el mensaje
        print(f"Mensaje recibido: '{message}' desde {addr}")

