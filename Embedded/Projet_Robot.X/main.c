#include <stdio.h>
#include <stdlib.h>
#include "timer.h"
#include "ChipConfig.h"
#include "IO.h"
#include <xc.h>
#include "PWM.h"

#define Moteur_droit 0
#define Moteur_gauche 1

int main(void) {

    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitIO();
    InitPWM();
 

  PWMSetSpeed(30,Moteur_gauche);
  PWMSetSpeed(50,Moteur_droit);
    
    
    
    
    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;

    while (1) {     
   
            }
}
