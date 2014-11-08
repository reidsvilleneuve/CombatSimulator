using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            combatSim();
        }

        /// <summary>
        /// A combat simulator where the player fights an assassin. All input and loops handled within the
        /// function.
        /// </summary>
        static void combatSim()
        {

            bool playAgain = true; //Will asks if player wants to play again. If so, all variables reset.
            
            do
            {
                // ---- System variables


                char input = ' '; // User input will be stored here.
                Random rng = new Random(); // Random event handler.

                //Game states:
                //-0: Start of the game: Player is sleeping and hears something.
                //-1: Start of the game: Player ignores assassin and takes very high damage.
                //-2: Start of the game: Player gets up to investigate sound and takes a thrown knife hit.
                //-3: Start of the game: Player pretends to sleep and blinds assassin with covers for 2 rounds.
                //-4: Start of the game: Assassin is blinded and player can look for items or go outside w/o punishment.
                //-5: Middle of the game: Player in active combat with assassin.
                //-6: Middle of the game: Player is looking for items.
                //-7: End of the game. Player has defeated the assassin, and must change locations to survive trap.
                int gameState = 0;
                int randomDamage = 0; //Used for non-weapon damage counters.
                bool playing = true; //Game is in progress. Set to false on win/loss.
                int counter = 0; //Used for misc. round counters.
                string enemyMessage = string.Empty; //Used to report enemy attacks/actions.
                string playerMessage = string.Empty; //Used to report player attacks/actions.


                // ---- Weapon variables


                // Weapon descriptions: Used to ease of passing into Weapon objects/
                string heavySwordDesc = "Grandfather's Heavy Sword: A giant sword that you haven't had pracice with, but deals heavy damage.";
                string knifeDesc = "Knife: A knife used for skinning. You can use it well, but it doesn't do much damage.";
                string fistDesc = "Fists: You were never trained to fight with your fists. While accurate with\nthem, you " +
                                  "dont punch very hard.";
                string bowDesc = "Bow: Your weapon of choice. Deals high damage and is fairly accurate.";

                // Weapon object parameters: name, minimum damage, maximum damage, miss chance, array of verbs used on hit.
                Weapon heavySword = new Weapon("heavy sword", 35, 45, 85, "destroy rip gore wound sever tear".Split(' '), heavySwordDesc);
                Weapon knife = new Weapon("knife", 5, 15, 5, "prick stick pierce puncture".Split(' '), knifeDesc);
                Weapon fist = new Weapon("fist", 1, 5, 5, "slap biff clip strike slug punch".Split(' '), fistDesc);
                Weapon bow = new Weapon("bow", 15, 30, 10, "shoot nail bullseye".Split(' '), bowDesc);

                bool fireUsed = false; //Will be set to True when Fire Powder is used.


                // ---- Player variables


                int playerHealth = 100;
                Weapon playerWeapon = new Weapon(fist); // Player starts with his or her fists.


                // ---- Location variables


                //Locations:
                //-0: Player's house. All weapons and special items available here.
                //-1: Woods. Bow available here.
                int location = 0;

                // These lists keep track of what item is where. Once a player picks up a weapon, fists are no longer usable.
                List<Weapon> houseHas = new List<Weapon> { heavySword, knife };
                List<Weapon> woodsHave = new List<Weapon> { bow };


                // ---- Enemy variables


                int enemyHealth = 100;
                int enemyMissChance = 15;

                //States:
                //-0: Melee combat. Uses sword and can be hit.
                //-1: Ranged combat. Uses throwing daggers and can not be hit, except with bow.
                //-2: Blinded. Can not attack and can be hit. Will not follow player outside.
                //    Blind is activated by fire powder.
                int enemyState = 0;
                int blindedTime = 0; // Blinded timer in rounds.
                bool canHeal = true;


                // ---- Main game loop.


                while (playing)
                {

                    Console.Clear(); //Clear console every loop.
                    input = ' '; //Reset any player input.
                    Console.WriteLine("\n\n");

                    //Game states:
                    //-0: Start of the game: Player is sleeping and hears something.
                    //-1: Start of the game: Player ignores assassin and takes very high damage.
                    //-2: Start of the game: Player gets up to investigate sound and takes a thrown knife hit.
                    //-3: Start of the game: Player pretends to sleep, and then blind, stand up, or continue to lie down.
                    //-4: Start of the game: Assassin is blinded and player can look for items or go outside w/o punishment.
                    //-5: Middle of the game: Player is in active combat with assassin.
                    //-6: Middle of the game: Player is looking for items.
                    //-7: End of the game. Player has defeated the assassin, and must change locations to survive trap.
                    switch (gameState)
                    {
                        case 0: //Start of the game: Player is sleeping and hears something.
                            Console.Write("The night, like every other before it, is very peaceful.\n\nOr at least: It was.\n\n" +
                                "The wind is blowing softly outside. You listen carefully, but are not exactly\nsure " +
                                "of what woke you up. Deer and bears are very common visitors during the\nnight. It wouldn't " +
                                "be the first time that their scurrying woke you up. Your\n");
                                
                            Console.ForegroundColor = ConsoleColor.Blue;

                            Console.Write("bow ");
                            
                            Console.ResetColor();
                        
                            Console.Write("is outside - you are quite " +
                                "good with it and, for a moment, you consider\nhunting the beast that woke you. Winter will " +
                                "be here soon, so the meat would\nbe rather welcome. It's still dark outside, though, and you are " +
                                "feeling groggy.\n\nWhat do you do?\n\n1. Go back to sleep. It's too late to hunt right now." +
                                "\n2. Get up. You could use the meat.\n3. Hold still in bed and wait for it to make another " +
                                "sound.\n\n(1, 2, 3?) ");

                            input = Console.ReadKey().KeyChar;
                            
                            switch(input)
                            {
                                case '1':
                                    gameState = 1; //From here on, this is how we progress to new game states.

                                    //Player has 1 more oppertunity to ignore the assassin if they hold still in bed. This
                                    //is how we differentiate between the two possibilities in game state 1.
                                    playerMessage = "You decide that it isn't worth the loss of rest. You have a busy day " +
                                        "ahead\nof you tomorrow. There is a lot of food to trap and gather before the weather\n" +
                                        "turns cold. Sleep starts to wrap you its silky tendrils.\n\nThis was a very bad idea.\n\n";
                                    break;

                                case '2':
                                    gameState = 2;
                                    break;

                                case '3':

                                    break;
                            }
                            
                            break;

                        case 1: //Start of the game: Player ignores assassin and takes very high damage.
                            Console.WriteLine(playerMessage);

                            randomDamage = heavySword.calcDamage(out playerMessage);
                            //Console.WriteLine(playerMessage);

                            input = Console.ReadKey().KeyChar;
                            break;

                        case 2: //Start of the game: Player gets up to investigate sound and takes a sword hit.

                            break;

                        case 3: //Start of the game: Player pretends to sleep and blinds assassin with covers for 2 rounds.

                            break;

                        case 4: //-4: Start of the game: Assassin is blinded and player can look for items or go outside w/o punishment.

                            break;

                        case 5: //-5: Middle of the game: Player is in active combat with assassin.

                            break;

                        case 6: //-6: Middle of the game: Player is looking for items.

                            break;

                        case 7: //End of the game. Player has defeated the assassin, and must change locations to survive trap.

                            break;
                    }

                    //Console.ReadKey();
                    //playing = false;
                } 

                Console.Clear();

                Console.WriteLine("\n\n\nPlay again? (Y/N) ");
                input = Console.ReadKey().KeyChar;
                input = char.ToLower(input);

                if (input == 'n')
                    playAgain = false;
                else if (input == 'y')
                {
                    playAgain = true;
                    playing = true;
                }
                else
                {
                    playing = true;
                    playing = false;
                }
            } while(playAgain == true);
        }


        /// <summary>
        /// Calculates damage for enemy's attack, and sets damageMessage to a proper message based on the
        /// weapon used. Damage will be selected randomly between minDamage and maxDamage. The enemy has
        /// a chance to miss.
        /// </summary>
        /// <param name="damageMessage">Sets given string to a properly formatted summary of the damage delt.</param>
        /// <param name="usingSword">Enemy will throw a dagger for less damage if set to False.</param>
        /// <param name="missChance">0-100% chance that the enemy will do 0 damage. Rounded back into this range.</param>
        /// <returns></returns>
        static int calcEnemyDamage(out string damageMessage, bool usingSword, int missChance)
        {
            Random rng = new Random();

            string[] swordVerbs = "cuts slashes swipes stabs".Split(' ');
            string[] daggerVerbs = "pierces stabs pokes ".Split(' ');

            //All of these values will be changed to Sword equivlants later if usingSword is true.
            string weapon = "thrown dagger"; 
            string verb = daggerVerbs[rng.Next(0, daggerVerbs.Length)];
            int damage = rng.Next(1, 11);
            int missValue = rng.Next(1, 101);

            
            if (usingSword)
            {
                weapon = "sword";
                verb = swordVerbs[rng.Next(0, swordVerbs.Length)];
                damage = rng.Next(10, 21);
            }

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.


            // ------ Combat text


            // The enemy hits.

            if (missValue > missChance) 
            {
                damageMessage = "The assassin " + verb + " you for " + damage + " with her " + weapon + "!";
                return damage;
            }
            
            //The enemy misses.

            damageMessage = "The assassin tries to hit you with her " + weapon + ", but misses!";
            return 0;
            
        }
    }

    /// <summary>
    /// Weapon class for CombatSimulator.
    /// </summary>
    public class Weapon
    {
        public string WeaponName { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int MissChance { get; set; }
        public string[] DamageVerbs { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// Stats of the weapon.
        /// </summary>
        /// <param name="weaponName">Name of the weapon. Will be converted to lowercase.</param>
        /// <param name="minDamage">Minimum damage of the weapon.</param>
        /// <param name="maxDamage">Maximum damage of the weapon.</param>
        /// <param name="missChance">0-100% chance that the weapon will miss. Values above and below this range
        /// are rounded back into it.</param>
        /// <param name="damageVerbs">Array of verbs that describe the kind of damage done, e.g. stab, slice, poke
        /// burn...</param>
        public Weapon(string weaponName, int minDamage, int maxDamage, int missChance, string[] damageVerbs, string description)
        {
            WeaponName = weaponName;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            MissChance = missChance;
            DamageVerbs = damageVerbs;
            Description = description;
        }

        /// <summary>
        /// Stats of the weapon.
        /// </summary>
        /// <param name="thisWeapon">Weapon to copy.</param>
        public Weapon(Weapon thisWeapon)
        {
            this.WeaponName = thisWeapon.WeaponName;
            this.MinDamage = thisWeapon.MinDamage;
            this.MaxDamage = thisWeapon.MaxDamage;
            this.MissChance = thisWeapon.MaxDamage;
            this.DamageVerbs = thisWeapon.DamageVerbs;
            this.Description = thisWeapon.Description;
        }

        /// <summary>
        /// Calculates damage for an attack, and sets damageMessage to a proper message based on the
        /// weapon used. Damage will be selected randomly between minDamage and maxDamage.
        /// </summary>
        /// <param name="weaponName">Name of the attack or weapon to be used.</param>
        /// <param name="minDamage">Minimum damage of the weapon or attack.</param>
        /// <param name="maxDamage">Maximum damage of the weapon or attack.</param>
        /// <param name="missChance">Chance to deal 0 damage from, 0 - 100 percent. Values under 0 or over 100 will be 
        /// rounded back to 0 or 100.</param>
        /// <param name="damageVerb">An array of strings of different kinds of verbs describing the attack performed.
        /// Ex. slashes, swipes, gores, cuts...</param>
        /// <param name="damageMessage">Sets given string to a properly formatted summary of the damage delt, including a
        /// given damageType adjectve.</param>
        /// <returns></returns>
        static int calcDamage(string weaponName, int minDamage, int maxDamage, int missChance, string[] damageVerb, out string damageMessage)
        {
            Random rng = new Random();
            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);
            string verb = damageVerb[rng.Next(0, damageVerb.Length)]; //Randomly select a damage verb

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > missChance)
                damage = rng.Next(minDamage, maxDamage + 1); //.Next's upper bound is exclusive, so +1 to correct that.


            // ---- Combat text.


            damageMessage = "You "; // Always displayed.

            if (damage == 0)
                damageMessage += "try to "; //Displayed on a miss


            damageMessage += verb + " the assassin with your ";

            Console.ForegroundColor = ConsoleColor.Blue;

            damageMessage += weaponName;

            Console.ResetColor();

            if (damage == 0) // Miss and hit messages.
                damageMessage += ", but you miss!";
            else
                damageMessage += "dealing " + damage + " damage!";
            return damage;
        }

        /// <summary>
        /// Calculates damage based on weapon stats, and returns the total value.
        /// </summary>
        /// <param name="damageMessage">String that will be returned to describe the attack itself.</param>
        /// <returns>Total calculated damage.</returns>
        public int calcDamage(out string damageMessage)
        {
            Random rng = new Random();
            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);
            string verb = DamageVerbs[rng.Next(0, DamageVerbs.Length)]; //Randomly select a damage verb

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > MissChance)
                damage = rng.Next(MinDamage, MaxDamage + 1); //.Next's upper bound is exclusive, so +1 to correct that.


            // ---- Combat text.


            damageMessage = "You "; // Always displayed.

            if (damage == 0)
                damageMessage += "try to "; //Displayed on a miss


            damageMessage += verb + " the assassin with your " + WeaponName + ", ";

            if (damage == 0) // Miss and hit messages.
                damageMessage += "but you miss!";
            else
                damageMessage += "dealing " + damage + " damage!";
            return damage;
        }

        /// <summary>
        /// Calculates damage based on weapon stats, and returns the total value.
        /// </summary>
        /// <returns>Total calculated damage.</returns>
        public int calcDamage()
        {
            Random rng = new Random();
            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > MissChance)
                damage = rng.Next(MinDamage, MaxDamage + 1); //.Next's upper bound is exclusive, so +1 to correct that.

            return damage;
        }


    }
}
