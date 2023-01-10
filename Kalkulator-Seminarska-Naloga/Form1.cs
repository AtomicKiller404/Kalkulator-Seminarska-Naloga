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
using static System.Windows.Forms.LinkLabel;
using System.Threading;
using System.CodeDom;
using System.Text.RegularExpressions;

namespace Kalkulator_Seminarska_Naloga
{
    public partial class Form1 : Form
    {
        double result = 0;
        string operation = ""; // Operation like + - / *

        List<string> convert = new List<string>(); // List for opeartions like DEC BIN ...
        bool con = false;

        bool execute = false;
        bool ex = false; // Checks if equation was typed in or was it clicked in

        bool quit = false; // Check if still reading from file

        private ManualResetEvent buttonClickedEvent = new ManualResetEvent(false);

        List<string> lines = new List<string>();
        int conv = 0;

        List<string> logic_operators = new List<string>();
        List<string> logic_functions = new List<string>();
        int index = 0;

        public Form1()
        {
            InitializeComponent();
            backwards.Visible = false;
            forward.Visible = false;
        }

        private void btn_click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0" || execute == true)
                textBox1.Clear();
            textBox1.Text += (sender as Button).Text; // Adds a number of button in text box
            execute = false; // Checks out that its not an operation
            ex = true; // Checks out that it was clicked in
        }

        bool first = true;
        private void zeroOne_click(object sender, EventArgs e)
        {
            if (execute == true || ((sender as Button).Text == "0" && textBox1.Text == "0") || ((sender as Button).Text == "1" && textBox1.Text == "0" && first))
            {
                textBox1.Clear();
                first = false;
            }
            textBox1.Text += (sender as Button).Text; // Adds a number of button in text box
            execute = false; // Checks out that its not an operation
            ex = true; // Checks out that it was clicked in
        }

        private void logicgt_click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0" || execute == true)
                textBox1.Clear();

            textBox1.Text += (sender as Button).Text.Substring((sender as Button).Text.IndexOf("(") + 1, 
                ((sender as Button).Text.Length - (sender as Button).Text.IndexOf("(")) - 2); // Adds a logic operator in text box
            logic_operators.Add((sender as Button).Text.Substring(0, (sender as Button).Text.IndexOf("(") - 1)); // Adds logic operator in list
            execute = false; // Checks out that its not an operation
            ex = true; // Checks out that it was clicked in
        }

        private void operator_click(object sender, EventArgs e)
        {
            try
            {
                if (result != 0) // If it isnt the first time clicing in
                {
                    button18.PerformClick(); // Run button18_Click() function
                    operation = (sender as Button).Text; // Remember the next operation
                    result = Double.Parse(textBox1.Text);
                    label1.Text = result + operation; // Displayes the new result
                    execute = true; // Checks out that the operator was clicked in
                    ex = true; // Checl out that it was clicked in
                }
                else // If it is the first time
                {
                    // The same as above minus the calling of the function
                    operation = (sender as Button).Text;
                    result = Double.Parse(textBox1.Text);
                    label1.Text = result + operation;
                    execute = true;
                    ex = true;
                }
            }
            catch (Exception w)
            {
                // If something went wrong throws an exception
                string message = w.Message;
                string title = "Warning";
                MessageBox.Show(message, title);
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (ex) // If it was clicked in 
            {
                if (operation != null) // If it is an equation
                {
                    switch (operation)
                    {
                        case "+": // Adds up the numbers
                            textBox1.Text = (result + Double.Parse(textBox1.Text)).ToString();
                            break;
                        case "-": // Subtracts the numbers
                            textBox1.Text = (result - Double.Parse(textBox1.Text)).ToString();
                            break;
                        case "x": // Multiplys the numbers
                            textBox1.Text = (result * Double.Parse(textBox1.Text)).ToString();
                            break;
                        case "/": // Divine the numbers
                            textBox1.Text = (result / Double.Parse(textBox1.Text)).ToString();
                            break;
                        default:
                            break;
                    }

                    // Resets variables
                    label1.Text = "";
                    result = 0;
                    execute = false;
                    ex = false;
                }
                else // If it is an convertion
                {
                    try
                    {
                        switch (convert[0])
                        {
                            case "DEC": // If the number that is beeing converted is DEC
                                foreach (var i in DEC_convert(convert[1]).AsEnumerable().Reverse())
                                {
                                    textBox1.Text += i.ToString();
                                }
                                break;
                            case "BIN": // If the number that is beeing converted is BIN
                                foreach (var i in BIN_convert(convert[1]).AsEnumerable().Reverse())
                                {
                                    textBox1.Text += i.ToString();
                                }
                                break;
                            case "OCT": // If the number that is beeing converted is OCT
                                foreach (var i in OCT_convert(convert[1]).AsEnumerable().Reverse())
                                {
                                    textBox1.Text += i.ToString();
                                }
                                break;
                            case "HEX": // If the number that is beeing converted is HEX
                                foreach (var i in HEX_convert(convert[1]).AsEnumerable().Reverse())
                                {
                                    textBox1.Text += i.ToString();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch(Exception w)
                    {
                        // If something went wrong throws an exception
                        string message = w.Message;
                        string title = "Warning";
                        MessageBox.Show(message, title);
                    }
                    // Resets variables
                    convert.Clear();
                    label1.Text = "";
                    result = 0;
                    execute = false;
                    ex = false;
                    buttonA.Visible = false;
                    buttonB.Visible = false;
                    buttonCL.Visible = false;
                    buttonD.Visible = false;
                    buttonE.Visible = false;
                    buttonF.Visible = false;
                }
            }
            else // If the equation was typed in
            {
                if (!con)
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
                else if(con)
                {
                    switch (convert[0])
                    {
                        case "DEC": // If the number that is beeing converted is DEC
                            foreach (var i in DEC_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        case "BIN": // If the number that is beeing converted is BIN
                            foreach (var i in BIN_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        case "OCT": // If the number that is beeing converted is OCT
                            foreach (var i in OCT_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        case "HEX": // If the number that is beeing converted is HEX
                            foreach (var i in HEX_convert(convert[1]).AsEnumerable().Reverse())
                            {
                                textBox1.Text += i.ToString();
                            }
                            break;
                        default:
                            break;
                    }
                    // Resets variables
                    convert.Clear();
                    result = 0;
                    execute = false;
                    ex = false;
                    con = false;
                }
            }
        }

        private List<string> DEC_convert(string second)
        {
            List<int> bin = new List<int>(); // List for bin numbers
            List<string> Rconvert = new List<string>(); // List for the complete
            string txt;
            if (!con)
                txt = textBox1.Text.Substring(4, textBox1.Text.Length - 8); // Get the number for conversion "Substring(where it starts, how long)"
            else
                txt = label1.Text.Substring(4, label1.Text.Length - 10); // Get the number for conversion "Substring(where it starts, how long)"
            int res = Convert.ToInt32(txt); // Converte that number into int

            bin = To_BIN(res);
            
            textBox1.Clear(); // Clear text box
            if(second == "BIN") // If the DEC is converted to BIN
            {
                foreach (var i in bin)
                {
                    Rconvert.Add(i.ToString()); // Copys the numbers into Rconvert
                }
            }
            else if (second == "OCT") // If the DEC is converted to OCT
            {
                Rconvert = BIN_convert(3, bin);
            }
            else if (second == "HEX") // If the DEC is converted to HEX
            {
                Rconvert = BIN_convert(4, bin);
            }

            return Rconvert;
        }

        private List<string> BIN_convert(string second)
        {
            List<int> bin = new List<int>(); // List for bin numbers
            List<string> Rconvert = new List<string>(); // List for the complete
            string txt;
            if (!con)
                txt = textBox1.Text.Substring(4, textBox1.Text.Length - 8); // Get the number for conversion "Substring(where it starts, how long)"
            else
                txt = label1.Text.Substring(4, label1.Text.Length - 10); // Get the number for conversion "Substring(where it starts, how long)"
            int res = 0;

            textBox1.Clear(); // Clears text box
            if (second == "DEC") // If the BIN is converted to DEC
            {
                int index = txt.Length - 1;
                foreach (char i in txt)
                {
                    res = res + (i - '0') * (int)Math.Pow(2, index); // Math equation for BIN to DEC 
                    index--;
                }
                string s = res.ToString(); // Converts int to string 
                foreach(char i in s.Reverse()) // So it can be reversed in foreach
                {
                    Rconvert.Add(i.ToString());
                }

            }
            else
            {
                foreach (char i in txt)
                {
                    bin.Add(i - '0'); // Fills the bin list with bin number in txt
                }
                List<int> Rbin = new List<int>(bin.AsEnumerable().Reverse()); // Creates new list that is reversed bin
 
                if (second == "OCT")  // If the BIN is converted to OCT
                {
                    Rconvert = BIN_convert(3, Rbin);
                }
                else if (second == "HEX")  // If the BIN is converted to HEX
                {
                    Rconvert = BIN_convert(4, Rbin);
                }
            }

            return Rconvert;
        }

        private List<string> OCT_convert(string second)
        {
            List<int> bin = new List<int>(); // List for bin numbers
            List<string> Rconvert = new List<string>(); // List for the complete
            string txt;
            if (!con)
                txt = textBox1.Text.Substring(4, textBox1.Text.Length - 8); // Get the number for conversion "Substring(where it starts, how long)"
            else
                txt = label1.Text.Substring(4, label1.Text.Length - 10); // Get the number for conversion "Substring(where it starts, how long)"
            int res = 0;

            textBox1.Clear(); // Clears text box

            int index = txt.Length - 1;
            foreach (char i in txt)
            {
                res = res + (i - '0') * (int)Math.Pow(8, index); // Math equation for OCT to DEC 
                index--;
            }

            if (second == "DEC") // If the OCT is converted to DEC
            {
                string s = res.ToString(); // Converts int to string
                foreach (char i in s.Reverse()) // So it can be reversed in foreach
                {
                    Rconvert.Add(i.ToString());
                }

                return Rconvert;
            }

            bin = To_BIN(res);
            foreach(int i in bin)
            {
                Rconvert.Add(i.ToString());
            } // Converts OCT to BIN

            if (second == "HEX")  // If the OCT is converted to HEX
            {
                Rconvert = BIN_convert(4, bin);
            }

            return Rconvert;
        }

        private List<string> HEX_convert(string second)
        {
            List<int> bin = new List<int>(); // List for bin numbers
            List<string> Rconvert = new List<string>(); // List for the complete
            string txt;
            if (!con)
                txt = textBox1.Text.Substring(4, textBox1.Text.Length - 8); // Get the number for conversion "Substring(where it starts, how long)"
            else
                txt = label1.Text.Substring(4, label1.Text.Length - 10); // Get the number for conversion "Substring(where it starts, how long)"
            List<string> chars = new List<string>();
            foreach(char c in txt)
            {
                List<char> l = new List<char>(new char[] {'A','B','C','D','E','F'});
                if (l.Contains(char.ToUpper(c)))
                {
                    chars.Add(((int)char.ToUpper(c) - 55).ToString());
                }
                else
                {
                    chars.Add(c.ToString());
                }
            }
            int res = 0;

            textBox1.Clear(); // Clears text box

            int index = txt.Length - 1;
            foreach (string i in chars)
            {
                res = res + (Convert.ToInt32(i)) * (int)Math.Pow(16, index); // Math equation for HEX to DEC 
                index--;
            }

            if (second == "DEC") // If the HEX is converted to DEC
            {
                string s = res.ToString(); // Converts the int to string
                foreach (char i in s.Reverse()) // So it can be reversed in foreach
                {
                    Rconvert.Add(i.ToString());
                }
                return Rconvert;
            }

            bin = To_BIN(res);
            foreach (int i in bin)
            {
                Rconvert.Add(i.ToString());
            } // Converts OCT to BIN

            if (second == "OCT")  // If the HEX is converted to OCT
            {
                Rconvert = BIN_convert(3, bin);
            }

            return Rconvert;
        }

        private List<string> BIN_convert(int n, List<int> bin)
        {
            List<string> Rconvert = new List<string>(); // List for conversion
            while (bin.Count % n != 0) // Checks if the bin number lenght can be divined by n 
            {
                bin.Add(0);
            }

            for (int i = 0; i < bin.Count / n; i++)
            {
                int r = 0;
                for (int j = i * n; j < i * n + n; j++)
                {
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
                if (r >= 10)
                {
                    Rconvert.Add(Convert.ToChar(r + 55).ToString());
                }
                else
                {
                    Rconvert.Add(r.ToString());
                }
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
            convert.Clear();
            buttonA.Visible = false;
            buttonB.Visible = false;
            buttonCL.Visible = false;
            buttonD.Visible = false;
            buttonE.Visible = false;
            buttonF.Visible = false;
            first = true;
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            result = 0;
            convert.Clear();
            buttonA.Visible = false;
            buttonB.Visible = false;
            buttonCL.Visible = false;
            buttonD.Visible = false;
            buttonE.Visible = false;
            buttonF.Visible = false;
        }

        private void backSpace_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
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
                string message = "Error evaluating expression";
                string title = "Warning";
                MessageBox.Show(message, title);
                //throw new Exception("Error evaluating expression");
            }
            else
            {
                // get the type and method from the compiled assembly
                Type type = results.CompiledAssembly.GetType("Calculator.Evaluator");
                System.Reflection.MethodInfo method = type.GetMethod("Evaluate");

                // invoke the method and return the result
                return (double)method.Invoke(null, null);
            }
            return 0;
        }

        private void buttonFile_Click(object sender, EventArgs e)
        {
            if (!quit)
            {
                checkBox1.Checked = true;
                quit = true;
            }
            else if(quit)
            {
                backwards.Visible = false;
                forward.Visible = false;
                buttonFile.Text = "FROM FILE";
                textBox1.Clear();
                label1.Text = "";
                lines.Clear();
                checkBox1.Checked = false;
                quit = false;
            }

        }

        private void fromFile(int i, List<string> lines, int conv)
        {
            label1.Text = lines[i];
            string str = "";
            if (label1.Text.Last() == '=') str = label1.Text.Substring(0, label1.Text.Length - 2);
            else str = label1.Text;
            if (conv == 0)
            {
                textBox1.Clear();
                textBox1.Text += Evaluate(str);
            }
            else if(conv == 1)
            {
                textBox1.Clear();
                convert.Add(str.Substring(0,3));
                convert.Add(str.Substring(str.Length - 3, 3));
                con = true;
                button18.PerformClick();
            }
            else if(conv == 2)
            {
                string[,] strings = { { "!", "Ʌ", "V", "X", "N", "A", "=>", "<=" }, { "!", "Ʌ", "V", "V̲", "↓", "↑", "=>", "<=>" }, { "NOT", "AND", "OR", "XOR", "NOR", "NAND", "IMPLY", "XNOR" } };

               // for (int x = 0; x < str.Length; x++)
               // {
                    for (int y = 0; y < strings.GetLength(1); y++)
                    {
                    //Regex.IsMatch(originalString, pattern)
                    //str.Contains(strings[0, y])
                        if (Regex.IsMatch(str, strings[0, y]))
                        {
                            logic_operators.Add(strings[2, y]);
                            label1.Text = str.Replace(strings[0, y], strings[1, y]);
                        }
                    }
                //}

                logic_cal();
            }
        }

        private void convert_click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0" || execute == true)
                textBox1.Clear();
            if (convert.Any())
            {
                textBox1.Text += " ";
                textBox1.Text += (sender as Button).Text;
            }
            else
            {
                textBox1.Text += (sender as Button).Text;
                textBox1.Text += " ";
            }
            execute = false;
            ex = true;
            operation = null;
            convert.Add((sender as Button).Text);
            if (convert[0] == "HEX")
            {
                buttonA.Visible = true;
                buttonB.Visible = true;
                buttonCL.Visible = true;
                buttonD.Visible = true;
                buttonE.Visible = true;
                buttonF.Visible = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();

                    openFileDialog1.InitialDirectory = "c:\\"; // Where the OpenFileDialog opens itself
                    openFileDialog1.Title = "Browse Text Files"; // Sets the dialog box title
                    openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"; // Filters the only files user can see and choose
                    openFileDialog1.CheckFileExists = true; // Checks if the file exists
                    openFileDialog1.CheckPathExists = true; // Checks if the path exists
                    openFileDialog1.Multiselect = false; // Multiselect turnd off

                    if (openFileDialog1.ShowDialog() == DialogResult.OK) // If everything is ok
                    {
                        // specify the file path
                        string filePath = openFileDialog1.FileName;
                        // open the file using a StreamReader
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            // read the file line by line
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                lines.Add(line);
                            }
                            backwards.Visible = true;
                            forward.Visible = true;
                            buttonFile.Text = "STOP READING";

                            int index = 1;
                            if (lines[0] == "$")
                            {
                                conv = 0;
                                fromFile(index, lines, conv);
                            }
                            else if(lines[0] == "#")
                            {
                                conv = 1;
                                fromFile(index, lines, conv);
                            }
                            else if(lines[0] == "&")
                            {
                                conv = 2;
                                fromFile(index, lines, conv);
                            }

                            // attach event handlers to the backwards and forward buttons
                            this.backwards.Click += (sender2, e2) =>
                            {
                                if(index - 1 >= 1)
                                    index--;
                                this.Invoke((MethodInvoker)delegate
                                {
                                    fromFile(index, lines, conv);
                                });
                            };
                            this.forward.Click += (sender2, e2) =>
                            {
                                if(index + 1 < lines.Count)
                                    index++;
                                this.Invoke((MethodInvoker)delegate
                                {
                                    fromFile(index, lines, conv);
                                });
                            };

                            // start the background thread
                            Task.Run(() =>
                            {
                                // run the loop on the background thread
                                while (!quit)
                                {
                                    // wait for a button click
                                }
                            });
                        }
                    }
                });
            }
        }

        bool cal = false;
        private void button25_Click(object sender, EventArgs e)
        {
            if (!cal)
            {
                buttonZero.Visible = true;
                buttonOne.Visible = true;
                buttonNOT.Visible = true;
                buttonAND.Visible = true;
                buttonOR.Visible = true;
                buttonXOR.Visible = true;
                buttonNOR.Visible = true;
                buttonNAND.Visible = true;
                buttonXNOR.Visible = true;
                buttonIMPLY.Visible = true;
                buttonEnter2.Visible = true;

                button25.Text = "CALCULATOR";

                cal = true;
            }
            else
            {
                buttonZero.Visible = false;
                buttonOne.Visible = false;
                buttonNOT.Visible = false;
                buttonAND.Visible = false;
                buttonOR.Visible = false;
                buttonXOR.Visible = false;
                buttonNOR.Visible = false;
                buttonNAND.Visible = false;
                buttonXNOR.Visible = false;
                buttonIMPLY.Visible = false;
                buttonEnter2.Visible = false;

                button25.Text = "LOGIC GATES";

                cal = true;
                cal = false;
            }
        }

        private bool toBool(char ch)
        {
            if (ch == '1')
                return true;
            else if (ch == '0')
                return false;
            else
                throw new ArgumentException("Invalid input");
        }

        private void equalLength(ref string input1, ref string input2)
        {
            if (input1.Length != input2.Length)
            {
                if (input1.Length > input2.Length)
                {
                    int size = input1.Length - input2.Length;
                    for (int i = 0; i < size; i++)
                    {
                        input2 = input2.Insert(0, "0");
                    }
                }
                else
                {
                    int size = input2.Length - input1.Length;
                    for (int i = 0; i < size; i++)
                    {
                        input1 = input1.Insert(0, "0");
                    }
                }
            }
        }

        private void Not(string inp)
        {
            string input = inp.Substring(1, inp.Length - 1);
            textBox1.Clear();
            for (int i = 0; i < input.Length; i++)
            {
                textBox1.Text += Convert.ToInt16(!toBool(input[i])).ToString();
            }
        }

        public void And(string inp)
        {
            string input1 = inp.Substring(0, inp.IndexOf("Ʌ"));
            string input2 = inp.Substring(inp.IndexOf("Ʌ") + 1, inp.Length - inp.IndexOf("Ʌ") - 1);
            textBox1.Clear();

            equalLength(ref input1, ref input2);

            for (int i = 0; i < input1.Length; i++)
            {
                textBox1.Text += (Convert.ToInt16(toBool(input1[i]) && toBool(input2[i]))).ToString();
            }
        }

        public void Or(string inp)
        {
            string input1 = inp.Substring(0, inp.IndexOf("V"));
            string input2 = inp.Substring(inp.IndexOf("V") + 1, inp.Length - inp.IndexOf("V") - 1);
            textBox1.Clear();

            equalLength(ref input1, ref input2);

            for (int i = 0; i < input1.Length; i++)
            {
                textBox1.Text += (Convert.ToInt16(toBool(input1[i]) || toBool(input2[i]))).ToString();
            }
        }

        public void Xor(string inp)
        {
            string input1 = inp.Substring(0, inp.IndexOf("V̲"));
            string input2 = inp.Substring(inp.IndexOf("V̲") + 2, inp.Length - inp.IndexOf("V̲") - 2);
            textBox1.Clear();

            equalLength(ref input1, ref input2);

            for (int i = 0; i < input1.Length; i++)
            {
                textBox1.Text += (Convert.ToInt16(toBool(input1[i]) ^ toBool(input2[i]))).ToString();
            }
        }

        public void Nor(string inp)
        {
            string input1 = inp.Substring(0, inp.IndexOf("↓"));
            string input2 = inp.Substring(inp.IndexOf("↓") + 2, inp.Length - inp.IndexOf("↓") - 2);
            textBox1.Clear();

            equalLength(ref input1, ref input2);

            for (int i = 0; i < input1.Length; i++)
            {
                textBox1.Text += (Convert.ToInt16(!(toBool(input1[i]) || toBool(input2[i])))).ToString();
            }
        }

        public void Nand(string inp)
        {
            string input1 = inp.Substring(0, inp.IndexOf("↑"));
            string input2 = inp.Substring(inp.IndexOf("↑") + 2, inp.Length - inp.IndexOf("↑") - 2);
            textBox1.Clear();

            equalLength(ref input1, ref input2);

            for (int i = 0; i < input1.Length; i++)
            {
                textBox1.Text += (Convert.ToInt16(!(toBool(input1[i]) && toBool(input2[i])))).ToString();
            }
        }

        public void Imply(string inp)
        {
            string input1 = inp.Substring(0, inp.IndexOf("=>"));
            string input2 = inp.Substring(inp.IndexOf("=>") + 2, inp.Length - inp.IndexOf("=>") - 2);
            textBox1.Clear();

            equalLength(ref input1, ref input2);

            for (int i = 0; i < input1.Length; i++)
            {
                textBox1.Text += (Convert.ToInt16(!toBool(input1[i]) || toBool(input2[i]))).ToString();
            }
        }

        public void Xnor(string inp)
        {
            string input1 = inp.Substring(0, inp.IndexOf("<=>"));
            string input2 = inp.Substring(inp.IndexOf("<=>") + 3, inp.Length - inp.IndexOf("<=>") - 3);
            textBox1.Clear();

            equalLength(ref input1, ref input2);

            for (int i = 0; i < input1.Length; i++)
            {
                textBox1.Text += (Convert.ToInt16(!(toBool(input1[i]) ^ toBool(input2[i])))).ToString();
            }
        }

        private void logic_cal()
        {
            try
            {
                string input = "";
                if (ex) input = textBox1.Text;
                else input = label1.Text;

                for (int i = 0; i < logic_operators.Count; i++)
                {
                    switch (logic_operators[i])
                    {
                        case "NOT": // Adds up the numbers
                            Not(input);
                            break;
                        case "AND": // Subtracts the numbers
                            And(input);
                            break;
                        case "OR": // Multiplys the numbers
                            Or(input);
                            break;
                        case "XOR": // Divine the numbers
                            Xor(input);
                            break;
                        case "NOR": // Divine the numbers
                            Nor(input);
                            break;
                        case "NAND": // Divine the numbers
                            Nand(input);
                            break;
                        case "IMPLY": // Divine the numbers
                            Imply(input);
                            break;
                        case "XNOR": // Divine the numbers
                            Xnor(input);
                            break;
                        default:
                            break;

                    }
                }
                
            }
            catch (Exception w)
            {
                // If something went wrong throws an exception
                string message = w.Message;
                string title = "Warning";
                MessageBox.Show(message, title);
            }
            ex = false;
            logic_operators.Clear();
        }

        private void buttonEnter2_Click(object sender, EventArgs e)
        {
            logic_cal();
        }
    }
}