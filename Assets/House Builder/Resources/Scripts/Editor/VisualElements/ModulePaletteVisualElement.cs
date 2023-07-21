using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class ModulePaletteVisualElement : VisualElement
    {
        private ModulePalette _palette;
        private SelectionMenu<string> _moduleType;
        private VisualElement _prefabsGrid;

        public event Action<ModulePalette> deleted;

        public ModulePaletteVisualElement(ModulePalette palette)
        {
            this.AddToClassList("module-palette");

            _palette = palette;

            var header = new VisualElement();
            header.AddToClassList("header");
            this.Add(header);

            _moduleType = new SelectionMenu<string>();
            _moduleType.onSelected += ModuleTypeChangeCallback;
            header.Add(_moduleType);


            var highlightBtn = new Button(HighlightPaletteCallback) { text = "Highlight" };
            highlightBtn.AddToClassList("highlight-btn");
            header.Add(highlightBtn);

            var deleteBtn = new Button() { text = "X" };
            deleteBtn.AddToClassList("delete-btn");
            header.Add(deleteBtn);
            deleteBtn.clicked += () => deleted?.Invoke(_palette);

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
            foreach(var prefab in _palette.Prefabs)
            {
                int i = index;
                PrefabPropertyVisualElement property = new PrefabPropertyVisualElement(prefab);
                property.prefabChanged += g => PrefabChangedCallback(i, g);
                _prefabsGrid.Add(property);
                index++;
            }
            var addPrefab = new PrefabPropertyVisualElement(null);
            addPrefab.prefabChanged += g => PrefabChangedCallback(index, g);
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

        private void PrefabChangedCallback(int index, GameObject newPrefab)
        {
            List<GameObject> prefabs = _palette.Prefabs.ToList();
            if (index == prefabs.Count)
            {
                prefabs.Add(newPrefab);

                var addPrefab = new PrefabPropertyVisualElement(null);
                addPrefab.prefabChanged += g => PrefabChangedCallback(prefabs.Count, g);
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
        }

    }
}