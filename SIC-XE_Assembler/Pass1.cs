using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIC_XE_Assembler
{
    class Pass1
    {
        
        public  string ProgName = "";
       
        public  int LastAddress;
        public  int Base { get; set; }


        public  int StartingAddress=0, EndAddress=0;
        public  Dictionary<Symbol, int> Symtab = new Dictionary<Symbol, int>();  //symbol table

        public  Dictionary<int, string> Comments = new Dictionary<int, string>();  // to put comments in
        List<string> TempDefRef = new List<string>();
        public  Dictionary<int, InputInstruction> Instructions = new Dictionary<int, InputInstruction>();   // to put read Instructions in

        public  Dictionary<int, InputDirective> Directives = new Dictionary<int, InputDirective>();

        StreamWriter writer, writer2 ;
            
        string[] lines;
       public Dictionary<string, int> DefRef = new Dictionary<string, int>();
        public List<string> ExtRef = new List<string>();
        int lct;
       public int Locctr;
        public static int number=0;
        public Pass1(StreamReader reader)
        {

            
                foreach (string s in Program.ProgNames)
                {
                    
                        ExtRef.Add(s);
                }
           

            number++;
             Locctr = StartingAddress;       //location counter
           
            string Label = "", Mnemonic = "", Comment = "", Operands = "";   // temporary variables to read in from stream

           
            
            lines = new string[200];
            lines[0] = string.Format("{0} {1} {2} {3} {4} {5} ", "lineNumber".PadRight(12), "Address".PadRight(15), "Label".PadRight(9), "Mnemonic".PadRight(9), "Operands".PadRight(20), "Comments".PadRight(40));


            char c; //used as Buffer
            char[] InstructionString2 = new char[66];
            string InstructionString="";
            int i, j = 0,k=0;
            Base = -1;

            do
            {

                k = 0;
                

                i = 0; // to indicate the column
                lct = Locctr;

                InstructionString = reader.ReadLine();
                while (i < InstructionString.Length)
                {

                    c = InstructionString[i];
                  if (c != '\n' && c != '\r' &&c!='\t'&& c != '\0')      {
                            c = Char.ToUpper(c);
                            
                                InstructionString2[k] = c;k++;
                        } i++;
                    }
                InstructionString = new string(InstructionString2,0,i);





               
                if (String.IsNullOrEmpty(InstructionString)) goto jinc;
                //instruction string trimming
                List<string> InstructionParam = Processor.ReadInstruction(InstructionString);
                Label = InstructionParam[0];
                Mnemonic = InstructionParam[1];
                Operands = InstructionParam[2];
                Comment = InstructionParam[3];
                lines[j + 1] = LineFormat(j + 1, Locctr, Label, Mnemonic, Operands, Comment);

               

                if (Processor.isComment(InstructionString)) goto jinc;



                if (Mnemonic.Trim(' ').Equals("EQU")|| Mnemonic.Trim(' ').Equals("ORG")) { getExpression(Label, Operands,Mnemonic.Trim(' ')); goto jinc; }
                if (Mnemonic.Trim(' ').Equals("CSECT")) { ProgName = Label.Trim(' '); goto jinc; }
                if (Mnemonic.Trim(' ').Equals("EXTREF")) {SaveRefsInList(Operands,ExtRef) ; goto jinc; }
                if (Mnemonic.Trim(' ').Equals("EXTDEF")) { SaveRefsInList(Operands, TempDefRef); goto jinc; }



                if (!CheckLabel(Label, Locctr,Mnemonic)) break;

                //create instance of instruction read
                InputInstruction inputinstruction = Processor.mnemonicExists(Mnemonic);
                InputDirective inputdirective = Processor.CheckDirective(Mnemonic);
                //adds operands to inpuinstruction 
                if (inputinstruction != null && inputinstruction.checkOperand(Operands))
                {
                    inputinstruction.Address = Locctr;
                    Instructions.Add(Locctr, inputinstruction);
                    Locctr += inputinstruction.Format;
                    
                }


                else if (inputdirective != null && inputdirective.CheckOperand(Operands,this))
                {
                   Program.Error = "";
                    if (inputdirective.Mnemonic == "BASE")
                    {
                        InputInstruction lastinst = Instructions.LastOrDefault().Value;
                        if (lastinst!=null)
                            
                        {
                            int key = Locctr - (Instructions.LastOrDefault().Value.Format);
                            inputdirective.Address = key;
                            Directives.Add(key, inputdirective);
                        }

                    }
                    else
                    {
                        inputdirective.Address = Locctr;
                        Directives.Add(Locctr, inputdirective);
                        Locctr += inputdirective.Length;
                    }
                  

                }



                InstructionString = "";
                InstructionString2 = new char[66];
                

            jinc: j++;
              if (!string.IsNullOrEmpty(Program.Error)) { lines[j + 1] = Program.Error; break; }
               if(lct>LastAddress) LastAddress = lct;
            } while (!reader.EndOfStream);

            //string pass1filename = String.Format("Pass1_{0}.txt", ProgName);
            writer = (number==1)?(
                new StreamWriter("Pass1.txt",false)): 
                (new StreamWriter("Pass1.txt", true));

            

            //add DefRefs in Dictionary
            foreach (string s in TempDefRef)
            {
                int address;
                TryGetValue2(s, out address);
                DefRef.Add(s, address);
            }
            

            Console.WriteLine(Program.Error);
            
            //checks errors for equ and writes symtab in file
            string symtabfilename = String.Format("SymTab_{0}.txt",ProgName.Trim());
            writer2 = (number == 1) ? (new StreamWriter("SymTab.txt")) : (new StreamWriter("SymTab.txt", true));
            if (number == 1)writer2.WriteLine("CSName".PadRight(40, ' ')+"Symbol".PadRight(40,' ')+"address".PadRight(10,' ') +"Type".PadRight(10, ' '));
            writer2.WriteLine(ProgName.PadRight(40, ' '));

            foreach (var a in Symtab)
            {
                
                //if (!(a.Value >= StartingAddress && a.Value <= LastAddress))
                 //  Console.WriteLine("Error in Symbol in symtab");

                writer2.WriteLine("".PadRight(40, ' ')+a.Key.Name.PadRight(40,' ')+a.Value.ToString().PadRight(10,' ')+(a.Key.Flag?"Absolute":"Relative").PadRight(10,' '));


            }
            writer2.Close();

            foreach (string line in lines)
            {

                if(!String.IsNullOrEmpty(line))
                writer.WriteLine(line);


            }
            writer.WriteLine(); writer.WriteLine();
            writer.Close();

            foreach (string s in Program.ProgNames)
            {
                ExtRef.Remove(s);
            }

        }

        //calculates expressions for equ 
        public void getExpression(string label, string operand, string mnemonic)
        {
            string[] expression;
            ArrayList tempexp = new ArrayList();
            ArrayList finexp = new ArrayList();
            List<int> expInNum = new List<int>();
            expression = operand.Split(new char[] { '+' });
            foreach (string s in expression)
            {
                tempexp.Add("+" + s);

            }
            expression = null;
            int i;
            foreach (string s in tempexp)
            {
                i = 1;


                expression = s.Split(new char[] { '-' });
                foreach (string s2 in expression)
                {

                    if (i == 1) { finexp.Add(s2); i++; continue; }
                    finexp.Add("-" + s2);


                }

            }
            i = 1;
            int totaddress = 0;
            int plus = 0, minus = 0;
            List<string> temp = new List<string>();
            foreach (string s in finexp)
            {
                
                s.Trim(' ');
                int tempaddress;
                if (int.TryParse(s, out tempaddress))
                    totaddress += tempaddress;
                else if (s[0] == '-')
                {
                    if (TryGetValue2(s.Trim('-'), out tempaddress))
                    {

                        totaddress -= tempaddress;
                        minus++;
                        temp.Add(s);
                    }
                }
            }
            foreach(string s in temp) { finexp.Remove(s); }

            foreach (string s in finexp) { 
                if (s[0] == '+')
                {
                    int tempaddress;
                    if (TryGetValue2(s.Trim('+'), out tempaddress))
                    {
                        totaddress += tempaddress;
                        plus++;
                    }
                }
                
                i++;
            }

            //rage3 3l condition // // // // // // <--------------------------------
            if (mnemonic.Equals("EQU"))
            {
                if (plus == minus)
                {
                    Symtab.Add(new Symbol(label, true), totaddress);
                }
                else Symtab.Add(new Symbol(label, false), totaddress);
            }
            else if (mnemonic.Equals("ORG")) Locctr = totaddress;
        }


        public bool CheckLabel(string label, int locctr, string mnemonic)
        {
            if (mnemonic.Trim().Equals("START")) { ProgName = label; return true; }
            string pattern = @"\s*\w+\d*\s*";
            string pattern2 = @"\s*";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(label);
            if (m.Success && m.Index == 0)
            {
                label = label.Trim();
                foreach (Symbol symbol in Symtab.Keys)
                {
                    if (label.Equals(symbol.Name))
                    {
                        Program.Error = "Error: Label exists.";
                        return false;
                    }
                }
                if (!mnemonic.Equals("EQU"))
                {
                    Symtab.Add(new Symbol(label, false), locctr);
                    return true;
                }
            }

            r = new Regex(pattern2, RegexOptions.IgnoreCase);
            m = r.Match(label);
            if (m.Success && m.Index == 0)
            {
                return true;
            }
            Program.Error = "Error: Label isn't valid.";
            return false;

        }

        public  bool TryGetValue2(string symbol, out int value)
        {
            foreach (var s in Symtab)
            {
                if (s.Key.Equals(symbol)) { value = s.Value; return true; }

            }
            value = 0;
            return false;
        }

        public  string LineFormat(int line, int address, string label , string mnemonic, string operands,string comments)
    {

            
                string format = string.Format("{0} {1:X} {2} {3} {4} {5} ", line.ToString().PadRight(12), Convert.ToString(address,16).PadRight(15), label.PadRight(9), mnemonic.PadRight(9), operands.PadRight(20), comments.PadRight(40));



            return format;
        }

        public void SaveRefsInList(string operands,List<string> list)
        {
            operands.Trim(' ');
            string[] extrefs = operands.Split(',');
            foreach (string s in extrefs)
            {
                list.Add(s);
            }
        }
        public bool FindExtRef(string extref)
        {
            foreach(string s in ExtRef)
            {
                if (s.Equals(extref)) return true;
            }
            return false;
        }

        public void SaveDefRefs(string operands)

        {
            operands.Trim(' ');
            string[] extrefs = operands.Split(',');
            foreach (string s in extrefs)
            {

            }
        }

    }

   
}