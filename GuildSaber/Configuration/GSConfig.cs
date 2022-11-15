using System.Runtime.CompilerServices;
using CP_SDK;
using CP_SDK.Config;
using IPA.Config.Stores;
using Newtonsoft.Json;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace GuildSaber.Configuration;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class GSConfig : JsonConfig<GSConfig>
{
    internal static class ConfigDefaults
    {
        internal static readonly Vector3    DefaultCardPosition       = new Vector3(0.0f, 0.02f, 1.0f);
        internal static readonly Quaternion DefaultCardRotation       = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        internal static readonly Vector3    DefaultInGameCardPosition = new Vector3(1.8f, 1, -1.5f);
        internal static readonly Quaternion DefaultInGameCardRotation = Quaternion.Euler(20, 135, 0);
    }

    [JsonProperty] internal bool Enabled = true;
    [JsonProperty] internal bool CardEnabled = true;
    [JsonProperty] internal bool LeaderboardEnabled = true;

    [JsonProperty] internal bool ShowCardInMenu = true;
    [JsonProperty] internal bool ShowCardInGame = true;

    [JsonProperty] internal SerializableVector3    CardPosition       = SerializableVector3.FromUnityVector3(ConfigDefaults.DefaultCardPosition);
    [JsonProperty] internal SerializableQuaternion CardRotation       = SerializableQuaternion.FromUnityQuat(ConfigDefaults.DefaultCardRotation);
    [JsonProperty] internal SerializableVector3    InGameCardPosition = SerializableVector3.FromUnityVector3(ConfigDefaults.DefaultInGameCardPosition);
    [JsonProperty] internal SerializableQuaternion InGameCardRotation = SerializableQuaternion.FromUnityQuat(ConfigDefaults.DefaultInGameCardRotation);

    [JsonProperty] internal bool ShowSettingsModal = true;
    [JsonProperty] internal bool CardHandleVisible = false;
    [JsonProperty] internal bool ShowDetailsLevels = true;
    [JsonProperty] internal bool ShowPlayTime      = true;

    [JsonProperty] internal int SelectedGuild = 0;

    [JsonProperty] internal Color CustomColor = Color.red;
    [JsonProperty] internal bool  UwUMode     = false;

    public override string GetRelativePath()
        => $"{ChatPlexSDK.ProductName}/GuildSaber/Config";

    protected override void OnInit(bool p_OnCreation)
    {
        Save();
    }
}

internal class SerializableVector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public SerializableVector3(float p_X, float p_Y, float p_Z)
    {
        x = p_X;
        y = p_Y;
        z = p_Z;
    }

    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }

    public static SerializableVector3 FromUnityVector3(Vector3 p_Vector)
    {
        return new SerializableVector3(p_Vector.x, p_Vector.y, p_Vector.z);
    }
}
internal class SerializableQuaternion
{
    SerializableVector3 Euler;

    public SerializableQuaternion(SerializableVector3 p_Euler)
    {
        Euler = p_Euler;
    }

    public Quaternion ToUnityQuat()
    {
        return Quaternion.Euler(Euler.x, Euler.y, Euler.z);
    }

    public static SerializableQuaternion FromUnityQuat(Quaternion p_Quat)
    {
        return new SerializableQuaternion(SerializableVector3.FromUnityVector3(p_Quat.eulerAngles));
    }
}

internal class SerializableColor
{
    public float r { get; set; }
    public float g { get; set; }
    public float b { get; set; }
    public float a { get; set; }

    public SerializableColor(float p_R, float p_G, float p_B, float p_A)
    {
        r = p_R;
        g = p_G;
        b = p_B;
        a = p_A;
    }

    public Color ToUnityColor()
    {
        return new Color(r, g, b, 1);
    }

    public static SerializableColor ToSerializableColor(Color p_Color)
    {
        return new SerializableColor(p_Color.r, p_Color.g, p_Color.b, 1);
    }
}
