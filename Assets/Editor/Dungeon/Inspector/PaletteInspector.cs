using UnityEngine;
using System.Collections;
using UnityEditor;

using Editor.Dungeon;
using System.Runtime.InteropServices;

[CustomEditor(typeof(Palette))]
public class PaletteInspector : UnityEditor.Editor
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        var palette = target as Palette;

        for (var i = 0; i < palette.Count; i++)   
        {
            var textureRect = r;
            textureRect.size = new Vector2(30, 30);

            var texturePos = r.position;

            texturePos.x += textureRect.size.x * i;
            textureRect.position = texturePos;

            var blockInfo = palette[i];
            if (blockInfo?.PreviewTexture == null)
                continue;

            EditorGUI.DrawPreviewTexture(textureRect, blockInfo.PreviewTexture);
        }
    }

}
