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

    public Color32 ToUnityColor()
    {
        return new Color32(Convert.ToByte(R), Convert.ToByte(G), Convert.ToByte(B), 155);
    }
}
