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
        /*Strings for charachter information*/
        string charName, job, sjob, location, lettercord, direction, time, day, conditions;
        /*Integers for charachter information*/
        int lvl, slvl, curEXP, nextEXP, numbercord, x, y, z,degree;
        /*strings for when a tell is recived*/
        string tellUser, tellMessage;
        /*other ints*/
        int currentDisplayMode = -1, previousDisplayMode = 0, returnDisplayTimer = 0;

        TcpClient tcpclnt = new TcpClient();
        NetworkStream stream = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            reciveInfo("Rosealyne", "RDM", 23, "DNC", 99, 118800, 118800, "Bastok Mines", "H", 8, -100, -100, -22, 359, "22:22", "Lightningsday", "Foggy");
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
        }

        private void Form1_OnClosing(object sender, FormClosingEventArgs e)
        {
            Logitech.LogiLcdShutdown();
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
            if (tcpclnt.Connected)
            {
                Byte[] data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.WriteLine(responseData);
            }
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

        private void reciveInfo(string charname, string job, int lvl, string sjob, int slvl, int curexp, int nextexp, string local, string letcord, int numcord, int px, int py, int pz, int deg, string gametime, string gameday, string weather)
        {
            while (charname.Length < 15) {
                charname += " ";
            }
            this.charName = charname;
            this.job = job;
            this.lvl = lvl;
            this.sjob = sjob;
            this.slvl = slvl;
            this.curEXP = curexp;
            this.nextEXP = nextexp;
            this.location = local;
            this.lettercord = letcord;
            this.numbercord = numcord;
            this.x = px;
            this.y = py;
            this.z = pz;
            this.degree = deg;
            this.direction = "NNW"; //TODO: determine direction bassed on degree
            this.time = gametime;
            this.day = gameday;
            this.conditions = weather;
        }

        private void reciveTell(string tellname, string tellmess)
        {
            //if(rb_tell_true.Enabled == true) {
                this.tellUser = tellname;
                this.tellMessage = tellmess;
                previousDisplayMode = currentDisplayMode;
                currentDisplayMode = 4;
            //}
        }

        double map(int x, int in_min, int in_max, int out_min,int out_max) {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private int expbarselection(int expcur, int expneed)
        {
            return Convert.ToInt32(System.Math.Round(map(expcur, 0, expneed, 0, 151)));
        }

        private void updateCurrentDisplay(int dispMode)
        {
            if (dispMode == -1) /*Initial Screen*/
            {
                Logitech.LogiLcdMonoSetText(0, "");
                Logitech.LogiLcdMonoSetText(1, "     Final Fantasy XI     ");
                Logitech.LogiLcdMonoSetText(2, "          Online          ");
                Logitech.LogiLcdMonoSetText(3, "   Awaiting Connection... ");
                Logitech.LogiLcdMonoSetBackground(Logitech.lcdBackroundBlank);
            }
            else if (dispMode == 0) /*First Tab*/
            {
                Logitech.LogiLcdMonoSetText(0, charName + " " + job + lvl + "/" + sjob + slvl);
                Logitech.LogiLcdMonoSetText(1, "");
                Logitech.LogiLcdMonoSetText(2, "EXP:" + curEXP + "/" + nextEXP);
                Logitech.LogiLcdMonoSetText(3, "");
                Logitech.LogiLcdMonoSetBackground(EXPbar.expbar[this.expbarselection(curEXP, nextEXP)]);
            }
            else if (dispMode == 1) /*Second Tab*/
            {
                Logitech.LogiLcdMonoSetText(0, location);
                if (numbercord < 10) { Logitech.LogiLcdMonoSetText(1, "(" + lettercord + "-" + numbercord + ")  " + time + "  " + day);}
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
            tcpclnt.Connect("localhost", 33941);
            stream = tcpclnt.GetStream();
        }
    }
}
