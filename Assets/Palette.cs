using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;
using UnityEngine.WSA;

namespace Editor.Dungeon
{
    [CreateAssetMenu(fileName = "DungeonPalette", menuName = "DungeonEditor/Palette")]
    public class Palette : ScriptableObject, IEnumerable<Palette.Element>
    {
        readonly static int MIN_ARRAY_INDEX = 0;

        public event Action addElementEvent;
        public event Action removeElementEvent;

        [SerializeField]
        List<Element> elements = new List<Element>() { Element.CreateEmpty() };

        public Element this[int i] => elements[i];

        public int Count => elements.Count;

        public void Add(string name, Texture2D texture2D, Element.TileType type)
        {
            elements.RemoveAll(e => e.Name == name);

            var element = Element.Create(name, texture2D, type);
            elements.Add(element);

            addElementEvent?.Invoke();
        }  

        public void Remove(Element element)
        {
            var idx = elements.IndexOf(element);
            Remove(idx);
        }

        public void Remove(int index)
        {
            if (MIN_ARRAY_INDEX <= 0 || index >= elements.Count)
                return;

            elements.RemoveAt(index);

            removeElementEvent?.Invoke();
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        [Serializable]
        public class Element
        {
            public enum TileType {Empty, Block, Door }

            [SerializeField]
            string name;
            public string Name => name;

            [SerializeField]
            Texture2D texture2D;
            public Texture2D Texture2D => texture2D;

            [SerializeField]
            TileType type;
            public TileType Type => type;

            public static Element Create(string name, Texture2D texture2d, TileType type)
            {
                return new Element(name, texture2d, type);
            }

            public static Element CreateEmpty()
            {
                return new Element(TileType.Empty);
            }

            Element(string name, Texture2D texture2D, TileType type)
            {
                this.type = type;
                this.name = name;
                this.texture2D = texture2D;
            }

            Element(TileType type)
            {
                this.type = type;
            }
        }    
    }
}
