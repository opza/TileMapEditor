using System;
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
        readonly string ROOM_FILE_EXTENTION = "asset";

        Room room; 

        Vector2IntField gridSizeField;
        VisualElement gridTilePanel;

        Button buildButton;
        Button loadButton;
        Button resizeButton;

        void SetBuildMenu(VisualElement root)
        {
            gridSizeField = root.Query<Vector2IntField>("size-field").First();
            gridTilePanel = root.Query<VisualElement>("tile-panel").First();

            buildButton = root.Query<Button>("build-tiles-button").First();
            loadButton = root.Query<Button>("load-tile-button").First();
            resizeButton = root.Query<Button>("resize-tile-button").First();

            buildButton.clickable.clicked += () =>
            {      
                room = CreateRoom(gridSizeField.value.x, gridSizeField.value.y);

                UpdateBuildMenu();
            };

            loadButton.clickable.clicked += () =>
            {
                var loadPath = EditorUtility.OpenFilePanel("Room 불러오기", Application.dataPath, ROOM_FILE_EXTENTION);
                room = LoadRoom(loadPath);

                UpdateBuildMenu();
            };

            resizeButton.clickable.clicked += () =>
            {
                room?.SetSize(gridSizeField.value.x, gridSizeField.value.y);

                UpdateBuildMenu();
            };
        }

        Room CreateRoom(int width, int height)
        {
            var room = CreateInstance<Room>();
            room.SetSize(width, height);
            room.updateEvent += UpdateBuildMenu;

            var savePath = EditorUtility.SaveFilePanel("Room 생성", Application.dataPath, "NewRoom", ROOM_FILE_EXTENTION);
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

            var loadedRoom = AssetDatabase.LoadAssetAtPath<Room>(relativePath);
            loadedRoom.updateEvent += UpdateBuildMenu;

            return loadedRoom;
        }

        void UpdateBuildMenu()
        {
            gridTilePanel.Clear();
            if (room == null)
                return;

            var gridElements = gridTilePanel.CreateGridButton(room.Width, room.Height, buildTileTree, buildTileStyleSheet, OnClickedEvnet);
            DrawGridButton(gridElements);

            if (GUI.changed)
                EditorUtility.SetDirty(room);
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
