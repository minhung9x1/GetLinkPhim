using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace GetLinkPhim
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class Form1 : Form
    {
        public const string htmlFile = @"index.html";
        private AbsGetLinkPhim Phims;
   
        public Form1()
        {
            InitializeComponent();
            textBox2.Text = GetLinkPhim.Properties.Settings.Default.savePath;
            web1 = new WebBrowser();
            web1.Navigate(@"file://" + Path.Combine(Application.StartupPath, htmlFile));
        }


        private void btnGet_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (!txtUrl.Text.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !txtUrl.Text.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                txtUrl.Text = "http://" + txtUrl.Text;
            }
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("Không được để trống Url");
                return;
            }
            if (txtUrl.Text.ToLower().Contains("studymovie.net"))
            {
                Phims = new PhimStudymovie(txtUrl.Text,web1);
            }
            else if (txtUrl.Text.ToLower().Contains("toomva.com"))
            {
                Phims = new PhimToomva(txtUrl.Text);
            }
            else
            {
                MessageBox.Show("Không hỗ trợ trang này");
                return;
            }
          
            Phims.GetAllPhim();
            label2.Text = Phims.LstPhims.Count.ToString();
            DrawData();
        }

        private void DrawData()
        {
            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].HeaderText = "Tập phim";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            foreach (PhimInfo item in Phims.LstPhims)
            {
//                txt += item.GetUrl + Environment.NewLine;
//                txt += !string.IsNullOrWhiteSpace(item.Sub1)? item.Sub1 + Environment.NewLine:"";
//                txt += !string.IsNullOrWhiteSpace(item.Sub2)? item.Sub2 + Environment.NewLine:"";
             
                dataGridView1.Rows.Add(item.Name);
            }
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            dataGridView1.Columns.Add(btn);
            btn.HeaderText = "Download";
            btn.Text = "Tải";
            btn.Name = "btn";
            btn.UseColumnTextForButtonValue = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
               Phims.DownloadWithIdm(Phims.LstPhims[e.RowIndex], Path.Combine(textBox2.Text, Phims.LstPhims[e.RowIndex].Name));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            txtUrl.Text = Clipboard.GetText();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = Path.GetDirectoryName(folderBrowser.FileName);
                GetLinkPhim.Properties.Settings.Default.savePath = Path.GetDirectoryName(folderBrowser.FileName);
                GetLinkPhim.Properties.Settings.Default.Save();
                // ...
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (Phims == null)
            {
                MessageBox.Show("Chưa get link phim!");
                return;
            }
            if (Phims.LstPhims.Count == 0)
            {
                MessageBox.Show("Rất tiếc, không có phim nào!");
                return;
            }
            foreach (PhimInfo item in Phims.LstPhims)
            {
                Phims.DownloadWithIdm(item, Path.Combine(textBox2.Text, item.Name));
            }
        }


        private WebBrowser web1;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string text = string.Empty;
                text += Phims.LstPhims[e.RowIndex].Name + Environment.NewLine;
                text += Phims.LstPhims[e.RowIndex].GetUrl + Environment.NewLine;
                text += Phims.LstPhims[e.RowIndex].Sub1 + Environment.NewLine;
                text += Phims.LstPhims[e.RowIndex].Sub2 + Environment.NewLine;
                MessageBox.Show(text);
            }
        }
    }
}