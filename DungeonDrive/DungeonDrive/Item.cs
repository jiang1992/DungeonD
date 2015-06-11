using System;
using System.Drawing;
using System.Media;

namespace DungeonDrive
{
    public abstract class Item
    {
        public enum AtkStyle
        {
            Basic,
            Flame,
            Frozen,
            Doom,
            Lightening,
            Poison
        }

        protected GameState state;
        protected static Random rand = new Random();
        public Bitmap img;
        public String name;
        public int level;
        public String description;
        public bool showDes = false;
        public static String[] adjectives = { "Abandoned", "Able", "Absolute", "Adorable", "Adventurous", "Academic", "Acceptable", "Acclaimed", "Accomplished", "Accurate", "Aching", "Acidic", "Acrobatic", "Active", "Actual", "Adept", "Admirable", "Admired", "Adolescent", "Adorable", "Adored", "Advanced", "Afraid", "Affectionate", "Aged", "Aggravating", "Aggressive", "Agile", "Agitated", "Agonizing", "Agreeable", "Ajar", "Alarmed", "Alarming", "Alert", "Alienated", "Alive", "All", "Altruistic", "Amazing", "Ambitious", "Ample", "Amused", "Amusing", "Anchored", "Ancient", "Angelic", "Angry", "Anguished", "Animated", "Annual", "Another", "Antique", "Anxious", "Any", "Apprehensive", "Appropriate", "Apt", "Arctic", "Arid", "Aromatic", "Artistic", "Ashamed", "Assured", "Astonishing", "Athletic", "Attached", "Attentive", "Attractive", "Austere", "Authentic", "Authorized", "Automatic", "Avaricious", "Average", "Aware", "Awesome", "Awful", "Awkward", "Babyish", "Bad", "Back", "Baggy", "Bare", "Barren", "Basic", "Beautiful", "Belated", "Beloved", "Beneficial", "Better", "Best", "Bewitched", "Big", "Big-Hearted", "Biodegradable", "Bite-Sized", "Bitter", "Black", "Black-And-White", "Bland", "Blank", "Blaring", "Bleak", "Blind", "Blissful", "Blond", "Blue", "Blushing", "Bogus", "Boiling", "Bold", "Bony", "Boring", "Bossy", "Both", "Bouncy", "Bountiful", "Bowed", "Brave", "Breakable", "Brief", "Bright", "Brilliant", "Brisk", "Broken", "Bronze", "Brown", "Bruised", "Bubbly", "Bulky", "Bumpy", "Buoyant", "Burdensome", "Burly", "Bustling", "Busy", "Buttery", "Buzzing", "Calculating", "Calm", "Candid", "Canine", "Capital", "Carefree", "Careful", "Careless", "Caring", "Cautious", "Cavernous", "Celebrated", "Charming", "Cheap", "Cheerful", "Cheery", "Chief", "Chilly", "Chubby", "Circular", "Classic", "Clean", "Clear", "Clear-Cut", "Clever", "Close", "Closed", "Cloudy", "Clueless", "Clumsy", "Cluttered", "Coarse", "Cold", "Colorful", "Colorless", "Colossal", "Comfortable", "Common", "Compassionate", "Competent", "Complete", "Complex", "Complicated", "Composed", "Concerned", "Concrete", "Confused", "Conscious", "Considerate", "Constant", "Content", "Conventional", "Cooked", "Cool", "Cooperative", "Coordinated", "Corny", "Corrupt", "Costly", "Courageous", "Courteous", "Crafty", "Crazy", "Creamy", "Creative", "Creepy", "Criminal", "Crisp", "Critical", "Crooked", "Crowded", "Cruel", "Crushing", "Cuddly", "Cultivated", "Cultured", "Cumbersome", "Curly", "Curvy", "Cute", "Cylindrical", "Damaged", "Damp", "Dangerous", "Dapper", "Daring", "Darling", "Dark", "Dazzling", "Dead", "Deadly", "Deafening", "Dear", "Dearest", "Decent", "Decimal", "Decisive", "Deep", "Defenseless", "Defensive", "Defiant", "Deficient", "Definite", "Definitive", "Delayed", "Delectable", "Delicious", "Delightful", "Delirious", "Demanding", "Dense", "Dental", "Dependable", "Dependent", "Descriptive", "Deserted", "Detailed", "Determined", "Devoted", "Different", "Difficult", "Digital", "Diligent", "Dim", "Dimpled", "Dimwitted", "Direct", "Disastrous", "Discrete", "Disfigured", "Disgusting", "Disloyal", "Dismal", "Distant", "Downright", "Dreary", "Dirty", "Disguised", "Dishonest", "Dismal", "Distant", "Distinct", "Distorted", "Dizzy", "Dopey", "Doting", "Double", "Downright", "Drab", "Drafty", "Dramatic", "Dreary", "Droopy", "Dry", "Dual", "Dull", "Dutiful", "Each", "Eager", "Earnest", "Early", "Easy", "Easy-Going", "Ecstatic", "Edible", "Educated", "Elaborate", "Elastic", "Elated", "Elderly", "Electric", "Elegant", "Elementary", "Elliptical", "Embarrassed", "Embellished", "Eminent", "Emotional", "Empty", "Enchanted", "Enchanting", "Energetic", "Enlightened", "Enormous", "Enraged", "Entire", "Envious", "Equal", "Equatorial", "Essential", "Esteemed", "Ethical", "Euphoric", "Even", "Evergreen", "Everlasting", "Every", "Evil", "Exalted", "Excellent", "Exemplary", "Exhausted", "Excitable", "Excited", "Exciting", "Exotic", "Expensive", "Experienced", "Expert", "Extraneous", "Extroverted", "Extra-Large", "Extra-Small", "Fabulous", "Failing", "Faint", "Fair", "Faithful", "Fake", "False", "Familiar", "Famous", "Fancy", "Fantastic", "Far", "Faraway", "Far-Flung", "Far-Off", "Fast", "Fat", "Fatal", "Fatherly", "Favorable", "Favorite", "Fearful", "Fearless", "Feisty", "Feline", "Female", "Feminine", "Few", "Fickle", "Filthy", "Fine", "Finished", "Firm", "First", "Firsthand", "Fitting", "Fixed", "Flaky", "Flamboyant", "Flashy", "Flat", "Flawed", "Flawless", "Flickering", "Flimsy", "Flippant", "Flowery", "Fluffy", "Fluid", "Flustered", "Focused", "Fond", "Foolhardy", "Foolish", "Forceful", "Forked", "Formal", "Forsaken", "Forthright", "Fortunate", "Fragrant", "Frail", "Frank", "Frayed", "Free", "French", "Fresh", "Frequent", "Friendly", "Frightened", "Frightening", "Frigid", "Frilly", "Frizzy", "Frivolous", "Front", "Frosty", "Frozen", "Frugal", "Fruitful", "Full", "Fumbling", "Functional", "Funny", "Fussy", "Fuzzy", "Gargantuan", "Gaseous", "General", "Generous", "Gentle", "Genuine", "Giant", "Giddy", "Gigantic", "Gifted", "Giving", "Glamorous", "Glaring", "Glass", "Gleaming", "Gleeful", "Glistening", "Glittering", "Gloomy", "Glorious", "Glossy", "Glum", "Golden", "Good", "Good-Natured", "Gorgeous", "Graceful", "Gracious", "Grand", "Grandiose", "Granular", "Grateful", "Grave", "Gray", "Great", "Greedy", "Green", "Gregarious", "Grim", "Grimy", "Gripping", "Gross", "Grotesque", "Grouchy", "Grounded", "Growing", "Growling", "Grown", "Grubby", "Gruesome", "Grumpy", "Guilty", "Gullible", "Gummy", "Hairy", "Half", "Handmade", "Handsome", "Handy", "Happy", "Happy-Go-Lucky", "Hard", "Hard-To-Find", "Harmful", "Harmless", "Harmonious", "Harsh", "Hasty", "Hateful", "Haunting", "Healthy", "Heartfelt", "Hearty", "Heavenly", "Heavy", "Hefty", "Helpful", "Helpless", "Hidden", "Hideous", "High", "High-Level", "Hilarious", "Hoarse", "Hollow", "Homely", "Honest", "Honorable", "Honored", "Hopeful", "Horrible", "Hospitable", "Hot", "Huge", "Humble", "Humiliating", "Humming", "Humongous", "Hungry", "Hurtful", "Husky", "Icky", "Icy", "Ideal", "Idealistic", "Identical", "Idle", "Idiotic", "Idolized", "Ignorant", "Ill", "Illegal", "Ill-Fated", "Ill-Informed", "Illiterate", "Illustrious", "Imaginary", "Imaginative", "Immaculate", "Immaterial", "Immediate", "Immense", "Impassioned", "Impeccable", "Impartial", "Imperfect", "Imperturbable", "Impish", "Impolite", "Important", "Impossible", "Impractical", "Impressionable", "Impressive", "Improbable", "Impure", "Inborn", "Incomparable", "Incompatible", "Incomplete", "Inconsequential", "Incredible", "Indelible", "Inexperienced", "Indolent", "Infamous", "Infantile", "Infatuated", "Inferior", "Infinite", "Informal", "Innocent", "Insecure", "Insidious", "Insignificant", "Insistent", "Instructive", "Insubstantial", "Intelligent", "Intent", "Intentional", "Interesting", "Internal", "International", "Intrepid", "Ironclad", "Irresponsible", "Irritating", "Itchy", "Jaded", "Jagged", "Jam-Packed", "Jaunty", "Jealous", "Jittery", "Joint", "Jolly", "Jovial", "Joyful", "Joyous", "Jubilant", "Judicious", "Juicy", "Jumbo", "Junior", "Jumpy", "Juvenile", "Kaleidoscopic", "Keen", "Key", "Kind", "Kindhearted", "Kindly", "Klutzy", "Knobby", "Knotty", "Knowledgeable", "Knowing", "Known", "Kooky", "Kosher", "Lame", "Lanky", "Large", "Last", "Lasting", "Late", "Lavish", "Lawful", "Lazy", "Leading", "Lean", "Leafy", "Left", "Legal", "Legitimate", "Light", "Lighthearted", "Likable", "Likely", "Limited", "Limp", "Limping", "Linear", "Lined", "Liquid", "Little", "Live", "Lively", "Livid", "Loathsome", "Lone", "Lonely", "Long", "Long-Term", "Loose", "Lopsided", "Lost", "Loud", "Lovable", "Lovely", "Loving", "Low", "Loyal", "Lucky", "Lumbering", "Luminous", "Lumpy", "Lustrous", "Luxurious", "Mad", "Made-Up", "Magnificent", "Majestic", "Major", "Male", "Mammoth", "Married", "Marvelous", "Masculine", "Massive", "Mature", "Meager", "Mealy", "Mean", "Measly", "Meaty", "Medical", "Mediocre", "Medium", "Meek", "Mellow", "Melodic", "Memorable", "Menacing", "Merry", "Messy", "Metallic", "Mild", "Milky", "Mindless", "Miniature", "Minor", "Minty", "Miserable", "Miserly", "Misguided", "Misty", "Mixed", "Modern", "Modest", "Moist", "Monstrous", "Monthly", "Monumental", "Moral", "Mortified", "Motherly", "Motionless", "Mountainous", "Muddy", "Muffled", "Multicolored", "Mundane", "Murky", "Mushy", "Musty", "Muted", "Mysterious", "Naive", "Narrow", "Nasty", "Natural", "Naughty", "Nautical", "Near", "Neat", "Necessary", "Needy", "Negative", "Neglected", "Negligible", "Neighboring", "Nervous", "New", "Next", "Nice", "Nifty", "Nimble", "Nippy", "Nocturnal", "Noisy", "Nonstop", "Normal", "Notable", "Noted", "Noteworthy", "Novel", "Noxious", "Numb", "Nutritious", "Nutty", "Obedient", "Obese", "Oblong", "Oily", "Oblong", "Obvious", "Occasional", "Odd", "Oddball", "Offbeat", "Offensive", "Official", "Old", "Old-Fashioned", "Only", "Open", "Optimal", "Optimistic", "Opulent", "Orange", "Orderly", "Organic", "Ornate", "Ornery", "Ordinary", "Original", "Other", "Our", "Outlying", "Outgoing", "Outlandish", "Outrageous", "Outstanding", "Oval", "Overcooked", "Overdue", "Overjoyed", "Overlooked", "Palatable", "Pale", "Paltry", "Parallel", "Parched", "Partial", "Passionate", "Past", "Pastel", "Peaceful", "Peppery", "Perfect", "Perfumed", "Periodic", "Perky", "Personal", "Pertinent", "Pesky", "Pessimistic", "Petty", "Phony", "Physical", "Piercing", "Pink", "Pitiful", "Plain", "Plaintive", "Plastic", "Playful", "Pleasant", "Pleased", "Pleasing", "Plump", "Plush", "Polished", "Polite", "Political", "Pointed", "Pointless", "Poised", "Poor", "Popular", "Portly", "Posh", "Positive", "Possible", "Potable", "Powerful", "Powerless", "Practical", "Precious", "Present", "Prestigious", "Pretty", "Precious", "Previous", "Pricey", "Prickly", "Primary", "Prime", "Pristine", "Private", "Prize", "Probable", "Productive", "Profitable", "Profuse", "Proper", "Proud", "Prudent", "Punctual", "Pungent", "Puny", "Pure", "Purple", "Pushy", "Putrid", "Puzzled", "Puzzling", "Quaint", "Qualified", "Quarrelsome", "Quarterly", "Queasy", "Querulous", "Questionable", "Quick", "Quick-Witted", "Quiet", "Quintessential", "Quirky", "Quixotic", "Quizzical", "Radiant", "Ragged", "Rapid", "Rare", "Rash", "Raw", "Recent", "Reckless", "Rectangular", "Ready", "Real", "Realistic", "Reasonable", "Red", "Reflecting", "Regal", "Regular", "Reliable", "Relieved", "Remarkable", "Remorseful", "Remote", "Repentant", "Required", "Respectful", "Responsible", "Repulsive", "Revolving", "Rewarding", "Rich", "Rigid", "Right", "Ringed", "Ripe", "Roasted", "Robust", "Rosy", "Rotating", "Rotten", "Rough", "Round", "Rowdy", "Royal", "Rubbery", "Rundown", "Ruddy", "Rude", "Runny", "Rural", "Rusty", "Sad", "Safe", "Salty", "Same", "Sandy", "Sane", "Sarcastic", "Sardonic", "Satisfied", "Scaly", "Scarce", "Scared", "Scary", "Scented", "Scholarly", "Scientific", "Scornful", "Scratchy", "Scrawny", "Second", "Secondary", "Second-Hand", "Secret", "Self-Assured", "Self-Reliant", "Selfish", "Sentimental", "Separate", "Serene", "Serious", "Serpentine", "Several", "Severe", "Shabby", "Shadowy", "Shady", "Shallow", "Shameful", "Shameless", "Sharp", "Shimmering", "Shiny", "Shocked", "Shocking", "Shoddy", "Short", "Short-Term", "Showy", "Shrill", "Shy", "Sick", "Silent", "Silky", "Silly", "Silver", "Similar", "Simple", "Simplistic", "Sinful", "Single", "Sizzling", "Skeletal", "Skinny", "Sleepy", "Slight", "Slim", "Slimy", "Slippery", "Slow", "Slushy", "Small", "Smart", "Smoggy", "Smooth", "Smug", "Snappy", "Snarling", "Sneaky", "Sniveling", "Snoopy", "Sociable", "Soft", "Soggy", "Solid", "Somber", "Some", "Spherical", "Sophisticated", "Sore", "Sorrowful", "Soulful", "Soupy", "Sour", "Spanish", "Sparkling", "Sparse", "Specific", "Spectacular", "Speedy", "Spicy", "Spiffy", "Spirited", "Spiteful", "Splendid", "Spotless", "Spotted", "Spry", "Square", "Squeaky", "Squiggly", "Stable", "Staid", "Stained", "Stale", "Standard", "Starchy", "Stark", "Starry", "Steep", "Sticky", "Stiff", "Stimulating", "Stingy", "Stormy", "Straight", "Strange", "Steel", "Strict", "Strident", "Striking", "Striped", "Strong", "Studious", "Stunning", "Stupendous", "Stupid", "Sturdy", "Stylish", "Subdued", "Submissive", "Substantial", "Subtle", "Suburban", "Sudden", "Sugary", "Sunny", "Super", "Superb", "Superficial", "Superior", "Supportive", "Sure-Footed", "Surprised", "Suspicious", "Svelte", "Sweaty", "Sweet", "Sweltering", "Swift", "Sympathetic", "Tall", "Talkative", "Tame", "Tan", "Tangible", "Tart", "Tasty", "Tattered", "Taut", "Tedious", "Teeming", "Tempting", "Tender", "Tense", "Tepid", "Terrible", "Terrific", "Testy", "Thankful", "That", "These", "Thick", "Thin", "Third", "Thirsty", "This", "Thorough", "Thorny", "Those", "Thoughtful", "Threadbare", "Thrifty", "Thunderous", "Tidy", "Tight", "Timely", "Tinted", "Tiny", "Tired", "Torn", "Total", "Tough", "Traumatic", "Treasured", "Tremendous", "Tragic", "Trained", "Tremendous", "Triangular", "Tricky", "Trifling", "Trim", "Trivial", "Troubled", "True", "Trusting", "Trustworthy", "Trusty", "Truthful", "Tubby", "Turbulent", "Twin", "Ugly", "Ultimate", "Unacceptable", "Unaware", "Uncomfortable", "Uncommon", "Unconscious", "Understated", "Unequaled", "Uneven", "Unfinished", "Unfit", "Unfolded", "Unfortunate", "Unhappy", "Unhealthy", "Uniform", "Unimportant", "United", "Unkempt", "Unknown", "Unlawful", "Unlined", "Unlucky", "Unnatural", "Unpleasant", "Unrealistic", "Unripe", "Unruly", "Unselfish", "Unsightly", "Unsteady", "Unsung", "Untidy", "Untimely", "Untried", "Untrue", "Unused", "Unusual", "Unwelcome", "Unwieldy", "Unwilling", "Unwitting", "Unwritten", "Upbeat", "Upright", "Upset", "Urban", "Usable", "Used", "Useful", "Useless", "Utilized", "Utter", "Vacant", "Vague", "Vain", "Valid", "Vapid", "Variable", "Vast", "Velvety", "Venerated", "Vengeful", "Verifiable", "Vibrant", "Vicious", "Victorious", "Vigilant", "Vigorous", "Villainous", "Violet", "Violent", "Virtual", "Virtuous", "Visible", "Vital", "Vivacious", "Vivid", "Voluminous", "Wan", "Warlike", "Warm", "Warmhearted", "Warped", "Wary", "Wasteful", "Watchful", "Waterlogged", "Watery", "Wavy", "Wealthy", "Weak", "Weary", "Webbed", "Wee", "Weekly", "Weepy", "Weighty", "Weird", "Welcome", "Well-Documented", "Well-Groomed", "Well-Informed", "Well-Lit", "Well-Made", "Well-Off", "Well-To-Do", "Well-Worn", "Wet", "Which", "Whimsical", "Whirlwind", "Whispered", "White", "Whole", "Whopping", "Wicked", "Wide", "Wide-Eyed", "Wiggly", "Wild", "Willing", "Wilted", "Winding", "Windy", "Winged", "Wiry", "Wise", "Witty", "Wobbly", "Woeful", "Wonderful", "Wooden", "Woozy", "Wordy", "Worldly", "Worn", "Worried", "Worrisome", "Worse", "Worst", "Worthless", "Worthwhile", "Worthy", "Wrathful", "Wretched", "Writhing", "Wrong", "Wry", "Yawning", "Yearly", "Yellow", "Yellowish", "Young", "Youthful", "Yummy", "Zany", "Zealous", "Zigzag" };
        public static String[] tierAdj = { "Broken", "Basic", "New", "Heavy", "Swift", "Iron", "Bronze", "Silver", "Well", "Shiny", "Rage", "Gold", "Platinum", "Samurai", "Legendary", "Ultimate" };

        public Item(GameState state) { this.state = state; }

        public double rdnDouble(double first, double second) { return rand.NextDouble() * (second - first) + first; }

        public String genName(int level)
        {
            String name = "";
            if (level == 1 || level == 2)
                name = tierAdj[0];
            else if (level > 2 && level <= 5)
                name = tierAdj[1];
            else if (level > 5 && level <= 10)
                name = tierAdj[2];
            else if (level > 10 && level <= 15)
                name = tierAdj[3];
            else if (level > 15 && level <= 20)
                name = tierAdj[4];
            else if (level > 20 && level <= 25)
                name = tierAdj[5];
            else if (level > 25 && level <= 30)
                name = tierAdj[6];
            else if (level > 30 && level <= 35)
                name = tierAdj[7];
            else if (level > 35 && level <= 40)
                name = tierAdj[8];
            else if (level > 40 && level <= 45)
                name = tierAdj[9];
            else if (level > 45 && level <= 50)
                name = tierAdj[10];
            else if (level > 50 && level <= 55)
                name = tierAdj[11];
            else if (level > 55 && level <= 60)
                name = tierAdj[12];
            else if (level > 60 && level <= 65)
                name = tierAdj[13];
            else if (level > 65 && level <= 70)
                name = tierAdj[14];
            else if (level > 70)
                name = tierAdj[15];

            return name;
        }
    }

    public class Helmet : Item
    {
        public double hp;
        public double hp_reg;

        public Helmet(GameState state)
            : base(state)
        {
            level = state.hero.level;
            name = adjectives[rand.Next(adjectives.Length)] + " " + genName(level);

            switch (rand.Next(1))
            {
                case 0:
                    name += " Helmet";
                    hp = rdnDouble(0.1 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level), 0.2 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level));
                    hp_reg = rdnDouble(0.001 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level), 0.002 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level));
                    setImg();
                    break;
            }

            setDesc();
        }

        public Helmet(GameState state, String name, int level, double hp, double hp_reg)
            : base(state)
        {
            this.name = name;
            this.level = level;
            this.hp = hp;
            this.hp_reg = hp_reg;
            setImg();
            setDesc();
        }

        private void setImg()
        {
            if (level == 1 || level == 2)
                this.img = Properties.Resources.helmet_1;
            else if (level > 2 && level <= 5)
                this.img = Properties.Resources.helmet_2;
            else if (level > 5 && level <= 10)
                this.img = Properties.Resources.helmet_3;
            else if (level > 10 && level <= 15)
                this.img = Properties.Resources.helmet_4;
            else if (level > 15 && level <= 20)
                this.img = Properties.Resources.helmet_5;
            else if (level > 20 && level <= 25)
                this.img = Properties.Resources.helmet_6;
            else if (level > 25 && level <= 30)
                this.img = Properties.Resources.helmet_7;
            else if (level > 30 && level <= 35)
                this.img = Properties.Resources.helmet_8;
            else if (level > 35 && level <= 40)
                this.img = Properties.Resources.helmet_9;
            else if (level > 40 && level <= 45)
                this.img = Properties.Resources.helmet_10;
            else if (level > 45 && level <= 50)
                this.img = Properties.Resources.helmet_11;
            else if (level > 50 && level <= 55)
                this.img = Properties.Resources.helmet_12;
            else if (level > 55 && level <= 60)
                this.img = Properties.Resources.helmet_13;
            else if (level > 60 && level <= 65)
                this.img = Properties.Resources.helmet_14;
            else if (level > 65 && level <= 70)
                this.img = Properties.Resources.helmet_15;
            else if (level > 70)
                this.img = Properties.Resources.helmet_16;
        }

        private void setDesc()
        {
            description = name
                + "\nLVL:  " + level
                + "\nHP : +" + Math.Round(hp, 2)
                + "\nHP REG: +" + Math.Round(hp_reg, 2);
        }
    }

    public class Armor : Item
    {
        public double hp;

        public Armor(GameState state)
            : base(state)
        {
            level = state.hero.level;
            name = adjectives[rand.Next(adjectives.Length)] + " " + genName(level);

            switch (rand.Next(1))
            {
                case 0:
                    name += " Armor";
                    hp = rdnDouble(0.3 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level), 0.6 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level));
                    setImg();
                    break;
            }

            setDesc();
        }

        public Armor(GameState state, String name, int level, double hp)
            : base(state)
        {
            this.name = name;
            this.level = level;
            this.hp = hp;
            setImg();
            setDesc();
        }

        private void setImg()
        {
            if (level == 1 || level == 2)
                this.img = Properties.Resources.armor_1;
            else if (level > 2 && level <= 5)
                this.img = Properties.Resources.armor_2;
            else if (level > 5 && level <= 10)
                this.img = Properties.Resources.armor_3;
            else if (level > 10 && level <= 15)
                this.img = Properties.Resources.armor_4;
            else if (level > 15 && level <= 20)
                this.img = Properties.Resources.armor_5;
            else if (level > 20 && level <= 25)
                this.img = Properties.Resources.armor_6;
            else if (level > 25 && level <= 30)
                this.img = Properties.Resources.armor_7;
            else if (level > 30 && level <= 35)
                this.img = Properties.Resources.armor_8;
            else if (level > 35 && level <= 40)
                this.img = Properties.Resources.armor_9;
            else if (level > 40 && level <= 45)
                this.img = Properties.Resources.armor_10;
            else if (level > 45 && level <= 50)
                this.img = Properties.Resources.armor_11;
            else if (level > 50 && level <= 55)
                this.img = Properties.Resources.armor_12;
            else if (level > 55 && level <= 60)
                this.img = Properties.Resources.armor_13;
            else if (level > 60 && level <= 65)
                this.img = Properties.Resources.armor_14;
            else if (level > 65 && level <= 70)
                this.img = Properties.Resources.armor_15;
            else if (level > 70)
                this.img = Properties.Resources.armor_16;
        }

        private void setDesc()
        {
            description = name
                + "\nLVL:  " + level
                + "\nHP : +" + Math.Round(hp, 2);
        }
    }

    public class Legs : Item
    {
        public double hp;
        public double ms;

        public Legs(GameState state)
            : base(state)
        {
            level = state.hero.level;
            name = adjectives[rand.Next(adjectives.Length)] + " " + genName(level);

            switch (rand.Next(1))
            {
                case 0:
                    name += " Legs";
                    hp = rdnDouble(0.1 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level), 0.2 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level));
                    ms = rdnDouble(0.06 * Math.Pow(1.01, (double)state.hero.level), 0.1 * Math.Pow(1.01, (double)state.hero.level));
                    setImg();
                    break;
            }

            setDesc();
        }

        public Legs(GameState state, String name, int level, double hp, double ms)
            : base(state)
        {
            this.name = name;
            this.level = level;
            this.hp = hp;
            this.ms = ms;
            setImg();
            setDesc();
        }

        private void setImg()
        {
            if (level == 1 || level == 2)
                this.img = Properties.Resources.legs_1;
            else if (level > 2 && level <= 5)
                this.img = Properties.Resources.legs_2;
            else if (level > 5 && level <= 10)
                this.img = Properties.Resources.legs_3;
            else if (level > 10 && level <= 15)
                this.img = Properties.Resources.legs_4;
            else if (level > 15 && level <= 20)
                this.img = Properties.Resources.legs_5;
            else if (level > 20 && level <= 25)
                this.img = Properties.Resources.legs_6;
            else if (level > 25 && level <= 30)
                this.img = Properties.Resources.legs_7;
            else if (level > 30 && level <= 35)
                this.img = Properties.Resources.legs_8;
            else if (level > 35 && level <= 40)
                this.img = Properties.Resources.legs_9;
            else if (level > 40 && level <= 45)
                this.img = Properties.Resources.legs_10;
            else if (level > 45 && level <= 50)
                this.img = Properties.Resources.legs_11;
            else if (level > 50 && level <= 55)
                this.img = Properties.Resources.legs_12;
            else if (level > 55 && level <= 60)
                this.img = Properties.Resources.legs_13;
            else if (level > 60 && level <= 65)
                this.img = Properties.Resources.legs_14;
            else if (level > 65 && level <= 70)
                this.img = Properties.Resources.legs_15;
            else if (level > 70)
                this.img = Properties.Resources.legs_16;
        }

        private void setDesc()
        {
            description = name
                + "\nLVL:  " + level
                + "\nHP : +" + Math.Round(hp, 2)
                + "\nMS : +" + Math.Round(ms, 2);
        }
    }

    public class Shield : Item
    {
        public double hp;
        public double blockDmg;
        public double blockChan;

        public Shield(GameState state)
            : base(state)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            level = state.hero.level;
            name = adjectives[rand.Next(adjectives.Length)] + " " + genName(level);

            switch (rand.Next(1))
            {
                case 0:
                    name += " Shield";
                    hp = rdnDouble(0.1 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level), 0.2 * state.hero.base_full_hp * Math.Pow(1.01, (double)state.hero.level));
                    blockDmg = Math.Pow(1.1, state.hero.level);                    
                    blockChan = rdnDouble(0.1 * Math.Pow(1.004, (double)state.hero.level), 0.4 * Math.Pow(1.004, (double)state.hero.level));
                    setImg();
                    break;
            }

            setDesc();
        }

        public Shield(GameState state, String name, int level, double hp, double blockDmg, double blockChan)
            : base(state)
        {
            this.name = name;
            this.level = level;
            this.hp = hp;
            this.blockDmg = blockDmg;
            this.blockChan = blockChan;
            setImg();
            setDesc();
        }

        private void setImg()
        {
            if (level == 1 || level == 2)
                this.img = Properties.Resources.shield_1;
            else if (level > 2 && level <= 5)
                this.img = Properties.Resources.shield_2;
            else if (level > 5 && level <= 10)
                this.img = Properties.Resources.shield_3;
            else if (level > 10 && level <= 15)
                this.img = Properties.Resources.shield_4;
            else if (level > 15 && level <= 20)
                this.img = Properties.Resources.shield_5;
            else if (level > 20 && level <= 25)
                this.img = Properties.Resources.shield_6;
            else if (level > 25 && level <= 30)
                this.img = Properties.Resources.shield_7;
            else if (level > 30 && level <= 35)
                this.img = Properties.Resources.shield_8;
            else if (level > 35 && level <= 40)
                this.img = Properties.Resources.shield_9;
            else if (level > 40 && level <= 45)
                this.img = Properties.Resources.shield_10;
            else if (level > 45 && level <= 50)
                this.img = Properties.Resources.shield_11;
            else if (level > 50 && level <= 55)
                this.img = Properties.Resources.shield_12;
            else if (level > 55 && level <= 60)
                this.img = Properties.Resources.shield_13;
            else if (level > 60 && level <= 65)
                this.img = Properties.Resources.shield_14;
            else if (level > 65 && level <= 70)
                this.img = Properties.Resources.shield_15;
            else if (level > 70)
                this.img = Properties.Resources.shield_16;
        }

        private void setDesc()
        {
            description = name
                + "\nLVL:  " + level
                + "\nHP : +" + Math.Round(hp,2)
                + "\nBLC DMG:  " + Math.Round(blockDmg,2)
                + "\nBLC CHN:  " + Math.Round(blockChan, 2);
        }
    }

    public class Weapon : Item
    {
        public double damage;
        public bool ranged;
        public double atk_speed;
        public double proj_speed;
        public int proj_range;
        public double powerSec;
        public double powerFac;
        public double critChan;
        public double lifestealChan;
        public AtkStyle style;
        public Bitmap projectileImg = null;

        public Weapon(GameState state)
            : base(state)
        {
            level = state.hero.level;
            name = adjectives[rand.Next(adjectives.Length)] + " " + genName(level);

            switch (rand.Next(2))
            {
                case 0:
                    name += " Wand";
                    damage = 1 + (int)((double)state.hero.level * rdnDouble(0.4, 0.7));
                    atk_speed = rdnDouble(0.5 * Math.Pow(0.99, (double)state.hero.level), 0.8 * Math.Pow(0.99, (double)state.hero.level));
                    proj_speed = rdnDouble(0.2 * Math.Pow(1.001, (double)state.hero.level), 0.8 * Math.Pow(1.001, (double)state.hero.level));
                    proj_range = (int)(rdnDouble(4 * Math.Pow(1.01, (double)state.hero.level), 6 * Math.Pow(1.01, (double)state.hero.level)));
                    style = (AtkStyle)rand.Next(0, 6);
                    powerSec = rdnDouble(0.5, 2.0);
                    powerFac = rdnDouble(0.3, 0.5);
                    ranged = true;
                    setProjImg();
                    break;
                case 1:
                    name += " Sword";
                    damage = 1 + (int)((double)state.hero.level * rdnDouble(0.7, 1.0));
                    atk_speed = rdnDouble(0.4 * Math.Pow(0.99, (double)state.hero.level), 0.6 * Math.Pow(0.99, (double)state.hero.level));
                    style = (AtkStyle)rand.Next(0, 6);
                    powerSec = rdnDouble(0.8, 2.3);
                    powerFac = rdnDouble(0.6, 0.8);
                    critChan = rdnDouble(0.1 * Math.Pow(1.004, (double)state.hero.level), 0.4 * Math.Pow(1.004, (double)state.hero.level));
                    lifestealChan = rdnDouble(0.1 * Math.Pow(1.004, (double)state.hero.level), 0.4 * Math.Pow(1.004, (double)state.hero.level));
                    ranged = false;
                    break;
            }

            setImg();
            setDesc();
        }

        public Weapon(GameState state, bool melee)
            : base(state)
        {
            level = state.hero.level;
            name = adjectives[rand.Next(adjectives.Length)] + " " + genName(level);

            if(!melee)
            {
                name += " Wand";
                damage = 1 + (int)((double)state.hero.level * rdnDouble(0.4, 0.7));
                atk_speed = rdnDouble(0.5 * Math.Pow(0.99, (double)state.hero.level), 0.8 * Math.Pow(0.99, (double)state.hero.level));
                proj_speed = rdnDouble(0.2 * Math.Pow(1.001, (double)state.hero.level), 0.8 * Math.Pow(1.001, (double)state.hero.level));
                proj_range = (int)(rdnDouble(4 * Math.Pow(1.01, (double)state.hero.level), 6 * Math.Pow(1.01, (double)state.hero.level)));
                style = (AtkStyle)rand.Next(0, 6);
                powerSec = rdnDouble(0.5, 2.0);
                powerFac = rdnDouble(0.3, 0.5);
                ranged = true;
                setProjImg();
            }
            else
            {
                name += " Sword";
                damage = 1 + (int)((double)state.hero.level * rdnDouble(0.7, 1.0));
                atk_speed = rdnDouble(0.4 * Math.Pow(0.99, (double)state.hero.level), 0.6 * Math.Pow(0.99, (double)state.hero.level));
                style = (AtkStyle)rand.Next(0, 6);
                powerSec = rdnDouble(0.8, 2.3);
                powerFac = rdnDouble(0.6, 0.8);
                critChan = rdnDouble(0.1 * Math.Pow(1.004, (double)state.hero.level), 0.4 * Math.Pow(1.004, (double)state.hero.level));
                lifestealChan = rdnDouble(0.1 * Math.Pow(1.004, (double)state.hero.level), 0.4 * Math.Pow(1.004, (double)state.hero.level));
                ranged = false;
            }

            setImg();
            setDesc();
        }

        public Weapon(GameState state, String name, int level, double damage, bool ranged, double atk_speed, double proj_speed, int proj_range, double powerSec, double powerFac, int style, double critChan, double lifestealChan)
            : base(state)
        {
            this.name = name;
            this.level = level;
            this.damage = damage;
            this.ranged = ranged;
            this.atk_speed = atk_speed;
            this.proj_speed = proj_speed;
            this.proj_range = proj_range;
            this.powerSec = powerSec;
            this.powerFac = powerFac;
            this.style = (AtkStyle)style;
            this.critChan = critChan;
            this.lifestealChan = lifestealChan;

            if (ranged)
                setProjImg();
            setImg();
            setDesc();
        }

        private void setDesc()
        {
            description = name
                + "\nLVL:  " + level
                + "\nSTY:  " + style.ToString()
                + "\nDMG: +" + Math.Round(damage, 2)
                + "\nATK SPD:  " + Math.Round(atk_speed, 2);

            if (ranged)
                description += "\nRNG:  " + proj_range;

            if (!ranged)
                description += "\nCRT CHN:  " + Math.Round(critChan,2)
                    + "\nLFS CHN:  " + Math.Round(lifestealChan,2);

            switch (style)
            {
                case AtkStyle.Frozen:
                    description += "\nSLW SEC:  " + Math.Round(powerSec, 2) + "\nSLW FAC:  " + Math.Round(powerFac, 2);
                    break;
                case AtkStyle.Flame:
                    description += "\nFLM SEC:  " + Math.Round(powerSec, 2) + "\nFLM FAC:  " + Math.Round(powerFac, 2);
                    break;
                case AtkStyle.Doom:
                    description += "\nSLW SEC:  " + Math.Round(powerSec, 2) + "\nSLW FAC:  " + Math.Round(powerFac, 2);
                    break;
                case AtkStyle.Lightening:
                    description += "\nFLM SEC:  " + Math.Round(powerSec, 2) + "\nFLM FAC:  " + Math.Round(powerFac, 2);
                    break;
                case AtkStyle.Poison:
                    description += "\nSLW SEC:  " + Math.Round(powerSec, 2) + "\nSLW FAC:  " + Math.Round(powerFac, 2);
                    break;
            }
        }

        private void setImg()
        {
            if (ranged)
            {
                if (level == 1 || level == 2)
                    this.img = Properties.Resources.wand_1;
                else if (level > 2 && level <= 5)
                    this.img = Properties.Resources.wand_2;
                else if (level > 5 && level <= 10)
                    this.img = Properties.Resources.wand_3;
                else if (level > 10 && level <= 15)
                    this.img = Properties.Resources.wand_4;
                else if (level > 15 && level <= 20)
                    this.img = Properties.Resources.wand_5;
                else if (level > 20 && level <= 25)
                    this.img = Properties.Resources.wand_6;
                else if (level > 25 && level <= 30)
                    this.img = Properties.Resources.wand_7;
                else if (level > 30 && level <= 35)
                    this.img = Properties.Resources.wand_8;
                else if (level > 35 && level <= 40)
                    this.img = Properties.Resources.wand_9;
                else if (level > 40 && level <= 45)
                    this.img = Properties.Resources.wand_10;
                else if (level > 45 && level <= 50)
                    this.img = Properties.Resources.wand_11;
                else if (level > 50 && level <= 55)
                    this.img = Properties.Resources.wand_12;
                else if (level > 55 && level <= 60)
                    this.img = Properties.Resources.wand_13;
                else if (level > 60 && level <= 65)
                    this.img = Properties.Resources.wand_14;
                else if (level > 65 && level <= 70)
                    this.img = Properties.Resources.wand_15;
                else if (level > 70)
                    this.img = Properties.Resources.wand_16;
            }
            else
            {
                if (level == 1 || level == 2)
                    this.img = Properties.Resources.sword_1;
                else if (level > 2 && level <= 5)
                    this.img = Properties.Resources.sword_2;
                else if (level > 5 && level <= 10)
                    this.img = Properties.Resources.sword_3;
                else if (level > 10 && level <= 15)
                    this.img = Properties.Resources.sword_4;
                else if (level > 15 && level <= 20)
                    this.img = Properties.Resources.sword_5;
                else if (level > 20 && level <= 25)
                    this.img = Properties.Resources.sword_6;
                else if (level > 25 && level <= 30)
                    this.img = Properties.Resources.sword_7;
                else if (level > 30 && level <= 35)
                    this.img = Properties.Resources.sword_8;
                else if (level > 35 && level <= 40)
                    this.img = Properties.Resources.sword_9;
                else if (level > 40 && level <= 45)
                    this.img = Properties.Resources.sword_10;
                else if (level > 45 && level <= 50)
                    this.img = Properties.Resources.sword_11;
                else if (level > 50 && level <= 55)
                    this.img = Properties.Resources.sword_12;
                else if (level > 55 && level <= 60)
                    this.img = Properties.Resources.sword_13;
                else if (level > 60 && level <= 65)
                    this.img = Properties.Resources.sword_14;
                else if (level > 65 && level <= 70)
                    this.img = Properties.Resources.sword_15;
                else if (level > 70)
                    this.img = Properties.Resources.sword_16;
            }
        }

        private void setProjImg()
        {
            switch (style)
            {
                case AtkStyle.Frozen:
                    projectileImg = Properties.Resources.ice;
                    break;
                case AtkStyle.Flame:
                    projectileImg = Properties.Resources.fire;
                    break;
                case AtkStyle.Poison:
                    projectileImg = Properties.Resources.poison;
                    break;
                case AtkStyle.Doom:
                    projectileImg = Properties.Resources.doom;
                    break;
                case AtkStyle.Lightening:
                    projectileImg = Properties.Resources.lightening;
                    break;

                default:
                    projectileImg = Properties.Resources.proj;
                    break;
            }
        }
    }

    public abstract class Consumable : Item
    {
        public Consumable(GameState state, Bitmap img, String name, String description)
            : base(state)
        {
            this.img = img;
            this.name = name;
            this.description = description;
        }

        public abstract void use();
    }

    public class SmallPotion : Consumable
    {
        private SoundPlayer sound = new SoundPlayer(Properties.Resources.potion);

        public SmallPotion(GameState state) : base(state, Properties.Resources.HP_Potion_s, "Small Potion", "Small Potion (+10 HP)") { }

        public override void use()
        {
            int hpBonus = 10;
            if (state.hero.hp + hpBonus < state.hero.full_hp)
                state.hero.hp += hpBonus;
            else
                state.hero.hp = state.hero.full_hp;

            if (Properties.Settings.Default.SoundEnabled)
                sound.Play();
        }
    }

    public class MediumPotion : Consumable
    {
        private SoundPlayer sound = new SoundPlayer(Properties.Resources.potion);

        public MediumPotion(GameState state) : base(state, Properties.Resources.HP_Potion_m, "Medium Potion", "Medium Potion (+30 HP)") { }

        public override void use()
        {
            int hpBonus = 30;
            if (state.hero.hp + hpBonus < state.hero.full_hp)
                state.hero.hp += hpBonus;
            else
                state.hero.hp = state.hero.full_hp;

            if (Properties.Settings.Default.SoundEnabled)
                sound.Play();
        }
    }

    public class LargePotion : Consumable
    {
        private SoundPlayer sound = new SoundPlayer(Properties.Resources.potion2);

        public LargePotion(GameState state) : base(state, Properties.Resources.HP_Postion_g, "Large Potion", "Large Potion (+50 HP)") { }

        public override void use()
        {
            int hpBonus = 50;
            if (state.hero.hp + hpBonus < state.hero.full_hp)
                state.hero.hp += hpBonus;
            else
                state.hero.hp = state.hero.full_hp;

            if (Properties.Settings.Default.SoundEnabled)
                sound.Play();
        }
    }

    public class Key : Item
    {
        public int quantity;

        public Key(GameState state)
            : base(state)
        {
            this.img = Properties.Resources.Key;
            quantity = 1;
        }

        public void increment()
        {
            quantity++;
            this.description = "Quantity: " + quantity;
        }

        public void decrement()
        {
            quantity--;
            this.description = "Quantity: " + quantity;
        }
    }
}