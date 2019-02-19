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
        const int b = 20;

        public MainWindow()
        {
            InitializeComponent();
            fileName = "";
            fileText = "";
            resultText = "";
        }

        private void readText()
        {
            this.fileText = File.ReadAllText(fileName);
            this.lblFileText.Content = fileText.ToString();
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
            string cipher = "";
            for (int i = 0; i < this.fileText.Length; i++)
            {
                if (this.fileText[i] != ' ')
                {
                    cipher = cipher + (char)((((a * (this.fileText[i] - 0)) + b) % 256) + 0);
                } else cipher += this.fileText[i];
            }

            this.resultText = cipher;

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();
            this.fileText = cipher;

            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(fileDirectory, "encrypted.txt")))
            {
                foreach (char c in this.resultText)
                {
                    outputFile.Write(c);
                }
            }
        }

        private void decrypt()
        {
            this.resultText = "";
            int aInverse = 0;
            int flag = 0;

            for (int i = 0; i < 255; i++)
            {
                flag = (a * i) % 256;
                if (flag == 1)
                {
                    aInverse = i;
                }
            }
            for (int i = 0; i < this.fileText.Length; i++)
            {
                if (this.fileText[i] != ' ')
                {
                    this.resultText = this.resultText + (char)(((aInverse * ((this.fileText[i] + 0 - b)) % 256)) + 0);
                } else {
                    this.resultText += this.fileText[i];
                }
            }

            string fileDirectory = System.IO.Path.GetDirectoryName(this.fileName);
            this.lblFileText.Content = this.resultText.ToString();
            string newFileName = fileDirectory + "\\output.txt";

            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(fileDirectory, "decrypted.txt")))
            {
                foreach (char c in this.resultText)
                {
                    outputFile.Write(c);
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

        }
    }
}
