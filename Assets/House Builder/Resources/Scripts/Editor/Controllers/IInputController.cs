using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface IInputController
    {
        KeyCommand Command { get; }

        int ScrollWheel { get; }

        Vector2 MousePosition { get; }

        void Update();

        void Clear();
    }

}