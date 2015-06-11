using System;
using System.Drawing;
using System.Threading;

namespace DungeonDrive
{
    public abstract class Unit
    {
        protected GameState state;
        public double x;
        public double y;
        public double origin_x;
        public double origin_y;
        public double center_x;
        public double center_y;
        public double base_speed = 0.01;
        public double speed = 0.01;
        public double radius = 0.5;
        public double full_hp = 1;
        public double base_full_hp = 1;
        public double hp = 1;
        public double hp_reg = 0;
        public double atk_dmg = 1;
        public double base_atk_dmg = 1;
        public double atk_speed = 1;
        public double base_atk_speed = 1;
        public bool stuned = false;
        public bool teleport = false;
        public bool moving = false;
        public bool lunge = false;
        public bool split = false;
        public int roomNum = -1;
        public double exp;
        public double expcap;
        public int level;
        public String filename;
        public String status;
        public bool displayname = false;
        public double dropWpnFac = 0;
        public double animFrame = 0;
        public bool phase = false;
        public int totalMoves = 4;
        public int moves;
        public int bind_remove = 0;
        public bool inCombat = false;
        public int combatCd = 0;
        public int frame = 0;

        public bool knockback = false;
        public double x_dist = 0;
        public double y_dist = 0;
        public double x_final = 0;
        public double y_final = 0;
        public double sleep_sec = 0;        // amount of time that unit can't move
        public double poison_sec = 0;       // amount of time that unit is poisoned
        public double curse_sec = 0;        // amount of time that unit is cursed
        public double bind_sec = 0;         // amount of time that unit is disabled
        public double burning_sec = 0;      // amount of time that unit is burning
        public double paralyze_sec = 0;     // amount of time that unit is paralyzed
        public double confuse_sec = 0;      // amount of time that unit is confused
        public double burning_amount = 0;

        public bool[] atk_cd = new bool[10];      // flags for different skill's availability

        public int sight = 7;

        public int DrawX { get { return (int)(x * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size - state.size * radius); } }
        public int DrawY { get { return (int)(y * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size - state.size * radius); } }

        public float dir = 0;
        public Unit(GameState state, double x, double y)
        {
            this.state = state;
            this.x = x;
            this.y = y;
            for (int i = 0; i < atk_cd.Length; i++) atk_cd[i] = true;
            
        }

        public abstract void act();
        public abstract void draw(Graphics g);

        private void atkSleep(double sec, int i)
        {
            atk_cd[i] = false;
            Thread.Sleep((int)(sec * 1000));
            atk_cd[i] = true;
            frame = 0;
        }

        public virtual void setDir() {
            if (this is Hero)
                return;
            this.dir = (float)Math.Atan2(state.hero.y - this.y, state.hero.x - this.x);
        }
        private void slowSleep(double sec, double fac)
        {
            this.speed *= fac;
            Thread.Sleep((int)(sec * 1000));
            this.speed /= fac;
        }

        public void cd(double sec, int i)
        {
            // disable the attack boolean at index i for certain given seconds
            new Thread(() => atkSleep(sec, i)).Start();
        }

        public void slow(double sec, double fac)
        {
            // slows down for sec second and factor of fac
            new Thread(() => slowSleep(sec, fac)).Start();
        }

        public void burn(double sec, double fac)
        {
            // burn the unit for sec second and factor of fac
            this.burning_sec = sec * 17;
            this.burning_amount = fac / this.burning_sec;
        }
        public void cast(Spell spell) {
            if (spell == null) { return; }
            spell.cast(this.state, this);
        }
        public void burning()
        {   
            this.hp -= this.burning_amount;
            if (this.hp <= 0)
                state.hero.deletingList.Add(this);
        }

        public void drawHpBar(Graphics g)
        {   g.FillRectangle(this.hp <= 0.4 * this.full_hp ? Brushes.Red : Brushes.DarkGreen, DrawX, DrawY - 5, (int)(radius * 2 * state.size * this.hp / this.full_hp), 2); }

        public void drawFileName(Graphics g)
        {   g.DrawString(filename, state.font, Brushes.White, new PointF(DrawX, DrawY - state.size / 2));   }

        public void addName(String filename)
        {   this.filename = filename;   }

        public void knockBack(Unit unit, double x_dist, double y_dist, double sleep_sec)
        {
            unit.x_dist = x_dist;
            unit.y_dist = y_dist;
            unit.x_final = unit.x + unit.x_dist;
            unit.y_final = unit.y + unit.y_dist;
            unit.sleep_sec = sleep_sec * 17;
            unit.moves = unit.totalMoves;
            unit.knockback = true;
        }

        public void knockBacked()
        {
            if (moves-- <= 0)
                knockback = false;
            
            if (tryMove(x + x_dist / moves, y + y_dist / moves, this) && (Math.Abs(x_final - x) <= Math.Abs(x_dist) || Math.Abs(y_final - y) <= Math.Abs(y_dist)))
            {
                x += x_dist / totalMoves;
                y += y_dist / totalMoves;
            }
        }

        public void attackHero()
        {
            if (this is Hero)
                return;

            if (this.atk_cd[0])
            {
                if (this is Boss)
                    this.frame = 5;
                int dirX = Math.Sign(state.hero.x - this.x);
                int dirY = Math.Sign(state.hero.y - this.y);
                this.knockBack(state.hero, dirX * 0.5, dirY * 0.5, 0);
                state.hero.inCombat = true;
                state.hero.combatCd = 3 * 17;
                this.inCombat = true;
                this.combatCd = 3 * 17;
                this.sleep_sec = 1 * 17;
                if (state.hero.shield != null && state.hero.shield.blockChan > state.hero.shield.rdnDouble(0.0, 1.0))
                    state.hero.hp -= (this.atk_dmg - state.hero.shield.blockDmg);
                else
                    state.hero.hp -= this.atk_dmg;
                this.cd(1, 0);
            }

        }

        public bool tryMove(double xNext, double yNext, Unit e)
        {
            
            int left = (int)(xNext - radius);
            int top = (int)(yNext - radius);
            int width = (int)(radius * 2 + (xNext - (int)xNext < radius || 1 - (xNext - (int)xNext) < radius ? 2 : 1));
            int height = (int)(radius * 2 + (yNext - (int)yNext < radius || 1 - (yNext - (int)yNext) < radius ? 2 : 1));

            bool canMove = left >= 0 && left + width < state.room.width && top >= 0 && top + height < state.room.height;

            for (int i = left; canMove && i < left + width; i++)
                for (int j = top; canMove && j < top + height; j++)
                    canMove = state.room.walkingSpace[i, j];

            if (this != state.hero)
            {
                foreach (Unit unit in state.room.enemies)
                    if (this != unit && Math.Sqrt(Math.Pow(xNext - unit.x, 2) + Math.Pow(yNext - unit.y, 2)) < radius + unit.radius)
                        return false;
            }
            else
            {
                if (state.room.stairSpace[(int)x, (int)y])
                {
                    foreach (Stairs stairs in state.room.stairs)
                        if (Math.Abs(stairs.x + 0.5 - x) < radius && Math.Abs(stairs.y + 0.5 - y) < radius * 2)
                        {
                            state.room.saveState();
                            state.room = new Room(state,stairs.path);
                            if(state.allLevelInfo.levelAlreadyExists(stairs.path)){
                                state.allLevelInfo.loadLevel(stairs.path);
                            }
                            else
                            {
                                state.allLevelInfo.addLevel(new LevelInfo(state,stairs.path,false));
                            }
                            return false;
                        }
                }
            }

            if (canMove || e.phase)
            {
                x = xNext;
                y = yNext;
            }
            else
            {
                if ((int)(x - radius) > (int)(xNext - radius))
                {
                    x = (int)x + radius + 0.001;
                    tryMove(x, yNext, e);
                }
                else if ((int)(x + radius) < (int)(xNext + radius))
                {
                    x = (int)x + 1 - radius - 0.001;
                    tryMove(x, yNext, e);
                }

                if ((int)(y - radius) > (int)(yNext - radius))
                {
                    y = (int)y + radius + 0.001;
                    tryMove(xNext, y, e);
                }
                else if ((int)(y + radius) < (int)(yNext + radius))
                {
                    y = (int)y + 1 - radius - 0.001;
                    tryMove(xNext, y, e);
                }
            }

            return canMove;
        }

        public void statusChanged(Unit unit, String ailment)
        {
            switch (ailment)
            {
                case "poison":
                    unit.status = "Poisoned";
                    break;

                case "paralyze":
                    unit.status = "Paralyzed";
                    break;

                case "curse":
                    unit.status = "Cursed";
                    break;

                case "arm_bind":
                    unit.status = "Binded Arm";
                    break;

                case "head_bind":
                    unit.status = "Binded Head";
                    break;

                case "sleep":
                    unit.status = "Sleep";
                    break;
            }
        }
    }
}
