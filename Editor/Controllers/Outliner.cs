using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Moths.ModularBuilder.Editor.Controllers
{
    public class Outliner : IOutliner
    {
        public struct Outline
        {
            public MeshFilter[] filters;
			public Material material;

            public Outline(MeshFilter[] renderers, Material material)
            {
                this.filters = renderers;
				this.material = material;

            }
        }

        private IEditor _editor;
        private Material _outlineMaterial;
        private Dictionary<GameObject, Outline> _outlinedObjects = new Dictionary<GameObject, Outline>();


		public Outliner(IEditor editor)
        {
            _editor = editor;

			_editor.OnSceneGUI += SceneGUI;

			InitMaterial();
		}

		private void InitMaterial()
        {
			_outlineMaterial = new Material(Shader.Find("Hidden/ModularBuilderHighlight"));
		}

		private void SceneGUI(SceneView view)
        {
			if (Event.current.type != EventType.Repaint) return;

			var cam = view.camera;

			foreach (var collection in _outlinedObjects)
			{
				Material mat = collection.Value.material;
				foreach (var filter in collection.Value.filters)
				{
					if (!filter)
                    {
						_outlinedObjects.Remove(collection.Key);
						return;
                    }
					Matrix4x4 TRS = Matrix4x4.TRS(filter.transform.position, filter.transform.rotation, filter.transform.localScale);

					Graphics.DrawMesh(filter.sharedMesh, TRS, mat, filter.gameObject.layer, cam);

				}
			}
		}

        public void Cleanup()
        {
			RemoveAll();
        }

        public void AddGameObject(GameObject go, Color color)
        {
			if (!go) return;
			if (_outlinedObjects.ContainsKey(go)) return;
			if (!_outlineMaterial)
            {
				InitMaterial();
			}
			Material mat = new Material(_outlineMaterial);
			mat.SetColor("_Color", color);
            _outlinedObjects[go] = new Outline(go.GetComponentsInChildren<MeshFilter>(), mat);
			
			//_editor.Logger.Log(nameof(Outliner), $"Outline object {go.name}");
		}

		public void RemoveGameObject(GameObject go)
        {
			if (!go) return;
			if (!_outlinedObjects.ContainsKey(go)) return;
            _outlinedObjects.Remove(go);
			
			//_editor.Logger.Log(nameof(Outliner), $"Remove outline object {go.name}");
		}

		public void RemoveAll()
        {
            _outlinedObjects.Clear();
        }

	}
}