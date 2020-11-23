using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.Dungeon
{
    public partial class DungeonEditor
    {
        Vector2IntField gridSizeField;
        VisualElement gridTilePanel;
        Button buildButton;

        void SetBuildMenu(VisualElement root)
        {
            gridSizeField = root.Query<Vector2IntField>("size-field").First();
            gridTilePanel = root.Query<VisualElement>("tile-panel").First();
            buildButton = root.Query<Button>("build-button").First();

            buildButton.clickable.clicked += () =>
            {
                gridTilePanel.Clear();
                //gridTilePanel.CreateGirdButton(gridSizeField.value.x, gridSizeField.value.y, button =>
                //{
                //    if (selectedPaletteEelment.IsEmpty)
                //        return;

                //    button.style.backgroundImage = new StyleBackground(selectedPaletteEelment.Value.Texture2D);
                //});

                gridTilePanel.CreateGirdButton(gridSizeField.value.x, gridSizeField.value.y, buildTileTree, buildTileStyleSheet, button =>
                {
                    if (selectedPaletteEelment.IsEmpty)
                        return;

                    button.style.backgroundImage = new StyleBackground(selectedPaletteEelment.Value.Texture2D);
                });
            };
        }
    }
}
