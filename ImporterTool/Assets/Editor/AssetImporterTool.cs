using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetImporterTool : MonoBehaviour
{
    [MenuItem("Assets/Apply Import Settings")]
    static void ApplySettings()
    {
        //The path of the asset the user has selected
        string path = "Assets";     //AssetDatabase.GetAssetPath(Selection.activeObject);
        
        foreach (Object selectedObject in Selection.GetFiltered<Object>(SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(selectedObject);

            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                path = System.IO.Path.GetFileName(path);
                break;
            }
        }

        //Find all the textures and audio in this folder, including subfolders
        string[] assetGUIDs = AssetDatabase.FindAssets("t:texture t:audioclip", new[] { path });

        ImportSettings currentSettings;

        //Get the path of the asset from the GUID, then find the correct settings for that asset and apply them.
        foreach (string assetGUID in assetGUIDs)
        {
            //I know that in this current state, the FindImportSettings function is run for every asset which isn't optimal, this is just for testing.
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            currentSettings = FindImportSettings(assetPath);
            ApplyImportSettingsToAsset(assetPath, currentSettings);
        }
    }

    static void ApplyImportSettingsToAsset(string assetPath, ImportSettings importSettings)
    {
        //System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(assetPath);
        //string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        //ImportSettings importSettings = FindImportSettings(path);

        AssetImporter importer = AssetImporter.GetAtPath(assetPath);

        TextureImporter textureImporter;
        AudioImporter audioImporter;

        if (importSettings != null)
        {
            if (textureImporter = importer as TextureImporter)
            {
                textureImporter.filterMode = importSettings.TetxureFilterMode;
                textureImporter.maxTextureSize = (int)importSettings.MaxTextureSize;
                textureImporter.anisoLevel = importSettings.FilterLevel;
            }

            if (audioImporter = importer as AudioImporter)
            {
                AudioImporterSampleSettings audioImportSettings = new AudioImporterSampleSettings();
                audioImportSettings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
                audioImportSettings.loadType = AudioClipLoadType.CompressedInMemory;

                audioImporter.defaultSampleSettings = audioImportSettings;
            }
        }
        else
        {
            Debug.Log("Not import settings found");
        }

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    }

    static ImportSettings FindImportSettings(string pathToSearch)
    {
        //I'm not happy with using a string here and not allowing the user the freedom to rename the ImportSettings asset. 
        //I attempted to use AssetDatabase.FindAssets, but it does a BFS on the path it's assigned so it doesn't work with my upwards recursive approach        

        //Attempt to get the ImportSettings scriptable object at this path.
        ImportSettings importSettings = AssetDatabase.LoadAssetAtPath<ImportSettings>(pathToSearch + "/ImportSettings.asset");

        //Search recursively up the directory structure until we either find 
        //import settings or reach the main Assets folder with no results
        if (importSettings == null)
        {
            //We've reached the main Assets folder. Stop searching and warn the user that no settings were found
            if (pathToSearch == "Assets")
            {
                Debug.Log("No import settings founds. Use Right Click > Create > ImportSettings to create some");
            }
            else
            {
                //We didn't find anything in this directory. Let's try checking one level up.
                pathToSearch = System.IO.Directory.GetParent(pathToSearch).ToString();
                return FindImportSettings(pathToSearch);
            }
        }
        else //We found an ImportSettings object to use
        {
            Debug.Log("Found settings: " + pathToSearch);
            return importSettings;
        }

        return null;
    }
}
