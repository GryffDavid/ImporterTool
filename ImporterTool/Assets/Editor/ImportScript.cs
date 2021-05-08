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
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 1024;
    }
}