using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTreeEditor.BTree;
using BehaviourTreeEditor.BTree.ActionNodes;
using BehaviourTreeEditor.BTree.ParentSharedVariable;
using BehaviourTreeEditor.SharedVariables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor
{
    public class CreateSharedVariableEditor
    {
        BTreeEditor bTreeEditor;
        readonly List<UnityEditor.Editor> editorList = new();

        readonly Dictionary<SharedVariable, IMGUIContainer> sharedVariableContainerDic = new();

        // SharedVariableContainer sharedVariableContainer;
        IMGUIContainer variableContainer;
        ScrollView scrollView;

        readonly Dictionary<string, Type> typeMap = new()
        {
            { "string", typeof(SharedString) },
            { "int", typeof(SharedInt) },
            { "float", typeof(SharedFloat) },
            { "bool", typeof(SharedBool) },
            { "Vector2", typeof(SharedVector2) },
            { "Vector3", typeof(SharedVector3) },
            { "Quaternion", typeof(SharedQuaternion) },
            { "Color", typeof(SharedColor) },
            { "Rect", typeof(SharedRect) },
            { "GameObject", typeof(SharedGameObject) },
            { "Transform", typeof(SharedTransform) },
            { "Texture2D", typeof(SharedTexture2D) },
            { "Sprite", typeof(SharedSprite) },
            { "Material", typeof(SharedMaterial) },
            { "AnimationCurve", typeof(SharedAnimationCurve) },
            { "Object", typeof(SharedObject) },
            { "Collider", typeof(SharedCollider) },
            { "LayerMask", typeof(SharedLayerMask) }
        };

        public CreateSharedVariableEditor(BTreeEditor bTreeEditor)
        {
            this.bTreeEditor = bTreeEditor;
        }


        public void CreateSharedVariableDropdownField(VisualElement root)
        {
            DropdownField field = root.Q<DropdownField>();
            field.choices.Clear();
            field.choices.AddRange(typeMap.Keys);
            string firstValue = field.choices.FirstOrDefault();
            field.SetValueWithoutNotify(firstValue);
        }

        public void AddVariable(VisualElement root, SharedVariableContainer sharedVariableContainer)
        {
            InitializeEditorWindowElements(root);
            var textField = variableContainer.Q<TextField>();
            var addVariableButton = variableContainer.Q<Button>();
            addVariableButton.clicked += () =>
            {
                var (variableName, typeSelected) = GetVariableNameAndSelectedType(textField, root);
                if (variableName == string.Empty)
                {
                    EditorUtility.DisplayDialog("Error", "Variable name cannot be empty.", "OK");
                    return;
                }

                var sharedVariable =
                    CreateSharedVariableScriptableObject(variableName, textField, typeSelected,
                        sharedVariableContainer);
                DisplaySharedVariableInIMGUI(sharedVariable);
            };
        }

        void InitializeEditorWindowElements(VisualElement root)
        {
            variableContainer = root.Q<IMGUIContainer>("AddVariableContainer");
            scrollView = root.Q<ScrollView>();
        }

        (string, string) GetVariableNameAndSelectedType(TextField textField, VisualElement root)
        {
            string variableName = textField.text;
            string typeSelected = root.Q<DropdownField>().text;
            return (variableName, typeSelected);
        }

        SharedVariable CreateSharedVariableScriptableObject(string variableName, TextField textField,
            string typeSelected, SharedVariableContainer sharedVariableContainer)
        {
            Type type = typeMap[typeSelected];
            SharedVariable sharedVariable = ScriptableObject.CreateInstance(type) as SharedVariable;
            sharedVariable.name = variableName;
            AssetDatabase.AddObjectToAsset(sharedVariable, sharedVariableContainer);
            SharedVariablePopupEditor.AddSharedVariable(sharedVariable);
            textField.SetValueWithoutNotify(string.Empty);
            return sharedVariable;
        }

        void DisplaySharedVariableInIMGUI(SharedVariable variable)
        {
            IMGUIContainer newIMGUIContainer = new IMGUIContainer()
            {
                style =
                {
                    marginBottom = 10,
                    marginRight = 15
                }
            };

            sharedVariableContainerDic[variable] = newIMGUIContainer;

            UnityEditor.Editor currentEditor = null;
            UpdateEditor(variable, ref currentEditor);
            NewIMGUIContainerHandler(newIMGUIContainer, currentEditor, variable);

            scrollView.Add(newIMGUIContainer);
        }

        void UpdateEditor(SharedVariable variable, ref UnityEditor.Editor currentEditor)
        {
            int editorIndex = editorList.FindIndex(editor => editor.target == variable);

            if (editorIndex < 0)
            {
                currentEditor = UnityEditor.Editor.CreateEditor(variable);
                editorList.Add(currentEditor);
            }
            else
            {
                UnityEditor.Editor.CreateCachedEditor(variable, null, ref currentEditor);
                editorList[editorIndex] = currentEditor;
            }
        }

        void NewIMGUIContainerHandler(IMGUIContainer newIMGUIContainer, UnityEditor.Editor currentEditor,
            SharedVariable variable)
        {
            if (currentEditor == null)
            {
                Debug.Log("currentEditor is null");
                return;
            }

            newIMGUIContainer.onGUIHandler = () =>
            {
                if (currentEditor.target != null)
                {
                    currentEditor.OnInspectorGUI();
                    LayoutDeleteVariableButton(variable, currentEditor);
                    DrawSeparator();
                }
            };
        }

        void LayoutDeleteVariableButton(SharedVariable variable, UnityEditor.Editor currentEditor)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Variable", GUILayout.ExpandWidth(true)))
            {
                //调用删除函数
                DeleteVariable(variable, editorList, currentEditor);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        void DrawSeparator()
        {
            Rect separatorRect =
                EditorGUILayout.GetControlRect(false, 1.5f); // 1 is the height of line
            GUI.Box(separatorRect, GUIContent.none);
        }

        void DeleteVariable(SharedVariable variable,
            List<UnityEditor.Editor> el, UnityEditor.Editor currentEditor)
        {
            AssetDatabase.RemoveObjectFromAsset(variable);
            el.Remove(currentEditor);
            Object.DestroyImmediate(currentEditor);
            if (sharedVariableContainerDic.TryGetValue(variable, out IMGUIContainer container))
            {
                scrollView.Remove(container);
                sharedVariableContainerDic.Remove(variable);
                SharedVariablePopupEditor.RemoveSharedVariable(variable);
            }
        }


        public SharedVariableContainer LoadSharedVariableContainerAsset(string treeName)
        {
            // string treeName = GetTreeName();
            string assetPath = AssetResourceManager.GetSharedVariableContainerAssetPath(treeName);

            SharedVariableContainer sv = AssetResourceManager.LoadAsset<SharedVariableContainer>(assetPath);
            if (sv == null)
            {
                AssetResourceManager.CreateScriptObjectAsset<SharedVariableContainer>(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return sv;
        }

        public void InitializeVariableEditors(string treeName,VisualElement root)
        {
            // editorList.Clear();
            // scrollView = root.Q<ScrollView>();
            // scrollView.Clear();
            // if (sv.sharedVariables.Count > 0)
            //     sv.sharedVariables.ForEach(v => Debug.Log(v.name));
            //     // sv.sharedVariables.ForEach(DisplaySharedVariableInIMGUI);
            // else
            // {
            //     Debug.Log("sv.sharedVariables.Count <= 0");
            // }
            
            editorList.Clear();
            scrollView = root.Q<ScrollView>();
            scrollView.Clear();
            string assetPath = AssetResourceManager.GetSharedVariableContainerAssetPath(treeName);
            var variables = AssetResourceManager.LoadAllAssets<SharedVariable>(assetPath);

            foreach (var variable in variables)
            {
                DisplaySharedVariableInIMGUI(variable);
            }
        }
    }
}