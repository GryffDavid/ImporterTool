using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetImporterTool : MonoBehaviour
{
    [MenuItem("Assets/Apply Import Settings")]
    static void ApplyImportSettings()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.filterMode = FilterMode.Trilinear;
        importer.maxTextureSize = 1024;

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}
