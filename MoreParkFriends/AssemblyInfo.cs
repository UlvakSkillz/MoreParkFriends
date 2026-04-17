using MelonLoader;
using BuildInfo = MoreParkFriends.BuildInfo;

[assembly: MelonInfo(typeof(MoreParkFriends.Main), BuildInfo.ModName, BuildInfo.ModVersion, BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 195, 0, 255)]
[assembly: MelonAuthorColor(255, 195, 0, 255)]
[assembly: VerifyLoaderVersion(0, 7, 2, true)]
[assembly: MelonAdditionalDependencies("UIFramework")]

