#include <stdio.h>
#include <stdlib.h>
#include "timer.h"
#include "ChipConfig.h"
#include "IO.h"
#include <xc.h>
#include "PWM.h"
#include "robot.h"
#include "ADC.h"

#define MoteurDroit 0
#define MoteurGauche 1
int ADCvalue0;
int ADCvalue1;
int ADCvalue2;

int main(void) {

    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitIO();
    InitPWM();
    InitADC1();

//PWMSetSpeedConsigne(30.0,MoteurGauche);
//PWMSetSpeedConsigne(30.0,MoteurDroit);
    
   
    
 
    

    while (1) {  
   if ( ADCIsConversionFinished()==1)
{
ADCClearConversionFinishedFlag();
unsigned int * result = ADCGetResult();
float volts =((float)result[2])*3.3/4096*3.2 ;
robotState.distanceTelemetreDroit = 34/volts - 5 ;
volts=((float)result[1])*3.3/4096*3.2 ;
robotState.distanceTelemetreGauche = 34/volts - 5 ;
volts=((float)result[0])*3.3/4096*3.2 ;
robotState.distanceTelemetreGauche = 34/volts - 5 ;


}
 // unsigned int * result = ADCGetResult();
//   if (ADCIsConversionFinished())
//   {
//       ADCClearConversionFinishedFlag();
//       ADCvalue0=result[0];
//       ADCvalue1=result[1];
//       ADCvalue2=result[2];
       
  // }
//   if (ADCvalue0>0x303)LED_ORANGE=1;
//   else LED_ORANGE=0;
    
  
}
}