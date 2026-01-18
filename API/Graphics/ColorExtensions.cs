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

        public static Color ActorMarked
        {
            get
            {
                return new(1f, 0f, 0f, 0.1f);
            }
        }

        public static Color ActorDisabledInput
        {
            get
            {
                return new(0f, 0f, 1f, 0.1f);
            }
        }

        public string ToRgbaStr()
        {
            return $"#{@this.R:x2}{@this.G:x2}{@this.B:x2}{@this.A:x2}";
        }

        /// <returns>
        /// A <c>Color</c> made from the given hex (<c>0xRRGGBB</c>)
        /// </returns>
        // Xna.Framework.Color is stored as AGBR, so we need to reverse the bit order
        public static Color FromRgb(uint rgb)
        {
            return new(0xff000000  // A
            | ((rgb & 0xff) << 16)       // R
            | (((rgb >> 8) & 0xff) << 8) // G
            | ((rgb >> 16) & 0xff));     // B
        }

        /// <returns>
        /// A array of <c>Color</c>s made from the given hexes (<c>0xRRGGBB</c>)
        /// </returns>
        public static Color[] FromRgbs(params uint[] hexes)
        {
            Color[] colors = new Color[hexes.Length];
            
            for (int i = 0; i < hexes.Length; i++)
            {
                colors[i] = Color.FromRgb(hexes[i]);
            }

            return colors;
        }
    }
}