using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Worlds
{
    [CustomPropertyDrawer(typeof(TileSet.TileSetElement))]
    public class TileDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            using(new EditorGUI.PropertyScope(position, label, property))
            {
                var maskProp = property.FindPropertyRelative("mask");
                DrawDirectionMask(maskProp, position, 10, 10, 15, 15);

                var spriteProp = property.FindPropertyRelative("sprite");
                var spritePos = new Rect(position);
                spritePos.x += 50;
                DrawSpriteField(spriteProp, spritePos, 40, 40);
            }
        }

        void DrawDirectionMask(SerializedProperty maskProp, Rect position, float width, float height, float xDistance, float yDistance)
        {
            var idx = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var boxRect = new Rect(position.x + x * xDistance, position.y + y * yDistance, width, height);

                    if (y == 1 && x == 1)
                    {
                        boxRect.width += 4;
                        boxRect.height += 5;

                        EditorGUI.DrawRect(boxRect, Color.green);
                        continue;
                    }
               
                    var bitMask = maskProp.GetArrayElementAtIndex(idx++);
                    bitMask.boolValue = EditorGUI.Toggle(boxRect, bitMask.boolValue);
                }
            }
        }

        void DrawSpriteField(SerializedProperty spriteProp, Rect position, float width, float height)
        {
            var spriteRect = new Rect(position.x, position.y, width, height);
            spriteProp.objectReferenceValue = EditorGUI.ObjectField(spriteRect, spriteProp.objectReferenceValue, typeof(Sprite), false);
        }

        void ClearMask(SerializedProperty maskProp)
        {
            for (int i = 0; i < maskProp.arraySize; i++)
            {
                maskProp.GetArrayElementAtIndex(i).boolValue = false;
            }
        }
    }
}
