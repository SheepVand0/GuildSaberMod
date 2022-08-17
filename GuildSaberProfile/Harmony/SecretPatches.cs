using HarmonyLib;
using TMPro;
using GuildSaberProfile.Configuration;
using System;
using IPA.Utilities;
using UnityEngine;

namespace GuildSaberProfile.Harmony
{
    [HarmonyPatch(typeof(FlyingObjectEffect), nameof(FlyingObjectEffect.InitAndPresent))]
    [HarmonyPriority(int.MinValue)]
    class FlyTextPatch
    {
        private static void Postfix(FlyingObjectEffect __instance/*, ref TValue __Result*/)
        {
                if (!PluginConfig.Instance.UwUMode) return;

                TextMeshPro l_Text = null;

                if (!Plugin.m_IsHsvInstalled)
                {
                    FlyingScoreEffect l_FlyingScore = (FlyingScoreEffect)__instance;
                    l_Text = l_FlyingScore.GetField<TextMeshPro, FlyingScoreEffect>("_text");
                }
                else
                {
                    TextMeshPro l_TempText = __instance.GetComponentInChildren<TextMeshPro>();
                    l_Text = new GameObject("HSText").AddComponent<TextMeshPro>();
                    l_Text.transform.SetParent(l_TempText.transform.parent, false);
                    l_Text.text = l_TempText.text;
                    l_Text.color = l_TempText.color;
                    l_TempText.gameObject.SetActive(false);
                }

                int l_Score = int.Parse(l_Text.text);

                if (l_Score == 115)
                    l_Text.text = "≧▽≦";
                else if (l_Score < 108 && l_Score > 100)
                    l_Text.text = "╯︿╰";
                else if (l_Score <= 100)
                    l_Text.text = "X﹏X";
        }
    }

    public static class HsvUtils
    {
        public static void Hide(this GameObject p_Gm)
        {
            p_Gm.SetActive(false);
        }
    }
}
