namespace LogitechLCDFFXI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.rb_notify_true = new System.Windows.Forms.RadioButton();
            this.rb_notify_false = new System.Windows.Forms.RadioButton();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.lblName = new System.Windows.Forms.Label();
            this.lblNotify = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.timerData = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 226);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Char Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // trayIcon
            // 
            this.trayIcon.Text = "FFXI LCD Applet";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
            // 
            // rb_notify_true
            // 
            this.rb_notify_true.AutoSize = true;
            this.rb_notify_true.Location = new System.Drawing.Point(124, 32);
            this.rb_notify_true.Name = "rb_notify_true";
            this.rb_notify_true.Size = new System.Drawing.Size(47, 17);
            this.rb_notify_true.TabIndex = 1;
            this.rb_notify_true.Text = "True";
            this.rb_notify_true.UseVisualStyleBackColor = true;
            // 
            // rb_notify_false
            // 
            this.rb_notify_false.AutoSize = true;
            this.rb_notify_false.Location = new System.Drawing.Point(177, 31);
            this.rb_notify_false.Name = "rb_notify_false";
            this.rb_notify_false.Size = new System.Drawing.Size(50, 17);
            this.rb_notify_false.TabIndex = 2;
            this.rb_notify_false.Text = "False";
            this.rb_notify_false.UseVisualStyleBackColor = true;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // lblName
            // 
            this.lblName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(0, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(284, 22);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Final Fantasy XI Online LCD Applet";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblNotify
            // 
            this.lblNotify.AutoSize = true;
            this.lblNotify.Location = new System.Drawing.Point(12, 36);
            this.lblNotify.Name = "lblNotify";
            this.lblNotify.Size = new System.Drawing.Size(106, 13);
            this.lblNotify.TabIndex = 4;
            this.lblNotify.Text = "Minimize Notification:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(96, 226);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Tell Test";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 197);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Start Server";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // timerData
            // 
            this.timerData.Enabled = true;
            this.timerData.Tick += new System.EventHandler(this.timerData_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblNotify);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.rb_notify_false);
            this.Controls.Add(this.rb_notify_true);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "FFXI LCD";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_OnClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.RadioButton rb_notify_true;
        private System.Windows.Forms.RadioButton rb_notify_false;
        public System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblNotify;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Timer timerData;
    }
}

