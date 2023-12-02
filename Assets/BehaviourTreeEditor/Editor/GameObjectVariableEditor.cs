using System;
using BehaviourTreeEditor.BTree;
using BehaviourTreeEditor.SharedVariables;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    // [CustomEditor(typeof(SharedGameObject))]
    public class GameObjectVariableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 获取当前的 ScriptableObject
            SharedGameObject gameObjectVariable = (SharedGameObject)target;

            // 获取所有 GameObject 的列表
            GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
            
            
            
            // 创建一个字符串列表，用于下拉菜单
            string[] options = new string[allGameObjects.Length];
            for (int i = 0; i < allGameObjects.Length; i++)
            {
                options[i] = allGameObjects[i].name;
            }

            // 获取当前选中的 GameObject 的索引
            int currentIndex = Array.IndexOf(allGameObjects, gameObjectVariable.sharedValue);

            // 创建下拉菜单
            int selectedIndex = EditorGUILayout.Popup("GameObject", currentIndex, options);

            // 更新选中的 GameObject
            if (selectedIndex >= 0)
            {
                gameObjectVariable.sharedValue = allGameObjects[selectedIndex];
            }

            // 保存更改
            if (GUI.changed)
            {
                EditorUtility.SetDirty(gameObjectVariable);
            }
        }
    }
    
}