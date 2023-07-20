using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class Tabs<T> : VisualElement where T : VisualElement
    {
        private List<T> _tabs = new List<T>();
        private Dictionary<int, Color> _bgColorOverrides = new Dictionary<int, Color>();

        public event Action<int> onTabClicked;

        public int Current { get; private set; }

        private Label _label;
        private VisualElement _tabsContainer;

        public string label
        {
            get
            {
                if (_label == null) return "";
                return _label.text;
            }
            set
            {
                if (_label == null) _label = new Label();
                if (string.IsNullOrEmpty(value))
                {
                    if (this.Contains(_label)) this.Remove(_label);
                }
                else
                {
                    _label.text = value;
                    if (!this.Contains(_label)) this.Insert(0, _label);
                }
            }
        }

        public Tabs()
        {
            Init();
        }

        public Tabs(params T[] tabs)
        {
            Init();
            for (int i = 0; i < tabs.Length; i++)
            {
                AddTab(tabs[i]);
            }
        }

        private void Init()
        {
            _label = new Label();
            _tabsContainer = new VisualElement();
            _tabsContainer.AddToClassList("tabs");

            this.Add(_tabsContainer);
        }


        public void AddTab(T tab)
        {
            VisualElement vs = tab;

            tab.AddToClassList("tab");

            int index = _tabs.Count;

            vs.RegisterCallback<ClickEvent>(evt => ButtonClick(index));

            _tabsContainer.Add(tab);
            _tabs.Add(tab);

            if (index == 0) ButtonClick(index);
        }

        private void ButtonClick(int index)
        {
            ClickNoCallback(index);
            onTabClicked?.Invoke(index);
        }

        public void ClearTabs()
        {
            _tabs.Clear();
            _tabsContainer.Clear();
        }

        public void Click(int index)
        {
            ButtonClick(index);
        }

        public void ClickNoCallback(int index)
        {
            if (index < 0) return;
            if (index >= _tabs.Count) return;
            Current = index;
            UpdateTabBackgroundColors();
        }


        public void UpdateTabBackgroundColors()
        {
            _tabs.FirstOrDefault(x => x.ClassListContains("selected"))?.RemoveFromClassList("selected");

            foreach (var container in _bgColorOverrides)
            {
                _tabs[container.Key].style.backgroundColor = container.Value;
            }

            _tabs[Current].AddToClassList("selected");
        }
    }
}