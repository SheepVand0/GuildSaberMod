using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CSProfile.Utils
{
    public class Color
    {
        public int R;

        public int G;

        public int B;

        public UnityEngine.Color32 ToUnityColor()
        {
            return new UnityEngine.Color32(Convert.ToByte(R), Convert.ToByte(G), Convert.ToByte(B), 155);
        }
    }
}
