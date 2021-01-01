using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

namespace Worlds
{
    public class ChangeBlockSpriteArgs : EventArgs
    {
  
        public Vector3Int Position { get; }
        public Sprite Sprite { get; }

        public ChangeBlockSpriteArgs(Vector3Int position, Sprite sprite)
        {
            Position = position;
            Sprite = sprite;
        }
    }
}
