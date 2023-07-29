using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class ButtonTabs : VisualElement
    {

        private List<Button> _buttons = new List<Button>();
        private Dictionary<int, Color> _bgColorOverrides = new Dictionary<int, Color>();

        public event Action<int, string> onTabClicked;

        public int CurrentTab { get; private set; }

        public int TabsCount => _buttons.Count;

        public Label label;

        private VisualElement _buttonsContainer;

        public IStyle tabsStyle { get => _buttonsContainer.style; }

        public ButtonTabs()
        {

            Init();

        }

        public ButtonTabs(params string[] tabs)
        {
            Init();
            for (int i = 0; i < tabs.Length; i++)
            {
                AddTab(tabs[i]);
            }
        }

        private void Init()
        {
            label = new Label();
            _buttonsContainer = new VisualElement();

            _buttonsContainer.AddToClassList("buttons-container");

            this.Add(label);
            this.Add(_buttonsContainer);
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


            if (index == 0) ButtonClick(index, tab);
        }

        public void RemoveLabel()
        {
            this.Remove(label);
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
            _buttons.FirstOrDefault(x => x.ClassListContains("selected"))?.RemoveFromClassList("selected");

            foreach(var container in _bgColorOverrides)
            {
                if (CurrentTab == container.Key) continue;
                _buttons[container.Key].style.backgroundColor = container.Value;
            }
            _buttons[CurrentTab].AddToClassList("selected");
        }

    }
}