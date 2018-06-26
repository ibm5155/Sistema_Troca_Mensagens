namespace ServerProcess
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.rtxtConversation = new System.Windows.Forms.RichTextBox();
            this.button_leader = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(164, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Iniciar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rtxtConversation
            // 
            this.rtxtConversation.Location = new System.Drawing.Point(12, 45);
            this.rtxtConversation.Name = "rtxtConversation";
            this.rtxtConversation.ReadOnly = true;
            this.rtxtConversation.Size = new System.Drawing.Size(609, 368);
            this.rtxtConversation.TabIndex = 3;
            this.rtxtConversation.Text = "";
            // 
            // button_leader
            // 
            this.button_leader.Location = new System.Drawing.Point(263, 6);
            this.button_leader.Name = "button_leader";
            this.button_leader.Size = new System.Drawing.Size(75, 23);
            this.button_leader.TabIndex = 4;
            this.button_leader.Text = "Tornar Lider";
            this.button_leader.UseVisualStyleBackColor = true;
            this.button_leader.Click += new System.EventHandler(this.button_leader_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 487);
            this.Controls.Add(this.button_leader);
            this.Controls.Add(this.rtxtConversation);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Site_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox rtxtConversation;
        private System.Windows.Forms.Button button_leader;
    }
}

