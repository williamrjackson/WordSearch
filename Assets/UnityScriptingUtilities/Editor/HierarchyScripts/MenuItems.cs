using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuItems : MonoBehaviour
{
    [MenuItem("Wrj/Number Children")]
    static void NumberChildren()
    {
        if (Selection.transforms.Length > 1)
        {
            Transform parent = Selection.activeTransform.parent;
            List<Transform> modifiables = new List<Transform>();
            foreach (var item in Selection.transforms)
            {
                if (item.parent != parent)
                {
                    Debug.Log("Multiple parents found.");
                    return;
                }
                modifiables.Add(item);
            }
            int index = 0;
            foreach (Transform child in parent)
            {
                if (modifiables.Contains(child))
                {
                    child.name = child.name + " " + index++;
                }
            }
            return;
        }
        foreach (Transform child in Selection.activeTransform)
        {
            child.name = child.name + " " + child.GetSiblingIndex();
        }
    }
}
