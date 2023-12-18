import cv2
import numpy as np
img = cv2.imread("Ballon.jpg")
hauteur = 1000
largeur = 1000

img = cv2.resize(img,(largeur,hauteur), interpolation=cv2.INTER_AREA)
#cv2.imshow('RoboCup␣image', img)
cv2.waitKey(0)


imagehsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
H, S, V = cv2.split(imagehsv)
#cv2.imshow("Hue", H)
cv2.waitKey(0)
#cv2.imshow("Saturation", S)
cv2.waitKey(0)
#cv2.imshow("Value", V)
cv2.waitKey(0)

hauteur = img.shape[0]
largeur = img.shape[1]

#Filtre Jaune
lower_yellow = np.array([20, 100, 100]) #code hsv
upper_yellow = np.array([30,255,255])
imagemaskyellow = cv2.inRange(imagehsv, lower_yellow, upper_yellow)
yellow_filtered_img = cv2.bitwise_and(img, img, mask=imagemaskyellow)
cv2.imshow("Image␣Masque␣Jaune", yellow_filtered_img)
cv2.waitKey(0)

image_jaune = np.zeros((hauteur, largeur, 3), dtype=np.uint8)
image_jaune[:, :, 1] = 255
image_jaune[:, :, 2] = 255

yellow_filtered_img = cv2.bitwise_and(image_jaune, image_jaune, mask=imagemaskyellow)
#cv2.imshow("Image jaune", yellow_filtered_img)

gris = cv2.cvtColor(yellow_filtered_img, cv2.COLOR_BGR2GRAY)

kernel = np.ones((5,5), np.uint8)
gris = cv2.erode(gris, kernel, iterations=1)
gris = cv2.dilate(gris, kernel, iterations=2)

# Trouver les contours dans l'image
contours, _ = cv2.findContours(gris, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Parcourir les contours
for contour in contours:
    # Ignorer les petits contours
    if cv2.contourArea(contour) > 500:  # 500 est une valeur à ajuster en fonction de votre image
        # Calculer le cercle englobant du contour
        ((x, y), rayon) = cv2.minEnclosingCircle(contour)

        # Vérifier si le contour est suffisamment circulaire
        area = cv2.contourArea(contour)
        perimeter = cv2.arcLength(contour, True)
        circularity = 4 * np.pi * (area / (perimeter * perimeter))
        if 0.8 <= circularity <= 1.2:  # Ces valeurs peuvent nécessiter un ajustement
            cv2.circle(img, (int(x), int(y)), int(rayon), (0, 255, 0), 2)

# Afficher l'image avec le cercle autour du ballon
cv2.imshow('Suivi du Ballon', img)

