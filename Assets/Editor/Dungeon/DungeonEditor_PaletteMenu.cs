﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.Dungeon
{
    public partial class DungeonEditor
    {
        VisualElement paletteElementPanel;
        Button addPaletteButton;

        void SetPaletteMenu(VisualElement root)
        {
            paletteElementPanel = root.Query<VisualElement>("palette-element-panel").First();
            addPaletteButton = root.Query<Button>("add-palette-button").First();

            addPaletteButton.clickable.clicked += () =>
            {
                var buildPaletteElementEditor = GetWindow<BuildPaletteElementEditor>();
                if (this.buildPaletteElementEditor == null)
                {
                    this.buildPaletteElementEditor = buildPaletteElementEditor;
                    buildPaletteElementEditor.AddElementButton.clickable.clicked += AddPaletteElement;
                }
            };
        }

        void AddPaletteElement()
        {
            var elementName = buildPaletteElementEditor.NameField.value;
            var elementTexture = buildPaletteElementEditor.ObjectField.value as Texture2D;

            if (elementTexture == null)
                return;

            if (string.IsNullOrEmpty(elementName))
                elementName = elementTexture.name;

            palette.Add(elementName, elementTexture);
        }

        void RemovePaletteElement(Palette.Element element)
        {
            palette.Remove(element);
        }

        void UpdatePaletteMenu()
        {
            paletteElementPanel.Clear();
            foreach (var element in palette)
            {
                var elementTemplate = paletteElementTree.CloneTree();
                elementTemplate.styleSheets.Add(paletteElementStyleSheet);

                var elementButton = elementTemplate.Query<Button>("element-button").First();
                elementButton.style.backgroundImage = new StyleBackground(element.Texture2D);
                elementButton.clickable.clicked += () => { selectedPaletteEelment.Value = element; };

                var removeButton = elementTemplate.Query<Button>("remove-button").First();
                removeButton.clickable.clicked += () => RemovePaletteElement(element);

                var elementName = elementTemplate.Query<Label>("element-name").First();
                elementName.text = element.Name;

                paletteElementPanel.Add(elementTemplate);
            }
        }

        void RemoveSelectedItem()
        {
            if (selectedPaletteEelment.IsEmpty)
                return;

            if (!palette.Contains(selectedPaletteEelment.Value))
                selectedPaletteEelment.SetEmpty();
        }
    }
}