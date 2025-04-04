using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor
{
    public class SelectionMenu<T> : VisualElement
    {
        private Button _button;
        private Label _label;

        private T[] _sets;
        private T _current;

        public event Func<T, bool> onSelected;
        public event Func<T, bool> isItemDisabled;

        private Func<T, string> GetName;

        public T Current => _current;

        public SelectionMenu(string label) 
        {
            Init(label);
        }

        public SelectionMenu()
        {
            Init("");
        }

        private void Init(string label)
        {
            this.AddToClassList("selection-menu");

            if (!string.IsNullOrEmpty(label))
            {
                _label = new Label();
                _label.text = label;
                _label.style.minWidth = 100;
                _label.AddToClassList("selection-label");
                this.Add(_label);
            }


            _button = new Button(OnButtonClick);
            _button.style.flexGrow = 1;
            _button.AddToClassList("selection-button");
            this.Add(_button);

            var dropdownIcon = new Label("v");
            dropdownIcon.style.width = Length.Percent(100);
            dropdownIcon.style.unityTextAlign = TextAnchor.MiddleRight;
            dropdownIcon.style.marginLeft = 4;
            _button.Add(dropdownIcon);
        }

        public void Refresh(T[] sets, Func<T, string> nameSelector)
        {
            GetName = nameSelector;

            if (_button == null)
            {
                _button = new Button(OnButtonClick);
                this.Add(_button);
            }
            if (sets.Length > 0)
            {
                _button.text = GetName(sets[0]);
            }
            else
            {
                _button.text = "";
            }
            _sets = sets;
        }

        void OnButtonClick()
        {
            if (_sets == null) return;

            // create the menu and add items to it
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < _sets.Length; i++)
            {
                int index = i;
                if (isItemDisabled != null && isItemDisabled(_sets[i]))
                {
                    menu.AddDisabledItem(new GUIContent(GetName(_sets[i])), false);
                }
                else
                {
                    menu.AddItem(new GUIContent(GetName(_sets[i])), EqualityComparer<T>.Default.Equals(_current, _sets[i]), () => OnItemSelected(index));
                }
            }

            // display the menu
            menu.ShowAsContext();
        }

        private void OnItemSelected(int index)
        {
            if (onSelected == null || onSelected(_sets[index]))
            {
                Select(_sets[index]);
            }
        }

        public void Select(T item)
        {
            _current = item;
            _button.text = GetName(_current);
        }
    }
}