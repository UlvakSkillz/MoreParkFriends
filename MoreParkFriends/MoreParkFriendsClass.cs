using MelonLoader;
using UnityEngine;
using RUMBLE.Environment;
using HarmonyLib;
using MoreParkFriends;

[HarmonyPatch(typeof(ParkBoardGymVariant), "OnPlayerEnteredTrigger")]
public static class Patch
{

    private static void Prefix()
    {
        int multiplier = MoreParkFriendsClass.multiplier;
        ParkBoardGymVariant parkBoardGymVariant = GameObject.Find("--------------LOGIC--------------/Heinhouser products/Parkboard").GetComponent<ParkBoardGymVariant>();
        parkBoardGymVariant.hostPlayerCapacity *= multiplier;
    }
}

namespace MoreParkFriends
{
    public class MoreParkFriendsClass : MelonMod 
    {
        public static int multiplier = int.Parse(System.IO.File.ReadAllText(@"UserData\MoreParkFriends\Multiplier.txt"));

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
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
                GameObject gameObject = GameObject.Find("--------------LOGIC--------------/Heinhouser products/Parkboard/RotatingScreen/HostPanel/Player Cpapcity/TextandIcons/");
                for (int i = 1; i <= 5; i++)
                {
                    gameObject.transform.GetChild(i).GetComponent<TMPro.TextMeshPro>().text = ((1+i) * multiplier).ToString();
                }
            }
        }
    }
}
