using System;
using System.IO;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public partial class FormMods : Form
    {
        string textDeleteMod = "Удалить мод?";
        string textNoFileSelect = "Не выбран файл.";
        string textNoUninstalFile = "Нет .txt файла инструкции.";

        public FormMods()
        {
            InitializeComponent();
            FuncMisc.setFormFont(this);
            if (FormMain.numberStyle > 1)
            {
                imageBackgroundImage();
            }
            if (FormMain.langTranslate == "EN")
            {
                langTranslateEN();
            }
            refreshFileList();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void imageBackgroundImage()
        {
            BackgroundImage = Properties.Resources.FormBackground;
            FuncMisc.textColor(this, System.Drawing.SystemColors.ControlLight, System.Drawing.Color.Black, false);
        }
        private void langTranslateEN()
        {
            button_Install.Text = "Install";
            button_Uninstall.Text = "Uninstall";
            label2.Text = @"Files from Skyrim\Mods";
            textDeleteMod = "Delete mod?";
            textNoFileSelect = "No file select.";
            textNoUninstalFile = "No .txt instruction file.";
        }
        private void FormMods_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                button_Close_Click(this, new EventArgs());
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void refreshFileList()
        {
            if (Directory.Exists(FormMain.pathModsFolder))
            {
                foreach (string line in Directory.EnumerateFiles(FormMain.pathModsFolder))
                {
                    if (FormMain.archiveExt.Exists(s => s.Equals(Path.GetExtension(line), StringComparison.OrdinalIgnoreCase)))
                    {
                        listBox1.Items.Add(Path.GetFileName(line));
                    }
                }
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void button_Install_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                fileUnpack(listBox1.SelectedItem.ToString());
                if (listBox1.SelectedItem.ToString().IndexOf("frostfall", StringComparison.OrdinalIgnoreCase) >= 0 || listBox1.SelectedItem.ToString().IndexOf("disablefasttravel", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    FuncMisc.wideScreenMods();
                }
            }
            else
            {
                MessageBox.Show(textNoFileSelect);
            }
        }
        private void fileUnpack(string filename)
        {
            FuncMisc.toggleButtons(this, false);
            listBox1.Enabled = false;
            FuncMisc.unpackRAR(FormMain.pathModsFolder + filename, false);
            FuncMisc.toggleButtons(this, true);
            listBox1.Enabled = true;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void button_Uninstall_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                if (FuncMisc.dialogResult(textDeleteMod))
                {
                    string file = FormMain.pathModsFolder + Path.GetFileNameWithoutExtension(FormMain.pathModsFolder + listBox1.SelectedItem.ToString()) + ".txt";
                    if (File.Exists(file))
                    {
                        foreach (string line in File.ReadLines(file))
                        {
                            if (line.StartsWith("["))
                            {
                                break;
                            }
                            else if (line.Length > 0)
                            {
                                FuncFiles.deleteAny(FormMain.pathGameFolder + line);
                            }
                        }
                        if (FuncParser.keyExists(file, "UNINSTALL", "UNPACK"))
                        {
                            FuncMisc.unpackRAR(FormMain.pathGameFolder + FuncParser.stringRead(file, "UNINSTALL", "UNPACK"), true);
                        }
                    }
                    else
                    {
                        MessageBox.Show(textNoUninstalFile);
                    }
                }
            }
            else
            {
                MessageBox.Show(textNoFileSelect);
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void button_Close_MouseEnter(object sender, EventArgs e)
        {
            button_Close.BackgroundImage = Properties.Resources.buttonCloseGlow;
        }
        private void button_Close_MouseLeave(object sender, EventArgs e)
        {
            button_Close.BackgroundImage = Properties.Resources.buttonClose;
        }
        private void button_Close_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}