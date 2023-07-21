using HouseBuilder.Editor.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor.Views
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

            _newPaletteSetBtn = new Button() { text = "New palette set" };
            _newPaletteSetBtn.clicked += NewPaletteSetCallback;

            _state = State.PaletteSetListing;
        }

        private void NewPaletteSetCallback()
        {
            PaletteSet set = ScriptableObject.CreateInstance<PaletteSet>();
            string path = EditorUtility.SaveFilePanel("Save new palette set", "Assets", "Palette Set", "asset");
            if (string.IsNullOrEmpty(path)) return;
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

            ScrollView modulePaletteList = new ScrollView();
            this.Add(modulePaletteList);

            foreach(var palette in _currentEditingSet.Palettes)
            {
                ModulePaletteVisualElement modulePalette = new ModulePaletteVisualElement(palette);
                modulePalette.deleted += ModulePaletteDeleteCallback;
                modulePaletteList.Add(modulePalette);
            }
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