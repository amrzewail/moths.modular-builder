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

        public event Action<int> onTabClicked;

        public int Current { get; private set; }

        public int TabsCount => _tabs.Count;

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
            this.AddToClassList("tabs-container");

            _label = new Label();
            _tabsContainer = new VisualElement();
            _tabsContainer.AddToClassList("tabs");

            this.Add(_tabsContainer);
        }


        public void AddTab(T tab, int autoSelectIndex = 0)
        {
            VisualElement vs = tab;

            tab.AddToClassList("tab");

            int index = _tabs.Count;

            vs.RegisterCallback<ClickEvent>(evt => ButtonClick(index));

            _tabsContainer.Add(tab);
            _tabs.Add(tab);

            if (index == autoSelectIndex) ButtonClick(index);
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

        public T GetTab(int index) => _tabs[index];

        public void ClickNoCallback(int index)
        {
            if (index < 0) return;
            if (index >= _tabs.Count) return;
            Current = index;

            _tabs.FirstOrDefault(x => x.ClassListContains("selected"))?.RemoveFromClassList("selected");
            _tabs[Current].AddToClassList("selected");
        }

        public void RemoveAllTabsClass(string className)
        {
            foreach(var tab in _tabs)
            {
                if (!tab.ClassListContains(className)) continue;
                tab.RemoveFromClassList(className);
            }
        }

        public void AddTabClass(int index, string className)
        {
            _tabs[index].AddToClassList(className);
        }

        public void ClearSelection()
        {
            if (Current < 0) return;
            if (_tabs[Current].ClassListContains("selected"))
            {
                _tabs[Current].RemoveFromClassList("selected");
            }
            Current = -1;
        }
    }
}