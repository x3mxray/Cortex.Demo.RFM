namespace Demo.Project.DemoDataExplorer
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.statusExplorer = new System.Windows.Forms.StatusStrip();
            this.tabAPI = new System.Windows.Forms.TabControl();
            this.tabLoader = new System.Windows.Forms.TabPage();
            this.lblUpladFileDesc = new System.Windows.Forms.Label();
            this.txtUploadLog = new System.Windows.Forms.TextBox();
            this.btnUpload = new System.Windows.Forms.Button();
            this.tabApiTester = new System.Windows.Forms.TabPage();
            this.txtClientActivityLog = new System.Windows.Forms.TextBox();
            this.grbClient = new System.Windows.Forms.GroupBox();
            this.lblClientMoney = new System.Windows.Forms.Label();
            this.txtClientMoney = new System.Windows.Forms.TextBox();
            this.chkClientOrder = new System.Windows.Forms.CheckBox();
            this.lblClientOrdersCount = new System.Windows.Forms.Label();
            this.numClientOrders = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddCustomer = new System.Windows.Forms.Button();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.grbSettings = new System.Windows.Forms.GroupBox();
            this.btnFileUplad = new System.Windows.Forms.Button();
            this.txtFileUpload = new System.Windows.Forms.TextBox();
            this.lblFileUpload = new System.Windows.Forms.Label();
            this.txtApi = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tabAPI.SuspendLayout();
            this.tabLoader.SuspendLayout();
            this.tabApiTester.SuspendLayout();
            this.grbClient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClientOrders)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.grbSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusExplorer
            // 
            this.statusExplorer.Location = new System.Drawing.Point(0, 439);
            this.statusExplorer.Name = "statusExplorer";
            this.statusExplorer.Size = new System.Drawing.Size(684, 22);
            this.statusExplorer.TabIndex = 0;
            this.statusExplorer.Text = "status";
            // 
            // tabAPI
            // 
            this.tabAPI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabAPI.Controls.Add(this.tabLoader);
            this.tabAPI.Controls.Add(this.tabApiTester);
            this.tabAPI.Location = new System.Drawing.Point(12, 102);
            this.tabAPI.Name = "tabAPI";
            this.tabAPI.SelectedIndex = 0;
            this.tabAPI.Size = new System.Drawing.Size(660, 323);
            this.tabAPI.TabIndex = 0;
            // 
            // tabLoader
            // 
            this.tabLoader.Controls.Add(this.lblUpladFileDesc);
            this.tabLoader.Controls.Add(this.txtUploadLog);
            this.tabLoader.Controls.Add(this.btnUpload);
            this.tabLoader.Location = new System.Drawing.Point(4, 22);
            this.tabLoader.Name = "tabLoader";
            this.tabLoader.Padding = new System.Windows.Forms.Padding(3);
            this.tabLoader.Size = new System.Drawing.Size(652, 297);
            this.tabLoader.TabIndex = 0;
            this.tabLoader.Text = "History import";
            this.tabLoader.UseVisualStyleBackColor = true;
            // 
            // lblUpladFileDesc
            // 
            this.lblUpladFileDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUpladFileDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpladFileDesc.Location = new System.Drawing.Point(6, 8);
            this.lblUpladFileDesc.Name = "lblUpladFileDesc";
            this.lblUpladFileDesc.Size = new System.Drawing.Size(640, 66);
            this.lblUpladFileDesc.TabIndex = 2;
            this.lblUpladFileDesc.Text = resources.GetString("lblUpladFileDesc.Text");
            // 
            // txtUploadLog
            // 
            this.txtUploadLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUploadLog.Location = new System.Drawing.Point(7, 77);
            this.txtUploadLog.Multiline = true;
            this.txtUploadLog.Name = "txtUploadLog";
            this.txtUploadLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUploadLog.Size = new System.Drawing.Size(639, 185);
            this.txtUploadLog.TabIndex = 0;
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpload.Location = new System.Drawing.Point(465, 268);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(181, 23);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "&Upload file";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // tabApiTester
            // 
            this.tabApiTester.Controls.Add(this.txtClientActivityLog);
            this.tabApiTester.Controls.Add(this.grbClient);
            this.tabApiTester.Controls.Add(this.label1);
            this.tabApiTester.Controls.Add(this.btnAddCustomer);
            this.tabApiTester.Location = new System.Drawing.Point(4, 22);
            this.tabApiTester.Name = "tabApiTester";
            this.tabApiTester.Padding = new System.Windows.Forms.Padding(3);
            this.tabApiTester.Size = new System.Drawing.Size(652, 297);
            this.tabApiTester.TabIndex = 1;
            this.tabApiTester.Text = "New customer";
            this.tabApiTester.UseVisualStyleBackColor = true;
            // 
            // txtClientActivityLog
            // 
            this.txtClientActivityLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClientActivityLog.Location = new System.Drawing.Point(9, 179);
            this.txtClientActivityLog.Multiline = true;
            this.txtClientActivityLog.Name = "txtClientActivityLog";
            this.txtClientActivityLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtClientActivityLog.Size = new System.Drawing.Size(636, 83);
            this.txtClientActivityLog.TabIndex = 13;
            // 
            // grbClient
            // 
            this.grbClient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbClient.Controls.Add(this.lblClientMoney);
            this.grbClient.Controls.Add(this.txtClientMoney);
            this.grbClient.Controls.Add(this.chkClientOrder);
            this.grbClient.Controls.Add(this.lblClientOrdersCount);
            this.grbClient.Controls.Add(this.numClientOrders);
            this.grbClient.Location = new System.Drawing.Point(9, 75);
            this.grbClient.Name = "grbClient";
            this.grbClient.Size = new System.Drawing.Size(636, 98);
            this.grbClient.TabIndex = 12;
            this.grbClient.TabStop = false;
            this.grbClient.Text = "Client data";
            // 
            // lblClientMoney
            // 
            this.lblClientMoney.AutoSize = true;
            this.lblClientMoney.Location = new System.Drawing.Point(14, 51);
            this.lblClientMoney.Name = "lblClientMoney";
            this.lblClientMoney.Size = new System.Drawing.Size(94, 13);
            this.lblClientMoney.TabIndex = 15;
            this.lblClientMoney.Text = "Total spent money";
            // 
            // txtClientMoney
            // 
            this.txtClientMoney.Location = new System.Drawing.Point(114, 48);
            this.txtClientMoney.MaxLength = 6;
            this.txtClientMoney.Name = "txtClientMoney";
            this.txtClientMoney.Size = new System.Drawing.Size(100, 20);
            this.txtClientMoney.TabIndex = 14;
            // 
            // chkClientOrder
            // 
            this.chkClientOrder.AutoSize = true;
            this.chkClientOrder.Location = new System.Drawing.Point(114, 74);
            this.chkClientOrder.Name = "chkClientOrder";
            this.chkClientOrder.Size = new System.Drawing.Size(238, 17);
            this.chkClientOrder.TabIndex = 13;
            this.chkClientOrder.Text = "Purchases are stretched all the time in history";
            this.chkClientOrder.UseVisualStyleBackColor = true;
            // 
            // lblClientOrdersCount
            // 
            this.lblClientOrdersCount.AutoSize = true;
            this.lblClientOrdersCount.Location = new System.Drawing.Point(43, 24);
            this.lblClientOrdersCount.Name = "lblClientOrdersCount";
            this.lblClientOrdersCount.Size = new System.Drawing.Size(63, 13);
            this.lblClientOrdersCount.TabIndex = 12;
            this.lblClientOrdersCount.Text = "Order count";
            // 
            // numClientOrders
            // 
            this.numClientOrders.Location = new System.Drawing.Point(114, 22);
            this.numClientOrders.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numClientOrders.Name = "numClientOrders";
            this.numClientOrders.Size = new System.Drawing.Size(43, 20);
            this.numClientOrders.TabIndex = 11;
            this.numClientOrders.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(639, 66);
            this.label1.TabIndex = 11;
            this.label1.Text = "Specify the number of purchases and the maximum amount of money the customer has " +
    "spent. Click on the checkbox if you want the purchases to be made evenly through" +
    "out the history.";
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddCustomer.Location = new System.Drawing.Point(516, 268);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(130, 23);
            this.btnAddCustomer.TabIndex = 0;
            this.btnAddCustomer.Text = "Add new client";
            this.btnAddCustomer.UseVisualStyleBackColor = true;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.grbSettings);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(684, 96);
            this.pnlSettings.TabIndex = 2;
            // 
            // grbSettings
            // 
            this.grbSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbSettings.Controls.Add(this.btnFileUplad);
            this.grbSettings.Controls.Add(this.txtFileUpload);
            this.grbSettings.Controls.Add(this.lblFileUpload);
            this.grbSettings.Controls.Add(this.txtApi);
            this.grbSettings.Controls.Add(this.lblServer);
            this.grbSettings.Location = new System.Drawing.Point(12, 5);
            this.grbSettings.Name = "grbSettings";
            this.grbSettings.Size = new System.Drawing.Size(656, 82);
            this.grbSettings.TabIndex = 0;
            this.grbSettings.TabStop = false;
            this.grbSettings.Text = "Settings";
            // 
            // btnFileUplad
            // 
            this.btnFileUplad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileUplad.Location = new System.Drawing.Point(469, 47);
            this.btnFileUplad.Name = "btnFileUplad";
            this.btnFileUplad.Size = new System.Drawing.Size(75, 23);
            this.btnFileUplad.TabIndex = 3;
            this.btnFileUplad.Text = "&Browse";
            this.btnFileUplad.UseVisualStyleBackColor = true;
            this.btnFileUplad.Click += new System.EventHandler(this.btnFileUplad_Click);
            // 
            // txtFileUpload
            // 
            this.txtFileUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileUpload.Location = new System.Drawing.Point(77, 48);
            this.txtFileUpload.Name = "txtFileUpload";
            this.txtFileUpload.ReadOnly = true;
            this.txtFileUpload.Size = new System.Drawing.Size(386, 20);
            this.txtFileUpload.TabIndex = 2;
            this.txtFileUpload.TextChanged += new System.EventHandler(this.txtFileUpload_TextChanged);
            // 
            // lblFileUpload
            // 
            this.lblFileUpload.AutoSize = true;
            this.lblFileUpload.Location = new System.Drawing.Point(48, 51);
            this.lblFileUpload.Name = "lblFileUpload";
            this.lblFileUpload.Size = new System.Drawing.Size(23, 13);
            this.lblFileUpload.TabIndex = 2;
            this.lblFileUpload.Text = "File";
            // 
            // txtApi
            // 
            this.txtApi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtApi.Location = new System.Drawing.Point(77, 17);
            this.txtApi.Name = "txtApi";
            this.txtApi.Size = new System.Drawing.Size(386, 20);
            this.txtApi.TabIndex = 0;
            this.txtApi.Text = "http://sc91.sc";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(7, 20);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(64, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "API address";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.tabAPI);
            this.Controls.Add(this.statusExplorer);
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "FormMain";
            this.Text = "Demo Data Explorer";
            this.tabAPI.ResumeLayout(false);
            this.tabLoader.ResumeLayout(false);
            this.tabLoader.PerformLayout();
            this.tabApiTester.ResumeLayout(false);
            this.tabApiTester.PerformLayout();
            this.grbClient.ResumeLayout(false);
            this.grbClient.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numClientOrders)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.grbSettings.ResumeLayout(false);
            this.grbSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusExplorer;
        private System.Windows.Forms.TabControl tabAPI;
        private System.Windows.Forms.TabPage tabLoader;
        private System.Windows.Forms.TabPage tabApiTester;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.GroupBox grbSettings;
        private System.Windows.Forms.Label lblFileUpload;
        private System.Windows.Forms.TextBox txtApi;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Button btnFileUplad;
        private System.Windows.Forms.TextBox txtFileUpload;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TextBox txtUploadLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label lblUpladFileDesc;
        private System.Windows.Forms.Button btnAddCustomer;
        private System.Windows.Forms.TextBox txtClientActivityLog;
        private System.Windows.Forms.GroupBox grbClient;
        private System.Windows.Forms.Label lblClientMoney;
        private System.Windows.Forms.TextBox txtClientMoney;
        private System.Windows.Forms.CheckBox chkClientOrder;
        private System.Windows.Forms.Label lblClientOrdersCount;
        private System.Windows.Forms.NumericUpDown numClientOrders;
        private System.Windows.Forms.Label label1;
    }
}

