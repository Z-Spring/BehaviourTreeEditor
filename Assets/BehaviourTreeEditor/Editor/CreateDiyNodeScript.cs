using UnityEditor;

namespace Editor
{
    public class CreateDiyNodeScript
    {
        [MenuItem("Assets/Create/DIYNode")]
        public static void Create()
        {
            // add script
            string templatePath = "Assets/Editor/DiyNodeTemplate.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile( templatePath,  "NewNode.cs");
        }
    }
}