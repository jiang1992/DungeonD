using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace DungeonDrive
{
    class MessageState : State
    {
        private String[] messages;
        private int messageLoc = 0;
        private int typeWriterLoc = 0;
        private Font font = new Font("Arial", 24);

        public MessageState(MainForm form, String[] messages) : base(form)
        {
            this.messages = messages;
        }

        public override void mouseMove(object sender, MouseEventArgs e) { }
        public override void mouseUp(object sender, MouseEventArgs e) { }
        public override void keyUp(object sender, KeyEventArgs e) { }

        public override void tick(object sender, EventArgs e)
        {
            if (typeWriterLoc < messages[messageLoc].Length)
            {
                typeWriterLoc++;
                form.Invalidate();
            }

            GamePadState current = GamePad.GetState(PlayerIndex.One);

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected)
                updateInput();
        }

        private void nextMessage()
        {
            if (typeWriterLoc < messages[messageLoc].Length)
            {
                typeWriterLoc = messages[messageLoc].Length;
                form.Invalidate();
            }
            else if (messageLoc < messages.Length - 1)
            {
                messageLoc++;
                typeWriterLoc = 0;
                form.Invalidate();
            }
            else
            {
                this.close();
            }
        }

        public override void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Properties.Settings.Default.SelectKey)
                nextMessage();
        }

        public override void mouseDown(object sender, MouseEventArgs e)
        {
            nextMessage();
        }

        public override void updateInput()
        {
            GamePadState current = GamePad.GetState(PlayerIndex.One);

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                nextMessage();
                System.Threading.Thread.Sleep(200);
            }
        }


        public override void paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(Properties.Resources.info, form.ClientSize.Width / 4, form.ClientSize.Height / 4, form.ClientSize.Width / 2, form.ClientSize.Height / 2);

            StringFormat align = new StringFormat();
            align.Alignment = StringAlignment.Center;
            align.LineAlignment = StringAlignment.Center;

            g.DrawString(messages[messageLoc].Substring(0, typeWriterLoc), font, Brushes.White, new RectangleF(form.ClientSize.Width / 4, form.ClientSize.Height / 4, form.ClientSize.Width / 2, form.ClientSize.Height / 2), align);
        }
    }
}
