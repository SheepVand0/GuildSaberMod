using GuildSaber.Logger;
using PlaylistManager.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.Utils
{
    internal class TextureUtils
    {

        public static Texture2D MakeCorrespondHeight(Texture2D p_Texture, Rect p_Rect)
        {
            if (p_Rect.y >= p_Texture.height) return p_Texture;

            UnityEngine.Color[] l_Texture = p_Texture.GetPixels(0, (int)(p_Texture.width - p_Rect.y)/2, p_Texture.width, (int)p_Rect.height);

            // ReSharper disable once PossibleLossOfFraction
            var l_ResultTexture = new Texture2D(p_Texture.width, (int)p_Rect.height);
            l_ResultTexture.SetPixels(l_Texture);
            l_ResultTexture.Apply();

            return l_ResultTexture;
        }

        public enum EGradientDirection
        {
            Horizontal,
            Vertical
        }

        public static Texture2D Gradient(Texture2D p_Texture, UnityEngine.Color p_Color1, UnityEngine.Color p_Color2, EGradientDirection p_Direction = EGradientDirection.Horizontal, bool p_Invert = false, bool p_UseAlpha = false)
        {
            Texture2D l_Origin = p_Texture;

            UnityEngine.Color l_FirstColor = (p_Invert) ? p_Color1 : p_Color2;
            UnityEngine.Color l_SecondColor = (p_Invert) ? p_Color2 : p_Color1;

            for (int l_X = 0; l_X < p_Texture.width;l_X++)
            {
                for (int l_Y = 0; l_Y < p_Texture.height;l_Y++)
                {
                    float l_Color2Multiplier = (float)(p_Direction == EGradientDirection.Horizontal ? l_X : l_Y) / (float)(p_Direction == EGradientDirection.Horizontal ? l_Origin.width : l_Origin.height);

                    float l_Alpha = (p_UseAlpha) ? (l_FirstColor.a + (l_SecondColor.a * l_Color2Multiplier)) / 2 : l_Origin.GetPixel(l_X, l_Y).a;


                    l_Origin.SetPixel(
                        l_X, l_Y,
                        new UnityEngine.Color(
                            (l_FirstColor.r + (l_SecondColor.r * l_Color2Multiplier)) / 2,
                            (l_FirstColor.g + (l_SecondColor.g * l_Color2Multiplier)) / 2,
                            (l_FirstColor.b + (l_SecondColor.b * l_Color2Multiplier)) / 2,
                            l_Alpha)
                        );
                }
            }

            l_Origin.Apply();

            File.WriteAllBytes("C:/ImageTest/tests.png", l_Origin.EncodeToPNG());

            return l_Origin;
        }

        public static Texture2D CreateRoundedTexture(Texture2D p_Origin, float p_Radius, int p_Offset = 0)
        {
            Texture2D l_Texture = p_Origin;

            for (int l_X = 0; l_X < p_Origin.width; l_X++)
            {
                for (int l_Y = 0; l_Y < p_Origin.height; l_Y++)
                {
                    Vector2 l_RadiusPoint;
                    //Corner Bottom Left
                    if (l_X < p_Origin.width / 2 && l_Y < (p_Origin.height / 2) && l_X < p_Radius && l_Y < p_Radius + p_Offset)
                    {
                        l_RadiusPoint = new Vector2(p_Radius, p_Radius + p_Offset);

                        if (Vector2.Distance(l_RadiusPoint, new Vector2(l_X, l_Y)) > p_Radius)
                        {
                            l_Texture.SetPixel(l_X, l_Y, UnityEngine.Color.red.ColorWithAlpha(0));
                        }

                        continue;
                    }

                    //Corner Bottom Right
                    if (l_X > p_Origin.width / 2 && l_Y < (p_Origin.height / 2) && l_X > p_Origin.width - p_Radius && l_Y < p_Radius + p_Offset)
                    {
                        l_RadiusPoint = new Vector2(p_Origin.width - p_Radius, p_Radius + p_Offset);

                        if (Vector2.Distance(l_RadiusPoint, new Vector2(l_X, l_Y)) > p_Radius)
                        {
                            l_Texture.SetPixel(l_X, l_Y, UnityEngine.Color.red.ColorWithAlpha(0));
                        }

                        continue;
                    }

                    //Corner Top Left
                    if (l_X < p_Origin.width / 2 && l_Y > (p_Origin.height / 2) && l_X < p_Radius && l_Y > p_Origin.height - p_Radius - p_Offset)
                    {
                        l_RadiusPoint = new Vector2(p_Radius, p_Origin.height - p_Radius - p_Offset);


                        if (Vector2.Distance(l_RadiusPoint, new Vector2(l_X, l_Y)) > p_Radius)
                        {
                            l_Texture.SetPixel(l_X, l_Y, UnityEngine.Color.red.ColorWithAlpha(0));
                        }

                        continue;
                    }

                    //Corner top right
                    if (l_X > p_Origin.width / 2 && l_Y > (p_Origin.height / 2) && l_X > p_Origin.width - p_Radius && l_Y > p_Origin.height - p_Radius - p_Offset)
                    {
                        l_RadiusPoint = new Vector2(p_Origin.width - p_Radius, p_Origin.height - p_Radius - p_Offset);

                        if (Vector2.Distance(l_RadiusPoint, new Vector2(l_X, l_Y)) > p_Radius)
                        {
                            l_Texture.SetPixel(l_X, l_Y, UnityEngine.Color.red.ColorWithAlpha(0));
                        }

                        continue;
                    }

                    if (l_Y > p_Origin.height - p_Offset || l_Y < p_Offset)
                    {
                        l_Texture.SetPixel(l_X, l_Y, UnityEngine.Color.red.ColorWithAlpha(0));
                    }
                }
            }

            l_Texture.Apply();
            return l_Texture;
        }

    }
}
