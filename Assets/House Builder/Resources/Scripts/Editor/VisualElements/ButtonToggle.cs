using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class ButtonToggle : VisualElement
    {
        public event Func<bool, bool> changed;

        public bool value { get; set; }

        private Button _button;

        public ButtonToggle(string label)
        {
            this.AddToClassList("button-toggle");

            _button = new Button();
            _button.clicked += ButtonClickCallback;
            _button.text = label;

            this.Add(_button);
        }

        private void ButtonClickCallback()
        {
            value = !value;
            if (changed != null && !changed(value))
            {
                value = !value;
            }

            if (value)
            {
                if (!_button.ClassListContains("isOn")) _button.AddToClassList("isOn");
            }
            else
            {
                if (_button.ClassListContains("isOn")) _button.RemoveFromClassList("isOn");
            }
        }
    }
}