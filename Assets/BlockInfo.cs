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
    Texture2D previewTexture;
    public Texture2D PreviewTexture => previewTexture;

    Sprite sprite;
    public Sprite Sprite
    {
        get
        {
            if (sprite == null)
                sprite = Sprite.Create(previewTexture, new Rect(0, 0, previewTexture.width, previewTexture.height), new Vector2(.5f, .5f));

            return sprite;
        }
    }

    [SerializeField]
    TileType type;
    public TileType Type => type;
}

