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
        readonly string MAIN_USS_PATH = "Assets/Editor/Dungeon/DungeonEditor.uss";

        readonly string PALETTE_ELEMENT_UXML_PATH = "Assets/Editor/Dungeon/PaletteElementTemplate/PaletteElementTemplate.uxml";
        readonly string PALETTE_ELEMENT_USS_PATH = "Assets/Editor/Dungeon/PaletteElementTemplate/PaletteElementTemplate.uss";

        readonly string BUILD_TILE_UXML_PATH = "Assets/Editor/Dungeon/BuildTileTemplate/BuildTileTemplate.uxml";
        readonly string BUILD_TILE_USS_PATH = "Assets/Editor/Dungeon/BuildTileTemplate/BuildTileTemplate.uss";

        VisualTreeAsset paletteElementTree;
        StyleSheet paletteElementStyleSheet;

        VisualTreeAsset buildTileTree;
        StyleSheet buildTileStyleSheet;
   
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
            paletteElementStyleSheet = Loader.LoadUss(PALETTE_ELEMENT_USS_PATH);

            buildTileTree = Loader.LoadUxml(BUILD_TILE_UXML_PATH);
            buildTileStyleSheet = Loader.LoadUss(BUILD_TILE_USS_PATH);

            selectedPaletteEelment = new SelectedItem<BlockInfo>();           

            SetBuildMenu(root);
            SetPaletteMenu(root);
        }

        void SetMainTree(VisualElement root)
        {
            var mainUxml = Loader.LoadUxml(MAIN_UXML_PATH);
            root.Add(mainUxml.CloneTree());

            var mainStyleSheet = Loader.LoadUss(MAIN_USS_PATH);
            root.styleSheets.Add(mainStyleSheet);      
        }

        class SelectedItem<T> where T : class
        {
            
            public T Value { get; set; }


            public bool IsEmpty => Value == null;
            public void SetEmpty() => Value = null;
            
        }

    }
}