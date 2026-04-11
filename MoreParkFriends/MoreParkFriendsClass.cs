using Il2CppPhoton.Pun;
using Il2CppRUMBLE.Environment;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Social;
using MelonLoader;
using RumbleModdingAPI.RMAPI; //allows for RumbleModdingAPI usage
using RumbleModUI; //allows for ModUI usage
using System;
using UnityEngine;

public static class BuildInfo
{
    public const string ModName = "More Park Friends";
    public const string ModVersion = "2.3.0";
    public const string Author = "UlvakSkillz";
}

namespace MoreParkFriends
{
    public class MoreParkFriendsClass : MelonMod
    {
        //Mod's variables
        public static int multiplier = 2;
        public static bool showExpanded = true;
        public static Mod MoreParkFriends = new Mod();
        internal static int playerCap = 1;
        internal static int additionsMade = 6;
        private static string currentScene = "Loader";
        private static string descriptionText = "Changes Max Players Allowed in a Park.";
        private static ModSetting descriptionSetting;

        public override void OnLateInitializeMelon()
        {
            //setup ModUI Info
            MoreParkFriends.ModName = BuildInfo.ModName;
            MoreParkFriends.ModVersion = BuildInfo.ModVersion;
            MoreParkFriends.SetFolder("MoreParkFriends"); //Set ModUI UserData Folder
            descriptionSetting = MoreParkFriends.AddDescription("Description", "", "", new Tags { IsEmpty = true, DoNotSave = true }); //can't use UI.instance.ForceRefresh() without a description :(
            MoreParkFriends.AddToList("Multiplier", 2, "Changes Player Counts on the Park Board. Limit: 1 - 3", new Tags { });
            MoreParkFriends.AddToList("Show Expanded Players", true, 0, "Scales the Player Tags on the Park Board to Show Every Player in the Park.", new Tags { });
            MoreParkFriends.GetFromFile(); //loads stored settings from file
            MoreParkFriends.ModSaved += Save; //runs Save Method when User Clicks Save in ModUI
            UI.instance.UI_Initialized += delegate { UI.instance.AddMod(MoreParkFriends); }; //Adds the Mod to ModUI when ModUI is ready
            Save(); //Run Save Method to setup multiplier variable
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName; //stores current scene for later use
            additionsMade = 6; //resets to the default 6 on scene load
            if (currentScene == "Gym") { UpdateGymText(); } //if in Gym, Show Multiplier numbers on board
        }

        private void Save()
        {
            descriptionText = $"Changes Max Players Allowed in a Park.";

            if (multiplier != (int)MoreParkFriends.Settings[1].SavedValue) //if user changed multiplier
            {
                int tempMult = (int)MoreParkFriends.Settings[1].SavedValue;
                if (Math.Max(1, Math.Min(3, tempMult)) != tempMult)
                {
                    tempMult = Math.Max(1, Math.Min(3, tempMult));
                    MoreParkFriends.Settings[1].Value = tempMult;
                    MoreParkFriends.Settings[1].SavedValue = tempMult;
                    if (UI.instance.IsUIVisible() && UI.instance.IsModSelected(BuildInfo.ModName) && UI.instance.IsOptionSelected("Multiplier"))
                    { //refresh ui if ui is open to Multiplier
                        UI.instance.ForceRefresh();
                    }
                }
                multiplier = tempMult; //save stored setting to variable

                descriptionText += $"{Environment.NewLine}Current Multiplier: x{multiplier}";

                if (currentScene == "Gym") { UpdateGymText(); } //if in Gym, Show Multiplier numbers on board
            }

            if (showExpanded != (bool)MoreParkFriends.Settings[2].SavedValue) //if user changed if the board should expand
            {
                showExpanded = (bool)MoreParkFriends.Settings[2].SavedValue; //save if it should extend the Park Board
                if (currentScene == "Park") { ScaleBoard(null, PlayerManager.instance.AllPlayers.Count); } //if in Park, Change the Board Scale
            }

            descriptionText += $"{Environment.NewLine}Shrink Player Tags to fit Player Count?: {(showExpanded ? "Yes" : "No")}";
            descriptionSetting.Description = descriptionText;
        }

        private void UpdateGymText()
        {
            GameObject textsParent = GameObjects.Gym.INTERACTABLES.Parkboard.RotatingScreen.HostPanel.PlayerCpapcity.TextandIcons.GetGameObject(); //get common texts parent
            for (int i = 1; i <= 5; i++) //loop each child
            {
                textsParent.transform.GetChild(i).GetComponent<Il2CppTMPro.TextMeshPro>().text = ((1 + i) * multiplier).ToString(); //update component text with new multiplier
            }
        }

        internal static void UpdateParkBoardPlayerTags(ParkBoardParkVariant __instance)
        {
            float playerCount = PlayerManager.instance.AllPlayers.Count;
            if (__instance == null) //if null:
            {
                try { __instance = GameObjects.Park.INTERACTABLES.ParkboardPark.GetGameObject().GetComponent<ParkBoardParkVariant>(); } //try to grab parkboard
                catch { return; } //if error, stop
                if (__instance == null) { return; } // if still null, stop
            }

            for (int i = 6; i < playerCap; i++)
            {
                if (i < playerCount) //if it's an actual player, load info
                {
                    GeneralData generalData = PlayerManager.instance.AllPlayers[i].Data.GeneralData;
                    __instance.parkBoardPlayerTags[i]?.Initialize(new UserData(generalData.PlayFabMasterId, generalData.PlayFabTitleId, generalData.PublicUsername, generalData.BattlePoints));
                    __instance.hostIcons[i]?.transform.parent.GetChild(1).gameObject.SetActive(showExpanded && PhotonNetwork.IsMasterClient);
                    __instance.parkBoardPlayerTags[i]?.gameObject.SetActive(showExpanded ? true : false);
                }
                else //if not an actual player, reset to default state
                {
                    __instance.parkBoardPlayerTags[i]?.Initialize(new UserData("", "", "Name", 0));
                    __instance.hostIcons[i]?.transform.parent.GetChild(0).gameObject.SetActive(false);
                    __instance.hostIcons[i]?.transform.parent.GetChild(1).gameObject.SetActive(false);
                    __instance.parkBoardPlayerTags[i]?.gameObject.SetActive(false);
                }
            }
            ScaleBoard(__instance, PlayerManager.instance.AllPlayers.Count); //set parents scale and position to fit new PlayerTag count as needed
        }



        private static void ScaleBoard(ParkBoardParkVariant __instance, float playerCount)
        {
            if (showExpanded) //if showEnabled is on in ModUI
            {
                //host & kick icons (scales parent called Addition)
                float posPlayerCountClamped = (11f / 30f) * (float)Math.Sqrt((Math.Max(0, playerCount - 6f)) / 12f);
                Melon<MoreParkFriendsClass>.Logger.Msg("Clamped: " + posPlayerCountClamped);
                __instance.hostIcons[0].transform.parent.parent.localPosition = new Vector3(0f, posPlayerCountClamped, 0f);
                __instance.hostIcons[0].transform.parent.parent.localScale = new Vector3(1f, Math.Min(1f, 6f / playerCount), 1f);

                //player tags
                __instance.parkBoardPlayerTags[0].transform.parent.localPosition = new Vector3(0f, posPlayerCountClamped, 0f);
                __instance.parkBoardPlayerTags[0].transform.parent.localScale = new Vector3(1f, Math.Min(1f, 6f / playerCount), 1f);
            }
            else //if showEnabled is off in ModUI
            {
                //host & kick icons (scales parent called Addition)
                __instance.hostIcons[0].transform.parent.parent.localPosition = Vector3.zero;
                __instance.hostIcons[0].transform.parent.parent.localScale = Vector3.one;

                //player tags
                __instance.parkBoardPlayerTags[0].transform.parent.localPosition = Vector3.zero;
                __instance.parkBoardPlayerTags[0].transform.parent.localScale = Vector3.one;
            }
        }
    }
}
