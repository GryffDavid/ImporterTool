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

    public enum SampleRateOverrideEnum
    {
        _8000Hz = 8000,
        _11025Hz = 11025,
        _22050Hz = 22050,
        _44100Hz = 44100,
        _48000Hz = 48000,
        _96000Hz = 96000,
        _192000Hz = 192000
    };

    [Header("Audio Settings")]
    public bool UseAudioSettings = true;
    public AudioSampleRateSetting AudioSampleRate;
    public SampleRateOverrideEnum SampleRateOverride;
    public AudioCompressionFormat CompressionFormat;
    public AudioClipLoadType AudioLoadType;

    [Header("Texture Settings")]
    public bool UseTextureSettings = true;
    public FilterMode TetxureFilterMode;

    [Range(0, 16)]
    public int FilterLevel = 1;

    public MaxTextureSizeEnum MaxTextureSize = MaxTextureSizeEnum._2048;

    [Header("Android Audio Overrides")]
    public bool OverrideAndroidAudioSettings = false;
    public AudioSampleRateSetting AndroidAudioSampleRate;
    public SampleRateOverrideEnum AndroidSampleRateOverride;
    public AudioCompressionFormat AndroidCompressionFormat;
    public AudioClipLoadType AndroidAudioClipLoadType;

    [Header("Android Texture Overrides")]
    public bool OverrideAndroidTextureSettings = false;
    public MaxTextureSizeEnum AndroidMaxTextureSize = MaxTextureSizeEnum._2048;
}
