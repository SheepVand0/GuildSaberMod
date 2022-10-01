using BeatSaberPlus.SDK.UI;
using BeatSaberMarkupLanguage;
using System.Reflection;
using System.Collections.Generic;
using GuildSaber.UI.Components;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using UnityEngine.UI;
using GuildSaber.API;
using GuildSaber.Configuration;

namespace GuildSaber.UI.CustomLevelSelection
{
    public class CategorySelectionViewController : ViewController<CategorySelectionViewController>
    {
        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "GuildSaber.UI.CustomLevelSelection.View.CategorySelectionViewController.bsml");
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("CategoriesContainer")] HorizontalLayoutGroup m_CategoriesContainer = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        List<LevelSelectionCustomCategory> m_CustomCategories = new List<LevelSelectionCustomCategory>();

        public int m_SelectedGuild = GSConfig.Instance.SelectedGuild;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void OnViewActivation()
        {

        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal void SelectCat(string p_Name)
        {

        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal void RefreshCategories()
        {

        }
    }

    internal struct Category
    {
        public ApiCategory m_ApiCategory;
    }

    internal struct Level
    {
        public ApiRankingLevel m_ApiLevel;
        public List<CategoryMap> m_Maps;
    }

    internal struct CategoryMap
    {
        public CategoryMap(string p_Hash, IBeatmapLevel p_Level, List<IDifficultyBeatmap> p_DifficultiesBeatmap, List<BeatmapDifficulty> p_Difficulties)
        {
            m_Hash = p_Hash;
            m_Level = p_Level;
            m_DifficultyBeatmps = p_DifficultiesBeatmap;
            m_CategoryRankedDifficulties = p_Difficulties;
            Setup(p_Hash, m_CategoryRankedDifficulties);
        }

        internal string m_Hash { get; set; }
        internal IBeatmapLevel m_Level { get; set; }
        internal List<IDifficultyBeatmap> m_DifficultyBeatmps { get; set; }
        internal List<BeatmapDifficulty> m_CategoryRankedDifficulties { get; set; }
        public static CategoryMap Setup(string p_Hash, List<BeatmapDifficulty> p_CategoryRankedDifficulties)
        {
            IBeatmapLevel l_Level = null;
            List<IDifficultyBeatmap> l_DifficultiesBeatmap = new List<IDifficultyBeatmap>();

            BeatSaberPlus.SDK.Game.Levels.LoadSong($"custom_level_{p_Hash}", (p_Beatmap) =>
            {
                l_Level = p_Beatmap;

                foreach (var l_Diff in p_CategoryRankedDifficulties)
                    foreach (var l_Current in p_Beatmap.beatmapLevelData.difficultyBeatmapSets)
                        foreach (var l_DiffBeatmap in l_Current.difficultyBeatmaps)
                            if (l_DiffBeatmap.difficulty == l_Diff)
                                l_DifficultiesBeatmap.Add(l_DiffBeatmap);
            });
            return new CategoryMap(p_Hash, l_Level, l_DifficultiesBeatmap, p_CategoryRankedDifficulties);
        }
    }
}
