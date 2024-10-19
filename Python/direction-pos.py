import socket
import cv2
from gaze_tracking import GazeTracking
import screeninfo

# Configuración del socket
HOST = '127.0.0.1'  # Dirección IP local
PORT = 65432        # Puerto de comunicación

# Inicializar GazeTracking
gaze = GazeTracking()
webcam = cv2.VideoCapture(0)

# Obtener la resolución de la pantalla
screen = screeninfo.get_monitors()[0]  # Usamos la primera pantalla conectada
screen_width, screen_height = 1920, 1080

# Iniciar el servidor UDP
with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
    print('Esperando conexión de Unity...')
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

            # Normalizar las coordenadas al rango [-1, 1]
            screen_x_normalized = (avg_pupil_x / frame.shape[1]) * 2 - 1  # Mapear de [0, 1] a [-1, 1]
            screen_y_normalized = (avg_pupil_y / frame.shape[0]) * 2 - 1  # Mapear de [0, 1] a [-1, 1]

            # Asegurar que estén en el rango [-1, 1] para evitar valores fuera de la pantalla
            screen_x_normalized = min(max(screen_x_normalized, -1), 1)
            screen_y_normalized = min(max(screen_y_normalized, -1), 1)

            # Imprimir la posición normalizada de la pantalla
            print(f'Posición en pantalla normalizada - X: {screen_x_normalized:.2f}, Y: {screen_y_normalized:.2f}')
            
            # Enviar la posición normalizada de la pantalla a Unity
            message = f'{screen_x_normalized},{screen_y_normalized}\n'
            s.sendto(message.encode('utf-8'), (HOST, PORT))
        else:
            print(f'No eyes detected')
            # Si no se detectan ojos, enviar un mensaje a Unity
            s.sendto(b'no_eyes_detected\n', (HOST, PORT))

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

webcam.release()
cv2.destroyAllWindows()

