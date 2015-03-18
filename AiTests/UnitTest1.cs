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
    }
}
