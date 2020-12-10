using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Util;

namespace Worlds
{
    public class MapGenerator
    {
        World world;
        Dictionary<string, MapSet> mapSets;

        public MapGenerator(World world, string mapSetsPath)
        {
            this.world = world;
            mapSets = ResourceLoader.LoadResources<MapSet>(mapSetsPath);
        }

        public void Generate(string mapName)
        {
            if (!mapSets.ContainsKey(mapName))
                return;

            var mapSet = mapSets[mapName];
            
        }


    }
}
