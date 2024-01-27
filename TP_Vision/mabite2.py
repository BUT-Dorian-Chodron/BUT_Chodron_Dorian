import cv2
print(cv2.__version__)
import numpy as np
img = cv2.imread("robocup.png")
img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
#cv2.imshow('RoboCup␣image', img)
cv2.waitKey(0)
img_blur = cv2.GaussianBlur(img,(3,3),0)

kernel1 = np.array([[-1, -2, -1],
[0, 0, 0],
[1, 2, 1]])
identity = cv2.filter2D(src=img_blur, ddepth=-1, kernel=kernel1)

edges = cv2.Canny(image=img_blur, threshold1=100, threshold2=500)
cv2.imshow("Canny Edge Detection", edges)
cv2.waitKey(0)
#cv2.imshow("Sobel X Y using Sobel() function", identity)
cv2.waitKey(0)
