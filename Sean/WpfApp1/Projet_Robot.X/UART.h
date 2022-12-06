/* 
 * File:   UART.h
 * Author: GEII Robot
 *
 * Created on 17 novembre 2022, 10:12
 */

#ifndef UART_H
#define	UART_H


void InitUART(void);
void SendMessageDirect(unsigned char* message, int length);
void __attribute__((interrupt, no_auto_psv)) _U1RXInterrupt(void);





#endif	/* UART_H */

