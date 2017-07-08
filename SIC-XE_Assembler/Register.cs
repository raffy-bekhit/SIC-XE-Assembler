using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_XE_Assembler
{
    class Register
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public Register(string name, int number)
        {
            Name = name;
            Name = name;
        }
        public  bool Equals(string name)
        {
            return Name.Equals(name);

        }
        public  int RegtoNumber() {
            return Number;
        }
    }

    
}
