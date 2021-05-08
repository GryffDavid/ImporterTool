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

        FindImportSettings(path); //Just testing 

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

    static void FindImportSettings(string pathToSearch)
    {
        string[] importSettingsGUIDs = AssetDatabase.FindAssets("t:ImportSettings", new[] { pathToSearch });

        if (importSettingsGUIDs.Length == 0)
        {
            //We've reached the main Assets folder. Stop searching and warn theuser that no settings wered found
            if (pathToSearch == "Assets")
            {
                Debug.LogWarning("No import settings found. Use Right Click > Create > Import Settings to create some");
            }
            else
            {
                pathToSearch = System.IO.Directory.GetParent(pathToSearch).ToString();
                FindImportSettings(pathToSearch);
            }
        }
        else //We found an ImportSettings object to use
        {
            foreach (string guid in importSettingsGUIDs)
            {
                string settingsPath = AssetDatabase.GUIDToAssetPath(guid);
                ImportSettings settingsObject = AssetDatabase.LoadAssetAtPath<ImportSettings>(settingsPath);
                Debug.Log($"Found settings: {settingsPath}");
            }
        }
    }
}
