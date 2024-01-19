using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyGraph
{
//Node's dependent means those nodes that relay on this node
//Node's dependee means this node's parent

    internal class DependencyManager
    {
        private Dictionary<String, Node> dependencyGraph;

        public DependencyManager(){
            dependencyGraph = new();
        }


    
        /// <summary>
        /// Add a node pair to dictionary
        /// </summary>
        /// <param name="dependee">the parent node</param>
        /// <param name="dependent">the child node</param>
        public void AddNodePair(String dependee, String dependent){
            Node parent = AddNode(dependee);
            Node child = AddNode(dependent);

            parent.AddChild(child);
            child.AddParent(parent);
        }

        /// <summary>
        /// Try to find the node
        /// </summary>
        /// <param name="name">node's name</param>
        /// <returns>the target node</returns>
        /// <exception cref="Exception">Throw error if node doesn't exist</exception>
        public Node FindNode(String name){
            if(dependencyGraph.ContainsKey(name)){
            return dependencyGraph[name];
            }throw new Exception($"can not find the node called {name}");
        }

        /// <summary>
        /// return the size of the dictionary
        /// </summary>
        public int Count() { return dependencyGraph.Count;  }

        /// <summary>
        /// Return a node's dependee Count
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int dependeeCount(String name) {
            return this.FindNode(name).ParentCount();
        }

        /// <summary>
        /// Return a node's dependent Count
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int dependentCount(String name)
        {
            return this.FindNode(name).ChildCount();
        }

        /// <summary>
        /// Add a new node to the dictionary (if it doesn't exist)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>return the node just added or the existing node</returns>
        private Node AddNode(String name)
        {
            Node node = new Node(name);
            if (dependencyGraph.TryAdd(name, node))
            {
                return node;
            }
            else return dependencyGraph[name];
        }




    }


    /// <summary>
    /// This is a node class, store two references to their parents and child
    /// </summary>
    internal class Node {

        List<Node> children;
        List<Node> parents;
        String name;


        /// <summary>
        /// Add a new node
        /// </summary>
        /// <param name="name"></param>
        public Node(String name) {
            this.name = name;
            children = new();
            parents = new();
        }


        /// <summary>
        /// Add a new parent to this node
        /// </summary>
        /// <param name="parent">reference to it's parent</param>
        public void AddParent(Node parent){
            parents.Add(parent);
        }

        /// <summary>
        /// Add a new child to this node
        /// </summary>
        /// <param name="child">reference to it's child</param>
        public void AddChild(Node child){
            children.Add(child);
        }

        /// <summary>
        /// Return parents count
        /// </summary>
        /// <returns></returns>
        public int ParentCount(){ return parents.Count; }

        /// <summary>
        /// Return Children count
        /// </summary>
        /// <returns></returns>
        public int ChildCount(){ return children.Count; }

    }
}
