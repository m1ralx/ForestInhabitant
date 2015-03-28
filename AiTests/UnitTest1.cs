using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForestInhabitant;
using System.Drawing;

namespace AiTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMoveAi()
        {
            var forest = Loader.LoadForest(@".\map.txt");
            forest.SpawnNewInhabitant("lol", new Point(7, 2));
            var bot = (Inhabitant)forest[new Point(7, 2)];
            var ai = new Ai(bot, new Point(7, 3), forest.MoveInhabitant, forest.Width, forest.Height);
            while (true)
            {
                ai.MoveInhabitantToDestination();
                if (bot.GetLocation() == new Point(7, 3))
                    break;
            }
            Assert.AreEqual(bot.GetLocation(),new Point(7, 3));
        }

        [TestMethod]
        public void TestMoveUp()
        {
            var forest = Loader.LoadForest(@".\map1.txt");
            forest.SpawnNewInhabitant("lol", new Point(0, 4));
            var bot = (Inhabitant)forest[new Point(0, 4)];
            var ai = new Ai(bot, new Point(0, 0), forest.MoveInhabitant, forest.Width, forest.Height);
            while (true)
            {
                ai.MoveInhabitantToDestination();
                if (bot.GetLocation() == new Point(0, 0))
                    break;
            }
            Assert.AreEqual(bot.GetLocation(), new Point(0, 0));
        }
        [TestMethod]
        public void TestMoveDown()
        {
            var forest = Loader.LoadForest(@".\map1.txt");
            forest.SpawnNewInhabitant("lol", new Point(0, 0));
            var bot = (Inhabitant)forest[new Point(0, 0)];
            var ai = new Ai(bot, new Point(0, 4), forest.MoveInhabitant, forest.Width, forest.Height);
            while (true)
            {
                ai.MoveInhabitantToDestination();
                if (bot.GetLocation() == new Point(0, 4))
                    break;
            }
            Assert.AreEqual(bot.GetLocation(), new Point(0, 4));
        }
        [TestMethod]
        public void TestMoveDiagonal()
        {
            var forest = Loader.LoadForest(@".\map1.txt");
            forest.SpawnNewInhabitant("lol", new Point(0, 0));
            var bot = (Inhabitant)forest[new Point(0, 0)];
            var ai = new Ai(bot, new Point(4, 4), forest.MoveInhabitant, forest.Width, forest.Height);
            while (true)
            {
                ai.MoveInhabitantToDestination();
                if (bot.GetLocation() == new Point(4, 4))
                    break;
            }
            Assert.AreEqual(bot.GetLocation(), new Point(4, 4));
        }
        [TestMethod]
        public void TestMoveEzMaze()
        {
            var forest = Loader.LoadForest(@".\ez_maze.txt");
            forest.SpawnNewInhabitant("lol", new Point(1, 1));
            var bot = (Inhabitant)forest[new Point(1, 1)];
            var ai = new Ai(bot, new Point(9, 8), forest.MoveInhabitant, forest.Width, forest.Height);
            while (true)
            {
                ai.MoveInhabitantToDestination();
                if (bot.GetLocation() == new Point(9, 8))
                    break;
            }
            Assert.AreEqual(bot.GetLocation(), new Point(9, 8));
        }
        [TestMethod]
        public void TestMoveMedMaze()
        {
            var forest = Loader.LoadForest(@".\med_maze.txt");
            forest.SpawnNewInhabitant("lol", new Point(1, 1));
            var bot = (Inhabitant)forest[new Point(1, 1)];
            var ai = new Ai(bot, new Point(9, 8), forest.MoveInhabitant, forest.Width, forest.Height);
            while (true)
            {
                ai.MoveInhabitantToDestination();
                if (bot.GetLocation() == new Point(9, 8))
                    break;
            }
            Assert.AreEqual(bot.GetLocation(), new Point(9, 8));
        }
        
    }
}
