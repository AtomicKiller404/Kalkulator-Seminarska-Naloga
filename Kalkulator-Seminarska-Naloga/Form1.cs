using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using Microsoft.CSharp;

namespace Kalkulator_Seminarska_Naloga
{
    public partial class Form1 : Form
    {
        double result = 0;
        string operation = "";
        List<string> convert = new List<string>();
        bool execute = false;
        bool ex = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0" || execute == true)
                textBox1.Clear();
            textBox1.Text += (sender as Button).Text;
            execute = false;
            ex = true;
        }

        private void operator_click(object sender, EventArgs e)
        {
            try
            {
                if (result != 0)
                {
                    button18.PerformClick();
                    operation = (sender as Button).Text;
                    result = Double.Parse(textBox1.Text);
                    label1.Text = result + operation;
                    execute = true;
                    ex = true;
                }
                else
                {
                    operation = (sender as Button).Text;
                    result = Double.Parse(textBox1.Text);
                    label1.Text = result + operation;
                    execute = true;
                    ex = true;
                }
            }
            catch (Exception w)
            {
                label2.Text = "";
                label2.Text += w.Message;
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (ex)
            {
                if (operation != null)
                {
                    switch (operation)
                    {
                        case "+":
                            textBox1.Text = (result + Double.Parse(textBox1.Text)).ToString();
                            break;
                        case "-":
                            textBox1.Text = (result - Double.Parse(textBox1.Text)).ToString();
                            break;
                        case "x":
                            textBox1.Text = (result * Double.Parse(textBox1.Text)).ToString();
                            break;
                        case "/":
                            textBox1.Text = (result / Double.Parse(textBox1.Text)).ToString();
                            break;
                        default:
                            break;
                    }
                    label1.Text = "";
                    result = 0;
                    execute = false;
                    ex = false;
                }
                else
                {
                    switch (convert[0])
                    {
                        case "DEC":
                            foreach(var i in DEC_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        case "BIN":
                            foreach (var i in BIN_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        case "OCT":
                            foreach (var i in OCT_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        case "HEX":
                            foreach (var i in HEX_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        default:
                            break;
                    }
                    convert.Clear();
                    label1.Text = "";
                    result = 0;
                    execute = false;
                    ex = false;
                }
            }
            else
            {
                label1.Text = "";
                if (textBox1.Text.Last() == '=')
                {
                    double evaluate = Evaluate(textBox1.Text.Substring(0, textBox1.Text.Length - 1));
                    textBox1.Text = evaluate.ToString();
                }
                else
                {
                    double evaluate = Evaluate(textBox1.Text);
                    textBox1.Text = evaluate.ToString();
                }
            }
        }

        private List<int> DEC_convert(string second)
        {
            List<int> bin = new List<int>();
            List<int> Rconvert = new List<int>();
            string txt = textBox1.Text.Substring(3, textBox1.Text.Length-6);
            int res = Convert.ToInt32(txt);

            bin = To_BIN(res);
            
            textBox1.Clear();
            if(second == "BIN")
            {
                foreach (var i in bin)
                {
                    Rconvert.Add(i);
                }
            }
            else if (second == "OCT")
            {
                Rconvert = BIN_convert(3, bin);
            }
            else if (second == "HEX")
            {
                Rconvert = BIN_convert(4, bin);
            }

            return Rconvert;
        }

        private List<int> BIN_convert(string second)
        {
            List<int> bin = new List<int>();
            List<int> Rconvert = new List<int>();
            string txt = textBox1.Text.Substring(3, textBox1.Text.Length - 6);
            int res = 0;

            textBox1.Clear();
            if (second == "DEC")
            {
                int index = txt.Length - 1;
                foreach (char i in txt)
                {
                    res = res + (i - '0') * (int)Math.Pow(2, index);
                    index--;
                }
                string s = res.ToString();
                foreach(char i in s.Reverse())
                {
                    Rconvert.Add(i - '0');
                }

            }
            else
            {
                foreach (char i in txt)
                {
                    bin.Add(i - '0');
                }
                List<int> Rbin = new List<int>(bin.AsEnumerable().Reverse());
                if (second == "OCT")
                {
                    Rconvert = BIN_convert(3, Rbin);
                }
                else if (second == "HEX")
                {
                    Rconvert = BIN_convert(4, Rbin);
                }
            }

            

            return Rconvert;
        }

        private List<int> OCT_convert(string second)
        {
            List<int> bin = new List<int>();
            List<int> Rconvert = new List<int>();
            string txt = textBox1.Text.Substring(3, textBox1.Text.Length - 6);
            int res = 0;

            textBox1.Clear();

            int index = txt.Length - 1;
            foreach (char i in txt)
            {
                res = res + (i - '0') * (int)Math.Pow(8, index);
                index--;
            }
            string s = res.ToString();

            foreach (char i in s.Reverse())
            {
                Rconvert.Add(i - '0');
            }

            if (second == "DEC")
            {
                return Rconvert;

            }

            bin = To_BIN(res);
            Rconvert = To_BIN(res);

            if (second == "HEX")
            {
                Rconvert = BIN_convert(4, bin);
            }

            return Rconvert;
        }

        private List<int> HEX_convert(string second)
        {
            List<int> bin = new List<int>();
            List<int> Rconvert = new List<int>();
            string txt = textBox1.Text.Substring(3, textBox1.Text.Length - 6);
            int res = 0;

            textBox1.Clear();

            int index = txt.Length - 1;
            foreach (char i in txt)
            {
                res = res + (i - '0') * (int)Math.Pow(16, index);
                index--;
            }
            string s = res.ToString();

            foreach (char i in s.Reverse())
            {
                Rconvert.Add(i - '0');
            }

            if (second == "DEC")
            {
                return Rconvert;

            }

            bin = To_BIN(res);
            Rconvert = To_BIN(res);

            if (second == "OCT")
            {
                Rconvert = BIN_convert(3, bin);
            }


            return Rconvert;
        }


        private List<int> BIN_convert(int n, System.Collections.Generic.List<int> bin)
        {
            List<int> Rconvert = new List<int>();

            while (bin.Count % n != 0)
            {
                bin.Add(0);
            }
            //bin.Reverse();
            for (int i = 0; i < bin.Count / n; i++)
            {
                int r = 0;
                for (int j = i * n; j < i * n + n; j++)
                {
                    //4 2 1
                    //0 0 0
                    //Prvi bo bil tezisca 4
                    if (bin[j] == 1)
                    {
                        for(int k = 0; k<n; k++)
                        {
                            if (j % n == k)
                            {
                                r = (int)(r + Math.Pow(2,k));
                            }
                        }
                    }
                }
                Rconvert.Add(r);
            }

            return Rconvert;
        }

        private List<int> To_BIN(int n)
        {
            List<int> bin = new List<int>();
            int res = n;

            while (res != 0)
            {
                bin.Add(res % 2);
                res = res / 2;
            }

            return bin;
        }

        private void buttonCE_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            label2.Text = "";
            convert.Clear();

        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            result = 0;
            convert.Clear();
            label2.Text = "";
        }

        private void backSpace_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);

            label2.Text = "";
        }

        private static double Evaluate(string expression)
        {
            // create a C# code provider
            CSharpCodeProvider provider = new CSharpCodeProvider();

            // define the sqrt and pow functions
            string code = @"
                using System;
                namespace Calculator
                {
                    public static class Evaluator
                    {
                        public static double sqrt(double x)
                        {
                            return Math.Sqrt(x);
                        }

                        public static double pow(double x, double y)
                        {
                            return Math.Pow(x, y);
                        }

                        public static double Evaluate()
                        {
                            return " + expression + @";
                        }
                    }
                }
            ";

            // compile the code into an assembly
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

            // check for errors
            if (results.Errors.Count > 0)
            {
                throw new Exception("Error evaluating expression");
            }

            // get the type and method from the compiled assembly
            Type type = results.CompiledAssembly.GetType("Calculator.Evaluator");
            System.Reflection.MethodInfo method = type.GetMethod("Evaluate");

            // invoke the method and return the result
            return (double)method.Invoke(null, null);
        }

        private void buttonFile_Click(object sender, EventArgs e)
        {
            // specify the file path
            string filePath = "C:\\Users\\mihap\\OneDrive\\Dokumenti\\School\\Programiranje\\Kalkulator-Seminarska-Naloga\\Kalkulator-Seminarska-Naloga\\Primer datoteke za računanje števil.txt";

            // open the file using a StreamReader
            using (StreamReader reader = new StreamReader(filePath))
            {
                // read the file line by line
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    textBox1.Text = line;
                    label1.Text += " ";
                    if (textBox1.Text.Last() == '=')
                        label1.Text += Evaluate(textBox1.Text.Substring(0, textBox1.Text.Length - 1));
                    else
                        label1.Text += Evaluate(textBox1.Text);
                }
            }
        }

        private void convert_click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0" || execute == true)
                textBox1.Clear();
            textBox1.Text += (sender as Button).Text;
            execute = false;
            ex = true;
            operation = null;
            convert.Add((sender as Button).Text);
        }
    }
}