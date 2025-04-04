using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor
{
    public class GridVisualElement : VisualElement
    {
        public GridVisualElement()
        {
            this.style.flexDirection = FlexDirection.Row;
            this.style.flexWrap = Wrap.Wrap;
        }

    }
}