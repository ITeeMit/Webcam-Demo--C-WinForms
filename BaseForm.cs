using System;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WebcamDemo
{
    public partial class BaseForm : Form
    {
        protected VideoCaptureDevice? videoSource;
        protected FilterInfoCollection? videoDevices;
        protected PictureBox pictureBoxCamera = null!;
        protected Label labelStatus = null!;
        protected bool isWebcamStarted = false;
        protected bool isDisposing = false;
        private readonly object webcamLock = new object();

        // Form navigation enumeration
        public enum FormType
        {
            MainForm = 1,
            Form2 = 2,
            Form3 = 3,
            Form4 = 4
        }

        public BaseForm()
        {
            InitializeBaseComponents();
            InitializeVideoDevices();
        }

        protected virtual void InitializeBaseComponents()
        {
            this.pictureBoxCamera = new PictureBox();
            this.labelStatus = new Label();
            this.SuspendLayout();

            // BaseForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += BaseForm_FormClosing;

            // pictureBoxCamera
            this.pictureBoxCamera.BackColor = Color.Black;
            this.pictureBoxCamera.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBoxCamera.Location = new Point(50, 50);
            this.pictureBoxCamera.Size = new Size(640, 480);
            this.pictureBoxCamera.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxCamera.TabStop = false;

            // labelStatus
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.labelStatus.Location = new Point(50, 15);
            this.labelStatus.Size = new Size(120, 21);
            this.labelStatus.Text = "สถานะ: เตรียมพร้อม";
            this.labelStatus.ForeColor = Color.Green;

            // Add controls to form
            this.Controls.Add(this.pictureBoxCamera);
            this.Controls.Add(this.labelStatus);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected virtual void InitializeVideoDevices()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                
                if (videoDevices.Count == 0)
                {
                    labelStatus.Text = "สถานะ: ไม่พบกล้อง";
                    labelStatus.ForeColor = Color.Red;
                    return;
                }

                labelStatus.Text = $"สถานะ: พบกล้อง {videoDevices.Count} ตัว";
                labelStatus.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการเริ่มต้นกล้อง: {ex.Message}", 
                    "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                labelStatus.Text = "สถานะ: เกิดข้อผิดพลาด";
                labelStatus.ForeColor = Color.Red;
            }
        }

        public virtual void StartWebcam()
        {
            lock (webcamLock)
            {
                if (isWebcamStarted || isDisposing || videoDevices == null || videoDevices.Count == 0)
                    return;

                try
                {
                    // ใช้กล้องตัวแรกที่พบ
                    videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                    videoSource.NewFrame += VideoSource_NewFrame;
                    
                    // เพิ่ม timeout สำหรับการเริ่มต้น
                    var startTask = Task.Run(() =>
                    {
                        try
                        {
                            videoSource.Start();
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    });

                    // รอการเริ่มต้นไม่เกิน 5 วินาที
                    if (startTask.Wait(5000) && startTask.Result)
                    {
                        isWebcamStarted = true;
                        UpdateStatus("สถานะ: กำลังบันทึก", Color.Blue);
                    }
                    else
                    {
                        // หยุดและทำความสะอาดหากเริ่มไม่สำเร็จ
                        videoSource?.SignalToStop();
                        videoSource = null;
                        UpdateStatus("สถานะ: ไม่สามารถเริ่มกล้องได้", Color.Red);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"เกิดข้อผิดพลาดในการเริ่มกล้อง: {ex.Message}", 
                        "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("สถานะ: เกิดข้อผิดพลาด", Color.Red);
                }
            }
        }

        public virtual void StopWebcam()
        {
            lock (webcamLock)
            {
                try
                {
                    if (videoSource != null)
                    {
                        // ยกเลิก event handler ก่อนหยุด
                        videoSource.NewFrame -= VideoSource_NewFrame;
                        
                        if (videoSource.IsRunning)
                        {
                            // ใช้ async stop เพื่อป้องกันการค้าง
                            var stopTask = Task.Run(() =>
                            {
                                try
                                {
                                    videoSource.SignalToStop();
                                    // รอการหยุดไม่เกิน 3 วินาที
                                    videoSource.WaitForStop();
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error stopping webcam: {ex.Message}");
                                }
                            });

                            // รอการหยุดไม่เกิน 4 วินาที
                            stopTask.Wait(4000);
                        }
                        
                        videoSource = null;
                    }

                    isWebcamStarted = false;
                    
                    // Clear the picture box safely
                    ClearPictureBox();
                    
                    if (!isDisposing)
                    {
                        UpdateStatus("สถานะ: หยุด", Color.Orange);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in StopWebcam: {ex.Message}");
                    if (!isDisposing)
                    {
                        UpdateStatus("สถานะ: เกิดข้อผิดพลาดในการหยุด", Color.Red);
                    }
                }
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (isDisposing || pictureBoxCamera == null)
                return;

            try
            {
                // Clone the frame to avoid disposing issues
                using (var originalFrame = eventArgs.Frame)
                {
                    var bitmap = new Bitmap(originalFrame);
                    
                    // Update UI on the main thread
                    if (pictureBoxCamera.InvokeRequired)
                    {
                        pictureBoxCamera.BeginInvoke(new Action(() =>
                        {
                            if (!isDisposing && pictureBoxCamera != null)
                            {
                                var oldImage = pictureBoxCamera.Image;
                                pictureBoxCamera.Image = bitmap;
                                oldImage?.Dispose();
                            }
                            else
                            {
                                bitmap?.Dispose();
                            }
                        }));
                    }
                    else
                    {
                        if (!isDisposing)
                        {
                            var oldImage = pictureBoxCamera.Image;
                            pictureBoxCamera.Image = bitmap;
                            oldImage?.Dispose();
                        }
                        else
                        {
                            bitmap?.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Frame processing error: {ex.Message}");
            }
        }

        private void BaseForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            isDisposing = true;
            StopWebcam();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposing = true;
                StopWebcam();
                pictureBoxCamera?.Dispose();
                labelStatus?.Dispose();
            }
            base.Dispose(disposing);
        }

        // Protected method สำหรับ Form ลูกที่จะ override
        protected virtual void OnFormLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Auto start webcam when form loads
            StartWebcam();
        }

        protected override void OnLoad(EventArgs e)
        {
            OnFormLoad(e);
        }

        private void UpdateStatus(string message, Color color)
        {
            try
            {
                if (labelStatus != null && !isDisposing)
                {
                    if (labelStatus.InvokeRequired)
                    {
                        labelStatus.Invoke(new Action(() =>
                        {
                            if (!isDisposing)
                            {
                                labelStatus.Text = message;
                                labelStatus.ForeColor = color;
                            }
                        }));
                    }
                    else
                    {
                        labelStatus.Text = message;
                        labelStatus.ForeColor = color;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating status: {ex.Message}");
            }
        }

        private void ClearPictureBox()
        {
            try
            {
                if (pictureBoxCamera != null && !isDisposing)
                {
                    if (pictureBoxCamera.InvokeRequired)
                    {
                        pictureBoxCamera.Invoke(new Action(() =>
                        {
                            if (!isDisposing)
                            {
                                var oldImage = pictureBoxCamera.Image;
                                pictureBoxCamera.Image = null;
                                oldImage?.Dispose();
                            }
                        }));
                    }
                    else
                    {
                        var oldImage = pictureBoxCamera.Image;
                        pictureBoxCamera.Image = null;
                        oldImage?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing picture box: {ex.Message}");
            }
        }

        #region Centralized Navigation Functions

        /// <summary>
        /// Navigate to the next form in sequence
        /// </summary>
        /// <param name="currentFormType">Current form type</param>
        protected virtual void NavigateToNextForm(FormType currentFormType)
        {
            FormType nextFormType = GetNextFormType(currentFormType);
            if (nextFormType != FormType.MainForm || currentFormType != FormType.Form4)
            {
                NavigateToForm(nextFormType, NavigationType.Forward);
            }
        }

        /// <summary>
        /// Navigate to the previous form in sequence
        /// </summary>
        /// <param name="currentFormType">Current form type</param>
        protected virtual void NavigateToPreviousForm(FormType currentFormType)
        {
            FormType previousFormType = GetPreviousFormType(currentFormType);
            NavigateToForm(previousFormType, NavigationType.Backward);
        }

        /// <summary>
        /// Show exit confirmation dialog and close application if confirmed
        /// </summary>
        protected virtual void ShowExitConfirmation()
        {
            DialogResult result = MessageBox.Show(
                "คุณต้องการปิดโปรแกรมหรือไม่?",
                "ยืนยันการปิดโปรแกรม",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                StopWebcam();
                Application.Exit();
            }
        }

        /// <summary>
        /// Central navigation method with webcam management
        /// </summary>
        /// <param name="targetFormType">Target form to navigate to</param>
        /// <param name="navigationType">Type of navigation (Forward/Backward)</param>
        private void NavigateToForm(FormType targetFormType, NavigationType navigationType)
        {
            try
            {
                // Stop current webcam
                StopWebcam();

                if (navigationType == NavigationType.Forward)
                {
                    // Create and show new form
                    Form targetForm = CreateFormInstance(targetFormType);
                    targetForm.Show();
                    this.Hide();
                }
                else // Backward navigation
                {
                    // Find existing form and reactivate it
                    Form? existingForm = FindExistingForm(targetFormType);
                    if (existingForm != null)
                    {
                        existingForm.Show();
                        // Restart webcam on target form after a delay
                        RestartWebcamOnForm(existingForm);
                        this.Close();
                    }
                    else
                    {
                        // Fallback: create new instance if not found
                        Form targetForm = CreateFormInstance(targetFormType);
                        targetForm.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการนำทาง: {ex.Message}",
                    "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Create form instance based on form type
        /// </summary>
        /// <param name="formType">Type of form to create</param>
        /// <returns>New form instance</returns>
        private Form CreateFormInstance(FormType formType)
        {
            return formType switch
            {
                FormType.MainForm => new MainForm(),
                FormType.Form2 => new Form2(),
                FormType.Form3 => new Form3(),
                FormType.Form4 => new Form4(),
                _ => throw new ArgumentException($"Unknown form type: {formType}")
            };
        }

        /// <summary>
        /// Find existing form instance in open forms
        /// </summary>
        /// <param name="formType">Type of form to find</param>
        /// <returns>Existing form instance or null</returns>
        private Form? FindExistingForm(FormType formType)
        {
            return formType switch
            {
                FormType.MainForm => Application.OpenForms.OfType<MainForm>().FirstOrDefault(),
                FormType.Form2 => Application.OpenForms.OfType<Form2>().FirstOrDefault(),
                FormType.Form3 => Application.OpenForms.OfType<Form3>().FirstOrDefault(),
                FormType.Form4 => Application.OpenForms.OfType<Form4>().FirstOrDefault(),
                _ => null
            };
        }

        /// <summary>
        /// Restart webcam on target form after navigation
        /// </summary>
        /// <param name="targetForm">Form to restart webcam on</param>
        private void RestartWebcamOnForm(Form targetForm)
        {
            if (targetForm is BaseForm baseForm)
            {
                System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
                {
                    if (!baseForm.IsDisposed && !baseForm.isDisposing)
                    {
                        baseForm.Invoke(new Action(() =>
                        {
                            if (!baseForm.isDisposing)
                            {
                                baseForm.StartWebcam();
                            }
                        }));
                    }
                });
            }
        }

        /// <summary>
        /// Get next form type in sequence
        /// </summary>
        /// <param name="currentType">Current form type</param>
        /// <returns>Next form type</returns>
        private FormType GetNextFormType(FormType currentType)
        {
            return currentType switch
            {
                FormType.MainForm => FormType.Form2,
                FormType.Form2 => FormType.Form3,
                FormType.Form3 => FormType.Form4,
                FormType.Form4 => FormType.Form4, // Stay at Form4
                _ => FormType.MainForm
            };
        }

        /// <summary>
        /// Get previous form type in sequence
        /// </summary>
        /// <param name="currentType">Current form type</param>
        /// <returns>Previous form type</returns>
        private FormType GetPreviousFormType(FormType currentType)
        {
            return currentType switch
            {
                FormType.Form4 => FormType.Form3,
                FormType.Form3 => FormType.Form2,
                FormType.Form2 => FormType.MainForm,
                FormType.MainForm => FormType.MainForm, // Stay at MainForm
                _ => FormType.MainForm
            };
        }

        /// <summary>
        /// Navigation type enumeration
        /// </summary>
        private enum NavigationType
        {
            Forward,
            Backward
        }

        #endregion
    }
}