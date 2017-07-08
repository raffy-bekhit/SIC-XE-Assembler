using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;
namespace SIC_XE_Assembler
{
    static class Processor
    {
        public static List<Instruction> InstructionSet { get; set; } = new List<Instruction>();
        public static List<Register> RegisterSet { get; set; } = new List<Register>();
        public static List<Directive> Directives { get; set; } = new List<Directive>();
        
        public static void InitSet()
        {
            InstructionSet.Add(new Instruction("ADD", 3, 0x18));
            InstructionSet.Add(new Instruction("ADDF", 3, 0x58));
            InstructionSet.Add(new Instruction("ADDR", 2, 0x90));
            InstructionSet.Add(new Instruction("AND", 3, 0x40));
            InstructionSet.Add(new Instruction("CLEAR", 2, 0xb4));
            InstructionSet.Add(new Instruction("COMP", 3, 0x28));
            InstructionSet.Add(new Instruction("COMPF", 3, 0x88));
            InstructionSet.Add(new Instruction("COMPR", 2, 0xa0));
            InstructionSet.Add(new Instruction("DIV", 3, 0x24));
            InstructionSet.Add(new Instruction("DIVF", 3, 0x64));
            InstructionSet.Add(new Instruction("DIVR", 2, 0x9c));
            InstructionSet.Add(new Instruction("FIX", 1, 0xc4));
            InstructionSet.Add(new Instruction("FLOAT", 1, 0xc0));
            InstructionSet.Add(new Instruction("HIO", 1, 0xf4));
            InstructionSet.Add(new Instruction("J",3 ,0x3c ));
            InstructionSet.Add(new Instruction("JEQ",3 ,0x30 ));
            InstructionSet.Add(new Instruction("JGT",3 , 0x34));  
            InstructionSet.Add(new Instruction("JLT",3 , 0x38));
            InstructionSet.Add(new Instruction("JSUB",3 , 0x48));
            InstructionSet.Add(new Instruction("LDA",3 ,0x00 ));
            InstructionSet.Add(new Instruction("LDB",3 ,0x68 ));

            InstructionSet.Add(new Instruction("LDCH",3 ,0x50 ));
            InstructionSet.Add(new Instruction("LDF",3 ,0x70 ));
            InstructionSet.Add(new Instruction("LDL",3 ,0x08 ));
            InstructionSet.Add(new Instruction("LDS",3 ,0x6c ));
            InstructionSet.Add(new Instruction("LDT",3 ,0x74 ));
            InstructionSet.Add(new Instruction("LDX",3 ,0x04 ));
            InstructionSet.Add(new Instruction("LPS", 3, 0xd0));
            InstructionSet.Add(new Instruction("MUL",3 ,0x20 ));
            InstructionSet.Add(new Instruction("MULF",3 ,0x60 ));
            InstructionSet.Add(new Instruction("MULR",2 ,0x98 ));
            InstructionSet.Add(new Instruction("NORM",1 ,0xc8 ));
            InstructionSet.Add(new Instruction("OR",3 ,0x44 ));
            InstructionSet.Add(new Instruction("RD",3 ,0xd8 ));
            InstructionSet.Add(new Instruction("RMO", 2, 0xac));
            InstructionSet.Add(new Instruction("RSUB",3 ,0x4C ));
            InstructionSet.Add(new Instruction("SHIFTL",2 ,0xa4 ));
            InstructionSet.Add(new Instruction("SHIFTR",2 ,0x18 ));
            InstructionSet.Add(new Instruction("SIO",1 ,0xf0 ));
            InstructionSet.Add(new Instruction("SSK",3 ,0xec ));
            InstructionSet.Add(new Instruction("STA",3 ,0x0c ));
            InstructionSet.Add(new Instruction("STB",3 ,0x78 ));
            InstructionSet.Add(new Instruction("STCH",3 ,0x54 ));
            InstructionSet.Add(new Instruction("STF",3 ,0x80 ));
            InstructionSet.Add(new Instruction("STI",3 ,0xd4 ));
            InstructionSet.Add(new Instruction("STL",3 ,0x14 ));
            InstructionSet.Add(new Instruction("STS",3 ,0x7c ));
            InstructionSet.Add(new Instruction("STSW",3 ,0xe8 ));
            InstructionSet.Add(new Instruction("SIT",3 ,0x84 ));
            InstructionSet.Add(new Instruction("STX",3 ,0x10 ));
            InstructionSet.Add(new Instruction("SUB", 3,0x10 ));
            InstructionSet.Add(new Instruction("SUBF",3 ,0x5c ));
            InstructionSet.Add(new Instruction("SUBR",2 ,0x94 ));
            InstructionSet.Add(new Instruction("SVC",2 ,0xB0 ));
            InstructionSet.Add(new Instruction("TD",3 ,0xe0 ));
            InstructionSet.Add(new Instruction("TIO",1 ,0xf8 ));
            InstructionSet.Add(new Instruction("TIX",3 ,0x2c ));
            InstructionSet.Add(new Instruction("TIXR",2 ,0xb8 ));
            InstructionSet.Add(new Instruction("WD",3 ,0xdc ));
           



            RegisterSet.Add(new Register("A",0));
            RegisterSet.Add(new Register("X",1));
            RegisterSet.Add(new Register("L",2));
            RegisterSet.Add(new Register("B",3));
            RegisterSet.Add(new Register("S",4));
            RegisterSet.Add(new Register("T",5));
            RegisterSet.Add(new Register("F",6));
            RegisterSet.Add(new Register("PC",8));
            RegisterSet.Add(new Register("SW",9));

            Directives.Add(new Directive("START"));
            Directives.Add(new Directive("END"));
            Directives.Add(new Directive("BASE"));
            Directives.Add(new Directive("NOBASE"));
            Directives.Add(new Directive("RESW"));
            Directives.Add(new Directive("RESB"));
            Directives.Add(new Directive("BYTE"));
            Directives.Add(new Directive("WORD"));
            Directives.Add(new Directive("ORG"));
            Directives.Add(new Directive("EQU"));


        }



        public static List<string> ReadInstruction(string instruction)
        {
            List<string> LineArray = new List<string>();
            string Label = "";
            string Mnemonic = "";
            string Operands = "";
            string Comment = "";


            //check if comment
            if (isComment(instruction))
            {
                Comment = instruction;
                LineArray.Add(Label);
                LineArray.Add(Mnemonic);
                LineArray.Add(Operands);
                LineArray.Add(Comment);
                return LineArray;
            }

            //fill Label variable
            for (int i = 0; i < 9&&i<instruction.Length; i++)
            {
                Label += instruction[i];
            }
            

            //fill mnemonic field
            for (int i = 9; i < 17 && i < instruction.Length ; i++)
            {
                Mnemonic += instruction[i];
            }


            //fill operands string
            for (int i = 17; i < 35 && i < instruction.Length ; i++)
            { Operands += instruction[i]; }
            Operands = Operands.Trim();

            //fill comment string
            for (int i = 35; i < 66 && i < instruction.Length ; i++)
            {
                Comment += instruction[i];
            }

            LineArray.Add(Label);
            LineArray.Add(Mnemonic);
            LineArray.Add(Operands);
            LineArray.Add(Comment);
            return LineArray;
        }

        //checks if line is comment
        public static bool isComment(string instruction)
        {
            for (int i = 0; i < 8; i++)
            { if (instruction[i]=='.') return true; }

            return false;
        }
        public static InputInstruction mnemonicExists(string mnemonic)
        {
            int e = 0;

            string pattern4 = @"\s*[+](\w+)\s+";
            Regex r = new Regex(pattern4, RegexOptions.IgnoreCase);
            Match m = r.Match(mnemonic);
            if (m.Success && m.Index == 0)
            {
                mnemonic = mnemonic.Trim(new char[] { '+' });
                e = 1;

            }

            //to remove spaces and check if ther is spaces syntax error
            string pattern = @"\s*(\w+)\s+";
            r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m2 = r.Match(mnemonic);

            if (!m2.Success && m2.Index == 0)
            {
                Program.Error = "Error: mnemonic doesn't exist!";
                return null; //print error
            }

            mnemonic = mnemonic.Trim();
            foreach (Instruction inst in InstructionSet)
            {
                if (inst.Equals(mnemonic))
                {

                    InputInstruction inputinstruction = new InputInstruction(inst.Mnemonic, inst.Format, inst.OpCode);
                    inputinstruction.Format += e;
                    inputinstruction.e = e;
                    return inputinstruction;
                }
            }

            Program.Error = "Error: mnemonic doesn't exist!";
            return null;
        }


        

        public static InputDirective CheckDirective(string mnemonic )
        {
            string pattern = @"\s*\w+\s*";
            Regex r = new Regex(pattern,RegexOptions.IgnoreCase);
            Match m = r.Match(mnemonic);

            if(m.Success&&m.Index==0)
            {
                mnemonic = mnemonic.Trim();
                foreach(Directive directive in Processor.Directives)
                {
                    if(directive.Mnemonic.Equals(mnemonic))
                    {
                        InputDirective inputdirective =  new InputDirective(mnemonic);
                        return inputdirective;
                    }
                }

            }
            return null;

        }

    }
}

