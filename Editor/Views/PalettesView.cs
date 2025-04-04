using Moths.ModularBuilder.Editor.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor.Views
{
    public class PalettesView : VisualElement
    {
        private enum State
        {
            PaletteSetListing,
            PaletteSetEditing,
        };

        private readonly IEditor _editor;
        private readonly IPaletteManager _paletteManager;

        private State _state;

        #region Listing

        private ScrollView _paletteSetList;
        private Button _newPaletteSetBtn;

        #endregion

        #region Editing

        private PaletteSet _currentEditingSet;

        #endregion

        public PalettesView(IEditor editor)
        {
            this.AddToClassList("palettes-view");

            _editor = editor;
            _paletteManager = _editor.Palettes;

            _paletteSetList = new ScrollView();

            _newPaletteSetBtn = new Button() { text = $"New {nameof(PaletteSet)}" };
            _newPaletteSetBtn.AddToClassList("new-palette-set-btn");
            _newPaletteSetBtn.clicked += NewPaletteSetCallback;

            _state = State.PaletteSetListing;
        }

        private void NewPaletteSetCallback()
        {
            string path = EditorUtility.SaveFilePanel("Save new palette set", "Assets", "Palette Set", "asset");
            if (string.IsNullOrEmpty(path)) return;

            PaletteSet set = ScriptableObject.CreateInstance<PaletteSet>();
            path = FileUtil.GetProjectRelativePath(path);
            AssetDatabase.CreateAsset(set, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(set);

            Refresh();
        }

        public void Refresh()
        {
            this.Clear();

            switch (_state)
            {
                case State.PaletteSetListing:
                    RefreshPaletteSetListing();
                    break;


                case State.PaletteSetEditing:
                    RefreshPaletteSetEditing();
                    break;
            }

            
        }

        private void RefreshPaletteSetListing()
        {
            this.Add(_paletteSetList);
            _paletteSetList.Clear();

            if (_paletteManager.LoadPaletteSets())
            {
                foreach (var set in _paletteManager.PaletteSets)
                {
                    PaletteSetListElement element = new PaletteSetListElement();
                    element.text = set.name;
                    element.target = set;
                    element.onEdit += PaletteSetEditCallback;
                    _paletteSetList.Add(element);
                }
            }
            else
            {
                _editor.Logger.Error(nameof(PalettesView), "Error loading palettes set");
            }

            _paletteSetList.Add(_newPaletteSetBtn);
        }

        private void RefreshPaletteSetEditing()
        {
            if (!_currentEditingSet)
            {
                _state = State.PaletteSetListing;
                Refresh();
                return;
            }

            VisualElement header = new VisualElement();
            header.AddToClassList("palette-set-editing-header");
            this.Add(header);

            Button paletteSetEditBackBtn = new Button(PaletteSetEditBackCallback)
            {
                text = "Back"
            };
            paletteSetEditBackBtn.AddToClassList("back-button");
            header.Add(paletteSetEditBackBtn);

            var currentPaletteSetLabel = new Label(_currentEditingSet.name);
            currentPaletteSetLabel.AddToClassList("title-label");
            header.Add(currentPaletteSetLabel);

            var highlightPaletteSet = new Button();
            highlightPaletteSet.text = "Highlight";
            highlightPaletteSet.clicked += () => EditorGUIUtility.PingObject(_currentEditingSet);
            header.Add(highlightPaletteSet);

            ScrollView modulePaletteList = new ScrollView();
            this.Add(modulePaletteList);

            var palettes = _currentEditingSet.Palettes == null ? new ModulePalette[0] : _currentEditingSet.Palettes;

            foreach (var palette in palettes)
            {
                ModulePaletteEditingVisualElement modulePalette = new ModulePaletteEditingVisualElement(palette);
                modulePalette.deleted += ModulePaletteDeleteCallback;
                modulePaletteList.Add(modulePalette);
            }
            PropertyVisualElement<ModulePalette> newModulePalette = new PropertyVisualElement<ModulePalette>(null);
            newModulePalette.AddToClassList("module-palette-property");
            newModulePalette.propertyChanged += AddModulePaletteCallback;
            newModulePalette.create += NewModulePaletteCallback;
            newModulePalette.imageFallbackText = $"Click to create new / Drag and Drop {nameof(ModulePalette)}";
            modulePaletteList.Add(newModulePalette);
        }

        private void NewModulePaletteCallback()
        {
            string path = EditorUtility.SaveFilePanel("Select palette", "Assets", "New Palette", "asset");
            if (string.IsNullOrEmpty(path)) return;
            path = FileUtil.GetProjectRelativePath(path);
            ModulePalette palette = null;
            var list = _currentEditingSet.Palettes.ToList();
            palette = AssetDatabase.LoadAssetAtPath<ModulePalette>(path);
            if (!palette)
            {
                palette = ScriptableObject.CreateInstance<ModulePalette>();
                AssetDatabase.CreateAsset(palette, path);
                AssetDatabase.SaveAssets();
            }
            else
            {
                if (list.Contains(palette)) return;
            }
            list.Add(palette);
            _currentEditingSet.SetPalettes(list.ToArray());
            BuilderEditorUtility.SaveAssetChanges(_currentEditingSet);
            Refresh();
        }

        private bool AddModulePaletteCallback(ModulePalette palette)
        {
            if (!palette) return false;
            var list = _currentEditingSet.Palettes == null ? new List<ModulePalette>() : _currentEditingSet.Palettes.ToList();
            if (list.Contains(palette)) return false;
            list.Add(palette);
            _currentEditingSet.SetPalettes(list.ToArray());
            BuilderEditorUtility.SaveAssetChanges(_currentEditingSet);
            Refresh();
            return true;
        }


        private void ModulePaletteDeleteCallback(ModulePalette palette)
        {
            var list = _currentEditingSet.Palettes.ToList();
            list.Remove(palette);
            _currentEditingSet.SetPalettes(list.ToArray());
            BuilderEditorUtility.SaveAssetChanges(_currentEditingSet);
            Refresh();

        }

        private void PaletteSetEditCallback(PaletteSetListElement element)
        {
            _currentEditingSet = element.target;
            _state = State.PaletteSetEditing;
            Refresh();
        }

        private void PaletteSetEditBackCallback()
        {
            _state = State.PaletteSetListing;
            Refresh();
        }
    }
}