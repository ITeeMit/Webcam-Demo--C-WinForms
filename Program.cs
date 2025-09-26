using System;
using System.Windows.Forms;

namespace WebcamDemo
{
    internal static class Program
    {
        /// <summary>
        ///  จุดเริ่มต้นหลักของแอปพลิเคชัน - เริ่มจาก Form1
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm()); // เริ่มจาก MainForm (Form1)
        }
    }
}