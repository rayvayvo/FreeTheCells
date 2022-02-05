# FreeTheCells

Hello there!

Raven here; I've included some developer notes in the readme


How far I got on the freecell project:
====================================

Completed:
------------------------------------


  -three screens (welcome, play, end)
  
  -game logic and rules
  
  -dealing cards
  
  -double clicking to move "open" cards to appropriate slot
  
  -game card drag/drop mechanics
  
  -not allowing illegal moves
  
  -JSON writing/reading for player stats
  
  -uploaded via Github


Not completed:
------------------------------------
  -special animations/effects


known bugs since I ran out of time:
------------------------------------

  -Double clicking cards to clear from "freecell" slot sometimes doesn't clear data properly and renders that slot unusable.
  
  -Drag and drop on Completed("foundation") slots doesn't work properly since Raycast ignore on the image components function has a bug.
  
  -under certain conditions you can move a stack of cards that match the alternating suit convention, but the top card value doesnt match (the following cards in the        stack do though).
  
  -shuffling a new game doesn't clear the Completed("foundation") stack properly
  
If I had one more day:
------------------------------------

  -API for downloading more card back styles
  
  -in game sounds
  
  -animation for victory (copy the old microsoft one with the waterfall of cards maybe?)
  
  -fix all the bugs
  
  -particle effects or animations 
  
  -UI improvements across the board
  
  -general polish
  
  -clean up code by removing all Debug.logs, fixing alignment and spacing, and maybe refactoring a function or two
  
  -add in save games so you can load unfinished games
  
  -move the load player stats function into its own donotdestroy script from title screen so it is loaded way before the game board for cleaner data management
