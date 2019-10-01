using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComplicatedEnigma
{
    public partial class Form1 : Form
    {
        string fileName;
        static byte[][] rotor = new byte[4][];
        static byte[][] switching = new byte[4][]; 
        static byte[] plainText, cText, shift, alphabet = new byte[256];
        static byte[] key;
        static byte p1, p2;

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 4; i++)
            {
                rotor[i] = new byte[256];
                switching[i] = new byte[16];
            }
            for (int i = 0; i < 256; i++)
            {
                alphabet[i] = (byte)i;
            }

            shift = new byte[4];
            key = new byte[16];
        }



        private void button2_Click(object sender, EventArgs e)
        {
            plainText = null;
            textBox5.Text = "";
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd.FileName;
                plainText = File.ReadAllBytes(fileName);
                Console.WriteLine();
                foreach (var item in plainText)
                {
                    textBox5.Text += (byte)item + ", ";  
                }
                Console.WriteLine(plainText.Length);
                ofd.Dispose();
            }
            key = File.ReadAllBytes("key.txt");
        }

        private void button3_Click(object sender, EventArgs e)
        {
             cText = null;
            textBox6.Text = "";
            byte[] result = Crypt(plainText, true);
            Console.WriteLine(result.Length);
            foreach (var item in Crypt(plainText, true))
                textBox6.Text += item + ", ";
        }

        public void button4_Click(object sender, EventArgs e)
        {
            cText = null;
            textBox6.Text = "";
            byte[] result = Crypt(plainText, false);
            Console.WriteLine(result.Length);
            foreach (var item in Crypt(plainText, false))
                textBox6.Text += item + ", ";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine();
            foreach (var item in cText)
            {
                Console.Write(item + " ");
            }
            File.WriteAllBytes("cyphered text.txt", cText);
        }

        public void PRVG()
        {
            byte[] bytes = new byte[276 + 16]; // 256rotors 16switch 4shift 
            byte[] sequence = new byte[276];

            //заполняем начальный массив байтов ключом
            for (int i = 0; i < key.Length; i++)
            {
                bytes[i] = key[i];
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                rotor[i][j] = (byte)j;
                }
            }
            
            for (int i = 16; i < bytes.Length; i++)
            {
                bytes[i] = (byte)((bytes[i - 16] + bytes[i - 5]) % 256);
            }
            sequence = bytes.Skip(16).ToArray();
            for (int i = 0; i < 4; i++)
            {
                switching[i] = sequence.Skip(i*16).Take(16).ToArray();
            }
            //switching = sequence.Take(16).ToArray();

            byte S = 0;
            byte helper = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    S = rotor[i][(S + rotor[i][j % 256] + switching[i][j % 16]) % 256];
                    helper = rotor[i][j % 256];
                    rotor[i][j % 256] = rotor[i][S];
                    rotor[i][S] = helper;
                    
                }    
            }

            

            //заполняем сдвиги
            shift = sequence.Skip(64).Take(4).ToArray();
            foreach (var s in shift)
            {
                Console.Write(s + "- s ");
            }
            Console.WriteLine(Environment.NewLine);
            //return sequence;
        }

        public byte[] Crypt(byte[] text, bool method)
        {
            int[] shifting = new int[4];
            cText =new byte[text.Length];
            PRVG();
            for (int i = 0; i < text.Length; i++)
            {
                if (method == true)
                {

                    cText[i] = RotorForaward(RotorForaward(RotorForaward(RotorForaward(text[i], rotor[0], shifting[0]), rotor[1], shifting[1]), rotor[2], shifting[2]), rotor[3], shifting[3]);
                    Console.Write($"{cText[i]} shift0 = {shifting[0]}, shift1 = {shifting[1]}, shift2 = {shifting[2]}, shift3 = {shifting[3]} ");
                    Console.WriteLine();
                }
                if (method == false)
                {  
                    cText[i] = RotorBackward(RotorBackward(RotorBackward(RotorBackward(text[i], rotor[3], shifting[3]), rotor[2], shifting[2]), rotor[1], shifting[1]), rotor[0], shifting[0]);
                    Console.Write($"{cText[i]} shift3 = {shifting[3]}, shift2 = {shifting[2]}, shift1 = {shifting[1]}, shift0 = {shifting[0]} ");
                }
                Console.WriteLine(shift.Length);
                for (int j = 0; j < shift.Length; j++) {
                    shifting[j] += shift[j];
                }
                Console.WriteLine();
            }
            
            Console.WriteLine();
            return (cText);
        }

        public static byte[] Switching(byte[] rotor)
        {
            byte S = 0, helper;
            for (int i = 0; i < rotor.Length; i++)
            {
                S = rotor[(S + rotor[i % 256] + switching[i][i % 16]) % 256];
                helper = rotor[i % 256];
                rotor[i % 256] = rotor[S];
                rotor[S] = helper;
            }
            return rotor;
        }

        public static byte RotorForaward(int text, byte[] rotAlph, int shift)
        {

            Console.WriteLine("new index " + ((text + shift) % 256));
            return rotAlph[(text + shift) % 256];
        }
        public static byte RotorBackward(int text, byte[] rotAlph, int shift)
        {
            return alphabet[(Array.IndexOf(rotAlph, text) + 256 * ((Array.IndexOf(rotAlph, text) < shift % 256) ? 1 : 0) - shift %256)];
        }

        static private int Mod(int a,int b)
        {
            return (b + a % b) % b;
        }
    }
}
//[1][2][3*][4][5][6_][7][8]

//генерируем последовательность
//заполняем роторы
//заполняем массив перестановок
//определение сдвигов 
//i = 115 rotAlph = 9, i = 9 rotAlph = 73, i = 73 rotAlph = 221, i = 221 rotAlph = 163, 


//new index 105
//new index 64
//new index 199
//new index 202
//160 shift0 = 0, shift1 = 0, shift2 = 0, shift3 = 0 
//4

//new index 143
//new index 44
//new index 13
//new index 88
//99 shift0 = 111, shift1 = 246, shift2 = 19, shift3 = 191 