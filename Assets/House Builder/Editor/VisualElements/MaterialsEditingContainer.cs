using HouseBuilder.Editor.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class MaterialsEditingContainer : ExpandableElement
    {
        private IEditor _editor;

        private VisualElement _expandedContainer = new VisualElement();
        private Tabs<MaterialPropertyVisualElement> _selectedMaterials;
        private VisualElement _tabs = new VisualElement();
        private MaterialPropertyVisualElement _extraProperty;

        private Label _noSelectionsLabel = new Label("No selected modules.");

        public MaterialsEditingContainer(IEditor editor) : base()
        {
            _editor = editor;

            title = "Materials";

            this.Add(_expandedContainer);
            expandableElement = _expandedContainer;

            _selectedMaterials = new Tabs<MaterialPropertyVisualElement>();
            _selectedMaterials.AddToClassList("selected-materials-container");
            _expandedContainer.Add(_selectedMaterials);

            _tabs.AddToClassList("materials-container");
            _expandedContainer.Add(_tabs);

            _noSelectionsLabel.AddToClassList("no-selections-label");
            _noSelectionsLabel.AddToClassList("selected-materials-container");
        }

        public void Refresh()
        {
            if (!_editor.IsHouseValid) return;

            List<Material> materials = _editor.House.GetAllModuleMaterials();

            _tabs.Clear();

            foreach(var material in materials)
            {
                var property = new MaterialPropertyVisualElement(material);
                property.propertyChanged += MaterialPropertyChangedCallback;
                property.replace += MaterialReplaceCallback;
                property.select += MaterialSelectCallback;
                _tabs.Add(property);
            }
            AddExtraMaterialProperty();
        }

        private void MaterialSelectCallback(Material material)
        {
            if (!material) return;
            if (!_editor.IsHouseValid) return;

            _editor.SceneEditor.SelectAllOfMaterial(material);
        }

        private void MaterialReplaceCallback(Material material)
        {
            if (_selectedMaterials.Current >= 0 && _selectedMaterials.TabsCount > 0)
            {
                _editor.SceneEditor.ReplaceSelectionWithMaterial(_selectedMaterials.CurrentTab.material, material);
                OnSelectionChanged();
            }
        }

        private void MaterialPropertyChangedCallback(Material oldMaterial, Material newMaterial)
        {
            if (!oldMaterial)
            {
                AddExtraMaterialProperty();
            }
        }

        private void AddExtraMaterialProperty()
        {
            _extraProperty = new MaterialPropertyVisualElement(null);
            _extraProperty.propertyChanged += MaterialPropertyChangedCallback;
            _extraProperty.replace += MaterialReplaceCallback;
            _extraProperty.select += MaterialSelectCallback;
            _tabs.Add(_extraProperty);
        }

        public void OnSelectionChanged()
        {
            var modules = _editor.Selector.CurrentMultiple;
            List<Material> materials = _editor.House.GetModulesMaterials(modules);
            _selectedMaterials.ClearTabs();
            if (materials.Count == 0)
            {
                if (!_expandedContainer.Contains(_noSelectionsLabel)) _expandedContainer.Insert(0, _noSelectionsLabel);
                if (_expandedContainer.Contains(_selectedMaterials)) _expandedContainer.Remove(_selectedMaterials);
                return;
            }

            if (_expandedContainer.Contains(_noSelectionsLabel)) _expandedContainer.Remove(_noSelectionsLabel);
            if (!_expandedContainer.Contains(_selectedMaterials)) _expandedContainer.Insert(0, _selectedMaterials);
            foreach (var mat in materials)
            {
                var materialProperty = new MaterialPropertyVisualElement(mat);
                materialProperty.Remove(materialProperty.buttons);
                materialProperty.isChangeable = false;
                _selectedMaterials.AddTab(materialProperty);
            }
        }
    }
}