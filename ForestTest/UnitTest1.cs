using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForestInhabitant;
using System.Drawing;

namespace ForestInhabitant
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestLoadForest()
        {
            var forest = Loader.LoadForest(@".\map.txt");
            Assert.IsTrue(forest[new Point(7, 6)] is MedKit);
        }

        [TestMethod]
        public void TestSpawnInhabitant()
        {
            var forest = Loader.LoadForest(@".\test_map.txt");
            forest.SpawnNewInhabitant("Legolas", new Point(3, 1), 3);
            var legolas = (Inhabitant)forest[new Point(3, 1)];
            Assert.IsTrue(legolas is Inhabitant);
            Assert.AreEqual(legolas.GetHitPoints(), 3);
        }

        [TestMethod]
        public void TestMoveInhabitant()
        {
            var forest = Loader.LoadForest(@".\test_map.txt");
            forest.SpawnNewInhabitant("Test", new Point(3, 1), 3);
            var test = (Inhabitant)forest[new Point(3, 1)];
            test.Move(forest, new Point(1, 0));
            Assert.IsTrue(forest[new Point(3, 1)] is Road);
            Assert.IsTrue(forest[new Point(4, 1)] is Inhabitant);
            Assert.AreEqual(test.GetLocation(), new Point(4, 1));
            Assert.AreEqual(forest[new Point(3, 1)].GetLocation(), new Point(3, 1));
        }

        [TestMethod]
        public void TestMoveInhabitantToTrap()
        {
            var forest = Loader.LoadForest(@".\test_map.txt");
            forest.SpawnNewInhabitant("Test", new Point(7, 2), 3);
            var test = (Inhabitant)forest[new Point(7, 2)];
            test.Move(forest, new Point(0, -1));
            Assert.IsTrue(forest[new Point(7, 2)] is Road);
            Assert.IsTrue(forest[new Point(7, 1)] is Inhabitant);
            Assert.AreEqual(test.GetHitPoints(), 2);
            Assert.AreEqual(test.GetLocation(), new Point(7, 1));
            Assert.AreEqual(forest[new Point(7, 2)].GetLocation(), new Point(7, 2));
        }
        [TestMethod]
        public void TestMoveFromTrap()
        {
            var forest = Loader.LoadForest(@".\test_map.txt");
            forest.SpawnNewInhabitant("Test", new Point(7, 2), 3);
            var test = (Inhabitant)forest[new Point(7, 2)];
            test.Move(forest, new Point(0, 1));
            test.Move(forest, new Point(0, 1));
            Assert.IsTrue(forest[new Point(7, 3)] is Trap);
        }
        
    }
}
