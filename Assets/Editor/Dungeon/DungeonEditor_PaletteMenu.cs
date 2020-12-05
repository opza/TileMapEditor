using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Editor.Dungeon
{
    public partial class DungeonEditor
    {
        readonly string PALETTE_FILE_EXTENTION = "asset";

        Palette currentPalette;

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
                currentPalette = CreatePalette();             
                UpdatePaletteMenu();
            };

            addPaletteButton.clickable.clicked += () =>
            {
                if (currentPalette == null)
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
                var loadPath = EditorUtility.OpenFilePanel("Palette 불러오기", Application.dataPath, PALETTE_FILE_EXTENTION);

                currentPalette = LoadPalette(loadPath);
                UpdatePaletteMenu();
            };
        }

        Palette CreatePalette()
        {
            var newPalette = CreateInstance<Palette>();

            var savePath = EditorUtility.SaveFilePanel("Palette 생성", Application.dataPath, "NewPalette", PALETTE_FILE_EXTENTION);
            var relativePath = Utility.Path.ConvertUnityRelativePath(savePath);
            if (string.IsNullOrEmpty(relativePath))
                return currentPalette;
            
            AssetDatabase.CreateAsset(newPalette, relativePath);

            InitPaletteEvent(newPalette);

            return newPalette;
        }

        Palette LoadPalette(string path)
        {
            var relativePath = Utility.Path.ConvertUnityRelativePath(path);
            if (string.IsNullOrEmpty(relativePath))
                return currentPalette;

            var loadedPalette = AssetDatabase.LoadAssetAtPath<Palette>(relativePath);
            if(loadedPalette == null)
            {
                Debug.LogError($"옳바른 형식이 아닙니다 {System.IO.Path.GetFileName(relativePath)}");
                return currentPalette;
            }

            InitPaletteEvent(loadedPalette);

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

            currentPalette.Add(elementName, elementTexture, Palette.Element.TileType.Empty);
        }

        void RemovePaletteElement(Palette.Element element)
        {
            currentPalette.Remove(element);
        }

        void UpdatePaletteMenu()
        {
            if (currentPalette == null)
                return;

            paletteElementPanel.Clear();
            foreach (var element in currentPalette)
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

            if (!currentPalette.Contains(selectedPaletteEelment.Value))
                selectedPaletteEelment.SetEmpty();
        }
    }
}
