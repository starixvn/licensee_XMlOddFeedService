namespace CouchbaseApp
{
    partial class Form1
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
            this.btnVersionFullChain = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnMatchOutright = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbVersionFullchain = new System.Windows.Forms.ComboBox();
            this.cbMatchOutright = new System.Windows.Forms.ComboBox();
            this.cbNumberVersion = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnVersionFullChain
            // 
            this.btnVersionFullChain.Location = new System.Drawing.Point(369, 24);
            this.btnVersionFullChain.Name = "btnVersionFullChain";
            this.btnVersionFullChain.Size = new System.Drawing.Size(75, 23);
            this.btnVersionFullChain.TabIndex = 0;
            this.btnVersionFullChain.Text = "Search";
            this.btnVersionFullChain.UseVisualStyleBackColor = true;
            this.btnVersionFullChain.Click += new System.EventHandler(this.btnVersionFullChain_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-2, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Version/FullChain Key";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Match/Outright Key";
            // 
            // btnMatchOutright
            // 
            this.btnMatchOutright.Location = new System.Drawing.Point(507, 52);
            this.btnMatchOutright.Name = "btnMatchOutright";
            this.btnMatchOutright.Size = new System.Drawing.Size(75, 23);
            this.btnMatchOutright.TabIndex = 3;
            this.btnMatchOutright.Text = "Search";
            this.btnMatchOutright.UseVisualStyleBackColor = true;
            this.btnMatchOutright.Click += new System.EventHandler(this.btnMatchOutright_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(122, 85);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(1189, 696);
            this.textBox1.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(71, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Result";
            // 
            // cbVersionFullchain
            // 
            this.cbVersionFullchain.FormattingEnabled = true;
            this.cbVersionFullchain.Items.AddRange(new object[] {
            "apollocurrent",
            "apollofullchain"});
            this.cbVersionFullchain.Location = new System.Drawing.Point(122, 25);
            this.cbVersionFullchain.Name = "cbVersionFullchain";
            this.cbVersionFullchain.Size = new System.Drawing.Size(241, 21);
            this.cbVersionFullchain.TabIndex = 8;
            // 
            // cbMatchOutright
            // 
            this.cbMatchOutright.FormattingEnabled = true;
            this.cbMatchOutright.Items.AddRange(new object[] {
            "apollolivematchs",
            "apollodeadballmatches",
            "apolloevents"});
            this.cbMatchOutright.Location = new System.Drawing.Point(122, 52);
            this.cbMatchOutright.Name = "cbMatchOutright";
            this.cbMatchOutright.Size = new System.Drawing.Size(241, 21);
            this.cbMatchOutright.TabIndex = 9;
            // 
            // cbNumberVersion
            // 
            this.cbNumberVersion.FormattingEnabled = true;
            this.cbNumberVersion.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59",
            "60"});
            this.cbNumberVersion.Location = new System.Drawing.Point(369, 52);
            this.cbNumberVersion.Name = "cbNumberVersion";
            this.cbNumberVersion.Size = new System.Drawing.Size(121, 21);
            this.cbNumberVersion.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1378, 806);
            this.Controls.Add(this.cbNumberVersion);
            this.Controls.Add(this.cbMatchOutright);
            this.Controls.Add(this.cbVersionFullchain);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnMatchOutright);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnVersionFullChain);
            this.Name = "Form1";
            this.Text = "View Couchbase Data";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnVersionFullChain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMatchOutright;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbVersionFullchain;
        private System.Windows.Forms.ComboBox cbMatchOutright;
        private System.Windows.Forms.ComboBox cbNumberVersion;
    }
}

