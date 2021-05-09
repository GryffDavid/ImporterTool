using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ImportSettings", menuName = "Import Settings", order = 1)]
public class ImportSettings : ScriptableObject
{
    //Made this into an enum so that the user gets a drop down menu instead of being trusted to type in a texture size that is a power of 2.
    public enum MaxTextureSizeEnum
    {
        _32 = 32,
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
        _8192 = 8192
    };

    [Header("Audio Settings")]
    public AudioSampleRateSetting AudioSampleRate;
    public AudioCompressionFormat CompressionFormat;
    public AudioClipLoadType AudioLoadType;    

    [Header("Texture Settings")]
    public FilterMode TetxureFilterMode;

    [Range(0, 16)]
    public int FilterLevel = 1;

    public MaxTextureSizeEnum MaxTextureSize = MaxTextureSizeEnum._2048;

    [Header("Android Audio Overrides")]
    public bool OverrideAudioSettingsForAndroid = false;
    public AudioSampleRateSetting AndroidAudioSampleRate;
    public AudioCompressionFormat AndroidCompressionFormat;
    public AudioClipLoadType AndroidAudioClipLoadType;

    [Header("Android Texture Overrides")]
    public bool OverrideTextureSettingsForAndroid = false;
    public MaxTextureSizeEnum AndroidMaxTextureSize = MaxTextureSizeEnum._2048;
}
