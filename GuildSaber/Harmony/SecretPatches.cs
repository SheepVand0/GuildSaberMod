using GuildSaber.Configuration;
using HarmonyLib;
using IPA.Utilities;
using Polyglot;
using TMPro;

namespace GuildSaber.Harmony
{

    [HarmonyPatch(typeof(ComboUIController), nameof(ComboUIController.Start))]
    public class ComboPatch
    {
        private static void Prefix(ComboUIController __instance)
        {
            if (!GSConfig.Instance.UwUMode) return;

            __instance.GetComponentInChildren<TextMeshProUGUI>().text = "COMBWO";
        }
    }

    [HarmonyPatch(typeof(GameplayModifierToggle), nameof(GameplayModifierToggle.Start))]
    public class ModifierPatch
    {
        private static void Postfix(GameplayModifierToggle __instance)
        {
            TextMeshProUGUI l_Text = __instance.GetField<TextMeshProUGUI, GameplayModifierToggle>("_nameText");

            if (!GSConfig.Instance.UwUMode) { return; }

            if (l_Text.text == Localization.Get("MODIFIER_NO_FAIL_ON_0_ENERGY"))
                l_Text.text = "Nowo Fwail";
            else if (l_Text.text == Localization.Get("MODIFIER_NO_BOMBS"))
                l_Text.text = "Nowo Bwombs";
        }
    }
}
