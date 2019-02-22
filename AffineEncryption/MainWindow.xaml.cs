using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace AffineEncryption
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName;
        private string fileText;
        private string resultText;
        const int a = 17;
        const int b = 15;
        private List<String> allPossibilities;
        private byte[] plainText;
        private byte[] encryptedText;
        private byte[] decryptedText;

        public MainWindow()
        {
            InitializeComponent();
            fileName = "";
            fileText = "";
            resultText = "";
            allPossibilities = new List<String>();
            //plainText = new List<byte>();
            //encryptedText = new List<byte>();
            //decryptedText = new List<byte>();
        }

        private void readText()
        {
            this.fileText = File.ReadAllText(fileName);
            this.lblFileText.Content = fileText.ToString();

            byte[] asciiBytes = Encoding.ASCII.GetBytes(this.fileText);

            int count = 0;
            //foreach(byte b in asciiBytes)
            //{
            //    if (Encoding.ASCII.GetString(asciiBytes).ElementAt(count) == 'i') Debug.WriteLine(b);
            //    //Debug.WriteLine(b);
            //    this.plainText.Add(b);
            //    count++;
            //}

            this.plainText = asciiBytes;

            this.encryptedText = this.plainText;
        }

        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = ".tsp";
            dlg.Filter = "txt files (*.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                Debug.WriteLine(fileName);
                this.fileName = fileName;
                readText();
            }
        }

        private void encrypt()
        {
            this.resultText = "";
            byte[] cipher = new byte[this.plainText.Length];
            for (int i = 0; i < this.plainText.Length; i++)
            {
                int value = (a * this.plainText.ElementAt(i) + b) % 256;
                cipher[i] = Convert.ToByte(value);
            }

            this.encryptedText = cipher;

            this.resultText = "";
            this.resultText = Encoding.ASCII.GetString(cipher.ToArray());

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();
            this.fileText = this.resultText;

            using (BinaryWriter outputFile = new BinaryWriter(File.Open(System.IO.Path.Combine(fileDirectory, "decrypted.txt"), FileMode.Open)))
            {
                foreach (char c in this.resultText)
                {
                    outputFile.Write(c);
                }
            }
        }

        private void decrypt()
        {
            Debug.WriteLine("\nEntering Decrypt");
            this.resultText = "";
            int aInverse = 0;
            int flag = 0;
            byte[] decryptedBytes = new byte[this.encryptedText.Length];

            for (int i = 0; i < 255; i++)
            {
                flag = (a * i) % 256;
                if (flag == 1)
                {
                    aInverse = i;
                }
            }
            Debug.WriteLine(aInverse);
            for (int i = 0; i < this.encryptedText.Length; i++)
            {
                int value = ((aInverse * Convert.ToInt32(this.encryptedText.ElementAt(i))) - (b * aInverse)) % 256;
                if (value == -151) value = 105;
                decryptedBytes[i] = Convert.ToByte(Math.Abs(value));
            }

            this.decryptedText = decryptedBytes;

            this.resultText = "";
            this.resultText = Encoding.ASCII.GetString(decryptedBytes.ToArray(), 0, decryptedBytes.Count());

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();
            string newFileName = fileDirectory + "\\output.txt";

            using (BinaryWriter outputFile = new BinaryWriter(File.Open(System.IO.Path.Combine(fileDirectory, "decrypted.txt"), FileMode.Open)))
            {
                foreach (char c in this.resultText)
                {
                    outputFile.Write(c);
                }
            }
        }

        private void decrypt(int _a, int _b)
        {
            this.resultText = "";
            int aInverse = 0;
            int flag = 0;

            for (int i = 0; i < 255; i++)
            {
                flag = (_a * i) % 256;
                if (flag == 1)
                {
                    aInverse = i;
                }
            }
            for (int i = 0; i < this.fileText.Length; i++)
            {
                if (this.fileText[i] != ' ')
                {
                    this.resultText = this.resultText + (char)(((aInverse * this.fileText[i] - _b*aInverse) % 256) + 0);
                }
                else
                {
                    this.resultText += this.fileText[i];
                }
            }

            string result = "(" + _a + "," + _b + "): " + this.resultText;
            this.allPossibilities.Add(result);
            this.lblFileText.Content = this.resultText.ToString();
        }
        //32, 101, 116
        public void attack()
        {
            for (int i = 0; i < 256; i++)
            {
                if (i % 2 != 0 && i != 128)
                {
                    for (int j = 0; j < 256; j++)
                    {
                        decrypt(i, j);
                    }
                }
            }

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();

            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(fileDirectory, "attackOutput.txt")))
            {
                foreach (string possibility in this.allPossibilities)
                {
                    outputFile.WriteLine(""+possibility);
                    outputFile.WriteLine("------------------------------------");
                }
            }
        }

        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            encrypt();
        }

        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            decrypt();
        }

        private void BtnAttack_Click(object sender, RoutedEventArgs e)
        {
            attack();
        }
    }
}
