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
using SciChart.Charting;

using MouseKeyboardActivityMonitor.WinApi;
using MouseKeyboardActivityMonitor;
using System.Windows.Forms;

using MathNet.Spatial.Euclidean;
using MathNet.Numerics.LinearRegression;

using SciChart.Charting.Visuals;
using SciChart.Charting3D.Model;
using SciChart.Charting3D.RenderableSeries;
using SciChart.Charting3D.PointMarkers;
using SciChart.Charting2D.Interop;
using SciChart.Data.Model;
using SciChart.Charting.ViewportManagers;
using SciChart.Charting3D;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;


namespace WpfApp1
{

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        ReliableSerialPort serialPort1;
        DispatcherTimer timerAffichage;
        Robot robot = new Robot();
        private readonly KeyboardHookListener m_KeyboardHookManager;

        XyzDataSeries3D<double> xyzDataSeries3D = new XyzDataSeries3D<double>() { SeriesName = "Series Ball" };
        ScatterRenderableSeries3D renderSerias = new ScatterRenderableSeries3D();

        public MainWindow()
        {
            // Set this code once in App.xaml.cs or application startup
            SciChartSurface.SetRuntimeLicenseKey("fUqreVDpZQkQbANd4/WZ+HaJ23zvpzFnBaxZt7av32tx2d1phoLOC5Oj1oqtdQrFr1X5Prg7GCuB1cQOgM50ChVxrpE9SEbo4XQjB/g0cJv0oyAryMA0U7FK2X0xXgTPjWtTu8pIsR9dgS/Sxb39j1mtDSPBaw7EbRizLvbJEV4+gFJ4e261fjEBQ1DcVX4t670uaGbGb0K6D5CsU4VOzhL+q+pommyuqQkDUkROWOy0xlF7wjiQG1rEizVbkJJkV0Ao+mzuq9uPS6TdIzgUEeAP+teRJSSeQk46uvJ7ULzfVaY7ylBtE9ePNVRO6B9f1dV9LJvGYzo75g+wRgjGJqHJi4gOJG08geEL0nSe4B2LLay+LVVlw6XC4+xQpE+Reipy/nnUZgrdHXphCa3qcVJ7/BFjaRSc382d5ja76cVxwVsyf0C/0MA8Yt8NTxp3+4iNdZRtcx/Sz8uvWR+j7UFOAtSltMEMDbFnXSCSIBQc0+Y0oZKmb2egaiV09OrEfgBo/KQN");
            //SciChartSurface.SetRuntimeLicenseKey("grFn6m7akvMOe4eERUFH7ZQnUR9WAFue5nYE/8cdvf21/39otXQO9ySDRAI/BAfgz97e7OQMFFzk4fY7FiBa/th2FidS5dkcvC5Yi9XfRRm8hSNxhw+aumT54a7BWgkQWt8qrgdgho7zJ6XVFqYTO6sQ5uJHh+se7FyWVJI2Z3I/C41P3yP/dDM4ZlLGXDwftFj0KFb7wKKNffhc6AmR0raCGLHeC1fwyWjei5pcojLQjuvvvimhryI/VntLBlJ7S4AMP/iGv3GQYrwTsKAjMqkluq8HUJpzMcksqsUXvODjMTJFBb/4/yVYsjm8mLWEE5Y90+SWy4n/vhjdOVLQzVROYGhuRDkEMi+ZNG4KkHYANy28F7LwQSOTJmRXd/i7NdmqgGbhq+EJAoflG+y6eOF/96tM11GrihwF/QN+4yBPPrX95s5IKdjV+1/oG0rOisAw/ptkia4F/fiaeKwOOgpuYdaLeGyoFXtobBtdA47sJ73xuJkqEoi1DHRYTysJCe3L+it6ZOcDdKw95TfdiNOmV0WvNKc/0eCkYQyjQBvxQdW1BzWzWispUSXO8HEzmkQW0Ra3G0mk75Ra");
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

            renderSerias.DataSeries = xyzDataSeries3D;
            renderSerias.PointMarker = new CubePointMarker3D();
            renderSerias.PointMarker.Size = 5;
            SciChart3DBall.RenderableSeries.Add(renderSerias);

            var camera = new Camera3D();
            camera.Position = new Vector3(200, -200, 200); // Ajuster la position de la caméra
            camera.Target = new Vector3(0, 0, 0); // Le point vers lequel la caméra est dirigée
            camera.OrbitalPitch = 33; // Inclinaison verticale
            camera.OrbitalYaw = -50; // Inclinaison horizontale
            SciChart3DBall.Camera = camera;

            //SciChart3DBall.Camera.Position = new Vector3(0, 0, -10);
            //SciChart3DBall.Camera.Target = new Vector3(0, 0, 10);
            //SciChart3DBall.Camera.OrbitalPitch = 0;
            //SciChart3DBall.Camera.OrbitalYaw = 0;

            //SciChart3DBall.XAxis.AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Never;
            //SciChart3DBall.XAxis.VisibleRange = new IndexRange(0, 10);
            //SciChart3DBall.YAxis.AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Never;
            //SciChart3DBall.YAxis.VisibleRange = new IndexRange(-4, 4);
            //SciChart3DBall.ZAxis.AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Never;
            //SciChart3DBall.ZAxis.VisibleRange = new IndexRange(-2, 2);
        }

        List<Point3D> trajectoire = new List<Point3D>();
        private void Prediction(int ballXRefPourri, int ballYRefPourri, int ballRadius)
        {
            //Détection fin trajectoire --> tableau de trajectoire : trajectoire
            //utilis&tion du temps
            double[] ptX = new double[] { 0, 1, 2, 3 };
            double[] ptY = new double[] { 4, 5, 6, 7 };
            double[] ptZ = new double[] { 0, 1, 2, 3 };

            (double Ax, double Bx) = Fit.Line(ptX, ptY);

        }


        private void PositionBall(int ballXRefPourri, int ballYRefPourri, int ballRadius)
        {
            int distPixel45 = 612;
            Vector3D axeOptique = new Vector3D(1, 0, 0);

            double ballX = ballXRefPourri - (1920 / 2);
            double ballY = (1080 / 2) - ballYRefPourri;

            //textBoxReception.Text = "X : " + ballX + " - Y : " + ballY + " - Radius : " + ballRadius + "\n" + textBoxReception.Text;

            //Traitement data
            double distancePtObjetAxeOptique = Math.Sqrt(Math.Pow(ballX, 2) + Math.Pow(ballY, 2));

            double theta = Math.Atan2(distancePtObjetAxeOptique, distPixel45);
            var phi = Math.Atan2(-ballX, ballY);
            //double phi = Math.Atan2(ballX-1920/2, ballY-1080/2);

            var matRotationTheta = Matrix3D.RotationAroundYAxis(MathNet.Spatial.Units.Angle.FromRadians(-theta));
            var axeTheta = axeOptique.TransformBy(matRotationTheta);

            var matRotationPhi = Matrix3D.RotationAroundArbitraryVector(axeOptique.Normalize(), MathNet.Spatial.Units.Angle.FromRadians(phi));
            var axeObjet = axeTheta.TransformBy(matRotationPhi);

            if (ballRadius != 0)
            {
                double distance = 120.0 / ballRadius;

                Vector3D ballPos = distance * axeObjet;
                textBoxReception.Text = "Bx : " + ballPos.X.ToString("N2") + " By : " + ballPos.Y.ToString("N2") + " Bz : " + ballPos.Z.ToString("N2") + "\n" + textBoxReception.Text;

                trajectoire.Add(new Point3D(ballPos.Y, ballPos.Z, ballPos.X));
                while (trajectoire.Count > 50) //Si il y a plus de XXX points
                {
                    trajectoire.RemoveAt(0);
                }

                xyzDataSeries3D.Clear();
                xyzDataSeries3D.Append(trajectoire.Select(o => o.X).ToList(), trajectoire.Select(o => o.Y).ToList(), trajectoire.Select(o => o.Z).ToList());

                SciChart3DBall.InvalidateArrange();
            }
        }


        List<byte> currentByteList = new List<byte>();

        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            //if (robot.receivedText != "")
            //{
            //    for (int i = 0; i < robot.receivedText.Length; i++)
            //    {
            //        // Vérifier si le caractère actuel est un saut de ligne
            //        if (robot.receivedText[i] == '\n')
            //        {
            //            // Extraire la sous-chaîne jusqu'à l'emplacement du saut de ligne
            //            textBoxReception.Text = +"" + robot.receivedText.Substring(0, i) + "\n";
            //            break;
            //        }
            //    }

            //   robot.receivedText = "";
            // }

            while (robot.byteListReceived.Count() > 0)
            {
                var c = robot.byteListReceived.Dequeue();

                if (c != '\r')
                {
                    if (c != '\n')
                        currentByteList.Add(c);
                }
                else
                {
                    /// On a terminé une trame, on l'analyse
                    var trame = Encoding.ASCII.GetString(currentByteList.ToArray());
                    var infos = trame.Split(' ');
                    int posX;
                    int radius;
                    int posY;
                    if (infos[0] == "ball")
                    {
                        int.TryParse(infos[1], out posX);
                        int.TryParse(infos[2], out posY);
                        int.TryParse(infos[3], out radius);
                        PositionBall(posX, posY, radius);
                        textBoxReception.Text = "X : " + posX + " - Y : " + posY + " - Radius : " + radius + "\n" + textBoxReception.Text;
                    }





                    if (textBoxReception.Text.Length > 500)
                        textBoxReception.Text = textBoxReception.Text.Substring(0, 500);
                    currentByteList.Clear();
                }

                //textBoxReception.Text += "0x" + c.ToString("X2") + " ";
                //DecodeMessage(c);
            }
            //PositionBall(500,1500,75);

        }

        private void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            //textBoxReception.Text += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            //robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
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
                        //textBoxReception.Text = "OK \n";
                        ProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    }
                    else
                    {
                        //textBoxReception.Text = "NON OK \n";
                    }
                    rcvState = StateReception.Waiting;

                    break;
                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }
        void ProcessDecodedMessage(int msgFunction, int msgPayloadLength, byte[] msgPayLoad)
        {
            if (msgFunction == 0x0030)
            {
                IRgauche.Text = "IR gauche : " + msgPayLoad[0] + "cm";
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
            if (msgFunction == 0x0080 || msgFunction == 0x051)
            {
                for (int i = 0; i < msgPayloadLength; i++)
                {
                    textBoxReception.Text += "0x" + msgPayLoad[i].ToString("X2") + " ";
                }
            }
            if (msgFunction == 0x0050)
            {

                int instant = (((int)msgPayLoad[1]) << 24) + (((int)msgPayLoad[2]) << 16) + (((int)msgPayLoad[3]) << 8) + ((int)msgPayLoad[4]);
                textBoxReception.Text += "\nRobot␣State␣:␣" + ((StateRobot)(msgPayLoad[0])).ToString() + "␣-␣" + instant.ToString() + "␣ms";
            }




        }

        private void textBoxEmission_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                sendMessage();
        }

        bool autoControlActivated = false;
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

    public class Point3D
    {
        public double X;
        public double Y;
        public double Z;

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
