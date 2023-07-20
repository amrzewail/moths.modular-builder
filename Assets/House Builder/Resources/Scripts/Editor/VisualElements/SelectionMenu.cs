using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
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
            this.style.flexDirection = FlexDirection.Row;
            this.style.justifyContent = Justify.SpaceBetween;

            _label = new Label();
            _label.text = label;
            _label.style.minWidth = 100;
            this.Add(_label);


            _button = new Button(OnButtonClick);
            _button.style.flexGrow = 1;
            this.Add(_button);
        }

        public SelectionMenu()
        {
            this.style.flexDirection = FlexDirection.Row;
            this.style.flexGrow = 1;
            this.style.justifyContent = Justify.SpaceBetween;

            _button = new Button(OnButtonClick);
            _button.style.flexGrow = 1;
            this.Add(_button);
        }

        public void Refresh(T[] sets, Func<T, string> nameSelector)
        {
            GetName = nameSelector;

            if (_button == null)
            {
                _button = new Button(OnButtonClick);
                this.Add(_button);
            }
            _button.text = GetName(sets[0]);
            _sets = sets;
        }

        void OnButtonClick()
        {
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