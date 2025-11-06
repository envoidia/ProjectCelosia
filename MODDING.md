# Modding Guide
Enable the mod loader from the settings, and all mods in the mods folder will be loaded on startup

## How to create your own mod
(todo more detail, provide template)

Create a class library that has a ProjectReference to API. Create a Main class that extends [IGameMod](API/Modding/IGameMod.cs)

The base game content ([Celosia](Celosia)) is coded as a mod, so reference that for details