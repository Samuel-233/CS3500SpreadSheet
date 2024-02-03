namespace SpreadsheetUtilities
{
    //Node's dependent means those nodes that relay on this node (Children)
    //Node's dependee means this node's parent

    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/1/23
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
    /// This is the main code for Dependency Graph, It uses Dictionary to Store Each node, so Look up time would be O(1)
    /// </summary>

    internal class DependencyManager
    {
        private Dictionary<String, Node> dependencyGraph;

        public DependencyManager()
        {
            dependencyGraph = new();
            allowLoop = true;
        }

        public DependencyManager(bool allowLoop)
        {
            dependencyGraph = new();
            this.allowLoop = allowLoop;
        }

        /// <summary>
        /// Add a node pair to dictionary
        /// </summary>
        /// <param name="dependee">the parent node name</param>
        /// <param name="dependent">the child node name</param>
        public bool AddNodePair(String dependee, String dependent)
        {
            //If passin null values, just return false;
            if (dependee == null || dependent == null) { return false; }

            Node parent = AddNode(dependee);
            Node child = AddNode(dependent);

            bool added = false;
            added |= parent.AddChild(child);
            added |= child.AddParent(parent);
            return added;
        }

        /// <summary>
        /// Remove a node pair
        /// </summary>
        /// <param name="dependee"></param>
        /// <param name="dependent"></param>
        /// <returns>return true of successfully removed a dependee or dependent or both</returns>
        public bool RemoveNodePair(String dependee, String dependent)
        {
            //ignore the null value
            if (dependent == null || dependee == null) { return false; }

            Node parent, child;
            dependencyGraph.TryGetValue(dependee, out parent);
            dependencyGraph.TryGetValue(dependent, out child);

            bool removed = false;
            if (parent != null) { removed |= parent.RemoveChild(child); }
            if (child != null) { removed |= child.RemoveParent(parent); }

            Clean(dependee);
            Clean(dependent);
            return removed;
        }

        /// <summary>
        /// Try to find the node
        /// </summary>
        /// <param name="name">node's name</param>
        /// <returns>the target node</returns>
        public Node FindNode(String name)
        {
            if (dependencyGraph.ContainsKey(name))
            {
                return dependencyGraph[name];
            }
            return null;
        }

        /// <summary>
        /// Return a node's all dependees
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Node> GetAllDependees(String name)
        {
            Node node = FindNode(name);
            if (node != null) return node.GetParents();
            return new List<Node>();
        }

        /// <summary>
        /// Return a node's all dependents
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Node> GetAllDependents(String name)
        {
            Node node = FindNode(name);
            if (node != null) return node.GetChildren();
            return new List<Node>();
        }

        /// <summary>
        /// Convert a list of node to a list of node's name
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<String> NodeToNodeName(List<Node> nodes)
        {
            List<String> names = new();
            foreach (Node node in nodes)
            {
                names.Add(node.Name);
            }
            return names;
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

        /// <summary>
        /// Remove this node from the dictionary if it has no relationship to other nodes
        /// </summary>
        /// <param name="name"></param>
        private void Clean(String name)
        {
            Node node = FindNode(name);
            if (node == null) return;

            if (node.GetParents().Count == 0 &&
                node.GetChildren().Count == 0) { dependencyGraph.Remove(name); }
        }
    }

    /// <summary>
    /// This is a node class, has two HashSet to store their parents and children, one String for it's name
    /// </summary>
    internal class Node
    {
        private HashSet<Node> children;
        private HashSet<Node> parents;
        private String name;

        /// <summary>
        /// Add a new node
        /// </summary>
        /// <param name="name"></param>
        public Node(String name)
        {
            this.name = name;
            children = new();
            parents = new();
        }

        /// <summary>
        /// Add a new parent to this node
        /// </summary>
        /// <param name="parent">reference to it's parent</param>
        public bool AddParent(Node parent)
        {
            return parents.Add(parent);
        }

        /// <summary>
        /// Add a new child to this node
        /// </summary>
        /// <param name="child">reference to it's child</param>
        public bool AddChild(Node child)
        {
            return children.Add(child);
        }

        /// <summary>
        /// Remove a parent
        /// </summary>
        /// <param name="parent">reference to it's parent</param>
        public bool RemoveParent(Node parent)
        {
            return parents.Remove(parent);
        }

        /// <summary>
        /// Remove a child
        /// </summary>
        /// <param name="child">reference to it's child</param>
        public bool RemoveChild(Node child)
        {
            return children.Remove(child);
        }

        /// <summary>
        /// Return list of parents
        /// </summary>
        /// <returns></returns>
        public List<Node> GetParents()
        { return parents.ToList(); }

        /// <summary>
        /// Return list of children
        /// </summary>
        /// <returns></returns>
        public List<Node> GetChildren()
        { return children.ToList(); }

        /// <summary>
        /// Node's name
        /// </summary>
        public String Name
        { get { return this.name; } }
    }
}