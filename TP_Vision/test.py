import cv2
print(cv2.__version__)
import numpy as np
img = cv2.imread("robocup.png")
#cv2.imshow('RoboCup␣image', img)
cv2.waitKey(0)
img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
B, G, R = cv2.split(img)
blue_img = cv2.merge([B, np.zeros_like(B), np.zeros_like(B)])
##cv2.imshow("original", img)
##cv2.waitKey(0)
#cv2.imshow("blue", blue_img)
##cv2.waitKey(0)
##cv2.imshow("Green", G)
##cv2.waitKey(0)
##cv2.imshow("Red", R)
##cv2.waitKey(0)

imagehsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
H, S, V = cv2.split(imagehsv)
#cv2.imshow("Hue", H)
#cv2.waitKey(0)
#cv2.imshow("Saturation", S)
#cv2.waitKey(0)
##cv2.imshow("Value", V)
cv2.waitKey(0)

hauteur = img.shape[0]
largeur = img.shape[1]

#Filtre Jaune
lower_yellow = np.array([80, 100, 100]) #code hsv
upper_yellow = np.array([100,255,255])
imagemaskyellow = cv2.inRange(imagehsv, lower_yellow, upper_yellow)
yellow_filtered_img = cv2.bitwise_and(img, img, mask=imagemaskyellow)
#cv2.imshow("Image␣Masque␣Jaune", yellow_filtered_img)
cv2.waitKey(0)

image_jaune = np.zeros((hauteur, largeur, 3), dtype=np.uint8)
image_jaune[:, :, 1] = 255
image_jaune[:, :, 2] = 255

yellow_filtered_img = cv2.bitwise_and(image_jaune, image_jaune, mask=imagemaskyellow)
#cv2.imshow("Image jaune", yellow_filtered_img)

#Filtre Vert
lower_green = np.array([35, 50, 50])
upper_green = np.array([85,255,255])
imagemaskgreen = cv2.inRange(imagehsv, lower_green, upper_green)
green_filtered_img = cv2.bitwise_and(img, img, mask=imagemaskgreen)
#cv2.imshow("Image␣Masque␣Vert", green_filtered_img)
cv2.waitKey(0)
# Créer une image vide avec une couleur verte

image_verte = np.zeros((hauteur, largeur, 3), dtype=np.uint8)
image_verte[:, :, 1] = 255  # Canal vert à 255 (vert pur)
green_filtered_img = cv2.bitwise_and(image_verte, image_verte, mask=imagemaskgreen)

# Afficher l'image verte
#cv2.imshow("Image verte", green_filtered_img)
#Filtre Bleu

lower_blue = np.array([0, 0, 0])
upper_blue = np.array([179,255,30])
imagemaskblue = cv2.inRange(imagehsv, lower_blue, upper_blue)
blue_filtered_img = cv2.bitwise_and(img, img, mask=imagemaskblue)
cv2.waitKey(0)

image_bleu = np.zeros((hauteur, largeur, 3), dtype=np.uint8)
image_bleu[:, :, 0] = 255
blue_filtered_img = cv2.bitwise_and(image_bleu, image_bleu, mask=imagemaskblue)
#cv2.imshow("Image bleu", blue_filtered_img)

#Filtre bleu+vert+jaune
alpha1 = 1
alpha2 = 1
alpha3 = 1
composite_image = cv2.addWeighted(blue_filtered_img, alpha1, green_filtered_img, alpha2, 0.0)
composite_image = cv2.addWeighted(composite_image, 1.0, yellow_filtered_img, alpha3, 0.0)
cv2.imshow("Image filtrée", composite_image)
img = cv2.imread("robocup.png")
#cv2.imshow('RoboCup␣image', img)
cv2.waitKey(0)
#img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
#TRANSFORMATIONS MANUELLES D’UNE IMAGE
height = img.shape[0]
width = img.shape[1]
channels = img.shape[2]
imgTransform = img
for x in range(0, (int)(width)):
    for y in range (0, (int)(height)):
        imgTransform[y,x][0] *= 0.5
        imgTransform[y,x][1] *= 0.5
        imgTransform[y,x][2] *= 0.5
##        grayscale_value = x / width  # Calcul du gradient de gris
##        imgTransform[y, x][0] *= grayscale_value
##        imgTransform[y, x][1] *= grayscale_value
##        imgTransform[y, x][2] *= grayscale_value
####
cv2.imshow("Transformation_manuelle_de_l’image", imgTransform)
##
### Définir le centre et le rayon du cercle
##centre_x, centre_y = width // 2, height // 2
##rayon = min(width, height) // 4  # Par exemple, un quart de la plus petite dimension de l'image
##
### Couleur du cercle (rouge) et facteur de transparence
##couleur_cercle = [0, 0, 255]  # Rouge
##alpha = 0.5  # Facteur de transparence (0 = transparent, 1 = opaque)
##
##for x in range(0, int(width)):
##    for y in range(0, int(height)):
##        # Calcul de la distance du pixel par rapport au centre
##        distance = ((x - centre_x) ** 2 + (y - centre_y) ** 2) ** 0.5
##        if distance < rayon:
##           imgTransform[y,x][0] *= 0
##           imgTransform[y,x][1] *= 0
##           imgTransform[y,x][2] *= 1
##    
##
##        
##cv2.imshow("Transformation_manuelle_de_l’image", imgTransform)
cv2.waitKey(0)

