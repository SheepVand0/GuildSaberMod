using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers
{
    internal class MapDetails : XUIVLayout
    {
        protected MapDetails(params IXUIElement[] p_Childs) : base("Map Details", p_Childs)
        {
            OnReady(OnLayoutReady);
        }

        protected XUIText m_MapName;
        protected XUIText m_MapAuthor;
        protected XUIText m_MapDuration;
        protected XUIImage m_MapCover;
        protected GSSecondaryButton m_PracticeButton;
        protected GSSecondaryButton m_PlayButton;

        public static MapDetails Make()
        {
            XUIImage l_Cover = XUIImage.Make();
            l_Cover.SetWidth(20).SetHeight(20);

            XUIText l_MapName = XUIText.Make(string.Empty);
            XUIText l_MapAuthor = XUIText.Make(string.Empty);
            XUIText l_MapDuration = XUIText.Make(string.Empty);

            XUIHLayout l_NamesLayout = XUIHLayout.Make(
                l_Cover,
                XUIVLayout.Make(
                    XUIHLayout.Make(
                            l_MapName,
                            l_MapDuration
                        ),
                    l_MapAuthor
                    )
                )
                .SetHeight(20)
                .SetWidth(50);

            return new MapDetails();
        }

        public MapDetails Bind(ref MapDetails p_Ref)
        {
            p_Ref = this;
            return this;
        }

        private void OnLayoutReady(CVLayout p_Layout)
        {

            XUIHLayout.Make(
                XUIImage.Make()
                    .SetWidth(20)
                    .SetHeight(20)
                    .Bind(ref m_MapCover),
                XUIVLayout.Make(
                    XUIHLayout.Make(
                            XUIText.Make(string.Empty)
                                .Bind(ref m_MapName),
                            XUIText.Make(string.Empty)
                                .Bind(ref m_MapDuration)
                        ),
                    XUIText.Make(string.Empty)
                        .Bind(ref m_MapAuthor)
                )
            ).SetWidth(50)
             .SetHeight(20)
             .BuildUI(Element.transform);

            XUIHLayout.Make(
                GSSecondaryButton.Make("Practice", 24, 15),
                GSSecondaryButton.Make("Play", 24, 15)
            ).BuildUI(Element.transform);
        }

        protected IDifficultyBeatmap m_Beatmap;

        static string DurationFormat(float p_Duration)
        {
            int l_Minutes = (int)(p_Duration / 60);
            int l_Hours = (int)(p_Duration / (60*60));
            int l_Seconds = (int)p_Duration - (l_Minutes * 60) - (l_Hours * 60);

            string l_SHours = (l_Hours > 0) ? $"{l_Hours}:" : string.Empty;

            return $"{l_SHours}{l_Minutes}{l_Seconds}";
        }

        public MapDetails SetMap(IDifficultyBeatmap p_Beatmap)
        {
            m_Beatmap = p_Beatmap;
            OnReady(async x =>
            {
                m_MapName.SetText(p_Beatmap.level.songName);
                m_MapAuthor.SetText(p_Beatmap.level.songAuthorName);
                m_MapDuration.SetText(DurationFormat(p_Beatmap.level.songDuration));

                Sprite l_Sprite = await p_Beatmap.level.GetCoverImageAsync(new CancellationToken());
                Texture2D l_Tex = l_Sprite.texture;
                l_Tex = TextureUtils.CreateRoundedTexture(l_Tex, l_Tex.width * 0.01f);
                m_MapCover.SetSprite(Sprite.Create(l_Tex, new Rect(0, 0, l_Tex.width, l_Tex.height), new Vector2()));
            });
            return this;
        }
    }
}
