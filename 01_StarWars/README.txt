Submitted by:
Raz Karl 311143127
Gal Zemach 301830733

Spaceship #1: CyberShip
* Raises shield when under threat (either an enemy spaceship or a shot are closer than a given threshold), 
* Otherwise targets the nearest spaceship and moves towards it's approximate location (adds the target's velocity vector to it's current position to determine the future position where it is going to be in the next game loop, achieving a better chasing behavior than the DarthShip)
* Saves shield energy for real emergencies by only using the shield when in real danger and just for a very short period of time

Spaceship #2: PaperTiger
* if the nearest ship turns to face it it runs away, if possible - to the second nearest ship.
* If no one is watching it tries to shoot at the nearest ship
