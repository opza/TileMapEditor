using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Editor.Utility;
using Worlds;

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

    

    private void OnEnable()
    {
        tileSet = target as TileSet;

        var mainVisualTree = Loader.LoadUxml(MAIN_UXML_PATH);

        root = new VisualElement();
        root.Add(mainVisualTree.CloneTree());

        InitTileSet(root);
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

        tilesListView.makeItem += () => new PropertyField();
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


}
