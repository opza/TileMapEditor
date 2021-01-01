using UnityEngine;
using UnityEditor;

using Editor.Utility;
using TMPro;

using Worlds.Generate;

[CustomEditor(typeof(Room))]
public class RoomInspector : UnityEditor.Editor
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        var room = target as Room;

        for (int x = 0; x < room.Width; x++)
        {
            for (int y = 0; y < room.Height; y++)
            {
                var textureRect = r;
                textureRect.size = new Vector2(10, 10);

                var texturePos = r.position;

                texturePos.x += textureRect.size.x * x;
                texturePos.y += textureRect.size.y * y;

                textureRect.position = texturePos;

                var tile = room.GetTile(x, y);
                if (tile?.BlockInfo?.PreviewTexture == null)
                    continue;

                EditorGUI.DrawPreviewTexture(textureRect, tile.BlockInfo.PreviewTexture);
            }
        }
    }

   // public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
   //{
   //     var icon = new Texture2D(width, height);
   //     EditorUtility.CopySerialized(this.icon, icon);

   //     return icon;
   // }
}