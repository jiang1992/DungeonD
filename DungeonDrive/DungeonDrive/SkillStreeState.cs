using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace DungeonDrive
{
    class SkillStreeState : State
    {
        public static int availablePoints = 1;
        //horizonal 
        public static int skillList = 7;
        //verticle, spell level
        public static int skillLevel = 3;
        public static Spell spellSelected;


        public static Spell[] spellStored = new Spell[skillList];

        private System.Drawing.Rectangle[,] skillFrame = new System.Drawing.Rectangle[skillList, skillLevel];
        private Bitmap[,] skillFrameImages = new Bitmap[skillList, skillLevel];

        private System.Drawing.Rectangle[,] skillSet = new System.Drawing.Rectangle[skillList, skillLevel];
        // already learnt spells
        private Bitmap[,] skillSetImages = new Bitmap[skillList, skillLevel];
        private Bitmap[,] _skillSetImages = new Bitmap[skillList, skillLevel]; // images of spells that has not learnt
        // not learnt spells
        //private Bitmap[,] _skillSetImages = new Bitmap[skillList, skillLevel];

        //frames of a learnt spell
        private Bitmap spellFrame = Properties.Resources.frame_7_blue;
        //frames of a not leant spell, but can be leant
        private Bitmap _spellFrame = Properties.Resources.frame_0_grey;
        //frames of a not learnt spell, and the hero doesnt fullfill the pre-req 
        private Bitmap _NAspellFrame = Properties.Resources.frame_4_grey;

        private int iconSize;
        private Bitmap highLight = Properties.Resources.frame_0_red;

        private System.Drawing.Rectangle skillTreeRectangle;
        private int skillSelected, levelSelected;
        private int skillMouseOver, levelMouseOver;
        private bool selectedOrNot = false, mouseOverOrNot = false;
        private bool isSettled = false;
        private int toLeft = 10, toTop = 10;
        private Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red);
        private int padding = 12;
        private System.Drawing.Point highlight2 = new System.Drawing.Point(6, 0);


        public SkillStreeState(MainForm form)
            : base(form)
        {
           

            for (int i = 0; i < skillList; i++)
            {
                for (int j = 0; j < skillLevel; j++)
                {
                    _skillSetImages[i, j] = Properties.Resources.empty;
                    if (GameState.heroSkill[i, j]) // if hero learnt this spell
                    {
                        skillSetImages[i, j] = Properties.Resources.empty;
                        
                        skillFrameImages[i, j] = this.spellFrame;

                    }
                    else
                    {

                        if (skillIsAvailable(i, j)) // skill is available
                        {
                            skillSetImages[i, j] = Properties.Resources.empty;
                            skillFrameImages[i, j] = this._spellFrame;


                        }
                        else
                        {
                            skillSetImages[i, j] = Properties.Resources.empty;
                            skillFrameImages[i, j] = this._NAspellFrame;
                        }

                    }
                }
            }

            addSpell(new LighteningBall(), 0);
            addSpell(new RuneOfFire(), 1);
            addSpell(new EnergyBarrier(), 2);
            addSpell(new CrusingFireBall(), 3);
            addSpell(new Pyroblast(), 4);
            addSpell(new ShadowStep(), 5);
            addSpell(new GravityForceField(), 6);

        }
        public bool skillIsAvailable(int i, int j)
        {

            if (j == 0) { return true; }
            else
            {
                if (GameState.heroSkill[i, j - 1])
                {
                    return true;
                }
            }
            return false;

        }

        private void addSpell(Spell spell, int i)
        {

            for (int j = 0; j < skillLevel; j++)
            {
                skillSetImages[i, j] = spell.spellIcon[j];
                _skillSetImages[i,j] = spell._spellIcon[j];
                spellStored[i] = spell;
            }

        }

        private GameState state { get { return (GameState)parent; } }
        public override void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Properties.Settings.Default.CloseKey || e.KeyCode == Properties.Settings.Default.SkillTreeKey)
                this.close();
        }
        public override void mouseMove(object sender, MouseEventArgs e)
        {
            System.Drawing.Rectangle click = new System.Drawing.Rectangle(e.X, e.Y, 1, 1);
            if (selectedOrNot == true)
            {
                for (int j = 0; j < skillLevel; j++)
                {
                    if (skillFrame[this.skillSelected, j].Contains(click))
                    {
                        this.levelMouseOver = j;
                        this.skillMouseOver = this.skillSelected;
                        mouseOverOrNot = true;
                        return;
                    }

                }
                mouseOverOrNot = false;
                return;
            }
            for (int i = 0; i < skillList; i++)
            {
                for (int j = 0; j < skillLevel; j++)
                {
                    if (skillFrame[i, j].Contains(click))
                    {
                        skillMouseOver = i;
                        levelMouseOver = j;
                        mouseOverOrNot = true;
                        return;
                    }
                }
            }

            mouseOverOrNot = false;
        }
        private RectangleF getBoxBounds(double i, int j)
        {

            System.Drawing.Rectangle rect = this.skillTreeRectangle;
            int size = form.ClientSize.Height / 10;
            int y = form.ClientSize.Height / (skillLevel + 1);
            int x = form.ClientSize.Width / (skillList + 1);
            iconSize = size;
            skillFrame[(int)i, j] = new System.Drawing.Rectangle(form.ClientSize.Width - size / 2 - (x * ((int)i + 1)), form.ClientSize.Height - size / 2 - (y * (j + 1)), size, size);
            return skillFrame[(int)i, j];

        }

        private RectangleF getIconBoxBounds(int i, int j)
        {

            System.Drawing.Rectangle rect = this.skillTreeRectangle;
            int size = form.ClientSize.Height / 10;
            int y = form.ClientSize.Height / (skillLevel + 1);
            int x = form.ClientSize.Width / (skillList + 1);
            iconSize = size;
            skillSet[i, j] = new System.Drawing.Rectangle(form.ClientSize.Width - size / 2 - (x * (i + 1)) + padding / 2, form.ClientSize.Height - size / 2
                - (y * (j + 1)) + padding / 2, size - padding, size - padding);
            return new System.Drawing.Rectangle(form.ClientSize.Width - size / 2 - (x * (i + 1)) + padding / 2, form.ClientSize.Height - size / 2
                - (y * (j + 1)) + padding / 2, size - padding, size - padding);

        }

        public override void mouseDown(object sender, MouseEventArgs e)
        {
            System.Drawing.Rectangle click = new System.Drawing.Rectangle(e.X, e.Y, 1, 1);
            if (e.Button == MouseButtons.Left)
            {
                
                if (selectedOrNot == true)
                {
                    for (int j = 0; j < skillLevel; j++)
                    {
                        if (skillFrame[this.skillSelected, j].Contains(click))
                        {
                            this.levelSelected = j;
                            selectedOrNot = true;
                            tryToLearn(this.skillSelected, this.levelSelected);
                            return;
                        }

                    }
                    selectedOrNot = false;
                    return;
                }

                for (int i = 0; i < skillList; i++)
                {
                    for (int j = 0; j < skillLevel; j++)
                    {
                        if (skillFrame[i, j].Contains(click))
                        {
                            this.skillSelected = i;
                            this.levelSelected = j;
                            selectedOrNot = true; 
                            spellSelected = spellStored[i];
                            return;
                        }
                    }

                }
                this.selectedOrNot = false;
            }
            else if(e.Button == MouseButtons.Right){
                spellSelected = null;
                for (int j = 0; j < skillLevel; j++)
                {
                    if (skillFrame[this.skillSelected, j].Contains(click))
                    {
                        
                        tryToUnlearn(this.skillSelected, j);
                    }

                }
            }

        }
        public override void mouseUp(object sender, MouseEventArgs e)
        {
        }



        private void tryToLearn(int i, int j) {

            if (skillIsAvailable(i, j) == true)
            {
                if (availablePoints >= 1)
                {
                    skillFrameImages[i, j] = spellFrame;
                    GameState.heroSkill[i, j] = true;
                    this.skillFrameImages[i, j] = spellFrame;
                    availablePoints = availablePoints - 1;
                }
            }
            else { 
                
            }       
        }

        private void tryToUnlearn(int i, int j) {
            if (GameState.heroSkill[i,j]) {
                if ( j == SkillStreeState.skillLevel-1)
                {
                    GameState.heroSkill[i, j] = false;
                    availablePoints = availablePoints + 1;
                    this.skillFrameImages[i,j] = _spellFrame;
                }
                else if(GameState.heroSkill[i,j+1] == false){

                    GameState.heroSkill[i, j] = false;
                    availablePoints = availablePoints + 1;
                    this.skillFrameImages[i, j] = _spellFrame;
                }
            }
            else { }
        
        }
        public override void tick(object sender, EventArgs e)
        {

            if (selectedOrNot == true)
            {
                if ((this.skillFrame[skillSelected, levelSelected].X - this.form.ClientSize.Width / 2) > 25)
                {
                    isSettled = false;
                    for (int j = 0; j < skillLevel; j++)
                    {
                        this.skillFrame[this.skillSelected, j].X -= 25;
                        this.skillSet[this.skillSelected, j].X -= 25;
                    }

                }
                else if ((this.skillFrame[skillSelected, levelSelected].X < this.form.ClientSize.Width / 2))
                {
                    isSettled = false;
                    for (int j = 0; j < skillLevel; j++)
                    {
                        this.skillFrame[this.skillSelected, j].X += 25;
                        this.skillSet[this.skillSelected, j].X += 25;
                    }
                }
                else
                {
                    isSettled = true;
                    
                }
            }

            GamePadState current = GamePad.GetState(PlayerIndex.One);

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected)
                updateInput();
        }
        public override void keyUp(object sender, KeyEventArgs e)
        {

        }

        public override void updateInput()
        {
            GamePadState current = GamePad.GetState(PlayerIndex.One);
            //System.Drawing.Rectangle[,] skills = skillFrame;

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.RightShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.close();
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (--highlight2.X < 0)
                    highlight2.X = skillList - 1;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                highlight2.X = (highlight2.X + 1) % skillList;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (--highlight2.Y < 0)
                    highlight2.Y = skillLevel - 1;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                highlight2.Y = (highlight2.Y + 1) % skillLevel;
                System.Threading.Thread.Sleep(100);
            }
            
            //System.Drawing.Rectangle click = new System.Drawing.Rectangle(highlight2.X, highlight2.Y, 1, 1);
            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                System.Drawing.Rectangle click = new System.Drawing.Rectangle(highlight2.X, highlight2.Y, 1, 1);

                if (selectedOrNot == true)
                {
                    for (int j = 0; j < skillLevel; j++)
                    {
                        if (this.skillSelected == highlight2.X && j == highlight2.Y)
                        {
                            this.levelSelected = j;
                            selectedOrNot = true;
                            tryToLearn(this.skillSelected, this.levelSelected);
                            return;
                        }

                    }
                    selectedOrNot = false;
                    return;
                }

                for (int i = 0; i < skillList; i++)
                {
                    for (int j = 0; j < skillLevel; j++)
                    {
                        if (i == highlight2.X && j == highlight2.Y)
                        {
                            this.skillSelected = i;
                            this.levelSelected = j;
                            selectedOrNot = true;
                            spellSelected = spellStored[i];
                            return;
                        }
                    }

                }
                this.selectedOrNot = false;

                System.Threading.Thread.Sleep(150);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.X == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                spellSelected = null;
                for (int j = 0; j < skillLevel; j++)
                {
                    if (this.skillSelected == highlight2.X && j == highlight2.Y)
                    {

                        tryToUnlearn(this.skillSelected, j);
                    }

                }
                System.Threading.Thread.Sleep(150);
            }
        }



        private System.Drawing.Rectangle getBackgroundRectangle(int top, int left)
        {

            return new System.Drawing.Rectangle(this.form.ClientSize.Width / left, this.form.ClientSize.Height / top,
                (left - 2) * form.ClientSize.Width / left, (top - 2) * form.ClientSize.Height / top);
        }

        private void drawSingleList(PaintEventArgs e, int i)
        {
            Graphics g = e.Graphics;
            GamePadState current = GamePad.GetState(PlayerIndex.One);

            for (int j = 0; j < skillLevel; j++)
            {
                g.DrawImage(this.skillFrameImages[i, j], this.skillFrame[i, j]);
                if (GameState.heroSkill[i, j])
                {
                    g.DrawImage(skillSetImages[i, j], this.skillSet[i, j]);
                    if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && highlight2.Y == j)
                        g.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red)), getBoxBounds(2, j));
                }
                else 
                {
                    g.DrawImage(_skillSetImages[i, j], this.skillSet[i, j]);
                    if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && highlight2.Y == j)
                        g.FillEllipse(new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red)), getBoxBounds(2, j));
                }

            }

        }

        private void connectSkillList(PaintEventArgs e, int i)
        {
            Graphics g = e.Graphics;
            
            pen.Width = 10;
            for (int j = 0; j < skillLevel - 1; j++)
            {
                if (skillIsAvailable(i, j+1)) { this.pen.Color = System.Drawing.Color.Green; }
                else { this.pen.Color = System.Drawing.Color.Gray; }
                g.DrawLine(pen, new System.Drawing.Point(skillFrame[i, j].X + iconSize / 2, skillFrame[i, j].Y),
                    new System.Drawing.Point(skillFrame[i, j + 1].X + iconSize / 2, skillFrame[i, j + 1].Y + +iconSize));
                
            }
        }
        public override void paint(object sender, PaintEventArgs e)
        {

            skillTreeRectangle = this.getBackgroundRectangle(toLeft, toTop);
            Graphics g = e.Graphics;
            GamePadState current = GamePad.GetState(PlayerIndex.One);

            // g.DrawImage(this.skillTreeBackGround, this.skillTreeRectangle);

            if (selectedOrNot == false)
            {
                for (int i = 0; i < skillList; i++)
                {
                    for (int j = 0; j < skillLevel; j++)
                    {
                        if (GameState.heroSkill[i, j] == true)
                        {
                            g.DrawImage(skillFrameImages[i,j], getBoxBounds(i, j));
                            g.DrawImage(skillSetImages[i, j], getIconBoxBounds(i, j));
                            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && highlight2.X == i && highlight2.Y == j)
                                g.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Yellow)), getBoxBounds(i, j));
                        }
                        else
                        {
                            g.DrawImage(skillFrameImages[i,j], getBoxBounds(i, j));
                            g.DrawImage(_skillSetImages[i, j], getIconBoxBounds(i, j));
                            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && highlight2.X == i && highlight2.Y == j)
                                g.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Yellow)), getBoxBounds(i, j));
                        }
                    }
                    /*if (highlight2.X == i)
                        g.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Yellow)), getBoxBounds(i, 0));*/
                }
            }
            else if (selectedOrNot == true)
            {
                g.DrawImage(_spellFrame, this.skillFrame[this.skillSelected, this.levelSelected]);
                drawSingleList(e, this.skillSelected);


                if (isSettled) { 
                    connectSkillList(e, this.skillSelected);                    
                }
            }
            if (mouseOverOrNot) { g.DrawImage(Properties.Resources.empty, this.skillFrame[this.skillMouseOver, this.levelMouseOver]); }

        }
    }
}