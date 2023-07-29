using HouseBuilder.Editor.Controllers;
using HouseBuilder.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

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

            Button moduleRuleBtn = new Button();
            moduleRuleBtn.text = "Make rule";
            moduleRuleBtn.clicked += ModuleRuleCallback;
            this.Add(moduleRuleBtn);


            _warningLabel = new Label();
            //_warningLabel.AddToClassList("no-palette-sets-label");
        }

        private void ModuleRuleCallback()
        {
            var selections = _editor.Selector.CurrentMultiple;
            Vector3 forward = _editor.Grid.rotation * Vector3.forward;
            Vector3 right = _editor.Grid.rotation * Vector3.right;

            List<RuleModule> rules = new List<RuleModule>();

            foreach(var m in selections)
            {
                RuleModule ruleModule = new RuleModule();
                ruleModule.ruleId = Guid.NewGuid().ToString();
                ruleModule.prefab = PrefabUtility.GetCorrespondingObjectFromSource(m);
                ruleModule.rotation = m.transform.localRotation;
                ruleModule.scale = m.transform.localScale;
                ruleModule.rules = new List<Rule>();

                GameObject rightModule = GetAtDirection(m, right);
                if (rightModule)
                {
                    Rule rule;
                    rule.direction = RuleDirection.Right;
                    rule.condition = RuleCondition.Exists;
                    rule.moduleType = _editor.House.GetModuleType(rightModule);
                    rule.unitDistance = Mathf.RoundToInt(Vector3.Distance(rightModule.transform.position, m.transform.position) / _editor.House.gridSize.x);
                    ruleModule.rules.Add(rule);
                }

                GameObject leftModule = GetAtDirection(m, -right);

                if (leftModule)
                {
                    Rule rule;
                    rule.direction = RuleDirection.Left;
                    rule.condition = RuleCondition.Exists;
                    rule.moduleType = _editor.House.GetModuleType(leftModule);
                    rule.unitDistance = Mathf.RoundToInt(Vector3.Distance(leftModule.transform.position, m.transform.position) / _editor.House.gridSize.x);
                    ruleModule.rules.Add(rule);
                }

                GameObject forwardModule = GetAtDirection(m, forward);

                if (forwardModule)
                {
                    Rule rule;
                    rule.direction = RuleDirection.Forward;
                    rule.condition = RuleCondition.Exists;
                    rule.moduleType = _editor.House.GetModuleType(forwardModule);
                    rule.unitDistance = Mathf.RoundToInt(Vector3.Distance(forwardModule.transform.position, m.transform.position) / _editor.House.gridSize.y);
                    ruleModule.rules.Add(rule);
                }

                GameObject backwardModule = GetAtDirection(m, -forward);

                if (backwardModule)
                {
                    Rule rule;
                    rule.direction = RuleDirection.Backward;
                    rule.condition = RuleCondition.Exists;
                    rule.moduleType = _editor.House.GetModuleType(backwardModule);
                    rule.unitDistance = Mathf.RoundToInt(Vector3.Distance(backwardModule.transform.position, m.transform.position) / _editor.House.gridSize.y);
                    ruleModule.rules.Add(rule);
                }

                rules.Add(ruleModule);
            }

            rules = rules.OrderByDescending(x => x.rules.Count).ToList();

            RuleModules obj = Resources.LoadAll<RuleModules>("")[0];
            obj.modulingRules = rules;
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssetIfDirty(obj);
            AssetDatabase.Refresh();
        }

        private GameObject GetAtDirection(GameObject m, Vector3 direction)
        {
            direction.Normalize();
            GameObject module = _editor.House.GetFirstByQuery(x => (x.transform.position - m.transform.position - direction).magnitude < _editor.Grid.gridSize.x / 4f);
            if (!module)
            {
                module = _editor.House.GetFirstByQuery(x => (x.transform.position - m.transform.position - direction * 2).magnitude < _editor.Grid.gridSize.x / 4f);
            }
            return module;
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