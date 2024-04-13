using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XDevkit;
using JRPC_Client;
using CBH_WinForm_Theme_Library_NET;
using System.Threading;
using System.Diagnostics;
using Be.Windows.Forms; // Removed Peekpoker was used for testing/messing with some stuff

namespace CBHThmRGHJTAGRTERTMBase
{
    public partial class MainForm : CrEaTiiOn_Form
    {
        private bool threadisrunning;
        public Thread ConsoleTemps;
        IXboxConsole XBOX;
        IXboxExecutableInfo XboxExecutableInfo;
        IXboxUser XBOXUser;
        IXboxManager XboxManager;
        IXboxDebugTarget XBOXDebug;
        IXboxExecutable XboxExecutable;

        public MainForm()
        {
            ConsoleTemps = new Thread(new ThreadStart(ConsoleTemp));
            InitializeComponent();
        }

        private void ConsoleTemp()
        {
            while (threadisrunning)
            {
                BoxCPUTemp.Text = "CPU: " + XBOX.GetTemperature(JRPC.TemperatureType.CPU).ToString();
                BoxGPUTemp.Text = "GPU: " + XBOX.GetTemperature(JRPC.TemperatureType.GPU).ToString();
                BoxMotherboardTemp.Text = "Board: " + XBOX.GetTemperature(JRPC.TemperatureType.MotherBoard).ToString();
                BoxRAMTemp.Text = "RAM: " + XBOX.GetTemperature(JRPC.TemperatureType.EDRAM).ToString();
                //BoxCPUTemp.Text = "CPU: " + Convert.ToString(CPU);
                //BoxGPUTemp.Text = "GPU: " + Convert.ToString(GPUtmp);
                //BoxMotherboardTemp.Text = "Board: " + Convert.ToString(Motherbrdtmp);
                //BoxRAMTemp.Text = "RAM: " + Convert.ToString(RAMtmp);
                Thread.Sleep(500);
            }
            ConsoleTemps.Abort();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            TimerDnT.Start();
        }

        private void TimerDnT_Tick(object sender, EventArgs e)
        {
            LabelTime.Text = DateTime.Now.ToLongTimeString();
            LabelDate.Text = DateTime.Now.ToLongDateString();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (XBOX.Connect(out XBOX, "default"))
            {    //if (!ConsoleTemps.IsAlive)
                //    {
                //        threadisrunning = true;
                //        ConsoleTemps.Start();
                //    }
                LabelStatus.ForeColor = Color.Green;
                LabelStatus.Text = "Connected";
                XBOX.XNotify("Connected");
                LabelGT.Text = Encoding.BigEndianUnicode.GetString(XBOX.GetMemory(2175412476U, 30U)).Trim().Trim(new char[1]); ;
                BoxKernel.Text = String.Format("Kernel: " + XBOX.GetKernalVersion());
                Box360IP.Text = String.Format("Console IP: " + XBOX.XboxIP());
                Box360Type.Text = String.Format("Console Type: " + XBOX.ConsoleType());
                BoxTitleID.Text = String.Format("Current Title ID: 0x" + XBOX.XamGetCurrentTitleId().ToString("X"));
                BoxCPUKey.Text = String.Format("CPUKey: " + XBOX.GetCPUKey());

            }
            else
            {
                MessageBox.Show("Failed to connect to console!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


            private void btnSendNoti_Click(object sender, EventArgs e)
        {
            XBOX.XNotify(BoxNotify.Text);
        }

        private void btnReboot_Click(object sender, EventArgs e)
        {
            try
            {
                XBOX.Reboot(null, null, null, XboxRebootFlags.Cold);
            }
            catch
            {
                LabelStatus.Text = "Can not reach console";
                LabelStatus.ForeColor = Color.Red;
                MessageBox.Show("Failed to Reboot console!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetTU18Name()
        {
            string bo2getnme;
            XBOX.SetMemory(0x82C55D00, new byte[] { 0x7C, 0x83, 0x23, 0x78, 0x3D, 0x60, 0x82, 0xC5, 0x38, 0x8B, 0x5D, 0x60, 0x3D, 0x60, 0x82, 0x4A, 0x39, 0x6B, 0xDC, 0xA0, 0x38, 0xA0, 0x00, 0x20, 0x7D, 0x69, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x20 });//Injects hook into empty memory
            /*Entry Address = 0x8293D724
             * lis r11, 0x82c5
             * addi r11, r11, 0x5d00
             * mtctr r11
             * bctr*/
            XBOX.SetMemory(0x8293D724, new byte[] { 0x3D, 0x60, 0x82, 0xC5, 0x39, 0x6B, 0x5D, 0x00, 0x7D, 0x69, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x20 });//PatchInJumps XamGetUserName to the hook previously written
            XBOX.SetMemory(0x8259B6A7, new byte[] { 0x00 });//Patch 1
            XBOX.SetMemory(0x822D1110, new byte[] { 0x40 });//Patch 2
            bo2getnme = XBOX.ReadString(0x82C55D60, 20) + XBOX.ReadString(0x841E1B30, 20); // From an open source tool on git never did anything with 360 its not as simple as on PS3 They patched name change stuff on here? lol 
            BoxBO2Name.Text = bo2getnme;
        }

        private void SetTU18Name()
        {
            XBOX.SetMemory(0x82C55D00, new byte[] { 0x7C, 0x83, 0x23, 0x78, 0x3D, 0x60, 0x82, 0xC5, 0x38, 0x8B, 0x5D, 0x60, 0x3D, 0x60, 0x82, 0x4A, 0x39, 0x6B, 0xDC, 0xA0, 0x38, 0xA0, 0x00, 0x20, 0x7D, 0x69, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x20 });//Injects hook into empty memory
            /*Entry Address = 0x8293D724
             * lis r11, 0x82c5
             * addi r11, r11, 0x5d00
             * mtctr r11
             * bctr*/
            XBOX.SetMemory(0x8293D724, new byte[] { 0x3D, 0x60, 0x82, 0xC5, 0x39, 0x6B, 0x5D, 0x00, 0x7D, 0x69, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x20 });//PatchInJumps XamGetUserName to the hook previously written
            XBOX.SetMemory(0x8259B6A7, new byte[] { 0x00 });//Patch 1
            XBOX.SetMemory(0x822D1110, new byte[] { 0x40 });//Patch 2
            //XBOX.SetMemory(0x841E1B30, Encoding.ASCII.GetBytes(BoxBO2Name.Text));//In Game
            //XBOX.SetMemory(0x82C55D60, Encoding.UTF8.GetBytes(BoxBO2Name.Text));//Write 32 Chars Pregame  // From an open source tool on git never did anything with 360 its not as simple as on PS3 They patched name change stuff on here? lol 
            XBOX.WriteString(0x841E1B30, BoxBO2Name.Text);
            XBOX.WriteString(0x82C55D60, BoxBO2Name.Text);
        }
        private void btnGetBO2Name_Click(object sender, EventArgs e)
        {
            GetTU18Name();

        }

        private void btnSetBO2Name_Click(object sender, EventArgs e)
        {
            SetTU18Name();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            XBOX.CloseConnection(1U);
        }

        private void btnWarmReboot_Click(object sender, EventArgs e)
        {
            try
            {
                XBOX.Reboot(null, null, null, XboxRebootFlags.Warm);
            }
            catch
            {
                LabelStatus.Text = "Can not reach console";
                LabelStatus.ForeColor = Color.Red;
                MessageBox.Show("Failed to Reboot console!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShutdown_Click(object sender, EventArgs e)
        {
            try
            {
                XBOX.ShutDownConsole();
            }
            catch
            {
                LabelStatus.Text = "Can not reach console";
                LabelStatus.ForeColor = Color.Red;
                MessageBox.Show("Failed to Power off console!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRestartGame_Click(object sender, EventArgs e)
        {
            try
            {
                string Gamename = XBOXDebug.RunningProcessInfo.ProgramName;
                string g123 = Gamename.Replace("\\Device\\Harddisk0\\Partition1", "Hdd:"); // From an open source tool on git thanks to the developer for uploading.
                string g223 = g123;
                int length = g223.LastIndexOf("\\");
                XBOX.XNotify(Gamename + "Restarting!");
                XBOX.Reboot(g123, g223.Substring(0, length), null, XboxRebootFlags.Title);
            }
            catch
            {
                LabelStatus.Text = "Can not reach console";
                LabelStatus.ForeColor = Color.Red;
                MessageBox.Show("Failed to Restart Game!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefreshTemps_Click(object sender, EventArgs e)
        {
            BoxCPUTemp.Text = "CPU: " + XBOX.GetTemperature(JRPC.TemperatureType.CPU).ToString() + " C";
            BoxGPUTemp.Text = "GPU: " + XBOX.GetTemperature(JRPC.TemperatureType.GPU).ToString() + " C";
            BoxMotherboardTemp.Text = "Board: " + XBOX.GetTemperature(JRPC.TemperatureType.MotherBoard).ToString() + " C"; // cant get thread working with temps so this will have to do lol
            BoxRAMTemp.Text = "RAM: " + XBOX.GetTemperature(JRPC.TemperatureType.EDRAM).ToString() + " C";
        }

        private void crEaTiiOn_LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string ThemeLink = "https://github.com/EternalModz/CrEaTiiOn-Brotherhood-Theme-Library-NET";
            Process.Start(ThemeLink);
        }

        private void crEaTiiOn_LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string EternalModzGithubLink = "https://github.com/EternalModz";
            Process.Start(EternalModzGithubLink);

        }
    }
}