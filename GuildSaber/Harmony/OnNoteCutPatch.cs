using System;
using System.Numerics;
using GuildSaber.Logger;
using HarmonyLib;

namespace GuildSaber.Harmony
{
    [HarmonyPatch(typeof(NoteController), "SendNoteWasCutEvent")]
    //[HarmonyPatch(typeof(GameNoteController), nameof(GameNoteController.HandleBigWasCutBySaber))]
    internal class OnNoteCutPatch
    {
        //private static void Postfix(GameNoteController __instance, Saber saber, Vector3 cutPoint, Quaternion orientation, Vector3 cutDirVec)
        private static void Postfix(NoteController __instance, in NoteCutInfo noteCutInfo)
        {

            GSLogger.Instance.Log("Notecutted", IPA.Logging.Logger.LogLevel.InfoUp);
            float l_X = noteCutInfo.saberDir.x;
            float l_Y = noteCutInfo.saberDir.y;
            float l_Hypotenus = (float)Math.Sqrt(Math.Pow(l_X, 2) + Math.Pow(l_Y, 2));
            float l_RotationFromCutDirVec = (float)Math.Acos(
                (0.5*(Math.Pow(l_Y, 2) + Math.Pow(l_Hypotenus, 2) - Math.Pow(l_X, 2))
                ) / l_Y * l_Hypotenus);

            GSLogger.Instance.Log(l_RotationFromCutDirVec, IPA.Logging.Logger.LogLevel.InfoUp);
            GSLogger.Instance.Log(__instance.worldRotation.z, IPA.Logging.Logger.LogLevel.InfoUp);

            if (__instance.worldRotation.z - l_RotationFromCutDirVec < __instance.worldRotation.z - 110 / 2 || __instance.worldRotation.z + l_RotationFromCutDirVec > __instance.worldRotation.z + 110 / 2)
                GSLogger.Instance.Log("Cheese", IPA.Logging.Logger.LogLevel.InfoUp);
        }

    }
}
