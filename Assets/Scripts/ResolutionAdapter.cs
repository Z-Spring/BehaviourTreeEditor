using UnityEngine;

public class ResolutionAdapter : MonoBehaviour
{
    public float targetAspectRatio = 16f / 9f; // 目标宽高比
    public float letterboxPadding = 0.1f; // 上下留白的比例

    void Start()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            float currentAspectRatio = (float)Screen.width / Screen.height;
            float scaleHeight = currentAspectRatio / targetAspectRatio;

            Rect cameraRect = mainCamera.rect;

            // 如果当前宽高比小于目标宽高比，说明屏幕比较矮，上下留白
            if (scaleHeight < 1.0f)
            {
                cameraRect.height = scaleHeight;
                cameraRect.y = (1.0f - scaleHeight) / 2.0f;
            }
            else // 如果当前宽高比大于目标宽高比，说明屏幕比较宽，左右留白
            {
                float scaleWidth = 1.0f / scaleHeight;
                cameraRect.width = scaleWidth;
                cameraRect.x = (1.0f - scaleWidth) / 2.0f;
            }

            mainCamera.rect = cameraRect;
        }
    }
}
