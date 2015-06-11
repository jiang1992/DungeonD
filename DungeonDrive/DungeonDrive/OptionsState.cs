using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace DungeonDrive
{
    public class OptionsState : State
    {
        private int selection = 0;
        private Font titleFont = new Font("Arial", 36);
        private Font selectionFont = new Font("Arial", 16);

        private String[] options;

        public OptionsState(MainForm form) : base(form) { updateOptions(); }

        private void updateOptions()
        {
            options = new String[]
            {
                "Close: " + Properties.Settings.Default.CloseKey.ToString(),
                "Up: " + Properties.Settings.Default.UpKey.ToString(),
                "Left: " + Properties.Settings.Default.LeftKey.ToString(),
                "Down: " + Properties.Settings.Default.DownKey.ToString(),
                "Right: " + Properties.Settings.Default.RightKey.ToString(),
                "Select: " + Properties.Settings.Default.SelectKey.ToString(),
                "Inventory: " + Properties.Settings.Default.InventoryKey.ToString(),
                "Skill Tree: " + Properties.Settings.Default.SkillTreeKey.ToString(),
                "Switch Skill: " + Properties.Settings.Default.SwitchSkillKey.ToString(),
                "Delete Attack: " + Properties.Settings.Default.DeleteAttackKey.ToString(),
                "Controller Enabled: " + Properties.Settings.Default.ControllerEnabled.ToString(),
                "Fullscreen: " + Properties.Settings.Default.FullScreen.ToString(),
                "Sound: " + Properties.Settings.Default.SoundEnabled.ToString(),
                "Exit"
            };
        }

        private void tryToggleMusic()
        {
            State curState = this.parent;

            while(curState != null)
            {
                if(curState is GameState)
                {
                    GameState gs = (GameState)curState;
                    if (Properties.Settings.Default.SoundEnabled)
                        gs.music.Play();
                    else
                        gs.music.Stop();
                }

                curState = curState.parent;
            }
        }

        private void rebind(object sender, KeyEventArgs e)
        {
            if (selection == 0)
                Properties.Settings.Default.CloseKey = e.KeyCode;
            else if (selection == 1)
                Properties.Settings.Default.UpKey = e.KeyCode;
            else if (selection == 2)
                Properties.Settings.Default.LeftKey = e.KeyCode;
            else if (selection == 3)
                Properties.Settings.Default.DownKey = e.KeyCode;
            else if (selection == 4)
                Properties.Settings.Default.RightKey = e.KeyCode;
            else if (selection == 5)
                Properties.Settings.Default.SelectKey = e.KeyCode;
            else if (selection == 6)
                Properties.Settings.Default.InventoryKey = e.KeyCode;
            else if (selection == 7)
                Properties.Settings.Default.SkillTreeKey = e.KeyCode;
            else if (selection == 8)
                Properties.Settings.Default.SwitchSkillKey = e.KeyCode;
            else if (selection == 9)
                Properties.Settings.Default.DeleteAttackKey = e.KeyCode;

            updateOptions();
            Properties.Settings.Default.Save();

            form.KeyDown -= this.rebind;
            form.KeyDown += this.keyDown;

            form.Invalidate();
        }

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
                if (selection >= 0 && selection <= 9)
                {
                    if (selection == 0)
                        Properties.Settings.Default.CloseKey = System.Windows.Forms.Keys.None;
                    else if (selection == 1)
                        Properties.Settings.Default.UpKey = System.Windows.Forms.Keys.None;
                    else if (selection == 2)
                        Properties.Settings.Default.LeftKey = System.Windows.Forms.Keys.None;
                    else if (selection == 3)
                        Properties.Settings.Default.DownKey = System.Windows.Forms.Keys.None;
                    else if (selection == 4)
                        Properties.Settings.Default.RightKey = System.Windows.Forms.Keys.None;
                    else if (selection == 5)
                        Properties.Settings.Default.SelectKey = System.Windows.Forms.Keys.None;
                    else if (selection == 6)
                        Properties.Settings.Default.InventoryKey = System.Windows.Forms.Keys.None;
                    else if (selection == 7)
                        Properties.Settings.Default.SkillTreeKey = System.Windows.Forms.Keys.None;
                    else if (selection == 8)
                        Properties.Settings.Default.SwitchSkillKey = System.Windows.Forms.Keys.None;
                    else if (selection == 9)
                        Properties.Settings.Default.DeleteAttackKey = System.Windows.Forms.Keys.None;

                    updateOptions();

                    form.KeyDown += this.rebind;
                    form.KeyDown -= this.keyDown;
                }
                else if (selection == 10)
                {
                    Properties.Settings.Default.ControllerEnabled = !Properties.Settings.Default.ControllerEnabled;
                    updateOptions();
                    Properties.Settings.Default.Save();
                }
                else if (selection == 11)
                {
                    form.setFullscreen(!Properties.Settings.Default.FullScreen);
                    updateOptions();
                    Properties.Settings.Default.Save();
                }
                else if (selection == 12)
                {
                    Properties.Settings.Default.SoundEnabled = !Properties.Settings.Default.SoundEnabled;
                    updateOptions();
                    tryToggleMusic();
                    Properties.Settings.Default.Save();
                }
                else if (selection == 13)
                {
                    this.close();
                }
            }

            form.Invalidate();
        }

        public override void updateInput()
        {
            GamePadState current = GamePad.GetState(PlayerIndex.One);

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
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
                if (selection >= 0 && selection <= 9)
                {
                    /*if (selection == 0)
                        current.Buttons.B
                    else if (selection == 1)
                        Properties.Settings.Default.UpKey = System.Windows.Forms.Keys.N
                    else if (selection == 2)
                        Properties.Settings.Default.LeftKey = System.Windows.Forms.Keys.None;
                    else if (selection == 3)
                        Properties.Settings.Default.DownKey = System.Windows.Forms.Keys.None;
                    else if (selection == 4)
                        Properties.Settings.Default.RightKey = System.Windows.Forms.Keys.None;
                    else if (selection == 5)
                        Properties.Settings.Default.SelectKey = System.Windows.Forms.Keys.None;
                    else if (selection == 6)
                        Properties.Settings.Default.InventoryKey = System.Windows.Forms.Keys.None;
                    else if (selection == 7)
                        Properties.Settings.Default.Attack1Key = System.Windows.Forms.Keys.None;
                    else if (selection == 8)
                        Properties.Settings.Default.Attack2Key = System.Windows.Forms.Keys.None;
                    else if (selection == 9)
                        Properties.Settings.Default.Attack3Key = System.Windows.Forms.Keys.None;*/

                    updateOptions();

                    /*form.KeyDown += this.rebind;
                    form.KeyDown -= this.keyDown;*/
                }
                else if (selection == 10)
                {
                    Properties.Settings.Default.ControllerEnabled = !Properties.Settings.Default.ControllerEnabled;
                    updateOptions();
                    Properties.Settings.Default.Save();
                    System.Threading.Thread.Sleep(100);
                }
                else if (selection == 11)
                {
                    form.setFullscreen(!Properties.Settings.Default.FullScreen);
                    updateOptions();
                    Properties.Settings.Default.Save();
                    System.Threading.Thread.Sleep(100);
                }
                else if (selection == 12)
                {
                    Properties.Settings.Default.SoundEnabled = !Properties.Settings.Default.SoundEnabled;
                    updateOptions();
                    Properties.Settings.Default.Save();
                    System.Threading.Thread.Sleep(100);
                }
                else if (selection == 13)
                {
                    this.close();
                    System.Threading.Thread.Sleep(100);
                }
            }

            form.Invalidate();
        }

        public override void paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(System.Drawing.Color.FromArgb(20, 20, 20));

            StringFormat align = new StringFormat();
            align.Alignment = StringAlignment.Center;
            align.LineAlignment = StringAlignment.Far;

            g.DrawString("Options", titleFont, Brushes.White, new RectangleF(0, 0, form.ClientSize.Width, 0.3f * form.ClientSize.Height), align);

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
            g.DrawString(selectString, selectionFont, Brushes.White, new RectangleF(0, 0.3f * form.ClientSize.Height, form.ClientSize.Width, 0.7f * form.ClientSize.Height), align);
        }
    }
}
