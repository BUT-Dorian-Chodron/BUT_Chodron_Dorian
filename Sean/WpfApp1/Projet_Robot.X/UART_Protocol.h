
#ifndef UART_PROTOCOL_H
#define	UART_PROTOCOL_H
#define Waiting 0
#define FunctionLSB 1
#define FunctionMSB 2
#define PayloadLengthMSB 3
#define PayloadLengthLSB 4
#define Payload 5
#define CheckSum 6

unsigned char UartCalculateChecksum(int msgFunction,int msgPayloadLength, unsigned char* msgPayload);
void UartEncodeAndSendMessage(int msgFunction,int msgPayloadLength, unsigned char* msgPayload);
void UartDecodeMessage(unsigned char c);
void UartProcessDecodedMessage(unsigned char function,unsigned char payloadLength, unsigned char* payload);

#endif	/* UART_PROTOCOL_H */

