using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// This is a class which is the basic element of a spreadSheet.
    /// </summary>
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
