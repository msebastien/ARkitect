using System.Runtime.CompilerServices;
using UnityEngine;

namespace ARKitect.UI.Colors
{
    /// <summary>
    /// Default color scheme for ARkitect
    /// </summary>
    public static class UIColors
    {
        /// <summary>
        /// 100% White
        /// </summary>
        public static Color White
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Color(1.0F, 1.0F, 1.0F);
            }
        }

        /// <summary>
        /// 95% White
        /// </summary>
        public static Color White2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Color(0.95F, 0.95F, 0.95F);
            }
        }

        public static Color Blue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Color(0.1921569F, 0.7098039F, 1.0F);
            }
        }

        public static Color Yellow
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Color(0.9254902F, 0.8470588F, 0.0F);
            }
        }

        public static Color Red
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Color(1.0F, 0.21568628F, 0.19215687F);
            }
        }

        public static Color Green
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Color(0.1019608F, 0.9058824F, 0.0F);
            }
        }
    }

}
