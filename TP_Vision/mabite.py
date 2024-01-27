# importing library for plotting
from matplotlib import pyplot as plt
import cv2
print(cv2.__version__)
import numpy as np
img = cv2.imread("robocup.png")
#cv2.imshow('RoboCup␣image', img)
cv2.waitKey(0)
img = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
#Conversion de l’image en niveaux de gris
imageGray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
cv2.imshow("Grayscale", imageGray)
#Calcul de l’histogramme
hist,bins = np.histogram(imageGray.flatten(),256,[0,256])
#Calcul de l’histogramme cumule
cdf = hist.cumsum()
cdf_normalized = cdf * float(hist.max()) / cdf.max()
#Affichage de l’histogramme cumule en bleu
plt.plot(cdf_normalized, color = "b")
#Affichage de l’histogramme en rouge
plt.hist(imageGray.flatten(),256,[0,256], color = "r")
plt.xlim([0,256])
plt.legend(("cdf","histogram"), loc = "upper left")
plt.show()

#Egalisation de l’histogramme
imgEqu = cv2.equalizeHist(imageGray)
#Calcul de l’histogramme egalise
histEq,binsEq = np.histogram(imgEqu.flatten(),256,[0,256])
#Calcul de l’histogramme écumul éégalis
cdfEq = histEq.cumsum()
cdfEq_normalized = cdfEq * float(histEq.max()) / cdfEq.max()
#Affichage de l’image egalisee
cv2.imshow("Image éégalise", imgEqu)
cv2.waitKey(0)
plt.clf()

#Affichage de l’histogramme egalise cumule en bleu
plt.plot(cdfEq_normalized, color = "b")
#Affichage de l’histogramme egalise en rouge
plt.hist(imgEqu.flatten(),256,[0,256], color = "r")
plt.xlim([0,256])
plt.legend(("cdfEq","histogramEq"), loc = "upper left")
plt.show()
cv2.waitKey(0)
