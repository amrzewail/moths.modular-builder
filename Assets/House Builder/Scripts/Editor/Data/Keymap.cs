using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Data
{
    [CreateAssetMenu(menuName = "HouseBuilder/Keymap")]
    public class Keymap : ScriptableObject
    {
        public KeyBinding[] bindings;

        [ContextMenu("Export JSON")]
        public void ExportJson()
        {
            string json = JsonUtility.ToJson(this);
            Debug.Log(json);
        }

        private void Reset()
        {
            
        }
    }
}