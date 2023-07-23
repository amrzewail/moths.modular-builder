using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Data
{
    [CreateAssetMenu(menuName = "HouseBuilder/Keymap")]
    public class Keymap : ScriptableObject
    {
        [System.Serializable]
        struct KeymapJson
        {
            public List<KeyBinding> bindings;
        }

        public int priority = 0;
        public InputContext context;
        public KeyBinding[] bindings;

        [ContextMenu("Export JSON")]
        public void ExportJson()
        {
            KeymapJson keymapJson;
            keymapJson.bindings = bindings.ToList();
            string json = JsonUtility.ToJson(keymapJson);
            string path = EditorUtility.SaveFilePanel("Save Keymap JSON", "", $"{context}_Keymap", "txt");
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, json);
                AssetDatabase.ImportAsset(FileUtil.GetProjectRelativePath(path));
            }
        }

        private void Reset()
        {
            KeymapJson defaultKeymap = JsonUtility.FromJson<KeymapJson>(Resources.Load<TextAsset>($"HouseBuilder/{context}_DefaultKeymap").text);
            bindings = defaultKeymap.bindings.ToArray();
            EditorUtility.SetDirty(this);
        }
    }
}