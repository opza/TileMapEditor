using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor
{
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
    }

    public static class Extention
    {
        public static void CreateGirdButton(this VisualElement parent, int width, int height, Action<Button> onClicked = null)
        {
            parent.CreateGrid(width, height, () =>
            {
                var button = new Button();
                button.clickable.clicked += () => onClicked(button);

                return button;
            });
        }

        public static void CreateGridButton(this VisualElement parent, int width, int heigth, VisualTreeAsset buttonTemplate, StyleSheet styleSheet = null, Action<Button> onClicked = null)
        {
            parent.CreateGrid(width, heigth, () =>
            {
                var template = buttonTemplate.CloneTree();
                var button = template.Query<Button>().First();
                button.clickable.clicked += () => onClicked(button);

                return button;
            }, styleSheet);
        }

        public static void CreateGrid(this VisualElement parent, int width, int height, Func<VisualElement> elementCreator, StyleSheet styleSheet = null)
        {
            if (width <= 0 || height <= 0)
                return;

            if (styleSheet != null)
                parent.styleSheets.Add(styleSheet);

            var rowFlex = new StyleEnum<FlexDirection>();
            rowFlex.value = FlexDirection.Row;

            for (int y = 0; y < height; y++)
            {
                var widthPanel = new VisualElement();

                widthPanel.style.flexDirection = rowFlex;

                for (int x = 0; x < width; x++)
                {
                    widthPanel.Add(elementCreator());
                }

                parent.Add(widthPanel);
            }
        }
    }
}
