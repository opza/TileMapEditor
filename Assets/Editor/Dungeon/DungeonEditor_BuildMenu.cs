﻿using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Data;


namespace Editor.Dungeon
{
    public partial class DungeonEditor
    {
        Room room; 

        Vector2IntField gridSizeField;
        VisualElement gridTilePanel;

        Button buildButton;
        Button loadButton;

        void SetBuildMenu(VisualElement root)
        {
            gridSizeField = root.Query<Vector2IntField>("size-field").First();
            gridTilePanel = root.Query<VisualElement>("tile-panel").First();

            buildButton = root.Query<Button>("build-button").First();
            loadButton = root.Query<Button>("load-tile-button").First();

            buildButton.clickable.clicked += () =>
            {      
                room = CreateRoom(gridSizeField.value.x, gridSizeField.value.y);

                UpdateBuildMenu();
            };

            loadButton.clickable.clicked += () =>
            {
                var loadPath = EditorUtility.OpenFilePanel("Room 불러오기", Application.dataPath, "asset");
                room = LoadRoom(loadPath);

                UpdateBuildMenu();
            };
        }

        Room CreateRoom(int width, int height)
        {
            var room = CreateInstance<Room>();
            room.SetSize(width, height);
            room.updateEvent += UpdateBuildMenu;

            var savePath = EditorUtility.SaveFilePanel("Room 생성", Application.dataPath, "NewRoom", "asset");
            var relativePath = Path.ConvertUnityRelativePath(savePath);
            if (string.IsNullOrEmpty(relativePath))
                return null;
            else
                AssetDatabase.CreateAsset(room, relativePath);

            return room;
        }

        Room LoadRoom(string path)
        {
            var relativePath = Path.ConvertUnityRelativePath(path);
            if (string.IsNullOrEmpty(relativePath))
                return null;

            return AssetDatabase.LoadAssetAtPath<Room>(relativePath);

        }

        void UpdateBuildMenu()
        {
            gridTilePanel.Clear();
            if (room == null)
                return;

            var gridElements = gridTilePanel.CreateGridButton(room.Width, room.Height, buildTileTree, buildTileStyleSheet, OnClickedEvnet);
            DrawGridButton(gridElements);
        }

        void DrawGridButton(VisualElement[,] elements)
        {
            var elementWidth = elements.GetLength(0);
            var elementHeight = elements.GetLength(1);

            if (elementWidth != room.Width || elementHeight != room.Height)
            {
                Debug.LogError("GridButton의 크기가 Room의 크기와 맞지 않습니다");
                return;
            }

            for (int x = 0; x < room.Width; x++)
            {
                for (int y = 0; y < room.Height; y++)
                {
                    var button = elements[x, y] as Button;
                    button.style.backgroundImage = room[x, y].Texture2D;
                }
            }
        }

        void OnClickedEvnet(Button button, int x, int y)
        {
            if (selectedPaletteEelment.IsEmpty)
                return;
     
            room.SetTile(selectedPaletteEelment.Value, x, y);
        }
    }
}
