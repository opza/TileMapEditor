using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Editor.Utility;
using Worlds;


////[CustomEditor(typeof(TileSet))]
//public class TileSetInspector : UnityEditor.Editor
//{
//    const int TILE_NAME_FIELD_WIDTH = 250;
//    const int TILE_NAME_FIELD_HEIGHT = 20;

//    ReorderableList tileSetElements;

//    SerializedProperty serializedTileName;
//    SerializedProperty serializedIsOnlyCross;
//    SerializedProperty serializedTileSetElementsProp;

//    private void OnEnable()
//    {
//        serializedTileName = serializedObject.FindProperty("tileName");
//        serializedIsOnlyCross = serializedObject.FindProperty("isOnlyCross");
//        serializedTileSetElementsProp = serializedObject.FindProperty("tileSetElements");

//        tileSetElements = new ReorderableList(
//            serializedObject,
//            serializedTileSetElementsProp,
//            true,
//            true,
//            true,
//            true);

//        tileSetElements.headerHeight = 100;
//        tileSetElements.elementHeight = 50;

//        tileSetElements.drawHeaderCallback = DrawHeader;
//        tileSetElements.drawElementCallback = DrawElement;

//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        tileSetElements.DoLayoutList();
//        serializedObject.ApplyModifiedProperties();
//    }

//    void DrawHeader(Rect rect)
//    {
//        EditorGUI.LabelField(rect, "TileSet Name");

//        var isOnlyCrossToggleRect = new Rect(rect.x, rect.y + 25, 20, 20);
//        serializedIsOnlyCross.boolValue = EditorGUI.Toggle(isOnlyCrossToggleRect, "IsOnlyCross", serializedIsOnlyCross.boolValue);

//        var tileNameFieldRect = new Rect(rect.x, rect.y + 70, TILE_NAME_FIELD_WIDTH, TILE_NAME_FIELD_HEIGHT);
//        serializedTileName.stringValue = EditorGUI.TextField(tileNameFieldRect, serializedTileName.stringValue);
//    }

//    void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
//    {
//        var element = serializedTileSetElementsProp.GetArrayElementAtIndex(index);
//        EditorGUI.PropertyField(rect, element);

//    }


//}


[CustomEditor(typeof(TileSet))]
public class TileSetInspector_Test : UnityEditor.Editor
{
    readonly string MAIN_UXML_PATH = "Assets/Editor/TileSet/TileSet.uxml";

    TileSet tileSet;

    VisualElement root;

    TextField tileNameField;
    Toggle onlyCrossToggle;
    ListView tilesListView;

    Button addTilesButton;
    Button removeTileButton;

    TextField tileSetSpriteFolderPathField;
    TextField tileRuleJsonPathField;
    Button loadTileSetSpriteButton;
    Button loadTileRuleJsonButton;
    Button autoGenerateButton;

    private void OnEnable()
    {
        tileSet = target as TileSet;

        var mainVisualTree = Loader.LoadUxml(MAIN_UXML_PATH);

        root = new VisualElement();
        root.Add(mainVisualTree.CloneTree());

        InitTileSet(root);
        InitTileSetGenerator(root);
    }

    public override VisualElement CreateInspectorGUI()
    {
        return root;
    }

    void InitTileSet(VisualElement root)
    {
        tileNameField = root.Query<TextField>("tile-name-field").First();
        onlyCrossToggle = root.Query<Toggle>("only-cross-toggle").First();
        tilesListView = root.Query<ListView>("tiles-list-view").First();

        addTilesButton = root.Query<Button>("add-tile-button").First();
        removeTileButton = root.Query<Button>("remove-tile-button").First();

        tileNameField.BindProperty(serializedObject.FindProperty("tileName"));
        onlyCrossToggle.BindProperty(serializedObject.FindProperty("onlyCross"));

        var serializedTileSetElements = serializedObject.FindProperty("tileSetElements");

        tilesListView.bindItem = (e, i) =>
        {
            if (i >= serializedTileSetElements.arraySize)
                return;

            (e as PropertyField).BindProperty(serializedTileSetElements.GetArrayElementAtIndex(i));
        };
        tilesListView.BindProperty(serializedTileSetElements);

        addTilesButton.clickable.clicked += () => tileSet.AddEmpty();
        removeTileButton.clickable.clicked += () => tileSet.RemoveAt(tilesListView.selectedIndex);
    }

    void InitTileSetGenerator(VisualElement root)
    {
        tileSetSpriteFolderPathField = root.Query<TextField>("tile-set-sprite-folder-path-field").First();
        tileRuleJsonPathField = root.Query<TextField>("tile-rule-json-path-field").First();

        loadTileSetSpriteButton = root.Query<Button>("load-tile-set-sprite-folder-button").First();
        loadTileRuleJsonButton = root.Query<Button>("load-tile-rule-json-button").First();
        autoGenerateButton = root.Query<Button>("auto-generate-button").First();



    }
}
