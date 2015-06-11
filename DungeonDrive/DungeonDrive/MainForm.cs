using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace DungeonDrive
{
    public class MainForm : Form
    {
        public Timer timer = new Timer();

        public MainForm()
        {
            this.Text = "Dungeon Drive (D:)";
            this.DoubleBuffered = true;
            timer.Interval = 17;

            setFullscreen(Properties.Settings.Default.FullScreen);
            timer.Start();
            new TitleState(this).open();
        }

        public void setFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Resize -= this.resize;
            }
            else
            {
                this.Resize += this.resize;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.ClientSize = new Size(Properties.Settings.Default.Width, Properties.Settings.Default.Height);
            }

            Properties.Settings.Default.FullScreen = fullscreen;
            Properties.Settings.Default.Save();
        }

        public void resize(object sender, EventArgs e)
        {
            Properties.Settings.Default.Width = this.ClientSize.Width;
            Properties.Settings.Default.Height = this.ClientSize.Height;
            Properties.Settings.Default.Save();

            this.Invalidate();
        }
    }
}
