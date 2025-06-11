namespace Turkcell_Akif_Abi
{
    partial class Raporlar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Raporlar));
            dateTimePickerBaslangic = new DateTimePicker();
            dateTimePickerBitis = new DateTimePicker();
            dataGridViewRapor = new DataGridView();
            button1 = new Button();
            button3 = new Button();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewRapor).BeginInit();
            SuspendLayout();
            // 
            // dateTimePickerBaslangic
            // 
            dateTimePickerBaslangic.Location = new Point(128, 29);
            dateTimePickerBaslangic.Name = "dateTimePickerBaslangic";
            dateTimePickerBaslangic.Size = new Size(200, 23);
            dateTimePickerBaslangic.TabIndex = 0;
            // 
            // dateTimePickerBitis
            // 
            dateTimePickerBitis.Location = new Point(334, 29);
            dateTimePickerBitis.Name = "dateTimePickerBitis";
            dateTimePickerBitis.Size = new Size(200, 23);
            dateTimePickerBitis.TabIndex = 1;
            // 
            // dataGridViewRapor
            // 
            dataGridViewRapor.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRapor.Location = new Point(12, 120);
            dataGridViewRapor.Name = "dataGridViewRapor";
            dataGridViewRapor.Size = new Size(651, 355);
            dataGridViewRapor.TabIndex = 3;
            // 
            // button1
            // 
            button1.Location = new Point(12, 91);
            button1.Name = "button1";
            button1.Size = new Size(213, 23);
            button1.TabIndex = 5;
            button1.Text = "Satış Raporu";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.Location = new Point(231, 91);
            button3.Name = "button3";
            button3.Size = new Size(213, 23);
            button3.TabIndex = 7;
            button3.Text = "Stok Raporu";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(450, 91);
            button4.Name = "button4";
            button4.Size = new Size(213, 23);
            button4.TabIndex = 8;
            button4.Text = "Gelir Gider Raporu";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Raporlar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Sepete_Ekle__10_;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(675, 491);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(dataGridViewRapor);
            Controls.Add(dateTimePickerBitis);
            Controls.Add(dateTimePickerBaslangic);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Raporlar";
            Text = "Raporlar";
            Load += Raporlar_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewRapor).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DateTimePicker dateTimePickerBaslangic;
        private DateTimePicker dateTimePickerBitis;
        private DataGridView dataGridViewRapor;
        private Button button1;
        private Button button3;
        private Button button4;
    }
}