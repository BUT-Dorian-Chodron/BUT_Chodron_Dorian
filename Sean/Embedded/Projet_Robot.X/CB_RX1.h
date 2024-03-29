/* 
 * File:   CB_RX1.h
 * Author: GEII Robot
 *
 * Created on 5 d�cembre 2022, 15:10
 */

#ifndef CB_RX1_H
#define	CB_RX1_H
void CB_RX1_Add(unsigned char value);
unsigned char CB_RX1_Get(void);
unsigned char CB_RX1_IsDataAvailable(void);
void __attribute__((interrupt, no_auto_psv)) _U1RXInterrupt(void);
int CB_RX1_GetRemainingSize(void);
int CB_RX1_GetDataSize(void);


#endif	/* CB_RX1_H */

