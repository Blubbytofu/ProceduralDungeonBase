# ProceduralDungeonBase
 A framework for procedurally generated dungeons

------------------------------------------------
Steps:
 1. Create a prospective max dungeon grid of floor tiles
 2. Use binary space partitioning to split the dungeon into prospective rooms
 3. For each room, use random walk to create the actual floor
 4. Use prim's algorithm to create links between room center tiles
 5. Use A* pathfinding to link room centers
 6. Instantiate floor tiles
 7. Instantiate wall tiles
 8. Instantiate ceiling tiles
 9. ???
------------------------------------------------