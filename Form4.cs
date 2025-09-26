using System;
using System.Drawing;
using System.Windows.Forms;

namespace WebcamDemo
{
    public partial class Form4 : BaseForm
    {
        private Button buttonBack = null!;
        private Label labelFormTitle = null!;

        public Form4()
        {
            InitializeComponent();
        }

        private new void InitializeComponent()
        {
            this.buttonBack = new Button();
            this.labelFormTitle = new Label();
            this.SuspendLayout();

            // Form4
            this.Text = "Form 4 - Webcam Demo";
            this.BackColor = Color.LightPink;

            // labelFormTitle
            this.labelFormTitle.AutoSize = true;
            this.labelFormTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.labelFormTitle.Location = new Point(350, 15);
            this.labelFormTitle.Size = new Size(100, 30);
            this.labelFormTitle.Text = "Form 4";
            this.labelFormTitle.ForeColor = Color.DarkRed;

            // buttonBack
            this.buttonBack.Location = new Point(700, 300);
            this.buttonBack.Size = new Size(90, 40);
            this.buttonBack.Text = "Back Form 3";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.buttonBack.BackColor = Color.LightSalmon;
            this.buttonBack.Click += ButtonBack_Click;

            // Add controls to form
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.labelFormTitle);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ButtonBack_Click(object? sender, EventArgs e)
        {
            // Use centralized navigation back to Form3
            NavigateToPreviousForm(FormType.Form4);
        }

        // Override เพื่อปรับแต่งเมื่อฟอร์มโหลด
        protected override void OnFormLoad(EventArgs e)
        {
            base.OnFormLoad(e);
            labelStatus.Text = "สถานะ: Form 4 - เริ่มต้นอัตโนมัติ";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                buttonBack?.Dispose();
                labelFormTitle?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}