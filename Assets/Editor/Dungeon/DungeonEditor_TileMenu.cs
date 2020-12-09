using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Data;

using Editor.Utility;

namespace Editor.Dungeon
{
    public partial class DungeonEditor
    {
        readonly string ROOM_FILE_EXTENTION = "asset";
        readonly int MIN_WIDTH = 1;
        readonly int MIN_HEGIHT = 1;

        Room currentRoom; 

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
                currentRoom = CreateRoom(gridSizeField.value.x, gridSizeField.value.y);

                UpdateBuildMenu();
            };

            loadButton.clickable.clicked += () =>
            {
                var loadPath = EditorUtility.OpenFilePanel("Room 불러오기", Application.dataPath, ROOM_FILE_EXTENTION);
                currentRoom = LoadRoom(loadPath);

                UpdateBuildMenu();
            };

            resizeButton.clickable.clicked += () =>
            {
                var width = gridSizeField.value.x;
                var height = gridSizeField.value.y;

                if (width < MIN_WIDTH || height < MIN_HEGIHT)
                    return;

                currentRoom?.SetSize(width, height);

                UpdateBuildMenu();
            };
        }

        Room CreateRoom(int width, int height)
        {
            if (width < MIN_WIDTH || height < MIN_HEGIHT)
                return currentRoom;       

            var savePath = EditorUtility.SaveFilePanel("Room 생성", Application.dataPath, "NewRoom", ROOM_FILE_EXTENTION);
            var relativePath = Path.ConvertUnityRelativePath(savePath);
            if (string.IsNullOrEmpty(relativePath))
                return currentRoom;

            var newRoom = CreateInstance<Room>();
            newRoom.SetSize(width, height);
            InitRoom(newRoom);

            AssetDatabase.CreateAsset(newRoom, relativePath);

            return newRoom;
        }

        Room LoadRoom(string path)
        {
            var relativePath = Path.ConvertUnityRelativePath(path);
            if (string.IsNullOrEmpty(relativePath))
                return currentRoom;

            var loadedRoom = AssetDatabase.LoadAssetAtPath<Room>(relativePath);
            if (loadedRoom == null)
            {
                Debug.LogError($"옳바른 형식이 아닙니다 {System.IO.Path.GetFileName(relativePath)}");
                return currentRoom;
            }

            InitRoom(loadedRoom);

            return loadedRoom;
        }

        void InitRoom(Room room)
        {
            room.updateEvent += UpdateBuildMenu;
        }

        void UpdateBuildMenu()
        {
            gridTilePanel.Clear();
            if (currentRoom == null)
                return;

            var gridElements = gridTilePanel.CreateGridButton(currentRoom.Width, currentRoom.Height, buildTileTree, buildTileStyleSheet, OnClickedEvnet);
            DrawGridButton(gridElements);

            if (GUI.changed)
                EditorUtility.SetDirty(currentRoom);
        }

        void DrawGridButton(VisualElement[,] elements)
        {
            var elementWidth = elements.GetLength(0);
            var elementHeight = elements.GetLength(1);

            if (elementWidth != currentRoom.Width || elementHeight != currentRoom.Height)
            {
                Debug.LogError("GridButton의 크기가 Room의 크기와 맞지 않습니다");
                return;
            }

            for (int x = 0; x < currentRoom.Width; x++)
            {
                for (int y = 0; y < currentRoom.Height; y++)
                {
                    var button = elements[x, y] as Button;
                    button.style.backgroundImage = currentRoom[x, y].BlockInfo?.PreviewTexture;
                }
            }
        }

        void OnClickedEvnet(Button button, int x, int y)
        {
            if (selectedPaletteEelment.IsEmpty)
                return;
     
            currentRoom.SetTile(selectedPaletteEelment.Value, x, y);
        }
    }
}
