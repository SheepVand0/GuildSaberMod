using HarmonyLib;
using TMPro;
using GuildSaberProfile.Configuration;
using System;
using IPA.Utilities;

namespace GuildSaberProfile.Harmony
{
    [HarmonyPatch(typeof(FlyingScoreEffect), nameof(FlyingScoreEffect.HandleCutScoreBufferDidFinish))]
    class FlyTextPatch
    {
        //[HarmonyPriority(int.MinValue)]
        private static void Prefix()
        {
            Plugin.Log.Info("Setting");
            if (!PluginConfig.Instance.UwUMode) return;

          /*  var l_Text = ____flyingScoreEffectPrefab.GetField<TextMeshPro, FlyingScoreEffect>("_text");

                if (l_Text.text == "115")
                {
                    l_Text.text = "≧▽≦";
                }
                else if (int.Parse(l_Text.text) < 108)
                {
                    Plugin.Log.Info("Setting");
                    l_Text.text = "＞︿＜";
                }*/
        }
    }
}
