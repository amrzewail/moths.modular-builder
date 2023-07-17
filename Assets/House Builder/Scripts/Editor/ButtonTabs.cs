using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class ButtonTabs : VisualElement
    {
        private List<Button> _buttons = new List<Button>();
        public event Action<int, string> onTabClicked;

        public int CurrentTab { get; private set; }

        public int TabsCount => _buttons.Count;

        public Label label;

        private VisualElement _buttonsContainer;

        public ButtonTabs(params string[] tabs)
        {
            

            label = new Label();
            _buttonsContainer = new VisualElement();

            this.Add(label);
            this.Add(_buttonsContainer);

            _buttonsContainer.style.flexShrink = 0;
            _buttonsContainer.style.flexDirection = FlexDirection.Row;


            for (int i = 0; i < tabs.Length; i++)
            {
                AddTab(tabs[i]);
            }
        }

        public void AddTab(string tab)
        {
            Button btn = new Button();
            btn.name = tab;
            btn.text = tab;

            int index = _buttons.Count;
            btn.clicked += () => ButtonClick(index, tab);

            _buttons.Add(btn);
            _buttonsContainer.Add(btn);

            _buttons[index].style.backgroundColor = Color.clear;

            if (index == 0) ButtonClick(index, tab);
        }

        public void ClearTabs()
        {
            _buttons.Clear();
            _buttonsContainer.Clear();
        }

        private void ButtonClick(int index, string name)
        {
            ClickNoCallback(index);
            onTabClicked?.Invoke(index, name);
        }

        public void Click(int index)
        {
            ButtonClick(index, _buttons[0].name);
        }

        public void ClickNoCallback(int index)
        {
            if (index < 0) return;
            if (index >= _buttons.Count) return;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].style.backgroundColor = Color.clear;
            }
            _buttons[index].style.backgroundColor = new Color(0, 0.7f, 0, 1);
            CurrentTab = index;
        }
    }
}