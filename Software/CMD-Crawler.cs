using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace GameDev
{
    /**
     * The main class of the Dungeon Game Application
     * 
     * You may add to your project other classes which are referenced.
     * Complete the templated methods and fill in your code where it says "Your code here".
     * Do not rename methods or variables which already eDist or change the method parameters.
     * You can do some checks if your project still aligns with the spec by running the tests in UnitTest1
     * 
     * For Questions do contact us!
     */
    public class Game
    {
        // -- general variables --
        int tempX, tempY; // temporary co-ordinate values to hold
        int coinCount = 0; int monstersQuantity = 0; int monsterCoinCount = 0; // initialises variables at 0
        int monsterHealth = 3; int playerHealth = 2; // initialises player and monster health
        char lastTile = '.'; // used to store the last tile the player walked over
        bool advanceChoosen; bool replayChosen = true; // states whether player should be able to make decision on advance & replay
        bool walkIntoMonster = false;
        string advancedChoice;
        /**
         * use the following to store and control the movement 
         */
        public enum PlayerActions { NOTHING, NORTH, EAST, SOUTH, WEST, PICKUP, ATTACK, DROP, QUIT }; // an enum to hold all actions
        private PlayerActions action = PlayerActions.NOTHING; // action which sets to each key press
        public enum GameState { UNKOWN, STOP, RUN, START, INIT };
        private GameState status = GameState.INIT;

        // jagged character arrays to load maps
        private char[][] originalMap = new char[0][];
        private char[][] workingMap = new char[0][];

        /**
        * tracks if the game is running
        */
        private bool advanced = false;

        private string currentMap;

        /**
         * Reads user input from the Console
         * 
         * Please use and implement this method to read the user input.
         * 
         * Return the input as string to be further processed
         * 
         */
        private string ReadUserInput()
        {
            string inputRead = string.Empty;
            if (status == GameState.INIT) // asks which map for selection
            {
                Console.WriteLine("Which map do you want to play?");
                Console.WriteLine("Please enter one of the following: \nload Simple.map \nload Simple2.map \nload Advanced.map");
                inputRead = Console.ReadLine().Trim();
                return inputRead;
            }

            else if (status == GameState.START) // if map is selected, game has started and user has to type 'start' or 'play' to load map and print
            {
                Console.WriteLine("Please type 'start' or 'play' to initiate the game:");
                inputRead = Console.ReadLine().Trim();
                while ((inputRead != "start") && (inputRead != "play"))
                {
                    Console.WriteLine("You have not input either of the words: start or play");
                    inputRead = Console.ReadLine();
                }
                return inputRead;
            }

            else if (status == GameState.RUN && advanceChoosen == false) // the player is asked if they want to enable advanced mode
            {
                Console.WriteLine("Type 'advanced' to initiate advanced mode, 'no' if not");
                advancedChoice = Console.ReadLine().Trim();
                while (advancedChoice != "advanced" && advancedChoice != "no" && advancedChoice != "No")
                {
                    advanced = false;
                    advancedChoice = Console.ReadLine();
                }
                if (advancedChoice == "advanced") 
                {   advanced = true;
                }
                advanceChoosen = true;
                return advancedChoice;
            }

            else if (replayChosen == false) // the player has won and hence asked to replay only in advanced mode
            {
                status = GameState.STOP;
                Console.WriteLine("YOU FOUND THE EXIT AND FLEED!");
                if (advanced == true) 
                {
                    Console.WriteLine("\n Do you want to replay? Type 'Replay' or 'No'");
                }
                inputRead = Console.ReadLine().Trim();
                return inputRead;
            }

            else if (status == GameState.RUN) // used to identify when the input is for key movement (after map selection)
            {
                inputRead = Console.ReadKey().KeyChar.ToString();
                return inputRead;
            }

            else // if user input is nor map selection or key movement input
            {
                Console.WriteLine("Cannot identify input");
                return inputRead;
            }
        }

        private int counter = -1;
        /// <summary>
        /// Returns the number of steps a player made on the current map. The counter only counts steps, not actions.
        /// </summary>
        public int GetStepCounter()
        {
            return counter;
        }

        public int CoinCounter()
        {
            return coinCount;
        }

        private void RestartTheGame() // if the user chooses to replay, console is cleared and all variables reset and GameState enum reset to INIT
        {
            status = GameState.INIT;
            tempX = 0;
            tempY = 0;
            coinCount = 0;
            lastTile = '.';
            replayChosen = true;
            monstersQuantity = 0;
            monsterHealth = 3;
            playerHealth = 2;
            advanceChoosen = false;
            walkIntoMonster = false;
            monsterCoinCount = 0;
        }

        /**
         * Processed the user input string
         * 
         * takes apart the user input and does control the information flow
         *  * initializes the map ( you must call InitializeMap)
         *  * starts the game when user types in Play
         *  * sets the correct playeraction which you will use in the Update
         *  
         *  DO NOT read any information from command line in here but only act upon what the method receives.
         */
        public void ProcessUserInput(string input)
        {
            input = input.ToUpper(); // to help with any case-sensitive input

            switch (input) // for map loading
            {
                default: action = PlayerActions.NOTHING; break;
                case "LOAD SIMPLE.MAP": LoadMapFromFile("Simple.map"); break;
                case "LOAD SIMPLE2.MAP": LoadMapFromFile("Simple2.map"); break;
                case "LOAD ADVANCED.MAP": LoadMapFromFile("Advanced.map"); break;
            }

            if (status == GameState.START)
            {
                switch (input) // game is deemed running if player types 'play' or 'start'
                {
                    default: action = PlayerActions.NOTHING; break;
                    case "PLAY":
                    case "START": status = GameState.RUN; counter = 0; break;
                    case "QUIT": status = GameState.STOP; break;
                }
            }

            if (advanceChoosen == true) // processes whether player wants to enable advanced mode or not
            {
                switch (input)
                {
                    default: break;
                    case "YES": advanced = true; break;
                    case "NO": advanced = false; break;
                }
            }

            if (status == GameState.RUN) // interprets player key movement
            {
                switch (input)
                {
                    default: action = PlayerActions.NOTHING; break;
                    case "W": action = PlayerActions.NORTH; counter++; break;
                    case "A": action = PlayerActions.WEST; counter++; break;
                    case "S": action = PlayerActions.SOUTH; counter++; break;
                    case "D": action = PlayerActions.EAST; counter++; break;
                    case "Q": action = PlayerActions.ATTACK; break;
                    case "Z": action = PlayerActions.PICKUP; break;
                }
            }

            if (replayChosen == false) // player has won, processes whether player wants to replay or quit
            {
                switch (input)
                {
                    default: action = PlayerActions.NOTHING; break;
                    case "REPLAY": replayChosen = true; RestartTheGame(); break; 
                    case "NO": QuitGame(); PrintMapToConsole(); break; // game quits and console is wiped
                }
            }

        }

        /**
         * The Main Game Loop. 
         * It updates the game state.
         * 
         * This is the method where you implement your game logic and alter the state of the map/game
         * use playeraction to determine how the character should move/act
         * the input should tell the loop if the game is active and the state should advance
         * 
         * Returns true if the game could be updated and is ongoing
         */
        public bool Update(GameState status)
        {
            if (status == GameState.RUN)
            {
                for (int y = 0; y < workingMap.Length; y++) // checks to see if the letter P is present and replaces it with character model @
                {
                    for (int x = 0; x < workingMap[y].Length; x++)
                    {
                        if (workingMap[y][x] == 'P') 
                        {
                            workingMap[y][x] = '@';
                        }
                    }
                }

                int[] playerPosition = GetPlayerPosition(); // stores player position temporarily
                int action = GetPlayerAction(); // processes the key input

                if (lastTile == 'C') // if the player is standing on a coin, lastTile = coin thus input interpreted as pickup or movement
                {
                    if (action == (int)PlayerActions.PICKUP)
                    { 
                        coinCount++;
                        playerHealth++; // player health increments when player picks up coin
                        lastTile = '.';
                    }
                } // if playerAction is not pick up coin, then it is interpreted as player movement

                try
                {
                    switch (action)
                    {
                        case (int)PlayerActions.NORTH: // W is pressed

                            if (workingMap[playerPosition[0] - 1][playerPosition[1]] == '.') // checks if north is empty space
                            {
                                if (lastTile != 'C')
                                {
                                    workingMap[playerPosition[0]][playerPosition[1]] = '.'; // moves north and replaces both values
                                    workingMap[playerPosition[0] - 1][playerPosition[1]] = '@';
                                }
                                else
                                {
                                    workingMap[playerPosition[0] - 1][playerPosition[1]] = '@'; // if player currently standing on coin but didn't pick up, it rewrites previous step
                                    workingMap[tempY][tempX] = 'C';
                                    lastTile = '.';
                                }

                            }
                            else if ((workingMap[playerPosition[0] - 1][playerPosition[1]] == 'D')) // if north is door, the game gives option to replay or quit
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0] - 1][playerPosition[1]] = '@';
                                Console.WriteLine("You have completed the game. Well done!");
                                replayChosen = false;
                            }
                            else if (workingMap[playerPosition[0] - 1][playerPosition[1]] == 'C') // if north is coin, player stands on it and lastTile is C which gives option to pickup next iteration
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0] - 1][playerPosition[1]] = '@';
                                lastTile = 'C';
                                for (int y = 0; y < workingMap.Length; y++) // moves north and find co-ordinates of @, so where C originally was
                                {
                                    for (int x = 0; x < workingMap[y].Length; x++)
                                    {
                                        if (workingMap[y][x] == '@')
                                        {
                                            tempX = x;
                                            tempY = y;
                                        }
                                    }
                                }
                                Console.WriteLine("Press Z to pick up!");
                            }
                            else if (workingMap[playerPosition[0] - 1][playerPosition[1]] == 'M')
                            {
                                walkIntoMonster = true;
                            }
                            else if (workingMap[playerPosition[0] - 1][playerPosition[1]] == '#')
                            { Console.WriteLine("Cannot move there"); }
                            break;


                        case (int)PlayerActions.EAST: // D is pressed
                            if (workingMap[playerPosition[0]][playerPosition[1] + 1] == '.') 
                            {
                                if (lastTile != 'C')
                                {
                                    workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                    workingMap[playerPosition[0]][playerPosition[1] + 1] = '@';
                                }
                                else
                                {
                                    workingMap[playerPosition[0]][playerPosition[1] + 1] = '@';
                                    workingMap[tempY][tempX] = 'C';
                                    lastTile = '.';
                                }

                            }
                            else if ((workingMap[playerPosition[0]][playerPosition[1] + 1] == 'D'))
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0]][playerPosition[1] + 1] = '@';
                                Console.WriteLine("You have completed the game. Well done!");
                                replayChosen = false;
                            }
                            else if (workingMap[playerPosition[0]][playerPosition[1] + 1] == 'C')
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0]][playerPosition[1] + 1] = '@';
                                lastTile = 'C';
                                for (int y = 0; y < workingMap.Length; y++) 
                                {
                                    for (int x = 0; x < workingMap[y].Length; x++)
                                    {
                                        if (workingMap[y][x] == '@')
                                        {
                                            tempX = x;
                                            tempY = y;
                                        }
                                    }
                                }
                                Console.WriteLine("Press Z to pick up!");
                            }
                            else if (workingMap[playerPosition[0]][playerPosition[1] + 1] == 'M')
                            {
                                walkIntoMonster = true;
                            }
                            else if (workingMap[playerPosition[0]][playerPosition[1] + 1] == '#')
                            { Console.WriteLine("Cannot move there"); }
                            break;


                        case (int)PlayerActions.SOUTH: // S is pressed
                            if (workingMap[playerPosition[0] + 1][playerPosition[1]] == '.') 
                            {
                                if (lastTile != 'C')
                                {
                                    workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                    workingMap[playerPosition[0] + 1][playerPosition[1]] = '@';
                                }
                                else
                                {
                                    workingMap[playerPosition[0] + 1][playerPosition[1]] = '@';
                                    workingMap[tempY][tempX] = 'C';
                                    lastTile = '.';
                                }

                            }
                            else if ((workingMap[playerPosition[0] + 1][playerPosition[1]] == 'D'))
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0] + 1][playerPosition[1]] = '@';
                                Console.WriteLine("You have completed the game. Well done!");
                                replayChosen = false;
                            }
                            else if (workingMap[playerPosition[0] + 1][playerPosition[1]] == 'C')
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0] + 1][playerPosition[1]] = '@';
                                lastTile = 'C';
                                for (int y = 0; y < workingMap.Length; y++) 
                                {
                                    for (int x = 0; x < workingMap[y].Length; x++)
                                    {
                                        if (workingMap[y][x] == '@')
                                        {
                                            tempX = x;
                                            tempY = y;
                                        }
                                    }
                                }
                                Console.WriteLine("Press Z to pick up!");
                            }
                            else if (workingMap[playerPosition[0] + 1][playerPosition[1]] == 'M')
                            {
                                walkIntoMonster = true;
                            }
                            else if (workingMap[playerPosition[0] + 1][playerPosition[1]] == '#')
                            { Console.WriteLine("Cannot move there"); }
                            break;


                        case (int)PlayerActions.WEST: // A is pressed
                            if (workingMap[playerPosition[0]][playerPosition[1] - 1] == '.') 
                            {
                                if (lastTile != 'C')
                                {
                                    workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                    workingMap[playerPosition[0]][playerPosition[1] - 1] = '@';
                                }
                                else
                                {
                                    workingMap[playerPosition[0]][playerPosition[1] - 1] = '@';
                                    workingMap[tempY][tempX] = 'C';
                                    lastTile = '.';
                                }

                            }
                            else if ((workingMap[playerPosition[0]][playerPosition[1] - 1] == 'D'))
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0]][playerPosition[1] - 1] = '@';
                                Console.WriteLine("You have completed the game. Well done!");
                                replayChosen = false;
                            }
                            else if (workingMap[playerPosition[0]][playerPosition[1] - 1] == 'C')
                            {
                                workingMap[playerPosition[0]][playerPosition[1]] = '.';
                                workingMap[playerPosition[0]][playerPosition[1] - 1] = '@';
                                lastTile = 'C';
                                for (int y = 0; y < workingMap.Length; y++) 
                                {
                                    for (int x = 0; x < workingMap[y].Length; x++)
                                    {
                                        if (workingMap[y][x] == '@')
                                        {
                                            tempX = x;
                                            tempY = y;
                                        }
                                    }
                                }
                                Console.WriteLine("Press Z to pick up!");
                            }
                            else if (workingMap[playerPosition[0]][playerPosition[1] - 1] == 'M')
                            {
                                walkIntoMonster = true;
                            }
                            else if (workingMap[playerPosition[0]][playerPosition[1] - 1] == '#')
                            { Console.WriteLine("Cannot move there"); }
                            break;
                    }
                }
                catch 
                {
                    return false;
                }

                if (walkIntoMonster == true) // if the player has confronted a monster
                {
                    try
                    {
                        if (workingMap[playerPosition[0] - 1][playerPosition[1]] == 'M') // if north is monster, you have to attack
                        {
                            while (monsterHealth > 0 && playerHealth > 0) // iterates until monster or player is dead
                            {
                                PrintMapToConsole();
                                PrintExtraInfo();
                                Console.WriteLine("  --  Danger is imminent- press Q to attack before the monster does  --  ");
                                string qOrNot = Console.ReadKey().KeyChar.ToString();
                                qOrNot = qOrNot.ToUpper();
                                if (qOrNot == "Q")
                                {
                                    monsterHealth--;

                                }
                                else { playerHealth--; }
                            }
                            if (monsterHealth == 0) // monster despawns from map after death
                            {
                                workingMap[playerPosition[0] - 1][playerPosition[1]] = '.';
                                if (monsterCoinCount > 0)
                                {
                                    workingMap[playerPosition[0] - 1][playerPosition[1]] = 'C';
                                    monsterCoinCount = 0;
                                }
                                monstersQuantity--;
                                monsterHealth = 3;
                                walkIntoMonster = false;
                            }
                            if (playerHealth == 0) // game quits if player dies
                            {
                                QuitGame();
                            }
                        }

                        if (workingMap[playerPosition[0]][playerPosition[1] + 1] == 'M') // same repeats for east
                        {
                            while (monsterHealth > 0 && playerHealth > 0)
                            {
                                PrintMapToConsole();
                                PrintExtraInfo();
                                Console.WriteLine("Danger is imminent- press Q to attack before he does!");
                                string qOrNot = Console.ReadKey().KeyChar.ToString();
                                qOrNot = qOrNot.ToUpper();
                                if (qOrNot == "Q")
                                {
                                    monsterHealth--;

                                }
                                else { playerHealth--; }
                            }
                            if (monsterHealth == 0)
                            {
                                workingMap[playerPosition[0]][playerPosition[1] + 1] = '.';
                                if (monsterCoinCount > 0)
                                {
                                    workingMap[playerPosition[0]][playerPosition[1] + 1] = 'C';
                                    monsterCoinCount = 0;
                                }
                                monstersQuantity--;
                                monsterHealth = 3;
                                walkIntoMonster = false;
                            }
                            if (playerHealth == 0)
                            {
                                QuitGame();
                            }
                        }

                        if (workingMap[playerPosition[0] + 1][playerPosition[1]] == 'M') // same repeats for south
                        {
                            while (monsterHealth > 0 && playerHealth > 0)
                            {
                                PrintMapToConsole();
                                PrintExtraInfo();
                                Console.WriteLine("Danger is imminent- press Q to attack before he does!");
                                string qOrNot = Console.ReadKey().KeyChar.ToString();
                                qOrNot = qOrNot.ToUpper();
                                if (qOrNot == "Q")
                                {
                                    monsterHealth--;

                                }
                                else { playerHealth--; }
                            }
                            if (monsterHealth == 0)
                            {
                                workingMap[playerPosition[0] + 1][playerPosition[1]] = '.';
                                if (monsterCoinCount > 0)
                                {
                                    workingMap[playerPosition[0] + 1][playerPosition[1]] = 'C';
                                    monsterCoinCount = 0;
                                }
                                monstersQuantity--;
                                monsterHealth = 3;
                                walkIntoMonster = false;
                                if (playerHealth == 0)
                                {
                                    QuitGame();
                                }
                            }
                        }
                        if (workingMap[playerPosition[0]][playerPosition[1] - 1] == 'M') // same repeats for west
                        {
                            while (monsterHealth > 0 && playerHealth > 0)
                            {
                                PrintMapToConsole();
                                PrintExtraInfo();
                                Console.WriteLine("Danger is imminent- press Q to attack before he does!");
                                string qOrNot = Console.ReadKey().KeyChar.ToString();
                                qOrNot = qOrNot.ToUpper();
                                if (qOrNot == "Q")
                                {
                                    monsterHealth--;

                                }
                                else { playerHealth--; }
                            }
                            if (monsterHealth == 0)
                            {
                                workingMap[playerPosition[0]][playerPosition[1] - 1] = '.';
                                if (monsterCoinCount > 0)
                                {
                                    workingMap[playerPosition[0]][playerPosition[1] - 1] = 'C';
                                    monsterCoinCount = 0;
                                }
                                monstersQuantity--;
                                monsterHealth = 3;
                                walkIntoMonster = false;

                            }
                            if (playerHealth == 0)
                            {
                                QuitGame();
                            }
                        }

                    }
                    catch 
                    {
                        return false;
                    }
                }
                if (advanced == true)
                {
                    MonsterRandomMovement();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
         * The Main Visual Output element. 
         * It draws the new map after the player did something onto the screen.
         * 
         * This is the method where you implement your the code to draw the map ontop the screen
         * and show the move to the user. 
         * 
         * The method returns true if the game is running and it can draw something, false otherwise.
        */
        public bool PrintMapToConsole()
        {
            if (status == GameState.RUN) // checks that the game is running
            {
               for (int i = 0; i < workingMap[1].Length; i++) // ensures the console window is cleared
               {
                    Console.WriteLine();
               }
               for (int x = 0; x < workingMap.Length; x++) //prints each element of the working map
               {
                  Console.WriteLine(workingMap[x]);
               }
            }
            return true;
        }

        /**
         * Additional Visual Output element. 
         * It draws the flavour texts and additional information after the map has been printed.
         * 
         * This is the method does not need to be used unless you want to output somethign else after the map onto the screen
         * 
         */ 
        public bool PrintExtraInfo()
        {
            if (counter > 0 && status == GameState.RUN)
            {
                Console.WriteLine("     ---  PLAYER'S INFORMATION   ---     ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Steps: " + GetStepCounter());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Coins: " + CoinCounter());
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Player Health: {playerHealth}");
                Console.ForegroundColor = ConsoleColor.White;
                if (monstersQuantity > 0) // only prints if monsters are present
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Monster health: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(monsterHealth);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.Write("Monster Count: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(monstersQuantity);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Monster Coin Count: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(monsterCoinCount);
                    Console.WriteLine();
                }
                else if (monstersQuantity == 0) // prints if no monsters are on map
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("All the monsters have been defeated!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return true;
            }
            else
            {
                return false;
            }
        } 

        /**
        * Map and GameState get initialized
        * mapName references a file name 
        * Do not use abosolute paths but use the files which are relative to the eDecutable.
        * 
        * Create a private object variable for storing the map in Game and using it in the game.
        */
        public bool LoadMapFromFile(String mapName)
        {
            string mapPathing = @".\maps\"; // writes the correct path to the chosen map
            if (mapName.ToUpper() == "SIMPLE.MAP") { mapName = mapPathing + "Simple.map"; }
            else if (mapName.ToUpper() == "SIMPLE2.MAP") { mapName = mapPathing + "Simple2.map"; }
            else if (mapName.ToUpper() == "ADVANCED.MAP") { mapName = mapPathing + "Advanced.map"; }
            try // loads the map into char arrays
            {
                string[] temporaryMapString = File.ReadAllLines(mapName); // the map file is read inside string array
                originalMap = new char[temporaryMapString.Length][]; // 2d jagged array to store the original map
                workingMap = new char[temporaryMapString.Length][]; // 2d jagged array to store the active game map with moving player
                for (int t = 0; t < temporaryMapString.Length; t++) // iterates thru the string, converts to char array and restores it in pre-existing char arrays
                {
                        originalMap[t] = temporaryMapString[t].ToCharArray();
                }
                for (int i = 0; i < temporaryMapString.Length; i++) // iterates thru the string, converts to char array and restores it in pre-existing char arrays - different internal storage to originalMap
                {
                        workingMap[i] = temporaryMapString[i].ToCharArray();
                }
                for (int y = 0; y < originalMap.Length; y++) // counts the amount of monsters
                {
                    for (int x = 0; x < originalMap[y].Length; x++)
                    {
                        if (originalMap[y][x] == 'M')
                        {
                            monstersQuantity++;
                        }
                    }
                }
                status = GameState.START;
                return true;
            }
            catch
            {
                Console.WriteLine("Map failed to load");
                return false;
            }
        }
        /**
         * Returns a representation of the currently loaded map
         * before any move was made.
         * This map should not change when the player moves
         */
        public char[][] GetOriginalMap()
        {
            return originalMap;
        }

        /*
         * Returns the current map state and contains the player's move
         * without altering it 
         */
        public char[][] GetCurrentMapState()
        {
            // iterates to find if the door to exit is present, or if player is stood on it (game should stop)
            char isDPresent = '.';
            for (int y = 0; y < workingMap.Length; y++) // checking if the door is present
            {
                for (int x = 0; x < workingMap[y].Length; x++)
                {
                    if (workingMap[y][x] == 'D') // checks if D is present
                    {
                        isDPresent = workingMap[y][x]; // stores the co-ordinates of D if D is present
                    }
                }
            }
            if (isDPresent == '.')
            {
                status = GameState.STOP;
            }
            return workingMap;
        }

        /**
         * Returns the current position of the player on the map
         * 
         * The first value is the y coordinate and the second is the x coordinate on the map
         */
        public int[] GetPlayerPosition()
        {
            int[] placeholder = {0, 0};
            if (status == GameState.START || status == GameState.RUN)
            {
                for (int y = 0; y < workingMap.Length; y++) // loops through the working map to find the co-ordinates of @ or P
                {
                    for (int x = 0; x < workingMap[y].Length; x++)
                    {
                        if (workingMap[y][x] == '@' || workingMap[y][x] == 'P')
                        {
                            placeholder[0] = y;
                            placeholder[1] = x;
                        }
                    }
                }
                return placeholder;
            }
            else // if game not running, player position is set to 0 y axis, 0 x axis
            {
                return placeholder;
            }
        }

        private void MonsterRandomMovement()
        {
            Random randomised = new Random();
            int randomisedMove = randomised.Next(1, 5); // randomisedMove is assigned random number

            for (int y = 0; y < workingMap.Length; y++) // iterates to find monsters
            {
                for (int x = 0; x < workingMap[y].Length; x++)
                {
                    if (workingMap[y][x] == 'M')
                    {
                        randomisedMove = randomised.Next(1, 5); // assigned random number from 1 - 4, moves depending on the number
                        if (randomisedMove == 1) // monster moves east
                        {
                            if (workingMap[y][x + 1] == '.')
                            {
                                workingMap[y][x + 1] = 'M';
                                workingMap[y][x] = '.';
                            }
                            else if (workingMap[y][x + 1] == 'C') // if monster walks over coin, it collects it and it's health increments by one
                            {
                                monsterHealth++;
                                monsterCoinCount++;
                                workingMap[y][x + 1] = 'M';
                                workingMap[y][x] = '.';
                            }
                        }
                        if (randomisedMove == 2) // monster moves north
                        {
                            if (workingMap[y - 1][x] == '.')
                            {
                                workingMap[y - 1][x] = 'M';
                                workingMap[y][x] = '.';
                            }
                            else if (workingMap[y - 1][x] == 'C')
                            {
                                monsterHealth++;
                                monsterCoinCount++;
                                workingMap[y - 1][x] = 'M';
                                workingMap[y][x] = '.';
                            }
                        }
                        if (randomisedMove == 3) // monster moves south
                        {
                            if (workingMap[y + 1][x] == '.')
                            {
                                workingMap[y + 1][x] = 'M';
                                workingMap[y][x] = '.';
                            }
                            else if (workingMap[y + 1][x] == 'C') 
                            {
                                monsterHealth++;
                                monsterCoinCount++;
                                workingMap[y + 1][x] = 'M';
                                workingMap[y][x] = '.';
                            }
                        }
                        if (randomisedMove == 4) // monster moves west
                        {
                            if (workingMap[y][x - 1] == '.')
                            {
                                workingMap[y][x - 1] = 'M';
                                workingMap[y][x] = '.';
                            }
                            else if (workingMap[y][x - 1] == 'C')
                            {
                                monsterHealth++;
                                monsterCoinCount++;
                                workingMap[y][x - 1] = 'M';
                                workingMap[y][x] = '.';
                            }
                        }
                    }
                }
            }
            PrintMapToConsole();
        }

        /**
        * Returns the next player action
        * 
        * This method does not alter any internal state
        */
        public int GetPlayerAction()
        {
            if (status == GameState.RUN)
            {
                return (int)action;
            }
            else
            {
                action = PlayerActions.NOTHING;
                return (int)action;
            }
        }
        

        public GameState GameIsRunning()
        {
            return status;
        }

        public GameState QuitGame()
        {
            return status = GameState.STOP;
        }

        /**
         * Main method and Dntry point to the program
         * ####
         * Do not change! 
        */
        static void Main(string[] args)
        {
            Game crawler = new Game();

            string input = string.Empty;
            Console.WriteLine("Welcome to the Commandline Dungeon!" + Environment.NewLine +
                "May your Quest be filled with riches!" + Environment.NewLine);

            // Loops through the input and determines when the game should quit
            while (crawler.GameIsRunning() != GameState.STOP && crawler.GameIsRunning() != GameState.UNKOWN)
            {
                Console.Write("Your Command: ");
                input = crawler.ReadUserInput();
                Console.WriteLine(Environment.NewLine);
                crawler.ProcessUserInput(input);
                crawler.Update(crawler.GameIsRunning());
                crawler.PrintMapToConsole();
                crawler.PrintExtraInfo();
            }

            Console.WriteLine("See you again" + Environment.NewLine +
                "In the CMD Dungeon! ");
        }
    }
}