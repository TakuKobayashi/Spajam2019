using UnityEngine;
using UnityEditor;

namespace AutoSpriteDistributer
{
    public class CustomizeConfigEditor : EditorWindow
    {
        private int enableAutoAttachTag = 0;
        private int overwriteTagFlag = 0;
        private string spriteDirectoryPath = "Assets/AutoSpriteDistributer/Sprites/";

        [MenuItem("Tools/AutoSpriteDistributerConfig")]
        static void ShowSettingWindow()
        {
            EditorWindow.GetWindow(typeof(CustomizeConfigEditor));
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enable to automatically convert sprite file and attach sprite tag");
            enableAutoAttachTag = EditorGUILayout.Toggle(PlayerPrefs.GetInt(SpritePostprocessor.EnableAutomaticKey, 0) == 1) ? 1 : 0;
            PlayerPrefs.SetInt(SpritePostprocessor.EnableAutomaticKey, enableAutoAttachTag);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Overwrite tag?");
            overwriteTagFlag = EditorGUILayout.Toggle(PlayerPrefs.GetInt(SpritePostprocessor.ForceUpdateFlagKey, 0) == 1) ? 1 : 0;
            PlayerPrefs.SetInt(SpritePostprocessor.ForceUpdateFlagKey, overwriteTagFlag);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search texture files in root directory");
            spriteDirectoryPath = (string)EditorGUILayout.TextField(PlayerPrefs.GetString(SpritePostprocessor.SpriteRootDirectoryKey, spriteDirectoryPath));
            UnityEngine.Object spriteDirectoryObject = EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(spriteDirectoryPath), typeof(UnityEngine.Object), false);
            if(spriteDirectoryObject != null){
                spriteDirectoryPath = AssetDatabase.GetAssetPath(spriteDirectoryObject);
            }
            PlayerPrefs.SetString(SpritePostprocessor.SpriteRootDirectoryKey, spriteDirectoryPath);
            GUILayout.EndHorizontal();

            if(enableAutoAttachTag == 0){
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Convert And Attach")))
                {
                    SpritePostprocessor.ConvertAndAttachAllSprite(overwriteTagFlag == 1);
                }
                GUILayout.EndHorizontal();
            }

            PlayerPrefs.Save();
        }
    }
}