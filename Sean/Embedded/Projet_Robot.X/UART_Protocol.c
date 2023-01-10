#include <xc.h>
#include "Uart_Protocol.h"
#include "CB_TX1.h"
#include "IO.h"
#include "PWM.h"
#define SET_ROBOT_STATE 0x0051
#define SET_ROBOT_MANUAL_CONTROL 0x0052

extern unsigned char autoControl;
extern unsigned char stateRobot;

int msgDecodedFunction = 0;
int msgDecodedPayloadLength = 0;
unsigned char msgDecodedPayload[128];
int msgDecodedPayloadIndex = 0;
int rcvState = Waiting;

unsigned char UartCalculateChecksum(int msgFunction, int msgPayloadLength, unsigned char* msgPayload) {
    unsigned char c = 0;
    c ^= 0xFE;
    c ^= (msgFunction >> 8);
    c ^= (msgFunction >> 0);
    c ^= (msgPayloadLength >> 8);
    c ^= (msgPayloadLength >> 0);
    for (int i = 0; i < msgPayloadLength; i++) {
        c ^= msgPayload[i];
    }
    return c;

}

void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, unsigned char* msgPayload) {
    unsigned char message[6 + msgPayloadLength ];
    int pos = 0;

    message[pos++] = 0xFE;
    message[pos++] = (msgFunction >> 8);
    message[pos++] = (msgFunction);
    message[pos++] = (msgPayloadLength >> 8);
    message[pos++] = (msgPayloadLength);
    for (int i = 0; i < msgPayloadLength; i++) {
        message[pos++] = msgPayload[i];
    }
    message[pos++] = UartCalculateChecksum(msgFunction, msgPayloadLength, msgPayload);

    SendMessage(message, pos);

}

void UartDecodeMessage(unsigned char c) {
    switch (rcvState) {
        case Waiting:

            if (c == 0xFE) rcvState = FunctionMSB;
            else rcvState = Waiting;
            break;

        case FunctionMSB:

            msgDecodedFunction = (int) (c << 8);
            rcvState = FunctionLSB;
            break;

        case FunctionLSB:

            msgDecodedFunction += (int) (c << 0);
            rcvState = PayloadLengthMSB;
            break;

        case PayloadLengthMSB:

            msgDecodedPayloadLength = (int) (c << 8);
            rcvState = PayloadLengthLSB;
            break;

        case PayloadLengthLSB:

            msgDecodedPayloadLength += (int) (c << 0);
            rcvState = Payload;

            msgDecodedPayloadIndex = 0;
            break;

        case Payload:

            msgDecodedPayload[msgDecodedPayloadIndex] = c;
            msgDecodedPayloadIndex++;

            if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
                rcvState = CheckSum;
            break;

        case CheckSum:



            if (UartCalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload) == c) {
                UartProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
            } else {

            }
            rcvState = Waiting;

            break;
        default:
            rcvState = Waiting;
            break;
    }


}

//void UartProcessDecodedMessage(unsigned char function, unsigned char payloadLength, unsigned char* payload) {
//    if (function == 0x0030)//telemetre
//    {
//        SendMessage(payload, payloadLength);
//    }
//    if (function == 0x0040) {
//        //PWMSetSpeedConsigne((float)playload[0],MoteurGauche);
//        //PWMSetSpeedConsigne((float)playload[1],MoteurDroit);
//    }
//    if (function == 0x0020) {
//        switch (payload[0]) {
//            case (0x01):
//                LED_BLANCHE = (int) payload[1];
//                break;
//
//            case (0x10):
//                LED_ORANGE = (int) payload[1];
//                break;
//
//            case (0x11):
//                LED_BLEUE = (int) payload[1];
//                break;
//
//        }
//    }
//}

void UartProcessDecodedMessage(unsigned char function,unsigned char payloadLength, unsigned char payload[]) {
    //Fonction éappele èaprs le édcodage pour éexcuter l?action
    //correspondant au message çreu
    switch (function) {
        case SET_ROBOT_STATE:

            SetRobotState(payload[0]);
            break;
        case SET_ROBOT_MANUAL_CONTROL:
            SetRobotAutoControlState(payload[0]);
            break;
        default:
            break;
    }
}

void SetRobotState(unsigned char state)
{
    stateRobot=state;
}

void SetRobotAutoControlState(unsigned char state)
{
    autoControl=state;
}