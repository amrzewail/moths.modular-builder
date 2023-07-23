using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Data
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
}