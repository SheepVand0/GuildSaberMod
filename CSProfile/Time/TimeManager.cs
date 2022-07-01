using CSProfile.UI.Card;
using UnityEngine;

namespace CSProfile.Time;

/// <summary>
///     Monobehaviours (scripts) are added to GameObjects.
///     For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
/// </summary>
public class TimeManager : MonoBehaviour
{
    public PlayerCardViewController m_CardControllerRef;

    public readonly OptimizedDateTime m_Time = new OptimizedDateTime();

    public void Awake()
    {
        m_Time.Init(0, 0, 0, 0, 0, 0);
    }

    public void Update()
    {
        if (m_CardControllerRef == null)
        {
            Plugin.Log.Error("Controller ref is null");
            return;
        }

        if (m_Time.AddSecondAndUpdateClock(UnityEngine.Time.deltaTime))
        {
            m_CardControllerRef.UpdateTime(m_Time);
        }
    }

    public void SetPlayerCardViewControllerRef(PlayerCardViewController p_Ref)
    {
        if (p_Ref != null)
            m_CardControllerRef = p_Ref;
    }
}