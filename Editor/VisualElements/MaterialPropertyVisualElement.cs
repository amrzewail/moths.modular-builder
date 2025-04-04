using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor
{
    public class MaterialPropertyVisualElement : VisualElement
    {
        public event Action<Material, Material> propertyChanged;

        private PropertyVisualElement<Material> _property;
        private VisualElement _buttons = new VisualElement();
        private Button _replaceBtn;
        private Button _selectBtn;

        private Material _currentMaterial;


        public Material material => _currentMaterial;
        public bool isChangeable { get; set; } = true;

        public VisualElement buttons => _buttons;

        public Button replaceBtn => _replaceBtn;
        public Button selectBtn => _selectBtn;

        public UnityEditor.UIElements.ObjectField propertyField => _property.propertyField;

        public Action<Material> replace;
        public Action<Material> select;

        public MaterialPropertyVisualElement(Material material)
        {
            AddToClassList("material-container");

            _currentMaterial = material;

            _property = new PropertyVisualElement<Material>(material);
            _property.deleteBtn.style.visibility = Visibility.Hidden;
            _property.propertyChanged += m => MaterialPropertyChangedCallback(_currentMaterial, m);
            _property.AddToClassList("material-tab");
            this.Add(_property);

            _buttons.AddToClassList("buttons");
            this.Add(_buttons);

            _selectBtn = new Button();
            _selectBtn.AddToClassList("select-btn");
            _selectBtn.text = "Select";
            _selectBtn.clicked += SelectClickCallback;
            _buttons.Add(_selectBtn);

            _selectBtn.style.visibility = _currentMaterial ? Visibility.Visible : Visibility.Hidden;

            _replaceBtn = new Button();
            _replaceBtn.AddToClassList("replace-btn");
            _replaceBtn.text = "Replace";
            _replaceBtn.clicked += ReplaceClickCallback;
            _buttons.Add(_replaceBtn);

            _replaceBtn.style.visibility = _currentMaterial ? Visibility.Visible : Visibility.Hidden;
        }

        private void ReplaceClickCallback()
        {
            if (!_currentMaterial) return;
            replace?.Invoke(_currentMaterial);
        }
        private void SelectClickCallback()
        {
            if (!_currentMaterial) return;
            select?.Invoke(_currentMaterial);
        }

        private bool MaterialPropertyChangedCallback(Material oldMaterial, Material material)
        {
            if (!isChangeable) return false;
            if (!material) return false;

            propertyChanged?.Invoke(oldMaterial, material);

            _currentMaterial = material;

            _replaceBtn.style.visibility = _currentMaterial ? Visibility.Visible : Visibility.Hidden;
            _selectBtn.style.visibility = _currentMaterial ? Visibility.Visible : Visibility.Hidden;

            return true;
        }
    }
}