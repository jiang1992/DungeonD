using System;
using System.Drawing;

namespace DungeonDrive
{
    public class Bat : Unit
    {
        private Bitmap[] imgs = new Bitmap[3];
        private Random rand;

        public Bat(GameState state, double x, double y)
            : base(state, x, y)
        {
            this.full_hp = 30 * Math.Pow(1.09, state.hero.level) - 20;
            this.hp = full_hp;
            this.hp_reg = this.full_hp * 0.001;
            this.atk_dmg = 1 * Math.Pow(1.1, state.hero.level);
            this.base_atk_dmg = this.atk_dmg;
            this.speed = 0.1 * Math.Pow(1.01, state.hero.level);
            this.radius = 0.45;
            this.origin_x = x;
            this.origin_y = y;
            this.center_x = x + radius;
            this.center_y = y + radius;
            this.exp = 2 * Math.Pow(1.09, state.hero.level);
            this.status = "Normal";
            this.inCombat = false;
            imgs[0] = new Bitmap(Properties.Resources.bat0);
            imgs[1] = new Bitmap(Properties.Resources.bat1);
            imgs[2] = new Bitmap(Properties.Resources.bat2);

            rand = new Random();
        }

        public void move()
        {
            double xNext;
            double yNext;

            //If bat units are below a certain HP threshold, they will start running from the player
            //Only a basic placeholder for future additions. Eventually, I will add more dynamic behaviors on top of this (ex. bats' escape route will prioritize nearby mobs and then turn on the player
            if (this.hp < this.full_hp * 0.4)
            {
                this.hp += this.hp_reg;
                xNext = x - Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                yNext = y - Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;

                return;
            }

            if (Math.Abs(state.hero.x - x) < 7 && Math.Abs(state.hero.y - y) < 7)
            {
                //Player draws aggro from bats if he is close enough
                this.moving = true;
                xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
            }
            else if (this.moving)
            {
                //Move towards original position
                xNext = x + Math.Cos(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
                if ((Math.Round(this.x, 1) == this.origin_x || Math.Round(this.y, 1) == this.origin_y))
                {
                    //Original position has been reached
                    this.moving = false;
                    this.x = origin_x;
                    this.y = origin_y;
                    return;
                }
            }
            else
            {
                if (this.hp + this.hp_reg * 5 < this.full_hp)
                    this.hp += this.hp_reg * 5;
                else
                    this.hp = this.full_hp;
            }
        }

        
        public override void act()
        {
            if (inCombat)
            {
                if (combatCd-- > 0);
                else
                    inCombat = false;
            }
            
            if (knockback)
                knockBacked();

            if (burning_sec-- >= 0)
                burning();

            if (sleep_sec > 0)
            {
                sleep_sec--;
                return;
            }

            if (state.hero.status.Equals("Paralyzed"))
                this.atk_dmg = this.base_atk_dmg * 1.3;
            else
                this.atk_dmg = this.base_atk_dmg;

             move();

            if (Math.Sqrt(Math.Pow(state.hero.x - x, 2) + Math.Pow(state.hero.y - y, 2)) < state.hero.radius + radius)
            {
                if (atk_cd[0])
                {
                    int random = rand.Next(0, 100);
                    if (random <= 0)
                        statusChanged(state.hero, "paralyze");
                }
            }

            if (state.room.currentRoom.Equals(state.graveyard))
            {
                this.full_hp = 15 * Math.Pow(1.09, state.hero.level);
                this.hp = full_hp;
                this.atk_dmg = atk_dmg * 3 * Math.Pow(1.09, state.hero.level);
                this.speed += (speed * 0.4) * Math.Pow(1.01, state.hero.level);
            }
            //tryMove(xNext, yNext);
        }

        public override void draw(Graphics g)
        {
            g.DrawImage(imgs[(int)animFrame], DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            animFrame = (animFrame + 0.1) % imgs.Length;
            //g.FillEllipse(Brushes.Red, DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            drawHpBar(g);
            if (this.displayname)
                drawFileName(g);
        }
    }

    public class Skeleton : Unit
    {
        private Bitmap[] imgs = new Bitmap[3];
        private Random rand;

        public Skeleton(GameState state, double x, double y, bool half)
            : base(state, x, y)
        {
            this.full_hp = 25 * Math.Pow(1.09, state.hero.level) - 20;
            this.hp = full_hp;
            this.hp_reg = this.full_hp * 0.005;
            this.atk_dmg = 2 * Math.Pow(1.1, state.hero.level);
            this.base_atk_dmg = this.atk_dmg;
            this.speed = 0.03 * Math.Pow(1.01, state.hero.level);
            this.radius = 0.4;
            this.origin_x = x;
            this.origin_y = y;
            this.center_x = x + radius;
            this.center_y = y + radius;
            this.exp = 2 * Math.Pow(1.09, state.hero.level);
            this.status = "Normal";
            this.inCombat = false;
            this.split = true;

            imgs[0] = new Bitmap(Properties.Resources.skeleton0);
            imgs[1] = new Bitmap(Properties.Resources.skeleton1);
            imgs[2] = new Bitmap(Properties.Resources.skeleton2);

            rand = new Random();
        }

        public void move()
        {
            double xNext;
            double yNext;

            if (this.hp < this.full_hp * 0.4)
            {
                this.hp += this.hp_reg;
                xNext = x - Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                yNext = y - Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;

                return;
            }

            if (Math.Abs(state.hero.x - x) < 7 && Math.Abs(state.hero.y - y) < 7)
            {
                //Player draws aggro from bats if he is close enough
                this.moving = true;
                xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
            }
            else if (this.moving)
            {
                //Move towards original position
                xNext = x + Math.Cos(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
                if ((Math.Round(this.x, 1) == this.origin_x || Math.Round(this.y, 1) == this.origin_y))
                {
                    //Original position has been reached
                    
                    this.moving = false;
                    this.x = origin_x;
                    this.y = origin_y;
                    return;
                }
            }
            else
            {
                if (this.hp + this.hp_reg * 5 < this.full_hp)
                    this.hp += this.hp_reg * 5;
                else
                    this.hp = this.full_hp;
            }
        }

        public override void act()
        {
            if (inCombat)
            {
                if (combatCd-- > 0);
                else
                    inCombat = false;
            }

            if (knockback)
                knockBacked();

            if (burning_sec-- >= 0)
                burning();

            if (sleep_sec > 0)
            {
                sleep_sec--;
                return;
            }

            move();

            if (state.hero.status.Equals("Paralyzed"))
                this.atk_dmg = this.base_atk_dmg * 1.3;
            else
                this.atk_dmg = this.base_atk_dmg;

            /*if (this.split)
            {
                if (this.hp <= this.full_hp / 2)
                {
                    state.room.addEnemy(new Skeleton(state, rand.Next(0, state.room.width - 1) + 0.5, rand.Next(0, state.room.height - 1) + 0.5, false), "temp.surprise_mothafucka");
                    state.room.numSkeletons++;
                    this.split = false;
                }
            }*/

            //double xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
            //double yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;

            if (Math.Sqrt(Math.Pow(state.hero.x - x, 2) + Math.Pow(state.hero.y - y, 2)) < state.hero.radius + radius)
            {
                if (atk_cd[0])
                {
                    int random = rand.Next(0, 100);
                    if (random <= 35)
                        statusChanged(state.hero, "arm_bind");
                }
            }

            if (state.room.currentRoom.Equals(state.graveyard))
            {
                this.full_hp = 10 * Math.Pow(1.09, state.hero.level);
                this.hp = full_hp;
                this.atk_dmg = atk_dmg * 3 * Math.Pow(1.09, state.hero.level);
                this.speed += (speed * 0.4) * Math.Pow(1.01, state.hero.level);
            }

            //tryMove(xNext, yNext);
        }

        public override void draw(Graphics g)
        {
            g.DrawImage(imgs[(int)animFrame], DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            animFrame = (animFrame + 0.1) % imgs.Length;
            //g.FillEllipse(Brushes.SaddleBrown, DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            drawHpBar(g);
            if (this.displayname)
                drawFileName(g);
        }
    }

    public class SkeletalMage : Unit
    {
        private Bitmap[] imgs = new Bitmap[3];
        private Random rand;

        public SkeletalMage(GameState state, double x, double y, bool half)
            : base(state, x, y)
        {
            this.full_hp = 25 * Math.Pow(1.09, state.hero.level) - 20;
            this.hp = full_hp;
            this.hp_reg = this.full_hp * 0.005;
            this.atk_dmg = 2 * Math.Pow(1.1, state.hero.level);
            this.base_atk_dmg = this.atk_dmg;
            this.speed = 0.03 * Math.Pow(1.01, state.hero.level);
            this.radius = 0.4;
            this.origin_x = x;
            this.origin_y = y;
            this.center_x = x + radius;
            this.center_y = y + radius;
            this.exp = 2 * Math.Pow(1.09, state.hero.level);
            this.status = "Normal";
            this.inCombat = false;
            this.split = true;

            imgs[0] = new Bitmap(Properties.Resources.skeletalMage0);
            imgs[1] = new Bitmap(Properties.Resources.skeletalMage1);
            imgs[2] = new Bitmap(Properties.Resources.skeletalMage2);

            rand = new Random();
        }

        public void move()
        {
            double xNext;
            double yNext;

            if (this.hp < this.full_hp * 0.4)
            {
                this.hp += this.hp_reg;
                xNext = x - Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                yNext = y - Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;

                return;
            }

            if (Math.Abs(state.hero.x - x) < 7 && Math.Abs(state.hero.y - y) < 7)
            {
                //Player draws aggro from bats if he is close enough
                this.moving = true;
                xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;

                if (this.atk_cd[1])
                {
                    GravityForceField gf = new GravityForceField();
                    this.cast(gf);
                    this.cd(10, 1);
                }
                else if (this.atk_cd[2])
                {
                    LighteningBall lb = new LighteningBall();
                    this.cast(lb);
                    this.cd(3, 2);
                }
            }
            else if (this.moving)
            {
                //Move towards original position
                xNext = x + Math.Cos(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
                if ((Math.Round(this.x, 1) == this.origin_x || Math.Round(this.y, 1) == this.origin_y))
                {
                    //Original position has been reached

                    this.moving = false;
                    this.x = origin_x;
                    this.y = origin_y;
                    return;
                }
            }
            else
            {
                if (this.hp + this.hp_reg * 5 < this.full_hp)
                    this.hp += this.hp_reg * 5;
                else
                    this.hp = this.full_hp;
            }
        }

        public override void act()
        {
            if (inCombat)
            {
                if (combatCd-- > 0) ;
                else
                    inCombat = false;
            }

            if (knockback)
                knockBacked();

            if (burning_sec-- >= 0)
                burning();

            if (sleep_sec > 0)
            {
                sleep_sec--;
                return;
            }

            move();

            if (state.hero.status.Equals("Paralyzed"))
                this.atk_dmg = this.base_atk_dmg * 1.3;
            else
                this.atk_dmg = this.base_atk_dmg;

            /*if (this.split)
            {
                if (this.hp <= this.full_hp / 2)
                {
                    state.room.addEnemy(new Skeleton(state, rand.Next(0, state.room.width - 1) + 0.5, rand.Next(0, state.room.height - 1) + 0.5, false), "temp.surprise_mothafucka");
                    state.room.numSkeletons++;
                    this.split = false;
                }
            }*/

            //double xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
            //double yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;

            if (Math.Sqrt(Math.Pow(state.hero.x - x, 2) + Math.Pow(state.hero.y - y, 2)) < state.hero.radius + radius)
            {
                if (atk_cd[0])
                {
                    int random = rand.Next(0, 100);
                    if (random <= 35)
                        statusChanged(state.hero, "arm_bind");
                }
            }

            if (state.room.currentRoom.Equals(state.graveyard))
            {
                this.full_hp = 10 * Math.Pow(1.09, state.hero.level);
                this.hp = full_hp;
                this.atk_dmg = atk_dmg * 3 * Math.Pow(1.09, state.hero.level);
                this.speed += (speed * 0.4) * Math.Pow(1.01, state.hero.level);
            }

            /*if (this.atk_cd[2])
            {
                
            }*/

            //tryMove(xNext, yNext);
        }

        public override void draw(Graphics g)
        {
            g.DrawImage(imgs[(int)animFrame], DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            animFrame = (animFrame + 0.1) % imgs.Length;
            //g.FillEllipse(Brushes.SaddleBrown, DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            drawHpBar(g);
            if (this.displayname)
                drawFileName(g);
        }
    }

    public class Snake : Unit
    {
        private Bitmap[] imgs = new Bitmap[3];
        private Random rand;

        public Snake(GameState state, double x, double y)
            : base(state, x, y)
        {
            this.full_hp = 25 * Math.Pow(1.09, state.hero.level) - 20;
            this.hp = full_hp;
            this.hp_reg = this.full_hp * 0.001;
            this.atk_dmg = 2 * Math.Pow(1.1, state.hero.level);
            this.base_atk_dmg = this.atk_dmg;
            this.speed = 0.1 * Math.Pow(1.01, state.hero.level);
            this.radius = 0.35;
            this.origin_x = x;
            this.origin_y = y;
            this.center_x = x + radius;
            this.center_y = y + radius;
            this.exp = 3 * Math.Pow(1.09, state.hero.level);
            this.status = "Normal";
            this.inCombat = false;
            this.lunge = true;

            imgs[0] = new Bitmap(Properties.Resources.snake0);
            imgs[1] = new Bitmap(Properties.Resources.snake1);
            imgs[2] = new Bitmap(Properties.Resources.snake2);

            rand = new Random();
        }

        public void move()
        {
            double xNext;
            double yNext;

            if (this.hp < this.full_hp * 0.4)
            {
                this.hp += this.hp_reg;
                xNext = x - Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                yNext = y - Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;

                return;
            }

            if (Math.Abs(state.hero.x - x) < 7 && Math.Abs(state.hero.y - y) < 7)
            {
                //Player draws aggro from bats if he is close enough
                this.moving = true;
                xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
            }
            else if (this.moving)
            {
                //Move towards original position
                xNext = x + Math.Cos(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
                if ((Math.Round(this.x, 1) == this.origin_x || Math.Round(this.y, 1) == this.origin_y))
                {
                    //Original position has been reached
                    this.moving = false;
                    this.x = origin_x;
                    this.y = origin_y;
                    return;
                }
            }
            else
            {
                if (this.hp + this.hp_reg * 5 < this.full_hp)
                    this.hp += this.hp_reg * 5;
                else
                    this.hp = this.full_hp;
            }
        }

        public override void act()
        {
            if (inCombat)
            {
                if (combatCd-- > 0);
                else
                    inCombat = false;
            }

            if (knockback)
                knockBacked();

            if (burning_sec-- >= 0)
                burning();

            if (sleep_sec > 0)
            {
                sleep_sec--;
                return;
            }

            move();

            if (state.hero.status.Equals("Paralyzed"))
                this.atk_dmg = this.base_atk_dmg * 1.3;
            else
                this.atk_dmg = this.base_atk_dmg;

            //double xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
            //double yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;

            /*if ((state.hero.x - x) < 3 && (state.hero.x - x) > 0.2 && (state.hero.y - y) < 3 && (state.hero.y - y) > 0.2)
                this.speed = 0.15;
            else
                this.speed = 0.03;*/

            if (state.room.currentRoom.Equals(state.graveyard))
            {
                this.full_hp = 5 * Math.Pow(1.09, state.hero.level);
                this.hp = full_hp;
                this.atk_dmg += (atk_dmg * 0.30) * Math.Pow(1.09, state.hero.level);
                this.speed += (speed * 0.4) * Math.Pow(1.01, state.hero.level);
            }

            if (Math.Sqrt(Math.Pow(state.hero.x - x, 2) + Math.Pow(state.hero.y - y, 2)) < state.hero.radius + radius)
            {
                if (atk_cd[0])
                {
                    int random = rand.Next(0, 100);
                    if (random <= 30)
                        statusChanged(state.hero, "poison");
                }
            }


            if (this.atk_cd[1] )
            {
                //LighteningBall LB = new LighteningBall();
                //LB.setLighteningBall(state, this);
                //LB.cast();
                this.cd(5, 1);
            }

            /*if ((state.hero.x - x) < 3 && (state.hero.y - y) < 3)
            {
                if (this.lunge)
                {
                    this.speed = 0.4;
                    this.lunge = false;
                }

                if (Math.Sqrt(Math.Pow(state.hero.x - x, 2) + Math.Pow(state.hero.y - y, 2)) < state.hero.radius + radius)
                {
                    this.speed = 0.1;
                }
            }*/

            //tryMove(xNext, yNext);
        }

        public override void draw(Graphics g)
        {
            g.DrawImage(imgs[(int)animFrame], DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            animFrame = (animFrame + 0.1) % imgs.Length;
            //g.FillEllipse(Brushes.SaddleBrown, DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            drawHpBar(g);
            if (this.displayname)
                drawFileName(g);
        }
    }

    public class Ghost : Unit
    {
        private Bitmap[] imgs = new Bitmap[3];
        private Random rand;
        private int sleeparoni;

        public Ghost(GameState state, double x, double y)
            : base(state, x, y)
        {
            this.full_hp = 20 * Math.Pow(1.09, state.hero.level) - 15;
            this.hp = full_hp;
            this.hp_reg = this.full_hp * 0.0005;
            this.atk_dmg = 3 * Math.Pow(1.1, state.hero.level);
            this.base_atk_dmg = this.atk_dmg;
            this.speed = 0.05 * Math.Pow(1.01, state.hero.level);
            this.radius = 0.45;
            this.origin_x = x;
            this.origin_y = y;
            this.center_x = x + radius;
            this.center_y = y + radius;
            this.exp = 4 * Math.Pow(1.09, state.hero.level);
            this.status = "Normal";
            this.teleport = false;
            this.inCombat = false;
            this.phase = true;

            imgs[0] = new Bitmap(Properties.Resources.ghost0);
            imgs[1] = new Bitmap(Properties.Resources.ghost1);
            imgs[2] = new Bitmap(Properties.Resources.ghost2);

            sleeparoni = 100;
            rand = new Random();
        }

        public void move()
        {
            double xNext;
            double yNext;

            /*if (this.hp < this.full_hp * 0.4)
            {
                this.hp += this.hp_reg;
                xNext = x - Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                yNext = y - Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;

                return;
            }*/

            if (Math.Abs(state.hero.x - x) < 7 && Math.Abs(state.hero.y - y) < 7)
            {
                //Player draws aggro from bats if he is close enough
                this.moving = true;
                xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
            }
            else if (this.moving)
            {
                //Move towards original position
                xNext = x + Math.Cos(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;
                if ((Math.Round(this.x, 1) == this.origin_x || Math.Round(this.y, 1) == this.origin_y))
                {
                    //Original position has been reached
                    this.moving = false;
                    this.x = origin_x;
                    this.y = origin_y;
                    return;
                }
            }
            else
            {
                if (this.hp + this.hp_reg * 5 < this.full_hp)
                    this.hp += this.hp_reg * 5;
                else
                    this.hp = this.full_hp;
            }
        }

        public override void act()
        {
            if (inCombat)
            {
                if (combatCd-- > 0);
                else
                    inCombat = false;
            }


            if (burning_sec-- >= 0)
                burning();

            if (sleep_sec > 0)
            {
                sleep_sec--;
                return;
            }

            move();

            if (state.hero.status.Equals("Paralyzed"))
                this.atk_dmg = this.base_atk_dmg * 1.3;
            else
                this.atk_dmg = this.base_atk_dmg;

            //double xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
            //double yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;

            /*if ((state.hero.x - x) < 3 && (state.hero.x - x) > 0.2 && (state.hero.y - y) < 3 && (state.hero.y - y) > 0.2)
                this.speed = 0.15;
            else
                this.speed = 0.03;*/

            if (state.room.currentRoom.Equals(state.graveyard))
            {
                this.full_hp = 5 * Math.Pow(1.09, state.hero.level);
                this.hp = full_hp;
                this.atk_dmg += (atk_dmg * 0.30) * Math.Pow(1.09, state.hero.level);
                this.speed += (speed * 0.4) * Math.Pow(1.01, state.hero.level);
            }



            if (Math.Sqrt(Math.Pow(state.hero.x - x, 2) + Math.Pow(state.hero.y - y, 2)) < state.hero.radius + radius)
            {
                if (atk_cd[0])
                {
                    int random = rand.Next(0, 100);
                    if (random <= 35)
                        statusChanged(state.hero, "curse");
                }
            }


            if (Math.Abs(state.hero.x - x) < 5 && Math.Abs(state.hero.y - y) < 5 && this.teleport)
            {
                if (sleeparoni > 0)
                {
                    sleeparoni--;
                    return;
                }

                this.teleport = false;

                //If hero is 5 units close to the boss, have the boss teleport behind the player

                if ((state.hero.x - x) < 0 && (state.hero.y - y) < 0)
                {
                    //Boss is in bottom-right direction with respect to hero
                    x = state.hero.x - 0.5;
                    y = state.hero.y - 0.5;
                    return;
                }
                else if ((state.hero.x - x) < 0 && (state.hero.y - y) > 0)
                {
                    //Boss is in upper-right direction with respect to hero
                    x = state.hero.x - 0.5;
                    y = state.hero.y + 0.5;
                    return;
                }
                else if ((state.hero.x - x) > 0 && (state.hero.y - y) < 0)
                {
                    //Boss is in bottom-left direction with respect to hero
                    x = state.hero.x + 0.5;
                    y = state.hero.y - 0.5;
                    return;
                }
                else if ((state.hero.x - x) > 0 && (state.hero.y - y) > 0)
                {
                    //Boss is in upper-left direction with respect to hero
                    x = state.hero.x + 0.5;
                    y = state.hero.y + 0.5;
                    return;
                }
                else if ((state.hero.x - x) == 0 && (state.hero.y - y) < 0)
                {
                    //Boss is directly south of the player
                    y = state.hero.y - 0.5;
                    return;
                }
                else if ((state.hero.x - x) == 0 && (state.hero.y - y) > 0)
                {
                    //Boss is directly north of the player
                    y = state.hero.y + 0.5;
                    return;
                }
                else if ((state.hero.x - x) < 0 && (state.hero.y - y) == 0)
                {
                    //Boss is directly east of the player
                    x = state.hero.x - 0.5;
                    return;
                }
                else if ((state.hero.x - x) > 0 && (state.hero.y - y) == 0)
                {
                    //Boss is directly west of the player
                    x = state.hero.x + 0.5;
                    return;
                }
            }

        }

        public override void draw(Graphics g)
        {
            g.DrawImage(imgs[(int)animFrame], DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            animFrame = (animFrame + 0.1) % imgs.Length;
            //g.FillEllipse(Brushes.SaddleBrown, DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            drawHpBar(g);
            if (this.displayname)
                drawFileName(g);
        }
    }

    public class Boss : Unit
    {
        private Bitmap[] imgs = new Bitmap[10];
        private Random rand;
        private int sleeparoni;

        public Boss(GameState state, double x, double y)
            : base(state, x, y)
        {
            this.full_hp = 150 * Math.Pow(1.09, state.hero.level) - 20;
            this.hp = full_hp;
            this.hp_reg = this.full_hp * 0.001;
            this.atk_dmg = 2 * Math.Pow(1.1, state.hero.level);
            this.base_atk_dmg = this.atk_dmg;
            this.speed = 0.1 * Math.Pow(1.01, state.hero.level);
            this.radius = 0.65;
            this.origin_x = x;
            this.origin_y = y;
            this.center_x = x + radius;
            this.center_y = y + radius;
            this.exp = 8 * Math.Pow(1.09, state.hero.level);
            this.status = "Normal";
            this.lunge = true;
            this.inCombat = false;
            this.teleport = true;

            imgs[0] = new Bitmap(Properties.Resources.boss1);
            imgs[1] = new Bitmap(Properties.Resources.boss2);
            imgs[2] = new Bitmap(Properties.Resources.boss3);
            imgs[3] = new Bitmap(Properties.Resources.boss4);
            imgs[4] = new Bitmap(Properties.Resources.boss5);
            imgs[5] = new Bitmap(Properties.Resources.boss6);
            imgs[6] = new Bitmap(Properties.Resources.boss7);
            imgs[7] = new Bitmap(Properties.Resources.boss8);
            imgs[8] = new Bitmap(Properties.Resources.boss9);
            imgs[9] = new Bitmap(Properties.Resources.boss10);

            sleeparoni = 100;
            rand = new Random();
        }

        public void move()
        {
            double xNext;
            double yNext;

            /*if (this.hp < this.full_hp * 0.4)
            {
                this.hp += this.hp_reg;
                xNext = x - Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                yNext = y - Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed * 0.6;
                tryMove(xNext, yNext, this);
                this.center_x = x + radius;
                this.center_y = y + radius;

                return;
            }*/

            if (Math.Abs(state.hero.x - x) < 7 && Math.Abs(state.hero.y - y) < 7)
            {
                //Player draws aggro from bats if he is close enough
                this.moving = true;
                xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
                //tryMove(xNext, yNext, this);
                x = xNext;
                y = yNext;
                this.center_x = x + radius;
                this.center_y = y + radius;
            }
            else if (this.moving)
            {
                //Move towards original position
                xNext = x + Math.Cos(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                yNext = y + Math.Sin(Math.Atan2(this.origin_y - y, this.origin_x - x)) * speed;
                //tryMove(xNext, yNext, this);
                x = xNext;
                y = yNext;
                this.center_x = x + radius;
                this.center_y = y + radius;
                if ((Math.Round(this.x, 1) == this.origin_x || Math.Round(this.y, 1) == this.origin_y))
                {
                    //Original position has been reached
                    this.moving = false;
                    this.x = origin_x;
                    this.y = origin_y;
                    return;
                }
            }
            else
            {
                if (this.hp + this.hp_reg * 5 < this.full_hp)
                    this.hp += this.hp_reg * 5;
                else
                    this.hp = this.full_hp;
            }

        }

        public override void act()
        {
            if (inCombat)
            {
                if (combatCd-- > 0);
                else
                    inCombat = false;
            }

            if (knockback)
                knockBacked();

            if (burning_sec-- >= 0)
                burning();

            if (sleep_sec > 0)
            {
                sleep_sec--;
                return;
            }

            if (this.hp < (this.full_hp / 2))
            {
                this.status = "Frenzied";
            }

            move();

            if (state.hero.status.Equals("Paralyzed"))
                this.atk_dmg = this.base_atk_dmg * 1.3;
            else
                this.atk_dmg = this.base_atk_dmg;

            //double xNext = x + Math.Cos(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;
            //double yNext = y + Math.Sin(Math.Atan2(state.hero.y - y, state.hero.x - x)) * speed;

            /*if ((state.hero.x - x) < 3 && (state.hero.x - x) > 0.2 && (state.hero.y - y) < 3 && (state.hero.y - y) > 0.2)
                this.speed = 0.15;
            else
                this.speed = 0.03;*/

            if (state.room.currentRoom.Equals(state.graveyard))
            {
                this.full_hp = 5 * Math.Pow(1.09, state.hero.level);
                this.hp = full_hp;
                this.atk_dmg += (atk_dmg * 0.30) * Math.Pow(1.09, state.hero.level);
                this.speed += (speed * 0.4) * Math.Pow(1.01, state.hero.level);
            }

            if (Math.Sqrt(Math.Pow(state.hero.x - x, 2) + Math.Pow(state.hero.y - y, 2)) < state.hero.radius + radius)
            {
                if (atk_cd[0])
                {
                    int random = rand.Next(0, 100);
                    if (random <= 35)
                        statusChanged(state.hero, "head_bind");
                }
            }


            if (this.atk_cd[1])
            {

                
                if (this.status.Equals("Frenzied"))
                    this.cd(1, 1);
                else
                    this.cd(3, 1);
            }
            else if (this.atk_cd[2])
            {
                ShadowStep ss = new ShadowStep();
                this.cast(ss);
                this.cd(ss.cd, 2);
            }
            else if (this.atk_cd[3])
            {
                CrusingFireBall cs = new CrusingFireBall();
                this.cast(cs);
                this.cd(cs.cd, 3);
            }
            /*if (Math.Abs(state.hero.x - x) < 5 && Math.Abs(state.hero.y - y) < 5 && this.teleport)
            {
                if (sleeparoni > 0)
                {
                    sleeparoni--;
                    return;
                }
                sleeparoni = 200;
                //this.teleport = false;
                //BAK
                //If hero is 5 units close to the boss, have the boss teleport behind the player

                if ((state.hero.x - x) < 0 && (state.hero.y - y) < 0)
                {
                    //Boss is in bottom-right direction with respect to hero
                    x = state.hero.x - 0.5;
                    y = state.hero.y - 0.5;
                    return;
                }
                else if ((state.hero.x - x) < 0 && (state.hero.y - y) > 0)
                {
                    //Boss is in upper-right direction with respect to hero
                    x = state.hero.x - 0.5;
                    y = state.hero.y + 0.5;
                    return;
                }
                else if ((state.hero.x - x) > 0 && (state.hero.y - y) < 0)
                {
                    //Boss is in bottom-left direction with respect to hero
                    x = state.hero.x + 0.5;
                    y = state.hero.y - 0.5;
                    return;
                }
                else if ((state.hero.x - x) > 0 && (state.hero.y - y) > 0)
                {
                    //Boss is in upper-left direction with respect to hero
                    x = state.hero.x + 0.5;
                    y = state.hero.y + 0.5;
                    return;
                }
                else if ((state.hero.x - x) == 0 && (state.hero.y - y) < 0)
                {
                    //Boss is directly south of the player
                    y = state.hero.y - 0.5;
                    return;
                }
                else if ((state.hero.x - x) == 0 && (state.hero.y - y) > 0)
                {
                    //Boss is directly north of the player
                    y = state.hero.y + 0.5;
                    return;
                }
                else if ((state.hero.x - x) < 0 && (state.hero.y - y) == 0)
                {
                    //Boss is directly east of the player
                    x = state.hero.x - 0.5;
                    return;
                }
                else if ((state.hero.x - x) > 0 && (state.hero.y - y) == 0)
                {
                    //Boss is directly west of the player
                    x = state.hero.x + 0.5;
                    return;
                }
            }*/
        }

        public override void draw(Graphics g)
        {
            g.DrawImage(imgs[(int)animFrame], DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            animFrame = (animFrame + 0.1) % 4 + frame;
            //g.FillEllipse(Brushes.SaddleBrown, DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            drawHpBar(g);
            if (this.displayname)
                drawFileName(g);
        }
    }
}
