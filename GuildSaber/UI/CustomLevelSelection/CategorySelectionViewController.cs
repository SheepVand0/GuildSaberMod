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
using System.Diagnostics;
using OVR.OpenVR;

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

        internal static List<Category> m_Categories = new List<Category>();

        public int m_SelectedGuild = GSConfig.Instance.SelectedGuild;

        public static int m_SelectedCategoryId = 0;

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
        public Category(ApiCategory p_Category)
        {
            m_ApiCategory = p_Category;
            m_Levels = default(List<Level>);
        }

        public Category(ApiCategory p_Category, List<Level> p_Levels)
        {
            m_ApiCategory = p_Category;
            m_Levels = p_Levels;
        }

        public ApiCategory m_ApiCategory;
        public List<Level> m_Levels;
    }

    internal struct Level
    {
        public Level(ApiRankingLevel p_Level)
        {
            m_ApiLevel = p_Level;
            m_Maps = default(List<CategoryMap>);
        }

        public Level(ApiRankingLevel p_Level, List<CategoryMap> p_Maps)
        {
            m_ApiLevel = p_Level;
            m_Maps = p_Maps;
        }

        public ApiRankingLevel m_ApiLevel;
        public List<CategoryMap> m_Maps;
    }

    internal struct CategoryMap
    {
        public CategoryMap(RankedDifficultyCollection.RankedMapData p_MapData, BeatmapDifficulty p_Difficulty)
        {
            m_MapData = p_MapData;
            m_Difficulty = p_Difficulty;
        }

        RankedDifficultyCollection.RankedMapData m_MapData { get; set; }
        BeatmapDifficulty m_Difficulty { get; set; }

        public IDifficultyBeatmap GetBeatmapDifficulty()
        {
            IBeatmapLevel l_Level = null;
            IDifficultyBeatmap l_DifficultyBeatmap = null;
            BeatmapDifficulty l_Difficulty = m_Difficulty;

            BeatSaberPlus.SDK.Game.Levels.LoadSong($"custom_level_{m_MapData.BeatSaverHash}", (p_Beatmap) =>
            {
                l_Level = p_Beatmap;

                    foreach (var l_Current in p_Beatmap.beatmapLevelData.difficultyBeatmapSets)
                        foreach (var l_DiffBeatmap in l_Current.difficultyBeatmaps)
                            if (l_DiffBeatmap.difficulty == l_Difficulty)
                                l_DifficultyBeatmap = l_DiffBeatmap;
            });
            return l_DifficultyBeatmap;
        }
    }
}
