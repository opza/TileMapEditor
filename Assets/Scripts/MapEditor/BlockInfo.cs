using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;
using Worlds;

[Serializable]
[CreateAssetMenu(fileName ="BlockInfo", menuName ="DungeonEditor/BlockInfo")]
public class BlockInfo : ScriptableObject
{
    public enum TileType { Empty, Block, Door }

    [SerializeField]
    new string name;
    public string Name => name;

    [SerializeField]
    Texture2D previewTexture;
    public Texture2D PreviewTexture => previewTexture;

    [SerializeField]
    TileType type;
    public TileType Type => type;

    [SerializeField]
    TileSet tileSet;
    public TileSet TileSet => tileSet;

    [SerializeField]
    GameObject light;
    public GameObject Light => light;
}

