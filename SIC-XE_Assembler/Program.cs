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
    class Program
    {
        public static string Error;
        public static List<string> ProgNames = new List<string>();
        public static int prognum = 0;
        static void Main(string[] args)
        {
            
            Processor.InitSet(); //initializes instruction set
            StreamReader reader = new StreamReader(@"C:\Users\lenovo\Desktop\sic2\SIC-XE_Assembler\control_section.txt");
           int  CSectNum = SplitCSects(reader);
            List<Pass1> pass1 = new List<Pass1>();
            List<Pass2> pass2 = new List<Pass2>();
            for (int i = 1; i <= CSectNum; i++)
            {
                string filename = String.Format("{0}.txt", i);
                reader = new StreamReader(filename);
                pass1.Add(new Pass1(reader));
            }

            foreach (Pass1 p1 in pass1)
            {
                pass2.Add(new Pass2(p1));
            }
            Console.ReadKey();
        }

        public static int SplitCSects(StreamReader reader)
        {
            string line;
            StreamWriter writer=new StreamWriter("1.txt");
            int i = 1;
            string pattern = @"(.)*(csect)(.)*";
            Regex r = new Regex(pattern,RegexOptions.IgnoreCase);
            do
            {
                line = "";
                line = reader.ReadLine();
                Match m = r.Match(line);
                if (m.Success && m.Index == 0)
                {
                    writer.Close();
                    i++;
                    string filename = String.Format("{0}.txt",i);
                    
                    writer = new StreamWriter(filename);
                ProgNames.Add(line.Split(' ').First().Trim(' '));
                    prognum++;

                }
                
                writer.WriteLine(line);

            } while (!reader.EndOfStream);
            writer.Close();
            return i;
        }
    }
}
