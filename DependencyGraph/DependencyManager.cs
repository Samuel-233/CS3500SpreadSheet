using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
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

        public void RemoveNodePair(String dependee, String dependent){
            Node parent,child;
            dependencyGraph.TryGetValue(dependee, out parent);
            dependencyGraph.TryGetValue(dependee, out child);

            parent.RemoveChild(child);
            child.RemoveParent(parent);
            Clean(dependee );
            Clean(dependent );
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
        /// Return a node's all dependees
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Node> GetAllDependees(String name) {
            return FindNode(name).GetChildren();
        }

        /// <summary>
        /// Return a node's all dependents
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Node> GetAllDependents(String name){
            return FindNode(name).GetParents();
        }

        /// <summary>
        /// Convert a list of node to a list of node's name
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<String> NodeToNodeName(List<Node> nodes){
            List<String> names = new();
            foreach(Node node in nodes){
                names.Add(node.Name);
            }
            return names;
        }

        public int Count{get{ return dependencyGraph.Count; } }

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

        /// <summary>
        /// Remove this node from the dictionary if it has no relationship to other nodes
        /// </summary>
        /// <param name="name"></param>
        private void Clean(String name) {
            int parentCount = FindNode(name).GetParents().Count;
            int childrenCount = FindNode(name).GetChildren().Count;

            if(parentCount == 0 &&  childrenCount == 0){ dependencyGraph.Remove(name); }
        }


    }


    /// <summary>
    /// This is a node class, store two references to their parents and child
    /// </summary>
    internal class Node {

        HashSet<Node> children;
        HashSet<Node> parents;
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
        public void AddParent(Node parent) {
            parents.Add(parent);
        }

        /// <summary>
        /// Add a new child to this node
        /// </summary>
        /// <param name="child">reference to it's child</param>
        public void AddChild(Node child) {
            children.Add(child);
        }

        /// <summary>
        /// Remove a parent
        /// </summary>
        /// <param name="parent">reference to it's parent</param>
        public void RemoveParent(Node parent)
        {
            parents.Remove(parent);
        }

        /// <summary>
        /// Remove a child
        /// </summary>
        /// <param name="child">reference to it's child</param>
        public void RemoveChild(Node child)
        {
            children.Remove(child);
        }



        /// <summary>
        /// Return list of parents
        /// </summary>
        /// <returns></returns>
        public List<Node> GetParents() { return parents.ToList(); }


        /// <summary>
        /// Return list of children
        /// </summary>
        /// <returns></returns>
        public List<Node> GetChildren() { return children.ToList(); }

        /// <summary>
        /// Node's name
        /// </summary>
        public String Name{ get { return this.name; } }
    }
}
