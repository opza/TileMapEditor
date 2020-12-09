using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    BlockInfo blockInfo;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer.sprite = blockInfo.Sprite;
    }

}
