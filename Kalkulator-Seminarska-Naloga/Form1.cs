using System;
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
            operation = textBox1.Text;
            execute = false;

        }

        private void operator_click(object sender, EventArgs e)
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
        }

        private void button18_Click(object sender, EventArgs e)
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
        }

        private void buttonCE_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            result = 0;
        }

        private void backSpace_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
        }
    }
}
