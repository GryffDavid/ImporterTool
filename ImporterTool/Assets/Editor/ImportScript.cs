using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ImportScript : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;
        String name = importer.assetPath.ToLower();

        if (name.Substring(name.Length - 4, 4) == ".png")
        {
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = 1024;
        }
    }
}