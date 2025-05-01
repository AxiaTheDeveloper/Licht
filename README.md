# GameJamIGI

[Licht Itch.io Link](https://dhyox.itch.io/licht) **(Windows Only)**

Unity 2021.3.10f1

(programmer notes ('25))

We developed this for the 3-days game jam by Ignite Game Jam 2023.

For this project I implemented SRP of SOLID principle.
For optimization: used Comparetag for comparing collider. 
For game programming pattern: I used singleton for managers and state pattern for the game state control. 
For design pattern: I still only used model view pattern.

My friend helped in movement on the first early day. Then I moved to create the slope movement, light mechanics, puzzles, and all the main mechanics of the game.
I specifically made all the puzzles to be game design friendly, so for example for a gate that needs to be open by levers. I make sure that my game designer can add as many levers as they want for the conditions to open the gate.

I made this after I developed Sign Wizard, so the same tight couplings can be seen.

For the tools and frameworks outside normal framework from unity, we used: Cinemachine & LeanTween
