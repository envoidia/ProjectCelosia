# Modding Guide
All mods in the mods folder will be loaded on startup

## How to create your own mod
(todo more detail, provide template)

This tutorial will assume that you're using C#, because it's what the game is written in. You can use other languages if desired. Other .NET languages (F# and VB.NET) are the easiest, as they can interact directly. Other languages will require some level of bindings and FFI

Create a class library that has a ProjectReference to API. Create a static class with the attribute [ModEntryPoint](API/Modding/ModEntryPointAttribute.cs) and add a public property of type [GameMod](API/Modding/GameMod.cs)

The base game content ([Celosia](Celosia/Main.cs)) is coded as a mod, so reference that for details