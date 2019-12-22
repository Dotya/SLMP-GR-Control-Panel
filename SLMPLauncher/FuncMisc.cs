using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public class FuncMisc
    {
        public static bool refreshButton(Button button, string file, string section, string parameter, string value, bool invert)
        {
            if (FuncParser.keyExists(file, section, parameter))
            {
                button.Enabled = true;
                string readString = FuncParser.stringRead(file, section, parameter);
                bool toggle = false;
                toggle = readString != null && (readString == value || readString == "1" || string.Equals(readString, "true", StringComparison.OrdinalIgnoreCase));
                toggle = invert ? !toggle : toggle;
                if (toggle)
                {
                    button.BackgroundImage = Properties.Resources.buttonToggleOn;
                }
                else
                {
                    button.BackgroundImage = Properties.Resources.buttonToggleOff;
                }
                return toggle;
            }
            else
            {
                button.BackgroundImage = Properties.Resources.buttonToggleDisable;
                button.Enabled = false;
                return false;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void refreshTrackBar(TrackBar trackbar, string file, string section, string parameter, int division, Label label)
        {
            if (FuncParser.keyExists(file, section, parameter))
            {
                trackbar.Enabled = true;
                int readINT = FuncParser.intRead(file, section, parameter);
                if (division != -1)
                {
                    readINT = readINT / division;
                }
                if (readINT >= trackbar.Minimum && readINT <= trackbar.Maximum)
                {
                    if (division != -1)
                    {
                        label.Text = (readINT * division).ToString();
                    }
                    else
                    {
                        label.Text = readINT.ToString();
                    }
                    trackbar.Value = readINT;
                }
                else
                {
                    label.Text = "N/A";
                }
            }
            else
            {
                label.Text = "N/A";
                trackbar.Enabled = false;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void refreshComboBox(ComboBox combobox, List<double> list, double value, bool notEqual, EventHandler onchange)
        {
            if (onchange != null)
            {
                combobox.SelectedIndexChanged -= onchange;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (value == list[i] || (notEqual && value < list[i]))
                {
                    combobox.SelectedIndex = i;
                    break;
                }
                else if (i == list.Count - 1)
                {
                    combobox.SelectedIndex = -1;
                }
            }
            if (onchange != null)
            {
                combobox.SelectedIndexChanged += onchange;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void refreshNumericUpDown(NumericUpDown numeric, string file, string section, string parametr, EventHandler onchange)
        {
            int value = FuncParser.intRead(file, section, parametr);
            if (onchange != null)
            {
                numeric.ValueChanged -= onchange;
            }
            if (value >= numeric.Minimum && value <= numeric.Maximum)
            {
                numeric.Value = value;
            }
            else
            {
                numeric.Value = numeric.Minimum;
            }
            if (onchange != null)
            {
                numeric.ValueChanged += onchange;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void toggleButtons(Form form, bool action)
        {
            foreach (Control line in form.Controls)
            {
                if (line is Button)
                {
                    line.Enabled = action;
                }
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void textColor(Form form, Color color, Color trackbar, bool button)
        {
            foreach (Control line in form.Controls)
            {
                if (line is Label || (line is Button && button))
                {
                    line.ForeColor = color;
                }
                else if (line is TrackBar)
                {
                    line.BackColor = trackbar;
                }
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void setFormFont(Form form)
        {
            if (FormMain.customFont != null)
            {
                foreach (Control line in form.Controls)
                {
                    if (line is Label || line is Button || line is ListBox || line is ListView || line is ComboBox || line is NumericUpDown)
                    {
                        line.Font = new Font(FormMain.customFont, line.Font.Size, FormMain.customFontStyle, GraphicsUnit.Point);
                    }
                }
            }
        }
        public static void supportStrikeOut(string font)
        {
            if (!testStrikeOut(font, FontStyle.Regular))
            {
                if (testStrikeOut(font, FontStyle.Bold))
                {
                    FormMain.customFontStyle = FontStyle.Bold;
                }
                else if (testStrikeOut(font, FontStyle.Italic))
                {
                    FormMain.customFontStyle = FontStyle.Italic;
                }
                else if (testStrikeOut(font, FontStyle.Strikeout))
                {
                    FormMain.customFontStyle = FontStyle.Strikeout;
                }
                else if (testStrikeOut(font, FontStyle.Underline))
                {
                    FormMain.customFontStyle = FontStyle.Underline;
                }
                else
                {
                    FormMain.customFont = null;
                }
            }
        }
        private static bool testStrikeOut(string font, FontStyle style)
        {
            try
            {
                using (Font strikeout = new Font(font, 10, style))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void appendToFile(string path, string line)
        {
            try
            {
                File.AppendAllText(path, line + Environment.NewLine, new UTF8Encoding(false));
            }
            catch
            {
                MessageBox.Show(FormMain.textCouldWriteFile + path);
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void writeToFile(string path, List<string> list)
        {
            try
            {
                File.WriteAllLines(path, list, new UTF8Encoding(false));
            }
            catch
            {
                MessageBox.Show(FormMain.textCouldWriteFile + path);
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void wideScreenMods()
        {
            if (FuncParser.intRead(FormMain.pathLauncherINI, "General", "AspectRatio") == 4)
            {
                if (File.Exists(FormMain.pathDataFolder + @"Frostfall.esp") && (!File.Exists(FormMain.pathDataFolder + @"Frostfall-WS.esp") || !File.Exists(FormMain.pathDataFolder + @"Frostfall-WS.bsa")))
                {
                    unpackRAR(FormMain.pathSystemFolder + "AR(4)FF.rar");
                }
                if (File.Exists(FormMain.pathDataFolder + @"DisableFastTravel.bsa") && new FileInfo(FormMain.pathDataFolder + @"DisableFastTravel.bsa").Length != 45717)
                {
                    unpackRAR(FormMain.pathSystemFolder + "AR(4)DFT.rar");
                }
            }
            else
            {
                FuncFiles.deleteAny(FormMain.pathDataFolder + @"Frostfall-WS.esp");
                FuncFiles.deleteAny(FormMain.pathDataFolder + @"Frostfall-WS.bsa");
                if (File.Exists(FormMain.pathDataFolder + @"DisableFastTravel.bsa") && new FileInfo(FormMain.pathDataFolder + @"DisableFastTravel.bsa").Length != 45701)
                {
                    unpackRAR(FormMain.pathModsFolder + "DisableFastTravel.rar");
                }
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void unpackRAR(string path)
        {
            if (File.Exists(path) && File.Exists(FormMain.pathLauncherFolder + "UnRAR.exe"))
            {
                runProcess(FormMain.pathLauncherFolder + "UnRAR.exe", " x -y \"" + path + "\"" + " " + "\"" + FormMain.pathGameFolder + "\"", null, null, true, true);
            }
            else
            {
                MessageBox.Show(FormMain.textCouldUnpack + path);
            }
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static bool dialogResult(string message)
        {
            DialogResult dialog = MessageBox.Show(message, FormMain.textConfirmTitle, MessageBoxButtons.YesNo);
            return dialog == DialogResult.Yes;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void runProcess(string path, string arg, EventHandler onexit, Form form, bool shell, bool wait)
        {
            if (File.Exists(path))
            {
                Process process = new Process();
                process.StartInfo.FileName = path;
                if (arg != null)
                {
                    process.StartInfo.Arguments = arg;
                }
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                process.StartInfo.UseShellExecute = shell;
                if (onexit != null && form != null && !wait)
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += onexit;
                }
                try
                {
                    process.Start();
                    if (wait)
                    {
                        process.WaitForExit();
                    }
                }
                catch
                {
                    MessageBox.Show(FormMain.textCouldRun + path);
                }
            }
        }
    }
}