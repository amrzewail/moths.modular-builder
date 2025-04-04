using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor
{
    public class ModulePaletteEditingVisualElement : VisualElement
    {
        private ModulePalette _palette;
        private SelectionMenu<string> _moduleType;
        private VisualElement _headerAlignRight;
        private VisualElement _prefabsGrid;

        public event Action<ModulePalette> deleted;

        public ModulePaletteEditingVisualElement(ModulePalette palette)
        {
            this.AddToClassList("module-palette");

            _palette = palette;

            var header = new VisualElement();
            header.AddToClassList("header");
            this.Add(header);

            _moduleType = new SelectionMenu<string>();
            _moduleType.AddToClassList("module-type-selection");
            _moduleType.onSelected += ModuleTypeChangeCallback;
            header.Add(_moduleType);

            _headerAlignRight = new VisualElement();
            _headerAlignRight.style.flexDirection = FlexDirection.Row;
            header.Add(_headerAlignRight);

            var highlightBtn = new Button(HighlightPaletteCallback) { text = "Highlight" };
            highlightBtn.AddToClassList("highlight-btn");
            _headerAlignRight.Add(highlightBtn);

            var deleteBtn = new Button() { text = "X" };
            deleteBtn.AddToClassList("delete-btn");
            deleteBtn.clicked += () => deleted?.Invoke(_palette);
            _headerAlignRight.Add(deleteBtn);

            _prefabsGrid = new VisualElement();
            _prefabsGrid.AddToClassList("grid");
            this.Add(_prefabsGrid);

            UpdateModuleType();
            UpdatePrefabs();
        }

        private void UpdatePrefabs()
        {
            var prefabsList = _palette.Prefabs.ToList();
            for (int i = 0; i < prefabsList.Count; i++)
            {
                if (prefabsList[i] == null)
                {
                    prefabsList.RemoveAt(i);
                    i--;
                }
            }
            if (prefabsList.Count != _palette.Prefabs.Length)
            {
                _palette.SetPrefabs(prefabsList.ToArray());
                BuilderEditorUtility.SaveAssetChanges(_palette);
            }

            _prefabsGrid.Clear();

            int index = 0;
            foreach(var prefab in prefabsList)
            {
                int i = index;
                PropertyVisualElement<GameObject> property = new PropertyVisualElement<GameObject>(prefab);
                property.propertyChanged += g => PrefabChangedCallback(i, g);
                _prefabsGrid.Add(property);
                index++;
            }
            var addPrefab = new PropertyVisualElement<GameObject>(null);
            addPrefab.propertyChanged += g => PrefabChangedCallback(index, g);
            _prefabsGrid.Add(addPrefab);
        }

        private bool ModuleTypeChangeCallback(string type)
        {
            _palette.SetType(type);
            BuilderEditorUtility.SaveAssetChanges(_palette);
            return true;
        }

        private void UpdateModuleType()
        {
            var types = ModuleTypeUtility.GetTypes();
            _moduleType.Refresh(types.ToArray(), x => x);
            _moduleType.Select(_palette.Type);
        }

        private void HighlightPaletteCallback()
        {
            EditorGUIUtility.PingObject(_palette);
        }

        private bool PrefabChangedCallback(int index, GameObject newPrefab)
        {
            List<GameObject> prefabs = _palette.Prefabs.ToList();
            if (index == prefabs.Count)
            {
                prefabs.Add(newPrefab);

                var addPrefab = new PropertyVisualElement<GameObject>(null);
                addPrefab.propertyChanged += g => PrefabChangedCallback(prefabs.Count, g);
                _prefabsGrid.Add(addPrefab);
            }
            else
            {
                if (newPrefab)
                {
                    prefabs[index] = newPrefab;
                }
                else
                {
                    prefabs.RemoveAt(index);
                }

            }
            _palette.SetPrefabs(prefabs.ToArray());
            BuilderEditorUtility.SaveAssetChanges(_palette);
            UpdatePrefabs();

            return true;
        }

    }
}