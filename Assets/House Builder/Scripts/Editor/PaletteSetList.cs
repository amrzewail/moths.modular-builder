using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class PaletteSetList : VisualElement
    {
        private Button _button;
        private PaletteSet[] _sets;
        private PaletteSet _current;

        public event Func<PaletteSet, bool> onSelected;

        public PaletteSetList()
        {
            var button = new Button();
            this.Add(button);
        }

        public void Refresh(PaletteSet[] sets)
        {
            this.Clear();

            _button = new Button(OnButtonClick);
            _button.text = sets[0].name;
            this.Add(_button);

            _sets = sets;
        }

        void OnButtonClick()
        {
            // create the menu and add items to it
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < _sets.Length; i++)
            {
                int index = i;
                menu.AddItem(new GUIContent(_sets[i].name), _current == _sets[i], () => OnItemSelected(index));
            }

            // display the menu
            menu.ShowAsContext();
        }

        private void OnItemSelected(int index)
        {
            if (onSelected(_sets[index]))
            {
                _current = _sets[index];
                _button.text = _sets[index].name;
            }
        }
    }
}