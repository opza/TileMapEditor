using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Data;

using Editor.Utility;

using Worlds.Generate;
using Editor.Dungeon.Draw;

namespace Editor.Dungeon
{
    public partial class DungeonEditor
    {
        readonly string ROOM_FILE_EXTENTION = "asset";
        readonly int MIN_WIDTH = 1;
        readonly int MIN_HEGIHT = 1;

        readonly string DOT_DRAW_MODE_TEXT = "연필 모드";
        readonly string LINE_DRAW_MODE_TEXT = "선 모드";
        readonly string REMOVE_MODE_TEXT = "지우기 모드";

        [SerializeField]
        Room currentRoom;

        IDrawer drawer;

        Vector2IntField gridSizeField;
        VisualElement gridTilePanel;

        Button buildButton;
        Button loadButton;
        Button resizeButton;
        Button removeButton;
        Button setDoorButton;

        Button setDotDrawButton;
        Button setLineDrawButton;
        Label drawModeLabel;

        VisualElement[,] gridElements;

        #region Init

        void InitBuildMenu(VisualElement root)
        {
            gridSizeField = root.Query<Vector2IntField>("size-field").First();
            gridTilePanel = root.Query<VisualElement>("tile-panel").First();

            buildButton = root.Query<Button>("build-tiles-button").First();
            loadButton = root.Query<Button>("load-tiles-button").First();
            resizeButton = root.Query<Button>("resize-tile-button").First();         
            setDoorButton = root.Query<Button>("set-door-button").First();

            buildButton.clickable.clicked += () =>
            {      
                currentRoom = CreateRoom(gridSizeField.value.x, gridSizeField.value.y);

                CreateGridButton();
            };

            loadButton.clickable.clicked += () =>
            {
                var loadPath = EditorUtility.OpenFilePanel("Room 불러오기", Application.dataPath, ROOM_FILE_EXTENTION);
                currentRoom = LoadRoom(loadPath);

                CreateGridButton();
            };

            resizeButton.clickable.clicked += () =>
            {
                var width = gridSizeField.value.x;
                var height = gridSizeField.value.y;

                if (width < MIN_WIDTH || height < MIN_HEGIHT)
                    return;

                currentRoom?.SetSize(width, height);
            };       

            setDoorButton.clickable.clicked += () =>
            {
                selectedPaletteEelment.Clear();
                drawer.DrawAction = SwitchDoorInTile;
            };
        }

        void InitDrawMenu(VisualElement root)
        {
            drawer = new DotDrawer();

            setDotDrawButton = root.Query<Button>("set-dot-button").First();
            setLineDrawButton = root.Query<Button>("set-line-button").First();
            removeButton = root.Query<Button>("remove-tile-button").First();
            drawModeLabel = root.Query<Label>("draw-mode-label").First();

            setDotDrawButton.clickable.clicked += () =>
            {
                drawer = new DotDrawer(SetTile);
                drawModeLabel.text = DOT_DRAW_MODE_TEXT;
            };

            setLineDrawButton.clickable.clicked += () =>
            {
                drawer = new LineDrawer(SetTile);
                drawModeLabel.text = LINE_DRAW_MODE_TEXT;
            };

            removeButton.clickable.clicked += () =>
            {
                drawModeLabel.text = REMOVE_MODE_TEXT;

                selectedPaletteEelment.Clear();
                drawer.DrawAction = SetTile;
            };
        }

        #endregion

        #region Room

        Room CreateRoom(int width, int height)
        {
            if (width < MIN_WIDTH || height < MIN_HEGIHT)
                return currentRoom;       

            var savePath = EditorUtility.SaveFilePanel("Room 생성", Application.dataPath, "NewRoom", ROOM_FILE_EXTENTION);
            var relativePath = Path.ConvertAbsoluteToUnityRelativePath(savePath);
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
            var relativePath = Path.ConvertAbsoluteToUnityRelativePath(path);
            if (string.IsNullOrEmpty(relativePath))
                return currentRoom;

            var loadedRoom = AssetDatabase.LoadAssetAtPath<Room>(relativePath);
            if (loadedRoom == null)
            {
                Debug.LogError($"옳바른 형식이 아닙니다 {System.IO.Path.GetFileName(relativePath)}");
                return currentRoom;
            }

            InitRoom(loadedRoom);
            gridSizeField.value = new Vector2Int(loadedRoom.Width, loadedRoom.Height);

            return loadedRoom;
        }


        void InitRoom(Room room)
        {
            room.resizeEvent += CreateGridButton;
            room.updateTileEvent += UpdateTile;
        }

        #endregion

        #region Create Grid

        void CreateGridButton()
        {
            gridTilePanel.Clear();
            if (currentRoom == null)
                return;

            gridElements = gridTilePanel.CreateGridButton(currentRoom.Width, currentRoom.Height, buildTileTree, OnClickTileButton);
            DrawGridButton(gridElements);
            
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
                    UpdateTile(x, y, elements);
                }
            }
        }

        #endregion

        #region UpdateTile

        void UpdateTile(int x, int y)
        {
            UpdateTile(x, y, gridElements);
        }

        void UpdateTile(int x, int y, VisualElement[,] anotherGridElements)
        {
            if (anotherGridElements == null)
                return;

            var button = anotherGridElements[x, y].Query<Button>().First();
            button.style.backgroundImage = currentRoom[x, y].BlockInfo?.PreviewTexture;

            if (currentRoom[x, y]?.IsDoor == true)
                button.style.SetBordersColor(new StyleColor(Color.yellow));
            else
                button.style.SetBordersColor(new StyleColor());

            EditorUtility.SetDirty(currentRoom);
        }

        #endregion

        void OnClickTileButton(int x, int y)
        {
            drawer?.Draw(x, y);
        }

        void SetTile(int x, int y)
        {
            currentRoom.SetTile(selectedPaletteEelment.Value, x, y);
        }

        void SwitchDoorInTile(int x, int y)
        {
            currentRoom.SwitchDoor(x, y);
        }
    }
}
