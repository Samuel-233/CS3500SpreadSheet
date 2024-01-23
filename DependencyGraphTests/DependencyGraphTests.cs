using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }


        /// <summary>
        ///Test for the size after remove multiple times 
        ///</summary>
        [TestMethod()]
        public void TestSizeAfterRemove()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            Assert.AreEqual(2, t.Size);
            for (int i = 0; i < 10; i++) { t.RemoveDependency("a", "b"); }
            Assert.AreEqual(1, t.Size);
            for (int i = 0; i < 10; i++) { t.RemoveDependency("a", "c"); }
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Test for the size after replace multiple times 
        ///</summary>
        [TestMethod()]
        public void TestSizeAfterReplace()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            Assert.AreEqual(2, t.Size);
            for (int i = 0; i < 10; i++) { t.ReplaceDependents("a", new HashSet<string>() { "b" }); }
            Assert.AreEqual(1, t.Size);
            for (int i = 0; i < 10; i++) { t.ReplaceDependents("a", new HashSet<string>() { "b","c" }); }
            Assert.AreEqual(2, t.Size);
            for (int i = 0; i < 10; i++) { t.ReplaceDependents("a", new HashSet<string>() { "d","e","f" }); }
            Assert.AreEqual(3, t.Size);
            for (int i = 0; i < 10; i++) { t.ReplaceDependents("b", new HashSet<string>() { "a", "e", "f" }); }
            Assert.AreEqual(6, t.Size);
        }


        /// <summary>
        ///Test get method on empty graph
        ///</summary>
        [TestMethod()]
        public void TestEmptyGet()
        {
            DependencyGraph t = new DependencyGraph();
            IEnumerable<String> s = t.GetDependents("a");
            Assert.AreEqual(0, s.Count());
            s = t.GetDependees("a");
            Assert.AreEqual(0, s.Count());
            bool b = true;
            b = t.HasDependents("a");
            Assert.AreEqual(false, b);
            b = t.HasDependees("a");
            Assert.AreEqual(false, b);
        }


        /// <summary>
        ///Test to add a node pair with a null parm
        ///</summary>
        [TestMethod()]
        public void TestAddNullElement()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency(null, "b");
            t.AddDependency("b", null);
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        ///Test to remove a node pair with a null parm
        ///</summary>
        [TestMethod()]
        public void TestRemoveNullElement()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("a", null);
            Assert.AreEqual(1, t.Size);
        }

        /// <summary>
        ///Add a node pair with same name
        ///</summary>
        [TestMethod()]
        public void TestAddSameElement()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "a");
            t.AddDependency("a", "a");
            Assert.AreEqual(1, t.Size);
            t.AddDependency("a", "b");
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("a", "a");
            Assert.AreEqual(1, t.Size);
        }

    }
}
