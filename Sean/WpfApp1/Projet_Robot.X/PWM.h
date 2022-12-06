/* 
 * File:   PWM.h
 * Author: GEII Robot
 *
 * Created on 12 septembre 2022, 09:40
 */

#ifndef PWM_H
#define	PWM_H
#define MoteurDroit 0
#define MoteurGauche 1
void InitPWM(void);
//void PWMSetSpeed(float vitesseEnPourcents,int moteur);
void PWMUpdateSpeed();
void PWMSetSpeedConsigne(float vitesseEnPourcents,int moteur);
#endif
