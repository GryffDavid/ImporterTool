using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetImporterTool : MonoBehaviour
{
    [MenuItem("Assets/Apply Import Settings")]
    static void ApplySettings()
    {
        ImportSettings currentSettings;

        //The path of the asset the user has selected
        string path = "Assets";
        
        //TODO: If multiple files are selected here, only the first will have settings applied
        foreach (Object selectedObject in Selection.GetFiltered<Object>(SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(selectedObject);

            //The user has selected a file to apply the settings to
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                //Get the settings relevant to the file and apply them
                currentSettings = FindImportSettings(Path.GetDirectoryName(path));
                ApplyImportSettingsToAsset(path, currentSettings);
                return;
            }
        }

        //Find all the textures and audio in this folder, including subfolders
        string[] assetGUIDs = AssetDatabase.FindAssets("t:texture t:audioclip", new[] { path });

        //Use a HashSet to get a list of unique folders where assets reside. 
        //Then loop through each asset in each unique folder, applying the relevant settings
        HashSet<string> assetFolders = new HashSet<string>();
        foreach (string assetGUID in assetGUIDs)
        {            
            assetFolders.Add(Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(assetGUID)));
        }

        foreach (string folder in assetFolders)
        {
            Object[] assetsInFolder = GetAssetsOfTypeAtPzth<Object>(folder);
            ImportSettings newSettings = FindImportSettings(folder);

            if (newSettings != null)
            {
                foreach (Object asset in assetsInFolder)
                {
                    ApplyImportSettingsToAsset(AssetDatabase.GetAssetPath(asset.GetInstanceID()), newSettings);
                }
            }
        }
    }

    public static void ApplyImportSettingsToAsset(string assetPath, ImportSettings importSettings)
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
                AudioImporterSampleSettings audioImportSettings = new AudioImporterSampleSettings()
                {
                    sampleRateSetting = importSettings.AudioSampleRate,
                    loadType = importSettings.AudioLoadType
                };

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
        //If this path is actually to a file, make sure to just get the directory.
        if (File.Exists(path))
        {
            path = Path.GetDirectoryName(path);
        }

        List<Object> assetList = new List<Object>();
        string[] folderFiles = Directory.GetFiles(path);

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
    public static ImportSettings FindImportSettings(string pathToSearch)
    {
        //Find valid import settings in this path
        Object[] importSettings = GetAssetsOfTypeAtPzth<ImportSettings>(pathToSearch);

        //Search recursively up the directory structure until we either find 
        //import settings or reach the main Assets folder with no results
        if (importSettings.Length == 0)
        {            
            if (pathToSearch == "Assets") //We've reached the main Assets folder. Stop searching and warn the user that no settings were found
            {
                Debug.Log("No import settings founds. Use Right Click > Create > ImportSettings to create some");
            }
            else
            {
                //We didn't find anything in this directory. Let's try checking one level up.
                pathToSearch = Directory.GetParent(pathToSearch).ToString();
                return FindImportSettings(pathToSearch);
            }
        }
        else //We found an ImportSettings object to use
        {
            if (importSettings.Length > 1)
            {
                Debug.Log($"More than 1 ImportSettings object was found. Defaulting to first object found: {importSettings[0].name}");
            }
            else
            {
                Debug.Log($"Found settings: {pathToSearch}");
            }

            return (ImportSettings)importSettings[0];
        }

        return null;
    }
}
