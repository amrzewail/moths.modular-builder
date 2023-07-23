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

        public KeyCommand Command => command;
        public KeyBehaviour Behaviour => behaviour;
        public KeyCode Key => key;
        public MouseButton Mouse => mouse;
        public bool Alt => alt;
        public bool Shift => shift;
        public bool Ctrl => ctrl;
    }
}