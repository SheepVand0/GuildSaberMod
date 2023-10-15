using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSSlider : XUISlider
    {
        protected GSSlider(string p_Name) : base(p_Name)
        {
            OnReady(OnCreation);
        }

        public static new GSSlider Make()
        {
            return new GSSlider("GuildSaberSlider");
        }

        private void OnCreation(CSlider p_Slider)
        {
            SetColor(Color.black.ColorWithAlpha(0.7f));
            GSText.PatchText(Element.transform.Find("SlidingArea").GetComponentInChildren<TextMeshProUGUI>());
        }
    }
}
