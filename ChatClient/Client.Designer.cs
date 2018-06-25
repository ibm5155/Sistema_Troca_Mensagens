namespace ChatClient
{
    partial class Client
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.rtxtConversation = new System.Windows.Forms.RichTextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassw = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.OnlineFriends = new System.Windows.Forms.GroupBox();
            this.userslistbox = new System.Windows.Forms.ListBox();
            this.btnellection = new System.Windows.Forms.Button();
            this.txtLider = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.RcQueueSize = new System.Windows.Forms.ProgressBar();
            this.fase2pic = new System.Windows.Forms.PictureBox();
            this.IniciarFaseII = new System.Windows.Forms.Button();
            this.button_faseIII = new System.Windows.Forms.Button();
            this.button_index = new System.Windows.Forms.Button();
            this.siteID = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.OnlineFriends.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fase2pic)).BeginInit();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(75, 27);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(150, 20);
            this.txtName.TabIndex = 1;
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(75, 53);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(150, 20);
            this.txtServerIP.TabIndex = 3;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(231, 27);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(131, 46);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "&Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // rtxtConversation
            // 
            this.rtxtConversation.Location = new System.Drawing.Point(12, 157);
            this.rtxtConversation.Name = "rtxtConversation";
            this.rtxtConversation.ReadOnly = true;
            this.rtxtConversation.Size = new System.Drawing.Size(368, 268);
            this.rtxtConversation.TabIndex = 1;
            this.rtxtConversation.Text = "";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(8, 20);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(262, 57);
            this.txtMessage.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(276, 20);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(86, 57);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "&Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtPassw);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.txtServerIP);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 127);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection Options";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Id:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(75, 108);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(150, 20);
            this.txtId.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Passw:";
            // 
            // txtPassw
            // 
            this.txtPassw.Location = new System.Drawing.Point(75, 82);
            this.txtPassw.Name = "txtPassw";
            this.txtPassw.Size = new System.Drawing.Size(150, 20);
            this.txtPassw.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Server IP:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtMessage);
            this.groupBox2.Controls.Add(this.btnSend);
            this.groupBox2.Location = new System.Drawing.Point(12, 445);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(368, 89);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Work";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(288, 541);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(86, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "E&xit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.siteID);
            this.groupBox3.Controls.Add(this.button_index);
            this.groupBox3.Controls.Add(this.button_faseIII);
            this.groupBox3.Location = new System.Drawing.Point(389, 520);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(287, 79);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Fase III";
            // 
            // OnlineFriends
            // 
            this.OnlineFriends.Controls.Add(this.userslistbox);
            this.OnlineFriends.Location = new System.Drawing.Point(395, 12);
            this.OnlineFriends.Name = "OnlineFriends";
            this.OnlineFriends.Size = new System.Drawing.Size(269, 365);
            this.OnlineFriends.TabIndex = 5;
            this.OnlineFriends.TabStop = false;
            this.OnlineFriends.Text = "Online Friends";
            // 
            // userslistbox
            // 
            this.userslistbox.FormattingEnabled = true;
            this.userslistbox.Location = new System.Drawing.Point(6, 27);
            this.userslistbox.Name = "userslistbox";
            this.userslistbox.Size = new System.Drawing.Size(257, 329);
            this.userslistbox.TabIndex = 0;
            this.userslistbox.Click += new System.EventHandler(this.userslistbox_Click);
            // 
            // btnellection
            // 
            this.btnellection.Location = new System.Drawing.Point(243, 97);
            this.btnellection.Name = "btnellection";
            this.btnellection.Size = new System.Drawing.Size(131, 46);
            this.btnellection.TabIndex = 9;
            this.btnellection.Text = "Requisitar Eleição";
            this.btnellection.UseVisualStyleBackColor = true;
            this.btnellection.Click += new System.EventHandler(this.ElectionRequested);
            // 
            // txtLider
            // 
            this.txtLider.AutoSize = true;
            this.txtLider.Location = new System.Drawing.Point(45, 541);
            this.txtLider.Name = "txtLider";
            this.txtLider.Size = new System.Drawing.Size(46, 13);
            this.txtLider.TabIndex = 10;
            this.txtLider.Text = "Escravo";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.RcQueueSize);
            this.groupBox4.Controls.Add(this.fase2pic);
            this.groupBox4.Controls.Add(this.IniciarFaseII);
            this.groupBox4.Location = new System.Drawing.Point(401, 401);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(287, 97);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Região Critica";
            // 
            // RcQueueSize
            // 
            this.RcQueueSize.Location = new System.Drawing.Point(117, 79);
            this.RcQueueSize.Name = "RcQueueSize";
            this.RcQueueSize.Size = new System.Drawing.Size(140, 14);
            this.RcQueueSize.TabIndex = 4;
            // 
            // fase2pic
            // 
            this.fase2pic.Image = global::ChatClient.Properties.Resources.SII_Idle;
            this.fase2pic.Location = new System.Drawing.Point(6, 14);
            this.fase2pic.Name = "fase2pic";
            this.fase2pic.Size = new System.Drawing.Size(91, 77);
            this.fase2pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.fase2pic.TabIndex = 3;
            this.fase2pic.TabStop = false;
            // 
            // IniciarFaseII
            // 
            this.IniciarFaseII.Location = new System.Drawing.Point(171, 19);
            this.IniciarFaseII.Name = "IniciarFaseII";
            this.IniciarFaseII.Size = new System.Drawing.Size(86, 57);
            this.IniciarFaseII.TabIndex = 2;
            this.IniciarFaseII.Text = "&Start";
            this.IniciarFaseII.UseVisualStyleBackColor = true;
            this.IniciarFaseII.Click += new System.EventHandler(this.IniciarFaseII_Click);
            // 
            // button_faseIII
            // 
            this.button_faseIII.Location = new System.Drawing.Point(183, 19);
            this.button_faseIII.Name = "button_faseIII";
            this.button_faseIII.Size = new System.Drawing.Size(86, 57);
            this.button_faseIII.TabIndex = 5;
            this.button_faseIII.Text = "&Start";
            this.button_faseIII.UseVisualStyleBackColor = true;
            this.button_faseIII.Click += new System.EventHandler(this.button_faseIII_Click);
            // 
            // button_index
            // 
            this.button_index.Location = new System.Drawing.Point(75, 19);
            this.button_index.Name = "button_index";
            this.button_index.Size = new System.Drawing.Size(86, 57);
            this.button_index.TabIndex = 6;
            this.button_index.Text = "&Get Index";
            this.button_index.UseVisualStyleBackColor = true;
            this.button_index.Click += new System.EventHandler(this.button_index_Click);
            // 
            // siteID
            // 
            this.siteID.Location = new System.Drawing.Point(6, 38);
            this.siteID.Name = "siteID";
            this.siteID.Size = new System.Drawing.Size(47, 20);
            this.siteID.TabIndex = 7;
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 611);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.txtLider);
            this.Controls.Add(this.btnellection);
            this.Controls.Add(this.OnlineFriends);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rtxtConversation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Client";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_FormClosing);
            this.Load += new System.EventHandler(this.Client_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.OnlineFriends.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fase2pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.RichTextBox rtxtConversation;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox OnlineFriends;
        private System.Windows.Forms.ListBox userslistbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassw;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Button btnellection;
        private System.Windows.Forms.Label txtLider;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button IniciarFaseII;
        private System.Windows.Forms.PictureBox fase2pic;
        private System.Windows.Forms.ProgressBar RcQueueSize;
        private System.Windows.Forms.Button button_faseIII;
        private System.Windows.Forms.Button button_index;
        private System.Windows.Forms.TextBox siteID;
    }
}

