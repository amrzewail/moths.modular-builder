using System.Collections;
using System.Collections.Generic;
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

    }
}