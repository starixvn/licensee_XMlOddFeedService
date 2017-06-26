namespace CouchbaseApp
{
    partial class LogFormcs
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
            this.OldRequestSubmit = new System.Windows.Forms.Button();
            this.RequestTimes = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.OldResultView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.NewRequestSubmit = new System.Windows.Forms.Button();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NewResultView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // OldRequestSubmit
            // 
            this.OldRequestSubmit.Location = new System.Drawing.Point(10, 26);
            this.OldRequestSubmit.Name = "OldRequestSubmit";
            this.OldRequestSubmit.Size = new System.Drawing.Size(75, 23);
            this.OldRequestSubmit.TabIndex = 1;
            this.OldRequestSubmit.Text = "Request";
            this.OldRequestSubmit.UseVisualStyleBackColor = true;
            this.OldRequestSubmit.Click += new System.EventHandler(this.RequestSubmit_Click);
            // 
            // RequestTimes
            // 
            this.RequestTimes.Location = new System.Drawing.Point(103, 19);
            this.RequestTimes.Name = "RequestTimes";
            this.RequestTimes.Size = new System.Drawing.Size(99, 20);
            this.RequestTimes.TabIndex = 2;
            this.RequestTimes.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Number request:";
            // 
            // OldResultView
            // 
            this.OldResultView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader7,
            this.columnHeader8});
            this.OldResultView.Location = new System.Drawing.Point(10, 55);
            this.OldResultView.Name = "OldResultView";
            this.OldResultView.Size = new System.Drawing.Size(584, 446);
            this.OldResultView.TabIndex = 4;
            this.OldResultView.UseCompatibleStateImageBehavior = false;
            this.OldResultView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No.";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "LiveDB";
            this.columnHeader2.Width = 106;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "DeadballDB";
            this.columnHeader3.Width = 186;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OldResultView);
            this.groupBox1.Controls.Add(this.OldRequestSubmit);
            this.groupBox1.Location = new System.Drawing.Point(15, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(610, 508);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Old Code";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.NewResultView);
            this.groupBox2.Controls.Add(this.NewRequestSubmit);
            this.groupBox2.Location = new System.Drawing.Point(643, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(611, 508);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "New Code";
            // 
            // NewRequestSubmit
            // 
            this.NewRequestSubmit.Location = new System.Drawing.Point(11, 23);
            this.NewRequestSubmit.Name = "NewRequestSubmit";
            this.NewRequestSubmit.Size = new System.Drawing.Size(75, 23);
            this.NewRequestSubmit.TabIndex = 1;
            this.NewRequestSubmit.Text = "Request";
            this.NewRequestSubmit.UseVisualStyleBackColor = true;
            this.NewRequestSubmit.Click += new System.EventHandler(this.NewRequestSubmit_Click);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "OutrightDB";
            this.columnHeader7.Width = 124;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "All";
            this.columnHeader8.Width = 100;
            // 
            // NewResultView
            // 
            this.NewResultView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader9,
            this.columnHeader10});
            this.NewResultView.Location = new System.Drawing.Point(11, 52);
            this.NewResultView.Name = "NewResultView";
            this.NewResultView.Size = new System.Drawing.Size(584, 446);
            this.NewResultView.TabIndex = 5;
            this.NewResultView.UseCompatibleStateImageBehavior = false;
            this.NewResultView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "No.";
            this.columnHeader4.Width = 50;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "LiveDB";
            this.columnHeader5.Width = 106;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "DeadballDB";
            this.columnHeader6.Width = 186;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "OutrightDB";
            this.columnHeader9.Width = 124;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "All";
            this.columnHeader10.Width = 100;
            // 
            // LogFormcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 605);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RequestTimes);
            this.Name = "LogFormcs";
            this.Text = "LogFormcs";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OldRequestSubmit;
        private System.Windows.Forms.TextBox RequestTimes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView OldResultView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button NewRequestSubmit;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ListView NewResultView;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
    }
}