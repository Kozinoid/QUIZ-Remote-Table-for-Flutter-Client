using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace NET_TCP_Device
{
    public partial class Form1 : Form
    {
        //*****************************************************  CONSTANTS  *******************************************************
        private const int appPort = 8060;           //  TCP Port
        private const int remotePort = 9010;        //  UDP порт для отправки данных
        //private const int localPort = 9010;         //  UDP локальный порт для прослушивания входящих подключений
        private const string addr = "235.5.5.254";  //  UDP IP address
        private const string appName = "#Q#U#I#Z";  // Код приложения для UDP

        //*************************************************************************************************************************
        //*                                                     MAIN FORM                                                         *
        //*************************************************************************************************************************
        ResultTableDataClass resultTablData = new ResultTableDataClass();
        bool wasMinimized = false;

        public Form1()
        {
            InitializeComponent();

            RegistryLoad();

            timer1.Start();

            resultTableView.DataTable = resultTablData;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tcp_divice = new TCP_NET_Server_Device();
            tcp_divice.Connect(TCP_NET_Server_Device.GetConnectionIP(appPort));
            while (!tcp_divice.IsConnected) { };
            tcp_divice.onRecieveMessage += tcp_divice_onRecieveMessage;
            this.Text = "QUIZ   Server address: " + TCP_NET_Server_Device.GetConnectionIP(appPort);

            udp_device = new UDP_NET_Device();
            udp_device.Connect(addr, remotePort, appName);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            udp_device.Disconnect();
            tcp_divice.onRecieveMessage -= tcp_divice_onRecieveMessage;
            tcp_divice.Disconnect();

            RegistrySave();
        }

        private void RegistryLoad()
        {
            //------------------------  Загрузка из реестра!!!!   -----------------------
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey softwareKey = currentUserKey.OpenSubKey("Software", true);
            RegistryKey subSoftwareKey = softwareKey.OpenSubKey("QUIZ_Table", true);

            if (subSoftwareKey == null)
            {
                subSoftwareKey = softwareKey.CreateSubKey("QUIZ_Table");

                subSoftwareKey.SetValue("TextFontName", "Times New Roman");
                subSoftwareKey.SetValue("TextFontSize", 16.0f);
                subSoftwareKey.SetValue("TextFontStyle", (int)FontStyle.Regular);
                subSoftwareKey.SetValue("PictuteFileDirectory", Path.GetDirectoryName(Application.ExecutablePath));
            }

            string name = (string)subSoftwareKey.GetValue("TextFontName");
            float size = float.Parse(subSoftwareKey.GetValue("TextFontSize").ToString());
            FontStyle fs = (FontStyle)(int)subSoftwareKey.GetValue("TextFontStyle");
            resultTableView.AllTextFont = new Font(name, size, fs);
            defaultPictureFileDirectory = (string)subSoftwareKey.GetValue("PictuteFileDirectory");
            if (!Directory.Exists(defaultPictureFileDirectory)) defaultPictureFileDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            subSoftwareKey.Close();
            softwareKey.Close();
        }

        private void RegistrySave()
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey softwareKey = currentUserKey.OpenSubKey("Software", true);
            RegistryKey subSoftwareKey = softwareKey.OpenSubKey("QUIZ_Table", true);

            subSoftwareKey = softwareKey.CreateSubKey("QUIZ_Table");

            subSoftwareKey.SetValue("TextFontName", resultTableView.AllTextFont.Name);
            subSoftwareKey.SetValue("TextFontSize", resultTableView.AllTextFont.Size);
            subSoftwareKey.SetValue("TextFontStyle", (int)resultTableView.AllTextFont.Style);
            subSoftwareKey.SetValue("PictuteFileDirectory", defaultPictureFileDirectory);

            subSoftwareKey.Close();
            softwareKey.Close();
        }

        //**************************************************************************************************************************
        //*                                                    CONNECTION                                                          *
        //**************************************************************************************************************************
        private delegate void TextRefredhDelegate(OnReciveMessageEventArgs e);
        TCP_NET_Server_Device tcp_divice;
        UDP_NET_Device udp_device;

        void tcp_divice_onRecieveMessage(object sender, OnReciveMessageEventArgs e)
        {
            switch (e.command)
            {
                case OnReciveMessageEventArgs.CONNECTED:
                    BeginInvoke(new TextRefredhDelegate(ConnectedMessage), e);
                    break;
                case OnReciveMessageEventArgs.DISCONNECTED:
                    BeginInvoke(new TextRefredhDelegate(DisconnectedMessage), e);
                    break;
                case OnReciveMessageEventArgs.TEXT_MESSAGE:
                    BeginInvoke(new TextRefredhDelegate(ProcessMessage), e);
                    break;
            }
        }

        private void ConnectedMessage(OnReciveMessageEventArgs e)
        {
            this.Text = "QUIZ   Server address: " + TCP_NET_Server_Device.GetConnectionIP(appPort) + " - connected";
        }

        private void DisconnectedMessage(OnReciveMessageEventArgs e)
        {
            this.Text = "QUIZ   Server address: " + TCP_NET_Server_Device.GetConnectionIP(appPort) + " - disconnected";
        }

        //**************************************************************************************************************************
        //*                                              PROCESSING MESSAGES                                                       *
        //**************************************************************************************************************************
        private string GetFirstStringArgFromIndex(string message)
        {
            int begin = message.IndexOf('<') + 1;
            int end = message.IndexOf('>', begin);
            int length = end - begin;
            return message.Substring(begin, length);
        }

        private string GetSecondStringArgFromIndex(string message)
        {
            int index = message.IndexOf('>') + 1;
            int begin = message.IndexOf('<', index) + 1;
            int end = message.IndexOf('>', begin);
            int length = end - begin;
            return message.Substring(begin, length);
        }

        private int GetIntArgFromIndex(string message)
        {
            int index = message.IndexOf('>') + 1;
            int end = message.IndexOf('#', index);
            int length = end - index;
            string str = message.Substring(index, length);
            int res = int.Parse(str);
            return res;
        }

        private void ProcessMessage(OnReciveMessageEventArgs e)
        {
            string message = e.text;
            if (message.Contains("#r#e#f"))
            {
                RefreshTeamScore(GetFirstStringArgFromIndex(e.text), GetIntArgFromIndex(e.text));
            }
            else if(message.Contains("#r#e#n"))
            {
                RenameTeam(GetFirstStringArgFromIndex(e.text), GetSecondStringArgFromIndex(e.text));
            }
            else if (message.Contains("#a#d#d"))
            {
                AddNewTeam(GetFirstStringArgFromIndex(e.text), GetIntArgFromIndex(e.text));
            }
            else if (message.Contains("#d#e#l"))
            {
                DeleteTeam(GetFirstStringArgFromIndex(e.text));
            }
            else if (message.Contains("#t#a#b"))
            {
                NewTab();
            }
            else if (message.Contains("#f#s#m"))
            {
                fullScreenToolStripMenuItem.Checked = !fullScreenToolStripMenuItem.Checked;
            }
        }

        //**************************************************************************************************************************
        //*                                                      COMMANDS                                                          *
        //**************************************************************************************************************************
        private void RefreshTeamScore(string name, int score)
        {
            resultTablData.RefreshTeamScore(name, score);
        }

        private void RenameTeam(string oldName, string newName)
        {
            resultTablData.RenameTeam(oldName, newName);
        }

        private void AddNewTeam(string name, int score)
        {
            resultTablData.AddNewTeam(name, score);
        }

        private void DeleteTeam(string name)
        {
            resultTablData.DeleteTeam(name);
        }

        private void NewTab()
        {
            resultTablData.ClearTabl();
        }

        //**************************************************************************************************************************
        //*                                                  FULL SCREEN MODE                                                      *
        //**************************************************************************************************************************
        private bool secondScreen = false;
        private bool fullScreen = false;
        private Point defLocation = new Point(0, 0);
        private Size defSize = new Size(400, 300);

        public bool FullScreen
        {
            set 
            {
                SetScreenMode(value);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Следим за состоянием второго экрана
            if ((Screen.AllScreens.Length > 1) && (!secondScreen))
            {
                secondScreen = true;
                if (fullScreen) SetScreenMode(true);
            }
            else if ((Screen.AllScreens.Length == 1) && (secondScreen))
            {
                secondScreen = false;
                if (fullScreen) SetScreenMode(true);
            }
        }

        private void SetScreenMode(bool fs)
        {
            if (!fullScreen)
            {
                if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                {
                    this.WindowState = System.Windows.Forms.FormWindowState.Normal;
                    wasMinimized = true;
                }
                else
                {
                    wasMinimized = false;
                }

                defLocation = this.Location;
                defSize = this.Size;
            }

            fullScreen = fs;
            if (fullScreen)
            {
                Console.WriteLine("Loc:" + defLocation + ", sz:" + defSize);

                Screen scr;
                if (secondScreen)
                {
                    scr = Screen.AllScreens[1];
                }
                else
                {
                    scr = Screen.AllScreens[0];
                }
                this.FormBorderStyle = FormBorderStyle.None;
                Point p = new Point(scr.Bounds.Location.X, scr.Bounds.Location.Y);
                Size s = scr.Bounds.Size;
                this.Location = p;
                this.Size = s;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Location = defLocation;
                this.Size = defSize;
                if (wasMinimized) this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
            {
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.S:
                            SavePictureAs();
                            break;
                    }
                }
            }
            else
            {
                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            fullScreenToolStripMenuItem.Checked = !fullScreenToolStripMenuItem.Checked;
                            break;
                        case Keys.Space:
                            fullScreenToolStripMenuItem.Checked = !fullScreenToolStripMenuItem.Checked;
                            break;
                        case Keys.S:
                            SavePicture();
                            break;
                        case Keys.F:
                            SelectFont();
                            break;
                    }
                }
            }
        }

        //**************************************************************************************************************************
        //*                                                  CONTEXT MENU STRIP                                                    *
        //**************************************************************************************************************************
        private void resultTableView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(this, e.Location);
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectFont();
        }

        private void SelectFont()
        {
            FontDialog fd = new FontDialog();
            fd.Font = resultTableView.AllTextFont;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                resultTableView.AllTextFont = new System.Drawing.Font(fd.Font.FontFamily, fd.Font.Size);
            }
        }

        private void exportPictureAsJPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePictureAs();
        }

        private void exportPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePicture();
        }

        private void fullScreenToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            FullScreen = fullScreenToolStripMenuItem.Checked;
        }

        //**************************************************************************************************************************
        //*                                                     SAVE PICTURE                                                       *
        //**************************************************************************************************************************
        string defaultPictureFileName = "Tabl.jpg";
        string defaultPictureFileDirectory = "";
        int defaultPictureFileExtention = 0;
        System.Drawing.Imaging.ImageFormat[] formatArray = { System.Drawing.Imaging.ImageFormat.Jpeg, System.Drawing.Imaging.ImageFormat.Png };

        private void SaveFile(string filename, System.Drawing.Imaging.ImageFormat format)
        {
            Image img = resultTableView.DrawToImage(Screen.AllScreens[0].Bounds.Size);
            img.Save(filename, format);
        }

        private void SavePicture()
        {
            string name = Path.Combine(defaultPictureFileDirectory, defaultPictureFileName);
            if (File.Exists(name))
            {
                SaveFile(name, formatArray[defaultPictureFileExtention]);
            }
            else
            {
                SavePictureAs();
            }
        }

        private void SavePictureAs()
        {
            string name = GetFileName();
            if (name != "")
            {
                defaultPictureFileDirectory = Path.GetDirectoryName(name);
                defaultPictureFileName = Path.GetFileName(name);
                SaveFile(name, formatArray[defaultPictureFileExtention]);
            }
        }

        private string GetFileName()
        {
            string res = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.InitialDirectory = defaultPictureFileDirectory;
            sfd.FileName = defaultPictureFileName;
            sfd.Filter = "jpeg-file (*.jpg)|*.jpg|png-file (*.png)|*.png";
            sfd.FilterIndex = defaultPictureFileExtention + 1;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                res = sfd.FileName;
                defaultPictureFileExtention = sfd.FilterIndex - 1;
            }
            return res;
        }
    }
}
