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
/// This is a class which is the basic element of a spreadSheet.
/// </summary>
namespace SpreadsheetUtilities
{

    internal class Cell
    {
        public Object value { get; set; }
        bool caled;

        public Object content { get; set; }
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
                //TODO Eval this!
            }
        }


    }
}
