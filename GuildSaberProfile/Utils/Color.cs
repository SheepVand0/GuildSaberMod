using System;
using UnityEngine;

namespace GuildSaberProfile.Utils;

// ReSharper disable once ClassNeverInstantiated.Global
public class Color
{
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public int B;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public int G;
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnassignedField.Global
    public int R;

    public Color(int p_R, int p_G, int p_B)
    {
        R = p_R;
        G = p_G;
        B = p_B;
    }

    public Color32 ToUnityColor()
    {
        return new Color32(Convert.ToByte(R), Convert.ToByte(G), Convert.ToByte(B), 155);
    }

    public static Color ToGSProfileColor(UnityEngine.Color p_UnityColor)
    {
        return new Color((int)(p_UnityColor.r*255), (int)(p_UnityColor.g * 255), (int)(p_UnityColor.b * 255));
    }
}