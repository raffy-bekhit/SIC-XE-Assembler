using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIC_XE_Assembler
{
    class InputDirective:Directive
    {
        public string Operand { get; set; }
        public string ObjectCode { get; set; }
        public int Address{ get; set; }
        public ArrayList ExtRef { get; set; } = new ArrayList();

        public InputDirective(string mnemonic) : base(mnemonic)
        {
            Operand = "";
         
        }


        public void getExpression( string operand,Pass1 pass1)
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
            int i = 1;
            foreach (string s in tempexp)
            {


                expression = s.Split(new char[] { '-' });
                foreach (string s2 in expression)
                {

                    if (i == 1) { finexp.Add(s2); i++; continue; }
                    finexp.Add("-" + s2);


                }

            }
            i = 1;
            int totaddress = 0;
            

            foreach (string s in finexp)
            {
                s.Trim(' ');
                int tempaddress;
                if (int.TryParse(s, out tempaddress))
                {
                    totaddress   += Convert.ToInt32(tempaddress.ToString(), 16);
                }
                else if (s[0] == '+')
                {

                    if (pass1.TryGetValue2(s.Trim('+'), out tempaddress))
                    {
                        totaddress += tempaddress;

                    }
                    else
                    {
                        bool found = false;
                        foreach (string e in pass1.ExtRef)

                        {
                            
                            if (s.Trim('+').Equals(e))
                            {
                                found = true;
                                ExtRef.Add("+"+e);
                                
                            }
                        }
                        if (!found) Console.WriteLine("label not found at {0}", pass1.Locctr);
                    }


                }
                else if (s[0] == '-')
                {
                    if (pass1.TryGetValue2(s.Trim('-'), out tempaddress))
                    {
                        totaddress -= tempaddress;

                    }
                    else
                    {
                        bool found = false;
                        foreach (string e in pass1.ExtRef)

                        {
                            if (s.Trim('-').Equals(e))
                            {
                                found = true;
                                ExtRef.Add("-"+e);
                                
                            }
                        }
                        if (!found) Console.WriteLine("label not found at {0}", pass1.Locctr);
                    }
                }
                i++;
            }
            Operand = totaddress.ToString();
        
        }

        public bool CheckOperand(string operand,Pass1 pass1)
        {
            string Lpattern = @"\w+\s*";
            string Npattern = @"\d+\s*";
            string Cpattern = @"\s*[C][']\w+[']\s*";
            string Xpattern = @"\s*[X][']\w*\d*[']\s*";

            Regex rL = new Regex(Lpattern, RegexOptions.IgnoreCase);
            Match mL = rL.Match(operand);
            Regex rN = new Regex(Npattern, RegexOptions.IgnoreCase);
            Match mN = rN.Match(operand);
            Regex rC = new Regex(Cpattern, RegexOptions.IgnoreCase);
            Match mC = rC.Match(operand);
            Regex rX = new Regex(Xpattern, RegexOptions.IgnoreCase);
            Match mX = rX.Match(operand);

            operand = operand.Trim();

            if (String.IsNullOrEmpty(operand) && Mnemonic.Equals("NOBASE"))
            {
                return true;
            }



            
            else if (Mnemonic.Equals("BYTE"))
            {
                if (mC.Success && mC.Index == 0)
                {
                    Operand = operand.Trim(new char[] { ' ', 'C', '\'' });
                    OperandType = 2;
                    Length = Operand.Length;

                    return true;
                }

                else if (mX.Success && mX.Index == 0)
                {
                    Operand = operand.Trim(new char[] { ' ', 'X', '\'' });
                    OperandType = 1;
                    Length = Operand.Length / 2;
                    return true;
                }

            }


            else if (Mnemonic == "RESW")
            {
                Length = Convert.ToInt32(operand) * 3;
                Operand = null;
                return true;
            }
            else if (Mnemonic == "RESB")
            {
                Length = Convert.ToInt32(operand) * 1;
                Operand = null;
                return true;
            }
            
        

            else if (Mnemonic.Equals("WORD"))
            {
                int temp;
                if (int.TryParse(operand, out temp))
                    Operand = operand.Trim();
                else { getExpression(operand, pass1); }

                Length = 3;
                return true;
            }

           else if (mN.Success && mN.Index == 0)

            {
                if (Mnemonic.Equals("START"))
                {

                    {
                        pass1.StartingAddress = Convert.ToInt32(operand, 16);
                        return true;
                    }
                }
               else if (Mnemonic.Equals("END"))
                {
                    if (String.IsNullOrEmpty(operand))
                    { pass1.EndAddress = pass1.StartingAddress; return true; }

                    else
                    {
                        pass1.EndAddress = Convert.ToInt32(operand, 16);
                        return true;
                    }
                }
            }

              else  if (mL.Success&&mL.Index==0)
            {
                if (Mnemonic.Equals("END"))
                {
                    operand.Trim();
                    pass1.TryGetValue2(Operand, out pass1.EndAddress);
                    return true;
                }
                else if (Mnemonic.Equals("BASE")) { Operand = operand.Trim(); return true; }
                }

            else if (Mnemonic.Equals("ORG"))
            {
                return true;
            
           
            }

                return false;
        }

        public string CalculateObjectCode(int x)
        {
            string objectcode;
            
            if (x==1)
            {
              
                ObjectCode = String.Format("{0:X6}", Convert.ToInt32(Operand));
            }
           else if (x == 0)
            {
                if (OperandType == 1)
                {

                    if (Operand.Length%2!=0)
                    {

                        ObjectCode = "0" + Operand;
                    }
                    else { ObjectCode = Operand; }
                }
                else if (OperandType==2)
                {
                    byte[] bytes;
                    ObjectCode = "";
                    bytes = Encoding.ASCII.GetBytes(Operand);
                    foreach (byte b in bytes)

                        ObjectCode = ObjectCode + b.ToString();
                    //Operand = Convert.ToString(result, 16);
                   Operand= ObjectCode;
                }
            }
            
            return ObjectCode;

        }
    }
}
