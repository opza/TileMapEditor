using System;
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
        Palette palette;

        VisualElement paletteElementPanel;

        Button buildPaletteButton;
        Button addPaletteButton;
        Button loadPaletteButton;

        void SetPaletteMenu(VisualElement root)
        {
            paletteElementPanel = root.Query<VisualElement>("palette-element-panel").First();

            buildPaletteButton = root.Query<Button>("build-palette-button").First();
            addPaletteButton = root.Query<Button>("add-palette-button").First();
            loadPaletteButton = root.Query<Button>("load-palette-button").First();

            buildPaletteButton.clickable.clicked += () =>
            {
                palette = CreatePalette();
                InitPaletteEvent(palette);
            };

            addPaletteButton.clickable.clicked += () =>
            {
                if (palette == null)
                    return;

                var buildPaletteElementEditor = GetWindow<BuildPaletteElementEditor>();
                if (this.buildPaletteElementEditor == null)
                {
                    this.buildPaletteElementEditor = buildPaletteElementEditor;
                    buildPaletteElementEditor.AddElementButton.clickable.clicked += AddPaletteElement;
                }
            };

            loadPaletteButton.clickable.clicked += () =>
            {
                var loadPath = EditorUtility.OpenFilePanel("Palette 불러오기", Application.dataPath, "asset");

                palette = LoadPalette(loadPath);
                InitPaletteEvent(palette);

                UpdatePaletteMenu();
            };
        }

        Palette CreatePalette()
        {
            var palette = CreateInstance<Palette>();

            var savePath = EditorUtility.SaveFilePanel("Palette 생성", Application.dataPath, "NewPalette", "asset");
            var relativePath = Path.ConvertUnityRelativePath(savePath);
            if (string.IsNullOrEmpty(relativePath))
                return null;
            else
                AssetDatabase.CreateAsset(palette, relativePath);

            return palette;
        }

        Palette LoadPalette(string path)
        {
            var relativePath = Path.ConvertUnityRelativePath(path);
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var loadedPalette = AssetDatabase.LoadAssetAtPath<Palette>(relativePath);

            return loadedPalette;
        }

        void InitPaletteEvent(Palette palette)
        {
            if (palette == null)
                return;

            palette.addElementEvent += UpdatePaletteMenu;

            palette.removeElementEvent += RemoveSelectedItem;
            palette.removeElementEvent += UpdatePaletteMenu;
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
