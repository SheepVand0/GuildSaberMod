using GuildSaber.UI.Card;
using UnityEngine;

namespace GuildSaber.Time;

/// <summary>
///     Monobehaviours (scripts) are added to GameObjects.
///     For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
/// </summary>
public class TimeManager : MonoBehaviour
{
    private PlayerCardViewController m_CardControllerRef;

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public OptimizedDateTime Time = new OptimizedDateTime();

    public void Awake()
    {
        Time.Init(0, 0, 0, 0, 0, 0);
    }

    public void Update()
    {
        if (m_CardControllerRef is null) return;

        if (Time.AddSecondAndUpdateClock(UnityEngine.Time.deltaTime))
            m_CardControllerRef.UpdateTime(Time);
    }

    public void SetPlayerCardViewControllerRef(PlayerCardViewController p_Ref)
    {
        m_CardControllerRef = p_Ref;
    }
}