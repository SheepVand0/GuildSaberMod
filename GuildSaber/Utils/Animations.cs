using GuildSaber.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GuildSaber.Utils;

public class FloatAnimation : MonoBehaviour
{
    public event Action<float> OnChange;

    public event Action<float> OnFinished;

    private float m_Start;

    private float m_End;

    private bool m_Started;

    private float m_Duration = 0;

    private float m_ValueDuration = 0;

    private float m_StartTime = 0;

    public float GetStart() => m_Start;
    public float GetEnd() => m_End;
    public bool IsPlaying() => m_Started;
    public float Duration() => m_Duration;

    protected virtual void OnPlay()
    {

    }

    protected virtual void OnStop()
    {

    }

    protected virtual void OnInit()
    {

    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Init anim
    /// </summary>
    /// <param name="p_Start"></param>
    /// <param name="p_Value"></param>
    /// <param name="p_Duration"></param>
    public void Init(float p_Start, float p_Value, float p_Duration)
    {
        m_Start = p_Start;
        m_End = p_Value;
        m_ValueDuration = m_End - m_Start;
        m_Duration = p_Duration;
        OnInit();
    }

    ////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get value from current time, start and end
    /// </summary>
    public void Update()
    {
        if (m_Started == false) return;

        float l_Prct = (UnityEngine.Time.realtimeSinceStartup - m_StartTime) / m_Duration;

        float l_Value = m_Start + (m_ValueDuration * l_Prct);

        OnChange?.Invoke(l_Value);

        if (l_Prct > 1) { m_Started = false; OnFinished?.Invoke(l_Value); }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Start animation
    /// </summary>
    public void Play()
    {
        m_StartTime = UnityEngine.Time.realtimeSinceStartup;
        if (m_Duration == 0 || float.IsPositiveInfinity(m_Duration) || float.IsNegativeInfinity(m_Duration))
        {
            OnChange?.Invoke(m_End);
            OnFinished?.Invoke(m_End);
            GameObject.DestroyImmediate(gameObject);
            return;
        }

        m_Started = true;
        OnPlay();
    }

    /// <summary>
    /// Stop current animation
    /// </summary>
    public void Stop()
    {
        m_Started = false;
        OnStop();
    }

}

public class Vector3Animation : MonoBehaviour
{

    public event Action<Vector3> OnVectorChange;

    public event Action<Vector3> OnFinished;

    public Vector3 m_Start;

    public Vector3 m_End;

    Vector3 m_Current;

    public float m_Duration;

    private int m_FinishedAnimCount = 0;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    FloatAnimation m_XAnim;
    FloatAnimation m_YAnim;
    FloatAnimation m_ZAnim;

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Create an animation in GameObject if no existing, else return existing
    /// </summary>
    /// <param name="p_GameObject">Target GameObject</param>
    /// <param name="p_Animation">Returned animation</param>
    public static void AddAnim(GameObject p_GameObject, out Vector3Animation p_Animation)
    {
        Vector3Animation l_ExistingAnim = p_GameObject.GetComponent<Vector3Animation>();
        if (l_ExistingAnim != null)
            p_Animation = l_ExistingAnim;
        else
            p_Animation = p_GameObject.AddComponent<Vector3Animation>();
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Init animation
    /// </summary>
    /// <param name="p_Start">Start value</param>
    /// <param name="p_Value">End value</param>
    /// <param name="p_Duration">Animation duration</param>
    public void Init(Vector3 p_Start, Vector3 p_Value, float p_Duration)
    {
        m_FinishedAnimCount = 0;
        m_Start = p_Start;
        m_End = p_Value;

        m_Duration = p_Duration;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// If all float animations have finished, invoke OnFinished event
    /// </summary>
    private void CheckFinishedAnims()
    {
        if (m_FinishedAnimCount == 3)
            OnFinished?.Invoke(m_Current);
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Play animation
    /// </summary>
    public void Play()
    {
        if (m_XAnim == null)
        {
            m_XAnim = gameObject.AddComponent<FloatAnimation>();
            m_XAnim.OnChange += (p_Val) =>
            {
                m_Current = new Vector3(p_Val, m_Current.y, m_Current.z);
                OnVectorChange?.Invoke(m_Current);
            };
            m_XAnim.OnFinished += (p_Val) => { m_FinishedAnimCount += 1; CheckFinishedAnims(); };
        }

        if (m_YAnim == null)
        {
            m_YAnim = gameObject.AddComponent<FloatAnimation>();
            m_YAnim.OnChange += (p_Val) =>
            {
                m_Current = new Vector3(m_Current.x, p_Val, m_Current.z);
                OnVectorChange?.Invoke(m_Current);
            };
            m_YAnim.OnFinished += (p_Val) => { m_FinishedAnimCount += 1; CheckFinishedAnims(); };
        }

        if (m_ZAnim == null)
        {
            m_ZAnim = gameObject.AddComponent<FloatAnimation>();
            m_ZAnim.OnChange += (p_Val) =>
            {
                m_Current = new Vector3(m_Current.x, m_Current.y, p_Val);
                OnVectorChange?.Invoke(m_Current);
            };
            m_ZAnim.OnFinished += (p_Val) => { m_FinishedAnimCount += 1; CheckFinishedAnims(); };
        }

        m_XAnim.Init(m_Start.x, m_End.x, m_Duration);
        m_YAnim.Init(m_Start.y, m_End.y, m_Duration);
        m_ZAnim.Init(m_Start.z, m_End.z, m_Duration);

        m_XAnim.Play();
        m_YAnim.Play();
        m_ZAnim.Play();
    }

    /// <summary>
    /// Stop current animation
    /// </summary>
    public void Stop()
    {
        foreach (FloatAnimation l_Current in gameObject.GetComponents<FloatAnimation>())
            l_Current.Stop();
    }
}

internal class FastAnimator : MonoBehaviour
{
    internal static FastAnimator Instance;

    internal enum EAnimType
    {
        Float,
        Vector
    }

    internal struct FloatAnimKey
    {
        public float Value;
        public float Exponent;
        public float Time;

        public FloatAnimKey(float p_Value, float p_Time, float p_Exponent = 1)
        {
            Value = p_Value;
            Time = p_Time;
            Exponent = p_Exponent;
        }
    }

    internal struct FloatAnimData
    {
        public List<FloatAnimKey> Keys;
        public Action<float> Callback;
        public Action OnFinished;
        public FloatAnimKey NextKey;
        public FloatAnimKey ActualKey;
        public FloatAnimKey LastKey;
        public float AddDeltaTime;

        public FloatAnimData(List<FloatAnimKey> p_Keys, Action<float> p_Callback, Action p_OnFinished)
        {
            Keys = p_Keys;
            Callback = p_Callback;
            OnFinished = p_OnFinished;
            NextKey = new FloatAnimKey(p_Keys[0].Value, 0);
            ActualKey = NextKey;
            AddDeltaTime = UnityEngine.Time.realtimeSinceStartup;
            if (p_Keys.Any())
                LastKey = p_Keys.Last();
            else
                LastKey = default;
        }
    }

    internal struct Vector3AnimKey
    {
        public Vector3 Start;
        public Vector3 End;
        public Vector3 Exponents;
        public float Duration;

        public Vector3AnimKey(Vector3 p_Start, Vector3 p_End, float p_Duration)
        {
            Start = p_Start;
            End = p_End;
            Duration = p_Duration;
            Exponents = new Vector3(1, 1, 1);
        }

        public Vector3AnimKey(Vector3 p_Start, Vector3 p_End, float p_Duration, Vector3 p_Exponents)
        {
            Start = p_Start;
            End = p_End;
            Duration = p_Duration;
            Exponents = p_Exponents;
        }
    }

    internal struct Vector3AnimData
    {
        internal List<Vector3AnimKey> Keys;
        internal Action<float> Callback;
        internal Action OnFinished;

        public Vector3AnimData(List<Vector3AnimKey> p_Keys, Action<float> p_Callback, Action p_OnFinished)
        {
            Keys = p_Keys;
            Callback = p_Callback;
            OnFinished = p_OnFinished;
        }
    }

    ////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////

    protected List<FloatAnimData> m_FloatAnimations = new List<FloatAnimData>();
    protected List<Vector3AnimData> m_Vector3Animations = new List<Vector3AnimData>();

    protected List<FloatAnimData> m_FloatAnimationsToEnd = new List<FloatAnimData>();

    ////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////

    public static void Animate(List<FloatAnimKey> p_Keys, Action<float> p_Callback, Action p_OnFinished = null)
    {
        if (p_Keys.Count < 2)
        {
            throw new Exception("Not enough keys to run an animation, 2 required");
        }

        if (Instance == null)
        {
            Instance = new GameObject("GuildSaberFastAnimator").AddComponent<FastAnimator>();
            GameObject.DontDestroyOnLoad(Instance);
        }

        FloatAnimData l_NewAnimation = new FloatAnimData(p_Keys, p_Callback, p_OnFinished);
        Instance.m_FloatAnimations.Add(l_NewAnimation);
    }

    ////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////

    public void Update()
    {
        for (int l_i = 0; l_i < m_FloatAnimations.Count;l_i++)
        {
            var l_Item = m_FloatAnimations[l_i];
            ParseFloatAnimData(l_Item, l_Item.AddDeltaTime, UnityEngine.Time.realtimeSinceStartup, l_i);
        }

        if (!m_FloatAnimationsToEnd.Any()) return;

        foreach (var l_Item in m_FloatAnimationsToEnd)
        {
            m_FloatAnimations.Remove(l_Item);
        }
        m_FloatAnimationsToEnd.Clear();
    }

    ////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////

    private void ParseFloatAnimData(FloatAnimData p_FloatAnimData, float p_StartTime, float p_DeltaTime, int p_IndexInList)
    {
        float l_Time = p_DeltaTime - p_StartTime;

        if (l_Time > p_FloatAnimData.LastKey.Time)
        {
            if (p_FloatAnimData.OnFinished != null)
                p_FloatAnimData.OnFinished.Invoke();

            p_FloatAnimData.Callback.Invoke(p_FloatAnimData.LastKey.Value);

            m_FloatAnimationsToEnd.Add(p_FloatAnimData);

            return;
        }

        ////////////////////////////////////////////////

        if (p_FloatAnimData.NextKey.Time == 0 || l_Time > p_FloatAnimData.NextKey.Time)
        {
            int l_KeysCount = p_FloatAnimData.Keys.Count();
            var l_Keys = p_FloatAnimData.Keys;

            ////////////////////////////////////////////////

            for (int l_i = 0; l_i < l_KeysCount;l_i++)
            {
                if (l_Keys[l_i].Time > l_Time)
                {
                    p_FloatAnimData.ActualKey = p_FloatAnimData.NextKey;
                    p_FloatAnimData.NextKey = l_Keys[l_i];

                    m_FloatAnimations[p_IndexInList] = p_FloatAnimData;
                    goto FunctionBody;
                }
            }

            
        }

/*    */
FunctionBody:

        FloatAnimKey l_ActualKey = p_FloatAnimData.ActualKey;
        FloatAnimKey l_NextKey = p_FloatAnimData.NextKey;
        float l_KeyIntervalTime = l_Time - l_ActualKey.Time;
        float l_KeyIntervalDuration = l_NextKey.Time - l_ActualKey.Time;

        float l_Value = CalculateFloatValue(l_ActualKey.Value, l_NextKey.Value, l_ActualKey.Exponent, l_KeyIntervalTime, l_KeyIntervalDuration);

        p_FloatAnimData.Callback.Invoke(
            l_Value
        );
    }

    private float CalculateFloatValue(float p_Start, float p_End, float p_Exponent, float p_Time, float p_Duration)
    {
        return p_Start + ((float)Math.Pow((p_Time / p_Duration), p_Exponent) * (p_End - p_Start));
    }

}