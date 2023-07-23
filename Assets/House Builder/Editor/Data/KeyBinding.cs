using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Data
{
    [System.Serializable]
    public struct KeyBinding
    {
        [SerializeField] KeyCommand command;
        [SerializeField] KeyBehaviour behaviour;
        [SerializeField] KeyCode key;
        [SerializeField] MouseButton mouse;
        [SerializeField] bool alt;
        [SerializeField] bool ctrl;
        [SerializeField] bool shift;
        [SerializeField] bool consumeEvent;

        public KeyCommand Command => command;
        public KeyBehaviour Behaviour => behaviour;
        public KeyCode Key => key;
        public MouseButton Mouse => mouse;
        public bool Alt => alt;
        public bool Shift => shift;
        public bool Ctrl => ctrl;

        public bool Consume => consumeEvent;

        public override string ToString()
        {
            string txt = "";
            txt += shift ? "Shift " : "";
            txt += ctrl ? "Ctrl " : "";
            txt += alt ? "Alt " : "";
            txt += Key != KeyCode.None ? $"{Key} " : "";
            txt += Mouse != MouseButton.None ? $"{Mouse.MouseButtonString()} " : "";
            txt += Behaviour == KeyBehaviour.ScrollWheel ? "Scroll Wheel " : "";
            txt += $": {Command.CommandString()}";
            return txt;
        }
    }
}