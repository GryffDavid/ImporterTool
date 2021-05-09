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

    [Range(0, 16)]
    public int FilterLevel = 1;

    public FilterMode TetxureFilterMode;
    public AudioSampleRateSetting AudioSampleRate;
    public AudioClipLoadType AudioLoadType;
    
    public MaxTextureSizeEnum MaxTextureSize = MaxTextureSizeEnum._2048;
}
