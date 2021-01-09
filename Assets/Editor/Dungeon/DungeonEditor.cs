using System;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Editor.Utility;


namespace Editor.Dungeon
{
    public partial class DungeonEditor : EditorWindow
    {
        readonly string MAIN_UXML_PATH = "Assets/Editor/Dungeon/DungeonEditor.uxml";
        readonly string PALETTE_ELEMENT_UXML_PATH = "Assets/Editor/Dungeon/PaletteElementTemplate/PaletteElementTemplate.uxml";
        readonly string BUILD_TILE_UXML_PATH = "Assets/Editor/Dungeon/BuildTileTemplate/BuildTileTemplate.uxml";

        VisualTreeAsset paletteElementTree;
        VisualTreeAsset buildTileTree;   
   
        SelectedItem<BlockInfo> selectedPaletteEelment;

        [MenuItem("Window/DungeonEditor")]
        public static void ShowWindow()
        {
            DungeonEditor wnd = GetWindow<DungeonEditor>();
            wnd.titleContent = new GUIContent("DungeonEditor");
        }

        public void OnEnable()
        {
            VisualElement root = rootVisualElement;
            SetMainTree(root);

            paletteElementTree = Loader.LoadUxml(PALETTE_ELEMENT_UXML_PATH);
            buildTileTree = Loader.LoadUxml(BUILD_TILE_UXML_PATH);

            selectedPaletteEelment = new SelectedItem<BlockInfo>();           

            InitBuildMenu(root);
            InitDrawMenu(root);

            InitPaletteMenu(root);
        }

        void SetMainTree(VisualElement root)
        {
            var mainUxml = Loader.LoadUxml(MAIN_UXML_PATH);
            root.Add(mainUxml.CloneTree());   
        }

        class SelectedItem<T> where T : class
        {
            
            public T Value { get; protected set; }


            public bool IsEmpty => Value == null;
            public void Clear() => Value = null;

            public void SetValue(T value) => Value = value;
            
        }

    }
}