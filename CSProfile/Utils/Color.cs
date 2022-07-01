using System;
using UnityEngine;

namespace CSProfile.Utils;

public class Color
{

    public int B;

    public int G;
    public int R;

    public Color32 ToUnityColor()
    {
        return new Color32(Convert.ToByte(R), Convert.ToByte(G), Convert.ToByte(B), 155);
    }
}
