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

        ImportSettings importSettings = FindImportSettings(path);

        AssetImporter importer = AssetImporter.GetAtPath(path);

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

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    static ImportSettings FindImportSettings(string pathToSearch)
    {
        //Attempt to get the ImportSettings scriptable object at this path.
        //I'm not happy with using a string here and not allowing the user the freedom to rename the ImportSettings asset. 
        //I attempted to use AssetDatabase.FindAssets, but it does a BFS on the path it's assigned so it doesn't work with my upwards recursive approach        
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
