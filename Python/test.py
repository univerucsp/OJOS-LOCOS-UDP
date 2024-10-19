import socket
import cv2
from gaze_tracking import GazeTracking
import time

# Configuración del socket para recibir el mensaje de activación
HOST = '127.0.0.1'  # Dirección IP local
PORT = 65433        # Puerto para enviar los datos de posición
CALIBRATION_PORT = 65432  # Puerto para recibir el mensaje de activación

# Inicializar GazeTracking
gaze = GazeTracking()
webcam = cv2.VideoCapture(0)

def calibrar():
    print("Por favor, mueve tus ojos a los extremos posibles (izquierda, derecha, arriba, abajo) durante los próximos 10 segundos.")
    
    max_x, max_y = float('-inf'), float('-inf')
    min_x, min_y = float('inf'), float('inf')
    
    start_time = time.time()
    last_detection_time = start_time
    detection_timeout = 5  # Si no se detectan ojos por 5 segundos, reiniciar la calibración
    calibration_duration = 10  # Tiempo total para calibrar
    
    while time.time() - start_time < calibration_duration:
        ret, frame = webcam.read()
        if not ret:
            print("Error al leer de la cámara.")
            break

        # Procesar el frame con GazeTracking
        gaze.refresh(frame)

        # Obtener las coordenadas de las pupilas
        left_pupil = gaze.pupil_left_coords()
        right_pupil = gaze.pupil_right_coords()

        if left_pupil and right_pupil:
            # Promediar las posiciones de ambas pupilas
            avg_pupil_x = (left_pupil[0] + right_pupil[0]) / 2
            avg_pupil_y = (left_pupil[1] + right_pupil[1]) / 2

            # Actualizar máximos y mínimos
            max_x = max(max_x, avg_pupil_x)
            max_y = max(max_y, avg_pupil_y)
            min_x = min(min_x, avg_pupil_x)
            min_y = min(min_y, avg_pupil_y)

            last_detection_time = time.time()  # Reiniciar el tiempo de detección
        else:
            # Si no se detectan pupilas por más de 5 segundos, reiniciar calibración
            if time.time() - last_detection_time > detection_timeout:
                print("No se detectaron ojos por 5 segundos. Por favor, reubícate frente a la cámara.")
                return calibrar()

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    # Verificar si se detectaron ojos correctamente durante el periodo de calibración
    if max_x == float('-inf') or min_x == float('inf'):
        print("No se pudo completar la calibración correctamente. Intenta nuevamente.")
        return calibrar()

    print(f"Calibración completa.\nValores detectados:\nMax X: {max_x}, Min X: {min_x}, Max Y: {max_y}, Min Y: {min_y}")
    return max_x, min_x, max_y, min_y

# Función para esperar el mensaje de activación de calibración
def esperar_mensaje_calibracion():
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
        s.bind((HOST, CALIBRATION_PORT))
        print(f'Esperando mensaje de activación en el puerto {CALIBRATION_PORT}...')
        
        while True:
            data, addr = s.recvfrom(1024)  # Esperar el mensaje de Unity
            message = data.decode('utf-8').strip()
            print(f"Mensaje recibido: {message} desde {addr}")
            
            if message == "activar-calibracion":
                print("Mensaje de activación de calibración recibido.")
                return

# Iniciar el servidor UDP para enviar las posiciones
with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
    print('Esperando conexión de Unity...')
    
    # Esperar hasta recibir el mensaje de "activar-calibracion"
    esperar_mensaje_calibracion()

    # Llamar a la función de calibración
    max_x, min_x, max_y, min_y = calibrar()
    
    while True:
        ret, frame = webcam.read()
        if not ret:
            break

        # Procesar el frame con GazeTracking
        gaze.refresh(frame)

        # Obtener las coordenadas de las pupilas
        left_pupil = gaze.pupil_left_coords()
        right_pupil = gaze.pupil_right_coords()

        if left_pupil and right_pupil:
            # Promediar las posiciones de ambas pupilas
            avg_pupil_x = (left_pupil[0] + right_pupil[0]) / 2
            avg_pupil_y = (left_pupil[1] + right_pupil[1]) / 2

            # Normalizar las posiciones usando los valores de calibración
            screen_x_normalized = (avg_pupil_x - min_x) / (max_x - min_x)  # Normalización de X
            screen_y_normalized = (avg_pupil_y - min_y) / (max_y - min_y)  # Normalización de Y
            
            # Ajustar a rango de -1 a 1
            screen_x_normalized = 2 * screen_x_normalized - 1
            screen_y_normalized = 2 * screen_y_normalized - 1

            # Imprimir la posición normalizada de la pantalla
            print(f'Posición en pantalla normalizada - X: {screen_x_normalized:.4f}, Y: {screen_y_normalized:.4f}')
            
            # Enviar la posición de la pantalla a Unity
            message = f'{screen_x_normalized},{screen_y_normalized}\n'
            s.sendto(message.encode('utf-8'), (HOST, PORT))
        else:
            print(f'No se detectaron ojos.')
            # Si no se detectan ojos, enviar un mensaje a Unity
            s.sendto(b'no_eyes_detected\n', (HOST, PORT))

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

webcam.release()
cv2.destroyAllWindows()

