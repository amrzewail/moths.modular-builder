using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface ISelector
    {
        Color? overrideColor { set; }

        GameObject Current { get; }

        List<GameObject> CurrentMultiple { get; }

        void Clear();
    }
}