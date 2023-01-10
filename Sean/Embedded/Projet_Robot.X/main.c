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
#include "CB_RX1.h"
#include <libpic30.h>

#include "UART_Protocol.h"


int main(void) {
    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitIO();
    InitPWM();
    InitADC1();
    InitTimer4();
    InitUART();
    unsigned char stateRobot;
    while (1) {
        //SendMessage((unsigned char*) "Bonjour" ,7);
        //SendMessageDirect((unsigned char*) "Bonjour" ,7);
        //__delay32(4000000);
//        int i;
        unsigned char payload[3];
//        for (i = 0; i < CB_RX1_GetDataSize(); i++) {
//            unsigned char c = CB_RX1_Get();
//            SendMessage(&c, 1);
//        }
       // __delay32(10000);
        //unsigned char payload[3];
        //void SendStateSupervision()
        //{
            //unsigned long timestampCourant=timestamp;
            //unsigned char payload[]={stateRobot,(unsigned char )(timestampCourant>>24),(unsigned char)(timestampCourant>>16),(unsigned char)(timestampCourant>>8),(unsigned char)(timestampCourant>>4),(unsigned char)(timestampCourant>>0)};
            //UartEncodeAndSendMessage(0x0050,5,payload);
        //}
        int i;
       
         for (i = 0; i < CB_RX1_GetDataSize(); i++) 
         {
            UartDecodeMessage(CB_RX1_Get()); 
        }
        
       

        if (ADCIsConversionFinished() == 1) {
            ADCClearConversionFinishedFlag();
            unsigned int * result = ADCGetResult();
            float volts = ((float) result[2])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreCentre = 34 / volts - 5;
            volts = ((float) result[3])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroit = 34 / volts - 5;
            volts = ((float) result[0])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGaucheex = 34 / volts - 5;
            volts = ((float) result[4])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreDroitex = 34 / volts - 5;
            volts = ((float) result[1])*3.3 / 4096 * 3.2;
            robotState.distanceTelemetreGauche = 34 / volts - 5;
            for (int i=0;i<3;i++)
            {
                payload[i]=result[i+1];
            }
            //UartEncodeAndSendMessage(0x0030,3,payload);
            
            
           // __delay32(4000000);
        }
        
unsigned char autoControl = 0;

void OperatingSystemLoop(void) {
    switch (stateRobot) {
        case STATE_ATTENTE:
            timestamp = 0;
            PWMSetSpeedConsigne(0, MoteurDroit);
            PWMSetSpeedConsigne(0, MoteurGauche);
            stateRobot = STATE_ATTENTE_EN_COURS;

        case STATE_ATTENTE_EN_COURS:
            if (timestamp > 1000)
                stateRobot = STATE_AVANCE;
            break;

        case STATE_AVANCE:
            PWMSetSpeedConsigne(15, MoteurDroit);
            PWMSetSpeedConsigne(15, MoteurGauche);
            stateRobot = STATE_AVANCE_EN_COURS;
            break;
        case STATE_AVANCE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_GAUCHE:
            PWMSetSpeedConsigne(15, MoteurDroit);
            PWMSetSpeedConsigne(-15, MoteurGauche);
            stateRobot = STATE_TOURNE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_GAUCHE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_DROITE:
            PWMSetSpeedConsigne(-15, MoteurDroit);
            PWMSetSpeedConsigne(15, MoteurGauche);
            stateRobot = STATE_TOURNE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_DROITE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

            //        case STATE_RECULE:
            //            timestamp = 0;
            //            PWMSetSpeedConsigne(-15, moteur_droit);
            //            PWMSetSpeedConsigne(-15, moteur_gauche);
            //            stateRobot = STATE_RECULE_EN_COURS;
            //            break;
            //            
            //             case STATE_RECULE_EN_COURS:
            //            if (timestamp > 2000)
            //                stateRobot = STATE_TOURNE_SUR_PLACE_DROITE ;
            //            break;

        case STATE_TOURNE_SUR_PLACE_GAUCHE:
            PWMSetSpeedConsigne(15, MoteurDroit);
            PWMSetSpeedConsigne(-15, MoteurGauche);
            stateRobot = STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS:
            SetNextRobotStateInAutomaticMode();
            break;

        case STATE_TOURNE_SUR_PLACE_DROITE:
            PWMSetSpeedConsigne(-10, MoteurDroit);
            PWMSetSpeedConsigne(10, MoteurGauche);
            stateRobot = STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS;
            break;
        case STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS:
            if (autoControl)
                SetNextRobotStateInAutomaticMode();
            break;

        default:
            stateRobot = STATE_ATTENTE;
            break;

    }
}

//void SendStateSupervision() {
//    unsigned long timestampCourant = timestamp;
//    unsigned char payload[] = {stateRobot, (unsigned char) (timestampCourant >> 24), (unsigned char) (timestampCourant >> 16), (unsigned char) (timestampCourant >> 8), (unsigned char) (timestampCourant >> 0)};
//    UartEncodeAndSendMessage(0x0050, 5, payload);
//}

unsigned char nextStateRobot = 0;

void SetNextRobotStateInAutomaticMode(void) {
    unsigned char positionObstacle = PAS_D_OBSTACLE;

    //Détermination de la position des obstacles en fonction des télémètres
    if (robotState.distanceTelemetreDroit > 30 &&
            robotState.distanceTelemetreDroitex > 20 &&
            robotState.distanceTelemetreCentre > 30 &&
            robotState.distanceTelemetreGaucheex > 20 &&
            robotState.distanceTelemetreGauche > 30) //pas d?obstacle
        positionObstacle = PAS_D_OBSTACLE;
    else if (robotState.distanceTelemetreCentre <= 30)
        positionObstacle = OBSTACLE_EN_FACE;
    else if (robotState.distanceTelemetreDroit < 30 ||
            robotState.distanceTelemetreDroitex < 20)
        positionObstacle = OBSTACLE_A_DROITE;
    else if (robotState.distanceTelemetreGaucheex < 30 ||
            robotState.distanceTelemetreGauche < 20)
        positionObstacle = OBSTACLE_A_GAUCHE;


    //Détermination de l?état à venir du robot
    if (positionObstacle == PAS_D_OBSTACLE)
        nextStateRobot = STATE_AVANCE;
    else if (positionObstacle == OBSTACLE_A_DROITE)
        nextStateRobot = STATE_TOURNE_GAUCHE;
    else if (positionObstacle == OBSTACLE_A_GAUCHE)
        nextStateRobot = STATE_TOURNE_DROITE;
    else if (positionObstacle == OBSTACLE_EN_FACE)
        nextStateRobot = STATE_TOURNE_SUR_PLACE_DROITE;

    //Si l?on n?est pas dans la transition de l?étape en cours
    if (nextStateRobot != stateRobot - 1) {
        stateRobot = nextStateRobot;
        //SendStateSupervision();
    }
}

    }
}