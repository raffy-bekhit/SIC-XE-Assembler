using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIC_XE_Assembler
{
    class Instruction
    {
        public string Mnemonic { get; set; }
        public int Format { get; set; }
        public int OpCode { get; set; }

        public Instruction(string mnemonic, int format, int opcode)
        {
            Mnemonic = mnemonic;
            Format = format;
            OpCode = opcode;
        }

        public override bool Equals(object mnemonic)
        {
            if (mnemonic.Equals(Mnemonic)) return true;
            else return false;
        }
    }
}
