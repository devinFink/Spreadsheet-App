using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    /// <summary>
    /// Author:    Devin Fink
    /// Partner:   None
    /// Date:      02/19/23
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Devin Fink - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Devin Fink, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    This project contains the bones for the structure of a spreadsheet. Contains a cell class
    ///    to store data and calculates dependency changes when cells are altered. Now also calculates values
    ///    as cells are updated and supports saving and retrieving from XML files. 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, Spreadsheet_Cell> cells;
        private DependencyGraph dependGraph;
        private bool isChanged;

        /// <summary>
        /// Constructor to build an empty spreadsheet with automatic assignments of delegates and version
        /// </summary>
        public Spreadsheet() : base(s => true, t => t, "default")
        {
            cells = new Dictionary<string, Spreadsheet_Cell>();
            dependGraph = new DependencyGraph();
            isChanged = false;
        }
        /// <summary>
        /// Constructor to manually give delegates and version number
        /// </summary>
        /// <param name="isValid">Delagate to check for variable validity</param>
        /// <param name="normalize">elegate to normalize all variables to a specific form</param>
        /// <param name="version">Version that the file should be</param>
        public Spreadsheet(Func<String, bool> isValid, Func<String, String> normalize, String version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Spreadsheet_Cell>();
            dependGraph = new DependencyGraph();
            isChanged = false;
        }

        /// <summary>
        /// Constructor to read in data from an xml file
        /// </summary>
        /// <param name="file">Filename to read</param>
        /// <param name="isValid">Delagate to check for variable validity</param>
        /// <param name="normalize">elegate to normalize all variables to a specific form</param>
        /// <param name="version">Version that the file should be</param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public Spreadsheet(String file, Func<String, bool> isValid, Func<String, String> normalize, String version) : base(isValid, normalize, version)
        {
            Dictionary<String, String> cellList = new Dictionary<String, String>();
            String currentCellName = String.Empty;
            isChanged = false;
            dependGraph = new DependencyGraph();
            cells = new Dictionary<string, Spreadsheet_Cell>();
            try
            {
                using (XmlReader reader = XmlReader.Create(file))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    if (version != reader["version"])
                                        throw new SpreadsheetReadWriteException("Version Number is Incorrect");
                                    break;

                                case "cell":
                                    break;

                                case "name":
                                    currentCellName = Normalize(reader.ReadString());
                                    if (!checkName(currentCellName))
                                        throw new SpreadsheetReadWriteException("Cell Name is Invalid");
                                    break;

                                case "contents":
                                    reader.MoveToContent();
                                    cellList.Add(currentCellName, reader.ReadString());
                                    break;
                            }
                        }
                        else // If it's not a start element, it's probably an end element
                        {
                            reader.ReadEndElement();
                        }
                    }
                }
                foreach (KeyValuePair<String, String> cell in cellList)
                {
                    SetContentsOfCell(cell.Key, cell.Value);
                }
            }
            catch(SpreadsheetReadWriteException)
            {
                throw new SpreadsheetReadWriteException("File Failed to load");
            }
            
        }

        /// <summary>
        /// Method used as a delegate to look up the value of a particular cell
        /// </summary>
        /// <param name="cellName">Cell to look for</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If cell does not exist or value is not parsable into a double</exception>
        protected double Lookup(String cellName)
        {
            if(!cells.ContainsKey(cellName))
            {
                throw new ArgumentException();
            }
            if (GetCellValue(cellName) is double)
                return (double)GetCellValue(cellName);
            else throw new ArgumentException();
        }

        /// <summary>
        /// Changed field allows program to know if a spreadsheet has been changed
        /// or not since the last time it has been saved
        /// </summary>
        public override bool Changed { get => isChanged; protected set => isChanged = false; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name">Cell to find </param>
        /// <returns>Cells contents in form of object</returns>
        /// <exception cref="InvalidNameException">If Name is invalid</exception>
        public override object GetCellContents(string name)
        {
            if (!checkName(name))
            {
                throw new InvalidNameException();
            }

            if (cells.ContainsKey(name))
            {
                return cells[name].getContent();
            }
            throw new InvalidNameException();
        }

        /// <summary>
        /// Returns a cells value (Not its contents)
        /// </summary>
        /// <param name="name">Cell to retrieve</param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            return cells[name].getValue();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Returns a list of all used cells
        /// </summary>
        /// <returns>All Cell Names</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }


        /// <summary>
        /// This method reads the fileName given and returns the version of that file
        /// </summary>
        /// <param name="filename">File to read</param>
        /// <returns>Srting version Number</returns>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public override string GetSavedVersion(string filename)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "spreadsheet")
                        {
                            String? version = reader.GetAttribute("version");
                            if (!(version is null))
                                return version;
                        }
                    }
                }
            }
            throw new SpreadsheetReadWriteException("Cannot find Version of file");
        }


        /// <summary>
        /// Writes the Current spreadsheet to an XML file to read it later
        /// Note: Some Code borrowed from the For-Students repository 
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            // We want some non-default settings for our XML writer.
            // Specifically, use indentation to make it more (human) readable.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            // Create an XmlWriter inside this block, and automatically Dispose() it at the end.
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteAttributeString("version", Version);

                foreach (KeyValuePair<string, Spreadsheet_Cell> cell in cells)
                {
                    cell.Value.WriteXml(writer);
                }

                writer.WriteEndElement(); // Ends the Spreadsheet block
                writer.WriteEndDocument();
            }
            isChanged = false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Sets a cell contents to a double and returns the cells that need
        /// to be refactored because of the new cell value
        /// </summary>
        /// <param name="name">Cell to change</param>
        /// <param name="number">Double to set Cell value to</param>
        /// <returns>List of all cells in need of Refactoring</returns>
        /// <exception cref="InvalidNameException">If Cell Name cannot Exist</exception>
        protected override IList<string> SetCellContents(string name, double number)
        {
            if (cells.ContainsKey(name))
            {
                cells[name].setContents(number);
            }
            else
            {
                cells.Add(name, new Spreadsheet_Cell(name, number));
            }
            dependGraph.ReplaceDependees(name, new List<String>());

            return (new List<String>(GetCellsToRecalculate(name)));
        }

        /// <summary>
        /// <inheritdoc/>
        /// Sets a cell contents to a String and returns the cells that need
        /// to be refactored because of the new cell value
        /// </summary>
        /// <param name="name">Cell to change</param>
        /// <param name="number">Double to set Cell value to</param>
        /// <returns>List of all cells in need of Refactoring</returns>
        /// <exception cref="InvalidNameException">If Cell name cannot exist</exception>
        /// <exception cref="ArgumentNullException">If text is null</exception>
        protected override IList<string> SetCellContents(string name, string text)
        {
            if (text == "")
            {
                cells.Remove(name);
                dependGraph.ReplaceDependees(name, new List<String>());
                return (new List<String>(GetCellsToRecalculate(name)));
            }

            if (cells.ContainsKey(name))
            {
                cells[name].setContents(text);
            }
            else
            {
                cells.Add(name, new Spreadsheet_Cell(name, text));
            }

            dependGraph.ReplaceDependees(name, new List<String>());
            return (new List<String>(GetCellsToRecalculate(name)));
        }

        /// <summary>
        /// <inheritdoc/>
        /// Sets a cell contents to a Formula and returns the cells that need
        /// to be refactored because of the new cell value
        /// </summary>
        /// <param name="name">Cell to change</param>
        /// <param name="number">Double to set Cell value to</param>
        /// <returns>List of all cells in need of Refactoring</returns>
        /// <exception cref="InvalidNameException">If cell name cannot exist</exception>
        /// <exception cref="ArgumentNullException">If Formula is null</exception>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            Spreadsheet_Cell previousCell;
            IEnumerable<String> previousDependees = dependGraph.GetDependees(name);

            if (cells.ContainsKey(name))
            {
                previousCell = cells[name];
                cells[name].setContents(formula);
            }
            else
            {
                cells.Add(name, new Spreadsheet_Cell(name, formula));
                previousCell = new Spreadsheet_Cell(name, "");
            }

            try
            {
                dependGraph.ReplaceDependees(name, formula.GetVariables());

                return (new List<String>(GetCellsToRecalculate(name)));
            }
            catch (CircularException)
            {
                cells[name] = previousCell;
                dependGraph.ReplaceDependees(name, previousDependees);
                throw;
            }
        }

        /// <summary>
        /// This is a wrapper method for the previous setcontents methods. It parses
        /// to find which method it needs to use, and takes care of the 
        /// normalization and validity checks before the others are called
        /// </summary>
        /// <param name="name">CellName to change or add</param>
        /// <param name="content">Content to set to the cell</param>
        /// <returns>IList of Strings representing the cells that have been recalculated due to the change.</returns>
        /// <exception cref="InvalidNameException"></exception>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            name = Normalize(name);

            if (!checkName(name))
            {
                throw new InvalidNameException();
            }
            isChanged = true;
            //First block to check if new content is a double and recalculate cells
            if (double.TryParse(content, out double d))
            {
                SetCellContents(name, d);
                foreach (String cell in GetCellsToRecalculate(name))
                {
                    cells[cell].changeValue(Lookup);
                }
                return (GetCellsToRecalculate(name).ToList());
            }
            //Second block to check if new content is formula, create formula, and recalculate cells
            else if (content.Length > 0 && content[0].Equals('='))
            {
                content = content.Substring(1);
                try
                {
                    Formula form = new Formula(content);
                    try
                    {
                        SetCellContents(name, form);
                        foreach (String cell in GetCellsToRecalculate(name))
                        {
                            cells[cell].changeValue(Lookup);
                        }
                        return (GetCellsToRecalculate(name).ToList());
                    }
                    catch (CircularException e)
                    {
                        throw e;
                    }
                }
                catch (FormulaFormatException e)
                {
                    throw e;
                }
            }
            //If not Number or Form, set cell as regular string
            else
            {
                SetCellContents(name, content);
                if (cells.ContainsKey(name))
                {
                    foreach (String cell in GetCellsToRecalculate(name))
                    {
                        cells[cell].changeValue(Lookup);
                    }
                }
                return GetCellsToRecalculate(name).ToList();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// Accesses the Dependency graph to show what cells directly depend on the current cell
        /// </summary>
        /// <param name="name">Cell to retrieve</param>
        /// <returns>List of cells</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependGraph.GetDependents(name);
        }

        /// <summary>
        /// Checks to determine if a cell name is valid and not null
        /// </summary>
        /// <param name="name">Cell to Check</param>
        /// <returns>True if valid, else false</returns>
        private bool checkName(String name)
        {
            name = Normalize(name);
            if (!IsValid(name))
            {
                return false;
            }
            if (Regex.IsMatch(name, "^[a-zA-Z]+[1-9]+$"))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// This class is for storing a cells Name, Contents, and Value. 
        /// It contains methods to change and retrieve contents, Name is immutable.
        /// </summary>
        private class Spreadsheet_Cell
        {
            private String name;
            private object contents;
            private object value;

            /// <summary>
            /// Constructor to create a new Cell
            /// </summary>
            /// <param name="name">name of Cell</param>
            /// <param name="cont">Contents of cell</param>
            public Spreadsheet_Cell(String name, object cont)
            {
                this.name = name;
                value = 0;
                contents = cont;
            }

            /// <summary>
            /// Changes the cells content to the given parameter
            /// </summary>
            /// <param name="content"></param>
            public void setContents(object content)
            {
                contents = content;
            }

            /// <summary>
            /// Changes the cells value based on its content and any dependencies
            /// </summary>
            /// <param name="content"></param>
            public void changeValue(Func<String, double> Lookup)
            {
                if (contents is double)
                {
                    value = (double)contents;
                }
                else if (contents is Formula)
                {
                    Formula form = (Formula)contents;
                    value = form.Evaluate(Lookup);
                }
                else
                {
                    value = contents;
                }
            }

            /// <summary>
            /// Write this State to an existing XmlWriter
            /// Note: This code has been retrieved from the For-Students Repository
            /// </summary>
            /// <param Name="writer"></param>
            public void WriteXml(XmlWriter writer)
            {
                writer.WriteStartElement("cell");
                // We use a shortcut to write an element with a single string
                writer.WriteElementString("name", name);
                writer.WriteElementString("contents", contents.ToString());
                writer.WriteEndElement(); // Ends the cell block
            }

            /// <summary>
            /// Retrieves a cells value
            /// </summary>
            /// <returns></returns>
            public object getValue()
            {
                return value;
            }

            /// <summary>
            /// Retrieves a cells content
            /// </summary>
            /// <returns></returns>
            public object getContent()
            {
                return contents;
            }
        }
    }
}
