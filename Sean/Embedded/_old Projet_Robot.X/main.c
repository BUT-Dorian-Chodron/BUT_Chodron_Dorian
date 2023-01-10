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
int ADCvalue3;
int ADCvalue4;
float vitesse;
int moteur;

int main(void) {

    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitIO();
    InitPWM();
    InitADC1();
    PWMSetSpeed(vitesse, moteur);
    PWMSetSpeedConsigne(vitesse, moteur);

//PWMSetSpeedConsigne(0.0,MoteurGauche);
//PWMSetSpeedConsigne(0.0,MoteurDroit);
    
   PWMSetSpeed(40.0,MoteurGauche);
   PWMSetSpeed(40.0,MoteurDroit);
    

    while (1) {  
/*   if ( ADCIsConversionFinished()==1)
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
  unsigned int * result = ADCGetResult();
   if (ADCIsConversionFinished())
   {
       ADCClearConversionFinishedFlag();
      ADCvalue0=result[0];
      ADCvalue1=result[1];
      ADCvalue2=result[2];
      ADCvalue3=result[3];
      ADCvalue4=result[4];
       
   }
   if (ADCvalue0>0x303)LED_ORANGE=1;
   else LED_ORANGE=0;
  
  if (ADCvalue2>0x303)LED_BLANCHE=1;
   else LED_BLANCHE=0;
  
  if (ADCvalue1>0x303)LED_BLEUE=1;
  else LED_BLEUE=0;  
  
  
  //if ((ADCvalue0>=1280) and (ADCvalue1>=1280) and (ADCvalue2>=1280) {
      
 // }
 */
}
}