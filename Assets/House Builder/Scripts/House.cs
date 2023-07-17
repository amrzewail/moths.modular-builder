using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder
{
    public class House : MonoBehaviour, IHouse
    {
        public bool hasReference => this;
        public Vector3 origin => transform.position;

        [SerializeField] Vector3 _gridSize = Vector3.one;
        [SerializeField] float _levelHeight = 3;

        public Vector3 gridSize => _gridSize;
        public float levelHeight => _levelHeight;

        public void Add(ModuleType type, int level, GameObject module)
        {
            Transform levelChild = transform.Find($"Level{level}");
            if (!levelChild)
            {
                levelChild = new GameObject($"Level{level}").transform;
                levelChild.SetParent(transform);
                levelChild.localPosition = Vector3.zero;
            }
            Transform child = levelChild.Find(type.ToString());
            if (child == null)
            {
                child = new GameObject(type.ToString()).transform;
                child.SetParent(levelChild);
                child.localPosition = Vector3.zero;
            }
            module.transform.SetParent(child, true);
        }

        public GameObject GetAtPosition(ModuleType type, int level, Vector3 worldPosition, float precision)
        {
            Transform levelChild = transform.Find($"Level{level}");
            if (levelChild)
            {
                Transform child = levelChild.Find(type.ToString());
                if (child)
                {
                    for (int i = 0; i < child.childCount; i++)
                    {
                        if (Vector3.Distance(child.GetChild(i).position, worldPosition) < precision) return child.GetChild(i).gameObject;
                    }
                }
            }
            return null;

        }

        public void HideLevelRange(int minInclusive, int maxExlusive)
        {
            int childCount = transform.childCount;
            int levelTxtLength = "Level".Length;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                int childLevel = int.Parse(child.name.Substring(levelTxtLength, child.name.Length - levelTxtLength));
                child.gameObject.SetActive(childLevel < minInclusive || childLevel >= maxExlusive);
            }
        }

        public void ShowAllLevels()
        {
            if (!transform) return;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
        }

        public List<GameObject> GetAllAtHeight(ModuleType type, int level, int heightIndex)
        {
            Transform levelChild = transform.Find($"Level{level}");
            List<GameObject> list = new List<GameObject>();
            if (levelChild)
            {
                Transform child = levelChild.Find(type.ToString());
                if (child)
                {
                    for (int i = 0; i < child.childCount; i++)
                    {
                        if (Mathf.Abs(child.GetChild(i).transform.localPosition.y - (levelHeight * level + heightIndex * gridSize.y)) > 0.1f) continue;
                        list.Add(child.GetChild(i).gameObject);
                    }
                }
            }
            return list;
        }
    }
}