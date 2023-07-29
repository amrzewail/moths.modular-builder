using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HouseBuilder.Editor
{
    public class PaletteSetListElement : VisualElement
    {
        public string text { get => _label.text; set=> _label.text = value; }
        public PaletteSet target { get; set; }
        public event Action<PaletteSetListElement> onEdit;
        public event Action<PaletteSetListElement> onHighlight;

        private Label _label;
        private VisualElement _buttons;
        private Button _editBtn;
        private Button _highlightBtn;

        public PaletteSetListElement()
        {

            _label = new Label();
            _buttons = new VisualElement();
            _editBtn = new Button();
            _highlightBtn = new Button();

            _editBtn.text = "Edit";
            _highlightBtn.text = "Highlight";

            _editBtn.clicked += ClickCallback;
            _highlightBtn.clicked += HighlightCallback;

            this.Add(_label);
            this.Add(_buttons);
            _buttons.Add(_editBtn);
            _buttons.Add(_highlightBtn);

            this.AddToClassList("palette-set-element");
            _buttons.AddToClassList("palette-set-element-buttons");
            _label.AddToClassList("palette-set-element-label");
            _editBtn.AddToClassList("palette-set-element-edit");
            _highlightBtn.AddToClassList("palette-set-element-highlight");
        }

        private void HighlightCallback()
        {
            UnityEditor.EditorGUIUtility.PingObject(target);

            onHighlight?.Invoke(this);
        }

        private void ClickCallback()
        {
            onEdit?.Invoke(this);
        }
    }
}