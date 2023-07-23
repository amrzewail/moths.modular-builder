using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Data
{
    public enum KeyBehaviour
    {
        Nothing = 0,
        Press = 10,
        Release = 20,
        Click = 30,
        DoubleClick = 40,
        ClickOrDrag = 50,
    }
}