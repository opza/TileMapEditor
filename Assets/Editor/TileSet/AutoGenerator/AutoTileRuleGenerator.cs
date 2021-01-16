using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using UnityEngine;
using UnityEditor;

using Worlds;
using Editor.Utility;
using Util;
using Util.SerializableObjects;

// TODO : Generator 및 생성 Window를 따로 만들어야함
public class AutoTileRuleGenerator
{
    public static TileSet Genearte(string spriteFolderPath, string jsonFilePath)
    {
        var tileSet = ScriptableObject.CreateInstance<TileSet>();

        var sprites = Loader.LoadSprites(spriteFolderPath);
        if (sprites.Count <= 0)
        {
            Debug.LogError("Sprite Count is 0");
            return null;
        }

        var json = File.ReadAllText(jsonFilePath);
        var jsonRules = JsonUtility.FromJson<SerializableDictionary<int, string>>(json);
        var rules = jsonRules.ToDictionary();

        if (rules.Count <= 0)
        {
            Debug.LogError("Json Rules Count is 0");
            return null;
        }

        foreach (var spriteName in sprites.Keys)
        {
            var sprite = sprites[spriteName];

            if (!int.TryParse(spriteName.Split('_').Last(), out var spriteNumber))
                continue;

            if (!rules.ContainsKey(spriteNumber))
                continue;

            var mask = rules[spriteNumber].ToMask();
            tileSet.Add(mask, sprite);
        }

        return tileSet;
    }

    public static void SaveJsonFormat(string savePath)
    {
        var initRule = new Dictionary<int, string>()
        {
            {0, "0000 0000" },
            {1, "1111 1111" }
        };

        var serializableDic = new SerializableDictionary<int, string>(initRule);
        var json = JsonUtility.ToJson(serializableDic);

        File.WriteAllText(savePath, json);
    }
}

