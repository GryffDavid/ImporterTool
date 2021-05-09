using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ImportSettings))]
public class ImportEditorLayout : Editor
{
    public override void OnInspectorGUI()
    {
        ImportSettings settings = target as ImportSettings;

        #region Audio import settings custom inspector layout
        EditorGUILayout.LabelField("Audio Settings", EditorStyles.boldLabel);
        settings.AudioSampleRate = (AudioSampleRateSetting)EditorGUILayout.EnumPopup("Audio Sample Rate", settings.AudioSampleRate);
        EditorGUI.BeginDisabledGroup(!(settings.AudioSampleRate == AudioSampleRateSetting.OverrideSampleRate));
        settings.SampleRateOverride = (ImportSettings.SampleRateOverrideEnum)EditorGUILayout.EnumPopup("Audio Sample Rate Override", settings.SampleRateOverride);
        EditorGUI.EndDisabledGroup();
        settings.CompressionFormat = (AudioCompressionFormat)EditorGUILayout.EnumPopup("Audio Compression Format", settings.CompressionFormat);
        settings.AudioLoadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("Audio Clip Load Type", settings.AudioLoadType);

        #region Android Audio Settings
        GUILayout.Space(10);
        settings.OverrideAndroidAudioSettings = GUILayout.Toggle(settings.OverrideAndroidAudioSettings, "Override Audio Settings for Android");
        EditorGUI.BeginDisabledGroup(!settings.OverrideAndroidAudioSettings);
        settings.AndroidAudioSampleRate = (AudioSampleRateSetting)EditorGUILayout.EnumPopup("Audio Sample Rate", settings.AndroidAudioSampleRate);
        EditorGUI.BeginDisabledGroup(!(settings.AndroidAudioSampleRate == AudioSampleRateSetting.OverrideSampleRate));
        settings.AndroidSampleRateOverride = (ImportSettings.SampleRateOverrideEnum)EditorGUILayout.EnumPopup("Audio Sample Rate Override", settings.AndroidSampleRateOverride);
        EditorGUI.EndDisabledGroup();
        settings.AndroidCompressionFormat = (AudioCompressionFormat)EditorGUILayout.EnumPopup("Audio Compression Format", settings.AndroidCompressionFormat);
        settings.AndroidAudioClipLoadType = (AudioClipLoadType)EditorGUILayout.EnumPopup("Audio Clip Load Type", settings.AndroidAudioClipLoadType);
        EditorGUI.EndDisabledGroup();
        #endregion 
        #endregion

        GUILayout.Space(10);

        #region Texture import settings custom inspector layout
        EditorGUILayout.LabelField("Texture Settings", EditorStyles.boldLabel);
        settings.MaxTextureSize = (ImportSettings.MaxTextureSizeEnum)EditorGUILayout.EnumPopup("Max Texture Size", settings.MaxTextureSize);
        settings.TetxureFilterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", settings.TetxureFilterMode);

        EditorGUI.BeginDisabledGroup(settings.TetxureFilterMode == FilterMode.Point);
        settings.FilterLevel = EditorGUILayout.IntSlider("Anisotropic Filtering Level", settings.FilterLevel, 0, 16);
        EditorGUI.EndDisabledGroup();

        #region Android Texture Settings
        GUILayout.Space(10);
        settings.OverrideAndroidTextureSettings = GUILayout.Toggle(settings.OverrideAndroidTextureSettings, "Override Texture Settings for Android");
        EditorGUI.BeginDisabledGroup(!settings.OverrideAndroidTextureSettings);
        settings.AndroidMaxTextureSize = (ImportSettings.MaxTextureSizeEnum)EditorGUILayout.EnumPopup("Max Android Texture Size", settings.AndroidMaxTextureSize);
        EditorGUI.EndDisabledGroup();
        #endregion 
        #endregion

        //base.OnInspectorGUI();
    }
}
