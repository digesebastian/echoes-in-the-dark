# TODO list

# Need to have

## Throwing coin

- [x] v1: throw coin from center of character. Throw a hardcoded distance in front of character.
- [x] Add player state for other scripts to access
- [x] make coin land properly
  - [x] make coin stay a few seconds after landing then disappear
  - [x] Make coin emit a circle of light
- [x] Create throwing bar
  - [x] Bar charges when holding down throw key, then throws when released
  - [x] Control throwing force according to bar
- [] Add collision for coin
- [] Add animation for throwing
  - [] create curve for thrown coin
  - [] create shadow

## Improving light circles

- [] Create base class for light circles. Make player and projectile use this class
- [] Add a clearer border to the rings of light around the player and thrown items
- [] Add ripples going outwards from sources of light
- [] Add hit detection to edges of light circles

# Nice to have

- [] move movement logic in movementcontroller to fixedupdate, while animation logic stays in update. 
This makes the move speed independent of frame rate
- 

# Bugs

- [] Sprinting animation plays all frames even when taking a small step