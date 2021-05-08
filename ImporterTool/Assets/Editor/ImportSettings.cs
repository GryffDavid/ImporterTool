using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ImportSettings", menuName = "Import Settings", order = 1)]
public class ImportSettings : ScriptableObject
{
    public FilterMode TetxureFilterMode;
    public AudioSampleRateSetting AudioSampleRate;

    public int MaxTextureSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
