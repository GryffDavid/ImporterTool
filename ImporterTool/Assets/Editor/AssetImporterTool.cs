using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetImporterTool : MonoBehaviour
{
    [MenuItem("Assets/Apply Import Settings")]
    static void ApplySettings()
    {
        ImportSettings currentSettings;

        //The path of the asset the user has selected
        string path = "Assets";
        
        foreach (Object selectedObject in Selection.GetFiltered<Object>(SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(selectedObject);

            //The user has selected a file to apply the settings to
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                //Get the settings relevant to the file and apply them
                currentSettings = FindImportSettings(System.IO.Path.GetDirectoryName(path));
                ApplyImportSettingsToAsset(path, currentSettings);
                return;
            }
        }

        //Find all the textures and audio in this folder, including subfolders
        string[] assetGUIDs = AssetDatabase.FindAssets("t:texture t:audioclip", new[] { path });

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
                audioImportSettings.sampleRateSetting = importSettings.AudioSampleRate;
                audioImportSettings.loadType = importSettings.AudioLoadType;

                audioImporter.defaultSampleSettings = audioImportSettings;
            }
        }
        else
        {
            Debug.Log("Not import settings found");
        }

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    }

    /// <summary>
    /// Get an array of assets matching the type T in the path specified. Does not search child folders.
    /// </summary>
    /// <typeparam name="T">Type of asset to find</typeparam>
    /// <param name="path">Path to search in</param>
    /// <returns>Returns an array of assets</returns>
    public static Object[] GetAssetsOfTypeAtPzth<T>(string path)
    {
        List<Object> assetList = new List<Object>();
        string[] folderFiles = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(path));

        foreach (string file in folderFiles)
        {
            Object asset = AssetDatabase.LoadAssetAtPath(file, typeof(T));

            if (asset != null)
            {
                assetList.Add(asset);
            }
        }

        return assetList.ToArray();
    }

    /// <summary>
    /// Search up recursively for the ImportSettings object
    /// </summary>
    /// <param name="pathToSearch">Path to start searching at</param>
    /// <returns>Relevant Import Settings</returns>
    static ImportSettings FindImportSettings(string pathToSearch)
    {
        Object[] importSettings = GetAssetsOfTypeAtPzth<ImportSettings>(pathToSearch);

        //Attempt to get the ImportSettings scriptable object at this path.
        //ImportSettings importSettings = AssetDatabase.LoadAssetAtPath<ImportSettings>(pathToSearch + "/ImportSettings.asset");

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
            return (ImportSettings)importSettings[0];
        }

        return null;
    }
}
