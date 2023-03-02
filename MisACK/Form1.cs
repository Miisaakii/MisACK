using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MisACK
{
    public partial class Form1 : Form
    {
        #region "Variables"
        public static Point newpoint = new Point();
        public static int x;
        public static int y;

        public Thread loop = new Thread(new ThreadStart(run))
        {
            IsBackground = true
        };

        public static int PressedKey;
        public static string APP_PATH;
        public static string LAG_TIME;
        public static int LAG_METHOD;
        public static bool LOOP_LAG;

        public static SoundPlayer LAG_ON_AUDIO = new SoundPlayer(MisACK.Properties.Resources.on);
        public static SoundPlayer LAG_OFF_AUDIO = new SoundPlayer(MisACK.Properties.Resources.off);

        public static void sleep(int seconds) { System.Threading.Thread.Sleep(seconds); }
        #endregion
        #region "Virtual Key State"
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int VirtualKeyPressed);
        #endregion
        #region "Virutal Key"
        public enum VKEY
        {
            VK_LBUTTON = 0x01, //Left mouse button
            VK_RBUTTON = 0x02, //Right mouse button
            VK_MBUTTON = 0x04, //Middle mouse button (three-button mouse)
            VK_XBUTTON1 = 0x05, //X1 mouse button
            VK_XBUTTON2 = 0x06, //X2 mouse button
            VK_SHIFT = 0x10, //SHIFT key
            VK_CONTROL = 0x11, //CTRL key
            VK_MENU = 0x12, //ALT key
            VK_A = 0x41, //A key
            VK_F = 0x46, //F key
            VK_F1 = 0x70, //F1 key
            VK_F2 = 0x71, //F2 key
            VK_F3 = 0x72, //F3 key
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region "System Move Title Panel"
        private void xMouseDown(object sender, MouseEventArgs e)
        {
            x = Control.MousePosition.X - base.Location.X;
            y = Control.MousePosition.Y - base.Location.Y;
        }
        private void xMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                newpoint = Control.MousePosition;
                newpoint.X -= x;
                newpoint.Y -= y;
                base.Location = newpoint;
            }
        }
        #endregion
        #region "Button Exit"
        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion
        #region "Form Load"
        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome to MisACK a simple program made by Misaki for make your lag on any games.", "MisACK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.gunaGradient2Panel1.MouseDown += this.xMouseDown;
            this.gunaGradient2Panel1.MouseMove += this.xMouseMove;
            this.gunaGradient2Panel1.MouseDown += this.xMouseDown;
            this.gunaGradient2Panel1.MouseMove += this.xMouseMove;

            loop.Start();

            gunaComboBox1.SelectedIndex = 1;
            gunaComboBox2.SelectedIndex = 2;
        }
        #endregion
        #region "Execute CMD"
        public static string ExecuteCommand(string command)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process proc = new Process())
            {
                proc.StartInfo = procStartInfo;
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();

                if (string.IsNullOrEmpty(output))
                    output = proc.StandardError.ReadToEnd();

                return output;
            }
        }

        #endregion
        #region "Select Program"
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Program (*.exe*)|*.exe*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Program found:\n\n" + openFileDialog.FileName, "MisACK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                APP_PATH = openFileDialog.FileName;
            }
        }
        #endregion
        #region "Set lag time"
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            LAG_TIME = guna2TextBox1.Text;
            MessageBox.Show("Timer has been set to: " + LAG_TIME, "MisACK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        #region "Loop Function"
        public async static void run()
        {
            while (true)
            {
                if (GetAsyncKeyState(Convert.ToInt32(PressedKey)) < 0)
                {
                    if (LAG_METHOD == 0)
                    {
                        LAG_ON_AUDIO.Play();
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=in action=block program=\"" + APP_PATH + "\"");
                        sleep(Convert.ToInt32(LAG_TIME));
                        ExecuteCommand("netsh advfirewall firewall delete rule name=\"lagswitch\"");
                        LAG_OFF_AUDIO.Play();
                    }
                    else if (LAG_METHOD == 1)
                    {
                        LAG_ON_AUDIO.Play();
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=out action=block program=\"" + APP_PATH + "\"");
                        sleep(Convert.ToInt32(LAG_TIME));
                        ExecuteCommand("netsh advfirewall firewall delete rule name=\"lagswitch\"");
                        LAG_OFF_AUDIO.Play();
                    }
                    else if (LAG_METHOD == 2)
                    {
                        LAG_ON_AUDIO.Play();
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=out action=block program=\"" + APP_PATH + "\"");
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=in action=block program=\"" + APP_PATH + "\"");
                        sleep(Convert.ToInt32(LAG_TIME));
                        ExecuteCommand("netsh advfirewall firewall delete rule name=\"lagswitch\"");
                        LAG_OFF_AUDIO.Play();
                    }
                }
                
                if (LOOP_LAG)
                {
                    if (LAG_METHOD == 0)
                    {
                        LAG_ON_AUDIO.Play();
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=in action=block program=\"" + APP_PATH + "\"");
                        sleep(Convert.ToInt32(LAG_TIME));
                        ExecuteCommand("netsh advfirewall firewall delete rule name=\"lagswitch\"");
                        LAG_OFF_AUDIO.Play();
                    }
                    else if (LAG_METHOD == 1)
                    {
                        LAG_ON_AUDIO.Play();
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=out action=block program=\"" + APP_PATH + "\"");
                        sleep(Convert.ToInt32(LAG_TIME));
                        ExecuteCommand("netsh advfirewall firewall delete rule name=\"lagswitch\"");
                        LAG_OFF_AUDIO.Play();
                    }
                    else if (LAG_METHOD == 2)
                    {
                        LAG_ON_AUDIO.Play();
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=out action=block program=\"" + APP_PATH + "\"");
                        ExecuteCommand("netsh advfirewall firewall add rule name =\"lagswitch\" dir=in action=block program=\"" + APP_PATH + "\"");
                        sleep(Convert.ToInt32(LAG_TIME));
                        ExecuteCommand("netsh advfirewall firewall delete rule name=\"lagswitch\"");
                        LAG_OFF_AUDIO.Play();
                    }
                }
            }
        }
        #endregion
        #region "Start Loop lags"
        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox1.Checked)
            {
                LOOP_LAG = true;
            }
            else
            {
                LOOP_LAG = false;
            }
        }
        #endregion
        #region "Select key press"
        private void gunaComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gunaComboBox1.SelectedIndex == 0)
            {
                PressedKey = 0x01;
            }
            else if (gunaComboBox1.SelectedIndex == 1)
            {
                PressedKey = 0x02;
            }
            else if (gunaComboBox1.SelectedIndex == 2)
            {
                PressedKey = 0x04;
            }
            else if (gunaComboBox1.SelectedIndex == 3)
            {
                PressedKey = 0x05;
            }
            else if (gunaComboBox1.SelectedIndex == 4)
            {
                PressedKey = 0x06;
            }
            else if (gunaComboBox1.SelectedIndex == 5)
            {
                PressedKey = 0x10;
            }
            else if (gunaComboBox1.SelectedIndex == 6)
            {
                PressedKey = 0x11;
            }
            else if (gunaComboBox1.SelectedIndex == 7)
            {
                PressedKey = 0x12;
            }
            else if (gunaComboBox1.SelectedIndex == 8)
            {
                PressedKey = 0x41;
            }
            else if (gunaComboBox1.SelectedIndex == 9)
            {
                PressedKey = 0x46;
            }
            else if (gunaComboBox1.SelectedIndex == 10)
            {
                PressedKey = 0x70;
            }
            else if (gunaComboBox1.SelectedIndex == 11)
            {
                PressedKey = 0x71;
            }
            else if (gunaComboBox1.SelectedIndex == 12)
            {
                PressedKey = 0x72;
            }
        }
        #endregion
        #region "Select lag methods"
        private void gunaComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gunaComboBox2.SelectedIndex == 0)
            {
                LAG_METHOD = 0;
            }
            else if (gunaComboBox2.SelectedIndex == 1)
            {
                LAG_METHOD = 1;
            }
            else if (gunaComboBox2.SelectedIndex == 2)
            {
                LAG_METHOD = 2;
            }
        }
        #endregion
    }
}
