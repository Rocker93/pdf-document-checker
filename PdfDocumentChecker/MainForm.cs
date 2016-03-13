using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PdfDocumentChecker
{
    public partial class MainForm : Form
    {
        private const string OkFolder = "\\OK\\";
        private const string WrongFolder = "\\WRONG\\";
        private string _pdfFolder;
        

        private List<string> _fileList;
        private List<string>.Enumerator _enumerator;
        private string _lastFile;
        private bool _lastClick = true;

        public MainForm()
        {
            InitializeComponent();
            AcceptButton = okBtn;
            CancelButton = wrongBtn;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string config = Directory.GetCurrentDirectory() + "\\config.txt";
            if (!File.Exists(config))
            {
                using (FolderBrowserDialog dlg = new FolderBrowserDialog())
                {
                    dlg.Description = "Choose PDF folder";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        // MessageBox.Show("You selected: " + dlg.SelectedPath);
                        StreamWriter file = new StreamWriter(config);
                        file.WriteLine(dlg.SelectedPath);
                        file.Close();
                        _pdfFolder = dlg.SelectedPath;
                    }
                    else
                    {
                        MessageBox.Show("You didn't chooose any folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
                }
            }
            else
            {
                StreamReader file = new StreamReader(config);
                _pdfFolder = file.ReadLine();
                file.Close();
            }
            bool exists = Directory.Exists(_pdfFolder + OkFolder);
            if (!exists)
            {
                Directory.CreateDirectory(_pdfFolder + OkFolder);
                Directory.CreateDirectory(_pdfFolder + WrongFolder);
            }
            _fileList = new List<string>(Directory.GetFiles(_pdfFolder, "*.pdf", SearchOption.TopDirectoryOnly));
            _enumerator = _fileList.GetEnumerator();
            NextFile();

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            MoveFile();
            okBtn.Focus();
        }

        private void MoveFile()
        {
            if (_lastFile != null)
            {
                string dir = Path.GetDirectoryName(_lastFile);
                string fn = Path.GetFileName(_lastFile);
                string path = dir + OkFolder + fn;
                string clickDir = _lastClick ? OkFolder : WrongFolder;
                File.Move(_lastFile, dir + clickDir + fn);
            }
            
        }


        private void OkButtonClick(object sender, EventArgs e)
        {
            _lastClick = true;
            NextFile();
        }

        private void WrongButton_Click(object sender, EventArgs e)
        {
            _lastClick = false;
            NextFile();
        }

        private void NextFile()
        {
            _lastFile = _enumerator.Current;
            if (_enumerator.MoveNext())
            {
                string currentFileName = _enumerator.Current;
                webBrowser1.Navigate(@"file:///" + currentFileName);
                string fn = Path.GetFileName(currentFileName);
                fileNameLabel.Text = fn;
            }
            else
            {
                webBrowser1.Dispose();
                MoveFile();
                MessageBox.Show(this, "Everything checked", "OK");
            }
        }
        
    }
}
