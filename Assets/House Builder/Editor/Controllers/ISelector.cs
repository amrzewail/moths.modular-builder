using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface ISelector
    {
        bool isEnabled { get; set; }

        Color? overrideColor { set; }

        Func<GameObject, bool> CanSelect { get; set; }

        GameObject Current { get; }

        List<GameObject> CurrentMultiple { get; }

        void Select(GameObject gameObject);

        void Clear();

    }
}