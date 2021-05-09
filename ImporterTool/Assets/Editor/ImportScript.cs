using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportScript : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        ApplySettingsToNewFile(assetImporter);
    }

    void OnPreprocessAudio()
    {
        ApplySettingsToNewFile(assetImporter);
    }

    //Should be very easy now to make the Importer handle new types of assets such as models

    void ApplySettingsToNewFile(AssetImporter importer)
    {
        bool isNewFile = assetImporter.importSettingsMissing; //This returns true if there is no meta file with the asset.

        if (isNewFile)
        {
            ImportSettings importSettings = AssetImporterTool.FindImportSettings(importer.assetPath);
            AssetImporterTool.ApplyImportSettingsToAsset(importer.assetPath, importSettings);
        }
    }
}