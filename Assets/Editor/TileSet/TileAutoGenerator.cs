using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

using Worlds;
using Editor.Utility;
using Util;
using Util.SerializableObjects;

// TODO : Generator 및 생성 Window를 따로 만들어야함
public class TileAutoGenerator
{
    public static void Genearte(string spriteFolderPath, string jsonFilePath, string saveTileSetPath)
    {
        var tileSet = ScriptableObject.CreateInstance<TileSet>();

        var sprites = Loader.LoadSprites(spriteFolderPath);
        if (sprites.Count <= 0)
        {
            Debug.LogError("Sprite Count is 0");
            return;
        }

        var jsonRules = JsonUtility.FromJson<SerializableDictionary<int, string>>(jsonFilePath);
        var rules = jsonRules.ToDictionary();

        if (rules.Count <= 0)
        {
            Debug.LogError("Json Rules Count is 0");
            return;
        }

        foreach (var spriteName in sprites.Keys)
        {
            var sprite = sprites[spriteName];

            if (!int.TryParse(spriteName.Split('_').Last(), out var spriteNumber))
                return;

            if (!rules.ContainsKey(spriteNumber))
                return;

            var mask = rules[spriteNumber].ToMask();
            tileSet.Add(mask, sprite);
        }

        AssetDatabase.CreateAsset(tileSet, saveTileSetPath);
    }
}

