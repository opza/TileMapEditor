using System;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

namespace Editor.Dungeon
{
    public class DungeonEditor : EditorWindow
    {
        readonly string MAIN_UXML_PATH = "Assets/Editor/Dungeon/DungeonEditor.uxml";
        readonly string MAIN_USS_PATH = "Assets/Editor/Dungeon/DungeonEditor.uss";

        readonly string PALETTE_ELEMENT_UXML_PATH = "Assets/Editor/Dungeon/PaletteElementTemplate/PaletteElementTemplate.uxml";
        readonly string PALETTE_ELEMENT_USS_PATH = "Assets/Editor/Dungeon/PaletteElementTemplate/PaletteElementTemplate.uss";

        BuildPaletteElementEditor buildPaletteElementEditor;

        Palette palette;

        VisualTreeAsset paletteElementTree;

        Vector2IntField gridSizeField;
        VisualElement gridTilePanel;
        Button buildButton;

        VisualElement paletteElementPanel;
        Button addPaletteButton;

        Palette.Element selectedPaletteEelment;

        [MenuItem("Window/DungeonEditor")]
        public static void ShowWindow()
        {
            DungeonEditor wnd = GetWindow<DungeonEditor>();
            wnd.titleContent = new GUIContent("DungeonEditor");
        }

        public void OnEnable()
        {
            VisualElement root = rootVisualElement;

            var mainUxml = Loader.LoadUxml(MAIN_UXML_PATH);
            root.Add(mainUxml.CloneTree());

            var mainStyleSheet = Loader.LoadUss(MAIN_USS_PATH);
            root.styleSheets.Add(mainStyleSheet);

            var paletteElementStyleSheet = Loader.LoadUss(PALETTE_ELEMENT_USS_PATH);
            root.styleSheets.Add(paletteElementStyleSheet);

            paletteElementTree = Loader.LoadUxml(PALETTE_ELEMENT_UXML_PATH);

            palette = new Palette();
            palette.changedElementEvent += UpdatePaletteMenu;
            palette.changedElementEvent += () =>
            {
                if (!palette.Any(e => e == selectedPaletteEelment))
                    selectedPaletteEelment = null;
            };

            SetBuildMenu(root);
            SetPaletteMenu(root);
        }  
       

        void SetBuildMenu(VisualElement root)
        {
            gridSizeField = root.Query<Vector2IntField>("size-field").First();
            gridTilePanel = root.Query<VisualElement>("tile-panel").First();
            buildButton = root.Query<Button>("build-button").First();

            buildButton.clickable.clicked += () =>
            {
                gridTilePanel.Clear();
                gridTilePanel.CreateGirdButton(gridSizeField.value.x, gridSizeField.value.y, button =>
                {
                    if (selectedPaletteEelment == null)
                        return;

                    button.style.backgroundImage = new StyleBackground(selectedPaletteEelment.Texture2D);
                });
            };
        }

        void SetPaletteMenu(VisualElement root)
        {
            paletteElementPanel = root.Query<VisualElement>("palette-element-panel").First();
            addPaletteButton = root.Query<Button>("add-palette-button").First();

            addPaletteButton.clickable.clicked += () =>
            {
                var buildPaletteElementEditor = GetWindow<BuildPaletteElementEditor>();
                if(this.buildPaletteElementEditor == null)
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
                elementTemplate.Query<Button>("element-button").First().style.backgroundImage = new StyleBackground(element.Texture2D);
                elementTemplate.Query<Button>("element-button").First().clickable.clicked += () => { selectedPaletteEelment = element; };
                elementTemplate.Query<Button>("remove-button").First().clickable.clicked += () => RemovePaletteElement(element);
                elementTemplate.Query<Label>("element-name").First().text = element.Name;

                paletteElementPanel.Add(elementTemplate);
            }
        }

    }
}