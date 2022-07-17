namespace krkrFgiEditor
{
    partial class MainWin
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
            this.openFile = new System.Windows.Forms.Button();
            this.fgiBox = new System.Windows.Forms.PictureBox();
            this.filePath = new System.Windows.Forms.TextBox();
            this.openPanel = new System.Windows.Forms.GroupBox();
            this.encodingLabel = new System.Windows.Forms.Label();
            this.encodingBox = new System.Windows.Forms.ComboBox();
            this.detectEncoding = new System.Windows.Forms.Button();
            this.layerPanel = new System.Windows.Forms.GroupBox();
            this.autoSort = new System.Windows.Forms.CheckBox();
            this.selectedLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.selectedBox = new krkrFgiEditor.ListBoxEx();
            this.layerBox = new krkrFgiEditor.ComboBoxEx();
            this.layerLabel = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.ComboBox();
            this.groupLabel = new System.Windows.Forms.Label();
            this.batchButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.selectedBoxTip = new System.Windows.Forms.ToolTip(this.components);
            this.savePanel = new System.Windows.Forms.Panel();
            this.fgiBoxTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.fgiBox)).BeginInit();
            this.openPanel.SuspendLayout();
            this.layerPanel.SuspendLayout();
            this.savePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFile
            // 
            this.openFile.Location = new System.Drawing.Point(6, 30);
            this.openFile.Name = "openFile";
            this.openFile.Size = new System.Drawing.Size(120, 35);
            this.openFile.TabIndex = 0;
            this.openFile.Text = "打开文件";
            this.openFile.UseVisualStyleBackColor = true;
            this.openFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // fgiBox
            // 
            this.fgiBox.BackColor = System.Drawing.SystemColors.Control;
            this.fgiBox.Enabled = false;
            this.fgiBox.Location = new System.Drawing.Point(500, 12);
            this.fgiBox.Name = "fgiBox";
            this.fgiBox.Size = new System.Drawing.Size(385, 544);
            this.fgiBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.fgiBox.TabIndex = 1;
            this.fgiBox.TabStop = false;
            this.fgiBox.Click += new System.EventHandler(this.FgiBox_Click);
            // 
            // filePath
            // 
            this.filePath.Location = new System.Drawing.Point(132, 32);
            this.filePath.Name = "filePath";
            this.filePath.Size = new System.Drawing.Size(335, 31);
            this.filePath.TabIndex = 1;
            this.filePath.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FilePath_KeyPress);
            // 
            // openPanel
            // 
            this.openPanel.Controls.Add(this.encodingLabel);
            this.openPanel.Controls.Add(this.encodingBox);
            this.openPanel.Controls.Add(this.detectEncoding);
            this.openPanel.Controls.Add(this.filePath);
            this.openPanel.Controls.Add(this.openFile);
            this.openPanel.Location = new System.Drawing.Point(12, 12);
            this.openPanel.Name = "openPanel";
            this.openPanel.Size = new System.Drawing.Size(473, 122);
            this.openPanel.TabIndex = 0;
            this.openPanel.TabStop = false;
            this.openPanel.Text = "坐标文件";
            // 
            // encodingLabel
            // 
            this.encodingLabel.AutoSize = true;
            this.encodingLabel.Enabled = false;
            this.encodingLabel.Location = new System.Drawing.Point(25, 74);
            this.encodingLabel.Name = "encodingLabel";
            this.encodingLabel.Size = new System.Drawing.Size(82, 24);
            this.encodingLabel.TabIndex = 2;
            this.encodingLabel.Text = "编码格式";
            // 
            // encodingBox
            // 
            this.encodingBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encodingBox.Enabled = false;
            this.encodingBox.FormattingEnabled = true;
            this.encodingBox.Items.AddRange(new object[] {
            "Shift JIS",
            "UTF-8",
            "UTF-16 LE"});
            this.encodingBox.Location = new System.Drawing.Point(132, 70);
            this.encodingBox.Name = "encodingBox";
            this.encodingBox.Size = new System.Drawing.Size(209, 32);
            this.encodingBox.TabIndex = 3;
            this.encodingBox.SelectedIndexChanged += new System.EventHandler(this.EncodingBox_SelectedIndexChanged);
            // 
            // detectEncoding
            // 
            this.detectEncoding.Enabled = false;
            this.detectEncoding.Location = new System.Drawing.Point(347, 69);
            this.detectEncoding.Name = "detectEncoding";
            this.detectEncoding.Size = new System.Drawing.Size(120, 35);
            this.detectEncoding.TabIndex = 4;
            this.detectEncoding.Text = "自动检测";
            this.detectEncoding.UseVisualStyleBackColor = true;
            this.detectEncoding.Click += new System.EventHandler(this.DetectEncoding_Click);
            // 
            // layerPanel
            // 
            this.layerPanel.Controls.Add(this.autoSort);
            this.layerPanel.Controls.Add(this.selectedLabel);
            this.layerPanel.Controls.Add(this.cancelButton);
            this.layerPanel.Controls.Add(this.downButton);
            this.layerPanel.Controls.Add(this.upButton);
            this.layerPanel.Controls.Add(this.deleteButton);
            this.layerPanel.Controls.Add(this.addButton);
            this.layerPanel.Controls.Add(this.selectedBox);
            this.layerPanel.Controls.Add(this.layerBox);
            this.layerPanel.Controls.Add(this.layerLabel);
            this.layerPanel.Controls.Add(this.groupBox);
            this.layerPanel.Controls.Add(this.groupLabel);
            this.layerPanel.Enabled = false;
            this.layerPanel.Location = new System.Drawing.Point(12, 140);
            this.layerPanel.Name = "layerPanel";
            this.layerPanel.Size = new System.Drawing.Size(473, 364);
            this.layerPanel.TabIndex = 1;
            this.layerPanel.TabStop = false;
            this.layerPanel.Text = "图层选择";
            // 
            // autoSort
            // 
            this.autoSort.AutoSize = true;
            this.autoSort.Checked = true;
            this.autoSort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSort.Location = new System.Drawing.Point(132, 110);
            this.autoSort.Name = "autoSort";
            this.autoSort.Size = new System.Drawing.Size(108, 28);
            this.autoSort.TabIndex = 4;
            this.autoSort.Text = "自动排序";
            this.autoSort.UseVisualStyleBackColor = true;
            this.autoSort.CheckedChanged += new System.EventHandler(this.AutoSort_CheckedChanged);
            // 
            // selectedLabel
            // 
            this.selectedLabel.AutoSize = true;
            this.selectedLabel.Location = new System.Drawing.Point(25, 210);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(82, 24);
            this.selectedLabel.TabIndex = 7;
            this.selectedLabel.Text = "当前图层";
            // 
            // cancelButton
            // 
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(249, 107);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 35);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // downButton
            // 
            this.downButton.Enabled = false;
            this.downButton.Location = new System.Drawing.Point(249, 308);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(100, 35);
            this.downButton.TabIndex = 10;
            this.downButton.Text = "下移";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // upButton
            // 
            this.upButton.Enabled = false;
            this.upButton.Location = new System.Drawing.Point(132, 308);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(100, 35);
            this.upButton.TabIndex = 9;
            this.upButton.Text = "上移";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(367, 308);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(100, 35);
            this.deleteButton.TabIndex = 11;
            this.deleteButton.Text = "删除";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(367, 107);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(100, 35);
            this.addButton.TabIndex = 6;
            this.addButton.Text = "添加";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // selectedBox
            // 
            this.selectedBox.AllowDrop = true;
            this.selectedBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.selectedBox.FormattingEnabled = true;
            this.selectedBox.ItemHeight = 25;
            this.selectedBox.Location = new System.Drawing.Point(132, 148);
            this.selectedBox.Name = "selectedBox";
            this.selectedBox.Size = new System.Drawing.Size(335, 154);
            this.selectedBox.TabIndex = 8;
            this.selectedBox.ItemsChanged += new System.EventHandler(this.SelectedBox_ItemsChanged);
            this.selectedBox.SelectedIndexChanged += new System.EventHandler(this.SelectedBox_SelectedIndexChanged);
            this.selectedBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SelectedBox_KeyPress);
            // 
            // layerBox
            // 
            this.layerBox.BackColor = System.Drawing.SystemColors.Window;
            this.layerBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.layerBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.layerBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.layerBox.Location = new System.Drawing.Point(132, 69);
            this.layerBox.MaxDropDownItems = 40;
            this.layerBox.Name = "layerBox";
            this.layerBox.Size = new System.Drawing.Size(335, 32);
            this.layerBox.TabIndex = 3;
            this.layerBox.HoveredIndexChanged += new System.EventHandler(this.Layer_HoveredIndexChanged);
            this.layerBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LayerBox_DrawItem);
            this.layerBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.LayerBox_MeasureItem);
            this.layerBox.SelectedIndexChanged += new System.EventHandler(this.LayerBox_SelectedIndexChanged);
            // 
            // layerLabel
            // 
            this.layerLabel.AutoSize = true;
            this.layerLabel.Location = new System.Drawing.Point(43, 73);
            this.layerLabel.Name = "layerLabel";
            this.layerLabel.Size = new System.Drawing.Size(46, 24);
            this.layerLabel.TabIndex = 2;
            this.layerLabel.Text = "图层";
            // 
            // groupBox
            // 
            this.groupBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox.FormattingEnabled = true;
            this.groupBox.Location = new System.Drawing.Point(132, 31);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(335, 33);
            this.groupBox.TabIndex = 1;
            this.groupBox.SelectedIndexChanged += new System.EventHandler(this.GroupBox_SelectedIndexChanged);
            // 
            // groupLabel
            // 
            this.groupLabel.AutoSize = true;
            this.groupLabel.Location = new System.Drawing.Point(34, 35);
            this.groupLabel.Name = "groupLabel";
            this.groupLabel.Size = new System.Drawing.Size(64, 24);
            this.groupLabel.TabIndex = 0;
            this.groupLabel.Text = "图层组";
            // 
            // batchButton
            // 
            this.batchButton.Enabled = false;
            this.batchButton.Location = new System.Drawing.Point(209, 3);
            this.batchButton.Name = "batchButton";
            this.batchButton.Size = new System.Drawing.Size(150, 35);
            this.batchButton.TabIndex = 1;
            this.batchButton.Text = "批量合成";
            this.batchButton.UseVisualStyleBackColor = true;
            this.batchButton.Click += new System.EventHandler(this.BatchButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(3, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(150, 35);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "保存结果";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // savePanel
            // 
            this.savePanel.Controls.Add(this.saveButton);
            this.savePanel.Controls.Add(this.batchButton);
            this.savePanel.Location = new System.Drawing.Point(67, 518);
            this.savePanel.Name = "savePanel";
            this.savePanel.Size = new System.Drawing.Size(362, 41);
            this.savePanel.TabIndex = 2;
            // 
            // MainWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(897, 574);
            this.Controls.Add(this.savePanel);
            this.Controls.Add(this.layerPanel);
            this.Controls.Add(this.openPanel);
            this.Controls.Add(this.fgiBox);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainWin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "krkrFgiEditor";
            ((System.ComponentModel.ISupportInitialize)(this.fgiBox)).EndInit();
            this.openPanel.ResumeLayout(false);
            this.openPanel.PerformLayout();
            this.layerPanel.ResumeLayout(false);
            this.layerPanel.PerformLayout();
            this.savePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openFile;
        private System.Windows.Forms.PictureBox fgiBox;
        private System.Windows.Forms.TextBox filePath;
        private System.Windows.Forms.GroupBox openPanel;
        private System.Windows.Forms.GroupBox layerPanel;
        private System.Windows.Forms.Label groupLabel;
        private ComboBoxEx layerBox;
        private System.Windows.Forms.Label layerLabel;
        private System.Windows.Forms.ComboBox groupBox;
        private ListBoxEx selectedBox;
        private System.Windows.Forms.Button batchButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ToolTip selectedBoxTip;
        private System.Windows.Forms.Panel savePanel;
        private System.Windows.Forms.Label encodingLabel;
        private System.Windows.Forms.ComboBox encodingBox;
        private System.Windows.Forms.Button detectEncoding;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Label selectedLabel;
        private System.Windows.Forms.ToolTip fgiBoxTip;
        private System.Windows.Forms.CheckBox autoSort;
        private System.Windows.Forms.Button deleteButton;
    }
}

