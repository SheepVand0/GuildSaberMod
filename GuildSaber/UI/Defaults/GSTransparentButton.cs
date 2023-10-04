using CP_SDK.UI.Components;
using CP_SDK.XUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI.Defaults
{
    internal class GSTransparentButton : XUISecondaryButton
    {
        protected GSTransparentButton(string p_Name, string p_Label, Action p_OnClick = null) : base(p_Name, p_Label, p_OnClick)
        {
            OnReady(OnCreation);
        }

        public static new GSTransparentButton Make(string p_Label, Action p_OnClick = null)
        {
            return new GSTransparentButton("GuildSaberTransparentButton", p_Label, p_OnClick);
        }

        private void OnCreation(CSecondaryButton p_Button)
        {
            Element.SetBackgroundColor(new UnityEngine.Color(0, 0, 0, 0));
        }
    }
}
