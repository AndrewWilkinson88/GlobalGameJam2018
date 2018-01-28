using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColorGame
{
    public enum GameColor
    {
        COLOR_BLUE = 1,
        COLOR_RED = 2,
        COLOR_MAGENTA = 3,
        COLOR_GREEN = 4,
        COLOR_CYAN = 5,
        COLOR_YELLOW = 6,
        COLOR_WHITE = 8
    }

    public enum Facing
    {
        LEFT = 0,
        RIGHT,
        UP,
        DOWN,
        LEFT_UP,
        RIGHT_UP,
        LEFT_DOWN,
        RIGHT_DOWN
    }

    public static class ColorDefs
    {
        public static GameColor CombineColors(GameColor color1, GameColor color2)
        {
            int mix = (int)color1 | (int)color2;
            int overrideMix = (int)color1 & (int)color2;

            if (Enum.IsDefined(typeof(GameColor), mix) && overrideMix == 0)
            {
                return (GameColor)mix;
            }
            else
            {
                return color2;
            }
        }

        public static Color GetColor(GameColor color)
        {
            switch (color)
            {
                case (GameColor.COLOR_BLUE):
                    return Color.blue;
                case (GameColor.COLOR_RED):
                    return Color.red;
                case (GameColor.COLOR_MAGENTA):
                    return Color.magenta;
                case (GameColor.COLOR_GREEN):
                    return Color.green;
                case (GameColor.COLOR_CYAN):
                    return Color.cyan;
                case (GameColor.COLOR_YELLOW):
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }
    }
}
