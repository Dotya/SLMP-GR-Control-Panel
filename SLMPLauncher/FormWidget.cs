using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public partial class FormWidget : Form
    {
        FormMain mainFormStyle = null;

        public FormWidget()
        {
            InitializeComponent();
            FuncMisc.setFormFont(this);
            if (FormMain.numberStyle > 1)
            {
                ImageBackgroundImage();
            }
            if (FormMain.langTranslate == "EN")
            {
                langTranslateEN();
            }
            else
            {
                refreshStyles();
            }
            if (FuncParser.readAsBool(FormMain.pathLauncherINI, "General", "HideWebButton"))
            {
                ClientSize = new System.Drawing.Size(232, 60);
                label1.Size = new System.Drawing.Size(232, 60);
                pictureBox2.Visible = false;
                button_Updates.Visible = false;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void ImageBackgroundImage()
        {
            BackgroundImage = Properties.Resources.FormBackground;
            label2.ForeColor = System.Drawing.SystemColors.ControlLight;
        }
        private void langTranslateEN()
        {
            comboBox_Styles.Items.Clear();
            comboBox_Styles.Items.AddRange(new object[] { "Bright", "Dark" });
            refreshStyles();
            label2.Text = "Style:";
            button_Updates.Text = "Updates";
            button_RU.BackgroundImage = Properties.Resources.RUoff;
            button_EN.BackgroundImage = Properties.Resources.EN;
        }
        private void FormWidget_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                mainFormStyle = Owner as FormMain;
                mainFormStyle.button_Widget_Click(this, new EventArgs());
                Dispose();
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void comboBox_Styles_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormMain.numberStyle = comboBox_Styles.SelectedIndex + 1;
            mainFormStyle = Owner as FormMain;
            mainFormStyle.refreshStyle();
            if (comboBox_Styles.SelectedIndex == 0)
            {
                BackgroundImage = Properties.Resources.FormBackgroundNone;
                label2.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else if (comboBox_Styles.SelectedIndex == 1)
            {
                ImageBackgroundImage();
            }
        }
        private void refreshStyles()
        {
            FuncMisc.refreshComboBox(comboBox_Styles, new List<double>() { 1, 2 }, FormMain.numberStyle, false, comboBox_Styles_SelectedIndexChanged);
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void button_RU_Click(object sender, EventArgs e)
        {
            FormMain.langTranslate = "RU";
            mainFormStyle = Owner as FormMain;
            mainFormStyle.setLangTranslateRU();
            label2.Text = "Стиль:";
            button_Updates.Text = "Обновления";
            comboBox_Styles.Items.Clear();
            comboBox_Styles.Items.AddRange(new object[] { "Светлый", "Темный" });
            refreshStyles();
            button_EN.BackgroundImage = Properties.Resources.ENoff;
            button_RU.BackgroundImage = Properties.Resources.RU;
        }
        private void button_EN_Click(object sender, EventArgs e)
        {
            FormMain.langTranslate = "EN";
            mainFormStyle = Owner as FormMain;
            mainFormStyle.setLangTranslateEN();
            langTranslateEN();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        private void button_Updates_Click(object sender, EventArgs e)
        {
            var form = new FormUpdates();
            form.ShowDialog(Owner);
            form = null;
        }
    }
}