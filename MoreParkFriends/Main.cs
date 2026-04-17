using Il2CppPhoton.Pun;
using Il2CppRUMBLE.Environment;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Social;
using MelonLoader;
using RumbleModdingAPI.RMAPI; //allows for RumbleModdingAPI usage
using UnityEngine;
using UIFramework;

namespace MoreParkFriends
{
    public static class BuildInfo
    {
        public const string ModName = "More Park Friends";
        public const string ModVersion = "2.4.2";
        public const string Author = "UlvakSkillz";
    }

    public class Main : MelonMod
    {
        //Mod's variables
        internal static int playerCap = 1;
        internal static int additionsMade = 6;
        private static string currentScene = "Loader";

        public override void OnInitializeMelon()
        {
            Preferences.InitPrefs();
            UI.Register((MelonBase)this, Preferences.MoreParkFriendsCategory).OnModSaved += Save;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName; //stores current scene for later use
            additionsMade = 6; //resets to the default 6 on scene load
            if (currentScene == "Gym") { UpdateGymText(); } //if in Gym, Show Multiplier numbers on board
        }

        private void Save()
        {
            if (Preferences.IsPrefChanged(Preferences.PrefMultiplier)) //if user changed multiplier
            {
                if (currentScene == "Gym") { UpdateGymText(); } //if in Gym, Show Multiplier numbers on board
            }

            if (Preferences.IsPrefChanged(Preferences.PrefShowExpandedPlayers)) //if user changed if the board should expand
            {
                if (currentScene == "Park") { ScaleBoard(null, PlayerManager.instance.AllPlayers.Count); } //if in Park, Change the Board Scale
            }
            Preferences.StoreLastSavedPrefs();
        }

        private static void UpdateGymText()
        {
            GameObject textsParent = GameObjects.Gym.INTERACTABLES.Parkboard.RotatingScreen.HostPanel.PlayerCpapcity.TextandIcons.GetGameObject(); //get common texts parent
            for (int i = 1; i <= 5; i++) //loop each child
            {
                textsParent.transform.GetChild(i).GetComponent<Il2CppTMPro.TextMeshPro>().text = ((1 + i) * Preferences.PrefMultiplier.Value).ToString(); //update component text with new multiplier
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
                    __instance.hostIcons[i]?.transform.parent.GetChild(1).gameObject.SetActive(Preferences.PrefShowExpandedPlayers.Value && PhotonNetwork.IsMasterClient);
                    __instance.parkBoardPlayerTags[i]?.gameObject.SetActive(Preferences.PrefShowExpandedPlayers.Value);
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
            if (Preferences.PrefShowExpandedPlayers.Value) //if showEnabled is on in ModUI
            {
                //host & kick icons (scales parent called Addition)
                float posPlayerCountClamped = (11f / 30f) * (float)Math.Sqrt((Math.Max(0, playerCount - 6f)) / 12f);
                Melon<Main>.Logger.Msg("Clamped: " + posPlayerCountClamped);
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
