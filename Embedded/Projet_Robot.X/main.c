#include <stdio.h>
#include <stdlib.h>
#include "timer.h"
#include "ChipConfig.h"
#include "IO.h"
#include <xc.h>

int main(void) {

    InitOscillator();
    InitTimer23();
    InitTimer1();
    InitIO();
    
    
    LED_BLANCHE = 1;
    LED_BLEUE = 1;
    LED_ORANGE = 1;

    while (1) {     
   
            }
}
