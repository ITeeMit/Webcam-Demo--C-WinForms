using System;
using System.Drawing;
using System.Windows.Forms;

namespace WebcamDemo
{
    public partial class Form2 : BaseForm
    {
        private Button buttonNext = null!;
        private Button buttonBack = null!;
        private Label labelFormTitle = null!;

        public Form2()
        {
            InitializeComponent();
        }

        private new void InitializeComponent()
        {
            this.buttonNext = new Button();
            this.buttonBack = new Button();
            this.labelFormTitle = new Label();
            this.SuspendLayout();

            // Form2
            this.Text = "Form 2 - Webcam Demo";
            this.BackColor = Color.LightGreen;

            // labelFormTitle
            this.labelFormTitle.AutoSize = true;
            this.labelFormTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.labelFormTitle.Location = new Point(350, 15);
            this.labelFormTitle.Size = new Size(100, 30);
            this.labelFormTitle.Text = "Form 2";
            this.labelFormTitle.ForeColor = Color.DarkGreen;

            // buttonNext
            this.buttonNext.Location = new Point(700, 280);
            this.buttonNext.Size = new Size(90, 40);
            this.buttonNext.Text = "Next Form 3";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.buttonNext.BackColor = Color.LightCoral;
            this.buttonNext.Click += ButtonNext_Click;

            // buttonBack
            this.buttonBack.Location = new Point(700, 330);
            this.buttonBack.Size = new Size(90, 40);
            this.buttonBack.Text = "Back Form 1";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.buttonBack.BackColor = Color.LightYellow;
            this.buttonBack.Click += ButtonBack_Click;

            // Add controls to form
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.labelFormTitle);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ButtonNext_Click(object? sender, EventArgs e)
        {
            // Use centralized navigation to Form3
            NavigateToNextForm(FormType.Form2);
        }

        private void ButtonBack_Click(object? sender, EventArgs e)
        {
            // Use centralized navigation back to Form1
            NavigateToPreviousForm(FormType.Form2);
        }

        // Override เพื่อปรับแต่งเมื่อฟอร์มโหลด
        protected override void OnFormLoad(EventArgs e)
        {
            base.OnFormLoad(e);
            labelStatus.Text = "สถานะ: Form 2 - เริ่มต้นอัตโนมัติ";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                buttonNext?.Dispose();
                buttonBack?.Dispose();
                labelFormTitle?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}