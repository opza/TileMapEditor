using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Worlds;

[CustomEditor(typeof(TileSet))]
public class TileSetInspector : UnityEditor.Editor
{
    const int TILE_NAME_FIELD_WIDTH = 250;
    const int TILE_NAME_FIELD_HEIGHT = 20;

    ReorderableList tileSetElements;

    SerializedProperty serializedTileName;
    SerializedProperty serializedIsOnlyCross;
    SerializedProperty serializedTileSetElementsProp;

    private void OnEnable()
    {
        serializedTileName = serializedObject.FindProperty("tileName");
        serializedIsOnlyCross = serializedObject.FindProperty("isOnlyCross");
        serializedTileSetElementsProp = serializedObject.FindProperty("tileSetElements");

        tileSetElements = new ReorderableList(
            serializedObject,
            serializedTileSetElementsProp,
            true,
            true,
            true,
            true);

        tileSetElements.headerHeight = 100;
        tileSetElements.elementHeight = 50;

        tileSetElements.drawHeaderCallback = DrawHeader;
        tileSetElements.drawElementCallback = DrawElement;
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        tileSetElements.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "TileSet Name");

        var isOnlyCrossToggleRect = new Rect(rect.x, rect.y + 25, 20, 20);
        serializedIsOnlyCross.boolValue = EditorGUI.Toggle(isOnlyCrossToggleRect, "IsOnlyCross", serializedIsOnlyCross.boolValue);

        var tileNameFieldRect = new Rect(rect.x, rect.y + 70, TILE_NAME_FIELD_WIDTH, TILE_NAME_FIELD_HEIGHT);
        serializedTileName.stringValue = EditorGUI.TextField(tileNameFieldRect, serializedTileName.stringValue);
    }

    void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = serializedTileSetElementsProp.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(rect, element);

    }

    
}
