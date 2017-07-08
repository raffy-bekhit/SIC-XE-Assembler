using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIC_XE_Assembler
{
    class InputInstruction : Instruction
    {
        public int n { get; set; }
        public int i { get; set; }
        public int x { get; set; }
        public int b { get; set; }
        public int p { get; set; }
        public int e { get; set; }
        public int Disp { get; set; }
        public string Operand1 { get; set; }
        public string Operand2 { get; set; }
        public int Address { get; set; }
        public string ObjectCode { get; set; }
        public int OperandType { get; set; } //0 for number , 1 fo label
        public int Format4MType; // 0 for progname & 1 for extref

        public InputInstruction(string mnemonic, int format, int opcode) : base(mnemonic, format, opcode)
        {
            
            Operand1 = null;
            Operand2 = "0";

        }


        public bool checkOperand(string operands)
        {


            if (Mnemonic == "RSUB")
            {
                if (operands.Trim().Equals("") == false)
                {
                    
                   
                }
                n = i = 0;x = 0;

                return true;
                   

            }

            if (Format == 1 && operands.Trim() == "")
            {
                return true; //print warning if ther is operand 
            }

            

                string nPattern = @"[@]\w+\s*";
                string inumberPattern = @"[#]\d+\s*";
                string ilabelPattern = @"[#]\w+\s*";
                string xPattern = @"\w+\s*[,]\s*[x]";
                string regPattern = @"\w+\s*[,]\s*(\w+|\d+)";


                string sPattern = @"\w+\s*";


                Regex r1 = new Regex(nPattern, RegexOptions.IgnoreCase);
                Match m1 = r1.Match(operands);

                Regex r2 = new Regex(inumberPattern, RegexOptions.IgnoreCase);
                Match m2 = r2.Match(operands);

                Regex r3 = new Regex(ilabelPattern, RegexOptions.IgnoreCase);
                Match m3 = r3.Match(operands);

                Regex r4 = new Regex(xPattern, RegexOptions.IgnoreCase);
                Match m4 = r4.Match(operands);

                Regex r5 = new Regex(regPattern, RegexOptions.IgnoreCase);
                Match m5 = r5.Match(operands);

                Regex r6 = new Regex(sPattern, RegexOptions.IgnoreCase);
                Match m6 = r6.Match(operands);

                if (m1.Success && m1.Index == 0)
                {
                    if (Format < 3) { Console.Write("Error in Operand"); return false ; }
                    operands = operands.Trim(new char[] { '@' , ' ' });
                Operand1 = operands;
                OperandType = 1;
                    n = 1; i = 0; x = 0;

                }

                else if (m2.Success && m2.Index == 0)
                {
                    if (Format < 3) { Console.Write("Error in Operand");  return false;}
                    operands = operands.Trim(new char[] { '#', ' ' });
                    Operand1=(operands);
                OperandType = 0;
                    i = 1;
                    n = 0;
                    x = 0;
                }

                else if (m3.Success && m3.Index == 0)
                {
                    if (Format == 1) { Program.Error = "Error in Operand"; return false; }
                    operands = operands.Trim(new char[] { '#', ' ' });
                Operand1 = operands;
                OperandType = 1;
                i = 1;
                    n = 0;
                    x = 0;
                }

                else if (m4.Success && m4.Index == 0&&Format>2)
                {
                    //if (Format < 2) { Program.Error="Error in Operand"; return false ; }
                    operands = operands.Split(new char[] { ',' }).First();
                OperandType = 1;
                    x = 1;
                    n = i = 1;
                Operand1 = operands;

                }

            else if (m5.Success && m5.Index == 0 && Format == 2)
            {
                //if (Format != 2) { Program.Error = "Error in Operand"; return false; }
                string operand1 = operands.Split(new char[] { ',' }).First();

                Operand1 = operand1.Trim();
                string operand2 = operands.Split(new char[] { ',' }).Last();
                Operand2 = operand2.Trim();
                foreach (Register r in Processor.RegisterSet)
                {
                    if (Operand1 == r.Name) Operand1 = r.Number.ToString();
                    if (Operand2 == r.Name) Operand2 = r.Number.ToString();
                }

                x = 0;
                n = i = 1;


            }

            else if (m6.Success && m6.Index == 0)
                {
                if (Format == 2) {
                    operands.Trim();
                    Operand1 = operands;
                    OperandType = 1;
                    foreach (Register r in Processor.RegisterSet)
                    {
                        if (Operand1 == r.Name) Operand1 = r.Number.ToString();
                        
                    }
                    Operand2 = "0";
                    
                }
                if (Format == 3 || Format == 4)
                {
                    operands.Trim();
                    Operand1 = operands;
                    OperandType = 1;
                    n = 1;
                    i = 1;
                    x = 0;
                }

                }
            
            return true;
            }
   public string CalculateObjectCode()
    {
            string  objectcode="";
            if (Format == 3) {
                
                int byte1 = ((OpCode>>2)<<2) + (n<<1) + i;
                int byte2 = (x << 7) + (b << 6) + (p << 5) + (e << 4) +( Disp >> 8);
                    int byte3= (Disp<<4)>>4;
                objectcode = String.Format("{0:X2}{1:X2}{2:X2}",byte1,byte2,byte3);
            }
            if (Format == 4)
            {

                int byte1 = ((OpCode >> 2) << 2) + (n << 1) + i;
                int byte2 = (x << 7) + (b << 6) + (p << 5) + (e << 4) + (Disp >> 16);
                int byte3 = (Disp<<4)>>4;
                objectcode = String.Format("{0:X2}{1:X2}{2:X4}", byte1, byte2, byte3);
            }

            if (Format==1)
            { objectcode = String.Format("{0:X2}", OpCode); }
            if(Format==2) {
               
              
                Disp = (Convert.ToInt32(Operand1) << 4) + Convert.ToInt32(Operand2);
                objectcode = String.Format("{0:X4}",(OpCode << 7) + Disp);  }

            ObjectCode = objectcode;
            return ObjectCode;
            
    }
        }
    }