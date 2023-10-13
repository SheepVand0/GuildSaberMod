using GuildSaber.Logger;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.Utils
{
    internal static class TextureUtils
    {

        public static Texture2D MakeCorrespondHeight(Texture2D p_Texture, Rect p_Rect)
        {
            if (p_Rect.y >= p_Texture.height) return p_Texture;

            UnityEngine.Color[] l_Texture = p_Texture.GetPixels(0, (int)(p_Texture.width - p_Rect.y) / 2, p_Texture.width, (int)p_Rect.height);

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

        public static async Task<Texture2D> Gradient(Texture2D p_Texture, UnityEngine.Color p_Color1, UnityEngine.Color p_Color2, EGradientDirection p_Direction = EGradientDirection.Horizontal, bool p_Invert = false, bool p_UseAlpha = false)
        {
            Texture2D l_Origin = p_Texture;

            UnityEngine.Color l_FirstColor = (p_Invert) ? p_Color1 : p_Color2;
            UnityEngine.Color l_SecondColor = (p_Invert) ? p_Color2 : p_Color1;
            await Task.Run(() =>
            {
                for (int l_X = 0; l_X < p_Texture.width; l_X++)
                {
                    for (int l_Y = 0; l_Y < p_Texture.height; l_Y++)
                    {
                        Color l_CurrentPixel = l_Origin.GetPixel(l_X, l_Y);

                        float l_Color2Multiplier = (float)(p_Direction == EGradientDirection.Horizontal ? l_X : l_Y) / (float)(p_Direction == EGradientDirection.Horizontal ? l_Origin.width : l_Origin.height);

                        float l_Alpha = (p_UseAlpha) ? ((l_FirstColor.a + l_SecondColor.a) / 2) * l_Color2Multiplier : l_Origin.GetPixel(l_X, l_Y).a;

                        l_Origin.SetPixel(
                            l_X, l_Y,
                            new Color(
                                l_CurrentPixel.r * ((l_FirstColor.r + (l_SecondColor.r * l_Color2Multiplier)) / 2),
                                l_CurrentPixel.g * ((l_FirstColor.g + (l_SecondColor.g * l_Color2Multiplier)) / 2),
                                l_CurrentPixel.b * ((l_FirstColor.b + (l_SecondColor.b * l_Color2Multiplier)) / 2),
                                l_CurrentPixel.a * l_Alpha)
                            );
                    }
                }
            });
            l_Origin.Apply();

            return l_Origin;
        }

        public async static Task<Texture2D> AddOffset(Texture2D p_Origin, int p_Offset)
        {
            int l_Height = p_Origin.height - (p_Offset * 2);

            if (l_Height <= 0)
            {
                l_Height = p_Origin.height;
            }
            Texture2D l_Result = new Texture2D(p_Origin.width, l_Height);

            await Task.Run(() =>
            {
                Color[] l_Colors = p_Origin.GetPixels();

                for (int l_X = 0; l_X < l_Result.width; l_X++)
                {
                    for (int l_Y = 0; l_Y < l_Result.height; l_Y++)
                    {
                        int l_FixedY = l_Y + p_Offset;
                        if (l_Result.height == p_Origin.height)
                            l_FixedY = l_Y;

                        l_Result.SetPixel(l_X, l_Y, p_Origin.GetPixel(l_X, l_FixedY));
                    }
                }
            });
            l_Result.Apply();
            return l_Result;
        }

        private static Color m_TransparentColor = new Color(1, 0, 0, 0);

        public static async Task<Texture2D> CreateRoundedTexture(Texture2D p_Origin, float p_Radius, bool p_PushPixels = false)
        {
            Texture2D l_Texture = p_Origin;

            

            await Task.Run(() =>
            {


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
                                Vector2 l_Point = new Vector2(l_X, l_Y);
                                Vector2 l_RadiusPoint = new Vector2(p_Radius, p_Radius);

                                if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                                {
                                    l_Moved = true;
                                    l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, m_TransparentColor);
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
                                Vector2 l_Point = new Vector2(l_X + (l_Texture.width - p_Radius), l_Y);
                                Vector2 l_RadiusPoint = new Vector2(l_Texture.width - p_Radius, p_Radius);

                                if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                                {
                                    l_Moved = true;
                                    l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, m_TransparentColor);
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
                                Vector2 l_Point = new Vector2(l_X, l_Y + (l_Texture.height - p_Radius));
                                Vector2 l_RadiusPoint = new Vector2(p_Radius, (l_Texture.height - p_Radius));

                                if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                                {
                                    l_Moved = true;
                                    l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, m_TransparentColor);
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
                                Vector2 l_Point = new Vector2(l_X + (l_Texture.width - p_Radius), l_Y + (l_Texture.height - p_Radius));
                                Vector2 l_RadiusPoint = new Vector2(l_Texture.width - p_Radius, l_Texture.height - p_Radius);

                                if (Vector2.Distance(l_Point, l_RadiusPoint) > p_Radius)
                                {
                                    l_Moved = true;
                                    l_Texture.SetPixel((int)l_Point.x, (int)l_Point.y, m_TransparentColor);
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
            });
            l_Texture.Apply();
            return l_Texture;
        }

        public static Texture2D GetCopy(this Texture2D p_Texture)
        {
            Texture2D l_New = new Texture2D(p_Texture.width, p_Texture.height);
            Color[] l_Olds = p_Texture.GetPixels();
            l_New.SetPixels(l_Olds);
            return l_New;
        }

        public struct FixedHeight
        {
            public int NewHeight;
            public int Position;
            public int TextureOffset;
        }

        public static FixedHeight GetHeight(int p_ImageViewWidth, int p_ImageViewHeight, int p_TextureWidth, int p_TextureHeigth)
        {
            int l_WantedImageSize = (int)(p_TextureWidth * ((float)p_ImageViewHeight / p_ImageViewWidth));
            int l_FixedHeigth = l_WantedImageSize;
            FixedHeight l_Result = new FixedHeight();
            l_Result.NewHeight = l_FixedHeigth;
            l_Result.TextureOffset = (p_TextureHeigth / 2) - (l_WantedImageSize / 2);
            return l_Result;

        }

        public static async Task<Texture2D> CreateFlatTexture(int p_Width, int p_Height, Color p_Color)
        {
            Texture2D l_Texture = new Texture2D(p_Width, p_Height, TextureFormat.RGBA64, false);

            await Task.Run(() =>
            {
                for (int l_X = 0; l_X < p_Width; l_X++)
                {
                    for (int l_Y = 0; l_Y < p_Height; l_Y++)
                    {
                        l_Texture.SetPixel(l_X, l_Y, p_Color);
                    }
                }
            });
            l_Texture.Apply();
            return l_Texture;
        }

        public static async Task<Texture2D> AddLine(Texture2D p_Texture, int p_Width, int p_Height, int p_PosX, int p_PosY, Color p_LineColor)
        {
            await Task.Run(() =>
            {
                for (int l_X = 0; l_X < p_Width; l_X++)
                {
                    for (int l_Y = 0;l_Y < p_Height;l_Y++)
                    {
                        p_Texture.SetPixel(l_X + p_PosX, l_Y + p_PosY, p_LineColor);
                    }
                }
            });
            p_Texture.Apply();

            return p_Texture;
        }
    }
}
