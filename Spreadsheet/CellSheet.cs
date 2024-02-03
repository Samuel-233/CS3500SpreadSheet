using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    internal class CellSheet
    {
        DependencyGraph dependency;
        Dictionary<string, Cell> cells;

        public CellSheet()
        {
            dependency = new();
            cells = new();
        }

        /// <summary>
        /// Create a new Cell to the sheet, also update this cell's father according to it's formula (if it is)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void AddCell(String name, Object content)
        {
            
            if (content is Formula) {
                IEnumerable<String> dependees = ((Formula)content).GetVariables();
                dependency.ReplaceDependees(name, dependees);
            }else dependency.ReplaceDependees(name, new HashSet<String> { });

            cells[name] =  new Cell(name, content);
        }

        /// <summary>
        /// Return a cell's direct Dependents
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<string> GetDirectDependents(String name)
        {
            if (!cells.TryGetValue(name, out Cell cell)) throw new ArgumentNullException("value is null");
            return dependency.GetDependents(name);
        }

        public IEnumerable<String> GetAllCellName() 
        {
            foreach(KeyValuePair<String, Cell> cell in cells) 
            { 
                if(cell.Value!=null)yield return cell.Value.name; 
            }
        }

        /// <summary>
        /// Get All dependees that need to cal first before cal this formula
        /// </summary>
        /// <returns></returns>
        public IEnumerable<String> GetAllDependees(String cellName)
        {
            return BFS(cellName, s => dependency.HasDependees(s), s => dependency.GetDependees(s));
        }

        /// <summary>
        /// Get All dependents that relay on this formula
        /// </summary>
        /// <returns></returns>
        public IEnumerable<String> GetAllDependents(String cellName)
        {
            return BFS(cellName, s => dependency.HasDependents(s), s => dependency.GetDependents(s));
        }

        /// <summary>
        /// A BFS Search to get A cell's all dependents or dependees
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="HasDep"></param>
        /// <param name="GetDep"></param>
        /// <returns></returns>
        private IEnumerable<String> BFS(String cellName,Func<String,bool> HasDep, Func<string,IEnumerable<String>>GetDep) {
            ClearVisitHistory();
            if (HasDep(cellName))
            {
                Queue<String> deps = new();
                deps.Enqueue(cellName);

                //BFS Search
                while (deps.Count > 0)
                {
                    string dep = deps.Dequeue();
                    IEnumerable<String> childDeps = GetDep(dep);
                    foreach (String childDep in childDeps)
                    {
                        Cell cell = cells[childDep];
                        if (!cell.visited)
                        {
                            cell.visited = true;
                            deps.Enqueue(childDep);
                            yield return childDep;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Set all node's visited to false
        /// </summary>
        private void ClearVisitHistory() {
            foreach (KeyValuePair<string, Cell> pair in cells) {
                ((Cell)pair.Value).visited = false;
            }
        }

        
    }


}