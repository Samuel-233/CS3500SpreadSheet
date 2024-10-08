﻿// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
/// <summary>
/// Author:    Shu Chen
/// Partner:   None
/// Date:      2024/2/15
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Shu Chen - This work may not
///            be copied for use in Academic Coursework.
///
/// I, Shu Chen, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All
/// references used in the completion of the assignments are cited
/// in my README file.
///
/// File Contents
///
/// This is a class to record dependency graph by calling dependency manager
/// </summary>

namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    ///
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a
    /// set, and the element is already in the set, the set remains unchanged.
    ///
    /// Given a DependencyGraph DG:
    ///
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)
    ///
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on)
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private DependencyManager dependencyGraph;
        private int size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependencyGraph = new();
            size = 0;
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return dependencyGraph.GetAllDependees(s).Count; }
        }

        /// <summary>
        ///  Reports whether dependents(s) is non-empty.
        /// </summary>
        /// <param name="s">node name</param>
        /// <returns></returns>
        public bool HasDependents(string s)
        {
            return dependencyGraph.GetAllDependents(s).Count > 0;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependencyGraph.GetAllDependees(s).Count > 0;
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            return dependencyGraph.GetAllDependents(s);
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            return dependencyGraph.GetAllDependees(s);
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        ///
        /// <para>This should be thought of as:</para>
        ///
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        ///
        public void AddDependency(string s, string t)
        {
            if (dependencyGraph.AddNodePair(s, t)) size++;
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependencyGraph.RemoveNodePair(s, t)) size--;
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            List<String> dependents = (List<string>)GetDependents(s);
            //Remove all dependents for node s
            foreach (String dependent in dependents)
            {
                if (dependencyGraph.RemoveNodePair(s, dependent))
                {
                    size--;
                }
            }

            //Add new dependents
            foreach (String dependent in newDependents)
            {
                if (dependencyGraph.AddNodePair(s, dependent))
                {
                    size++;
                }
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            List<String> dependees = (List<string>)GetDependees(s);
            //Remove all dependees for node s
            foreach (String dependee in dependees)
            {
                if (dependencyGraph.RemoveNodePair(dependee, s))
                {
                    size--;
                }
            }

            //Add new dependees
            foreach (String dependee in newDependees)
            {
                if (dependencyGraph.AddNodePair(dependee, s))
                {
                    size++;
                }
            }
        }
    }
}