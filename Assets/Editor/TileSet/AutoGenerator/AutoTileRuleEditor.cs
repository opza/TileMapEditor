using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Editor.Utility;

public class AutoTileRuleEditor : EditorWindow
{
    readonly string TILE_SET_EXTENTION = "asset";

    readonly string MAIN_UXML_PATH = "Assets/Editor/TileSet/AutoGenerator/AutoTileRuleEditor.uxml";

    TextField tileSetSpriteFolderPathField;
    TextField tileRuleJsonPathField;
    Button loadTileSetSpriteButton;
    Button loadTileRuleJsonButton;
    Button autoGenerateButton;

    Button buildJsonFormatButton;

    [MenuItem("Window/TileSetGenerator")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<AutoTileRuleEditor>();
        wnd.titleContent = new GUIContent("AutoTileSetGenerator");
    }

    private void OnEnable()
    {
        var root = rootVisualElement;
        var mainTree = Loader.LoadUxml(MAIN_UXML_PATH);

        root.Add(mainTree.CloneTree());

        InitTileSetGenerator(root);
    }

    void InitTileSetGenerator(VisualElement root)
    { 
        tileSetSpriteFolderPathField = root.Query<TextField>("tile-set-sprite-folder-path-field").First();
        tileRuleJsonPathField = root.Query<TextField>("tile-rule-json-path-field").First();

        loadTileSetSpriteButton = root.Query<Button>("load-tile-set-sprite-folder-button").First();
        loadTileRuleJsonButton = root.Query<Button>("load-tile-rule-json-button").First();
        autoGenerateButton = root.Query<Button>("auto-generate-button").First();

        buildJsonFormatButton = root.Query<Button>("build-format-button").First();

        loadTileSetSpriteButton.clickable.clicked += () =>
        {
            var spritesFolderAbsolutePath = EditorUtility.OpenFolderPanel("Open Sprite Folder", "", "");
            var relativePath = Path.ConvertAbsoluteToUnityRelativePath(spritesFolderAbsolutePath);

            tileSetSpriteFolderPathField.value = relativePath;
        };

        loadTileRuleJsonButton.clickable.clicked += () =>
        {
            var jsonFileAbsolutePath = EditorUtility.OpenFilePanel("Open Json Rule", "", "json");
            var relativePath = Path.ConvertAbsoluteToUnityRelativePath(jsonFileAbsolutePath);
            if(System.IO.Path.GetExtension(relativePath) != ".json")
            {
                Debug.LogError("Json 형식이 아닙니다");
                return;
            }

            tileRuleJsonPathField.value = relativePath;
        };

        autoGenerateButton.clickable.clicked += () =>
        {
            var tileSet = AutoTileRuleGenerator.Genearte(tileSetSpriteFolderPathField.value, tileRuleJsonPathField.value);
            if (tileSet == null)
                return;

            var absoluteSavePath = EditorUtility.SaveFilePanel("Save TileSet", Application.dataPath, "NewTileSet", TILE_SET_EXTENTION);
            var relativeSavePath = Path.ConvertAbsoluteToUnityRelativePath(absoluteSavePath);
            if (string.IsNullOrEmpty(relativeSavePath))
                return;

            AssetDatabase.CreateAsset(tileSet, relativeSavePath);
            EditorUtility.DisplayDialog("SucessMessage", "생성 완료", "OK");
        };

        buildJsonFormatButton.clickable.clicked += () =>
        {
            var absoluteSavePath = EditorUtility.SaveFilePanel("Save Rule", Application.dataPath, "NewJsonRule", "json");
            if (string.IsNullOrEmpty(absoluteSavePath))
                return;

            AutoTileRuleGenerator.SaveJsonFormat(absoluteSavePath);
            EditorUtility.DisplayDialog("SucessMessage", "생성 완료", "OK");
        };
    }

}