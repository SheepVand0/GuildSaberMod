using CP_SDK.XUI;
using GuildSaber.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSButtonDropdown : GSSecondaryButton
    {
        protected GSButtonDropdown(List<string> p_Options) : base("GSButtonDropdown", "None", 15, 4, null)
        {
            OnClick(OnButtonClicked);
            if (p_Options != null)
            {
                m_Options = p_Options;
            }
        }

        public static GSButtonDropdown Make(List<string> p_Options)
        {
            return new GSButtonDropdown(p_Options);
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        protected List<string> m_Options = new List<string>();
        protected List<Action<string, int>> m_OnValueChangedActions = new List<Action<string, int>>();
        protected int m_SelectedIndex = 0;

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        public void OnValueChanged(Action<string, int> p_OnValueChangeAction)
        {
            m_OnValueChangedActions.Add(p_OnValueChangeAction);
        }

        public void SetOptions(List<string> p_Options)
        {
            m_Options = p_Options;

            SetSelectedIndex(0, false);
        }

        public void SetSelectedIndex(int p_Index, bool p_FireEvent = true)
        {
            if (m_Options.Count() == 0)
            {
                m_SelectedIndex = -1;
                SetText("None");
                return;
            }

            try
            {
                string l_Selected = m_Options.ElementAt(p_Index);
                m_SelectedIndex = p_Index;

                SetText(l_Selected);

                if (p_FireEvent == false) return;

                foreach (var l_Event in m_OnValueChangedActions)
                {
                    try
                    {
                        if (l_Event != null)
                        {
                            l_Event.Invoke(l_Selected, m_SelectedIndex);
                        }
                    }
                    catch (Exception p_E)
                    {
                        GSLogger.Instance.Error(p_E, nameof(GSButtonDropdown), $"{nameof(SetSelectedIndex)}_Events");
                    }
                }

            } catch (Exception p_E)
            {
                GSLogger.Instance.Error(p_E, nameof(GSButtonDropdown), nameof(SetSelectedIndex));
                SetText("None");
            }
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////

        private void OnButtonClicked()
        {
            int l_OptionsCount = m_Options.Count();
            if (l_OptionsCount == 1)
            {
                return;
            }

            if (m_SelectedIndex == l_OptionsCount - 1)
            {
                SetSelectedIndex(0);
                return;
            }
            SetSelectedIndex(m_SelectedIndex + 1);
        }
    }
}
