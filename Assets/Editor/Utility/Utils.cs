using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace Editor.Utility
{

    public static class Util
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T buf = b;
            b = a;
            a = buf;
        }
    }

    public static class Path
    {
        public static string ConvertUnityRelativePath(string absoultePath)
        {
            var matchedPath = Regex.Match(absoultePath, @"Assets/.*");
            if (!matchedPath.Success)
                return string.Empty;

            return matchedPath.Value;
        }
    }

    public static class Loader
    {
        public static VisualTreeAsset LoadUxml(string path)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            return visualTree;
        }

        public static StyleSheet LoadUss(string path)
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            return styleSheet;
        }

        public static Texture2D LoadTexture2D(string path)
        {
            var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            return texture2D;
        }

        public static Dictionary<string, Sprite> LoadSprites(string path)
        {
            var sprites = new Dictionary<string, Sprite>();

            var objects = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var obj in objects)
            {
                if (!(obj is Sprite))
                    continue;

                sprites[obj.name] = obj as Sprite;
            }

            return sprites;
        }


    }

    
}
