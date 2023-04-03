using System;
using UnityEngine;

namespace GuildSaber.Utils.DiscordColor;

// ReSharper disable once ClassNeverInstantiated.Global
public class Color
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public int B;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public int G;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public int R;
    public int RawValue;
    public Color(int p_R, int p_G, int p_B)
    {
        R = p_R;
        G = p_G;
        B = p_B;
    }

    public Color32 ToUnityColor32() { return new Color32(Convert.ToByte(R), Convert.ToByte(G), Convert.ToByte(B), 255); }

    public UnityEngine.Color ToUnityColor() { return new UnityEngine.Color((float)R / 255, (float)G / 255, (float)B / 255, 1); }
}

public static class ColorUtils
{
    public static Color ToGSProfileColor(this UnityEngine.Color p_UnityColor) { return new Color((int)(p_UnityColor.r * 255), (int)(p_UnityColor.g * 255), (int)(p_UnityColor.b * 255)); }
}