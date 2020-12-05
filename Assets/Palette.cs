using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;

namespace Editor.Dungeon
{
    [CreateAssetMenu(fileName = "DungeonPalette", menuName = "DungeonEditor/Palette")]
    public class Palette : ScriptableObject, IEnumerable<Palette.Element>
    {
        public event Action addElementEvent;
        public event Action removeElementEvent;

        [SerializeField]
        List<Element> elements = new List<Element>() { Element.CreateEmpty() };

        public Element this[int i] => elements[i];

        public int Count => elements.Count;

        public void Add(string name, Texture2D texture2D)
        {
            elements.RemoveAll(e => e.Name == name);

            var element = Element.Create(name, texture2D);
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
            if (index <= 0 || index >= elements.Count)
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
            [SerializeField]
            string name;
            public string Name => name;

            [SerializeField]
            Texture2D texture2D;
            public Texture2D Texture2D => texture2D;

            public static Element Create(string name, Texture2D texture2d)
            {
                return new Element(name, texture2d);
            }

            public static Element CreateEmpty()
            {
                return new Element();
            }

            Element(string name, Texture2D texture2d)
            {
                this.name = name;
                this.texture2D = texture2d;
            }

            Element() { }
        }    
    }
}
