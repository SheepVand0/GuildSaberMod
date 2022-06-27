using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CSProfile.UI;

namespace CSProfile.Time
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public class TimeManager : MonoBehaviour
    {
        PlayerCardViewController m_cardControllerRef = null;

        public OptimizedDateTime m_time = new OptimizedDateTime();

        public void Awake()
        {
            m_time.Init(0, 0, 0, 0, 0, 0);
        }

        public void Update()
        {
            if (m_cardControllerRef == null)
            {
                Plugin.Log.Error("Controller ref is null");
                return;
            }

            if (m_time.AddSecondAndUpdateclock(UnityEngine.Time.deltaTime) == true) {
                m_cardControllerRef.UpdateTime(m_time);
            }
        }

        public void SetPlayerCardViewControllerRef(PlayerCardViewController p_ref)
        {
            if (p_ref != null) 
                m_cardControllerRef = p_ref;
        }
    }
}
