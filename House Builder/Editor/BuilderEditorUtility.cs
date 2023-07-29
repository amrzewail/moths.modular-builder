using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor
{
    public static class BuilderEditorUtility
    {
        public static float Snap(float value, float snap, float offset)
        {
            if (snap == 0) return value;

            value = Mathf.Round((value - offset) / snap) * snap + offset;

            return value;
        }


        public static Vector3 MouseToWorldPosition(SceneView view, Vector3 origin)
        {
            Camera cam = view.camera;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 direction = ray.direction;

            float y = cam.transform.position.y - origin.y;
            float theta = Vector3.Angle(direction, Vector3.down) * Mathf.Deg2Rad;
            float distance = y / Mathf.Cos(theta);

            Vector3 position = cam.transform.position + direction * distance;

            return position;

        } 


        public static async Task<Texture2D> GetAssetTexturePreview(Object obj)
        {
            if (obj is GameObject)
            {
                if (((GameObject)obj).TryGetComponent<HouseModule>(out HouseModule h))
                {
                    obj = h.previewGameObject;
                }
            }

            Texture2D texture = null;
            while (texture == null)
            {
                texture = AssetPreview.GetAssetPreview(obj);
                if (texture == null) await System.Threading.Tasks.Task.Delay(100);
            }
            return texture;
        }

        public static void SaveAssetChanges(Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssetIfDirty(obj);
        }

    }
}