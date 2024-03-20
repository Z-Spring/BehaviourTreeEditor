using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class EditorPlayController : UnityEditor.Editor
    {
        Button playBtn;
        Button pauseBtn;
        Button stepBtn;
        Texture2D playIcon;
        Texture2D pauseIcon;
        Texture2D stepIcon;

        public void Init(VisualElement root)
        {
            LoadIcon();
            SetButton(ref playBtn, EditorPlay, playIcon);
            SetButton(ref pauseBtn, EditorPause, pauseIcon);
            SetButton(ref stepBtn, EditorStep, stepIcon);
            ToolbarAddButton(root);
        }

        void LoadIcon()
        {
            playIcon = AssetResourceManager.LoadAsset<Texture2D>(AssetResourceManager.PlayIconPath);
            pauseIcon = AssetResourceManager.LoadAsset<Texture2D>(AssetResourceManager.PauseIconPath);
            stepIcon = AssetResourceManager.LoadAsset<Texture2D>(AssetResourceManager.StepIconPath);
        }

        void SetButton(ref Button button, Action clickAction, Texture2D icon)
        {
            button = new Button(clickAction)
            {
                style =
                {
                    backgroundImage = new StyleBackground(icon),
                    width = 20,
                    height = 18,
                    marginLeft = 0,
                    marginRight = 0
                }
            };
        }

        void ToolbarAddButton(VisualElement root)
        {
            var bottomToolbar = root.Q<Toolbar>("BottomToolbar");
            bottomToolbar.Add(playBtn);
            bottomToolbar.Add(pauseBtn);
            bottomToolbar.Add(stepBtn);
        }


        void EditorPlay()
        {
            EditorApplication.isPlaying = !EditorApplication.isPlaying;
        }

        void EditorPause()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPaused = !EditorApplication.isPaused;
            }
        }

        void EditorStep()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.Step();
            }
        }
    }
}