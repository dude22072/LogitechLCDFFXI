using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;

namespace LogitechLCDFFXI
{
    public class Worker
    {
        static System.Net.IPAddress local = System.Net.IPAddress.Parse("127.0.0.1");
        public static volatile TcpListener tcpclnt = new TcpListener(local, 33941);
        NetworkStream stream = null;
        public static volatile TcpClient client = null;
        public static volatile string[] weather = new string[20] { "Clear", "Sunshine", "Clouds", "Fog", "Fire", "Fire x2", "Water", "Water x2", "Earth", "Earth x2", "Wind", "Wind x2", "Ice", "Ice x2", "Thunder", "Thunder x2", "Light", "Light x2", "Dark", "Dark x2" };
        public static volatile string[] days = new string[8] { "     Firesday", "    Earthsday", "    Watersday", "     Windsday", "       Iceday", " Lightningday", "    Lightsday", "     Darksday" };
        // This method will be called when the thread is started. 
        public void DoWork()
        {
            while (!_shouldStop)
            {
                Byte[] bytes = new Byte[256];
                String data = null;
                if (Form1.started)
                {
                    if (tcpclnt.Pending())
                    {
                        client = tcpclnt.AcceptTcpClient();
                        stream = client.GetStream();
                        Form1.connected = true;
                        Debug.WriteLine("connected");
                    }
                    if (Form1.connected)
                    {
                        int i;
                        if ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            Debug.WriteLine("Received: {0}", data);

                            string[] multirecive = data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                            int k;
                            for (k = 0; k < multirecive.Length; k++)
                            {
                                string[] cmdData = multirecive[k].Split(new char[] { ':' });
                                switch (cmdData[0].ToUpper())
                                {
                                    case "NAME":
                                        Form1.reviceName(cmdData[1]);
                                        break;
                                    case "JOB":
                                        Form1.job = cmdData[1];
                                        break;
                                    case "JOBL":
                                        Form1.lvl = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "SJOB":
                                        Form1.sjob = cmdData[1];
                                        break;
                                    case "SJOBL":
                                        Form1.slvl = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "HP":
                                        Form1.hp = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "MHP":
                                        Form1.mhp = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "MP":
                                        Form1.mp = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "MMP":
                                        Form1.mmp = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "EXP":
                                        Form1.curEXP = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "EXPN":
                                        Form1.nextEXP = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "LOC":
                                        Form1.location = cmdData[1];
                                        break;
                                    /*case "LCD":
                                        Form1.lettercord = cmdData[1];
                                        break;
                                    case "NCD":
                                        Form1.numbercord = cmdData[1];
                                        break;*/
                                    case "TIM":
                                        Form1.time = Form1.formatTime(Convert.ToInt32(cmdData[1]));
                                        break;
                                    case "DAY":
                                        Form1.day = days[Convert.ToInt32(cmdData[1])];
                                        break;
                                    case "X":
                                        Form1.x = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "Y":
                                        Form1.y = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "Z":
                                        Form1.z = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "DEG":
                                        Form1.degree = Convert.ToInt32(cmdData[1]);
                                        break;
                                    case "WTH":
                                        Form1.conditions = weather[Convert.ToInt32(cmdData[1])];
                                        break;
                                    case "TELL":
                                        Form1.reciveTell(cmdData[1], cmdData[2]);
                                        break;
                                    case "POS":
                                        Form1.lettercord = cmdData[1].Substring(1, 1);
                                        if (cmdData[1].Length == 5)
                                        {
                                            Form1.numbercord = cmdData[1].Substring(3, 1);
                                        }
                                        else
                                        {
                                            Form1.numbercord = cmdData[1].Substring(3, 2);
                                        }
                                        break;
                                    default:
                                        Debug.WriteLine("???:" + data);
                                        break;
                                }
                            }
                        }
                    }
                }

            }
            Debug.WriteLine("worker thread: terminating gracefully.");
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 
        private volatile bool _shouldStop;
    }

    static class Program
    {
        public static volatile Worker workerObject = new Worker();
        public static volatile Thread workerThread = new Thread(workerObject.DoWork);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Create the thread object. This does not start the thread.


            // Start the worker thread.
            workerThread.Start();
            Console.WriteLine("main thread: Starting worker thread...");

            // Loop until worker thread activates. 
            while (!workerThread.IsAlive) ;

            // Put the main thread to sleep for 1 millisecond to 
            // allow the worker thread to do some work:
            Thread.Sleep(1);
            Application.Run(new Form1());
        }
    }
}
