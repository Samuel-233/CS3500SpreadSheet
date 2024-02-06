using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;

namespace SS
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/2/3
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
        CellSheet sheet;

        public Spreadsheet()
        {
            this.sheet = new();
        }

        /// <summary>
        /// Get a specific cell's value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            CheckNameValid(name);
            return sheet.GetCellContent(name);
        }

        /// <summary>
        /// Get all cells which value is not ""
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return sheet.GetAllCellName();
        }

        /// <summary>
        /// Set a cell contents with a given number
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            return AddCell(name, number);
        }

        /// <summary>
        /// Set a cell contents with a given text
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            return AddCell(name, text);
        }

        /// <summary>
        /// Set a cell contents with a given formula
        /// </summary>
        /// <param name="name">Cell's name</param>
        /// <param name="formula"></param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            return AddCell(name, formula);
        }

        /// <summary>
        /// Get the Direct Dependents of corresponding cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            CheckNameValid(name);
            return this.sheet.GetDirectDependents(name);
        }

        /// <summary>
        /// Check if a variable name is valid or not
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidNameException"></exception>
        private void CheckNameValid(string name)
        {
            if (name == null) throw new ArgumentNullException("name is null");
            if (!Regex.IsMatch(name, @"^[a-zA-Z_]+[0-9]*$"))
                throw new InvalidNameException();
        }

        /// <summary>
        /// This is a method that convert IE to I set
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
        /// <returns>a set of to show which cells need to recalculate</returns>
        private ISet<string> AddCell(string name, object content) {
            CheckNameValid(name);
            object contentBackUp = sheet.GetCellContent(name);
            sheet.AddCell(name, content);
            try{
                GetCellsToRecalculate(IE2ISet(sheet.GetDirectDependents(name)));
            }catch(CircularException e){
                sheet.AddCell(name, contentBackUp);
                throw e;
            }
            
            return IE2ISet(GetCellsToRecalculate(name));
        }
    }
}
