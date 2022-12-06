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
        //SendMessage((unsigned char*) "Bonjour" ,7);
        //SendMessageDirect((unsigned char*) "Bonjour" ,7);
        //__delay32(4000000);
        int i;
        for (i = 0; i < CB_RX1_GetDataSize(); i++) {
            unsigned char c = CB_RX1_Get();
            SendMessage(&c, 1);
        }
        __delay32(10000);

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
        }
    }
}