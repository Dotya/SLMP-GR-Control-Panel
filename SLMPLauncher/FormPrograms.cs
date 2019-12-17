using System;
using System.IO;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public partial class FormPrograms : Form
    {
        public FormPrograms()
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
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void imageBackgroundImage()
        {
            BackgroundImage = Properties.Resources.FormBackground;
            label2.ForeColor = System.Drawing.SystemColors.ControlLight;
        }
        private void langTranslateEN()
        {
            label2.Text = "Unpack programs at " + Environment.NewLine + "the root of the game:";
        }
        private void FormPrograms_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                button_Close_Click(this, new EventArgs());
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void button_CreationKit_Click(object sender, EventArgs e)
        {
            programsUnpack("CREATIONKIT.rar");
        }
        private void button_TES5Edit_Click(object sender, EventArgs e)
        {
            programsUnpack("TES5EDIT.rar");
        }
        private void button_TES5LODGen_Click(object sender, EventArgs e)
        {
            programsUnpack("TES5LODGEN.rar");
        }
        private void programsUnpack(string FileName)
        {
            FuncMisc.toggleButtons(this, false);
            FuncMisc.unpackRAR(FormMain.pathProgramFilesFolder + FileName);
            FuncMisc.toggleButtons(this, true);
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