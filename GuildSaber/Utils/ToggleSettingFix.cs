using BeatSaberMarkupLanguage.Components.Settings;
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
    internal class ToggleSettingFix
    {
        public static void Setup(
      ToggleSetting p_Setting,
      BSMLAction p_Action,
      bool p_Value,
      bool p_RemoveLabel)
        {
            ((Component)p_Setting).gameObject.SetActive(false);
            p_Setting.Value = p_Value;
            if (p_Action != null)
                ((GenericSetting)p_Setting).onChange = p_Action;
            if (p_RemoveLabel)
            {
                GameObject.Destroy((GameObject)((Component)((TMP_Text)((IEnumerable<TextMeshProUGUI>)((Component)p_Setting).gameObject.GetComponentsInChildren<TextMeshProUGUI>()).ElementAt<TextMeshProUGUI>(0)).transform).gameObject);
                RectTransform child = ((Component)p_Setting).gameObject.transform.GetChild(1) as RectTransform;
                child.anchorMin = Vector2.zero;
                child.anchorMax = Vector2.one;
                child.sizeDelta = Vector2.one;
                ((Component)p_Setting).gameObject.GetComponent<LayoutElement>().preferredWidth = -1f;
            }
      ((Component)p_Setting).gameObject.SetActive(true);
        }

    }
}
