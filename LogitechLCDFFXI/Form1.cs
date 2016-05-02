using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogitechLCDFFXI
{ 
    
    public partial class Form1 : Form
    {
        string charName, job, sjob;
        int lvl, slvl, curEXP, nextEXP;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            reciveInfo("Rosealyne", "RDM", 23, "DNC", 99, 118800, 118800);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Logitech.LogiLcdInit("FFXI", Logitech.LcdType.Mono);
            trayIcon.BalloonTipText = "Running in tray. Double click tray icon to maximize.";
            trayIcon.BalloonTipTitle = "FFXI LCD Applet";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.Icon = this.Icon;
            rb_notify_true.Checked = true;
            Logitech.LogiLcdMonoSetText(1, "     Final Fantasy XI     ");
            Logitech.LogiLcdMonoSetText(2, "          Online          ");
            Logitech.LogiLcdMonoSetText(3, "   Awaiting Connection... ");
        }

        private void Form1_OnClosing(object sender, FormClosingEventArgs e)
        {
            Logitech.LogiLcdShutdown();
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
                Logitech.LogiLcdUpdate();
            }
            if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton0))
            {
                Logitech.LogiLcdMonoSetText(0, charName + " " + job + lvl + "/" + sjob + slvl);
                Logitech.LogiLcdMonoSetText(1, "");
                Logitech.LogiLcdMonoSetText(2, "EXP:" + curEXP + "/" + nextEXP);
                Logitech.LogiLcdMonoSetText(3, "");
                Logitech.LogiLcdMonoSetBackground(EXPbar.expbar[this.expbarselection(curEXP, nextEXP)]);
            }else if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton1))
            {
                Logitech.LogiLcdMonoSetText(0, "Tab 2");
                Logitech.LogiLcdMonoSetText(1, "");
                Logitech.LogiLcdMonoSetText(2, "");
                Logitech.LogiLcdMonoSetText(3, "");
            }
            else if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton2))
            {
                Logitech.LogiLcdMonoSetText(0, "Tab 3");
                Logitech.LogiLcdMonoSetText(1, "");
                Logitech.LogiLcdMonoSetText(2, "");
                Logitech.LogiLcdMonoSetText(3, "");
            }
            else if (Logitech.LogiLcdIsButtonPressed(Logitech.Buttons.MonoButton3))
            {
                Logitech.LogiLcdMonoSetText(0, "Tab 4");
                Logitech.LogiLcdMonoSetText(1, "");
                Logitech.LogiLcdMonoSetText(2, "");
                Logitech.LogiLcdMonoSetText(3, "");
            }
        }

        private void reciveInfo(string charname, string job, int lvl, string sjob, int slvl, int curexp, int nextexp)
        {
            while (charname.Length < 15) {
                charname = charname + " ";
            }
            this.charName = charname;
            this.job = job;
            this.lvl = lvl;
            this.sjob = sjob;
            this.slvl = slvl;
            this.curEXP = curexp;
            this.nextEXP = nextexp;
        }

        double map(int x, int in_min, int in_max, int out_min,int out_max) {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private int expbarselection(int expcur, int expneed)
        {
            return Convert.ToInt32(System.Math.Round(map(expcur, 0, expneed, 0, 151)));
        }
    }
}
