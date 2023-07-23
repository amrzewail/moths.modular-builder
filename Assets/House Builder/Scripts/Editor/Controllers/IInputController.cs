using HouseBuilder.Editor.Data;
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

        bool IsDragging { get; }

        void SetContext(InputContext context);

        void Update();

        void Clear();
    }

}