using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Editor.Dungeon
{
    [CreateAssetMenu(fileName = "DungeonPalette", menuName = "DungeonEditor/Palette")]
    public class Palette : ScriptableObject, IEnumerable<Palette.Element>
    {
        public event Action changedElementEvent;

        List<Element> elements = new List<Element>();

        public void Add(string name, Texture2D texture2D)
        {
            elements.RemoveAll(e => e.Name == name);

            var element = new Element(name, texture2D);
            elements.Add(element);

            changedElementEvent?.Invoke();
        }  

        public void Remove(int index)
        {
            if (index >= elements.Count)
                return;

            elements.RemoveAt(index);

            changedElementEvent?.Invoke();
        }

        public void Remove(Element element)
        {
            if (!elements.Contains(element))
                return;

            elements.Remove(element);

            changedElementEvent?.Invoke();
        }

        public IEnumerator<Element> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        public class Element
        {
            public string Name { get; }
            public Texture2D Texture2D { get; }

            public Element(string name, Texture2D texture2d)
            {
                Name = name;
                Texture2D = texture2d;
            }
        }    
    }
}
