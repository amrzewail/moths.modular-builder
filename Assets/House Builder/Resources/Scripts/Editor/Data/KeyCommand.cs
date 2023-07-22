using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor
{
    public enum KeyCommand
    {
        None,
        LeftMouseButtonUp,
        PrepareHighlight,
        Delete,
        Rotate,
        Flip,
        ChangeHeight,
        Frame,
        UnselectedPrefab,
        HighlightDrag,
        HighlightClick,
        Extrude,
        HighlightAll,
    }
}