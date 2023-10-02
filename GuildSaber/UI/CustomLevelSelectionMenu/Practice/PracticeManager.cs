using GuildSaber.Logger;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.Practice
{
    internal class PracticeManager
    {
        public static void Init(float p_NJS)
        {
            var l_Controller = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();
            if (l_Controller == null) {
                GSLogger.Instance.Log("BeatmapObjectSpawnController is null", IPA.Logging.Logger.LogLevel.NoticeUp);
                return;
            }

            l_Controller.beatmapObjectSpawnMovementData.SetField("_moveSpeed", p_NJS);
        }

    }
}
