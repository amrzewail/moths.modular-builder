using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Moths.ModularBuilder.Editor
{
    public class ExpandableElement : VisualElement
    {
        private VisualElement _header;
        private Button _titleBtn;
        private VisualElement _expandableContainer;
        private bool _isExpanded;

        public bool isExpanded => _isExpanded;

        protected VisualElement expandableElement
        {
            get => _expandableContainer;
            set
            {
                if (_expandableContainer != null) _expandableContainer.RemoveFromClassList("expandable-container");
                _expandableContainer = value;
                _expandableContainer.AddToClassList("expandable-container");
            }
        }

        public string title
        {
            get => _titleBtn.text;
            set => _titleBtn.text = value;
        }

        public ExpandableElement()
        {
            this.AddToClassList("expandable-element");

            _header = new VisualElement();
            _header.AddToClassList("header");
            this.Add(_header);

            _titleBtn = new Button();
            _titleBtn.AddToClassList("title-btn");
            _titleBtn.clicked += ExpandClickCallback;

            _header.Add(_titleBtn);

            UpdateExpand(true);
        }


        private void ExpandClickCallback()
        {
            _isExpanded = !_isExpanded;

            UpdateExpand(_isExpanded);


            OnExpandUpdated();
        }

        protected virtual void OnExpandUpdated()
        {

        }

        public void UpdateExpand(bool isExpanded)
        {
            _isExpanded = isExpanded;
            if (expandableElement == null) return;
            if (_isExpanded)
            {
                if (expandableElement.ClassListContains("unexpanded")) expandableElement.RemoveFromClassList("unexpanded");
            }
            else
            {
                if (!expandableElement.ClassListContains("unexpanded")) expandableElement.AddToClassList("unexpanded");
            }
        }

    }
}