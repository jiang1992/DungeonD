using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace DungeonDrive
{
    public class InventoryState : State
    {
        private Item selection = null;
        private PointF selectOrigin = new PointF(-1, -1);
        private System.Drawing.Point dragLoc = new System.Drawing.Point(-1, -1);
        private System.Drawing.Point mouse = new System.Drawing.Point(-1, -1);
        private bool dragging = false;
        private GameState state { get { return (GameState)parent; } }
        private Item mouseOverItem = null;
        private System.Drawing.Point highlight = new System.Drawing.Point(0, 0);

        private Bitmap boxImg = new Bitmap(Properties.Resources.box);
        private Bitmap helmetImg = new Bitmap(Properties.Resources.helmet);
        private Bitmap armorImg = new Bitmap(Properties.Resources.armor);
        private Bitmap legsImg = new Bitmap(Properties.Resources.legs);
        private Bitmap shieldImg = new Bitmap(Properties.Resources.shield);
        private Bitmap weaponImg = new Bitmap(Properties.Resources.weapon);
        private Bitmap infoImg = new Bitmap(Properties.Resources.info);
        private Font font;

        public InventoryState(MainForm form) : base(form)
        {
            font = new Font("Arial", form.ClientSize.Height / 70);
        }

        private RectangleF getBoxBounds(float i, float j)
        {
            Item[][] inventory = state.inventory;
            float boxSize = form.ClientSize.Height / (inventory.Length + 4);
            float padding = form.ClientSize.Height / 300;

            float left = form.ClientSize.Width / 2.0f - (inventory.Length + 1.5f) / 2.0f * boxSize + (i + 2) * boxSize + padding;
            float top = (j + 2) * boxSize + padding;
            float size = boxSize - 2 * padding;

            return new RectangleF(left, top, size, size);
        }

        private void updateMouseOverItem(int x, int y)
        {
            Item[][] inventory = state.inventory;
            System.Drawing.Rectangle click = new System.Drawing.Rectangle(x, y, 1, 1);

            for (int i = 0; i < inventory.Length; i++)
                for (int j = 0; j < inventory[i].Length; j++)
                    if (getBoxBounds(i, j).Contains(click))
                    {
                        if (inventory[i][j] != null)
                            mouseOverItem = inventory[i][j];

                        return;
                    }

            if (getBoxBounds(-1, 0.5f).Contains(click))
                mouseOverItem = state.hero.shield;
            else if (getBoxBounds(-2, 0).Contains(click))
                mouseOverItem = state.hero.helmet;
            else if (getBoxBounds(-2, 1).Contains(click))
                mouseOverItem = state.hero.armor;
            else if (getBoxBounds(-2, 2).Contains(click))
                mouseOverItem = state.hero.legs;
            else if (getBoxBounds(-3, 0.5f).Contains(click))
                mouseOverItem = state.hero.weapon;
            else
                mouseOverItem = null;
        }

        public override void keyUp(object sender, KeyEventArgs e) { }

        public override void tick(object sender, EventArgs e) 
        {
            GamePadState current = GamePad.GetState(PlayerIndex.One);

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected)
                updateInput();
        }

        public override void mouseDown(object sender, MouseEventArgs e)
        {
            Item[][] inventory = state.inventory;
            System.Drawing.Rectangle click = new System.Drawing.Rectangle(e.X, e.Y, 1, 1);

            for(int i=0;i<inventory.Length;i++)
                for(int j=0;j<inventory[i].Length;j++)
                    if(getBoxBounds(i, j).Contains(click))
                    {
                        selection = inventory[i][j];
                        inventory[i][j] = null;
                        selectOrigin = new System.Drawing.Point(i, j);
                        dragLoc = new System.Drawing.Point(e.X, e.Y);

                        mouse = e.Location;
                        form.Invalidate();
                        return;
                    }

            if (getBoxBounds(-1, 0.5f).Contains(click))
            {
                if (state.hero.shield != null)
                {
                    selection = state.hero.shield;
                    state.hero.shield = null;
                    selectOrigin = new System.Drawing.Point(-1, 1);
                }
            }
            else if (getBoxBounds(-2, 0).Contains(click))
            {
                if (state.hero.helmet != null)
                {
                    selection = state.hero.helmet;
                    state.hero.helmet = null;
                    selectOrigin = new System.Drawing.Point(-2, 0);
                }
            }
            else if (getBoxBounds(-2, 1).Contains(click))
            {
                if (state.hero.armor != null)
                {
                    selection = state.hero.armor;
                    state.hero.armor = null;
                    selectOrigin = new System.Drawing.Point(-2, 1);
                }
            }
            else if (getBoxBounds(-2, 2).Contains(click))
            {
                if (state.hero.legs != null)
                {
                    selection = state.hero.legs;
                    state.hero.legs = null;
                    selectOrigin = new System.Drawing.Point(-2, 2);
                }
            }
            else if (getBoxBounds(-3, 0.5f).Contains(click))
            {
                if (state.hero.weapon != null)
                {
                    selection = state.hero.weapon;
                    state.hero.weapon = null;
                    selectOrigin = new PointF(-3, 0.5f);
                }
            }

            dragLoc = new System.Drawing.Point(e.X, e.Y);
            mouse = e.Location;
            form.Invalidate();
        }

        public override void mouseUp(object sender, MouseEventArgs e)
        {
            if (selection != null)
            {
                Item[][] inventory = state.inventory;

                if (!dragging)
                {
                    if (selection is Weapon)
                    {
                        if (state.hero.weapon != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.weapon;
                        state.hero.weapon = (Weapon)selection;
                    }
                    else if (selection is Helmet)
                    {
                        if (state.hero.helmet != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.helmet;
                        state.hero.helmet = (Helmet)selection;
                    }
                    else if (selection is Armor)
                    {
                        if (state.hero.armor != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.armor;
                        state.hero.armor = (Armor)selection;
                    }
                    else if (selection is Legs)
                    {
                        if (state.hero.legs != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.legs;
                        state.hero.legs = (Legs)selection;
                    }
                    else if (selection is Shield)
                    {
                        if (state.hero.shield != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.shield;
                        state.hero.shield = (Shield)selection;
                    }
                    else if (selection is Consumable)
                    {
                        Consumable item = (Consumable)selection;
                        item.use();
                    }
                    selection = null;
                }
                else
                {
                    System.Drawing.Rectangle click = new System.Drawing.Rectangle(e.X, e.Y, 1, 1);

                    for (int i = 0; i < inventory.Length; i++)
                        for (int j = 0; j < inventory[i].Length; j++)
                            if (getBoxBounds(i, j).Contains(click))
                            {
                                if (inventory[i][j] != null)
                                {
                                    if (selectOrigin.X >= 0 && selectOrigin.X < inventory.Length && selectOrigin.Y >= 0 && selectOrigin.Y < inventory[i].Length)
                                    {
                                        inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = inventory[i][j];
                                        inventory[i][j] = selection;
                                    }
                                    else
                                    {
                                        state.room.droppedItems.Add(selection, new PointF((float)state.hero.x, (float)state.hero.y));
                                    }
                                }
                                else
                                {
                                    inventory[i][j] = selection;
                                }
                                selection = null;

                                dragging = false;
                                updateMouseOverItem(e.X, e.Y);
                                form.Invalidate();
                                return;
                            }

                    if (selection is Shield && getBoxBounds(-1, 0.5f).Contains(click))
                    {
                        if (state.hero.shield != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.shield;
                        state.hero.shield = (Shield)selection;
                    }
                    else if (selection is Helmet && getBoxBounds(-2, 0).Contains(click))
                    {
                        if (state.hero.helmet != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.helmet;
                        state.hero.helmet = (Helmet)selection;
                    }
                    else if (selection is Armor && getBoxBounds(-2, 1).Contains(click))
                    {
                        if (state.hero.armor != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.armor;
                        state.hero.armor = (Armor)selection;
                    }
                    else if (selection is Legs && getBoxBounds(-2, 2).Contains(click))
                    {
                        if (state.hero.legs != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.legs;
                        state.hero.legs = (Legs)selection;
                    }
                    else if (selection is Weapon && getBoxBounds(-3, 0.5f).Contains(click))
                    {
                        if (state.hero.weapon != null)
                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.weapon;
                        state.hero.weapon = (Weapon)selection;
                    }
                    else
                    {
                        state.room.droppedItems.Add(selection, new PointF((float)state.hero.x, (float)state.hero.y));
                    }
                    selection = null;
                }

                dragging = false;
                updateMouseOverItem(e.X, e.Y);
                form.Invalidate();
            }
        }

        public override void mouseMove(object sender, MouseEventArgs e)
        {
            if (selection != null)
            {
                mouse = e.Location;
                if (!dragging && Math.Sqrt(Math.Pow(e.X - dragLoc.X, 2) + Math.Pow(e.Y - dragLoc.Y, 2)) > 20)
                    dragging = true;
            }
            else
            {
                updateMouseOverItem(e.X, e.Y);
            }

            form.Invalidate();
        }

        public override void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Properties.Settings.Default.CloseKey || e.KeyCode == Properties.Settings.Default.InventoryKey)
                this.close();
        }

        public override void updateInput()
        {
            GamePadState current = GamePad.GetState(PlayerIndex.One);
            Item[][] inventory = state.inventory;

            if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.close();
                System.Threading.Thread.Sleep(200);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                highlight.X = (highlight.X + 1) % inventory.Length;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (--highlight.X < 0)
                    highlight.X = inventory.Length - 1;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                highlight.Y = (highlight.Y + 1) % inventory[0].Length;
                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (--highlight.Y < 0)
                    highlight.Y = inventory[0].Length - 1;

                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                //TO-DO
                //System.Drawing.Rectangle click = new System.Drawing.Rectangle(e.X, e.Y, 1, 1);

                for (int i = 0; i < inventory.Length; i++)
                    for (int j = 0; j < inventory[i].Length; j++)
                        if (i == highlight.X && j == highlight.Y)
                        {
                            selection = inventory[i][j];
                            inventory[i][j] = null;
                            selectOrigin = new System.Drawing.Point(highlight.X, highlight.Y);
                            dragLoc = new System.Drawing.Point(highlight.X, highlight.Y);

                            form.Invalidate();
                            //return;
                        }
                if (highlight.X == -1 && highlight.Y == 0.5f)
                {
                    if (state.hero.shield != null)
                    {
                        selection = state.hero.shield;
                        state.hero.shield = null;
                        selectOrigin = new System.Drawing.Point(-1, 1);
                    }
                }
                else if (highlight.X == -2 && highlight.Y == 0)
                {
                    if (state.hero.helmet != null)
                    {
                        selection = state.hero.helmet;
                        state.hero.helmet = null;
                        selectOrigin = new System.Drawing.Point(-2, 0);
                    }
                }
                else if (highlight.X == -2 && highlight.Y == -1)
                {
                    if (state.hero.armor != null)
                    {
                        selection = state.hero.armor;
                        state.hero.armor = null;
                        selectOrigin = new System.Drawing.Point(-2, 1);
                    }
                }
                else if (highlight.X == -2 && highlight.Y == -2)
                {
                    if (state.hero.legs != null)
                    {
                        selection = state.hero.legs;
                        state.hero.legs = null;
                        selectOrigin = new System.Drawing.Point(-2, 2);
                    }
                }
                else if (highlight.X == -3 && highlight.Y == 0.5f)
                {
                    if (state.hero.weapon != null)
                    {
                        selection = state.hero.weapon;
                        state.hero.weapon = null;
                        selectOrigin = new PointF(-3, 0.5f);
                    }
                }

                dragLoc = new System.Drawing.Point(highlight.X, highlight.Y);
                //mouse = e.Location;
                form.Invalidate();

                if (selection != null)
                {
                    if (!dragging)
                    {
                        if (selection is Weapon)
                        {
                            if (state.hero.weapon != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.weapon;
                            state.hero.weapon = (Weapon)selection;
                        }
                        else if (selection is Helmet)
                        {
                            if (state.hero.helmet != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.helmet;
                            state.hero.helmet = (Helmet)selection;
                        }
                        else if (selection is Armor)
                        {
                            if (state.hero.armor != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.armor;
                            state.hero.armor = (Armor)selection;
                        }
                        else if (selection is Legs)
                        {
                            if (state.hero.legs != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.legs;
                            state.hero.legs = (Legs)selection;
                        }
                        else if (selection is Shield)
                        {
                            if (state.hero.shield != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.shield;
                            state.hero.shield = (Shield)selection;
                        }
                        else if (selection is Consumable)
                        {
                            Consumable item = (Consumable)selection;
                            item.use();
                        }
                        selection = null;
                    }
                    else
                    {
                        System.Drawing.Rectangle click = new System.Drawing.Rectangle(highlight.X, highlight.Y, 1, 1);

                        for (int i = 0; i < inventory.Length; i++)
                            for (int j = 0; j < inventory[i].Length; j++)
                                if (i == highlight.X && j == highlight.Y)
                                {
                                    if (inventory[i][j] != null)
                                    {
                                        if (selectOrigin.X >= 0 && selectOrigin.X < inventory.Length && selectOrigin.Y >= 0 && selectOrigin.Y < inventory[i].Length)
                                        {
                                            inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = inventory[i][j];
                                            inventory[i][j] = selection;
                                        }
                                        else
                                        {
                                            state.room.droppedItems.Add(selection, new PointF((float)state.hero.x, (float)state.hero.y));
                                        }
                                    }
                                    else
                                    {
                                        inventory[i][j] = selection;
                                    }
                                    selection = null;

                                    dragging = false;
                                    updateMouseOverItem(highlight.X, highlight.Y);
                                    form.Invalidate();
                                    //return;
                                }

                        if (selection is Shield && highlight.X == -1 && highlight.Y == 0.5f)
                        {
                            if (state.hero.shield != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.shield;
                            state.hero.shield = (Shield)selection;
                        }
                        else if (selection is Helmet && highlight.X == -2 && highlight.Y == 0)
                        {
                            if (state.hero.helmet != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.helmet;
                            state.hero.helmet = (Helmet)selection;
                        }
                        else if (selection is Armor && highlight.X == -2 && highlight.Y == 1)
                        {
                            if (state.hero.armor != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.armor;
                            state.hero.armor = (Armor)selection;
                        }
                        else if (selection is Legs && highlight.X == -2 && highlight.Y == -2)
                        {
                            if (state.hero.legs != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.legs;
                            state.hero.legs = (Legs)selection;
                        }
                        else if (selection is Weapon && highlight.X == -3 && highlight.Y == 0.5f)
                        {
                            if (state.hero.weapon != null)
                                inventory[(int)selectOrigin.X][(int)selectOrigin.Y] = state.hero.weapon;
                            state.hero.weapon = (Weapon)selection;
                        }
                        else
                        {
                            state.room.droppedItems.Add(selection, new PointF((float)state.hero.x, (float)state.hero.y));
                        }
                        selection = null;
                    }

                    dragging = false;
                    updateMouseOverItem(highlight.X, highlight.Y);
                    form.Invalidate();
                }

                System.Threading.Thread.Sleep(100);
            }
            else if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && current.Buttons.X == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (inventory[i][j] == null)
                        {
                            if (state.hero.shield != null)
                            {
                                inventory[i][j] = state.hero.shield;
                                state.hero.shield = null;
                            }
                            else if (state.hero.helmet != null)
                            {
                                inventory[i][j] = state.hero.helmet;
                                state.hero.helmet = null;
                            }
                            else if (state.hero.armor != null)
                            {
                                inventory[i][j] = state.hero.armor;
                                state.hero.armor = null;
                            }
                            else if (state.hero.legs != null)
                            {
                                inventory[i][j] = state.hero.legs;
                                state.hero.legs = null;
                            }
                            else if (state.hero.weapon != null)
                            {
                                inventory[i][j] = state.hero.weapon;
                                state.hero.weapon = null;
                            }
                        }
                    }
                }
            }
        }

        public override void paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            GamePadState current = GamePad.GetState(PlayerIndex.One);

            Item[][] inventory = state.inventory;
            int boxSize = form.ClientSize.Height / (inventory.Length + 4);

            for (int i = 0; i < inventory.Length; i++)
                for (int j = 0; j < inventory[i].Length; j++)
                {
                    g.DrawImage(boxImg, getBoxBounds(i, j));

                    if (Properties.Settings.Default.ControllerEnabled && current.IsConnected && highlight.X == i && highlight.Y == j)
                        g.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Yellow)), getBoxBounds(i, j));

                    if(inventory[i][j] != null)
                        g.DrawImage(inventory[i][j].img, getBoxBounds(i, j));
                }

            g.DrawImage(shieldImg, getBoxBounds(-1, 0.5f));
            if(state.hero.shield != null)
                g.DrawImage(state.hero.shield.img, getBoxBounds(-1, 0.5f));
            g.DrawImage(armorImg, getBoxBounds(-2, 1));
            if (state.hero.armor != null)
                g.DrawImage(state.hero.armor.img, getBoxBounds(-2, 1));
            g.DrawImage(helmetImg, getBoxBounds(-2, 0));
            if (state.hero.helmet != null)
                g.DrawImage(state.hero.helmet.img, getBoxBounds(-2, 0));
            g.DrawImage(legsImg, getBoxBounds(-2, 2));
            if (state.hero.legs != null)
                g.DrawImage(state.hero.legs.img, getBoxBounds(-2, 2));
            g.DrawImage(weaponImg, getBoxBounds(-3, 0.5f));
            if (state.hero.weapon != null)
                g.DrawImage(state.hero.weapon.img, getBoxBounds(-3, 0.5f));

            RectangleF infoBox = getBoxBounds(-3, inventory.Length - 2);
            infoBox.Width *= 3;
            infoBox.Height *= 2;
            g.DrawImage(infoImg, infoBox);

            String infoString = "";
            if (mouseOverItem != null)
                infoString = mouseOverItem.description;
            if (selection != null)
                infoString = selection.description;
            if(infoString == "")
            {
                String spellName = "None";
                if (SkillStreeState.spellSelected != null)
                    spellName = SkillStreeState.spellSelected.spellName;

                infoString = "Character Stats"
                + "\nLVL: " + state.hero.level
                + "\t\tHP: " + Math.Round(state.hero.hp, 2) + "/" + Math.Round(state.hero.full_hp, 2)
                + "\nHP REG: " + Math.Round(state.hero.hp_reg, 2)
                + "\tATK: " + ((state.hero.weapon != null && state.hero.weapon.ranged) ? Math.Round(state.hero.weapon.damage + state.hero.atk_dmg, 2) : Math.Round(state.hero.atk_dmg, 2))
                + "\nATK SPD: " + ((state.hero.weapon != null && state.hero.weapon.ranged) ? Math.Round(state.hero.weapon.atk_speed, 2) : Math.Round(state.hero.atk_speed, 2))
                + "\tMV SPD: " + Math.Round(state.hero.speed, 2)
                + "\nATK RNG: " + ((state.hero.weapon != null && state.hero.weapon.ranged) ? state.hero.weapon.proj_range : 1)
                + "\tEXP: " + Math.Round(state.hero.exp, 2) + "/" + Math.Round(state.hero.expcap, 2)
                + "\nSkill Points: " + SkillStreeState.availablePoints
                + "\nSPELL: " + spellName
                + "\nATK STY: " + (state.hero.weapon != null ? state.hero.weapon.style : Item.AtkStyle.Basic)
                + "\nSTATUS: " + state.hero.status;
            }

            g.DrawString(infoString, font, Brushes.White, new PointF(infoBox.X + infoBox.Width / 20, infoBox.Y + infoBox.Height / 20));

            if (selection != null)
            {
                if (dragging)
                    g.DrawImage(selection.img, mouse.X - boxSize / 2, mouse.Y - boxSize / 2, boxSize, boxSize);
                else
                    g.DrawImage(selection.img, getBoxBounds((int)selectOrigin.X, (int)selectOrigin.Y));
            }
        }
    }
}
