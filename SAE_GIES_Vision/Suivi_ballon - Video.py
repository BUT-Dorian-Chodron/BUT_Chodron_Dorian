import cv2
import numpy as np

print(cv2.__version__)

# Remplacez le chemin du fichier par celui de votre vidéo
cap = cv2.VideoCapture('Video_ballon.mp4')

while cap.isOpened():
    ret, img = cap.read()
    if not ret:
        break

    hauteur = 1000
    largeur = 1000
    img = cv2.resize(img,(largeur,hauteur), interpolation=cv2.INTER_AREA)

    imagehsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)

    # Filtre Jaune
    lower_yellow = np.array([20, 100, 100])
    upper_yellow = np.array([30,255,255])
    imagemaskyellow = cv2.inRange(imagehsv, lower_yellow, upper_yellow)

    image_jaune = np.zeros((hauteur, largeur, 3), dtype=np.uint8)
    image_jaune[:, :, 1] = 255
    
    image_jaune[:, :, 2] = 255
    yellow_filtered_img = cv2.bitwise_and(image_jaune, image_jaune, mask=imagemaskyellow)

    gris = cv2.cvtColor(yellow_filtered_img, cv2.COLOR_BGR2GRAY)

    kernel = np.ones((5,5), np.uint8)
    gris = cv2.erode(gris, kernel, iterations=1)
    gris = cv2.dilate(gris, kernel, iterations=2)

    # Trouver les contours dans l'image
    contours, _ = cv2.findContours(gris, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    # Parcourir les contours
    for contour in contours:
        if cv2.contourArea(contour) > 500:
            ((x, y), rayon) = cv2.minEnclosingCircle(contour)
            area = cv2.contourArea(contour)
            perimeter = cv2.arcLength(contour, True)
            circularity = 4 * np.pi * (area / (perimeter * perimeter))
            if 0.2 <= circularity <= 1.2:
                cv2.circle(img, (int(x), int(y)), int(rayon), (0, 255, 0), 2)

    # Afficher l'image avec le cercle autour du ballon
    cv2.imshow('Suivi du Ballon', img)

    # Pour arrêter la vidéo, appuyez sur la touche 'q'
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
