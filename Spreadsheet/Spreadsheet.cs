using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SS
{

    public class Spreadsheet : AbstractSpreadsheet
    {
        CellSheet sheet;

        public Spreadsheet()
        {
            this.sheet = new();
        }

        public override object GetCellContents(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return sheet.GetAllCellName();
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            CheckNameValid(name);
            sheet.AddCell(name, number);
            return this.GetCellsToRecalculate(name) as ISet<string>;
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            CheckNameValid(name);
            sheet.AddCell(name, text);
            return this.GetCellsToRecalculate(name) as ISet<string>;
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            CheckNameValid(name);
            sheet.AddCell(name, formula);
            return this.GetCellsToRecalculate(name) as ISet<string>;
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {

            return this.sheet.GetDirectDependents(name);
        }

        private void CheckNameValid(string name)
        {
            if (name == null) throw new ArgumentNullException("name is null");
            if (!Regex.IsMatch(name, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*")) throw new InvalidNameException();
        }
    }
}
