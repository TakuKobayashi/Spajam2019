using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace AutoSpriteDistributer{
    public class SpritePostprocessor : AssetPostprocessor
    {
        public const string EnableAutomaticKey = "AutoSpriteDistributer_Enable_Automatic";
        public const string SpriteRootDirectoryKey = "AutoSpriteDistributer_Sprite_Root_Directory";
        public const string ForceUpdateFlagKey = "AutoSpriteDistributer_Force_Update_Flag";

        /// <summary>
        /// <para>Called just before the texture file is imported.</para>
        /// </summary>
        private void OnPreprocessTexture()
        {
            if (PlayerPrefs.GetInt(EnableAutomaticKey, 0) == 0) return;
            string spriteDirPath = PlayerPrefs.GetString(SpriteRootDirectoryKey);
            if (!this.assetImporter.assetPath.Contains(spriteDirPath))
            {
                return;
            }
            UpdatePackingSpriteInfo();
        }

        /// <summary>
        /// <para>The texture file convert to sprite and add tag by folder name to all Sprites</para>
        /// </summary>
        public static void ConvertAndAttachAllSprite(bool overwriteTag = false)
        {
            List<TextureImporter> importList = new List<TextureImporter>();
            string spriteDirPath = PlayerPrefs.GetString(SpriteRootDirectoryKey);
            string[] pathes = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < pathes.Length; ++i)
            {
                string path = pathes[i];
                if (path.Contains(spriteDirPath))
                {
                    TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
                    if(importer != null){
                        if(!overwriteTag && !string.IsNullOrEmpty(importer.spritePackingTag))
                        {
                            continue;
                        }
                        importList.Add(importer);
                    }
                }
            }
            AssetDatabase.StartAssetEditing();
            for (int i = 0; i < importList.Count; ++i)
            {
                ConvertSpriteAndAttachTag(importList[i]);
            }
            AssetDatabase.StopAssetEditing();
            for (int i = 0; i < importList.Count; ++i)
            {
                EditorUtility.SetDirty(importList[i]);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //Edit the setting to be imported texture file so that it can be packed with SpritePacker.
        private void UpdatePackingSpriteInfo()
        {
            string spriteDirPath = PlayerPrefs.GetString(SpritePostprocessor.SpriteRootDirectoryKey);
            //Class to set Texture file on import.
            TextureImporter textureImporter = this.assetImporter as TextureImporter;
            //Ignore others except the target directories.
            if (!textureImporter.assetPath.Contains(spriteDirPath) || !string.IsNullOrEmpty(textureImporter.spritePackingTag))
            {
                return;
            }
            ConvertSpriteAndAttachTag(textureImporter);
        }

        private static void ConvertSpriteAndAttachTag(TextureImporter textureImporter){
            //Get the parent directory name
            string directoryName = Path.GetFileName(Path.GetDirectoryName(textureImporter.assetPath));

            //setting the texture
            textureImporter.textureType = TextureImporterType.Sprite; //change textureType to Sprite

            //Add only PackingTag for textures not sliced.
            Vector4 borderVec = textureImporter.spriteBorder;
            bool slice = (borderVec.magnitude > 0f) ? true : false;
            if (!slice)
            {
                textureImporter.spritePackingTag = directoryName;              //Set the tag by directory name
            }
        }
    }
}