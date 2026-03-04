using MelonLoader;
using UnityEngine;
using Il2CppRUMBLE.Environment;
using HarmonyLib;
using MoreParkFriends;
using RumbleModdingAPI.RMAPI;
using RumbleModUI;

[HarmonyPatch(typeof(ParkBoardGymVariant), "OnPlayerEnteredTrigger")]
public static class Patch
{
    private static void Prefix()
    {
        int multiplier = MoreParkFriendsClass.multiplier;
        ParkBoardGymVariant parkBoardGymVariant = GameObjects.Gym.INTERACTABLES.Parkboard.GetGameObject().GetComponent<ParkBoardGymVariant>();
        parkBoardGymVariant.hostPlayerCapacity *= multiplier;
    }
}

namespace MoreParkFriends
{
    public class MoreParkFriendsClass : MelonMod
    {
        public static int multiplier = 2;
        public static Mod MoreParkFriends = new Mod();

        public override void OnLateInitializeMelon()
        {
            MoreParkFriends.ModName = "More Park Friends";
            MoreParkFriends.ModVersion = "2.2.1";
            MoreParkFriends.SetFolder("MoreParkFriends");
            MoreParkFriends.AddToList("Multiplier", 2, "Changes Player Counts on the Park Board.", new Tags { });
            MoreParkFriends.GetFromFile();
            MoreParkFriends.ModSaved += Save;
            UI.instance.UI_Initialized += UIInit;
            Save();
        }

        private void Save()
        {
            if ((int)MoreParkFriends.Settings[0].SavedValue <= 0)
            {
                MoreParkFriends.Settings[0].Value = 1;
                MoreParkFriends.Settings[0].SavedValue = 1;
            }
            if (9 < (int)MoreParkFriends.Settings[0].SavedValue)
            {
                MoreParkFriends.Settings[0].Value = 9;
                MoreParkFriends.Settings[0].SavedValue = 9;
            }
            multiplier = (int)MoreParkFriends.Settings[0].SavedValue;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Gym")
            {
                GameObject textsParent = GameObjects.Gym.INTERACTABLES.Parkboard.RotatingScreen.HostPanel.PlayerCpapcity.TextandIcons.GetGameObject();
                for (int i = 1; i <= 5; i++)
                {
                    textsParent.transform.GetChild(i).GetComponent<Il2CppTMPro.TextMeshPro>().text = ((1 + i) * multiplier).ToString();
                }
            }
        }

        private static void UIInit()
        {
            UI.instance.AddMod(MoreParkFriends);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Gym")
            {
                if (multiplier <= 0)
                {
                    multiplier = 1;
                }
                if (9 < multiplier)
                {
                    multiplier = 9;
                }
                GameObject textsParent = GameObjects.Gym.INTERACTABLES.Parkboard.RotatingScreen.HostPanel.PlayerCpapcity.TextandIcons.GetGameObject();
                for (int i = 1; i <= 5; i++)
                {
                    textsParent.transform.GetChild(i).GetComponent<Il2CppTMPro.TextMeshPro>().text = ((1+i) * multiplier).ToString();
                }
            }
        }
    }
}
