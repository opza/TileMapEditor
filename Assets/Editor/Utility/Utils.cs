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
        public static string ConvertAbsoluteToUnityRelativePath(string absoultePath)
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
            path = path.Replace("Assets/Resources", string.Empty);
            if (string.IsNullOrEmpty(path))
                path = "/";

            var sprites = Resources.LoadAll<Sprite>(path);
            return sprites.ToDictionary(sprite => sprite.name);
        }


    }

    
}
