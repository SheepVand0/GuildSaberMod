using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace GuildSaber.Utils
{
    internal class ColorSettingsFix
    {

        public static void Setup(
          BeatSaberMarkupLanguage.Components.Settings.ColorSetting p_Setting,
          BSMLAction p_Action,
          UnityEngine.Color p_Value,
          bool p_RemoveLabel)
        {
            p_Setting.gameObject.SetActive(false);
            p_Value.a = 1f;
            p_Setting.CurrentColor = p_Value;
            if (p_Action != null)
                p_Setting.onChange = p_Action;
            p_Setting.updateOnChange = true;
            if (p_RemoveLabel)
            {
                UnityEngine.Object.Destroy((p_Setting.gameObject.GetComponentsInChildren<TextMeshProUGUI>()).ElementAt<TextMeshProUGUI>(0).transform.gameObject);
                RectTransform child = p_Setting.gameObject.transform.GetChild(1) as RectTransform;
                child.anchorMin = Vector2.zero;
                child.anchorMax = Vector2.one;
                child.sizeDelta = Vector2.one;
                p_Setting.gameObject.GetComponent<LayoutElement>().preferredWidth = -1f;
            }
            p_Setting.gameObject.SetActive(true);
        }

    }
}
