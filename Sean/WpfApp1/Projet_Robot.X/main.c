#include <stdio.h>
#include <stdlib.h>
#include "timer.h"
#include "ChipConfig.h"
#include "IO.h"
#include <xc.h>
#include "PWM.h"
#include "robot.h"
#include "ADC.h"
#include "main.h"
#include "UART.h"
#include "CB_TX1.h"

int yohan=0;
unsigned char stateRobot;
/*void OperatingSystemLoop(void)
{
switch (stateRobot)
{
case STATE_ATTENTE:
timestamp = 0;
PWMSetSpeedConsigne(0, MOTEUR_DROIT);
PWMSetSpeedConsigne(0, MOTEUR_GAUCHE);
stateRobot = STATE_ATTENTE_EN_COURS;

case STATE_ATTENTE_EN_COURS:
if (timestamp > 1000)
stateRobot = STATE_AVANCE;
break;

case STATE_AVANCE:
PWMSetSpeedConsigne(20, MOTEUR_DROIT);
PWMSetSpeedConsigne(20, MOTEUR_GAUCHE);
stateRobot = STATE_AVANCE_EN_COURS;
break;
case STATE_AVANCE_EN_COURS:
SetNextRobotStateInAutomaticMode();
break;

case STATE_TOURNE_GAUCHE:
PWMSetSpeedConsigne(20, MOTEUR_DROIT);
PWMSetSpeedConsigne(0, MOTEUR_GAUCHE);
stateRobot = STATE_TOURNE_GAUCHE_EN_COURS;
break;
case STATE_TOURNE_GAUCHE_EN_COURS:
SetNextRobotStateInAutomaticMode();
break;

case STATE_TOURNE_DROITE:
PWMSetSpeedConsigne(0, MOTEUR_DROIT);
PWMSetSpeedConsigne(20, MOTEUR_GAUCHE);
stateRobot = STATE_TOURNE_DROITE_EN_COURS;
break;
case STATE_TOURNE_DROITE_EN_COURS:
SetNextRobotStateInAutomaticMode();
break;

case STATE_TOURNE_SUR_PLACE_GAUCHE:
PWMSetSpeedConsigne(15, MOTEUR_DROIT);
PWMSetSpeedConsigne(-15, MOTEUR_GAUCHE);
stateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS;
break;
case STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS:
SetNextRobotStateInAutomaticMode();
break;

case STATE_TOURNE_SUR_PLACE_DROITE:
PWMSetSpeedConsigne(-15, MOTEUR_DROIT);
PWMSetSpeedConsigne(15, MOTEUR_GAUCHE);
stateRobot = STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS;
break;
case STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS:
SetNextRobotStateInAutomaticMode();
break;
case STATE_TOURNE_SUR_PLACE:
            PWMSetSpeedConsigne(15, MOTEUR_DROIT);
            PWMSetSpeedConsigne(15, MOTEUR_GAUCHE);
            stateRobot = STATE_TOURNE_SUR_PLACE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;
default :
stateRobot = STATE_ATTENTE;
break;

}

}

unsigned char nextStateRobot=0;

void SetNextRobotStateInAutomaticMode()
{
  
unsigned char positionObstacle = PAS_D_OBSTACLE;
if ((robotState.distanceTelemetreDroitex>30) && (robotState.distanceTelemetreGaucheex>30))
{
    LED_BLEUE=0;
//Détermination de la position des obstacles en fonction des télémètres
if ( robotState.distanceTelemetreDroit < 30 &&
robotState.distanceTelemetreCentre > 20 &&
robotState.distanceTelemetreGauche > 30) //Obstacle à droite
positionObstacle = OBSTACLE_A_DROITE;
else if(robotState.distanceTelemetreDroit > 30 &&
robotState.distanceTelemetreCentre > 20 &&
robotState.distanceTelemetreGauche < 30) //Obstacle à gauche
positionObstacle = OBSTACLE_A_GAUCHE;
else if(robotState.distanceTelemetreCentre < 20) //Obstacle en face
positionObstacle = OBSTACLE_EN_FACE;
else if(robotState.distanceTelemetreDroit > 30 &&
robotState.distanceTelemetreCentre > 20 &&
robotState.distanceTelemetreGauche > 30) //pas d?obstacle
positionObstacle = PAS_D_OBSTACLE;
}
else
{
if (robotState.distanceTelemetreGaucheex<30)yohan=yohan+16;
if (robotState.distanceTelemetreGauche<30)yohan=yohan+8;
if (robotState.distanceTelemetreCentre<30)yohan=yohan+4;
if (robotState.distanceTelemetreDroit<30)yohan=yohan+2;
if (robotState.distanceTelemetreDroitex<30)yohan=yohan+1;
LED_BLEUE=1;
switch (yohan) {
     
        case 0b00001:
            nextStateRobot = STATE_TOURNE_GAUCHE;
            break;
        
        case 0b00011:
            nextStateRobot = STATE_TOURNE_GAUCHE;
            break;
        
        case 0b00101:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
            break;
        
        case 0b00111:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
            break;
        
        case 0b01001:
            nextStateRobot = STATE_TOURNE_DROITE;
            break;
        
        case 0b01011:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
            break;
        
        case 0b01101:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
            break;
        
        case 0b01111:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
            break;
        case 0b10000:
            nextStateRobot = STATE_TOURNE_DROITE;
            break;
        case 0b10001:
            nextStateRobot = STATE_AVANCE;
            break;
        case 0b10010:
            nextStateRobot = STATE_TOURNE_GAUCHE;
            break;
        case 0b10011:
            nextStateRobot = STATE_TOURNE_GAUCHE;
            break;
        case 0b10100:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
            break;
        case 0b10101:
            nextStateRobot = STATE_TOURNE_SUR_PLACE;
            break;
        case 0b10110:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
            break;
        case 0b10111:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;
            break;
        case 0b11000:
            nextStateRobot = STATE_TOURNE_DROITE;
            break;
        case 0b11001:
            nextStateRobot = STATE_TOURNE_DROITE;
            break;
        case 0b11010:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
            break;
        case 0b11011:
            nextStateRobot = STATE_TOURNE_SUR_PLACE;
            break;
        case 0b11100:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
            break;
        case 0b11101:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
            break;
        case 0b11110:
            nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;
            break;
        case 0b11111:
            nextStateRobot = STATE_TOURNE_SUR_PLACE;
            break;
            
        default:
            stateRobot = STATE_ATTENTE;
            break;
    }
}
    
//Détermination de l?état à venir du robot
if (positionObstacle == PAS_D_OBSTACLE)
nextStateRobot = STATE_AVANCE;
else if (positionObstacle == OBSTACLE_A_DROITE)
nextStateRobot = STATE_TOURNE_GAUCHE;
else if (positionObstacle == OBSTACLE_A_GAUCHE)
nextStateRobot = STATE_TOURNE_DROITE;
else if (positionObstacle == OBSTACLE_EN_FACE)
nextStateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE;

//Si l?on n?est pas dans la transition de l?étape en cours
if (nextStateRobot != stateRobot - 1)
stateRobot = nextStateRobot;

}*/

int main(void) {
    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitIO();
    InitPWM();
    InitADC1();
    InitTimer4();
    InitUART();
    
  
    

    while (1) {  
     SendMessage((unsigned char*) "Bonjour" ,7);
    //SendMessageDirect((unsigned char*) "Bonjour" ,7);
    __delay32(4000000);
    
    
   if ( ADCIsConversionFinished()==1)
{
ADCClearConversionFinishedFlag();
unsigned int * result = ADCGetResult();
float volts =((float)result[2])*3.3/4096*3.2 ;
robotState.distanceTelemetreCentre = 34/volts - 5 ;
volts=((float)result[3])*3.3/4096*3.2 ;
robotState.distanceTelemetreDroit = 34/volts - 5 ;
volts=((float)result[0])*3.3/4096*3.2 ;
robotState.distanceTelemetreGaucheex = 34/volts - 5 ;
volts =((float)result[4])*3.3/4096*3.2 ;
robotState.distanceTelemetreDroitex = 34/volts - 5 ;
volts =((float)result[1])*3.3/4096*3.2 ;
robotState.distanceTelemetreGauche = 34/volts - 5 ;


 
 

    }
}
}