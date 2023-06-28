# Destiny2D

## Small 2D Networking Dungeon Crawler

## How To Play
- Load initialize scene and press play.
- Spawn a weapon using the spawn weapon button.
- Open and close inventory using "I".
- Click on items on inventory and click the single slot on the left to equip.
- Attack using "Space".

## Main Components

### Dungeon Generation


  Used binary partitioning and Cellular Automaton algorithms to create the rooms.
  Created a graph representation of the rooms through nodes and edges. Then used MST algorithm to create a set of hallways that connects all rooms.

  Relevannt Directories:
  
  - Scripts/Dungeon

### Networking
  
  Used Netcode for Gameobjects to add multiplayer compatibility
  
  Inventory and the Player Controller Scripts are the most implemented

  Relevant Directories:
  
  - Scripts/Client
  
  - Scripts/Shared
  
  - Scripts/Server
