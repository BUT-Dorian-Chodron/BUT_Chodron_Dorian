
#include <xc.h>

#include "IO.h"



void InitIO()

{

    // IMPORTANT : d�sactiver les entr�es analogiques, sinon on perd les entr�es num�riques

    ANSELA = 0; // 0 desactive

    ANSELB = 0;

    ANSELC = 0;

    ANSELD = 0;

    ANSELE = 0;

    ANSELF = 0;

    ANSELG = 0;



    //********** Configuration des sorties : _TRISxx = 0 ********************************

    // LED

    _TRISC10 = 0;  // LED Orange

    _TRISG6 = 0; //LED Blanche

    _TRISG7 = 0; // LED Bleue
    
    _TRISB14 = 0; //MOTEUR1 IN1
    _TRISB15 = 0; //MOTEUR1 IN2
    _TRISC6 = 0; //MOTEUR6 IN1
    _TRISC7 = 0; //MOTEUR6 IN2
    _U1RXR = 24; //remappe la RP24 sur l'entr�e RX1
    _RP36R=0b00001; //Remappe la sortie Tx1 vers RP36

    

    // Moteurs 



    //********** Configuration des entr�es : _TRISxx = 1 ********************************   

    

}
