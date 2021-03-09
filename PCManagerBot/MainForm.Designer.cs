namespace PCManagerBot
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tBText = new System.Windows.Forms.TextBox();
            this.btnGetlogs = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.tBTgId = new System.Windows.Forms.TextBox();
            this.tBTgToken = new System.Windows.Forms.TextBox();
            this.tGIdLabel = new System.Windows.Forms.Label();
            this.tGTokenLabel = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // tBText
            // 
            this.tBText.Location = new System.Drawing.Point(12, 12);
            this.tBText.Multiline = true;
            this.tBText.Name = "tBText";
            this.tBText.ReadOnly = true;
            this.tBText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tBText.Size = new System.Drawing.Size(860, 480);
            this.tBText.TabIndex = 0;
            // 
            // btnGetlogs
            // 
            this.btnGetlogs.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGetlogs.Location = new System.Drawing.Point(12, 506);
            this.btnGetlogs.Name = "btnGetlogs";
            this.btnGetlogs.Size = new System.Drawing.Size(179, 43);
            this.btnGetlogs.TabIndex = 1;
            this.btnGetlogs.Text = "Get Latest Logs";
            this.btnGetlogs.UseVisualStyleBackColor = true;
            this.btnGetlogs.Click += new System.EventHandler(this.btnGetlogs_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(693, 506);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(179, 43);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel.Controls.Add(this.okBtn);
            this.panel.Controls.Add(this.tGTokenLabel);
            this.panel.Controls.Add(this.tGIdLabel);
            this.panel.Controls.Add(this.tBTgToken);
            this.panel.Controls.Add(this.tBTgId);
            this.panel.Location = new System.Drawing.Point(270, 81);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(358, 132);
            this.panel.TabIndex = 3;
            this.panel.Visible = false;
            // 
            // tBTgId
            // 
            this.tBTgId.Location = new System.Drawing.Point(3, 27);
            this.tBTgId.Name = "tBTgId";
            this.tBTgId.Size = new System.Drawing.Size(352, 20);
            this.tBTgId.TabIndex = 0;
            // 
            // tBTgToken
            // 
            this.tBTgToken.Location = new System.Drawing.Point(3, 69);
            this.tBTgToken.Name = "tBTgToken";
            this.tBTgToken.Size = new System.Drawing.Size(352, 20);
            this.tBTgToken.TabIndex = 1;
            // 
            // tGIdLabel
            // 
            this.tGIdLabel.AutoSize = true;
            this.tGIdLabel.Location = new System.Drawing.Point(3, 11);
            this.tGIdLabel.Name = "tGIdLabel";
            this.tGIdLabel.Size = new System.Drawing.Size(119, 13);
            this.tGIdLabel.TabIndex = 2;
            this.tGIdLabel.Text = "Enter your Telegram ID:";
            // 
            // tGTokenLabel
            // 
            this.tGTokenLabel.AutoSize = true;
            this.tGTokenLabel.Location = new System.Drawing.Point(3, 53);
            this.tGTokenLabel.Name = "tGTokenLabel";
            this.tGTokenLabel.Size = new System.Drawing.Size(161, 13);
            this.tGTokenLabel.TabIndex = 3;
            this.tGTokenLabel.Text = "Enter your Telegram Bot\'s token:";
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(111, 95);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(139, 23);
            this.okBtn.TabIndex = 4;
            this.okBtn.Text = "Ok";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(197, 506);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(191, 43);
            this.button1.TabIndex = 4;
            this.button1.Text = "Change ID and Token";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnGetlogs);
            this.Controls.Add(this.tBText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PC Manager Bot";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TextBox tBText;
        private System.Windows.Forms.Button btnGetlogs;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.TextBox tBTgToken;
        private System.Windows.Forms.TextBox tBTgId;
        private System.Windows.Forms.Label tGTokenLabel;
        private System.Windows.Forms.Label tGIdLabel;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button button1;
    }
}

