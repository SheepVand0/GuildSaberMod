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

            return l_Origin;
        }

        public static Texture2D CreateRoundedTexture(Texture2D p_Origin, float p_Radius, int p_Offset = 0, bool p_PushPixels = false)
        {
            Texture2D l_Texture = p_Origin;

            for (int l_i = 0; l_i < 4; l_i++)
            {
                for (int l_X = 0; l_X < p_Radius; l_X++)
                {
                    bool l_Moved = false;

                    for (int l_Y = 0; l_Y < p_Radius; l_Y++)
                    {
                        /// Corner Bottom Left
                        if (l_i == 0)
                        {
                            Vector2 l_Point = new Vector2(l_X, l_Y + p_Offset);
                            Vector2 l_RadiusPoint = new Vector2(p_Radius, p_Radius + p_Offset);

                            if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                            {
                                l_Moved = true;
                                l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, UnityEngine.Color.white.ColorWithAlpha(0));
                            }

                            if (p_PushPixels && !l_Moved)
                            {
                                UnityEngine.Color l_PixelColor = l_Texture.GetPixel((int)l_Point.x, (int)l_Point.y);
                                l_Texture.SetPixel((int)(p_Radius + l_Point.x), (int)(p_Radius + l_Point.y), l_PixelColor);
                            }

                            continue;
                        }

                        /// Corner Bottom Right
                        if (l_i == 1)
                        {
                            Vector2 l_Point = new Vector2(l_X + (l_Texture.width - p_Radius), l_Y + p_Offset);
                            Vector2 l_RadiusPoint = new Vector2(l_Texture.width - p_Radius, p_Radius + p_Offset);

                            if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                            {
                                l_Moved = true;
                                l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, UnityEngine.Color.white.ColorWithAlpha(0));
                            }

                            if (p_PushPixels)
                            {
                                UnityEngine.Color l_PixelColor = l_Texture.GetPixel((int)l_Point.x, (int)l_Point.y);
                                l_Texture.SetPixel((int)((l_Texture.width - p_Radius * 2) + l_Point.x), (int)(p_Radius + (p_Radius - l_Point.y)), l_PixelColor.ColorWithAlpha(1));
                            }

                            continue;
                        }

                        /// Corner Top Left
                        if (l_i == 2)
                        {
                            Vector2 l_Point = new Vector2(l_X, l_Y + (l_Texture.height - p_Radius - p_Offset));
                            Vector2 l_RadiusPoint = new Vector2(p_Radius, (l_Texture.height - p_Radius - p_Offset));

                            if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                            {
                                l_Moved = true;
                                l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, UnityEngine.Color.white.ColorWithAlpha(0));
                            }

                            if (p_PushPixels)
                            {
                                UnityEngine.Color l_PixelColor = l_Texture.GetPixel((int)l_Point.x, (int)l_Point.y);
                                l_Texture.SetPixel((int)(p_Radius + (p_Radius - l_Point.x)), (int)((l_Texture.height - p_Radius * 2) + l_Point.y), l_PixelColor.ColorWithAlpha(1));
                            }
                        }

                        /// Corner Top Right
                        if (l_i == 3)
                        {
                            Vector2 l_Point = new Vector2(l_X + (l_Texture.width - p_Radius), l_Y + (l_Texture.height - p_Radius - p_Offset));
                            Vector2 l_RadiusPoint = new Vector2(l_Texture.width - p_Radius, l_Texture.height - p_Radius - p_Offset);

                            if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                            {
                                l_Moved = true;
                                l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, UnityEngine.Color.white.ColorWithAlpha(0));
                            }

                            if (p_PushPixels)
                            {
                                UnityEngine.Color l_PixelColor = l_Texture.GetPixel((int)l_Point.x, (int)l_Point.y);
                                l_Texture.SetPixel((int)((l_Texture.width - p_Radius * 2) + l_Point.x), (int)((l_Texture.height - p_Radius * 2) + l_Point.y), l_PixelColor.ColorWithAlpha(1));
                            }

                        }

                    }
                }
            }

            if (p_Offset > 0)
            {
                for (int l_i = 0; l_i < 2; l_i++)
                {
                    for (int l_X = 0; l_X < l_Texture.width; l_X++)
                    {
                        for (int l_Y = 0; l_Y < p_Offset; l_Y++)
                        {
                            if (l_i == 0)
                            {
                                l_Texture.SetPixel(l_X, l_Y, UnityEngine.Color.white.ColorWithAlpha(0));
                                continue;
                            }

                            if (l_i == 1)
                            {
                                l_Texture.SetPixel(l_X, l_Y + (p_Offset + (l_Texture.height - p_Offset * 2)), UnityEngine.Color.white.ColorWithAlpha(0));
                                continue;
                            }
                        }
                    }
                }
            }

            l_Texture.Apply();
            return l_Texture;
        }

    }
}
