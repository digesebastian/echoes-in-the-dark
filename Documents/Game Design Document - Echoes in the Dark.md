# Overview and Vision statement

You play as a blind thief who carries out heists in places like art galleries and jewellery stores. Since you can't see, to navigate you need to use the sounds you make to briefly reveal the world around you.

# Gameplay

- **Perspective:** Fixed top-down maze, with the camera following the player in a smaller view of the maze.

- **Vision System:** You start only able to see your own sprite. Moving creates visual sound waves that light up a circle around you (clearer up close, blurrier further away). The faster and louder you move, the bigger your vision circle gets.

- **Objective:** Find the treasure(s) and get to the escape door. Once you grab the loot, a timer starts. If it runs out, the alarm goes off and guards become frenetic, making escape much harder.

- **The Escape:** Exits can be simple open doors or locked ones. For locked doors, you might need a code (found on paper or overheard) or you might just have to time your escape when guards open the door for a few seconds.

- **Distractions & Throwing Mechanism**:
  You can throw items to make noise where they land. This reveals a new area for a short time but attracts nearby guards to that spot. 
  The player controls how far they throw by holding down a key (with a visible charge bar).

	- **Limited Supply:** You only have a limited number of items to throw per level, so you have to use them strategically.
    
	- **Coins:** These make a sound exactly where they land. The noise briefly reveals that area to you and attracts the nearest guards to investigate the spot.
    
	- **Rocks:** Rocks have a different physics mechanic and can bounce a maximum of 2 times.
    
	    - **Sound Waves:** Every time the rock bounces or lands, it creates a sound circle. If a guard is inside this circle, they get alerted and their own detection circle gets bigger.
        
	    - **Knockouts:** If you manage to hit a guard directly with the rock, it knocks them out (KO) for a specific amount of time.

- **Enemies & Detection**

	- **The Dark Advantage:** You start every level by cutting the power, blinding the guards and turning the heist into a timed puzzle. You have to grab the loot before the lights come back on. If the lights turn on, you lose the level.
    
	- **Tracking Guards:** Guards patrol the maze on set paths. When they are nearby but outside your direct vision, you can track them by hearing their footsteps, which also show up visually on your screen (both the footsteps and the circle around them).
    
	- **Detection Circles:** Every guard has a detection circle around them. If a guard's circle collides with your vision circle, they will spot you and chase you.
    
	- **Game Over:** If a guard touches you, you're caught and the run is dead.
    
	- **Alarms:** When you steal the loot, an alarm goes off, the guards move faster and their detection circles get much bigger, making them significantly harder to avoid.
	  
- **Pre-planning:** no animation, arrow pointing towards the goal (treasure, then escape door)
  

# Characters

- **The Blind Thief:** The player character.
   
- **The Guards:** The main obstacles patrolling the area.

# Story

There isn't a deep narrative yet, just the core setup: a blind thief relying on an informer to pull off high-stakes heists.

# World

Mazes taking place in high-security locations like art galleries and jewellery stores.

# Media list

...

# Technical specification

- Top-down 2D camera system following the player.

# Design Decisions

Open questions and ideas:

- **Wall Memory:** When a player moves away from a wall they’ve "seen", does the outline stay on the screen forever, or does it fade back to black?

- **The Informer:** To increase difficulty later in the game, should the informer sometimes be untrustworthy and give us wrong information on the sketch map?

# Development decisions

- **Core Base:** First step is just getting the character into a single room with the camera following them.

- **Prototype Scope:** The first prototype will feature exactly 1 level, 1 thing to steal, an unlocked exit door, the blindness and throwing mechanics, guards, basic sounds (footsteps and a thrown coin), character controllers/location.

---
# Tasks

### Deadline: 27/10

- Code the bare minimum core (character in a room, camera follow) - Sherif
- Format the Game Design Document (GDD) - Francesco
- Set up the GitHub repository - Sebastien
- Draw/create the characters' sprites - Karen

### Deadline: 3 november

- maze + treasure and escape door - Karen
- blindness mechanism (circle around the player) + fixed camera, player controller with fast and slow - Sherif and Sebastien 
- guards behavior (tree) - Francesco and Wojtek

### Deadline: 10 november

- couple of rooms, hallway - Karen
- guards, attention point mechanism (with priorities), circle around them + chasing mechanic - Francesco and Wojtek
- throwing mechanism - Sebastien
- sounds - Sherif

### Deadline: 17 November

- sounds (footsteps, throwing sound) - Sherif 
- fix the guard movement, guard root, pathfinding - Wojtek
- add the map, footstep animation - Karen
- outline the thrown coin, collision and bouncing, "throwing power" mechanism - Sebastien
- guards react to the throwing, merge guards and throwing - Francesco

---

# Changelog

- 20/10: first version of the GDD
- 27/10: 
	- removed pre-planning animation
	- improved guards' behaviour
	- improved throwing mechanism

---

# 27/10 meeting notes

- corners, narrow places
- strange architecture
- multiple paths (2-3) to the treasure

controller:
- w: walking - circle of a fixed size
- shift + w: running - circle gets bigger and bigger, you accelerate
- space + w: stealth mode - going at a fixed slow speed, circle is small

---
# 3/11 meeting notes

- floors with different materials that emit different sounds
- achievements/stars for how well you completed the level
