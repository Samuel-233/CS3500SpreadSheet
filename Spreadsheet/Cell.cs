using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
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
/// This is a class which is the basic element of a spreadSheet.
/// </summary>
namespace SpreadsheetUtilities
{

    internal class Cell
    {
        public Object value { get; set; }

        /// <summary>
        /// A bool to record the cell's value is newest or not
        /// </summary>
        public bool caled { get; set; }

        /// <summary>
        /// The content of the string, can be string or double, or formula
        /// </summary>
        public Object content { get; set; }

        /// <summary>
        /// Cell's name 
        /// </summary>
        public String name { get; set; }


        public Cell(String name, Object content)
        {
            this.name = name;
            if (content is String)
            {
                this.content = content;
                this.value = content;
                this.caled = true;
            }
            else if (content is Double)
            {
                this.content = content;
                this.value = content;
                this.caled = true;
            } else if (content is Formula) {
                this.content = content;
                this.caled = false;
                this.value = 0.0;
            }
        }

        /// <summary>
        /// Write this cell in to XML format
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("cell");
            writer.WriteElementString("name", this.name);
            writer.WriteElementString("content", ContentToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Return the content in string
        /// </summary>
        /// <returns></returns>
        private string ContentToString() {
            if (content is Formula)
            {
                return "=" + ((Formula)content).ToString();
            }
            else if (content is String)
            {
                return (string)content;
            }
            else return ((double)content).ToString();
        }

    }
}
