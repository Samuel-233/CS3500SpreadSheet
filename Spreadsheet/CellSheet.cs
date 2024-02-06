using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author:    Shu Chen
/// Partner:   None
/// Date:      2024/2/6
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
/// This is a class which is to store the basic element of a spreadSheet.
/// </summary>
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
        /// If the justAdd param is true, it will just add the cell in to dict, and not care about the dependency, vise virsa
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void AddCell(String name, Object content)
        {
            if (content is Formula) {
                IEnumerable<String> dependees = ((Formula)content).GetVariables();
                dependency.ReplaceDependees(name, dependees);
            }else dependency.ReplaceDependees(name, new HashSet<String> { });
            cells[name] = new Cell(name, content);
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

        /// <summary>
        /// Loop over all cells and extract their name
        /// </summary>
        /// <returns></returns>
        public IEnumerable<String> GetAllCellName() 
        {
            foreach(KeyValuePair<String, Cell> cell in cells) 
            { 
                if(cell.Value!=null&&!((Cell)cell.Value).value.Equals("") ) yield return cell.Value.name; 
            }
        }

        /// <summary>
        /// Get cell's content by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetCellContent(string name){
            Cell cell;
            if (cells.TryGetValue(name, out cell)) return cell.content;
            else return "";
        }


    }


}