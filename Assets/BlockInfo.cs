using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[Serializable]
[CreateAssetMenu(fileName ="BlockInfo", menuName ="DungeonEditor/BlockInfo")]
public class BlockInfo : ScriptableObject
{
    public enum TileType { Empty, Block, Door }

    [SerializeField]
    new string name;
    public string Name => name;

    [SerializeField]
    Texture2D texture2D;
    public Texture2D Texture2D => texture2D;

    [SerializeField]
    TileType type;
    public TileType Type => type;

    [SerializeField]
    Light2D light2D;

    void Test()
    {
        
    }
}

