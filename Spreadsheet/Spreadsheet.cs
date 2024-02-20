using SpreadsheetUtilities;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/2/14
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
        private DependencyGraph dependency;
        private Dictionary<string, Cell> cells;

        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Constructor of Spread sheet class with out isValid, normalize Func, and version pram
        /// </summary>
        public Spreadsheet() :
            this(s => true, s => s, "default")
        {
            this.dependency = new();
            this.cells = new();
        }

        /// <summary>
        /// Constructor of Spread sheet class with isValid, normalize Func, and version pram
        /// </summary>
        /// <param name="isValid">a function to tell how is the variable should look like beside the Regex rule</param>
        /// <param name="normalize">a function to make the variable in a proper format</param>
        /// <param name="version">version to help saving</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            this.dependency = new();
            this.cells = new();
        }

        /// <summary>
        /// Constructor of Spread sheet class with isValid, normalize Func,version pram and file path
        /// </summary>
        /// <param name="pathToFile">a path to save the file</param>
        /// <param name="isValid">a function to tell how is the variable should look like beside the Regex rule</param>
        /// <param name="normalize">a function to make the variable in a proper format</param>
        /// <param name="version">version to help saving</param>
        public Spreadsheet(string pathToFile, Func<string, bool> isValid, Func<string, string> normalize, string version) :
            this(isValid, normalize, version)
        {
            LoadFile(pathToFile);
        }

        /// <summary>
        /// Get a specific cell's value
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <returns>the content of the cell</returns>
        public override object GetCellContents(string name)
        {
            NormalizeName(ref name);
            CheckNameValid(name);
            return GetCellContent(name);
        }

        /// <summary>
        /// Get all cells which value is not ""
        /// </summary>
        /// <returns>a list contain all nonempty cells</returns>
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
        /// <param name="name">The name of the cell</param>
        /// <param name="content">The new content of the cell</param>
        ///<exception cref="InvalidNameException">
        ///   If the name parameter is null or invalid, throw an InvalidNameException
        /// </exception>
        /// <returns>all elements that need to recalculate</returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            NormalizeName(ref name);
            if (content == null) return new List<string>();
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
                //TODO This might throw an exception, catch it and display in the GUI, then keep the orig content
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
        /// <param name="name">The name of the cell</param>
        /// <param name="number">the new content that want to set to the cell</param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return AddCell(name, number);
        }

        /// <summary>
        /// Set a cell contents with a given text
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <param name="number">the new content that want to set to the cell</param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            return AddCell(name, text);
        }

        /// <summary>
        /// Set a cell contents with a given formula
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <param name="number">the new content that want to set to the cell</param>
        /// <returns>a set of to show which cells need to recalculate</returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            return AddCell(name, formula);
        }

        /// <summary>
        /// Check if a variable name is valid or not
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <exception cref="InvalidNameException">Return exception when the name (variable) is null, mismatch, or invalid</exception>
        private void CheckNameValid(string name)
        {
            if (name == null || !Regex.IsMatch(name, @"^[a-zA-Z_]+[0-9]*$") || !this.IsValid(name))
                throw new InvalidNameException();
        }

        /// <summary>
        /// This is a method that convert IE to IList
        /// </summary>
        /// <param name="strings">A IEnumerable that want to turn in to IList</param>
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
        /// Add the cell to the sheet, if the cell cause a loop, undo this
        /// </summary>
        /// <param name="name">The cell's name</param>
        /// <param name="content">the new content that user want to set</param>
        /// <exception cref="CircularException"> if a circular relation found, and undo the add cell</exception>
        /// <returns>a set of to show which cells need to recalculate</returns>
        private IList<string> AddCell(string name, object content)
        {
            object contentBackUp = GetCellContent(name);
            TryAddCell(name, content);
            LinkedList<string> cellsNeedToReCal;
            try
            {
                cellsNeedToReCal = (LinkedList<string>)GetCellsToRecalculate(name);
            }
            catch (CircularException e)
            {
                AddCell(name, contentBackUp);
                throw e;
            }

            return IE2IList(cellsNeedToReCal);
        }

        /// <summary>
        /// Get the Direct Dependents of corresponding cell
        /// </summary>
        /// <param name="name">name of the cell</param>
        /// <returns>the direct dependents of the cells</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            CheckNameValid(name);
            if (!cells.TryGetValue(name, out Cell cell)) throw new ArgumentNullException("value is null");
            return dependency.GetDependents(name);
        }

        /// <summary>
        /// A func to check the file's version
        /// </summary>
        /// <param name="filename">File name include file path(If needed)</param>
        /// <returns>a version info</returns>
        /// <exception cref="SpreadsheetReadWriteException">Throw error if any error occurs</exception>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                XmlReader xmlReader = XmlReader.Create(filename);

                while (xmlReader.Read())
                {
                    //keep reading until we see your element
                    if (xmlReader.Name.Equals("spreadsheet") && (xmlReader.NodeType == XmlNodeType.Element))
                    {
                        // get attribute from the XML element here
                        return xmlReader.GetAttribute("version");
                    }
                }
                throw new SpreadsheetReadWriteException("Can not find the version");
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException($"Can not read the file {filename}, exception is {e.Message}");
            }
        }

        /// <summary>
        /// A func to save the spread sheet in to XML format
        /// </summary>
        /// <param name="filename">the file name</param>
        /// <exception cref="SpreadsheetReadWriteException">Throw error if any error occurs</exception>
        public override void Save(string filename)
        {
            if (Changed == false) return;
            try
            {
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    string xmlContent = GetXML();
                    sw.Write(xmlContent);
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
            this.Changed = false;
        }

        /// <summary>
        /// Turn the spread sheet data in to XML format
        /// </summary>
        /// <returns>a XML format string</returns>
        public override string GetXML()
        {
            Utf8StringWriter sw = new();

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8
            };

            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", this.Version);
                foreach (KeyValuePair<string, Cell> cell in cells)
                {
                    cell.Value.WriteXml(writer);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return sw.ToString();
        }

        /// <summary>
        /// A function to Evaluate the formula in the cell, or return a string if the values is a string
        /// </summary>
        /// <param name="name">the cell's name</param>
        /// <exception cref="InvalidNameException">
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// <returns>can be string, double, or formula error</returns>
        public override object GetCellValue(string name)
        {
            object o = GetCellContent(name);

            if (o is Formula)
            {
                object value;
                try
                {
                    value = ((Formula)o).Evaluate(LookUp);
                }
                catch (Exception e)
                {
                    return new FormulaError(e.Message);
                }
                Cell cell = cells[name];
                cell.value = value;
                return value;
            }
            if (o is double) { return (double)o; }
            else return (string)o;
        }

        /// <summary>
        /// Return the cell need to re calculate if change the given cell
        /// </summary>
        /// <param name="name">cell that might change</param>
        /// <returns>cells need to recalculate</returns>
        public IEnumerable<string> GetCellsNeedToReCal(string name){
            return GetCellsToRecalculate(name);
        }

        /// <summary>
        /// Create a new Cell to the sheet, also update this cell's father according to it's formula (if it is)
        /// If the justAdd param is true, it will just add the cell in to dict, and not care about the dependency, vise virsa
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <param name="content">the content want to set</param>
        private void TryAddCell(string name, object content)
        {
            if (content is Formula)
            {
                IEnumerable<string> dependees = ((Formula)content).GetVariables();
                dependency.ReplaceDependees(name, dependees);
            }
            else dependency.ReplaceDependees(name, new HashSet<string> { });
            cells[name] = new Cell(name, content);
            Changed = true;
        }

        /// <summary>
        /// Get cell's content by its name
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <exception cref="InvalidNameException">
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// <returns>Cell's content</returns>
        private object GetCellContent(string name)
        {
            Cell cell;
            CheckNameValid(name);
            if (cells.TryGetValue(name, out cell)) return cell.content;
            else return "";
        }

        /// <summary>
        /// A look up function to find cell value
        /// </summary>
        /// <param name="name">the name of the cell</param>
        /// <returns></returns>
        /// <exception cref="Exception">If the target cell is a string or cannot evaluate</exception>
        private double LookUp(string name)
        {
            Cell cell;
            NormalizeName(ref name);
            CheckNameValid(name);
            if (!cells.TryGetValue(name, out cell)) throw new Exception($"Cell {name} is empty"); ;

            //If this formula is caled, then return the value, or recursively cal the formula in it.
            if (cell.content is Formula)
            {
                if (cell.value is double) return (double)cell.value;
                else throw new Exception($"cell {cell.name} has a Formula error");
            }

            if (cell.content is double) { return (double)cell.content; }

            throw new Exception($"Cell {cell.name} is a string");
        }

        /// <summary>
        /// Update all values that needs to be re eval.
        /// </summary>
        /// <param name="cellsNeedToReCal">cells that needs to recalculate</param>
        private void UpdateValue(IList<string> cellsNeedToReCal)
        {
            foreach (string cellName in cellsNeedToReCal)
            {
                Cell cell = cells[cellName];
                GetCellValue(cellName);
            }
        }

        /// <summary>
        /// Normalize the string
        /// </summary>
        /// <param name="name">cell that need to normalize</param>
        private void NormalizeName(ref string name)
        {
            name = this.Normalize(name);
        }

        /// <summary>
        /// Load The XML File
        /// </summary>
        /// <param name="pathToFile">file location</param>
        /// <exception cref="SpreadsheetReadWriteException">Throw error if can not open it</exception>
        private void LoadFile(string pathToFile)
        {
            if (!this.Version.Equals(GetSavedVersion(pathToFile)))
            {
                throw new SpreadsheetReadWriteException("Version misMatch");
            }
            try
            {
                using (StreamReader reader = new StreamReader(pathToFile, true))
                {
                    string content = reader.ReadToEnd();
                    CreateCellFromXML(content);
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Read a string in XML format and create cell from it.
        /// </summary>
        /// <param name="XML">Data in XML Format</param>
        private void CreateCellFromXML(string XML)
        {
            // Create an XmlReader inside this block, and automatically Dispose() it at the end.
            using (XmlReader reader = XmlReader.Create(new StringReader(XML)))
            {
                string cellName = "";
                string content = "";
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "name":
                                reader.Read();
                                cellName = reader.Value;
                                break;

                            case "content":
                                reader.Read();
                                content = reader.Value;
                                SetContentsOfCell(cellName, content);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A Utf 8 String Writer Class
        /// </summary>
        internal class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}