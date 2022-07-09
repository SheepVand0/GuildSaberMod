using UnityEngine;

namespace CSProfile;

/// <summary>
///     Monobehaviours (scripts) are added to GameObjects.
///     For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
/// </summary>
public class CSProfileController : MonoBehaviour
{
    private static CSProfileController Instance { get; set; }

    // These methods are automatically called by Unity, you should remove any you aren't using.

    #region Monobehaviour Messages

    /// <summary>
    ///     Only ever called once, mainly used to initialize variables.
    /// </summary>
    private void Awake()
    {
        // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
        //   and destroy any that are created while one already exists.
        if (Instance != null)
        {
            Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
            DestroyImmediate(this);
            return;
        }
        DontDestroyOnLoad(this); // Don't destroy this object on scene changes
        Instance = this;
        Plugin.Log?.Debug($"{name}: Awake()");
    }

    /// <summary>
    ///     Called when the script is being destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Plugin.Log?.Debug($"{name}: OnDestroy()");
        if (Instance == this)
            Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.
    }

    #endregion

}
