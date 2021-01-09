using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Editor.Utility;
using Worlds.Generate;

using Editor.Utility;

namespace Editor.Dungeon.Draw
{
    public class LineDrawer : IDrawer
    {
        public Action<int, int> DrawAction { get; set; }

        int startX;
        int startY;

        int endX;
        int endY;

        bool isStartedDrawing;

        public LineDrawer() { }
        public LineDrawer(Action<int,int> drawAction)
        {
            DrawAction = drawAction;
        }

        // THINGK : 결국에는 2가지일을 하는것이 아닌가?
        public void Draw(int x, int y)
        {
            if(DrawAction == null)
            {
                isStartedDrawing = false;
                return;
            }

            if(!isStartedDrawing)
            {
                startX = x;
                startY = y;
                isStartedDrawing = true;

                return;
            }

            endX = x;
            endY = y;         

            DrawLine(DrawAction, startX, startY, endX, endY);

            isStartedDrawing = false;
        }

        void DrawLine(Action<int, int> drawAction, int startX, int startY, int endX, int endY)
        {
            if (startX == startY && endX == endY)
                return;


            if (startX == endX)
            {
                DrawYLine(drawAction, startY, endY, startX);
                return;
            }

            if(startY == endY)
            {
                DrawXLine(drawAction, startX, endX, startY);
                return;
            }

            DrawDiagonalLine(drawAction, startX, startY, endX, endY);
        }

        void DrawXLine(Action<int, int> drawAction, int startX, int endX, int y)
        {
            if (startX > endX)
                Utility.Util.Swap(ref startX, ref endX);

            for (int x = startX; x <= endX; x++)
            {
                drawAction?.Invoke(x, y);
            }
        }

        void DrawYLine(Action<int, int> drawAction, int startY, int endY, int x)
        {
            if (startY > endY)
                Utility.Util.Swap(ref startY, ref endY);

            for (int y = startY; y <= endY; y++)
            {
                drawAction?.Invoke(x, y);
            }
        }

        void DrawDiagonalLine(Action<int, int> drawAction, float startX, float startY, float endX, float endY)
        {
            var graphSlope = (endY - startY) / (Mathf.Abs(endX - startX));
            var graphB = startY - graphSlope * startX;

            for (int x = (int)startX; x <= endX; x++)
            {
                var y = Mathf.RoundToInt(graphSlope * x + graphB);
                drawAction?.Invoke(x, y);
            }
        }

        
    }
}
