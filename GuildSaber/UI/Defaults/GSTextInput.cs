using CP_SDK.UI.Components;
using CP_SDK.UI.DefaultComponents;
using CP_SDK.XUI;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace GuildSaber.UI.Defaults
{
    internal class GSTextInput : XUITextInput
    {
        protected GSTextInput(string p_Name, string p_PlaceHolder) : base(p_Name, p_PlaceHolder)
        {
            OnReady(OnCreation);
        }

        public static new GSTextInput Make(string p_PlaceHolder)
        {
            return new GSTextInput("GuildSaberTextinput", p_PlaceHolder);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private void OnCreation(CTextInput p_TextInput)
        {
            var l_TextInput = (DefaultCTextInput)p_TextInput;


            var l_ValueText = l_TextInput.GetField<CText, DefaultCTextInput>("m_ValueText");
            var l_Background = l_TextInput.GetField<Image, DefaultCTextInput>("m_BG");

            GSText.PatchText(l_ValueText);
            l_Background.color = new UnityEngine.Color(0, 0, 0, 0.7f);
        }

    }
}
