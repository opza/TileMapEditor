using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class MapSet : ScriptableObject
{
    [SerializeField]
    List<Room> rooms;
    public Room[] Room => rooms.ToArray();


}