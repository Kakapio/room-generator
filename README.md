# room-generator
A simple 2d generator that uses an implementation of the drunkard walk algorithm in order to create caves.

## technicals
Project was implemented with Monogame. Monogame was used to render/update game loop and to receive input. The drunkard walk implementation worked by spawning 'eaters' in a grid of walls, where eaters would move in a random direction on each update and remove a wall. Upon each attempted movement by an eater, it has a chance of spawning another eater at its current location. I used a 1/400 chance to spawn eaters, as this gave me nice caves without spawning too many eaters. There is also a limit on the number of walls that can be eaten away, which is why it is important that there are not too many eaters (as they would prematurely consume all the walls and leave us with a blob).

## cave generation

![](https://i.imgur.com/59EUsrW.gif)
