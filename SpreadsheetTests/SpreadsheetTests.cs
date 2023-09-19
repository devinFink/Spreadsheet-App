global using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Xml;

namespace SpreadsheetTests
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
    ///    This project contains the tests for the spreadsheet project.
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// Test to get all names of all cells with values
        /// </summary>
        [TestMethod]
        public void testGetAllNames()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "10");
            testSheet.SetContentsOfCell("B2","=1 * 1");
            testSheet.SetContentsOfCell("C3", "Hello I am a String");
            Assert.IsTrue(testSheet.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(testSheet.GetNamesOfAllNonemptyCells().Contains("B2"));
            Assert.IsTrue(testSheet.GetNamesOfAllNonemptyCells().Contains("C3"));
        }

        /// <summary>
        /// Test Retrieving the contents of a single cell
        /// </summary>
        [TestMethod]
        public void testGetCells()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "10.0");
            testSheet.SetContentsOfCell("B2", "=1*1");
            testSheet.SetContentsOfCell("C3", "Hello I am a String");
            Assert.IsTrue(testSheet.GetCellContents("B2") is Formula);
            Assert.AreEqual(10.0, testSheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Test to set cell to invalid name
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void testInvalidNameSet()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "10.0");
            testSheet.SetContentsOfCell("B2", "9.99");
            testSheet.SetContentsOfCell("-_-3", "15.6");
        }

        /// <summary>
        /// Test to get value from a cell that does not eist
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void testInvalidCellRetrieval()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.GetCellContents("-_-3");
        }

        /// <summary>
        /// Test to get cell that doesnt exist but has a name that could
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void testCellDoesntExist()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.GetCellContents("a1");
        }

        /// <summary>
        /// Test to see if cells can be properly set to new contents
        /// </summary>
        [TestMethod]
        public void testRefactoringCells()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("B2", "=1 + 1");
            testSheet.SetContentsOfCell("C3", "1");
            testSheet.SetContentsOfCell("A1", "=B2 + C3");
            testSheet.SetContentsOfCell("B2", 9.99.ToString());
            testSheet.SetContentsOfCell("C3", 15.6.ToString()); 
            testSheet.SetContentsOfCell("A1", 84.0.ToString());

            Assert.AreEqual(84.0, testSheet.GetCellContents("A1"));
            Assert.IsTrue(!(testSheet.SetContentsOfCell("B2", "0").Contains("A1")));
        }

        /// <summary>
        /// Test that circular dependencies are accounted for
        /// </summary>
        [ExpectedException(typeof(CircularException))]
        [TestMethod]
        public void testCircularDependency()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "=B2 + C3");
            testSheet.SetContentsOfCell("B2", "=A1 + C3");
        }

        /// <summary>
        /// Test to add a formula that cannot exist
        /// </summary>
        [TestMethod]
        public void testDependenciesThatDontExistCell()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "=B2 + C3");
            Assert.IsTrue(testSheet.GetCellValue("A1") is FormulaError);
        }

        /// <summary>
        /// Test to remove a cell with an empty string
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void testRemoveDependency()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "=1 + 1");
            testSheet.SetContentsOfCell("A1", "");
            
            testSheet.GetCellContents("A1");
        }

        /// <summary>
        /// Tests creating a cell with an invalid name
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void testBadCellNameString()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("--", "Hello I am A string");
        }


        /// <summary>
        /// Tests creating a cell with an invalid name with formula
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void testBadCellNameFormula()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("--", "=1+1");
        }

        /// <summary>
        /// Test to see if indirect Dependencies are seen
        /// </summary>
        [TestMethod]
        public void testIndirectDependencies()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "=B2 + C3");
            testSheet.SetContentsOfCell("B2", "=D4 + D5");
            testSheet.SetContentsOfCell("C3", 15.6.ToString());
            testSheet.SetContentsOfCell("D4", 15.6.ToString());
            testSheet.SetContentsOfCell("D5", 15.6.ToString());

            Assert.IsTrue((testSheet.SetContentsOfCell("C3", "0").Contains("A1")));
        }

        /// <summary>
        /// Test Incorrect Format Structure
        /// </summary>
        [ExpectedException(typeof(FormulaFormatException))]
        [TestMethod]
        public void testFormatError()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "=B2 + C3");
            testSheet.SetContentsOfCell("B2", "=+ D5");
        }

        /// <summary>
        /// Test Incorrect Format Structure
        /// </summary>
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void testNameError()
        {
            Spreadsheet testSheet = new Spreadsheet(s => false, s => s, "1.0");
            testSheet.SetContentsOfCell("A1", "=B2 + C3");
            testSheet.SetContentsOfCell("000", "=+ D5");
        }

        /// <summary>
        /// Test to see if indirect Dependencies are seen
        /// </summary>
        [TestMethod]
        public void testFormulaValueCalculation()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "8.0");
            testSheet.SetContentsOfCell("B2", "4");
            testSheet.SetContentsOfCell("C3", "=A1 + B2");
            testSheet.SetContentsOfCell("D4", 15.6.ToString());
            testSheet.SetContentsOfCell("D5", 15.6.ToString());

            Assert.IsTrue(testSheet.GetCellValue("C3").Equals(12.0));
        }

        /// <summary>
        /// Test to see if indirect Dependencies are seen
        /// </summary>
        [TestMethod]
        public void testIndirectFormulaValueCalculation()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "8.0");
            testSheet.SetContentsOfCell("B2", "4");
            testSheet.SetContentsOfCell("C3", "=A1 + B2");
            testSheet.SetContentsOfCell("D4", "=C3 + 8");
            testSheet.SetContentsOfCell("D5", 15.6.ToString());

            Assert.IsTrue(testSheet.GetCellValue("D4").Equals(20.0));
        }

        /// <summary>
        /// Test isChanged
        /// </summary>
        [TestMethod]
        public void testisChanged()
        {
            Spreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "=1+1");
            Assert.IsTrue(testSheet.Changed);
        }

        /// <summary>
        /// Test isChanged
        /// </summary>
        [TestMethod]
        public void testisChangedFalse()
        {
            Spreadsheet testSheet = new Spreadsheet();
            Assert.IsTrue(!testSheet.Changed);
        }

        /// <summary>
        /// Test that the spreadsheet can be saved properly
        /// </summary>
        [TestMethod]
        public void testSaveSpreadsheet()
        {
            Spreadsheet testSheet = new Spreadsheet(s => true, s =>s, "1.0");
            testSheet.SetContentsOfCell("A1", "8.0");
            testSheet.SetContentsOfCell("B2", "4");
            testSheet.SetContentsOfCell("C3", "=A1 + B2");
            testSheet.SetContentsOfCell("D4", "=C3 + 8");
            testSheet.SetContentsOfCell("D5", 15.6.ToString());

            testSheet.Save("save.txt");
            Assert.IsTrue(testSheet.GetSavedVersion("save.txt") == "1.0");

        }

        /// <summary>
        /// Test that the spreadsheet can be saved properly
        /// </summary>
        [TestMethod]
        public void testReadSavedSpreadsheet()
        {
            using (XmlWriter writer = XmlWriter.Create("saveWrite.txt")) 
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

           
            AbstractSpreadsheet ss = new Spreadsheet("saveWrite.txt", s => true, s => s, "");

            Assert.IsTrue(ss.GetCellContents("A1").Equals("hello"));
        }

        /// <summary>
        /// Test that the spreadsheet will not return false version
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void testReadWrongversion()
        {
            using (XmlWriter writer = XmlWriter.Create("saveWrite.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

          
            AbstractSpreadsheet ss = new Spreadsheet("saveWrite.txt", s => true, s => s, "");
            ss.GetSavedVersion("saveWrite.txt");
            
        }

        /// <summary>
        /// Test that the spreadsheet can be saved properly
        /// </summary>
        [TestMethod]
        public void testSaveAndRead()
        {
            Spreadsheet testSheet = new Spreadsheet(s => true, s => s, "1.0");
            testSheet.SetContentsOfCell("A1", "8.0");
            testSheet.SetContentsOfCell("B2", "4");
            testSheet.SetContentsOfCell("C3", "=A1 + B2");
            testSheet.SetContentsOfCell("D4", "=C3 + 8");
            testSheet.SetContentsOfCell("D5", 15.6.ToString());

            testSheet.Save("save2.txt");
            Assert.IsTrue(testSheet.GetSavedVersion("save2.txt") == "1.0");

            Spreadsheet openSheet = new Spreadsheet("save2.txt", s => true, s => s, "1.0");

            Assert.AreEqual(8.0, openSheet.GetCellContents("A1"));


        }

        /// <summary>
        /// Test that the spreadsheet can be saved properly
        /// </summary>
        [TestMethod]
        public void testStressTest()
        {
            Spreadsheet testSheet = new Spreadsheet(s => true, s => s, "1.0");
            testSheet.SetContentsOfCell("A1", "8.0");
            testSheet.SetContentsOfCell("B2", "4");
            testSheet.SetContentsOfCell("C3", "=A1 + B2");
            testSheet.SetContentsOfCell("D4", "=C3 + 8");
            testSheet.SetContentsOfCell("D5", 15.6.ToString());
            for(int i = 0; i < 1000; i++)
            {
                testSheet.SetContentsOfCell("D5", "Hi");
                testSheet.SetContentsOfCell("D5", "=C3 + B2");
            }

            testSheet.Save("save2.txt");
            Assert.IsTrue(testSheet.GetSavedVersion("save2.txt") == "1.0");

            Spreadsheet openSheet = new Spreadsheet("save2.txt", s => true, s => s, "1.0");

            Assert.AreEqual(8.0, openSheet.GetCellContents("A1"));


        }

        /// <summary>
        /// Test that the spreadsheet will not return false version
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void testReadFileVersion()
        {

            using (XmlWriter writer = XmlWriter.Create("saveDummy.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }


            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "");
            ss.GetSavedVersion("saveDummy.txt");

        }

        /// <summary>
        /// Test that the spreadsheet will not return false version
        /// </summary>
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod]
        public void testReadBrokenFile()
        {

            using (XmlWriter writer = XmlWriter.Create("saveDummy.txt")) // NOTICE the file with no path
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "---");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }


            AbstractSpreadsheet ss = new Spreadsheet("saveDummy.txt", s => true, s => s, "");

        }
    }
}