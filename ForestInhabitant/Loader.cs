using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public class Loader
    {
        private static ForestObject[,] GetMapFromLines(string[] linesFromFile)
        {
            var width = linesFromFile[0].Length;
            var height = linesFromFile.Length;
            var map = new ForestObject[linesFromFile[0].Length, linesFromFile.Length];
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var cellType = linesFromFile[y][x].ToString();
                    map[x, y] = Forest.CellTypes[cellType](new Point(x, y));
                }
            return map;
        }
        public static IForest LoadForest(string pathToFile)
        {
            Forest.InitCellTypes();
            var linesFromFile = File.ReadAllLines(pathToFile);
            return new Forest(GetMapFromLines(linesFromFile));
        }
    }
}
