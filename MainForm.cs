using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WebcamDemo
{
    public partial class MainForm : BaseForm
    {
        private Button buttonNext = null!;
        private Button buttonExit = null!;
        private Label labelFormTitle = null!;

        public MainForm()
        {
            InitializeComponent();
        }

        private new void InitializeComponent()
        {
            this.buttonNext = new Button();
            this.buttonExit = new Button();
            this.labelFormTitle = new Label();
            this.SuspendLayout();

            // MainForm
            this.Text = "Form 1 - Webcam Demo";
            this.BackColor = Color.LightBlue;

            // labelFormTitle
            this.labelFormTitle.AutoSize = true;
            this.labelFormTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.labelFormTitle.Location = new Point(350, 15);
            this.labelFormTitle.Size = new Size(100, 30);
            this.labelFormTitle.Text = "Form 1";
            this.labelFormTitle.ForeColor = Color.DarkBlue;

            // buttonNext
            this.buttonNext.Location = new Point(700, 280);
            this.buttonNext.Size = new Size(90, 40);
            this.buttonNext.Text = "Next Form 2";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.buttonNext.BackColor = Color.LightGreen;
            this.buttonNext.Click += ButtonNext_Click;

            // buttonExit
            this.buttonExit.Location = new Point(700, 330);
            this.buttonExit.Size = new Size(90, 40);
            this.buttonExit.Text = "ปิดโปรแกรม";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.buttonExit.BackColor = Color.LightCoral;
            this.buttonExit.ForeColor = Color.White;
            this.buttonExit.Click += ButtonExit_Click;

            // Add controls to form
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.labelFormTitle);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ButtonNext_Click(object? sender, EventArgs e)
        {
            // Use centralized navigation to Form2
            NavigateToNextForm(FormType.MainForm);
        }

        private void ButtonExit_Click(object? sender, EventArgs e)
        {
            // Use centralized exit confirmation
            ShowExitConfirmation();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                buttonNext?.Dispose();
                buttonExit?.Dispose();
                labelFormTitle?.Dispose();
            }
            base.Dispose(disposing);
        }
        // Override เพื่อปรับแต่งเมื่อฟอร์มโหลด
        protected override void OnFormLoad(EventArgs e)
        {
            base.OnFormLoad(e);
            labelStatus.Text = "สถานะ: Form 1 - เริ่มต้นอัตโนมัติ";
        }
    }
}