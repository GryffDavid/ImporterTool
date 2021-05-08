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

        AssetImporter importer = AssetImporter.GetAtPath(path);

        TextureImporter textureImporter;
        AudioImporter audioImporter;

        if (textureImporter = importer as TextureImporter)
        {
            textureImporter.filterMode = FilterMode.Trilinear;
            textureImporter.maxTextureSize = 1024;
            textureImporter.anisoLevel = 16;
        }

        if (audioImporter = importer as AudioImporter)
        {
            AudioImporterSampleSettings audioImportSettings = new AudioImporterSampleSettings();
            audioImportSettings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            audioImportSettings.loadType = AudioClipLoadType.CompressedInMemory;

            audioImporter.defaultSampleSettings = audioImportSettings;
        }

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}
