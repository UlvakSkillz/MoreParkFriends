using MelonLoader;
using UnityEngine;
using RUMBLE.Environment;
using HarmonyLib;

[HarmonyPatch(typeof(ParkBoardGymVariant), "OnPlayerEnteredTrigger")]
public static class Patch
{
    private static void Prefix()
    {
        ParkBoardGymVariant parkBoardGymVariant = GameObject.Find("--------------LOGIC--------------/Heinhouser products/Parkboard").gameObject.GetComponent<ParkBoardGymVariant>();
        parkBoardGymVariant.hostPlayerCapacity *= 2;
    }
}

namespace MoreParkFriends
{
    public class MoreParkFriendsClass : MelonMod {}
}
