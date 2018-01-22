# VerticalShooter
Example of a vertical shooter name made on Unity

This is a vertical shooter game:

![Image of the game](http://juanmaramon.com/assets/img/shooter.png)

* Player’s plane can be controlled with AWSD or arrow keys and fire with Space bar.
* Configurable win conditions can be configure on GameController script:

![Image of inspector](http://juanmaramon.com/assets/img/inspectorPlanes.png)

* Here is the UML diagram with Project clases:

![UML classes](http://juanmaramon.com/assets/img/UMLClasses.png)
 
Classes inside Assets/_Scripts/Core are common functionality that can be useful for other projects.

Planes are modelled in this way:
* IKillable and IDamagable are interfaces that define the ability to receive damage and be killed.
* PlaneBase class model common functionality for all planes:
  * Player plane
  * Enemy planes
* PlayerController is the class that allow the player control his plane and interact with all the gameplay elements.
* EnemyBase has the common functionality for all enemy types:
  * Column enemies: They move in diagonal and return to top border of the screen.
  * Side enemies: Go to sides of the screen and are more tough

Here are the resources used for this game:

* http://unluckystudio.com/free-game-artassets-for-games-12-top-down-planes-sprites-pack/ planes
* https://opengameart.org/textures/5672 fx
* https://opengameart.org/node/7690 fx
* https://freesound.org/people/Nbs%20Dark/sounds/94185/ audio
* https://freesound.org/people/burning-mir/sounds/155139/ audio
* http://seamless-pixels.blogspot.com.es/p/free-seamless-ground-textures.html background texture

