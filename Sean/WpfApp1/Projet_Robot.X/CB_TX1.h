/* 
 * File:   CB_TX1.h
 * Author: GEII Robot
 *
 * Created on 24 novembre 2022, 11:36
 */

#ifndef CB_TX1_H
#define	CB_TX1_H

void SendMessage(unsigned char* message, int length);
void CB_TX1_Add(unsigned char value);
unsigned char CB_TX1_Get(void);
void __attribute__((interrupt, no_auto_psv)) _U1TXInterrupt(void);
void SendOne();
unsigned char CB_TX1_IsTranmitting(void);
int CB_TX1_RemainingSize(void);
int CB_TX1_DataSize(void);

#endif	/* CB_TX1_H */

