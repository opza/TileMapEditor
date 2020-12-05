using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


using Editor;
using Editor.Dungeon;
using Editor.Utility;

public class BuildPaletteElementEditor : EditorWindow
{
    readonly string UXML_PATH = "Assets/Editor/Dungeon/BuildPaletteElementEditor/BuildPaletteElementEditor.uxml";
    readonly string USS_PATH = "Assets/Editor/Dungeon/BuildPaletteElementEditor/BuildPaletteElementEditor.uss";

    TextField nameField;
    public TextField NameField => nameField;

    ObjectField objectField;
    public ObjectField ObjectField => objectField;

    Button addElementButton;
    public Button AddElementButton => addElementButton;

    DungeonEditor dungeonEditor;

    public void OnEnable()
    {
        VisualElement root = rootVisualElement;

        var uxml = Loader.LoadUxml(UXML_PATH);
        root.Add(uxml.CloneTree());

        var styleSheet = Loader.LoadUss(USS_PATH);
        root.styleSheets.Add(styleSheet);
      
        SetAddElementMenu(root);
    }

    public void SetDungeonEditor(DungeonEditor dungeonEditor)
    {
        this.dungeonEditor = dungeonEditor;
    }

    void SetAddElementMenu(VisualElement root)
    {
        nameField = root.Query<TextField>("name-field").First();

        objectField = root.Query<ObjectField>("texture-field").First();
        objectField.objectType = typeof(Texture2D);

        addElementButton = root.Query<Button>("add-element-button").First();
    }

}