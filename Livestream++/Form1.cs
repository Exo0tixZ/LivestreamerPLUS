using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Livestream__
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();

            int id = 0;     
            RegisterHotKey(Handle, id, (int)KeyModifier.Alt, Keys.L.GetHashCode());
        }

        #region GlobalHotkey
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
 
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }
 

 
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
 
            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */
 
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.
    
                Show();
                WindowState = FormWindowState.Normal;
                

            }
        }
        #endregion

        void ExecuteCommand(string channel)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " +   "livestreamer twitch.tv/" + channel + " " + quality() );
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            
            process.Close();
        }

        private string quality()
        {
            string selectedItem = metroComboBox1.SelectedItem.ToString();

            if(selectedItem == "Source (Best)")
            {
                return "source";
            }
            if (selectedItem == "High")
            {
                return "high";
            }
            if (selectedItem == "Medium")
            {
                return "medium";
            }
            if (selectedItem == "Low")
            {
                return "low";
            }
            if (selectedItem == "Mobile (Worst)")
            {
                return "mobile";
            }
            return "audio";
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, 0);    
            Application.Exit();
        }



        private void btn_startStream_Click(object sender, EventArgs e)
        {
            if (txt_Channel.Text == null || metroComboBox1.SelectedItem == null)
            {
                MetroFramework.MetroMessageBox.Show(this,
                    "Please determine a proper username and/or select a Stream quality.");
            }
            else
            {
                Hide();
                ExecuteCommand(txt_Channel.Text.ToLower());
                
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }


    }
}
