using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_XE_Assembler
{
    class Directive
    {
       public string Mnemonic { get; set; }
        public int Length { get; set; }
        
        public int OperandType { get; set; }
        //public int Type { get; set; }

        public Directive(string mnemonic)
        {
            Mnemonic = mnemonic;
            
         

        }
        public  bool Equals(string obj)
        {
            return Mnemonic.Equals(obj);
        }
    }
}
