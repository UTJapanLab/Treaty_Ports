# Ready Set Yokohama

## About the Project

Ready Set Yokohama! is a digital version of a Meiji Era Japanese board game (type called Suguroku) that depicts Japan's transition from a isolated nation
into a global power. The game acts as a guide through your jounrey from Tokyo to Yokohama treaty port, depicting some of the sights you may see as well as adding in 
some history snippets of the places you visit. Get ready to race your friends during this jounrey to Yokohama and back to Tokyo. Now, READY..... SET...... YOKOHAMA!

## Helpful Coding Files

### GameControl.cs
This is the main file for game controls and holds the names of the all of the board spaces along with the camera movement. Each function is named after its intended usage.

### FollowThePath.cs
This file is used to follow the player's path as it moves through the board. It uses waypoints in order to have the camera follow the players movements rather than 
teleporting straight to the intended board space.

### SetupGame.cs
This file  setsup the game in regards to the number of players and what order the players take turns.

### PlayerInfo.cs
This file contains a player's info. Specifically, it contains the current position, the current place in the race, if the player is going back to Tokyo from Yokohama,
if the player lost a turn, and the player's already visited board spaces.

### CloseSpace.cs
This file closes an already open board space which is opened up each time a player lands on a new board space.

### Dice.cs
This file controls the dice rolling of the game.

### GameOver.cs
This file ends the game and turns off the camera.
