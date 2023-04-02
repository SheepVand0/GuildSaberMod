using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace GuildSaber.Utils
{
    internal class SliderSettingsFix
    {
        public static void Setup(
          BeatSaberMarkupLanguage.Components.Settings.SliderSetting p_Setting,
          BSMLAction p_Action,
          BSMLAction p_Formatter,
          float p_Value,
          bool p_RemoveLabel,
          bool p_AddControls = false,
          Vector2 p_NewRectMin = default(Vector2),
          Vector2 p_NewRectMax = default(Vector2))
        {
            p_Setting.gameObject.SetActive(false);
            if (p_Formatter != null)
                p_Setting.formatter = p_Formatter;
            p_Setting.slider.value = p_Value;
            if (p_Action != null)
                p_Setting.onChange = p_Action;
            p_Setting.updateOnChange = true;
            if (p_RemoveLabel)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object)((IEnumerable<TextMeshProUGUI>)p_Setting.gameObject.GetComponentsInChildren<TextMeshProUGUI>()).ElementAt<TextMeshProUGUI>(0).transform.gameObject);
                RectTransform child = p_Setting.gameObject.transform.GetChild(1) as RectTransform;
                child.anchorMin = Vector2.zero;
                child.anchorMax = Vector2.one;
                child.sizeDelta = Vector2.one;
                p_Setting.gameObject.GetComponent<LayoutElement>().preferredWidth = -1f;
                if (p_AddControls)
                {
                    RectTransform rectTransform = p_Setting.gameObject.transform.Find("BSMLSlider") as RectTransform;
                    rectTransform.anchorMin = p_NewRectMin;
                    rectTransform.anchorMax = p_NewRectMax;
                    FormattedFloatListSettingsValueController settingsValueController = UnityEngine.Object.Instantiate<FormattedFloatListSettingsValueController>(((IEnumerable<FormattedFloatListSettingsValueController>)UnityEngine.Resources.FindObjectsOfTypeAll<FormattedFloatListSettingsValueController>()).First<FormattedFloatListSettingsValueController>((Func<FormattedFloatListSettingsValueController, bool>)(x => x.name == "VRRenderingScale")), p_Setting.gameObject.transform, false);
                    UnityEngine.UI.Button l_DecButton = ((IEnumerable<UnityEngine.UI.Button>)settingsValueController.transform.GetChild(1).GetComponentsInChildren<UnityEngine.UI.Button>()).First<UnityEngine.UI.Button>();
                    UnityEngine.UI.Button l_IncButton = ((IEnumerable<UnityEngine.UI.Button>)settingsValueController.transform.GetChild(1).GetComponentsInChildren<UnityEngine.UI.Button>()).Last<UnityEngine.UI.Button>();
                    l_DecButton.transform.SetParent(p_Setting.gameObject.transform, false);
                    l_DecButton.name = "BSP_DecButton";
                    l_IncButton.transform.SetParent(p_Setting.gameObject.transform, false);
                    l_IncButton.name = "BSP_IncButton";
                    l_IncButton.transform.SetAsFirstSibling();
                    l_DecButton.transform.SetAsFirstSibling();
                    foreach (Component component in settingsValueController.transform)
                        UnityEngine.Object.Destroy((UnityEngine.Object)component.gameObject);
                    UnityEngine.Object.Destroy((UnityEngine.Object)settingsValueController);
                    p_Setting.slider.valueDidChangeEvent += (Action<RangeValuesTextSlider, float>)((_, p_NewValue) =>
                    {
                        l_DecButton.interactable = (double)p_NewValue > (double)p_Setting.slider.minValue;
                        l_IncButton.interactable = (double)p_NewValue < (double)p_Setting.slider.maxValue;
                        p_Setting.ApplyValue();
                        p_Setting.ReceiveValue();
                    });
                    l_DecButton.interactable = (double)p_Setting.slider.value > (double)p_Setting.slider.minValue;
                    l_IncButton.interactable = (double)p_Setting.slider.value < (double)p_Setting.slider.maxValue;
                    l_DecButton.onClick.RemoveAllListeners();
                    l_DecButton.onClick.AddListener((UnityAction)(() =>
                    {
                        p_Setting.slider.value -= p_Setting.increments;
                        l_DecButton.interactable = (double)p_Setting.slider.value > (double)p_Setting.slider.minValue;
                        l_IncButton.interactable = (double)p_Setting.slider.value < (double)p_Setting.slider.maxValue;
                        p_Setting.slider.HandleNormalizedValueDidChange((TextSlider)p_Setting.slider, p_Setting.slider.normalizedValue);
                    }));
                    l_IncButton.onClick.RemoveAllListeners();
                    l_IncButton.onClick.AddListener((UnityAction)(() =>
                    {
                        p_Setting.slider.value += p_Setting.increments;
                        l_DecButton.interactable = (double)p_Setting.slider.value > (double)p_Setting.slider.minValue;
                        l_IncButton.interactable = (double)p_Setting.slider.value < (double)p_Setting.slider.maxValue;
                        p_Setting.slider.HandleNormalizedValueDidChange((TextSlider)p_Setting.slider, p_Setting.slider.normalizedValue);
                    }));
                }
            }
            p_Setting.gameObject.SetActive(true);
        }

        public static void SetInteractable(BeatSaberMarkupLanguage.Components.Settings.SliderSetting p_Setting, bool p_Interactable)
        {
            if (p_Setting.slider.interactable == p_Interactable)
                return;
            p_Setting.gameObject.SetActive(false);
            p_Setting.slider.interactable = p_Interactable;
            if ((bool)(UnityEngine.Object)p_Setting.gameObject.transform.GetChild(2).Find("BG"))
                p_Setting.gameObject.transform.GetChild(2).Find("BG").gameObject.SetActive(p_Interactable);
            UnityEngine.UI.Button component1 = p_Setting.gameObject.transform.Find("BSP_DecButton")?.GetComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.Button component2 = p_Setting.gameObject.transform.Find("BSP_IncButton")?.GetComponent<UnityEngine.UI.Button>();
            if ((UnityEngine.Object)component1 != (UnityEngine.Object)null)
                component1.interactable = p_Interactable && (double)p_Setting.slider.value > (double)p_Setting.slider.minValue;
            if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
                component2.interactable = p_Interactable && (double)p_Setting.slider.value < (double)p_Setting.slider.maxValue;
            p_Setting.gameObject.SetActive(true);
        }

        public static void SetValue(BeatSaberMarkupLanguage.Components.Settings.SliderSetting p_Setting, float p_Value)
        {
            p_Setting.slider.value = p_Value;
            p_Setting.ApplyValue();
            p_Setting.ReceiveValue();
        }
    }
}
