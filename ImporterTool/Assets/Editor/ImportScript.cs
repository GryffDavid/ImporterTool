using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportScript : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //I originally tried checking for a .meta file, but the OnPreprocessTexture runs after the .meta has been generated
         
        bool isNewFile = assetImporter.importSettingsMissing; //This returns true if there is no meta file with the asset.

        if (isNewFile)
        {
            TextureImporter importer = assetImporter as TextureImporter;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = 1024;
        }
    }
}