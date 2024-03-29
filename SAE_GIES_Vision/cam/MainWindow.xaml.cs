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

namespace cam
{

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReliableSerialPort serialPort1;
        DispatcherTimer timerAffichage;

        public MainWindow()
        {
            InitializeComponent();
            //serialPort1 = new ReliableSerialPort("COM7", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            //serialPort1.DataReceived += SerialPort1_DataReceived;
            //serialPort1.Open();
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


        }

        //private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        //{
            //textBoxReception.Text += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
        //}

        void sendMessage()
        {

            //serialPort1.WriteLine(textBoxEmission.Text);
            textBoxReception.Text = "Reçu : " + textBoxEmission.Text + "\n";
            textBoxEmission.Text = "";

        }

        //private void textBoxEmission_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //        sendMessage();
        //}
    }
}
