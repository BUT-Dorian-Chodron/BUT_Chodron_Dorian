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
//        for (i = 0; i < CB_RX1_GetDataSize(); i++) {
//            unsigned char c = CB_RX1_Get();
//            SendMessage(&c, 1);
//        }
       // __delay32(10000);
        unsigned char payload[3];
        void SendStateSupervision()
        {
            unsigned long timestampCourant=timestamp;
            unsigned char payload[]={stateRobot,(unsigned char )(timestampCourant>>24),(unsigned char)(timestampCourant>>16),(unsigned char)(timestampCourant>>8),(unsigned char)(timestampCourant>>4),(unsigned char)(timestampCourant>>0)};
            UartEncodeAndSendMessage(0x0050,5,payload);
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
            UartEncodeAndSendMessage(0x0030,3,payload);
            
            
            __delay32(4000000);
        }
    }
}