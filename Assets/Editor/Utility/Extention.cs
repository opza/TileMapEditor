using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.Utility
{
    

    public static class Extention
    {

        public static VisualElement[,] CreateGridButton(this VisualElement parent, int width, int height, Action<Button> onClicked = null)
        {
            return parent.CreateGrid(width, height, (x, y) =>
            {
                var button = new Button();
                button.clickable.clicked += () => onClicked(button);

                return button;
            });
        }

        public static VisualElement[,] CreateGridButton(this VisualElement parent, int width, int heigth, VisualTreeAsset buttonTemplate, StyleSheet styleSheet = null, Action<Button, int, int> onClicked = null)
        {
            return parent.CreateGrid(width, heigth, (x, y) =>
            {
                var template = buttonTemplate.CloneTree();
                var button = template.Query<Button>().First();
                button.clickable.clicked += () => onClicked(button, x, y);

                return button;
            }, styleSheet);
        }

        public static VisualElement[,] CreateGrid(this VisualElement parent, int width, int height, Func<int, int, VisualElement> elementCreator, StyleSheet styleSheet = null)
        {
            var gridElements = new VisualElement[width, height];

            if (width <= 0 || height <= 0)
                return gridElements;

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
                    var element = elementCreator(x, y);
                    gridElements[x, y] = element;
                    widthPanel.Add(element);
                }

                parent.Add(widthPanel);
            }

            return gridElements;
        }
    }
}
