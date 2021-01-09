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

        public static VisualElement[,] CreateGridButton(this VisualElement parent, int width, int heigth, VisualTreeAsset buttonTemplate, Action<int, int> onClicked = null)
        {
            return parent.CreateGrid(width, heigth, (x, y) =>
            {
                //var template = buttonTemplate.CloneTree();
                var t = new VisualElement();
                var template = buttonTemplate.CloneTree();
                var button = template.Query<Button>().First();
                button.clickable.clicked += () => onClicked?.Invoke(x, y);

                return template;
            });
        }

        public static VisualElement[,] CreateGrid(this VisualElement parent, int width, int height, Func<int, int, VisualElement> elementCreator)
        {
            var gridElements = new VisualElement[width, height];

            if (width <= 0 || height <= 0)
                return gridElements;

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

        public static void SetBordersColor(this IStyle style, StyleColor styleColor)
        {
            style.borderBottomColor = styleColor;
            style.borderLeftColor = styleColor;
            style.borderRightColor = styleColor;
            style.borderTopColor = styleColor;
        }
    }
}
