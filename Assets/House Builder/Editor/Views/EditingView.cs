using HouseBuilder.Editor.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor.Views
{
    public class EditingView : VisualElement
    {
        private readonly IEditor _editor;

        private Label _warningLabel;

        private int _lastSelectionCount = -1;

        private MaterialsEditingContainer _materialEditingContainer;

        public EditingView(IEditor editor)
        {

            _editor = editor;


            CreateGUI();

        }


        private void CreateGUI()
        {
            this.AddToClassList("editing-view");

            _materialEditingContainer = new MaterialsEditingContainer(_editor);
            _materialEditingContainer.AddToClassList("materials-list");
            this.Add(_materialEditingContainer);


            _warningLabel = new Label();
            //_warningLabel.AddToClassList("no-palette-sets-label");
        }


        public void Refresh()
        {
            if (this.Contains(_warningLabel)) this.Remove(_warningLabel);

            _materialEditingContainer.Refresh();

            _editor.OnUpdate += Update;
        }

        public void Close()
        {
            _editor.OnUpdate -= Update;
        }

        private void OnSelectionChanged()
        {
            _materialEditingContainer.OnSelectionChanged();
        }

        private void Update()
        {
            if (_lastSelectionCount != _editor.Selector.CurrentMultiple.Count)
            {
                OnSelectionChanged();
                _lastSelectionCount = _editor.Selector.CurrentMultiple.Count;
            }
        }
    }
}