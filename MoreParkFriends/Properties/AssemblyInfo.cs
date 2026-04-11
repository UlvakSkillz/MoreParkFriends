using MelonLoader;
using MoreParkFriends; // The namespace of your mod class

[assembly: MelonInfo(typeof(MoreParkFriendsClass), BuildInfo.ModName, BuildInfo.ModVersion, BuildInfo.Author)] //Melonloader Info for your Mod. See BuildInfo in Class File
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")] //Tells MelonLoader this Mod is for Rumble
[assembly: MelonColor(255, 195, 0, 255)] //Mod Name Color when Mod Loads and Logs.
[assembly: MelonAuthorColor(255, 195, 0, 255)] //Author Color when Mod Loads.
[assembly: VerifyLoaderVersion(0, 7, 2, true)] //MelonLoader 0.7.2 or above

