using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Worlds.Generate;
using Util;

namespace Worlds
{
    public class MapGeneratorManager : MonoBehaviour
    {
        World world;

        [SerializeField]
        int roomMinSize = 10;

        [SerializeField]
        int roomMaxSize = 10;

        // TODO : 나중에 Custom Insepctor 추가해야함
        [SerializeField]
        List<MapSet> level0Maps;

        [SerializeField]
        int level0Depth;

        [SerializeField]
        List<MapSet> level1Maps;

        [SerializeField]
        int level1Depth;

        [SerializeField]
        List<MapSet> level2Maps;

        [SerializeField]
        int level2Depth;


        MapGenerator mapGenerator;

        private void Awake()
        {
            world = WorldManager.World;

            var levelMapSets = new List<MapGenerator.LevelMapSet>()
            {
                new MapGenerator.LevelMapSet(0, level0Maps, level0Depth),
                new MapGenerator.LevelMapSet(1, level1Maps, level1Depth),
                new MapGenerator.LevelMapSet(2, level2Maps, level2Depth)
            };

            mapGenerator = MapGenerator.CreateGenerator(world, levelMapSets);

        }

        private void Start()
        {
            mapGenerator.Generate(0);
        }
    }

       
}
