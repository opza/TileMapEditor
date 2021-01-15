using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Worlds;
using Editor.Utility;

// TODO : UI Toolkit 및 자동으로 타일을 Split하게 바꿔야함

[CustomPropertyDrawer(typeof(TileSet.TileSetElement))]
public class TileDrawer : PropertyDrawer
{
    readonly string MAIN_UXML_PATH = "Assets/Editor/TileSet/TileDrawer.uxml";

    PropertyField spriteField;
    List<Toggle> maskToggles;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var mainVisulTree = Loader.LoadUxml(MAIN_UXML_PATH);

        var root = new VisualElement();
        root.Add(mainVisulTree.CloneTree());

        Init(root, property);

        return root;
    }

    void Init(VisualElement root, SerializedProperty property)
    {
        spriteField = root.Query<PropertyField>("sprite-field").First();
        maskToggles = root.Query<Toggle>(className: "mask-toggle").ToList();

        var spriteProperty = property.FindPropertyRelative("sprite");
        var maskProperty = property.FindPropertyRelative("mask");

        spriteField.BindProperty(spriteProperty);

        for (int i = 0; i < maskToggles.Count; i++)
        {
            maskToggles[i].BindProperty(maskProperty.GetArrayElementAtIndex(i));
        }

    }
}

