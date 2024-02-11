using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace SS
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/2/10
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
    /// This is a Spread sheet class which implement Abstract Spreadsheet class, It can add dependency between cells by cells formula
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        DependencyGraph dependency;
        Dictionary<string, Cell> cells;


        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        //TODO Let user can do : AbstractSpreadsheet sheet3 = new Spreadsheet(PathToFile, ValidityDelegate, NormalizeDelegate, VersionString);

        /// <summary>
        /// Constructor of Spread sheet class with out isValid, normalize Func, and version pram
        /// </summary>
        public Spreadsheet() :
            this(s => true,s => s, "default version")
        {
            this.dependency = new();
            this.cells = new();
        }
        /// <summary>
        /// Constructor of Spread sheet class with isValid, normalize Func, and version pram
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            this.dependency = new();
            this.cells = new();
        }

        /// <summary>
        /// Get a specific cell's value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            CheckNameValid(name);
            return GetCellContent(name);
        }

        /// <summary>
        /// Get all cells which value is not ""
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (KeyValuePair<string, Cell> cell in cells)
            {
                if (cell.Value != null && !((Cell)cell.Value).value.Equals("")) yield return cell.Value.name;
            }
        }

        /// <summary>
        /// If content can parse to double, then add it directly, if starts with "=" then add it as formula, else add as string to the cell content
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        ///<exception cref="InvalidNameException"> 
        ///   If the name parameter is null or invalid, throw an InvalidNameException
        /// </exception>
        /// <returns>all elements that need to recalculate</returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            CheckNameValid(name);
            double number;
            IList<string> reCalList;
            if (double.TryParse(content, out number))
            {
                reCalList = SetCellContents(name, number);
                UpdateValue(reCalList);
                return reCalList;
            }
            else if (content.Length > 0 && content[0].Equals(Char.Parse("=")))
            {
                reCalList = SetCellContents(name, new Formula(content.Substring(1), this.Normalize, this.IsValid));
                UpdateValue(reCalList);
                return reCalList;
            }
            
            reCalList = SetCellContents(name, content);
            UpdateValue(reCalList);
            return reCalList;

        }

        /// <summary>
        /// Set a cell contents with a given number
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return AddCell(name, number);
        }

        /// <summary>
        /// Set a cell contents with a given text
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            return AddCell(name, text);
        }

        /// <summary>
        /// Set a cell contents with a given formula
        /// </summary>
        /// <param name="name">Cell's name</param>
        /// <param name="formula"></param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            return AddCell(name, formula);
        }



        /// <summary>
        /// Check if a variable name is valid or not
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        private void CheckNameValid(string name)
        {
            if (name == null || !Regex.IsMatch(name, @"^[a-zA-Z_]+[0-9]*$") || !this.IsValid(name))
                throw new InvalidNameException();
        }

        /// <summary>
        /// This is a method that convert IE to IList
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private IList<string> IE2IList(IEnumerable<string> strings)
        {
            IList<string> list = new List<string>();
            foreach (string s in strings)
            {
                list.Add(s);
            }
            return list;
        }

        /// <summary>
        /// This is a method that convert IE to ISet
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private ISet<string> IE2ISet(IEnumerable<string> strings)
        {
            ISet<string> set = new HashSet<string>();
            foreach (string s in strings)
            {
                set.Add(s);
            }
            return set;
        }


        /// <summary>
        /// Add the cell to the sheet, if the cell cause a loop, undo this
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <exception cref="CircularException"> if a circular relation found, and undo the add cell</exception>
        /// <returns>a set of to show which cells need to recalculate</returns>
        private IList<string> AddCell(string name, object content) {
            object contentBackUp = GetCellContent(name);
            TryAddCell(name, content);
            LinkedList<string> cellsNeedToReCal;
            try
            {
                cellsNeedToReCal = (LinkedList<string>)GetCellsToRecalculate(IE2ISet(GetDirectDependents(name)));
                cellsNeedToReCal.AddFirst(name);
            }catch(CircularException e){
                AddCell(name, contentBackUp);
                throw e;
            }
            
            return IE2IList(cellsNeedToReCal);
        }

        /// <summary>
        /// Get the Direct Dependents of corresponding cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            CheckNameValid(name);
            if (!cells.TryGetValue(name, out Cell cell)) throw new ArgumentNullException("value is null");
            return dependency.GetDependents(name);
        }



        public override string GetSavedVersion(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public override string GetXML()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            
            object o = GetCellContent(name);
            
            if(o is Formula) {
                object value;
                try
                {
                    value = ((Formula)o).Evaluate(LookUp);
                }catch(Exception e){
                    return new FormulaError(e.Message);
                }
                return value;
            }
            if (o is double) { return (double)o; }
            else return (string)o;

            

        }

        

        /// <summary>
        /// Create a new Cell to the sheet, also update this cell's father according to it's formula (if it is)
        /// If the justAdd param is true, it will just add the cell in to dict, and not care about the dependency, vise virsa
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        private void TryAddCell(string name, object content)
        {
            if (content is Formula)
            {
                IEnumerable<string> dependees = ((Formula)content).GetVariables();
                dependency.ReplaceDependees(name, dependees);
            }
            else dependency.ReplaceDependees(name, new HashSet<string> { });
            cells[name] = new Cell(name, content);
        }

        /// <summary>
        /// Get cell's content by its name
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// <returns></returns>
        private object GetCellContent(string name)
        {
            Cell cell;
            CheckNameValid(name);
            if (cells.TryGetValue(name, out cell)) return cell.content;
            else return "";
        }

        /// <summary>
        /// A look up function to find 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private double LookUp(string name)
        {
            Cell cell;
            CheckNameValid(name);
            if (!cells.TryGetValue(name, out cell)) return 0.0;

            //If this formula is caled, then return the value, or recursively cal the formula in it.
            if (cell.content is Formula) {

                if (cell.caled) return (double)cell.value;

                object result = ((Formula)cell.content).Evaluate(LookUp);
                if (result is double)
                {
                    cell.caled = true;
                    cell.value = result;
                    return (double)result;
                } else
                {
                    //Throw the error (Get value func will catch it and return a formula exception)
                    throw new Exception($"Cell {cell.name} can not evaluate");
                }
            }

            if(cell.content is double){ return (double)cell.content; }

            throw new Exception($"Cell {cell.name} is a string");
        }

        /// <summary>
        /// Update all values that needs to be re eval.
        /// </summary>
        /// <param name="cellsNeedToReCal"></param>
        private void UpdateValue(IList<string> cellsNeedToReCal){
            foreach(string cellName in cellsNeedToReCal){
                Cell cell = cells[cellName];
                cell.caled=false;
                GetCellValue(cellName);
            }
        }

    }


}
