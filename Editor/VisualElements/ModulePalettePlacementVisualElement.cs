using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor
{
    public class ModulePalettePlacementVisualElement : ExpandableElement
    {
        private static Dictionary<ModulePalette, bool> _paletteExpands = new Dictionary<ModulePalette, bool>();

        private ModulePalette _palette;

        private Tabs<PrefabButtonVisualElement> _tabs = new Tabs<PrefabButtonVisualElement>();
        private List<PrefabButtonVisualElement> _tabsList = new List<PrefabButtonVisualElement>();

        public event Action<string, GameObject> selected;
        public Action<string, GameObject> replace;
        public Action<string, GameObject> add;

        public ModulePalettePlacementVisualElement(ModulePalette palette) : base()
        {
            this.AddToClassList("module-type-element");

            _palette = palette;

            if (!palette) return;

            title = palette.Type;

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

            this.expandableElement = _tabs;

            bool isExpanded = true;

            if (_paletteExpands.ContainsKey(_palette))
            {
                isExpanded = _paletteExpands[_palette];
            }
            UpdateExpand(isExpanded);
        }

        private void ReplacePrefabCallback(GameObject g)
        {
            replace?.Invoke(_palette.Type, g);
        }

        private void AddPrefabCallback(GameObject g)
        {
            add?.Invoke(_palette.Type, g);
        }

        private void PrefabButtonCallback(string moduleType, int index)
        {
            selected?.Invoke(moduleType, _tabs.GetTab(index).prefab);
        }

        protected override void OnExpandUpdated()
        {
            _paletteExpands[_palette] = isExpanded;
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