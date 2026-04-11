using HarmonyLib; //allows Harmony Usage
using Il2CppRUMBLE.Environment;
using Il2CppRUMBLE.Interactions.InteractionBase;
using Il2CppRUMBLE.Social;
using Il2CppRUMBLE.Social.Phone;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreParkFriends
{
    //Runs as the player enters the Gondola
    [HarmonyPatch(typeof(ParkBoardGymVariant), nameof(ParkBoardGymVariant.OnPlayerEnteredTrigger), new Type[] { })]
    public static class OnPlayerEnteredTriggerPatch
    {
        private static void Prefix(ref ParkBoardGymVariant __instance) //Prefix is important so it can be changed before leaving the Gym
        {
            __instance.hostPlayerCapacity *= MoreParkFriendsClass.multiplier; //updates the Hosting Player Capacity value
        }
    }

    //Runs when a player Joins/Leaves (shoot updates when leaving too)
    [HarmonyPatch(typeof(ParkBoardParkVariant), nameof(ParkBoardParkVariant.UpdatePlayerList), new Type[] { })]
    public static class UpdatePlayerListPatch
    {
        private static void Postfix(ref ParkBoardParkVariant __instance)
        {
            MoreParkFriendsClass.UpdateParkBoardPlayerTags(__instance); //toggles on/off Player Tags and updates info
        }
    }

    //Runs to Setup ParkBoard Info
    [HarmonyPatch(typeof(ParkBoardParkVariant), nameof(ParkBoardParkVariant.Initialize), new Type[] { typeof(ParkBoard) })]
    public static class InitializePatch
    {
        private static void Postfix(ref ParkBoardParkVariant __instance, ParkBoard owner)
        {
            //grabs stored lists I want to add to
            List<GameObject> hostIcons = ListConverter<GameObject>(__instance.hostIcons);
            List<InteractionButton> kickButtons = ListConverter<InteractionButton>(__instance.kickButtons);
            List<PlayerTag> playerTags = ListConverter<PlayerTag>(__instance.parkBoardPlayerTags);

            //variables
            int nameNumber = MoreParkFriendsClass.additionsMade;
            float yOffsetNewAddition = -1.1f;
            float yOffsetNewPlayerTag = -0.649f;
            float offsetChangePerStep = -0.18f;
            MoreParkFriendsClass.playerCap = int.Parse(__instance.currentParkPlayerCapText.text.Split('/')[1]);

            while (nameNumber < MoreParkFriendsClass.playerCap) //Creates a Tag Spot for each potential player (player cap will be total)
            {
                GameObject newAddition = GameObject.Instantiate(hostIcons[0].transform.parent.gameObject); //create new Host/Kick GameObject
                newAddition.name = $"Addition ({nameNumber})"; //name it
                newAddition.transform.SetParent(hostIcons[0].transform.parent.parent); //re-parent it
                newAddition.transform.localPosition = new Vector3(0f, yOffsetNewAddition, 0f); //move it
                newAddition.transform.localRotation = Quaternion.identity; //rotate it
                newAddition.transform.localScale = hostIcons[0].transform.parent.transform.localScale; //scale it

                GameObject playerTagGO = GameObject.Instantiate(__instance.parkBoardPlayerTags[0].gameObject); //create new PlayerTag spot
                playerTagGO.name = $"Player Tag 2.0 ({nameNumber})"; //name it
                playerTagGO.transform.SetParent(__instance.parkBoardPlayerTags[0].gameObject.transform.parent); //re-parent it
                playerTagGO.transform.localPosition = new Vector3(-0.06f, yOffsetNewPlayerTag, -0.053f); //move it
                playerTagGO.transform.localRotation = Quaternion.identity; //rotate it
                playerTagGO.transform.localScale = __instance.parkBoardPlayerTags[0].transform.localScale; //scale it

                PlayerTag newPlayerTag = playerTagGO.GetComponent<PlayerTag>(); //store the component for multiple uses
                newPlayerTag.Initialize(new UserData("", "", "Name", 0)); //initialize it with default info so it mimicks the others

                //add new items to stored lists
                hostIcons.Add(newAddition.transform.GetChild(0).gameObject);
                kickButtons.Add(newAddition.transform.GetChild(1).GetComponent<InteractionButton>());
                playerTags.Add(newPlayerTag);

                //increase incremental variables
                nameNumber++;
                yOffsetNewAddition += offsetChangePerStep;
                yOffsetNewPlayerTag += offsetChangePerStep;
            } //all potential tags created and placed when this exits

            //give the updated lists back to the original lists
            __instance.hostIcons = ListConverter<GameObject>(hostIcons);
            __instance.kickButtons = ListConverter<InteractionButton>(kickButtons);
            __instance.parkBoardPlayerTags = ListConverter<PlayerTag>(playerTags);
            MoreParkFriendsClass.UpdateParkBoardPlayerTags(__instance); //Controls the New Items starting state
        }

        private static List<T> ListConverter<T>(T[] list)
        {
            List<T> newList = new List<T>();
            foreach (var item in list) { newList.Add(item); }
            return newList;
        }

        private static T[] ListConverter<T>(List<T> list)
        {
            T[] newList = new T[list.Count];
            for(int i = 0; i < list.Count; i++) { newList[i] = list[i]; }
            return newList;
        }
    }
}
