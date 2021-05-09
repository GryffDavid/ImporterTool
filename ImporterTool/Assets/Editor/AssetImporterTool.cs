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
        string assetPath = "Assets";
        string settingsPath = "Assets";
        ImportSettings currentSettings = null;

        //Go through each object the user has selected - both folders and files
        foreach (Object selectedObject in Selection.GetFiltered<Object>(SelectionMode.Assets))
        {
            assetPath = AssetDatabase.GetAssetPath(selectedObject);

            //A file has been selected, find the settings and apply them
            if (!string.IsNullOrEmpty(assetPath) && File.Exists(assetPath))
            {
                //Only search for new settings if the directory of the selected files has changed
                if (Path.GetDirectoryName(settingsPath) != Path.GetDirectoryName(assetPath))
                {
                    settingsPath = assetPath;
                    currentSettings = FindImportSettings(settingsPath);
                }
                
                ApplyImportSettingsToAsset(assetPath, currentSettings);
            }
            else //A folder has been selected. Find all the assets in the folder and apply the relevant settings
            {
                //Find all the textures and audio in this folder, including sub folders
                string[] assetGUIDs = AssetDatabase.FindAssets("t:texture t:audioclip", new[] { assetPath });

                //Use a HashSet to get a list of unique folders where assets reside. 
                //Then loop through each asset in each unique folder, applying the relevant settings
                HashSet<string> assetFolders = new HashSet<string>();
                foreach (string guid in assetGUIDs)
                {
                    string folderPath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guid));

                    //HashSet.Add will only return true if the string does not exist in the HashSet already
                    //This asset is a new folder, apply settings to all the assets inside
                    if (assetFolders.Add(folderPath))
                    {
                        Object[] assetsInFolder = GetAssetsOfTypeAtPzth<Object>(folderPath);
                        ImportSettings newSettings = FindImportSettings(folderPath);

                        //If we found valid settings that apply to this folder, apply them to each asset we found in the folder
                        if (newSettings != null)
                        {
                            foreach (Object asset in assetsInFolder)
                            {
                                ApplyImportSettingsToAsset(AssetDatabase.GetAssetPath(asset.GetInstanceID()), newSettings);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Attempt to apply the given settings to the asset found at the path
    /// </summary>
    /// <param name="assetPath">Path to the asset</param>
    /// <param name="importSettings">Settings to apply</param>
    public static void ApplyImportSettingsToAsset(string assetPath, ImportSettings importSettings)
    {
        if (importSettings != null)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);

            #region Apply Texture Import Settings
            if (importer.GetType() == typeof(TextureImporter))
            {
                TextureImporter textureImporter = (TextureImporter)importer;

                //TODO: If this is set to false, the assets need to be stopped from using android overrides
                if (importSettings.OverrideTextureSettingsForAndroid == true)
                {
                    TextureImporterPlatformSettings androidTextureSettings = new TextureImporterPlatformSettings()
                    {
                        name = "Android",
                        maxTextureSize = (int)importSettings.AndroidMaxTextureSize,
                        overridden = true
                    };

                    textureImporter.SetPlatformTextureSettings(androidTextureSettings);
                }

                textureImporter.filterMode = importSettings.TetxureFilterMode;
                textureImporter.maxTextureSize = (int)importSettings.MaxTextureSize;
                textureImporter.anisoLevel = importSettings.FilterLevel;
            }
            #endregion

            #region Apply Audio Import Settings
            if (importer.GetType() == typeof(AudioImporter))
            {
                AudioImporter audioImporter = (AudioImporter)importer;

                AudioImporterSampleSettings audioImportSettings = new AudioImporterSampleSettings()
                {
                    sampleRateSetting = importSettings.AudioSampleRate,
                    compressionFormat = importSettings.CompressionFormat,
                    loadType = importSettings.AudioLoadType                    
                };

                if (importSettings.OverrideAudioSettingsForAndroid == true)
                {
                    AudioImporterSampleSettings androidAudioImportSettings = new AudioImporterSampleSettings()
                    {
                        sampleRateSetting = importSettings.AndroidAudioSampleRate,
                        compressionFormat = importSettings.AndroidCompressionFormat,
                        loadType = importSettings.AndroidAudioClipLoadType
                    };

                    audioImporter.SetOverrideSampleSettings("Android", androidAudioImportSettings);
                }

                audioImporter.defaultSampleSettings = audioImportSettings;
            }
            #endregion

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
        else
        {
            Debug.Log("Not import settings found");
        }        
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
        IEnumerable folderFiles = Directory.EnumerateFiles(path);

        //Loop through each file in the folder. If the file is a matching asset, store it in the AssetList
        foreach (string file in folderFiles)
        {
            if (!file.EndsWith(".meta")) //Don't bother checking .meta files
            {
                Object asset = AssetDatabase.LoadAssetAtPath(file, typeof(T));

                if (asset != null)
                {
                    assetList.Add(asset);
                }
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
