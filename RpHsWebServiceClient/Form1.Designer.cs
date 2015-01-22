namespace HSControl
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
            this.buttonArm = new System.Windows.Forms.Button();
            this.checkBoxArmed = new System.Windows.Forms.CheckBox();
            this.labelResponse = new System.Windows.Forms.Label();
            this.buttonGetArmed = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonArm
            // 
            this.buttonArm.Location = new System.Drawing.Point(49, 46);
            this.buttonArm.Name = "buttonArm";
            this.buttonArm.Size = new System.Drawing.Size(75, 23);
            this.buttonArm.TabIndex = 0;
            this.buttonArm.Text = "Arm";
            this.buttonArm.UseVisualStyleBackColor = true;
            this.buttonArm.Click += new System.EventHandler(this.buttonArm_Click);
            // 
            // checkBoxArmed
            // 
            this.checkBoxArmed.AutoSize = true;
            this.checkBoxArmed.Location = new System.Drawing.Point(148, 46);
            this.checkBoxArmed.Name = "checkBoxArmed";
            this.checkBoxArmed.Size = new System.Drawing.Size(56, 17);
            this.checkBoxArmed.TabIndex = 1;
            this.checkBoxArmed.Text = "Armed";
            this.checkBoxArmed.UseVisualStyleBackColor = true;
            // 
            // labelResponse
            // 
            this.labelResponse.BackColor = System.Drawing.SystemColors.Info;
            this.labelResponse.Location = new System.Drawing.Point(49, 94);
            this.labelResponse.Name = "labelResponse";
            this.labelResponse.Size = new System.Drawing.Size(139, 120);
            this.labelResponse.TabIndex = 2;
            // 
            // buttonGetArmed
            // 
            this.buttonGetArmed.Location = new System.Drawing.Point(49, 76);
            this.buttonGetArmed.Name = "buttonGetArmed";
            this.buttonGetArmed.Size = new System.Drawing.Size(75, 23);
            this.buttonGetArmed.TabIndex = 3;
            this.buttonGetArmed.Text = "GetArmed";
            this.buttonGetArmed.UseVisualStyleBackColor = true;
            this.buttonGetArmed.Click += new System.EventHandler(this.buttonGetArmed_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.buttonGetArmed);
            this.Controls.Add(this.labelResponse);
            this.Controls.Add(this.checkBoxArmed);
            this.Controls.Add(this.buttonArm);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonArm;
        private System.Windows.Forms.CheckBox checkBoxArmed;
        private System.Windows.Forms.Label labelResponse;
        private System.Windows.Forms.Button buttonGetArmed;
    }
}

