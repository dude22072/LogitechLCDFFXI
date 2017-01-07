using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Diagnostics;

namespace LogitechLCDFFXI
{ 
    
    public partial class Form1 : Form
    {
        EXPbar expbar = new EXPbar();
        /*Strings for charachter information*/
        public static volatile string charName = "???????????????";
        public static volatile string job, sjob, location, lettercord, numbercord, direction, time, day, conditions = "???";
        /*Integers for charachter information*/
        public static volatile int hp, mhp, mp, mmp, lvl, slvl, curEXP, nextEXP, x, y, z,degree;
        /*strings for when a tell is recived*/
        public static volatile string tellUser, tellMessage;
        /*other ints*/
        static int currentDisplayMode = -1, previousDisplayMode = 0, returnDisplayTimer = 0;
       
        public static volatile Boolean started,connected = false;
        public static volatile String[] locationTable = new String[] {"Residential Area",//0
        "Phanauet Channel","Carpenters Landing","Manaclipper","Bibiki Bay","Uleguerand Range","Bearclaw Pinnacle","Attohwa Chasm","Boneyard Gully","PsoXja","The Shrouded Maw",//10
        "Oldton Movalpolos","Newton Movalpolos","Mine Shaft 2716","Hall of Transference","Abyssea-Konschtat","Promyvion-Holla","Spire of Holla","Promyvion-Dem","Spire of Dem","Promyvion-Mea",//20
        "Spire of Mea","Promyvion-Vahzl","Spire of Vahzl","Lufaise Meadows","Misareaux Coast","Tavnazian Safehold","Phomiuna Aqueducts","Sacrarium","Riverne-Site B01","Riverne-Site A01",//30
        "Monarch Linn","Sealions Den","AlTaieu","Grand Palace of HuXzoi","The Garden of RuHmet","Empyreal Paradox","Temenos","Apollyon","Dynamis-Valkurm","Dynamis-Buburimu",//40
        "Dynamis-Qufim","Dynamis-Tavnazia","Diorama Abdhaljs-Ghelsba","Abdhaljs Isle-Purgonorgo","Abyssea-Tahrongi","Open sea -> Al Zahbi","Open sea -> Mhaura","Al Zahbi","noname","Aht Urhgan Whitegate",//50
        "Wajaom Woodlands","Bhaflau Thickets","Nashmau","Arrapago Reef","Ilrusi Atoll","Periqia","Talacca Cove","Silver Sea -> Nashmau","Silver Sea -> Al Zahbi","The Ashu Talif",//60
        "Mount Zhayolm","Halvung","Lebros Cavern","Navukgo Execution Chamber","Mamook","Mamool Ja Training Grounds","Jade Sepulcher","Aydeewa Subterrane","Leujaoam Sanctum","Chocobo Circuit",//70
        "","","","","","","","","","Southern San d'Oria",//80
        "","","","","","","","","","",//90
        "","","","","","","","","","",//100
        "","","","","","","","","","",//110
        "","","","","","","","","","",//120
        "","","","","","","","","","",//130
        "","","noname","","","","","","","",//140
        "","","","","","","","","","",//150
        "","","","","","","","","","",//160
        "","","","","","","","","","",//170
        "","","","","","","","","","",//180
        "","","","","","","","","noname","",//190
        "","","","","","","","","","",//200
        "","","","","","","","","","noname",//210
        "","","","","","","","","noname","",//220
        "","","","","","","","","noname","Southern San d'Oria",//230
        "Northern San d'Oria","Port San d'Oria","Chateau d'Oranguille","Bastok Mines","Bastok Markets","Port Bastok","Metalworks","Windrust Waters","Windrust Walls","Port Windrust",//240
        "Windrust Woods","Heavens Tower","RuLude Gardens","Upper Jeuno","Lower Jueno","Port Jueno","Rabao","Selbina","Mhaura","Kazham",//250
        "Hall of the Gods","Norg","Abyssea-Uleguerand","Abyssea-Grauberg","Abyssea-Empyreal Paradox","Western Adoulin","Eastern Adoulin","Rala Waterways","Rala Waterways","Yahse Hunting Grounds",//260
        "Ceizak Battlegrounds","Foret de Hennetiel","Yorcia Weald","Yorcia Weald","Morimar Basalt Fields","Marjami Ravine","Kamihr Drifts","Sih Gates","Moh Gates","Cirdas Caverns",//270
        "Cirdas Caverns","Dho Gates","Woh Gates","","","","","","","Mog Garden",//280
        "","","","","Mog House"
        };
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            reciveInfo("TEST", "TST", 99, "TST", 99, 52800, 118800, "234", "H", "8", -100, -100, -22, 359, "22:22", Worker.days[5], Worker.weather[3]);
            currentDisplayMode = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logitech.LogiLcdInit("FFXI", Logitech.LcdType.Mono);
            trayIcon.BalloonTipText = "Running in tray. Double click tray icon to maximize.";
            trayIcon.BalloonTipTitle = "FFXI LCD Applet";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.Icon = this.Icon;
            rb_notify_true.Checked = true;
            rb_exp_text.Checked = true;
        }

        private void Form1_OnClosing(object sender, FormClosingEventArgs e)
        {
            Logitech.LogiLcdShutdown();
            Program.workerObject.RequestStop();
            if (Worker.client != null) { Worker.client.Close(); }
            Worker.tcpclnt.Stop();
            trayIcon.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                trayIcon.Visible = true;
                if (rb_notify_true.Checked) {
                    trayIcon.ShowBalloonTip(500);
                }
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                trayIcon.Visible = false;
            }
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            
            if (Logitech.LogiLcdIsConnected(Logitech.LcdType.Mono) /*|| Logitech.LogiLcdIsConnected(Logitech.LcdType.Color)*/)
            {
                updateCurrentDisplay(currentDisplayMode);
                Logitech.LogiLcdUpdate();
            }
            if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton0))
            {
                currentDisplayMode = 0;
            }else if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton1))
            {
                currentDisplayMode = 1;
            }
            else if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton2))
            {
                currentDisplayMode = 2;
            }
            else if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton3))
            {
                currentDisplayMode = 3;
            }
        }

        [System.Obsolete("FFXI doesn't work well with a \"HERES EVERYTHING\" solution.")]
        private void reciveInfo(string charname, string job, int lvl, string sjob, int slvl, int curexp, int nextexp, string local, string letcord, string numcord, int px, int py, int pz, int deg, string gametime, string gameday, string weather)
        {
            while (charname.Length < 15) {
                charname += " ";
            }
            Form1.charName = charname;
            Form1.job = job;
            Form1.lvl = lvl;
            Form1.sjob = sjob;
            Form1.slvl = slvl;
            Form1.curEXP = curexp;
            Form1.nextEXP = nextexp;
            Form1.location = local;
            Form1.lettercord = letcord;
            Form1.numbercord = numcord;
            Form1.x = px;
            Form1.y = py;
            Form1.z = pz;
            Form1.degree = deg;
            Form1.direction = "NNW"; //TODO: determine direction bassed on degree
            Form1.time = gametime;
            Form1.day = gameday;
            Form1.conditions = weather;
        }

        public static void reciveTell(string tellname, string tellmess)
        {
            //if(rb_tell_true.Enabled == true) {
                tellUser = tellname;
                tellMessage = tellmess;
                previousDisplayMode = currentDisplayMode;
                currentDisplayMode = 4;
            //}
        }

        public static void reviceName(string name)
        {
            while (name.Length < 15)
            {
                name += " ";
            }
            charName = name;
        }

        public static double map(int x, int in_min, int in_max, int out_min,int out_max) {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private void updateCurrentDisplay(int dispMode)
        {
            if (dispMode == -1) /*Initial Screen*/
            {
                Logitech.LogiLcdMonoSetText(0, "");
                Logitech.LogiLcdMonoSetText(1, "     Final Fantasy XI     ");
                Logitech.LogiLcdMonoSetText(2, "          Online          ");
                if (started && !connected)
                {
                    Logitech.LogiLcdMonoSetText(3, "   Awaiting Connection... ");
                } else { Logitech.LogiLcdMonoSetText(3, ""); }
                Logitech.LogiLcdMonoSetBackground(Logitech.lcdBackroundBlank);
            }
            else if (dispMode == 0) /*First Tab*/
            {
                //TODO:HP, MP, EXP
                Logitech.LogiLcdMonoSetText(0, charName + " " + job + lvl + "/" + sjob + slvl);
                Logitech.LogiLcdMonoSetText(1, "HP: " + hp + "/" + mhp);
                Logitech.LogiLcdMonoSetText(2, "MP: " + mp + "/" + mmp);
                if (rb_exp_text.Checked)
                {
                    Logitech.LogiLcdMonoSetText(3, "EXP: " + curEXP + "/" + nextEXP);
                    Logitech.LogiLcdMonoSetBackground(Logitech.lcdBackroundBlank);
                }
                else if (rb_exp_bar.Checked)
                {
                    Logitech.LogiLcdMonoSetText(3, "");
                    Logitech.LogiLcdMonoSetBackground(expbar.createExpBar(map(curEXP, 0, nextEXP, 0, 152)));
                }
            }
            else if (dispMode == 1) /*Second Tab*/
            {
                if (location != "?")
                {
                    Logitech.LogiLcdMonoSetText(0, locationTable[Convert.ToInt64(location)]);
                } else { Logitech.LogiLcdMonoSetText(0, "?");  }
                if (numbercord.Length < 2) { Logitech.LogiLcdMonoSetText(1, "(" + lettercord + "-" + numbercord + ")  " + time + "  " + day);}
                else { Logitech.LogiLcdMonoSetText(1, "(" + lettercord + "-" + numbercord + ") " + time + "  " + day); }
                Logitech.LogiLcdMonoSetText(2, "X:"+x+" Y:"+y+" Z:"+z + " " + direction + degree + "°");
                Logitech.LogiLcdMonoSetText(3, "Weather: " + conditions);
                Logitech.LogiLcdMonoSetBackground(Logitech.lcdBackroundBlank);
            }
            else if (dispMode == 2) /*Third Tab*/
            {
                Logitech.LogiLcdMonoSetText(0, "Tab 3");
                Logitech.LogiLcdMonoSetText(1, "");
                Logitech.LogiLcdMonoSetText(2, "");
                Logitech.LogiLcdMonoSetText(3, "");
                Logitech.LogiLcdMonoSetBackground(Logitech.lcdBackroundBlank);
            }
            else if (dispMode == 3) /*Fourth Tab*/
            {
                Logitech.LogiLcdMonoSetText(0, "Tab 4");
                Logitech.LogiLcdMonoSetText(1, "");
                Logitech.LogiLcdMonoSetText(2, "");
                Logitech.LogiLcdMonoSetText(3, "");
                Logitech.LogiLcdMonoSetBackground(Logitech.lcdBackroundBlank);
            }
            else if (dispMode == 4) /*Tell Recived*/
            {
                Logitech.LogiLcdMonoSetText(0, tellUser);
                Logitech.LogiLcdMonoSetText(1, tellMessage.Substring(0, tellMessage.Length));
                if (tellMessage.Length > 26)
                {
                    Logitech.LogiLcdMonoSetText(2, tellMessage.Substring(26, tellMessage.Length-26));
                } else { Logitech.LogiLcdMonoSetText(2, ""); }
                if (tellMessage.Length > 53)
                {
                    Logitech.LogiLcdMonoSetText(3, tellMessage.Substring(53, tellMessage.Length-53));
                } else { Logitech.LogiLcdMonoSetText(3, ""); }
                Logitech.LogiLcdMonoSetBackground(Logitech.lcdBackroundBlank);
                returnDisplayTimer++;
                if(returnDisplayTimer >= 50) {
                    returnDisplayTimer = 0;
                    currentDisplayMode = previousDisplayMode;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reciveTell("FriendSomething","This is a test of the tell notification on dude22072's logitech gamepanel LCD applet.");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Worker.tcpclnt.Start();
            started = true;
        }

        public static string formatTime(int input)
        {
            /*hour = input / 60;
            min = input % 60;
            return hour + ":" + min.ToString().PadLeft(2, '0');*/
            return input / 60 + ":" + (input % 60).ToString().PadLeft(2, '0');
        }

        [System.Obsolete("")]
        private void timerData_Tick(object sender, EventArgs e)
        {
            /*
            {
                Byte[] data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.WriteLine(responseData);
            }*/
        }
    }
}
