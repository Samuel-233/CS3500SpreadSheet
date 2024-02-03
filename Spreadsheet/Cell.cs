using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetUtilities
{
    internal class Cell
    {
        Object content;
        Object value;
        bool caled;
        

        //Name of the cell, or position of the cell
        public String name { get; set; }

        /*        Type type;

                enum Type{
                    String = 0,
                    Double = 1,
                    Error = 2
                }
        */

        public bool visited { get; set; }

        public Cell(String name)
        {
            this.name = name;
            content = new String("");
            visited = false;
        }

        public Cell(String name, Object content)
        {
            this.name = name;
            visited = false;
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

        public Object GetValue(){
            return this.value;
        }

    }
}
