using MelonLoader;
using UnityEngine;
using Il2CppRUMBLE.Environment;
using HarmonyLib;
using MoreParkFriends;
using System.Collections;
using System.IO;

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
        public string FILEPATH = @"UserData\MoreParkFriends";
        public string FILENAME = @"Multiplier.txt";
        public static int multiplier = 2;

        public override void OnLateInitializeMelon()
        {
            MelonCoroutines.Start(CheckIfFileExists(FILEPATH, FILENAME));
        }

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
                    gameObject.transform.GetChild(i).GetComponent<Il2CppTMPro.TextMeshPro>().text = ((1+i) * multiplier).ToString();
                }
            }
        }

        public IEnumerator CheckIfFileExists(string filePath, string fileName)
        {
            if (!File.Exists($"{filePath}\\{fileName}"))
            {
                if (!Directory.Exists(filePath))
                {
                    MelonLogger.Msg($"Folder Not Found, Creating Folder: {filePath}");
                    Directory.CreateDirectory(filePath);
                }
                if (!File.Exists($"{filePath}\\{fileName}"))
                {
                    MelonLogger.Msg($"Creating File {filePath}\\{fileName}");
                    File.Create($"{filePath}\\{fileName}");
                }
                multiplier = 2;
                for (int i = 0; i < 60; i++) { yield return new WaitForFixedUpdate(); }
                string[] newFileText = new string[1];
                newFileText[0] = "2";
                File.WriteAllLines($"{filePath}\\{fileName}", newFileText);
            }
            else
            {
                multiplier = int.Parse(File.ReadAllLines($"{FILEPATH}\\{FILENAME}")[0]);
            }
            yield return null;
        }
    }
}
