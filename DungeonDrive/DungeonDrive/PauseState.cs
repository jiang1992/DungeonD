using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace DungeonDrive
{
    public class PauseState : State
    {
        private int selection = 0;
        private Font titleFont = new Font("Arial", 36);
        private Font selectionFont = new Font("Arial", 16);
        private GameState state { get { return (GameState)parent; } }

        private String[] options = new String[]
        {
            "Resume",
            "Save Game",
            "Options",
            "Exit"
        };

        public PauseState(MainForm form) : base(form) { }

        public override void keyUp(object sender, KeyEventArgs e) { }
        public override void mouseDown(object sender, MouseEventArgs e) { }
        public override void mouseUp(object sender, MouseEventArgs e) { }
        public override void mouseMove(object sender, MouseEventArgs e) { }
        public override void tick(object sender, EventArgs e) 
        {
            GamePadState current = GamePad.GetState(PlayerIndex.One);

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected)
                updateInput();
        }

        public override void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Properties.Settings.Default.CloseKey)
            {
                this.close();
            }
            else if (e.KeyCode == Properties.Settings.Default.UpKey)
            {
                if (--selection < 0)
                    selection = options.Length - 1;
            }
            else if (e.KeyCode == Properties.Settings.Default.DownKey)
            {
                selection = (selection + 1) % options.Length;
            }
            else if (e.KeyCode == Properties.Settings.Default.SelectKey)
            {
                if (selection == 0)
                    this.close();
                else if (selection == 1)
                    state.saveGame();
                else if (selection == 2)
                    this.addChildState(new OptionsState(form), true, true);
                else if (selection == 3)
                {
                    state.music.Stop();
                    parent.close();
                }
            }

            form.Invalidate();
        }

        public override void updateInput()
        {
            GamePadState current = GamePad.GetState(PlayerIndex.One);

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.Start == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.close();
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (--selection < 0)
                    selection = options.Length - 1;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                selection = (selection + 1) % options.Length;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (selection == 0)
                {
                    this.close();
                    System.Threading.Thread.Sleep(100);
                }
                else if (selection == 1)
                {
                    state.saveGame();
                    System.Threading.Thread.Sleep(100);
                }
                else if (selection == 2)
                {
                    this.addChildState(new OptionsState(form), true, true);
                    System.Threading.Thread.Sleep(100);
                }
                else if (selection == 3)
                {
                    parent.close();
                    System.Threading.Thread.Sleep(100);
                }
            }

            form.Invalidate();
        }



        public override void paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(Properties.Resources.info, form.ClientSize.Width / 4, form.ClientSize.Height / 4, form.ClientSize.Width / 2, form.ClientSize.Height / 2);

            StringFormat align = new StringFormat();
            align.Alignment = StringAlignment.Center;
            align.LineAlignment = StringAlignment.Far;

            g.DrawString("Paused", titleFont, Brushes.White, new RectangleF(form.ClientSize.Width / 4, form.ClientSize.Height / 4, form.ClientSize.Width / 2, form.ClientSize.Height / 4), align);

            String selectString = "";
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selection)
                    selectString += "> ";
                selectString += options[i];
                if (i == selection)
                    selectString += " <";
                if (i != options.Length - 1)
                    selectString += "\n";
            }
            align.LineAlignment = StringAlignment.Near;
            g.DrawString(selectString, selectionFont, Brushes.White, new RectangleF(form.ClientSize.Width / 4, form.ClientSize.Height / 2, form.ClientSize.Width / 2, form.ClientSize.Height / 4), align);
        }
    }
}
