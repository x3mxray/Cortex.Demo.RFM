using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Demo.Project.DemoDataExplorer.Entities;
using Demo.Project.DemoDataExplorer.Extensions;

namespace Demo.Project.DemoDataExplorer
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Text = "Demo Data Explorer " + Assembly.GetExecutingAssembly().GetName().Version;

            var excelFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Online Retail.xlsx");

            if (File.Exists(excelFile))
                txtFileUpload.Text = excelFile;
        }

        private void btnFileUplad_Click(object sender, System.EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    RestoreDirectory = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtFileUpload.Text = dialog.FileName;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Thread th;
        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateValues(ApiMethod.UploadFile))
                {
                    ClearLogs();
                    LockInteraface();

                    AddUploadLog("Upload started", true);
                    AddUploadLog("Please wait. It may take quite a long time (5 ~ 15 minutes)...");

                    if(th != null && th.IsAlive)
                       th.Abort();

                    th = new Thread(() =>
                    {
                        var client = new WebClient();

                        var uri = new Uri(txtApi.Text.CombineUrl("/api/contactapi/UploadClientsHistory"));
                        var data = System.IO.File.ReadAllBytes(txtFileUpload.Text);

                        _percentageUploaded = 0;

                        client.UploadDataCompleted += new UploadDataCompletedEventHandler(UploadDataCallback);
                        client.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgressChanged);
                        client.UploadDataAsync(uri, "POST", data);

                    }){ IsBackground = true };
                    th.Start();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UploadDataCallback(Object sender, UploadDataCompletedEventArgs e)
        {
            try
            {
                Thread.Sleep(1000);
                
                byte[] data = (byte[]) e.Result;
                string reply = System.Text.Encoding.UTF8.GetString(data);

                AddUploadLog("SUCCESS!");
                AddUploadLog(reply);

                MessageBox.Show("Upload completed", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                AddUploadLog("ERROR. Can not upload");
            }
            finally
            {
                if (th != null && th.IsAlive)
                    th.Abort();

                UlockInterface();
            }
        }

        private int _percentageUploaded;
        private void UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage % 10 == 0 && e.ProgressPercentage > _percentageUploaded)
            {
                _percentageUploaded = e.ProgressPercentage;

                int p = e.ProgressPercentage;

                this.InvokeEx(x =>
                {
                    x.txtUploadLog.Text += System.Environment.NewLine + DateTime.Now.ToString("T") + " - " + p + "%";
                });
            }

            if (_percentageUploaded == 100)
            {
                _percentageUploaded = 0;
                this.InvokeEx(x =>
                {
                    x.txtUploadLog.Text += System.Environment.NewLine + DateTime.Now.ToString("T") + " - " + "Calculating. Please wait ...";
                });
            }
        }

        private void AddUploadLog(string text, bool skipNewLine = false)
        {
            this.InvokeEx(x =>
            {
                x.txtUploadLog.Text += skipNewLine
                    ? DateTime.Now.ToString("T") + " - " + text
                    : System.Environment.NewLine + DateTime.Now.ToString("T") + " - " + text;
            });
        }

        private bool ValidateValues(ApiMethod apiMethod)
        {
            StringBuilder stringBuilder = new StringBuilder();

            switch (apiMethod)
            {
                case ApiMethod.UploadFile:
                {
                    if (string.IsNullOrEmpty(txtFileUpload.Text) || !File.Exists(txtFileUpload.Text))
                        stringBuilder.AppendLine("Excel file not found");

                    if (string.IsNullOrEmpty(txtApi.Text))
                        stringBuilder.AppendLine("Api url is empty");
                }
                break;

                case ApiMethod.AddCustomer:
                {
                    if (string.IsNullOrEmpty(txtApi.Text))
                        stringBuilder.AppendLine("Api url is empty");
                }
                break;
            }

            if (stringBuilder.Length > 0)
            {
                MessageBox.Show(stringBuilder.ToString(), "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ClearLogs()
        {
            this.InvokeEx(x =>
            {
                x.txtUploadLog.Clear();
            });
        }

        private void LockInteraface()
        {
            this.InvokeEx(x =>
            {
                x.btnFileUplad.Enabled = false;
                x.btnUpload.Enabled = false;
            });
        }

        private void UlockInterface()
        {
            this.InvokeEx(x =>
            {
                x.btnFileUplad.Enabled = true;
                x.btnUpload.Enabled = true;
            });
        }

        private void txtClientMoney_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtFileUpload_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateValues(ApiMethod.UploadFile))
                {
                    ClearLogs();
                    LockInteraface();

                    AddUploadLog("Upload started", true);
                    AddUploadLog("Please wait. It may take quite a long time (5 ~ 15 minutes)...");

                    if (th != null && th.IsAlive)
                        th.Abort();

                    th = new Thread(() =>
                    {
                        var client = new WebClient();

                        var uri = new Uri(txtApi.Text.CombineUrl("/api/contactapi/uploadproducts"));
                        var data = System.IO.File.ReadAllBytes(txtFileUpload.Text);

                        _percentageUploaded = 0;

                        client.UploadDataCompleted += new UploadDataCompletedEventHandler(UploadDataCallback);
                        client.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadProgressChanged);
                        client.UploadDataAsync(uri, "POST", data);

                    })
                    { IsBackground = true };
                    th.Start();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
