﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtendedSerialPort;
using System.Windows.Threading;

namespace WpfApp1
{
    
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReliableSerialPort serialPort1;
        DispatcherTimer timerAffichage;
        Robot robot = new Robot();

        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM9", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();
           



        }

        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            //if (robot.receivedText != "")
            //{
            //   textBoxReception.Text += "Utilisation Serial port : "+robot.receivedText + "\n";
            //   robot.receivedText = "";
            // }
            
                while(robot.byteListReceived.Count()>0)
                {
                    var c=robot.byteListReceived.Dequeue();
                   textBoxReception.Text += "0x"+c.ToString("X2")+" ";
                    
                }
               
            
        }

        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            //textBoxReception.Text += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            foreach (var c in e.Data)
           {
                robot.byteListReceived.Enqueue(c);
            }
        }


        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            if (buttonEnvoyer.Background == Brushes.RoyalBlue)
            {
                buttonEnvoyer.Background = Brushes.Beige;
            }
            else buttonEnvoyer.Background = Brushes.RoyalBlue;

            sendMessage();

            

        }
        void sendMessage()
        {
           serialPort1.WriteLine(textBoxEmission.Text);
            textBoxReception.Text = "Reçu : " + textBoxEmission.Text + "\n";
            textBoxEmission.Text = "";

        }

        private void textBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                sendMessage();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            textBoxEmission.Text = "";
            textBoxReception.Text = "";

        }
        byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte c = 0x00;
            c ^= 0xFE;
            c ^= (byte)(msgFunction >> 8);
            c ^= (byte)(msgFunction >> 0);
            c ^= (byte)(msgPayloadLength >> 8);
            c ^= (byte)(msgPayloadLength >> 0);
            for (int i = 0; i < msgPayloadLength; i++)
            {
                c ^= (byte)(msgPayload[i]);
            }
            return c;
        }
        void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            //Encode
            byte[] message = new byte[msgPayloadLength + 6];
            int pos = 0;

            message[pos++] = 0xFE;
            message[pos++] = (byte)(msgFunction >> 8);
            message[pos++] = (byte)(msgFunction);
            message[pos++] = (byte)(msgPayloadLength >> 8);
            message[pos++] = (byte)(msgPayloadLength);
            for (int i = 0; i < msgPayloadLength; i++)
            {
                message[pos++] = msgPayload[i];
            }
            message[pos++] = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);

            //Send message
            serialPort1.Write(message, 0, pos);


        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            string s = "Bonjour";
            byte[] array = Encoding.ASCII.GetBytes(s);
            UartEncodeAndSendMessage(0x0080, array.Length, array);
        }
    }
}
