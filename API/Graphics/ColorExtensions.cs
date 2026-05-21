using System;
using Microsoft.Xna.Framework;

namespace API.Graphics;

public static class ColorExtensions
{
    extension(Color @this)
    {
        public static Color Trans
        {
            get
            {
                return new(0);
            }
        }

        // Debug outlines for actors
        public static Color ActorOutline
        {
            get
            {
                return Color.Blue;
            }
        }

        public static Color ActorOutlineProg0
        {
            get
            {
                return Color.Yellow;
            }
        }

        public static Color ActorOutlineProg1
        {
            get
            {
                return Color.Fuchsia;
            }
        }

        public static Color ActorPadding
        {
            get
            {
                return Color.Cyan;
            }
        }

        public static Color ActorOrigin
        {
            get
            {
                return Color.Lime;
            }
        }

        public static Color ActorDisabledInput
        {
            get
            {
                return new(0f, 0f, 1f, 0.2f);
            }
        }

        public static Color ActorMouseHover
        {
            get
            {
                return new(0f, 1f, 0f, 0.2f);
            }
        }

        public string ToRgbaStr()
        {
            return $"#{@this.R:x2}{@this.G:x2}{@this.B:x2}{@this.A:x2}";
        }

        /// <returns>
        /// A <c>Color</c> made from the given hex (<c>0xRRGGBB</c> or <c>0xRRGGBBAA</c>)
        /// </returns>
        // Color is stored as AGBR, so we need to swap around some bits
        public static Color FromRgba(uint rgba)
        {
            uint rgb;
            uint alpha;

            // RGB input
            if ((rgba & 0xff000000) == 0)
            {
                rgb = rgba & 0x00ffffff;
                alpha = 0xff;
            }
            // RGBA input
            else
            {
                rgb = (rgba >> 8) & 0x00ffffff;
                alpha = rgba & 0xff;
            }

            return new Color(
                (alpha << 24)   // A
                | ((rgb & 0xff) << 16)       // R
                | (((rgb >> 8) & 0xff) << 8) // G
                | ((rgb >> 16) & 0xff));     // B
        }

        /// <returns>
        /// A array of <c>Color</c>s made from the given hexes (<c>0xRRGGBB</c>)
        /// </returns>
        public static Span<Color> FromRgbs(params ReadOnlySpan<uint> hexes)
        {
            Span<Color> colors = new Color[hexes.Length];

            for (int i = 0; i < hexes.Length; i++)
            {
                colors[i] = Color.FromRgba(hexes[i]);
            }

            return colors;
        }
    }
}
