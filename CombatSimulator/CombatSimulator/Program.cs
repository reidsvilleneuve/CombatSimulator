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
            bool playing = true; //Game is in progress. Set to false on win/loss.
            
            do
            {
             
                
                // ---- System variables


                char input = ' '; //User input will be stored here.
                int intInput = 0; //Input chars converted to Ints when needed. (I.e. TryParse)
                Random rng = new Random(); //Random event handler.
                bool allowAttacks = true; //Will stop damage from being done in the case of incorrect input.

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
                int counter = 0; //Used for misc. round counters.
                string enemyMessage = string.Empty; //Used to report enemy attacks/actions.
                string playerMessage = string.Empty; //Used to report player attacks/actions.
                bool inputCorrect = false; //Used to loop input when needed.


                // ---- Weapon variables


                // Weapon descriptions: Used to ease of passing into Weapon objects/
                string heavySwordDesc = "Grandfather's Heavy Sword. A giant sword that you haven't had\npracice with, but deals heavy damage.";
                string knifeDesc = "Knife. A knife used for skinning. You can use it well, but it\ndoesn't do much damage.";
                string fistDesc = "Fists. You were never trained to fight with your fists. While\naccurate with them, you " +
                                  "dont punch very hard.";
                string bowDesc = "Bow and arrow. Your weapon of choice. Deals high damage and is\nsomewhat accurate.";
                string gunDesc = "Strange metallic tube with stuff on it. What is this device?"; //Cheat.

                string[] gunVerbs = { "straight shoot", "pop a cap in", "blast away" }; //Cheat.

                // Weapon object parameters: name, minimum damage, maximum damage, miss chance, array of verbs used on hit.
                Weapon heavySword = new Weapon("heavy sword", 35, 45, 70, "destroy rip gore wound sever tear".Split(' '), heavySwordDesc);
                Weapon knife = new Weapon("knife", 5, 20, 15, "prick stick pierce puncture".Split(' '), knifeDesc);
                Weapon fist = new Weapon("fist", 1, 5, 5, "slap biff clip strike slug punch".Split(' '), fistDesc);
                Weapon bow = new Weapon("bow and arrow", 15, 30, 20, "shoot nail bullseye gouge pin".Split(' '), bowDesc);
                Weapon gun = new Weapon("strange object", 100, 1000, 50, gunVerbs, gunDesc); //Cheat.

                bool fireUsed = false; //Will be set to True when Fire Powder is used.
                bool fireDropped = false; //Will be set to true upon first player attack or location change.


                // ---- Player variables


                int playerHealth = 100;
                Weapon playerWeapon = new Weapon(fist); // Player starts with his or her fists.


                // ---- Location variables


                //Locations:
                //-0: Player's house. All weapons and special items available here.
                //-1: Woods. Bow available here.
                int location = 0;

                // These lists keep track of what item is where. Once a player picks up a weapon, fists are no longer usable.
                List<Weapon> houseHas = new List<Weapon>() { heavySword, knife };
                List<Weapon> woodsHave = new List<Weapon>() { bow };
                List<Weapon> thisHas = new List<Weapon>(); //Set to player's curret location.
                thisHas = houseHas;


                // ---- Enemy variables


                int enemyHealth = 200;
                int enemyMissChance = 15;
                int blindedTime = 0; // Blinded timer in rounds.
                bool canHeal = true;


                // ---- Main game loop.


                while (playing)
                {

                    Console.Clear(); //Clear console every loop.
                    input = ' '; //Reset any player input.
                    Console.WriteLine();

                    //Game states:
                    //-0: Start of the game: Player is sleeping and hears something.
                    //-1: Start of the game: Player ignores assassin and takes very high damage.
                    //-2: Start of the game: Player gets up to investigate sound and takes a thrown knife hit.
                    //-3: Start of the game: Player stays in bed. Can blind assassin or wait and get hit.
                    //-4: Start of the game: Assassin is blinded and player can look for items or go outside w/o punishment.
                    //-5: Middle of the game: Player is in active combat with assassin.
                    //-6: Middle of the game: Player is looking for items.
                    //-7: End of the game. Player has defeated the assassin, and must change locations to survive trap.
                    switch (gameState)
                    {


                        //----Start of the game: Player is sleeping and hears something.


                        case 0:
                            Console.Write("The night, like every other before it, is very peaceful.\n\nOr at least: It was.\n\n" +
                                "The wind is blowing softly outside. You listen carefully, but are not exactly\nsure " +
                                "of what woke you up. Deer and bears are very common visitors during the\nnight. It wouldn't " +
                                "be the first time that their scurrying woke you. Your bow\nis outside - you are quite " +
                                "good with it and, for a moment, you consider\nhunting the beast that woke you. Winter will " +
                                "be here soon, so the meat would\nbe rather welcome. It's still dark outside, though, and you are " +
                                "feeling groggy.\n\nWhat do you do?\n\n1. Go back to sleep. It's too late to hunt right now." +
                                "\n2. Get up. You could use the meat.\n3. Hold still in bed and wait for it to make another " +
                                "sound.\n\n(1, 2, 3?) ");

                            input = Console.ReadKey().KeyChar;
                            
                            switch(input)
                            {
                                case '1': //----Go back to sleep
                                    //Player has 1 more oppertunity to ignore the assassin if they hold still in bed. This
                                    //is how we differentiate between the two possibilities in game state 1.
                                    playerMessage = "You decide that it isn't worth the loss of rest. You have a busy day " +
                                        "ahead\nof you tomorrow. There is a lot of food to trap and gather before the weather\n" +
                                        "turns cold. Sleep starts to wrap you its silky tendrils.\n\nThis was a very bad idea.\n\n" +
                                        "You awake with a massive pain in your chest. A feminine figure wearing black\nrobes stands " +
                                        "over you, her sword at the ready.\n";

                                    gameState = 1; //From here on, this is how we progress to new game states.
                                    break;

                                case '2': //----Get up to hunt.
                                    //As with holding still, there are multiple paths that lead to standing up.
                                    playerMessage = "You drag yourself out of bed. This won't be pleasent, but it's worth " +
                                        "it in the\nlong run.\n\nThat's when you see her.\n\nA feminine figure wearing black " +
                                        "robes crouches very still about 10 feet from\nyour bed. \"What is this? Who are you?\" "+
                                        "you nervously ask. She makes a sudden\nmovement.";
                                    gameState = 2;
                                    break;

                                case '3': //----Hold still in bed.
                                    gameState = 3;
                                    break;
                            }
                            
                            break;


                        //----Pre-battle: Player ignores assassin and takes very high damage.


                        case 1:
                            Console.WriteLine(playerMessage);

                            randomDamage = rng.Next(30, 41);
                            Console.Write("The assassin attacks you while you are in bed. You take " + randomDamage +
                                " damage.\n\nDefend yourself!\n\n(Press any key to continue.)");

                            playerHealth -= randomDamage;

                            Console.ReadKey();
                            gameState = 5;
                            break;


                        //----Pre-battle: Player gets up to investigate sound and takes a knife hit.


                        case 2:

                            Console.WriteLine(playerMessage + "\n\nThe assassin quickly pulls a dagger out of her sleeve.");

                            playerHealth -= calcEnemyDamage(out enemyMessage, false, 0);

                            Console.Write("\n" + enemyMessage + "\n\nDefend yourself!\n\n(Press any key to continue.)");

                            Console.ReadKey();
                            gameState = 5;
                            break;


                        //----Pre-battle: Player stays in bed. Can blind assassin or wait and get hit.


                        case 3: 

                            Console.Write("You stay in bed. You see a shadowy feminine figure approaching through\nyour " +
                                "covers.\n\nWhat do you do?\n\n1. Stand up and tell the crook to leave.\n2. Keep still. " +
                                "Hopefully you won't be noticed.\n\n(1, 2?) ");

                                input = Console.ReadKey().KeyChar;

                                switch (input)
                                {
                                    case '1': //----Stand up
                                        playerMessage = "You arn't going to let this intuder get the jump on you. You stand up.";
                                        
                                        gameState = 2;
                                        break;

                                    case '2': //----Keep still
                                        do
                                        {
                                            Console.Clear();
                                            Console.Write("\nYou wait to see what the intruder does. You see her move " +
                                                "closer.\n\nWhat do you do?\n\n1. Keep waitin");
                                            Console.ForegroundColor = ConsoleColor.White;

                                            Console.Write("g");

                                            Console.ResetColor();
                                            Console.Write(". She may not see you.\n2. " +
                                                "Throw the covers on her.\n\n(1, 2?)");

                                            input = Console.ReadKey().KeyChar;

                                            switch(input)
                                            {
                                                case '1': //----Player waits
                                                    playerMessage = "You can't risk a confrontation. You wait. The assassin " +
                                                        "suddenly jumps forward,\ndrawing a sword.";

                                                    if (houseHas.Contains(gun))
                                                        playerMessage += " As she leaps, you see a strange metal device falls out of her\nrobes!";
                                                    
                                                    gameState = 1;
                                                    break;

                                                case '2': //----Player throws cover on assassin.
                                                    gameState = 4;
                                                    break;

                                                case 'G': //----Cheat code. Assassin drops a gun. See gamestate 4.
                                                    if(!houseHas.Contains(gun)) houseHas.Add(gun);
                                                    break;
                                            }
                                        } while (input != '1' && input != '2'); // Loops until invalid input is recieved.
                                        break;
                                } 
                            
                            break;


                        //----Pre-battle: Assassin is blinded and player can look for items or go outside w/o punishment.

                        case 4:

                            blindedTime = 2;

                            Console.Write("You throw your covers on the intruder. She rips at the covers with her sword. ");
                            if(houseHas.Contains(gun)) Console.Write("\nA strange looking object flies out of her robes as she struggles.");
                            Console.Write("\nYou have a precious free second to act.\n\nDefend yourself!\n\n" +
                            "(Press any key to continue.) ");
                            Console.ReadKey();

                            gameState = 5;
                            break;


                        //----Mid-game: Player is in active combat with assassin.


                        case 5:

                            if (blindedTime > 0) blindedTime--; //Decrement enemy blinded timer.
                            if (playerHealth < 1)
                            {
                                playerHealth = 1; //One last fighting chance - this happens on a location change or look-around.
                                Console.WriteLine("You stumble back. That can't kill you, but this isn't looking good!\n");
                            }

                            switch(location)
                            {
                                case 0: //In the cabin
                                    Console.WriteLine("You are in the cabin.\n");
                                    break;

                                case 1: //Outside:
                                    Console.WriteLine("You are in the woods.\n");
                                    break;
                            }

                            Console.WriteLine("Your weapon: " + playerWeapon.Description + "\n\nYour health: " +
                                playerHealth + "\nEnemy health: " + enemyHealth + "\n\n----------------------\n");

                            if (blindedTime == 0) //Enemy blind/ready messages.
                                Console.WriteLine("The assassin studies you, awaiting your next move.");
                            else
                                Console.WriteLine("The assassin is blinded! Now's your chance!");

                            Console.Write("\n1. Attack with your " + playerWeapon.WeaponName + "." +
                                "\n2. Look around for something to help.\n3. Run ");
                            
                            //Changes depending on location.
                            if(location == 0) Console.Write("outside.");
                            else Console.Write("inside.");
                            
                            Console.Write("\n\n(1, 2, 3?)");

                            input = Console.ReadKey().KeyChar;

                            switch(input)
                            {
                                case '1': //Attack with current weapon.
                                    Console.Clear();
                                    Console.WriteLine();

                                    if (fireDropped == false) // On first location change.
                                    {
                                        Console.WriteLine("As you charge toward the assassin, you notice a strange vial of "
                                        + "glowing orange\nlight fall out of her robes as she reacts!\n");
                                        fireDropped = true;
                                    }
                                    
                                    
                                    if(blindedTime > 0)
                                    {
                                        //Assassin almost always misses while blinded.
                                        playerHealth -= calcEnemyDamage(out enemyMessage, true, 100);
                                        enemyHealth -= playerWeapon.calcDamage(out playerMessage, 0);
                                        playerMessage += "\nThe assassin can't dodge while blinded!";
                                    }
                                    else if (blindedTime == 0 && (playerWeapon == bow || playerWeapon == gun))
                                    {
                                        //Player can hit enemy with bow even if not blinded.
                                        playerHealth -= calcEnemyDamage(out enemyMessage, true, enemyMissChance);
                                        enemyHealth -= playerWeapon.calcDamage(out playerMessage);
                                    }
                                    else
                                    {
                                        playerMessage = "You try to hit the assassin with your " + playerWeapon.WeaponName +
                                            ", but she is fast!\nShe jumps back and throws a knife at you!";
                                        playerHealth -= calcEnemyDamage(out enemyMessage, false, enemyMissChance);
                                    }
                                    Console.WriteLine(playerMessage + "\n\n" + enemyMessage + "\n\n(Press any key to continue.)");
                                    Console.ReadKey();

                                    if(playerHealth < 1 || enemyHealth < 1)
                                        gameState = 7; //Either player or enemy died - go to End Of Game.

                                    break;


                                case '2': //Look for something to help.
                                    gameState = 6;
                                    break;

                                case '3': //Change location.

                                    Console.Clear();

                                    if (blindedTime == 0)
                                    {
                                        Console.WriteLine("\nYou run for the doorway. She grabs a knife!");
                                        //This is a harder throw, so add 20 onto miss chance.
                                        playerHealth -= calcEnemyDamage(out enemyMessage, false, enemyMissChance + 20);
                                        Console.WriteLine(enemyMessage);
                                    }

                                    if (location == 0)
                                    {
                                        if (fireDropped == false) // On first location change.
                                        {
                                            Console.WriteLine("\nAs you dodge past the assassin, you notice a strange vial of "
                                            + "glowing orange\nlight fall out of her robes as she makes a move toward you!");
                                            fireDropped = true;
                                        }

                                        location = 1;

                                        //Message changes depending on assassin's blinded state.

                                        Console.Write("\nYou run though the door into the woods. The assassin ");

                                        if (blindedTime > 0)
                                            Console.Write("stumbles through behind\nyou!");
                                        else
                                            Console.Write("follows closely!");

                                        Console.Write("\n\n(Press any key to continue.)");

                                        Console.ReadKey();

                                        thisHas = woodsHave; //Update current location's item list.
                                    }
                                    else
                                    {
                                        location = 0;

                                        //Message changes based on assassin's blinded state.
                                        Console.Write("\nYou run though the door into your house. The assassin ");

                                        if (blindedTime > 0)
                                            Console.WriteLine("stumbles through behind you!");
                                        else
                                            Console.WriteLine("follows closely!\n\n(Press any key to continue.)");

                                        Console.ReadKey();

                                        
                                        thisHas = houseHas; //Update current location's item list.
                                    }
                                    break;

                                default: //Something else pushed. allowAttacks set to false stops attacks.
                                    allowAttacks = false;
                                    break;
                            }
                            break;


                        //----Mid-game: Player is looking for items.


                        case 6: 

                            if (thisHas.Count > 0)
                            {

                                inputCorrect = false; //Will be set to true when correct input is recieved.

                                do //Loops until valid input is entered.
                                {

                                    if (blindedTime == 0)
                                    {
                                        Console.WriteLine("She sees your momentary distraction and grabs a knife!");
                                        playerHealth -= calcEnemyDamage(out enemyMessage, false, enemyMissChance);
                                        Console.WriteLine(enemyMessage + "\n");
                                    }

                                    switch (location)
                                    {
                                        case 0: //In the cabin
                                            Console.WriteLine("You scan around your belongings for something that can help you!\n");
                                            break;
                                        case 1: //Outside
                                            Console.WriteLine("You try to find something that you can use in the snow!\n");
                                            break;
                                    }

                                    //This list changes based on where the player is/what has been picked up, so it must
                                    //be procedurally generated.

                                    //    List generation

                                    //Generate list of objects in player's current location.
                                    for (int i = 0; i < thisHas.Count; i++)
                                        Console.WriteLine((i + 1) + ": " + thisHas[i].Description);

                                    //Player decides to not switch weapons.
                                    Console.WriteLine((thisHas.Count + 1) + ": Turn your attention back to the assassin.");
                                
                                    //Player uses the fire powder if they are inside AND the assassin dropped the fire powder.
                                    if (location == 0 && fireUsed == false && fireDropped == true)
                                        Console.WriteLine((thisHas.Count + 2) + ": Use the glowing bottle that the Assassin dropped.");

                                    //    User choices in parenthesis generation
                                
                                    Console.Write("\n(");

                                    for (int i = 0; i < thisHas.Count; i++) //One for every item.
                                    {
                                        Console.Write((i + 1) + ", ");
                                    }
                                
                                    Console.Write((thisHas.Count + 1)); //One for turning back to the assassin.
                                                                        //Comma ommited in case fire powder isn't available.

                                    if (location == 0 && fireUsed == false && fireDropped == true) //One for fire powder. End of list.
                                        Console.Write(", " + (thisHas.Count + 2) + "?) ");
                                    else
                                        Console.Write("?) "); //No fire powder. End of list.

                                    input = Console.ReadKey().KeyChar; //String is required for TryParse.

                                    //    User input processing

                                    Console.Clear();
                                    Console.WriteLine();

                                    //Ifs are used here instead of Switch because more logic is required.

                                    //TryParse only accepts a string, so convert input to String for the following...
                                    if (int.TryParse(input.ToString(), out intInput)) //If the user inputs a valid number...
                                    {
                                        if (intInput <= thisHas.Count) //User selected something in the item list.
                                        {
                                            if (!(playerWeapon.WeaponName == "fist")) //If the current weapon is NOT fist...
                                            {
                                                thisHas.Add(playerWeapon); //...add current weapon to this location's weapon list.

                                                //Display this message after input processing...
                                                playerMessage = "You drop the " + playerWeapon.WeaponName +
                                                    " and pick up the " + thisHas[intInput - 1].WeaponName + "!";
                                            }
                                            else //Fists used. Add nothing to loctaion and set up message.
                                                playerMessage = "You pick up the " + thisHas[intInput - 1].WeaponName + "!";

                                            //Give player selected weapon and remove it from old location.
                                            playerWeapon = thisHas[intInput - 1]; //Give player selected weapon.
                                            thisHas.RemoveAt(intInput - 1); //Remove selected weapon from weapon list.

                                            gameState = 5; //Back to fighting game state.
                                            inputCorrect = true;
                                        }
                                        else //Something outside the item list selected
                                        {
                                            if (intInput == thisHas.Count + 1) // Attention back to assassin.
                                            {
                                                gameState = 5; //Back to fighting game state.
                                                playerMessage = "Nothing looks useful right now. You turn your attention back to the fight.";
                                                
                                                inputCorrect = true;
                                            }

                                            // If inside, AND firepowder not used, AND it dropped, AND they select it's option...
                                            else if (location == 0 && fireUsed == false && fireDropped == true 
                                            && intInput == thisHas.Count + 2)
                                            {
                                                randomDamage = rng.Next(20, 36);
                                                blindedTime = 5; //This will be 4 rounds - subtracted during loop.
                                                enemyHealth -= randomDamage;
                                                fireUsed = true;

                                                gameState = 5; //Back to fighting game state.
                                                playerMessage = "You pick up the mysterious bottle and throw it at the assassin! " +
                                                    "The room lights\nup in a flash of orange. The assassin takes " + randomDamage +
                                                    " damage. She thrashes her sword\naround madly. She has been blinded!";

                                                inputCorrect = true;
                                            }
                                        }
                                    }

                                    if(inputCorrect) //They inputted correctly. Update location lists, display message and pause.
                                    {
                                        if (location == 0) houseHas = thisHas;
                                        else woodsHave = thisHas;

                                        Console.Clear();
                                        Console.Write("\n" + playerMessage + "\n\n(Press any key to continue.) ");
                                        Console.ReadKey();
                                    }
                                } while (inputCorrect == false);
                            }
                            else // There are no weapons in this area's location. 
                            {
                                gameState = 5; //Back to fighting game mode.
                                Console.Clear();
                                Console.Write("\nThere is nothing of use here. Get back to the fight!\n\n" +
                                    "(Press any key to continue.)");
                                Console.ReadKey();
                            }

                            break;


                        //----End of the game: Player has defeated/died to the assassin.


                        case 7:

                            if(playerHealth > 0) // Player won.
                            {
                                Console.WriteLine("You fall to your knees and look at the fallen foe before you. Blood trickles\n" +
                                    "down both of your figures. It takes a moment for you to focus your eyes.\n\nA swirl of " +
                                    "light starts to rise from her body, slowly streaming out the window.\nIs this magic?\n\n" +
                                    "You notice a gold-woven insignia on her cape. It seems to be a godly hand\nweilding a warhammer " +
                                    "emitting from the clouds. Her sword is very ornate,\nlined with jewels and gold.\n\n" +
                                    "Who is this person? You're but a lonely hunter/gatherer - why would anyone\nwant to " +
                                    "assassinate you?\n\nYou stand up and start to clean the debris.");

                                playing = false;
                            }
                            else //Player lost. Tie goes to the enemy.
                            {
                                Console.WriteLine("You fall face-down into the ground. Or is it snow? Nothing is making " +
                                    "any sense.\nThe world is darkening around you. You hear footsteps approach. They " +
                                    "stop,\nand after a few seconds, your hear-\n\n\n...");

                                playing = false;
                            }

                            break;
                    }


                } 


                Console.Write("\n\n\nPlay again? (Y/N) ");

                do
                {
                    input = char.ToLower(Console.ReadKey(true).KeyChar); //"True" stops player's chose from displying in the console.

                    if (input == 'n')
                        playAgain = false; // Stop main loop; end function.
                    else if (input == 'y')
                        playing = true; //Main loop continues, game loop resets.

                } while(!(input == 'y' || input == 'n'));
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
            string[] daggerVerbs = "pierces stabs pokes nails targets".Split(' ');

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
            this.MissChance = thisWeapon.MissChance;
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


            damageMessage += verb + " the assassin with your " + weaponName;

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
            string verb = DamageVerbs[rng.Next(0, this.DamageVerbs.Length)]; //Randomly select a damage verb

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > this.MissChance)
                damage = rng.Next(this.MinDamage, this.MaxDamage + 1); //.Next's upper bound is exclusive, so +1 to correct that.


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
        /// <param name="damageMessage">String that will be returned to describe the attack itself.</param>
        /// <param name="missChanceOverride">Miss chance.</param>
        /// <returns>Total calculated damage.</returns>
        public int calcDamage(out string damageMessage, int missChanceOverride)
        {
            Random rng = new Random();
            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);
            string verb = DamageVerbs[rng.Next(0, this.DamageVerbs.Length)]; //Randomly select a damage verb

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > missChanceOverride)
                damage = rng.Next(this.MinDamage, this.MaxDamage + 1); //.Next's upper bound is exclusive, so +1 to correct that.


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
