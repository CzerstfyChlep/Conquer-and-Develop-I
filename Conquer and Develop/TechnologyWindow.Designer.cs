namespace Conquer_and_Develop
{
    partial class TechnologyWindow
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
            this.components = new System.ComponentModel.Container();
            this.TechPointsLabel = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.MinDamageLabel = new System.Windows.Forms.Label();
            this.MinDamageBar = new System.Windows.Forms.ProgressBar();
            this.MinDamageButton = new System.Windows.Forms.Button();
            this.MaxDamageButton = new System.Windows.Forms.Button();
            this.MaxDamageBar = new System.Windows.Forms.ProgressBar();
            this.MaxDamageLabel = new System.Windows.Forms.Label();
            this.MDefenceButton = new System.Windows.Forms.Button();
            this.MDefenceBar = new System.Windows.Forms.ProgressBar();
            this.MDefenceLabel = new System.Windows.Forms.Label();
            this.RangeDefenceButton = new System.Windows.Forms.Button();
            this.RangeDefenceBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.DodgeButton = new System.Windows.Forms.Button();
            this.DodgeBar = new System.Windows.Forms.ProgressBar();
            this.DodgeLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TechPointsLabel
            // 
            this.TechPointsLabel.AutoSize = true;
            this.TechPointsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TechPointsLabel.Location = new System.Drawing.Point(139, 9);
            this.TechPointsLabel.Name = "TechPointsLabel";
            this.TechPointsLabel.Size = new System.Drawing.Size(132, 25);
            this.TechPointsLabel.TabIndex = 50;
            this.TechPointsLabel.Text = "TechPoints: ";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Pikemen",
            "Infantry",
            "Light Cavalry",
            "Cavalry",
            "Archers"});
            this.comboBox1.Location = new System.Drawing.Point(12, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 51;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // MinDamageLabel
            // 
            this.MinDamageLabel.AutoSize = true;
            this.MinDamageLabel.Location = new System.Drawing.Point(12, 48);
            this.MinDamageLabel.Name = "MinDamageLabel";
            this.MinDamageLabel.Size = new System.Drawing.Size(73, 13);
            this.MinDamageLabel.TabIndex = 52;
            this.MinDamageLabel.Text = "Min. Damage:";
            // 
            // MinDamageBar
            // 
            this.MinDamageBar.Location = new System.Drawing.Point(135, 48);
            this.MinDamageBar.Maximum = 4;
            this.MinDamageBar.Name = "MinDamageBar";
            this.MinDamageBar.Size = new System.Drawing.Size(100, 13);
            this.MinDamageBar.Step = 1;
            this.MinDamageBar.TabIndex = 53;
            // 
            // MinDamageButton
            // 
            this.MinDamageButton.Enabled = false;
            this.MinDamageButton.Location = new System.Drawing.Point(241, 45);
            this.MinDamageButton.Name = "MinDamageButton";
            this.MinDamageButton.Size = new System.Drawing.Size(48, 19);
            this.MinDamageButton.TabIndex = 54;
            this.MinDamageButton.Text = "UP";
            this.MinDamageButton.UseVisualStyleBackColor = true;
            // 
            // MaxDamageButton
            // 
            this.MaxDamageButton.Enabled = false;
            this.MaxDamageButton.Location = new System.Drawing.Point(241, 77);
            this.MaxDamageButton.Name = "MaxDamageButton";
            this.MaxDamageButton.Size = new System.Drawing.Size(48, 19);
            this.MaxDamageButton.TabIndex = 57;
            this.MaxDamageButton.Text = "UP";
            this.MaxDamageButton.UseVisualStyleBackColor = true;
            // 
            // MaxDamageBar
            // 
            this.MaxDamageBar.Location = new System.Drawing.Point(135, 80);
            this.MaxDamageBar.Maximum = 5;
            this.MaxDamageBar.Name = "MaxDamageBar";
            this.MaxDamageBar.Size = new System.Drawing.Size(100, 13);
            this.MaxDamageBar.Step = 1;
            this.MaxDamageBar.TabIndex = 56;
            // 
            // MaxDamageLabel
            // 
            this.MaxDamageLabel.AutoSize = true;
            this.MaxDamageLabel.Location = new System.Drawing.Point(12, 80);
            this.MaxDamageLabel.Name = "MaxDamageLabel";
            this.MaxDamageLabel.Size = new System.Drawing.Size(76, 13);
            this.MaxDamageLabel.TabIndex = 55;
            this.MaxDamageLabel.Text = "Max. Damage:";
            // 
            // MDefenceButton
            // 
            this.MDefenceButton.Enabled = false;
            this.MDefenceButton.Location = new System.Drawing.Point(241, 110);
            this.MDefenceButton.Name = "MDefenceButton";
            this.MDefenceButton.Size = new System.Drawing.Size(48, 19);
            this.MDefenceButton.TabIndex = 60;
            this.MDefenceButton.Text = "UP";
            this.MDefenceButton.UseVisualStyleBackColor = true;
            // 
            // MDefenceBar
            // 
            this.MDefenceBar.Location = new System.Drawing.Point(135, 113);
            this.MDefenceBar.Maximum = 5;
            this.MDefenceBar.Name = "MDefenceBar";
            this.MDefenceBar.Size = new System.Drawing.Size(100, 13);
            this.MDefenceBar.Step = 1;
            this.MDefenceBar.TabIndex = 59;
            // 
            // MDefenceLabel
            // 
            this.MDefenceLabel.AutoSize = true;
            this.MDefenceLabel.Location = new System.Drawing.Point(12, 113);
            this.MDefenceLabel.Name = "MDefenceLabel";
            this.MDefenceLabel.Size = new System.Drawing.Size(83, 13);
            this.MDefenceLabel.TabIndex = 58;
            this.MDefenceLabel.Text = "Melee Defence:";
            this.MDefenceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RangeDefenceButton
            // 
            this.RangeDefenceButton.Enabled = false;
            this.RangeDefenceButton.Location = new System.Drawing.Point(241, 141);
            this.RangeDefenceButton.Name = "RangeDefenceButton";
            this.RangeDefenceButton.Size = new System.Drawing.Size(48, 19);
            this.RangeDefenceButton.TabIndex = 63;
            this.RangeDefenceButton.Text = "UP";
            this.RangeDefenceButton.UseVisualStyleBackColor = true;
            // 
            // RangeDefenceBar
            // 
            this.RangeDefenceBar.Location = new System.Drawing.Point(135, 144);
            this.RangeDefenceBar.Maximum = 5;
            this.RangeDefenceBar.Name = "RangeDefenceBar";
            this.RangeDefenceBar.Size = new System.Drawing.Size(100, 13);
            this.RangeDefenceBar.Step = 1;
            this.RangeDefenceBar.TabIndex = 62;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 61;
            this.label2.Text = "Range Defence:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DodgeButton
            // 
            this.DodgeButton.Enabled = false;
            this.DodgeButton.Location = new System.Drawing.Point(241, 170);
            this.DodgeButton.Name = "DodgeButton";
            this.DodgeButton.Size = new System.Drawing.Size(48, 19);
            this.DodgeButton.TabIndex = 66;
            this.DodgeButton.Text = "UP";
            this.DodgeButton.UseVisualStyleBackColor = true;
            // 
            // DodgeBar
            // 
            this.DodgeBar.Location = new System.Drawing.Point(135, 173);
            this.DodgeBar.Maximum = 5;
            this.DodgeBar.Name = "DodgeBar";
            this.DodgeBar.Size = new System.Drawing.Size(100, 13);
            this.DodgeBar.Step = 1;
            this.DodgeBar.TabIndex = 65;
            // 
            // DodgeLabel
            // 
            this.DodgeLabel.AutoSize = true;
            this.DodgeLabel.Location = new System.Drawing.Point(12, 173);
            this.DodgeLabel.Name = "DodgeLabel";
            this.DodgeLabel.Size = new System.Drawing.Size(42, 13);
            this.DodgeLabel.TabIndex = 64;
            this.DodgeLabel.Text = "Dodge:";
            this.DodgeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(10, 210);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(279, 23);
            this.CloseButton.TabIndex = 67;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // TechnologyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 245);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.DodgeButton);
            this.Controls.Add(this.DodgeBar);
            this.Controls.Add(this.DodgeLabel);
            this.Controls.Add(this.RangeDefenceButton);
            this.Controls.Add(this.RangeDefenceBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MDefenceButton);
            this.Controls.Add(this.MDefenceBar);
            this.Controls.Add(this.MDefenceLabel);
            this.Controls.Add(this.MaxDamageButton);
            this.Controls.Add(this.MaxDamageBar);
            this.Controls.Add(this.MaxDamageLabel);
            this.Controls.Add(this.MinDamageButton);
            this.Controls.Add(this.MinDamageBar);
            this.Controls.Add(this.MinDamageLabel);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.TechPointsLabel);
            this.Name = "TechnologyWindow";
            this.Text = "TechnologyWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label TechPointsLabel;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label MinDamageLabel;
        private System.Windows.Forms.ProgressBar MinDamageBar;
        private System.Windows.Forms.Button MinDamageButton;
        private System.Windows.Forms.Button MaxDamageButton;
        private System.Windows.Forms.ProgressBar MaxDamageBar;
        private System.Windows.Forms.Label MaxDamageLabel;
        private System.Windows.Forms.Button MDefenceButton;
        private System.Windows.Forms.ProgressBar MDefenceBar;
        private System.Windows.Forms.Label MDefenceLabel;
        private System.Windows.Forms.Button RangeDefenceButton;
        private System.Windows.Forms.ProgressBar RangeDefenceBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button DodgeButton;
        private System.Windows.Forms.ProgressBar DodgeBar;
        private System.Windows.Forms.Label DodgeLabel;
        private System.Windows.Forms.Button CloseButton;
    }
}