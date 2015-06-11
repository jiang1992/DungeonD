using System;
using System.Drawing;

namespace DungeonDrive
{
    public class Projectile
    {
        public const int MAX_RADIUS = 4;
        private GameState state;
        
        public double dmg = 1;
        public double atk_speed = 0.5;
        public double proj_speed = 0.8;
        public int proj_range = 10;
        public int proj_duration = 100;
        public int timer = 0;
        public Item.AtkStyle style = Item.AtkStyle.Basic;
        public double powerSec = 1;
        public double powerFac = 0.3;
        public Bitmap proj_img = Properties.Resources.lighteningball_1_20_9;
        public double x, y;
        public double x_origin, y_origin;
        public double x_speed, y_speed;
        public double radius = 0.3;
        public bool friendlyFire = true;
        public int frame = 0;
        public int maxFrame = 20;
        public Bitmap[] animation = new Bitmap[20];
        public bool isMagic = false;
        public Unit shooter;
        public int DrawX { get { return (int)(x * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size - state.size * radius); } }
        public int DrawY { get { return (int)(y * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size - state.size * radius); } }

        public Projectile(GameState state, double x, double y, double x_dir, double y_dir)
        {
            this.state = state;
            this.x = x;
            this.y = y;
            this.x_origin = x;
            this.y_origin = y;
            this.x_speed = x_dir * proj_speed;
            this.y_speed = y_dir * proj_speed;
        }
        public Projectile(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range)
        {
            this.state = state;
            this.x = x;
            this.y = y;
            this.x_origin = x;
            this.y_origin = y;
            this.x_speed = x_dir * proj_speed;
            this.y_speed = y_dir * proj_speed;
            this.proj_range = proj_range;
        }
        public void setProjectile(GameState state, double x, double y, double x_dir, double y_dir)
        {
            this.state = state;
            this.x = x;
            this.y = y;
            this.x_origin = x;
            this.y_origin = y;
            this.x_speed = x_dir * proj_speed;
            this.y_speed = y_dir * proj_speed;
        }
        public virtual void tryMove(double xNext, double yNext)
        {
            if (Math.Sqrt(Math.Pow(x - x_origin, 2) + Math.Pow(y - y_origin, 2)) >= proj_range)
                endingEffect();
            blockProjectiles();
            int left = (int)(xNext - radius);
            int top = (int)(yNext - radius);
            int width = (int)(radius * 2 + (xNext - (int)xNext < radius || 1 - (xNext - (int)xNext) < radius ? 2 : 1));
            int height = (int)(radius * 2 + (yNext - (int)yNext < radius || 1 - (yNext - (int)yNext) < radius ? 2 : 1));

            for (int i = left; i < left + width; i++)
                for (int j = top; j < top + height; j++)
                    if (i < 0 || i >= state.room.width || j < 0 || j >= state.room.height || !state.room.walkingSpace[i, j])
                        endingEffect();
            if (friendlyFire == true)
            {
                foreach (Unit unit in state.room.enemies)
                    if (Math.Sqrt(Math.Pow(xNext - unit.x, 2) + Math.Pow(yNext - unit.y, 2)) < radius + unit.radius)
                    {
                        endingEffect(unit);
                       
                        if (unit.hp <= 0)
                            state.hero.deletingList.Add(unit);
                        state.hero.inCombat = true;
                        state.hero.combatCd = 3 * 17;
                    }
            }
            else {
                if (Math.Sqrt(Math.Pow(xNext - state.hero.x, 2) + Math.Pow(yNext - state.hero.y, 2)) < radius + state.hero.radius)
                {

                    endingEffect(state.hero);
                    if (state.hero.hp <= 0)
                    {
                        state.addChildState(new GameOverState(state.form), false, true);
                        return;
                    }
                }
            
            }
            
            x = xNext;
            y = yNext;
            trailType();
        }
        public virtual void blockProjectiles() { }
        public virtual void trailType() { 
        
        }
        public virtual void endingEffect(Unit unit) {
            unit.hp -= this.dmg;
            if (this.style == Item.AtkStyle.Frozen)
                unit.slow(this.powerSec, this.powerFac);
            else if (this.style == Item.AtkStyle.Flame)
                unit.burn(this.powerSec, this.powerFac * this.dmg);
            state.room.removeProj(this);
        }
        public virtual void endingEffect()
        {
            state.room.removeProj(this);
        }

        public virtual void act()
        {
            frame++;
            timer++;
            if (frame >= maxFrame) { frame = 0; }

            tryMove(x + x_speed, y + y_speed);
            if (this.radius <= 0 || this.radius >= MAX_RADIUS || this.timer >= this.proj_duration) { state.room.removeProj(this); }
        }

        public void draw(Graphics g)
        {
            if (!isMagic)
                g.DrawImage(this.proj_img, new Rectangle(DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size)));
            else
                g.DrawImage(this.animation[frame], new Rectangle(DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size)));
        }
    }


    public class knockBackProjectile : Projectile
    {
        private GameState state;
        private Unit castor;
        private int bounce = 0;
        public double force = 0.5;
        public knockBackProjectile(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range, Unit castor, double force)
            : base(state, x, y, x_dir, y_dir, proj_speed, proj_range)
        {
            this.state = state;
            this.castor = castor;
            this.force = force;
        }

        public override void endingEffect(Unit unit)
        {
            double dir = (float)Math.Atan2(unit.y - this.y, unit.x - this.x);
            unit.knockBack(unit, Math.Cos((double)(dir)) * force, Math.Sin((double)(dir)) * force, 0);
            unit.hp -= unit.hp - this.dmg;
            unit.burn(this.powerSec, this.powerFac * this.dmg);
        }
        public override void endingEffect()
        {
            bounce++;
            this.x_speed = -this.x_speed;
            this.y_speed = -this.y_speed;
            if(bounce >=30){
                state.room.removeProj(this);
            }
        }
        public override void act()
        {
            base.act();
            this.radius += 0.01;
        }

    }

    public class barrierProjectiles : Projectile {
        private GameState state;
        public double force = 0.3;
        public barrierProjectiles(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range, double force)
            : base(state, x, y, x_dir, y_dir, proj_speed, proj_range)
        {
            this.state = state;
            this.force = force;
        }
        public override void endingEffect()
        {

        }
        public override void endingEffect(Unit unit)
        {
            unit.hp -= this.dmg;
            double dir = (float)Math.Atan2(unit.y - this.y, unit.x - this.x);
            unit.knockBack(unit, Math.Cos((double)(dir)) * force, Math.Sin((double)(dir)) * force, 0);
        }
        public override void trailType()
        {
            this.x = state.hero.x;
            this.y = state.hero.y;
        } 
    }
    public class CircleAroundProjectiles : Projectile {
        private GameState state;
        public CircleAroundProjectiles(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range, Unit shooter)
            : base(state, x, y, x_dir, y_dir, proj_speed, proj_range)
        {
            this.shooter = shooter;
            this.state = state;
        }
        public override void trailType()
        {
            //float dir = (float)Math.Atan2(state.hero.y - this.y, state.hero.x - this.x);
            float dir = (float)Math.Atan2(shooter.y - this.y, shooter.x - this.x);
            if (Math.Sqrt(Math.Pow(this.x - shooter.x, 2) + Math.Pow(this.y - shooter.y, 2)) >= 4)
            {
                this.x_speed = this.proj_speed/4 * Math.Cos(dir);
                this.y_speed = this.proj_speed/4 * Math.Sin(dir);
            }
            else if (Math.Sqrt(Math.Pow(this.x - shooter.x, 2) + Math.Pow(this.y - shooter.y, 2)) >= 3.5)
            {

                this.x_speed = this.proj_speed/4 * Math.Cos(dir - Math.PI / 2);
                this.y_speed = this.proj_speed/4 * Math.Sin(dir - Math.PI / 2);
            }

        }
        public override void endingEffect()
        {
            //base.endingEffect();
        }
        public override void endingEffect(Unit unit)
        {
            unit.hp -= this.dmg*2;
        }
    }

    public class staticProjectiles : Projectile
    {
        private GameState state;
        public staticProjectiles(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range)
            : base(state, x, y, x_dir, y_dir, proj_speed, proj_range)
        {
            this.state = state;
        }

        public override void endingEffect()
        {
            
        }
        public override void endingEffect(Unit unit)
        {
            unit.hp -= this.dmg;
            
        }
        public override void act()
        {
            frame++;
            timer++;
            if (frame >= maxFrame) { frame = 0; }

            tryMove(x, y);
            if (this.radius <= 0 || this.radius >= MAX_RADIUS || this.timer >= this.proj_duration) { state.room.removeProj(this); }
        }

    }
    public class pullingProjectile : Projectile
    {
        private GameState state;
        double force = 0.2;
        public pullingProjectile(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range, double force)
            : base(state, x, y, x_dir, y_dir, proj_speed, proj_range)
        {
            this.state = state;
            this.force = force;
        }

        public override void endingEffect()
        {

        }
        public override void endingEffect(Unit unit)
        {
            if(unit is Boss){return;}
            double dir = (float)Math.Atan2(unit.y - this.y, unit.x - this.x);
            unit.knockBack(unit, Math.Cos((double)(dir)) * -force, Math.Sin((double)(dir)) * -force, 0);
        }
        public override void blockProjectiles()
        {
            Rectangle Rekt = new Rectangle(DrawX, DrawY, (int)(radius * 2 * state.size), (int)(radius * 2 * state.size));
            foreach(Projectile proj in state.room.projectiles){
                 
                if (proj is pullingProjectile) {  }
                /*else if(proj is knockBackProjectile){ }*/
                else{
                    Rectangle temp = new Rectangle((int)proj.DrawX, (int)proj.DrawY, 1, 1);
                    if (Rekt.Contains(temp))
                    {
                        double dir = (float)Math.Atan2(proj.y - this.y, proj.x - this.x);
                        proj.x_speed = proj.x_speed - Math.Cos(dir) * 0.05;
                        proj.y_speed = proj.y_speed-  Math.Sin(dir) * 0.05;
                    }
                }
            }
        }

    }

    public class bossProjectile : Projectile {
        private GameState state;
        public bossProjectile(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range)
            : base(state, x, y, x_dir, y_dir, proj_speed, proj_range)
        {
            this.shooter = shooter;
            this.state = state;
            this.atk_speed = 1;
            this.dmg = 0.01;
            this.proj_img = Properties.Resources.fire;

        }

        public override void endingEffect(Unit unit)
        {
           // Bat bat = new Bat(state, this.DrawX, this.DrawY) ;
          //  state.room.addEnemy(bat, "Minion");
            state.room.removeProj(this);
            unit.hp -= this.dmg;
        }
    }
    /*
    public class enemyProjectile : Projectile
    {
        private GameState state;
        public enemyProjectile(GameState state, double x, double y, double x_dir, double y_dir, double proj_speed, int proj_range)
            : base(state, x, y, x_dir, y_dir, proj_speed, proj_range)
        {
            this.state = state;
        }
        public override void tryMove(double xNext, double yNext)
        {
            if (Math.Sqrt(Math.Pow(x - x_origin, 2) + Math.Pow(y - y_origin, 2)) >= proj_range)
                endingEffect();

            int left = (int)(xNext - radius);
            int top = (int)(yNext - radius);
            int width = (int)(radius * 2 + (xNext - (int)xNext < radius || 1 - (xNext - (int)xNext) < radius ? 2 : 1));
            int height = (int)(radius * 2 + (yNext - (int)yNext < radius || 1 - (yNext - (int)yNext) < radius ? 2 : 1));

            for (int i = left; i < left + width; i++)
                for (int j = top; j < top + height; j++)
                    if (i < 0 || i >= state.room.width || j < 0 || j >= state.room.height || !state.room.walkingSpace[i, j])
                        endingEffect();


            if (Math.Sqrt(Math.Pow(xNext - state.hero.x, 2) + Math.Pow(yNext - state.hero.y, 2)) < radius + state.hero.radius)
            {
                state.hero.hp -= this.dmg;
                if (this.style == Item.AtkStyle.Frozen)
                    state.hero.slow(this.powerSec, this.powerFac);
                else if (this.style == Item.AtkStyle.Flame)
                    state.hero.burn(this.powerSec, this.powerFac * this.dmg);

                endingEffect();
                if (state.hero.hp <= 0)
                {
                    state.addChildState(new GameOverState(state.form), false, true);
                    return;
                }
            }

            x = xNext;
            y = yNext;
        }

    }
    */

}