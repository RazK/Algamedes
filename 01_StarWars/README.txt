Submitted by:
Raz Karl 311143127
<student name> <id>

Spaceship #1: CyberShip
* Raises shield when under threat (either an enemy spaceship or a shot are closer than a given threshold), 
* Otherwise targets the nearest spaceship and moves towards it's approximate location (adds the target's velocity vector to it's current position to determine the future position where it is going to be in the next game loop, achieving a better chasing behavior than the DarthShip)
* Saves shield energy for real emergencies by only using the shield when in real danger and just for a very short period of time

Spaceship #2: <name>
<strategy description>
