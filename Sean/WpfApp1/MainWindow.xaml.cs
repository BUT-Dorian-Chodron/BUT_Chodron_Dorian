using System;
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

using MouseKeyboardActivityMonitor.WinApi;
using MouseKeyboardActivityMonitor;
using System.Windows.Forms;
using MathNet.Spatial.Euclidean;

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
        private readonly KeyboardHookListener m_KeyboardHookManager;

        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM5", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            m_KeyboardHookManager = new KeyboardHookListener(new GlobalHooker());
            m_KeyboardHookManager.Enabled = true;
            m_KeyboardHookManager.KeyDown += HookManager_KeyDown;

        }

        private void PositionBall(object sender,EventArgs e)
        {
            string[] parts = robot.receivedText.Split('/');
            int distPixel45 = 612;
            Vector3D axeRobot = new Vector3D(1, 0, 0);
            // Extraire les valeurs et les convertir en entiers

            string ballXString = parts[0].Trim();
                string ballYString = parts[1].Trim();
                string ballRadiusString = parts[2].Trim();

                int ballX = int.Parse(ballXString);
                int ballY = int.Parse(ballYString);
                int ballRadius = int.Parse(ballRadiusString);  
            //Traitement data
            double distanceObj = Math.Sqrt(Math.Pow(910-ballX,2) + Math.Pow(540-ballY,2));
            double theta = Math.Atan2(distanceObj, distPixel45);
            var matRotationTheta = Matrix3D.RotationAroundYAxis(MathNet.Spatial.Units.Angle.FromRadians(-theta));
            double phi = Math.Atan2(ballX, ballY);
            var matRotationPlongeeCamera = Matrix3D.RotationAroundYAxis(MathNet.Spatial.Units.Angle.FromRadians(0));
            var axeOptique = axeRobot.TransformBy(matRotationPlongeeCamera);
            var matRotationPhi = Matrix3D.RotationAroundArbitraryVector(axeOptique.Normalize(), MathNet.Spatial.Units.Angle.FromRadians(phi));





        }

        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            if (robot.receivedText != "")
            {
                for (int i = 0; i < robot.receivedText.Length; i++)
                {
                    // Vérifier si le caractère actuel est un saut de ligne
                    if (robot.receivedText[i] == '\n')
                    {
                        // Extraire la sous-chaîne jusqu'à l'emplacement du saut de ligne
                        textBoxReception.Text = "" + robot.receivedText.Substring(0, i) + "\n";
                        break;
                    }
                }
               
               robot.receivedText = "";
             }

            while (robot.byteListReceived.Count() > 0)
            {
                var c = robot.byteListReceived.Dequeue();
                //textBoxReception.Text += "0x" + c.ToString("X2") + " ";
                DecodeMessage(c);
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
            //textBoxReception.Text = "Reçu : " + textBoxEmission.Text + "\n";
            textBoxEmission.Text = "";

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
            //string s = "Bonjour";
            //byte[] array = Encoding.ASCII.GetBytes(s);
            //UartEncodeAndSendMessage(0x0080, array.Length, array);

            byte[] led = { 0x11, 0x01 };
            UartEncodeAndSendMessage(0x0020, 2, led);

            //byte[] Telemetre = { 0x0E, 0xA1, 0x10 };
           // UartEncodeAndSendMessage(0x0030, 3, Telemetre);

            // byte[] vitesse = { 0xA0, 0x85 };
            // UartEncodeAndSendMessage(0x0040, 2, vitesse);

            //ProcessDecodedMessage(0x0040, 2, vitesse);
            ProcessDecodedMessage(0x0020, 2, led);
            //ProcessDecodedMessage(0x0030, 2, Telemetre);




        }
        public enum StateReception
        {
            Waiting,
            FunctionMSB,
            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }
        public enum StateRobot
        {
            STATE_ATTENTE = 0,
            STATE_ATTENTE_EN_COURS = 1,
            STATE_AVANCE = 2,
            STATE_AVANCE_EN_COURS = 3,
            STATE_TOURNE_GAUCHE = 4,
            STATE_TOURNE_GAUCHE_EN_COURS = 5,
            STATE_TOURNE_DROITE = 6,
            STATE_TOURNE_DROITE_EN_COURS = 7,
            STATE_TOURNE_SUR_PLACE_GAUCHE = 8,
            STATE_TOURNE_SUR_PLACE_GAUCHE_EN_COURS = 9,
            STATE_TOURNE_SUR_PLACE_DROITE = 10,
            STATE_TOURNE_SUR_PLACE_DROITE_EN_COURS = 11,
            STATE_ARRET = 12,
            STATE_ARRET_EN_COURS = 13,
            STATE_RECULE = 14,
            STATE_RECULE_EN_COURS = 15
        }

        StateReception rcvState = StateReception.Waiting;
        int msgDecodedFunction = 0;
        int msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        int msgDecodedPayloadIndex = 0;

        private void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:

                    if (c == 0xFE) rcvState = StateReception.FunctionMSB;
                    else rcvState = StateReception.Waiting;  
                    break;

                case StateReception.FunctionMSB:

                    msgDecodedFunction = (int)(c << 8);
                    rcvState = StateReception.FunctionLSB;
                    break;

                case StateReception.FunctionLSB:

                    msgDecodedFunction += (int)(c << 0);
                    rcvState = StateReception.PayloadLengthMSB;
                    break;

                case StateReception.PayloadLengthMSB:

                    msgDecodedPayloadLength = (int)(c << 8);
                    rcvState = StateReception.PayloadLengthLSB;
                    break;

                case StateReception.PayloadLengthLSB:

                    msgDecodedPayloadLength += (int)(c << 0);
                    rcvState = StateReception.Payload;
                    msgDecodedPayload = new byte[msgDecodedPayloadLength];
                    msgDecodedPayloadIndex = 0;
                    break;

                case StateReception.Payload:

                    msgDecodedPayload[msgDecodedPayloadIndex] = c;
                    msgDecodedPayloadIndex++;

                    if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
                        rcvState = StateReception.CheckSum;
                    break;

                case StateReception.CheckSum:

                    byte checksum = CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);

                    if (checksum == c)
                    {
                        textBoxReception.Text = "OK \n";
                        ProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    }
                    else
                    {
                        textBoxReception.Text = "NON OK \n";
                    }   
                    rcvState = StateReception.Waiting;

                    break;
                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }
        void ProcessDecodedMessage(int msgFunction, int msgPayloadLength,byte[]msgPayLoad)
        {
            if (msgFunction==0x0030)
            {
                IRgauche.Text = "IR gauche : " + msgPayLoad[0]+"cm";
                IRcentre.Text = "IR centre : " + msgPayLoad[1] + "cm";
                IRdroite.Text = "IR droite : " + msgPayLoad[2] + "cm";
            }
            if (msgFunction == 0x0040)
            {
                moteur_gauche.Text = "Moteur gauche : " + msgPayLoad[0] + "%";
                moteur_droit.Text = "Moteur droit : " + msgPayLoad[1] + "%";
                

            }
            if (msgFunction == 0x0020)
            {
                switch (msgPayLoad[0])
                {
                    case (0x01):

                        if (msgPayLoad[1] == 0) Led1.IsChecked = false;
                        if (msgPayLoad[1] == 1) Led1.IsChecked = true;
                        break;
                    case (0x10):
                        if (msgPayLoad[1] == 0) Led2.IsChecked = false;
                        if (msgPayLoad[1] == 1) Led2.IsChecked = true;
                        break;
                    case (0x11):

                        if (msgPayLoad[1] == 0) Led3.IsChecked = false;
                        if (msgPayLoad[1] == 1) Led3.IsChecked = true;
                        break;

                }
            }
            if (msgFunction == 0x0080 || msgFunction ==0x051)
            {
                for(int i  = 0; i< msgPayloadLength; i++)
                    {
                    textBoxReception.Text += "0x" + msgPayLoad[i].ToString("X2") + " ";
                }
            }
            if (msgFunction == 0x0050)
            {
                
                int instant = (((int)msgPayLoad[1]) << 24) + (((int)msgPayLoad[2]) << 16)+ (((int)msgPayLoad[3]) << 8) + ((int)msgPayLoad[4]);
                textBoxReception.Text += "\nRobot␣State␣:␣" +((StateRobot)(msgPayLoad[0])).ToString() +"␣-␣" + instant.ToString() + "␣ms";
            }




        }
       
        private void textBoxEmission_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                sendMessage();
        }

        bool autoControlActivated=false;
        private void HookManager_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (autoControlActivated == false)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        UartEncodeAndSendMessage(0x0051, 1, new byte[] {
                (byte)StateRobot.STATE_TOURNE_SUR_PLACE_GAUCHE});
                break;
            case Keys.Right:
                    UartEncodeAndSendMessage(0x0051, 1, new byte[] {
                    (byte)StateRobot.STATE_TOURNE_SUR_PLACE_DROITE });
                break;

            case Keys.Up:
                    UartEncodeAndSendMessage(0x0051, 1, new byte[]
            { (byte)StateRobot.STATE_AVANCE });
                break;

            case Keys.Down:
                    UartEncodeAndSendMessage(0x0051, 1, new byte[]
             { (byte)StateRobot.STATE_ARRET });
                break;

            case Keys.PageDown:
                    UartEncodeAndSendMessage(0x0051, 1, new byte[]
             { (byte)StateRobot.STATE_RECULE });
                break;
            }
        }
    }


    }
}
