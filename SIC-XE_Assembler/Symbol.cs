using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_XE_Assembler
{
    class Symbol
    {
        public bool Flag { get; set; } // true for absolute
        public String Name { get; set; }

        public Symbol(String name, bool flag)
        {
            Name = name;
            Flag = flag;

        }

        public bool Equals(String name)
        {
            if (Name.Equals(name)) return true;
            return false;
        }
    }
}
