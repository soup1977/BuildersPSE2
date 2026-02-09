' =====================================================
' frmRandomLengthsImportManager.Designer.vb
' Designer file for Random Lengths Import Manager
' BuildersPSE2 - PriceLock Module
' =====================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRandomLengthsImportManager
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.splitContainer = New System.Windows.Forms.SplitContainer()
        Me.grpImports = New System.Windows.Forms.GroupBox()
        Me.dgvImports = New System.Windows.Forms.DataGridView()
        Me.pnlImportButtons = New System.Windows.Forms.Panel()
        Me.lblImportCount = New System.Windows.Forms.Label()
        Me.btnDeleteImport = New System.Windows.Forms.Button()
        Me.btnEditImport = New System.Windows.Forms.Button()
        Me.btnNewImport = New System.Windows.Forms.Button()
        Me.grpPricingDetail = New System.Windows.Forms.GroupBox()
        Me.dgvPricing = New System.Windows.Forms.DataGridView()
        Me.pnlPricingButtons = New System.Windows.Forms.Panel()
        Me.btnCopyFromPrevious = New System.Windows.Forms.Button()
        Me.btnSavePricing = New System.Windows.Forms.Button()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnImportfromExcel = New System.Windows.Forms.Button()
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer.Panel1.SuspendLayout()
        Me.splitContainer.Panel2.SuspendLayout()
        Me.splitContainer.SuspendLayout()
        Me.grpImports.SuspendLayout()
        CType(Me.dgvImports, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlImportButtons.SuspendLayout()
        Me.grpPricingDetail.SuspendLayout()
        CType(Me.dgvPricing, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlPricingButtons.SuspendLayout()
        Me.pnlBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'splitContainer
        '
        Me.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer.Location = New System.Drawing.Point(0, 0)
        Me.splitContainer.Name = "splitContainer"
        Me.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitContainer.Panel1
        '
        Me.splitContainer.Panel1.Controls.Add(Me.grpImports)
        Me.splitContainer.Panel1MinSize = 150
        '
        'splitContainer.Panel2
        '
        Me.splitContainer.Panel2.Controls.Add(Me.grpPricingDetail)
        Me.splitContainer.Panel2MinSize = 200
        Me.splitContainer.Size = New System.Drawing.Size(984, 661)
        Me.splitContainer.SplitterDistance = 220
        Me.splitContainer.TabIndex = 0
        '
        'grpImports
        '
        Me.grpImports.Controls.Add(Me.dgvImports)
        Me.grpImports.Controls.Add(Me.pnlImportButtons)
        Me.grpImports.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpImports.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.grpImports.Location = New System.Drawing.Point(0, 0)
        Me.grpImports.Name = "grpImports"
        Me.grpImports.Padding = New System.Windows.Forms.Padding(8)
        Me.grpImports.Size = New System.Drawing.Size(984, 220)
        Me.grpImports.TabIndex = 0
        Me.grpImports.TabStop = False
        Me.grpImports.Text = "Random Lengths Imports"
        '
        'dgvImports
        '
        Me.dgvImports.AllowUserToAddRows = False
        Me.dgvImports.AllowUserToDeleteRows = False
        Me.dgvImports.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvImports.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgvImports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvImports.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvImports.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.dgvImports.Location = New System.Drawing.Point(8, 24)
        Me.dgvImports.MultiSelect = False
        Me.dgvImports.Name = "dgvImports"
        Me.dgvImports.ReadOnly = True
        Me.dgvImports.RowHeadersWidth = 30
        Me.dgvImports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvImports.Size = New System.Drawing.Size(968, 148)
        Me.dgvImports.TabIndex = 1
        '
        'pnlImportButtons
        '
        Me.pnlImportButtons.Controls.Add(Me.btnImportfromExcel)
        Me.pnlImportButtons.Controls.Add(Me.lblImportCount)
        Me.pnlImportButtons.Controls.Add(Me.btnDeleteImport)
        Me.pnlImportButtons.Controls.Add(Me.btnEditImport)
        Me.pnlImportButtons.Controls.Add(Me.btnNewImport)
        Me.pnlImportButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlImportButtons.Location = New System.Drawing.Point(8, 172)
        Me.pnlImportButtons.Name = "pnlImportButtons"
        Me.pnlImportButtons.Padding = New System.Windows.Forms.Padding(0, 8, 0, 0)
        Me.pnlImportButtons.Size = New System.Drawing.Size(968, 40)
        Me.pnlImportButtons.TabIndex = 0
        '
        'lblImportCount
        '
        Me.lblImportCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblImportCount.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblImportCount.ForeColor = System.Drawing.Color.Gray
        Me.lblImportCount.Location = New System.Drawing.Point(848, 11)
        Me.lblImportCount.Name = "lblImportCount"
        Me.lblImportCount.Size = New System.Drawing.Size(120, 20)
        Me.lblImportCount.TabIndex = 3
        Me.lblImportCount.Text = "0 import(s)"
        Me.lblImportCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnDeleteImport
        '
        Me.btnDeleteImport.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.btnDeleteImport.Location = New System.Drawing.Point(200, 8)
        Me.btnDeleteImport.Name = "btnDeleteImport"
        Me.btnDeleteImport.Size = New System.Drawing.Size(90, 28)
        Me.btnDeleteImport.TabIndex = 2
        Me.btnDeleteImport.Text = "Delete"
        Me.btnDeleteImport.UseVisualStyleBackColor = True
        '
        'btnEditImport
        '
        Me.btnEditImport.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.btnEditImport.Location = New System.Drawing.Point(104, 8)
        Me.btnEditImport.Name = "btnEditImport"
        Me.btnEditImport.Size = New System.Drawing.Size(90, 28)
        Me.btnEditImport.TabIndex = 1
        Me.btnEditImport.Text = "Edit..."
        Me.btnEditImport.UseVisualStyleBackColor = True
        '
        'btnNewImport
        '
        Me.btnNewImport.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.btnNewImport.Location = New System.Drawing.Point(0, 8)
        Me.btnNewImport.Name = "btnNewImport"
        Me.btnNewImport.Size = New System.Drawing.Size(98, 28)
        Me.btnNewImport.TabIndex = 0
        Me.btnNewImport.Text = "New Import..."
        Me.btnNewImport.UseVisualStyleBackColor = True
        '
        'grpPricingDetail
        '
        Me.grpPricingDetail.Controls.Add(Me.dgvPricing)
        Me.grpPricingDetail.Controls.Add(Me.pnlPricingButtons)
        Me.grpPricingDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpPricingDetail.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.grpPricingDetail.Location = New System.Drawing.Point(0, 0)
        Me.grpPricingDetail.Name = "grpPricingDetail"
        Me.grpPricingDetail.Padding = New System.Windows.Forms.Padding(8)
        Me.grpPricingDetail.Size = New System.Drawing.Size(984, 437)
        Me.grpPricingDetail.TabIndex = 0
        Me.grpPricingDetail.TabStop = False
        Me.grpPricingDetail.Text = "Pricing Detail"
        '
        'dgvPricing
        '
        Me.dgvPricing.AllowUserToAddRows = False
        Me.dgvPricing.AllowUserToDeleteRows = False
        Me.dgvPricing.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvPricing.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgvPricing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPricing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvPricing.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.dgvPricing.Location = New System.Drawing.Point(8, 24)
        Me.dgvPricing.Name = "dgvPricing"
        Me.dgvPricing.RowHeadersWidth = 30
        Me.dgvPricing.Size = New System.Drawing.Size(968, 365)
        Me.dgvPricing.TabIndex = 1
        '
        'pnlPricingButtons
        '
        Me.pnlPricingButtons.Controls.Add(Me.btnCopyFromPrevious)
        Me.pnlPricingButtons.Controls.Add(Me.btnSavePricing)
        Me.pnlPricingButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlPricingButtons.Location = New System.Drawing.Point(8, 389)
        Me.pnlPricingButtons.Name = "pnlPricingButtons"
        Me.pnlPricingButtons.Padding = New System.Windows.Forms.Padding(0, 8, 0, 0)
        Me.pnlPricingButtons.Size = New System.Drawing.Size(968, 40)
        Me.pnlPricingButtons.TabIndex = 0
        '
        'btnCopyFromPrevious
        '
        Me.btnCopyFromPrevious.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.btnCopyFromPrevious.Location = New System.Drawing.Point(0, 8)
        Me.btnCopyFromPrevious.Name = "btnCopyFromPrevious"
        Me.btnCopyFromPrevious.Size = New System.Drawing.Size(140, 28)
        Me.btnCopyFromPrevious.TabIndex = 0
        Me.btnCopyFromPrevious.Text = "Copy from Previous"
        Me.btnCopyFromPrevious.UseVisualStyleBackColor = True
        '
        'btnSavePricing
        '
        Me.btnSavePricing.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSavePricing.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.btnSavePricing.Location = New System.Drawing.Point(868, 8)
        Me.btnSavePricing.Name = "btnSavePricing"
        Me.btnSavePricing.Size = New System.Drawing.Size(100, 28)
        Me.btnSavePricing.TabIndex = 1
        Me.btnSavePricing.Text = "Save Pricing"
        Me.btnSavePricing.UseVisualStyleBackColor = True
        '
        'pnlBottom
        '
        Me.pnlBottom.Controls.Add(Me.btnClose)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 661)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Padding = New System.Windows.Forms.Padding(8)
        Me.pnlBottom.Size = New System.Drawing.Size(984, 50)
        Me.pnlBottom.TabIndex = 1
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.btnClose.Location = New System.Drawing.Point(884, 11)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(90, 28)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnImportfromExcel
        '
        Me.btnImportfromExcel.Location = New System.Drawing.Point(302, 11)
        Me.btnImportfromExcel.Name = "btnImportfromExcel"
        Me.btnImportfromExcel.Size = New System.Drawing.Size(127, 24)
        Me.btnImportfromExcel.TabIndex = 4
        Me.btnImportfromExcel.Text = "Import from Excel"
        Me.btnImportfromExcel.UseVisualStyleBackColor = True
        '
        'frmRandomLengthsImportManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(984, 711)
        Me.Controls.Add(Me.splitContainer)
        Me.Controls.Add(Me.pnlBottom)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "frmRandomLengthsImportManager"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Random Lengths Import Manager"
        Me.splitContainer.Panel1.ResumeLayout(False)
        Me.splitContainer.Panel2.ResumeLayout(False)
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer.ResumeLayout(False)
        Me.grpImports.ResumeLayout(False)
        CType(Me.dgvImports, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlImportButtons.ResumeLayout(False)
        Me.grpPricingDetail.ResumeLayout(False)
        CType(Me.dgvPricing, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlPricingButtons.ResumeLayout(False)
        Me.pnlBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents splitContainer As SplitContainer
    Friend WithEvents grpImports As GroupBox
    Friend WithEvents dgvImports As DataGridView
    Friend WithEvents pnlImportButtons As Panel
    Friend WithEvents btnNewImport As Button
    Friend WithEvents btnEditImport As Button
    Friend WithEvents btnDeleteImport As Button
    Friend WithEvents lblImportCount As Label
    Friend WithEvents grpPricingDetail As GroupBox
    Friend WithEvents dgvPricing As DataGridView
    Friend WithEvents pnlPricingButtons As Panel
    Friend WithEvents btnSavePricing As Button
    Friend WithEvents btnCopyFromPrevious As Button
    Friend WithEvents pnlBottom As Panel
    Friend WithEvents btnClose As Button
    Friend WithEvents btnImportfromExcel As Button
End Class
