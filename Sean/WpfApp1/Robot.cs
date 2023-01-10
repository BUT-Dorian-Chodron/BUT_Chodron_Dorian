using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Robot
    {
        public string receivedText = "";
        public float distanceTelemetreDroit;
        public float distanceTelemetreCentre;
        public float distanceTelemetreGauche;
        public byte byteListreceived;
        
       public  Queue<byte> byteListReceived = new Queue<byte>();



        public Robot()
        {  
             
        }
       
    }
}
