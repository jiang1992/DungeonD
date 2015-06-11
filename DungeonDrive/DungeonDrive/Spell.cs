using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DungeonDrive
{
    public class Spell
    {
        public Bitmap[] spellIcon = new Bitmap[SkillStreeState.skillLevel];
        public Bitmap[] _spellIcon = new Bitmap[SkillStreeState.skillLevel];
        public int duration;
        public int cd = 5;
        public int maxFrame;
        public String spellName = "";
        public String spellDesc;
        public int level = 1;
        public Spell() {}
        public virtual void cast(GameState state, Unit unit) {}
        public virtual void tick() { }

        public int getSkillLevel(Spell spell)
        {

            for (int i = 0; i < SkillStreeState.skillList; i++)
            {
                if (SkillStreeState.spellStored[i].Equals(spell))
                {
                    for (int j = 0; j < SkillStreeState.skillLevel; j++)
                    {
                        if (GameState.heroSkill[i, j] && j == SkillStreeState.skillLevel - 1) { return SkillStreeState.skillLevel; }
                        else if (GameState.heroSkill[i, j]) { }
                        else
                        {
                            return j;
                        }
                    }

                }


            }

            return 1;


        }
    }
    public class BossSpell : Spell {
        private GameState state;
        private Unit unit;
        public BossSpell()
            : base(){}
        public override void cast(GameState state, Unit unit)
        {
            setLighteningBall(state, unit);
            bossProjectile proj1 = new bossProjectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.2, 15);
            proj1.isMagic = false;

            proj1.friendlyFire = false;
            
            state.room.projectiles.Add(proj1);
        }
        public void setLighteningBall(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;
           
        }
    }
    public class LighteningBall : Spell {
        
        //public static Bitmap[] Icon = new Bitmap[SkillStreeState.skillLevel];
        public Bitmap[] animation= new Bitmap[20];
        
        GameState state;
        Unit unit;
        public override void cast(GameState state, Unit unit)
        {
            setLighteningBall(state, unit);
            Projectile proj1 = new Projectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.2, 15);
            proj1.isMagic = true;
            proj1.animation = this.animation;
            proj1.dmg = proj1.dmg + proj1.dmg * (unit.level / 10);
            Projectile proj2 = new Projectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.3, 15);
            proj2.isMagic = true;
            proj2.animation = this.animation;
            proj2.dmg = proj1.dmg + proj2.dmg * (unit.level / 10);
            Projectile proj3 = new Projectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.4, 15);
            proj3.isMagic = true;
            proj3.animation = this.animation;
            proj3.dmg = proj3.dmg + proj3.dmg * (unit.level / 10);
            Projectile proj4 = new Projectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.5, 15);
            proj4.isMagic = true;
            proj4.animation = this.animation;
            proj4.dmg = proj4.dmg + proj4.dmg * (unit.level / 10);
            Projectile proj5 = new Projectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.6, 15);
            //proj5.isMagic = true;
            proj5.proj_img = Properties.Resources.lightening;
            proj5.animation = this.animation;
            proj5.dmg = proj5.dmg + proj5.dmg * (unit.level / 10);
            if (this.unit is Hero) {
                this.level = getSkillLevel(this);
            }
            else
            {

                proj1.friendlyFire = false;
                proj2.friendlyFire = false;
                proj3.friendlyFire = false;
                proj4.friendlyFire = false;
            }

            state.room.projectiles.Add(proj1);
            state.room.projectiles.Add(proj2);
            state.room.projectiles.Add(proj3);
            if (this.level == 2)
            {
                state.room.projectiles.Add(proj4);
            }
            else if (this.level == 3)
            {
                state.room.projectiles.Add(proj5);
            }
        }

        public void setLighteningBall(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;       
        }

        public LighteningBall()
            : base()

        {
            this.spellName = "Lightening Ball";
            for (int i = 0; i < SkillStreeState.skillLevel; i++)
            {
                spellIcon[i] = Properties.Resources.ghost0;
                _spellIcon[i] = Properties.Resources.ghost0;
            }
            spellIcon[0] = Properties.Resources.LighteningBall1;
            spellIcon[1] = Properties.Resources.LighteningBall2;
            spellIcon[2] = Properties.Resources.LighteningBall3;
            _spellIcon[0] = Properties.Resources.notLighteningBall1;
            _spellIcon[1] = Properties.Resources.notLighteningBall2;
            _spellIcon[2] = Properties.Resources.notLighteningBall3;

            this.animation[0] = Properties.Resources.lighteningball_1_20_1;
            this.animation[1] = Properties.Resources.lighteningball_1_20_2;
            this.animation[2] = Properties.Resources.lighteningball_1_20_3;
            this.animation[3] = Properties.Resources.lighteningball_1_20_4;
            this.animation[4] = Properties.Resources.lighteningball_1_20_5;
            this.animation[5] = Properties.Resources.lighteningball_1_20_6;
            this.animation[6] = Properties.Resources.lighteningball_1_20_7;
            this.animation[7] = Properties.Resources.lightening;
            this.animation[8] = Properties.Resources.lighteningball_1_20_9;
            this.animation[9] = Properties.Resources.lighteningball_1_20_10;
            this.animation[10] = Properties.Resources.lighteningball_1_20_11;
            this.animation[11] = Properties.Resources.lighteningball_1_20_12;
            this.animation[12] = Properties.Resources.lighteningball_1_20_13;
            this.animation[13] = Properties.Resources.lighteningball_1_20_14;
            this.animation[14] = Properties.Resources.lighteningball_1_20_15;
            this.animation[15] = Properties.Resources.lighteningball_1_20_16;
            this.animation[16] = Properties.Resources.lighteningball_1_20_17;
            this.animation[17] = Properties.Resources.lighteningball_1_20_18;
            this.animation[18] = Properties.Resources.lighteningball_1_20_19;
            this.animation[19] = Properties.Resources.lightening;


        }

    
    }

    public class RuneOfFire : Spell {
        public Bitmap[] animation = new Bitmap[22];
        
        GameState state;
        Unit unit;
        public void setRuneOfFrost(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;
        }
        public RuneOfFire() : base()

        {
            this.spellName = "Rune Of Fire";
            for (int i = 0; i < SkillStreeState.skillLevel; i++)
            {
                spellIcon[i] = Properties.Resources.ghost0;
            }
            spellIcon[0] = Properties.Resources.rune_1;
            spellIcon[1] = Properties.Resources.rune_2;
            spellIcon[2] = Properties.Resources.rune_3;

            _spellIcon[0] = Properties.Resources.notRuneOfFire1;
            _spellIcon[1] = Properties.Resources.notRuneOfFire2;
            _spellIcon[2] = Properties.Resources.notRuneOfFire3;
            this.animation[0] = Properties.Resources.RUNEOFFIRE1;
            this.animation[1] = Properties.Resources.RUNEOFFIRE2;
            this.animation[2] = Properties.Resources.RUNEOFFIRE3;
            this.animation[3] = Properties.Resources.RUNEOFFIRE4;
            this.animation[4] = Properties.Resources.RUNEOFFIRE5;
            this.animation[5] = Properties.Resources.RUNEOFFIRE6;
            this.animation[6] = Properties.Resources.RUNEOFFIRE7;
            this.animation[7] = Properties.Resources.RUNEOFFIRE8;
            this.animation[8] = Properties.Resources.RUNEOFFIRE9;
            this.animation[9] = Properties.Resources.RUNEOFFIRE10;
            this.animation[10] = Properties.Resources.RUNEOFFIRE10;
            this.animation[11] = Properties.Resources.RUNEOFFIRE10;
            this.animation[12] = Properties.Resources.RUNEOFFIRE10;
            this.animation[13] = Properties.Resources.RUNEOFFIRE9;
            this.animation[14] = Properties.Resources.RUNEOFFIRE8;
            this.animation[15] = Properties.Resources.RUNEOFFIRE7;
            this.animation[16] = Properties.Resources.RUNEOFFIRE6;
            this.animation[17] = Properties.Resources.RUNEOFFIRE5;
            this.animation[18] = Properties.Resources.RUNEOFFIRE4;
            this.animation[19] = Properties.Resources.RUNEOFFIRE3;
            this.animation[20] = Properties.Resources.RUNEOFFIRE2;
            this.animation[21] = Properties.Resources.RUNEOFFIRE1;
        }

        public override void cast(GameState state, Unit unit)
        {
            setRuneOfFrost( state, unit);
            staticProjectiles proj1 = new staticProjectiles(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0, 5);
            proj1.isMagic = true;
            proj1.animation = this.animation;
            proj1.dmg = 0.1;
            proj1.dmg = proj1.dmg * (1 + unit.level / 10);
            proj1.radius = 2.9;

            if (this.unit is Hero) {
                this.level = getSkillLevel(this);
            }
            else
            {
                proj1.friendlyFire = false;
            }

            if (this.level == 2)
            {
                proj1.radius = 3.4;
                proj1.proj_duration = 200;
            }
            else if (this.level == 3)
            {
                proj1.radius = 3.9;
                proj1.proj_duration = 300;
            }
            state.room.projectiles.Add(proj1);



        }
    }

    public class EnergyBarrier : Spell
    {

        public Bitmap[] animation = new Bitmap[32];
       

        GameState state;
        Unit unit;
        public void setEnergyBarrier(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;
        }
        public EnergyBarrier()
            : base()
        {
            this.maxFrame = 32;
            this.spellName = "Energy Barrier";
            for (int i = 0; i < SkillStreeState.skillLevel; i++)
            {
                spellIcon[i] = Properties.Resources.ghost0;
            }
            spellIcon[0] = Properties.Resources.EB1;
            spellIcon[1] = Properties.Resources.EB2;
            spellIcon[2] = Properties.Resources.EB3;
            _spellIcon[0] = Properties.Resources.notEB1;
            _spellIcon[1] = Properties.Resources.notEB2;
            _spellIcon[2] = Properties.Resources.notEB3;
            this.animation[0] = Properties.Resources.EnergyBarrier1;
            this.animation[1] = Properties.Resources.EnergyBarrier2;
            this.animation[2] = Properties.Resources.EnergyBarrier3;
            this.animation[3] = Properties.Resources.EnergyBarrier4;
            this.animation[4] = Properties.Resources.EnergyBarrier5;
            this.animation[5] = Properties.Resources.EnergyBarrier6;
            this.animation[6] = Properties.Resources.EnergyBarrier7;
            this.animation[7] = Properties.Resources.EnergyBarrier8;
            this.animation[8] = Properties.Resources.EnergyBarrier9;
            this.animation[9] = Properties.Resources.EnergyBarrier10;
            this.animation[10] = Properties.Resources.EnergyBarrier11;
            this.animation[11] = Properties.Resources.EnergyBarrier12;
            this.animation[12] = Properties.Resources.EnergyBarrier13;
            this.animation[13] = Properties.Resources.EnergyBarrier14;
            this.animation[14] = Properties.Resources.EnergyBarrier15;
            this.animation[15] = Properties.Resources.EnergyBarrier15;
            this.animation[16] = Properties.Resources.EnergyBarrier17;
            this.animation[17] = Properties.Resources.EnergyBarrier18;
            this.animation[18] = Properties.Resources.EnergyBarrier19;
            this.animation[19] = Properties.Resources.EnergyBarrier20;
            this.animation[20] = Properties.Resources.EnergyBarrier21;
            this.animation[21] = Properties.Resources.EnergyBarrier22;
            this.animation[22] = Properties.Resources.EnergyBarrier23;
            this.animation[23] = Properties.Resources.EnergyBarrier24;
            this.animation[24] = Properties.Resources.EnergyBarrier25;
            this.animation[25] = Properties.Resources.EnergyBarrier26;
            this.animation[26] = Properties.Resources.EnergyBarrier27;
            this.animation[27] = Properties.Resources.EnergyBarrier28;
            this.animation[28] = Properties.Resources.EnergyBarrier29;
            this.animation[29] = Properties.Resources.EnergyBarrier30;
            this.animation[30] = Properties.Resources.EnergyBarrier31;
            this.animation[31] = Properties.Resources.EnergyBarrier32;
            
        }

        public override void cast(GameState state, Unit unit)
        {
            setEnergyBarrier(state, unit);
            double force = 0.3;
            barrierProjectiles proj1 = new barrierProjectiles(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0, 2, force);
            proj1.isMagic = true;
            proj1.animation = this.animation;
            proj1.dmg = 0;
            proj1.radius = 2;
            proj1.radius = proj1.radius + proj1.radius * unit.level / 10;
            proj1.maxFrame = this.maxFrame;
            proj1.proj_duration = 1000;
            if (this.unit is Hero) {
                this.level = getSkillLevel(this);
            }
            else
            {
                proj1.friendlyFire = false;
            }

            if (this.level == 2)
            {
                proj1.radius = 2.5;
                proj1.force = 0.5;
            }
            else if (this.level == 3)
            {
                proj1.radius = 3.5;
                proj1.force = 0.7;
         
            }
            state.room.projectiles.Add(proj1);

        }
    }

    public class CrusingFireBall : Spell{
        public Bitmap[] animation = new Bitmap[32];
       
        GameState state;
        Unit unit;

        public CrusingFireBall()
            : base()
        {
            this.maxFrame = 31;
            this.spellName = "Crusing Fire";
            for (int i = 0; i < SkillStreeState.skillLevel; i++)
            {
                spellIcon[i] = Properties.Resources.ghost0;
            }
            spellIcon[0] = Properties.Resources.CrusingfireIcon1;
            spellIcon[1] = Properties.Resources.CrusingfireIcon2;
            spellIcon[2] = Properties.Resources.CrusingfireIcon3;
            _spellIcon[0] = Properties.Resources.notCF1;
            _spellIcon[1] = Properties.Resources.notCF2;
            _spellIcon[2] = Properties.Resources.notCF3;
            this.animation[0] = Properties.Resources.CrusingFire1;
            this.animation[1] = Properties.Resources.CrusingFire1;
            this.animation[2] = Properties.Resources.CrusingFire2;
            this.animation[3] = Properties.Resources.CrusingFire2;
            this.animation[4] = Properties.Resources.CrusingFire3;
            this.animation[5] = Properties.Resources.CrusingFire3;
            this.animation[6] = Properties.Resources.CrusingFire4;
            this.animation[7] = Properties.Resources.CrusingFire5;
            this.animation[8] = Properties.Resources.CrusingFire6;
            this.animation[9] = Properties.Resources.CrusingFire7;
            this.animation[10] = Properties.Resources.CrusingFire8;
            this.animation[11] = Properties.Resources.CrusingFire9;
            this.animation[12] = Properties.Resources.CrusingFire10;
            this.animation[13] = Properties.Resources.CrusingFire11;
            this.animation[14] = Properties.Resources.CrusingFire12;
            this.animation[15] = Properties.Resources.CrusingFire13;
            this.animation[16] = Properties.Resources.CrusingFire14;
            this.animation[17] = Properties.Resources.CrusingFire14;
            this.animation[18] = Properties.Resources.CrusingFire13;
            this.animation[19] = Properties.Resources.CrusingFire12;
            this.animation[20] = Properties.Resources.CrusingFire11;
            this.animation[21] = Properties.Resources.CrusingFire10;
            this.animation[22] = Properties.Resources.CrusingFire9;
            this.animation[23] = Properties.Resources.CrusingFire8;
            this.animation[24] = Properties.Resources.CrusingFire7;
            this.animation[25] = Properties.Resources.CrusingFire6;
            this.animation[26] = Properties.Resources.CrusingFire5;
            this.animation[27] = Properties.Resources.CrusingFire4;
            this.animation[28] = Properties.Resources.CrusingFire3;
            this.animation[29] = Properties.Resources.CrusingFire2;
            this.animation[30] = Properties.Resources.CrusingFire1;
                       
        }
        public override void cast(GameState state, Unit unit)
        {
            setCrusingFire(state, unit);
            CircleAroundProjectiles proj1 = new CircleAroundProjectiles(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.3,2, unit);
            proj1.isMagic = true;
            proj1.animation = this.animation;
            proj1.dmg = 0.1;
            proj1.dmg = proj1.dmg + proj1.dmg * unit.level / 10;
            proj1.radius = 1;
            proj1.proj_duration = 1000;
            proj1.maxFrame = this.maxFrame;
            if (this.unit is Hero) {

                this.level = getSkillLevel(this);

            }
            else
            {
                proj1.friendlyFire = false;
                proj1.dmg = 0.3;
                proj1.dmg = proj1.dmg + proj1.dmg * unit.level / 10;
                
            }

            if (this.level == 2)
            {
                //proj1.animation[25] = Properties.Resources.lightening;
                
            }
            else if (this.level == 3)
            {
                //proj1.animation[25] = Properties.Resources.lightening;
                //proj1.proj_duration = 300;
            }
            state.room.projectiles.Add(proj1);

        }
        public void setCrusingFire(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;
        }
    }

    public class Pyroblast : Spell{
        public Bitmap[] animation = new Bitmap[32];
       
        GameState state;
        Unit unit;
        public Pyroblast()
            : base()
        {
            this.maxFrame = 31;
            this.spellName = "Pyroblast";
            for (int i = 0; i < SkillStreeState.skillLevel; i++)
            {
                spellIcon[i] = Properties.Resources.ghost0;
            }
            spellIcon[0] = Properties.Resources.pyroblast1;
            spellIcon[1] = Properties.Resources.pyroblast2;
            spellIcon[2] = Properties.Resources.pyroblast3;

            _spellIcon[0] = Properties.Resources.notPB1;
            _spellIcon[1] = Properties.Resources.notPB2;
            _spellIcon[2] = Properties.Resources.notPB3;
            this.animation[0] = Properties.Resources.fireball_1_64_1;
            this.animation[1] = Properties.Resources.fireball_1_64_2;
            this.animation[2] = Properties.Resources.fireball_1_64_3;
            this.animation[3] = Properties.Resources.fireball_1_64_4;
            this.animation[4] = Properties.Resources.fireball_1_64_5;
            this.animation[5] = Properties.Resources.fireball_1_64_6;
            this.animation[6] = Properties.Resources.fireball_1_64_7;
            this.animation[7] = Properties.Resources.fireball_1_64_8;
            this.animation[8] = Properties.Resources.fireball_1_64_9;
            this.animation[9] = Properties.Resources.fireball_1_64_10;
            this.animation[10] = Properties.Resources.fireball_1_64_11;
            this.animation[11] = Properties.Resources.fireball_1_64_12;
            this.animation[12] = Properties.Resources.fireball_1_64_13;
            this.animation[13] = Properties.Resources.fireball_1_64_14;
            this.animation[14] = Properties.Resources.fireball_1_64_15;
            this.animation[15] = Properties.Resources.fireball_1_64_16;
            this.animation[16] = Properties.Resources.fireball_1_64_17;
            this.animation[17] = Properties.Resources.fireball_1_64_18;
            this.animation[18] = Properties.Resources.fireball_1_64_19;
            this.animation[19] = Properties.Resources.fireball_1_64_20;
            this.animation[20] = Properties.Resources.fireball_1_64_21;
            this.animation[21] = Properties.Resources.fireball_1_64_22;
            this.animation[22] = Properties.Resources.fireball_1_64_23;
            this.animation[23] = Properties.Resources.fireball_1_64_24;
            this.animation[24] = Properties.Resources.fireball_1_64_25;
            this.animation[25] = Properties.Resources.fireball_1_64_26;
            this.animation[26] = Properties.Resources.fireball_1_64_27;
            this.animation[27] = Properties.Resources.fireball_1_64_28;
            this.animation[28] = Properties.Resources.fireball_1_64_29;
            this.animation[29] = Properties.Resources.fireball_1_64_30;
            this.animation[30] = Properties.Resources.fireball_1_64_31;
                       
        }
        public override void cast(GameState state, Unit unit)
        {
            setPyroblast(state, unit);
            knockBackProjectile proj1 = new knockBackProjectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.1, 12, unit, 0.5);
            //Projectile proj1 = new Projectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0.2, 15);
            //pullingProjectile proj1 = new pullingProjectile(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0, 15);
            proj1.isMagic = true;
            proj1.animation = this.animation;
            proj1.dmg = 0.1;
            proj1.radius = 0.2;
            //proj1.maxFrame = this.maxFrame;
            if (this.unit is Hero) { this.level = getSkillLevel(this); }
            else
            {
                proj1.friendlyFire = false;
            }
            if (this.level == 2)
            {
                proj1.force = 0.6;

            }
            else if (this.level == 3)
            {
                proj1.force = -0.5;
                this.cd = 1;
                //proj1.proj_duration = 300;
            }
            state.room.projectiles.Add(proj1);

        }
        public void setPyroblast(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;
        }

    }

    public class GravityForceField : Spell {
        public Bitmap[] animation = new Bitmap[52];
        
        GameState state;
        Unit unit;
        public GravityForceField()
            : base()
        {
            this.maxFrame = 48;
            this.spellName = "GravityForceField";
            for (int i = 0; i < SkillStreeState.skillLevel; i++)
            {
                spellIcon[i] = Properties.Resources.ghost0;
            }
            spellIcon[0] = Properties.Resources.GF1;
            spellIcon[1] = Properties.Resources.GF2;
            spellIcon[2] = Properties.Resources.GF3;
            _spellIcon[0] = Properties.Resources.notGF1;
            _spellIcon[1] = Properties.Resources.notGF2;
            _spellIcon[2] = Properties.Resources.notGF3;
            this.animation[0] = Properties.Resources.pulling1;
            this.animation[1] = Properties.Resources.pulling1;
            this.animation[2] = Properties.Resources.pulling1;
            this.animation[3] = Properties.Resources.pulling2;
            this.animation[4] = Properties.Resources.pulling2;
            this.animation[5] = Properties.Resources.pulling2;
            this.animation[6] = Properties.Resources.pulling3;
            this.animation[7] = Properties.Resources.pulling3;
            this.animation[8] = Properties.Resources.pulling3;
            this.animation[9] = Properties.Resources.pulling4;
            this.animation[10] = Properties.Resources.pulling4;
            this.animation[11] = Properties.Resources.pulling4;
            this.animation[12] = Properties.Resources.pulling5;
            this.animation[13] = Properties.Resources.pulling5;
            this.animation[14] = Properties.Resources.pulling5;
            this.animation[15] = Properties.Resources.pulling6;
            this.animation[16] = Properties.Resources.pulling6;
            this.animation[17] = Properties.Resources.pulling6;
            this.animation[18] = Properties.Resources.pulling7;
            this.animation[19] = Properties.Resources.pulling7;
            this.animation[20] = Properties.Resources.pulling7;
            this.animation[21] = Properties.Resources.pulling8;
            this.animation[22] = Properties.Resources.pulling8;
            this.animation[23] = Properties.Resources.pulling8;
            this.animation[24] = Properties.Resources.pulling9;
            this.animation[25] = Properties.Resources.pulling9;
            this.animation[26] = Properties.Resources.pulling9;
            this.animation[27] = Properties.Resources.pulling10;
            this.animation[28] = Properties.Resources.pulling10;
            this.animation[29] = Properties.Resources.pulling10;
            this.animation[30] = Properties.Resources.pulling11;
            this.animation[31] = Properties.Resources.pulling11;
            this.animation[32] = Properties.Resources.pulling11;
            this.animation[33] = Properties.Resources.pulling12;
            this.animation[34] = Properties.Resources.pulling12;
            this.animation[35] = Properties.Resources.pulling12;
            this.animation[36] = Properties.Resources.pulling13;
            this.animation[37] = Properties.Resources.pulling13;
            this.animation[38] = Properties.Resources.pulling13;
            this.animation[39] = Properties.Resources.pulling14;
            this.animation[40] = Properties.Resources.pulling14;
            this.animation[41] = Properties.Resources.pulling14;
            this.animation[42] = Properties.Resources.pulling15;
            this.animation[43] = Properties.Resources.pulling15;
            this.animation[44] = Properties.Resources.pulling16;
            this.animation[45] = Properties.Resources.pulling17;
            this.animation[46] = Properties.Resources.pulling17;
            this.animation[47] = Properties.Resources.pulling18;
            this.animation[48] = Properties.Resources.pulling19;
            this.animation[49] = Properties.Resources.pulling20;
            this.animation[50] = Properties.Resources.spell_bluetop_1_1;
            this.animation[51] = Properties.Resources.spell_bluetop_1_2;
            
        }
        public override void cast(GameState state, Unit unit)
        {
            
           
            setGravityForceField(state, unit);
            pullingProjectile proj1;
            double force = 0.2;
            force = force + (unit.level)/40;
            if (this.unit is Hero) {
                if (state.room.walkingSpace[(int)GameState.xMouse, (int)GameState.yMouse] == false) { return; }
                proj1 = new pullingProjectile(state, GameState.xMouse, GameState.yMouse, Math.Cos(unit.dir), Math.Sin(unit.dir), 0, 3, force);
                this.level = getSkillLevel(this);
            }
            else
            {
                proj1 = new pullingProjectile(state, state.hero.x, state.hero.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0, 3, force);
                proj1.friendlyFire = false;
                proj1.x = state.hero.x;
                proj1.y = state.hero.y;
            }
            proj1.isMagic = true;
            proj1.animation = this.animation;
            proj1.dmg = 0;
            proj1.radius = 2.9;
            proj1.maxFrame = this.maxFrame;
            proj1.proj_duration = 10000;

            if (this.level == 2)
            {
                proj1.radius = 3.4;
                this.cd = 4;
            }
            else if (this.level == 3)
            {
                proj1.radius = 3.9;
                this.cd = 3;
                //proj1.proj_duration = 300;
            }

            state.room.projectiles.Add(proj1);

        }
        public void setGravityForceField(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;
        }
    }

    public class ShadowStep : Spell {

        public Bitmap[] animation = new Bitmap[32];
        
        GameState state;
        Unit unit;
        public ShadowStep()
            : base()
        {
            this.maxFrame = 20;
            this.spellName = "ShadowStep";
            for (int i = 0; i < SkillStreeState.skillLevel; i++)
            {
                spellIcon[i] = Properties.Resources.ghost0;
            }
            spellIcon[0] = Properties.Resources.SS1;
            spellIcon[1] = Properties.Resources.SS2;
            spellIcon[2] = Properties.Resources.SS3;
            _spellIcon[0] = Properties.Resources.notSS1;
            _spellIcon[1] = Properties.Resources.notSS2;
            _spellIcon[2] = Properties.Resources.notSS3;
            this.animation[0] = Properties.Resources.shadowStep1;
            this.animation[1] = Properties.Resources.shadowStep2;
            this.animation[2] = Properties.Resources.shadowStep3;
            this.animation[3] = Properties.Resources.shadowStep4;
            this.animation[4] = Properties.Resources.shadowStep5;
            this.animation[5] = Properties.Resources.shadowStep6;
            this.animation[6] = Properties.Resources.shadowStep7;
            this.animation[7] = Properties.Resources.shadowStep8;
            this.animation[8] = Properties.Resources.shadowStep9;
            this.animation[9] = Properties.Resources.shadowStep10;
            this.animation[10] = Properties.Resources.shadowStep11;
            this.animation[11] = Properties.Resources.shadowStep12;
            this.animation[12] = Properties.Resources.shadowStep13;
            this.animation[13] = Properties.Resources.shadowStep14;
            this.animation[14] = Properties.Resources.shadowStep15;
            this.animation[15] = Properties.Resources.shadowStep16;
            this.animation[16] = Properties.Resources.shadowStep17;
            this.animation[17] = Properties.Resources.shadowStep18;
            this.animation[18] = Properties.Resources.shadowStep19;
            this.animation[19] = Properties.Resources.shadowStep19;
            /*this.animation[20] = Properties.Resources.pul;
            this.animation[21] = Properties.Resources.EnergyBarrier22;
            this.animation[22] = Properties.Resources.EnergyBarrier23;
            this.animation[23] = Properties.Resources.EnergyBarrier24;
            this.animation[24] = Properties.Resources.EnergyBarrier25;
            this.animation[25] = Properties.Resources.EnergyBarrier26;
            this.animation[26] = Properties.Resources.EnergyBarrier27;
            this.animation[27] = Properties.Resources.EnergyBarrier28;
            this.animation[28] = Properties.Resources.EnergyBarrier29;
            this.animation[29] = Properties.Resources.EnergyBarrier30;
            this.animation[30] = Properties.Resources.EnergyBarrier31;
            this.animation[31] = Properties.Resources.EnergyBarrier32;*/
            
        }
        public void setShadowStep(GameState state, Unit unit)
        {
            this.state = state;
            this.unit = unit;
        }
        public override void cast(GameState state, Unit unit)
        {
            setShadowStep(state, unit);
            staticProjectiles proj1 = new staticProjectiles(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0, 5);
            proj1.proj_duration = 10;
            proj1.isMagic = true;
            proj1.radius = 1;
            proj1.animation = this.animation;
            
            if (unit is Hero)
            {
                if (state.room.walkingSpace[(int)GameState.xMouse, (int)GameState.yMouse])
                {
                   state.hero.x = GameState.xMouse;
                   state.hero.y = GameState.yMouse;
                   state.angle = 0;
                   state.hero.basicAtk();
                }
            }
            else {
                if (Math.Sqrt(Math.Pow(state.hero.x - unit.x, 2) + Math.Pow(state.hero.y - unit.y, 2)) < 5 &&
                    state.room.walkingSpace[(int)(state.hero.x - 0.5), (int)(state.hero.y - 0.5)]) // distance
                {
                    unit.x = state.hero.x - 0.5;
                    unit.y = state.hero.y - 0.5;
                    unit.attackHero();
                }
                else { return; }
            }
            
            
            staticProjectiles proj2 = new staticProjectiles(state, unit.x, unit.y, Math.Cos(unit.dir), Math.Sin(unit.dir), 0, 5);
            proj2.proj_duration = 10;
            proj2.isMagic = true;
            proj2.radius = 1;
            proj2.animation = this.animation;

            if (unit is Hero)
            {
                this.level = getSkillLevel(this);
            }
            else {
                proj1.friendlyFire = false;
                proj2.friendlyFire = false;
            }
            proj1.dmg = 0;
            proj2.dmg = 0;

            if (this.level == 2)
            {
               
                this.cd = 2;
            }
            else if (this.level == 3)
            {
               
                this.cd = 1;
                //proj1.proj_duration = 300;
            }
            state.room.projectiles.Add(proj1);
            state.room.projectiles.Add(proj2);
        }
    }
}
