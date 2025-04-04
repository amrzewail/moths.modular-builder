using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moths.ModularBuilder.Editor.Data
{
    public enum KeyCommand
    {
        None = 0,
        Place = 10,
        Delete = 20,
        Rotate = 30,
        Flip = 40,
        Extrude = 50,
        AdjustHeight = 60,
        Frame = 70,
        UnselectPrefab = 80,
        HidePreview = 90,
        Highlight = 100,
        HighlightAll = 110,
    }

    public static class KeyCommandExtensions
    {
        public static string CommandString(this KeyCommand command)
        {
            switch (command)
            {
                case KeyCommand.Place: return "Place";
                case KeyCommand.Delete: return "Delete";
                case KeyCommand.Rotate: return "Rotate";
                case KeyCommand.Flip: return "Flip";
                case KeyCommand.Extrude: return "Extrude";
                case KeyCommand.AdjustHeight: return "Adjust height";
                case KeyCommand.Frame: return "Frame";
                case KeyCommand.UnselectPrefab: return "Unselect prefab";
                case KeyCommand.HidePreview: return "Hide preview";
                case KeyCommand.Highlight: return "Highlight";
                case KeyCommand.HighlightAll: return "Highlight all";
            }
            return "";
        }
    }
}