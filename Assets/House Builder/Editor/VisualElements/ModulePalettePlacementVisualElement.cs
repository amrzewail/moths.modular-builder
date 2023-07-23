using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class ModulePalettePlacementVisualElement : VisualElement
    {
        private static Dictionary<ModulePalette, bool> _paletteExpands = new Dictionary<ModulePalette, bool>();

        private bool _isExpanded = true;
        private ModulePalette _palette;

        private VisualElement _header;
        private Tabs<PrefabButtonVisualElement> _tabs = new Tabs<PrefabButtonVisualElement>();
        private List<PrefabButtonVisualElement> _tabsList = new List<PrefabButtonVisualElement>();

        public event Action<string, GameObject> selected;
        public Action<string, GameObject> replace;
        public Action<string, GameObject> add;

        public ModulePalettePlacementVisualElement(ModulePalette palette)
        {
            this.AddToClassList("module-type-element");

            _palette = palette;

            if (!palette) return;

            _header = new VisualElement();
            _header.AddToClassList("header");
            this.Add(_header);

            Button titleBtn = new Button();
            titleBtn.AddToClassList("title-btn");
            titleBtn.text = palette.Type;
            titleBtn.clicked += ExpandClickCallback;

            _header.Add(titleBtn);

            _tabsList.Clear();
            _tabs = new Tabs<PrefabButtonVisualElement>();
            _tabs.AddToClassList("prefabs-tabs");
            _tabs.onTabClicked += index => PrefabButtonCallback(palette.Type, index);
            this.Add(_tabs);

            int i = 0;
            foreach (var prefab in palette.Prefabs)
            {
                var prefabVE = new PrefabButtonVisualElement(prefab);
                prefabVE.AddToClassList("prefab-tab");
                prefabVE.replace += ReplacePrefabCallback;
                prefabVE.add += AddPrefabCallback;
                _tabs.AddTab(prefabVE, -1);
                _tabsList.Add(prefabVE);
                i++;
            }


            if (_paletteExpands.ContainsKey(_palette))
            {
                _isExpanded = _paletteExpands[_palette];
            }
            UpdateExpandVisuals();
        }

        private void ReplacePrefabCallback(GameObject g)
        {
            replace?.Invoke(_palette.Type, g);
        }

        private void AddPrefabCallback(GameObject g)
        {
            add?.Invoke(_palette.Type, g);
        }

        private void ExpandClickCallback()
        {
            _isExpanded = !_isExpanded;
            _paletteExpands[_palette] = _isExpanded;

            UpdateExpandVisuals();
        }

        private void UpdateExpandVisuals()
        {
            if (_isExpanded)
            {
                if (_tabs.ClassListContains("unexpanded")) _tabs.RemoveFromClassList("unexpanded");
            }
            else
            {
                if (!_tabs.ClassListContains("unexpanded"))  _tabs.AddToClassList("unexpanded");
            }
        }

        private void PrefabButtonCallback(string moduleType, int index)
        {
            selected?.Invoke(moduleType, _tabs.GetTab(index).prefab);
        }

        public void ClearSelection() => _tabs.ClearSelection();

        public void SetHoverElementsVisibility(bool visibility)
        {
            foreach(var tab in _tabsList)
            {
                foreach (var element in tab.onHoverElements) element.style.visibility = visibility ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}