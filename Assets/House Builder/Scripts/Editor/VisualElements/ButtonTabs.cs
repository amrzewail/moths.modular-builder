using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class ButtonTabs : VisualElement
    {
        private readonly Color _selectedColor = new Color(0, 0.7f, 0, 1);

        private List<Button> _buttons = new List<Button>();
        private Dictionary<int, Color> _bgColorOverrides = new Dictionary<int, Color>();

        public event Action<int, string> onTabClicked;

        public int CurrentTab { get; private set; }

        public int TabsCount => _buttons.Count;

        public Label label;

        private VisualElement _buttonsContainer;

        public IStyle tabsStyle { get => _buttonsContainer.style; }


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
            CurrentTab = index;
            UpdateTabBackgroundColors();
        }

        public void ClearBackgroundColorOverrides()
        {
            _bgColorOverrides.Clear();
        }

        public void OverrideTabBackgroundColor(int index, Color color)
        {
            _bgColorOverrides[index] = color;
        }

        public void UpdateTabBackgroundColors()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_bgColorOverrides.ContainsKey(i))
                {
                    _buttons[i].style.backgroundColor = _bgColorOverrides[i];
                }
                else
                {
                    _buttons[i].style.backgroundColor = Color.clear;
                }
            }
            _buttons[CurrentTab].style.backgroundColor = _selectedColor;
        }

    }
}