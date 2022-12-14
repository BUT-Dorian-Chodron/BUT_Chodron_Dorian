/* 
 * File:   ToolBox.h
 * Author: GEII Robot
 *
 * Created on 12 septembre 2022, 09:57
 */

#ifndef TOOLBOX_H
#define	TOOLBOX_H
#define PI 3.141592653589793
float DegreeToRadian(float value);
float RadianToDegree(float value);
float LimitToInterval(float value, float lowLimit, float highLimit);
float Min(float value, float value2);
float Max(float value, float value2);
float Abs(float value);

#endif	/* TOOLBOX_H */

