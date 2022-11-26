using System.Collections.Generic;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberPlus.SDK.UI;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.Utils;
using HMUI;
using TMPro;

// ReSharper disable once CheckNamespace
namespace GuildSaber.UI.CustomLevelSelectionMenu
{
    internal class GuildDescriptionLine
    {
        private string Line { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent(("Line"))] private readonly TextMeshProUGUI m_Line = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public GuildDescriptionLine(string p_Line)
        {
            Line = p_Line;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIAction("#post-parse")]
        private void PostParse()
        {
            m_Line.text = Line;
        }

    }

    internal class GuildDescriptionViewController : ViewController<GuildDescriptionViewController>
    {
        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(),
                $"{GuildSelectionMenu.VIEW_CONTROLLERS_PATH}.GuildDescription.bsml");
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent(("GuildName"))] private readonly TextMeshProUGUI m_GuildName = null;
        [UIComponent("DescriptionList")] private readonly CustomCellListTableData m_LineList = null;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once CollectionNeverQueried.Local
        [UIValue("DescriptionLines")] private List<object> m_DescriptionLines = new List<object>();

        private bool m_Parsed = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void OnViewCreation()
        {
            m_Parsed = true;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set description
        /// </summary>
        /// <param name="p_Name">Guild Name</param>
        /// <param name="p_Description">Guild Description</param>
        public async void SetNameAndDescription(string p_Name, string p_Description)
        {
            await WaitUtils.Wait(() => m_Parsed, 1, 20, p_CodeLine: 72);

            m_GuildName.text = p_Name;

            List<string> l_Lines = new List<string>();

            /// Transforming original string description in multiple lines (32 char for each line)

            bool l_MoveNext = false;
            while (!l_MoveNext)
            {
                for (int l_i = 0; l_i < p_Description.Length; l_i++)
                {
                    if (l_i == p_Description.Length)
                    {
                        l_Lines.Add(p_Description);
                        l_MoveNext = true;
                        break;
                    }

                    if (l_i != 32)
                        continue;

                    string l_Value = p_Description.Substring(0, 32);
                    l_Lines.Add(l_Value);
                    p_Description = p_Description.Remove(0, 32);
                    break;
                }
            }

            /// Transforming it into TextMeshProUGUI

            m_DescriptionLines.Clear();

            l_Lines.ForEach((p_Value) => { m_DescriptionLines.Add(new GuildDescriptionLine(p_Value)); });

            m_LineList.tableView.ReloadData();
        }

    }
}
