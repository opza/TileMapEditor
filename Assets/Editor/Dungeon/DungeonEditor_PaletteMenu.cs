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
        readonly string BLOCK_INFO_FILE_EXTENTION = "asset";

        Palette currentPalette;

        VisualElement paletteElementPanel;

        Button buildPaletteButton;
        Button loadBlockInfoPaletteButton;
        Button loadPaletteButton;

        void SetPaletteMenu(VisualElement root)
        {
            paletteElementPanel = root.Query<VisualElement>("palette-element-panel").First();

            buildPaletteButton = root.Query<Button>("build-palette-button").First();
            loadBlockInfoPaletteButton = root.Query<Button>("add-palette-button").First();
            loadPaletteButton = root.Query<Button>("load-palette-button").First();

            buildPaletteButton.clickable.clicked += () =>
            {
                currentPalette = CreatePalette();             
                DrawPaletteElement();
            };

            loadBlockInfoPaletteButton.clickable.clicked += () =>
            {
                if (currentPalette == null)
                    return;

                var loadPath = EditorUtility.OpenFilePanel("BlockInfo 불러오기", Application.dataPath, BLOCK_INFO_FILE_EXTENTION);
                var loadedBlockInfo = LoadBlockInfo(loadPath);
                if (loadedBlockInfo == null)
                    return;

                currentPalette.Add(loadedBlockInfo);
                EditorUtility.SetDirty(currentPalette);
            };

            loadPaletteButton.clickable.clicked += () =>
            {
                var loadPath = EditorUtility.OpenFilePanel("Palette 불러오기", Application.dataPath, PALETTE_FILE_EXTENTION);

                currentPalette = LoadPalette(loadPath);
                DrawPaletteElement();
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

        BlockInfo LoadBlockInfo(string path)
        {
            var relativePath = Utility.Path.ConvertUnityRelativePath(path);
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var loadedBlockInfo = AssetDatabase.LoadAssetAtPath<BlockInfo>(relativePath);
            if(loadedBlockInfo == null)
            {
                Debug.LogError($"옳바른 형식이 아닙니다 {System.IO.Path.GetFileName(relativePath)}");
                return null;
            }

            return loadedBlockInfo;

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

            palette.updateElementEvent += DrawPaletteElement;
        }

        void DrawPaletteElement()
        {
            if (currentPalette == null)
                return;

            paletteElementPanel.Clear();
            foreach (var blockInfo in currentPalette)
            {
                if (blockInfo == null)
                    continue;

                var elementTemplate = paletteElementTree.CloneTree();

                var elementButton = elementTemplate.Query<Button>("element-button").First();
                elementButton.style.backgroundImage = new StyleBackground(blockInfo.PreviewTexture);
                elementButton.clickable.clicked += () => 
                {
                    selectedPaletteEelment.SetValue(blockInfo);
                    currentTileEvent = SetTile;
                };

                var removeButton = elementTemplate.Query<Button>("remove-button").First();
                removeButton.clickable.clicked += () => RemovePaletteElement(blockInfo);

                var elementName = elementTemplate.Query<Label>("element-name").First();
                elementName.text = blockInfo.Name;

                paletteElementPanel.Add(elementTemplate);
            }
        }

        void RemovePaletteElement(BlockInfo blockInfo)
        {
            currentPalette.Remove(blockInfo);

            if (!currentPalette.Contains(selectedPaletteEelment.Value))
                selectedPaletteEelment.Clear();
        }

    }
}
