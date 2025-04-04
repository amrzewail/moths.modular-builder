using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder.Editor.Data
{
    public enum MouseButton
    {
        None = 0,
        Left = 10,
        Right = 20,
    }

    public static class MouseButtonExtensions
    {
        public static string MouseButtonString(this MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left: return "LMB";
                case MouseButton.Right: return "RMB";
            }
            return "";
        }
    }
}