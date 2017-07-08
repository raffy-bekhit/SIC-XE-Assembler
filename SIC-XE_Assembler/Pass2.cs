using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SIC_XE_Assembler
{
    class Pass2
    {
        int PC2 = 0;
        public static int number = 0;
        public Dictionary<int, string> ObjectCode = new Dictionary<int, string>();
        public  List<string> Mrecord { get; set; } = new List<string>();
        int h,j=0,x=0,k=0;
        int Lenght;
        string oc;
        bool newrec = true;
        List<string> Record = new List<string>(100);

        List<string> Copy = new List<string>(100);
        List<string> Trecord = new List<string>(100);


        //> objectCode = new List<string>();
        int recadd;
        string Hrecord, Erecord;
        public Pass2(Pass1 pass1)
        {
            Pass2.number++;
            
            int PC = pass1.StartingAddress;
            int beforePC = pass1.StartingAddress;
            Lenght = pass1.LastAddress - pass1.StartingAddress;
            InputInstruction inputinstruction;
            InputDirective inputdirective;
            //insert in loop

           
            
            for ( x=0; PC < pass1.LastAddress;x++)
            {
                if (newrec)
                {
                    recadd = PC;
                    Copy.Insert(k,"");
                    Record.Insert(k, "");
                    Trecord.Insert(k, "");
                    newrec = false;

                }
                
                //  if (x > 0) { Copy[x - 1] = Record[x - 1]; }
                //Trecord[x] = "T" + Convert.ToString(PC, 16);

                if (pass1.Directives.TryGetValue(PC, out inputdirective))
                {
                    if (inputdirective.Mnemonic.Equals("BASE"))
                    {
                        string operand = inputdirective.Operand;
                        int Base = GetFromSymTab(operand, pass1);
                        if (Base == -1)
                        {
                            Console.WriteLine("at instruction of ADdress{0} label not found", PC); break;
                        }

                        pass1.Base = Base;

                    }
                    else if (inputdirective.Mnemonic.Equals("NOBASE"))
                    {



                        pass1.Base = -1;

                    }

                }

                if (pass1.Instructions.TryGetValue(PC, out inputinstruction))
                {
                    if (inputinstruction.Format == 3)
                    {
                        PC2 = PC + 3;
                        int Ta;
                        string operand = inputinstruction.Operand1;
                        if (inputinstruction.OperandType == 1)
                        {
                            Ta = GetFromSymTab(operand, pass1);
                            if (Ta == -1)
                            {
                                if (pass1.ProgName.Trim(' ').Equals(operand.Trim(' '))) Ta = 0;
                                else
                                Console.WriteLine("at instruction of ADdress{0} label not found", PC); //break;
                            }
                            int Base = pass1.Base;
                            if ((Ta - PC2) >= -2048 && (Ta - PC2) <= 2047) { inputinstruction.Disp = Ta - PC2; inputinstruction.p = 1; inputinstruction.b = 0; }
                            else if ((Ta - Base) >= 0 && (Ta - Base) <= 4095 && Base != -1) { inputinstruction.Disp = Ta - Base; inputinstruction.p = 0; inputinstruction.b = 1; }
                            else
                            {
                                Console.WriteLine("Range of displacement is very hiph. at instruction of address{0}", PC); break;
                            }

                        }
                        else
                        {

                            inputinstruction.Disp = Convert.ToInt32(operand);

                        }




                        if (inputinstruction.Mnemonic.Equals("RSUB")) inputinstruction.Disp = 0;

                    }


                    else if (inputinstruction.Format == 4)
                    {
                        bool ModRecTypeFlag = false;
                        bool literalflag = false;
                        int Ta;
                        string operand = inputinstruction.Operand1;
                        if (inputinstruction.OperandType == 1)
                        {
                            Ta = GetFromSymTab(operand, pass1);


                            if (Ta == -1)
                            {
                                if (pass1.FindExtRef(operand)) { ModRecTypeFlag = true; inputinstruction.Disp = 0; }
                                else
                                    Console.WriteLine("at instruction of ADdress{0} label not found", PC); //break;
                            }
                            else { ModRecTypeFlag = false; inputinstruction.Disp = Ta; }


                        }

                        else
                        {
                            inputinstruction.Disp = Convert.ToInt32(operand);
                            literalflag = true;
                        }


                        h = PC;
                        h++;
                        if (!literalflag)
                            Mrecord.Add(String.Format("M{0:X6}05+{1}", h, ModRecTypeFlag ? operand : pass1.ProgName));



                    }
                    if (inputinstruction.Mnemonic.Equals("RSUB")) inputinstruction.Disp = 0;
                    oc = inputinstruction.CalculateObjectCode();
                    ObjectCode.Add(PC, oc);
                    beforePC = PC;
                    PC += inputinstruction.Format;

                }








                else if (pass1.Directives.TryGetValue(PC, out inputdirective))
                {
                    if (inputdirective.Mnemonic.Equals("RESW"))
                    {
                        newrec = true;
                        PC += inputdirective.Length;

                    }
                    else if (inputdirective.Mnemonic.Equals("RESB"))
                    {
                        newrec = true;
                        PC += inputdirective.Length;
                    }
                    else if (inputdirective.Mnemonic.Equals("BYTE"))
                    {
                        oc = inputdirective.CalculateObjectCode(0);
                        ObjectCode.Add(PC, oc);
                        PC += inputdirective.Length;
                    }
                    else if (inputdirective.Mnemonic.Equals("WORD"))
                    {
                        oc = inputdirective.CalculateObjectCode(1);
                        foreach(string s in inputdirective.ExtRef)
                        Mrecord.Add(String.Format("M{0:X6}06{1}", PC,s));
                        ObjectCode.Add(PC, oc);
                        PC += 3;
                    }
                }

                
                
                
                
               
                Copy[k]=Record.ElementAt(k);
            
                Record[k]=(Copy.ElementAt(k)+oc);

                if ((newrec == true || Record[k].Length > 60) && !String.IsNullOrEmpty(Copy[k]))
                {
                    Trecord.Insert(k, TrecFormat(Copy.ElementAt(k)));
                    newrec = true;
                    k++;

                }
                
               
             
            }
            try{
                if (!String.IsNullOrEmpty(Record[k]))
                {
                    Trecord.Insert(k, TrecFormat(Copy.ElementAt(k)));
                    newrec = true;
                    k++;

                } }
            catch(Exception ) {; }

            Hrecord = String.Format("H{0:g6}{1:X6}{2:X6}" ,pass1.ProgName,pass1.StartingAddress,Lenght);
            Erecord = String.Format("E{0:X6}" , pass1.EndAddress);

            string filename = String.Format("{0}.txt", pass1.ProgName.Trim());

            StreamWriter writer = (number == 1) ? (new StreamWriter("Pass2.txt",false)) : (new StreamWriter("Pass2.txt", true));
            writer.WriteLine();
            writer.WriteLine();

            writer.WriteLine(Hrecord);
            //writing D record
            List<string> DRec = WriteDRec(pass1);
           foreach (string s in DRec)
            {
                writer.WriteLine(s);
            }

            List<string> RRec = WriteRRec(pass1);
            foreach (string s in RRec)
            {
                writer.WriteLine(s);
            }

            
            foreach (string s in Trecord)
            {if(!String.IsNullOrEmpty(s))
                writer.WriteLine(s);
            }
            foreach (string s in Mrecord)
            {
                writer.WriteLine(s);
            }

            writer.WriteLine(Erecord);


            writer.Close();
        }

        public int GetFromSymTab(string label,Pass1 pass1) {
            int address;
            if (pass1.TryGetValue2(label, out address)|| pass1.TryGetValue2(label, out address))
            {
                return address;
            }
                else return -1;
        }
        public string TrecFormat(string rec)
        {
            
            return String.Format("T{0:X6}{1:X2}{2}",recadd,rec.Length/2,rec);
        }


        public List<string> WriteDRec(Pass1 pass1)
        {
            List<string> RecContent=new List<string>();
            int i = 0;
            string temp = "";
            foreach (var s in pass1.DefRef)
            {
                if (i == 0) {  temp = "D"; }
               
                temp=temp+ String.Format("{0:X6}{1}", s.Value, s.Key.PadRight(6,' '));
                i++;
                if (i > 6) { RecContent.Add(temp); i = 0; }
            }
            RecContent.Add(temp);
            return RecContent;
        }

        public List<string> WriteRRec(Pass1 pass1)
        {
            List<string> RecContent = new List<string>();
            int i = 0;
            string temp = "";
            foreach (string s in pass1.ExtRef)
            {
                if (i == 0) { temp = "R"; }

                temp = temp + String.Format("{0}", s.PadRight(6, ' '));
                i++;
                if (i > 12) { RecContent.Add(temp); i = 0; }
            }
            RecContent.Add(temp);
            return RecContent;
        }
    }
}
           
               

 
            

        