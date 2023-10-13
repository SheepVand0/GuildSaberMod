using BeatSaberPlus.SDK.Game;
using CP_SDK.XUI;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers
{
    internal class PracticeViewController : BeatSaberPlus.SDK.UI.ViewController<PracticeViewController>
    {

        protected IDifficultyBeatmap m_Beatmap;

        protected XUIVLayout m_MainLayout;

        protected XUISlider m_TimeSlider;
        protected XUISlider m_SpeedSlider;
        protected XUISlider m_NJSSlider;
        protected XUIToggle m_EnableBot;

        protected float m_Time;
        protected float m_SpeedMutliplier = 1;
        protected float m_NJS;

        protected SongPreviewPlayer m_PreviewAudio;

        public void SetBeatmap(IDifficultyBeatmap p_Beatmap)
        {
            m_Beatmap = p_Beatmap;

            if (m_PreviewAudio == null)
                m_PreviewAudio = MapDetails.m_MapPreviewAudio;

            m_TimeSlider.SetMaxValue(m_Beatmap.level.beatmapLevelData.audioClip.length - 0.01f);
            m_TimeSlider.SetValue(0);
            m_TimeSlider.SetIncrements(0.1f);
            m_NJS = m_Beatmap.noteJumpMovementSpeed;
            m_NJSSlider.SetValue(m_NJS);
            m_SpeedSlider.SetValue(1);
            //GSLogger.Instance.Log(m_Beatmap.level.beatmapLevelData.audioClip.length, IPA.Logging.Logger.LogLevel.InfoUp);
        }

        protected override void OnViewCreation()
        {
            Templates.FullRectLayout(
                XUIVLayout.Make(
                    XUIText.Make("Time : "),
                    XUISlider.Make()
                    .Bind(ref m_TimeSlider)
                    .SetInteger(false)
                    .OnValueChanged(OnTimeSliderChanged)
                    .SetFormatter(Formatters.SimpleTimeFormat),
                    XUIText.Make("Speed : "),
                    XUISlider.Make()
                    .Bind(ref m_SpeedSlider)
                    .SetInteger(false)
                    .SetMinValue(0.1f)
                    .SetMaxValue(3)
                    .SetIncrements(0.1f)
                    .SetFormatter(null)
                    .OnValueChanged(OnSpeedSliderChanged),
                    XUIText.Make("Note Jump Speed : "),
                    XUISlider.Make()
                    .Bind(ref m_NJSSlider)
                    .SetMinValue(1)
                    .SetMaxValue(50)
                    .SetIncrements(0.5f)
                    .SetInteger(false)
                    .OnValueChanged(OnNJSSliderChanged),
                    GSSecondaryButton.Make("Play", 24, 15).OnClick(() =>
                    {
                        CustomLevelSelectionMenuReferences.IsInPractice = true;
                        PlayerData l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
                        GSBeatmapUtils.PlaySong(m_Beatmap,
                            l_PlayerData.overrideEnvironmentSettings, l_PlayerData.colorSchemesSettings.GetSelectedColorScheme(),
                            l_PlayerData.gameplayModifiers, l_PlayerData.playerSpecificSettings, new PracticeSettings(m_Time, m_SpeedMutliplier), p_OnMapFinished: (x, y) =>
                            {
                                GSLogger.Instance.Log("Exiting practice mode", IPA.Logging.Logger.LogLevel.NoticeUp);
                                CustomLevelSelectionMenuReferences.IsInPractice = false;
                            });
                    })
                    ).SetWidth(100)
                )
                .Bind(ref m_MainLayout)
                .BuildUI(transform);
        }

        public float GetCustomNJS()
        {
            return m_NJS;
        }

        protected string SpeedSliderFormatter(float p_Value)
        {
            return $"{(p_Value * 100):00}";
        }

        protected void OnTimeSliderChanged(float p_Value)
        {
            m_Time = p_Value;
            m_PreviewAudio.CrossfadeTo(m_Beatmap.level.beatmapLevelData.audioClip, 1, m_Time, 10, null);
            /*m_PreviewAudio.Stop();
            m_PreviewAudio.time = p_Value;
            m_PreviewAudio.Play();*/
        }

        protected void OnSpeedSliderChanged(float p_Value)
        {
            m_SpeedMutliplier = p_Value;
        }

        protected void OnNJSSliderChanged(float p_Value)
        {
            m_NJS = p_Value;
        }

    }
}
