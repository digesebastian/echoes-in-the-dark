# Overview and Vision statement

You play as a blind thief who carries out heists in places like art galleries and jewellery stores. Since you can't see, to navigate you need to use the sounds you make to briefly reveal the world around you.

# Gameplay

- **Perspective:** Fixed top-down maze, with the camera following the player in a smaller view of the maze.

- **Vision System:** You start only able to see your own sprite. Moving creates visual sound waves that light up a circle around you (clearer up close, blurrier further away). The faster and louder you move, the bigger your vision circle gets.

- **Objective:** Find the treasure(s) and get to the escape door. Once you grab the loot, a timer starts. If it runs out, the alarm goes off and guards become frenetic, making escape much harder.

- **The Escape:** Exits can be simple open doors or locked ones. For locked doors, you might need a code (found on paper or overheard) or you might just have to time your escape when guards open the door for a few seconds.

- **Distractions:** You can throw items to make noise where they land. This reveals a new area for a short time but attracts nearby guards to that spot. 
  The player controls how far they throw by holding down a key (with a visible charge bar).
  Stretch Goal: give the player multiple items to be thrown that make different amounts of noise and make sound in different moments (e.g. as they fly through the air or only when they hit the ground).	

- **Enemies:** Guards patrol the maze on set paths. If they enter your vision circle, they'll chase you. If they're outside your circle, you can track them by hearing footsteps or jingling keys. Outside of your circle they're visually represented as footsteps.

- **Pre-planning:** Before a level starts, an "informer" gives you a sketched map showing the loot, guards, and exit so you have a rough idea of where to go.

# Characters

- **The Blind Thief:** The player character.

- **The Informer:** The person scouting the area and giving you an idea of the place beforehand.
   
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

- **Prototype Scope:** The first prototype will feature exactly 1 level, 1 thing to steal, an unlocked exit door, the blindness and throwing mechanics, guards, basic sounds (footsteps and a thrown coin), character controllers/location, and the pre-level sketch map.

---
# Tasks

### Deadline: 27/10

- Code the bare minimum core (character in a room, camera follow) - Sherif
- Format the Game Design Document (GDD) - Francesco
- Set up the GitHub repository - Sebastien
- Draw/create the characters' sprites - Karen
