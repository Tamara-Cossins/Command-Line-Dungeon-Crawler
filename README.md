# 2022-COMP1000-CW

## Code Walkthrough Video: https://www.youtube.com/watch?v=I4BOwNzBFa0

## UNIT TESTS PASSED: 13 / 13



# - Introduction - :skull_and_crossbones:
A Dungeon Crawler infested with monsters preventing you from reaching the exit. Collect the coins to regenerate health and flee to the exit without dying!

Advanced Map Demo:


![AdvancedGIF - Made with Clipchamp](https://user-images.githubusercontent.com/114652776/212640009-cca8ab4f-f3ac-42a2-a27c-e4c6f9f2035a.gif)


## Explanation:
**@** is the player.

**#**'s are walls which are non-passable for the player.

**.** are empty spaces that the player can walk into.

**C**'s are coins which are collectable to both the player and monsters and increment their health.

**M**'s are monsters which start with 3 hearts, and can increment their health by collecting coins.

**D** is the exit. Upon standing on the tile. the game ends and asks if the user wants to replay.

You can select one of three maps to play: Simple.map, Simple2.map and Advanced.map, each with varying levels of difficulty.




# - How to play -

## How to install:

1. Download the Dungeon-10780414.zip file

2. Extract the compressed file

3. Copy the filepath of the folder

4. Type: cd [filepath] into command line

5. Type: start Dungeon.exe



![2023-01-16](https://user-images.githubusercontent.com/114652776/212658617-8a4e1f13-0ed8-4701-bb3d-d29809929529.png)



### Starting the Game: 
• Type in "load Simple.map" or "load Simple2.map" or "load Advanced.map" depending on desired map choice

• Type in "play" or "start"

• Type in "advanced" or "No" depending on desired advanced behaviours choice



## Player Controls: :joystick:
**W**, **A**, **S**, **D** are the input controls used to move the player.

**W** = Player moves one step North

**A** = Player moves one step West

**S** = Player moves one step South

**D** = Player moves one step East

**Z** = Player picks up coin (when standing on it)

**Q** = Player attacks monster (deals one damage point)


## The Advanced Behaviours I used:
✅ Using the command “advanced” before typing in 'play' is used to enabled the advanced
mode - only then are advanced features enabled.       

✅ Without adding “advanced” the game is in basic mode without moving monsters.

✅ Using the “Q” key the player attacks the nearby monster and takes away one heart.

✅ The advanced map can be loaded and completed by the user.

✅ When player moves over a tile the tile is hidden and the player symbol is rendered until
player leaves the tile.

  ✅ Map elements:
  
✅ o **“M”** are monsters (which you can move over empty spaces).

✅ o **“#”** are walls which cannot be passed.

✅ o **“C”** are gold pieces which can be collected or walked over.

✅ Monsters start off with 3 hearts.

✅ An attack from the player only deals 1 damage.

✅ Players heal one damage when they collect a coin and start with 2 health.

✅ Monsters deal damage to player once confronted and if the player does not attack in time.

✅ Monsters can eat coins to get stronger.

✅ When Monsters die they drop their coins, which the player can then pick up.

✅ Upon entering the exit and finishing the game, the player get displayed a status message to either replay or quit the game.

✅ Allow for a Replay of the map using the command: “replay

## Code Functionality:

I used:

using System;

using System.Linq;

using System.IO;

using System.Collections.Generic;


Throughout the code, I used different C# techniques and data structures. Examples being Jagged Array data structures, the use of File Reading and the directory class, programming constructs such as foor loops, if statements and switch statements. 


# RESOURCES USED: 
• https://guides.github.com/features/mastering-markdown/

• https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/jagged-arrays

•  https://www.w3schools.com/cs/cs_files.php
 
