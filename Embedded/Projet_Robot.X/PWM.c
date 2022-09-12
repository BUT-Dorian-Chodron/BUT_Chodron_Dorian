//Partie PWM
#include <xc.h> // library xc.h inclut tous les uC
#include "IO.h"
#include "PWM.h"
#include "Robot.h"
#include "ToolBox.h"

#define PWMPER 40.0

#define Moteur_droit 0
#define Moteur_gauche 1
unsigned char acceleration = 20;

void InitPWM(void)
{
PTCON2bits.PCLKDIV = 0b000; //Divide by 1
PTPER = 100*PWMPER; //P�riode en pourcentage

//R�glage PWM moteur 1 sur hacheur 1
IOCON1bits.POLH = 1; //High = 1 and active on low =0
IOCON1bits.POLL = 1; //High = 1
IOCON1bits.PMOD = 0b01; //Set PWM Mode to Redundant
FCLCON1 = 0x0003; //D�sactive la gestion des faults

//Reglage PWM moteur 2 sur hacheur 6
IOCON6bits.POLH = 1; //High = 1
IOCON6bits.POLL = 1; //High = 1
IOCON6bits.PMOD = 0b01; //Set PWM Mode to Redundant
FCLCON6 = 0x0003; //D�sactive la gestion des faults

/* Enable PWM Module */
PTCONbits.PTEN = 1;
}

void PWMSetSpeed(float vitesseEnPourcents,int moteur)
{

robotState.vitesseGaucheCommandeCourante = vitesseEnPourcents;
robotState.vitesseDroiteCommandeCourante = vitesseEnPourcents;

if (vitesseEnPourcents>=0){
   if (moteur==Moteur_droit)
    {
    MOTEUR_DROIT_L_PWM_ENABLE = 1; //Pilotage de la pin en mode IO
    MOTEUR_DROIT_L_IO_OUTPUT = 1; //Mise � 1 de la pin
    MOTEUR_DROIT_H_IO_OUTPUT = 0; //Mise � 0 de la pin
    MOTEUR_DROIT_H_PWM_ENABLE = 0;//Pilotage de la pin en mode PWM
    MOTEUR_DROIT_DUTY_CYCLE =Abs(robotState.vitesseGaucheCommandeCourante*PWMPER);
   }
   else if(moteur==Moteur_gauche)
   {
MOTEUR_GAUCHE_L_PWM_ENABLE = 0; //Pilotage de la pin en mode IO
MOTEUR_GAUCHE_L_IO_OUTPUT = 1; //Mise � 1 de la pin
MOTEUR_GAUCHE_H_IO_OUTPUT = 0; //Mise � 0 de la pin
MOTEUR_GAUCHE_H_PWM_ENABLE = 1; //Pilotage de la pin en mode PWM
MOTEUR_GAUCHE_DUTY_CYCLE = Abs(robotState.vitesseDroiteCommandeCourante*PWMPER);
   }

}

else if (vitesseEnPourcents<0)
{if (moteur==Moteur_droit)
    {
    MOTEUR_DROIT_L_PWM_ENABLE = 1; //Pilotage de la pin en mode IO
    MOTEUR_DROIT_L_IO_OUTPUT = 0; //Mise � 1 de la pin
    MOTEUR_DROIT_H_IO_OUTPUT = 1; //Mise � 0 de la pin
    MOTEUR_DROIT_H_PWM_ENABLE = 0; //Pilotage de la pin en mode PWM
    MOTEUR_DROIT_DUTY_CYCLE =Abs(robotState.vitesseDroiteCommandeCourante*PWMPER);
   }
   else if(moteur==Moteur_gauche)
   {
MOTEUR_GAUCHE_L_PWM_ENABLE = 1; //Pilotage de la pin en mode IO
MOTEUR_GAUCHE_L_IO_OUTPUT = 1; //Mise � 1 de la pin
MOTEUR_GAUCHE_H_IO_OUTPUT = 0; //Mise � 0 de la pin
MOTEUR_GAUCHE_H_PWM_ENABLE = 0; //Pilotage de la pin en mode PWM
MOTEUR_GAUCHE_DUTY_CYCLE = Abs(robotState.vitesseGaucheCommandeCourante*PWMPER);
   }
}
}