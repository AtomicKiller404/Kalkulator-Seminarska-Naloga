using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;

namespace Kalkulator_Seminarska_Naloga
{
    public partial class Form1 : Form
    {
        double result = 0;
        string operation = "";
        bool execute = false;

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
                }
                else
                {
                    operation = (sender as Button).Text;
                    result = Double.Parse(textBox1.Text);
                    label1.Text = result + operation;
                    execute = true;
                }
            }catch(Exception w)
            {
                label2.Text = "";
                label2.Text+=w.Message;
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (execute)
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
            }
            else
            {
                label1.Text = "";
                if(textBox1.Text.Last() == '=')
                    label1.Text += Evaluate(textBox1.Text.Substring(0, textBox1.Text.Length - 1));
                else
                    label1.Text += Evaluate(textBox1.Text);
            }
        }

        private void buttonCE_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            label2.Text = "";
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            result = 0;
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
    }
}
