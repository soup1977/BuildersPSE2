<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPriceLockAdmin
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
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnPriceLocks = New System.Windows.Forms.Button()
        Me.btnComparisonReport = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnBuilders = New System.Windows.Forms.Button()
        Me.btnPlans = New System.Windows.Forms.Button()
        Me.btnOptions = New System.Windows.Forms.Button()
        Me.btnMaterialCategories = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnRandomLengthsImport = New System.Windows.Forms.Button()
        Me.btnRLMapping = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(20, 20)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(172, 25)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "PriceLock Module"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(20, 60)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(144, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Price Lock Management"
        '
        'btnPriceLocks
        '
        Me.btnPriceLocks.Location = New System.Drawing.Point(30, 85)
        Me.btnPriceLocks.Name = "btnPriceLocks"
        Me.btnPriceLocks.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnPriceLocks.Size = New System.Drawing.Size(320, 35)
        Me.btnPriceLocks.TabIndex = 2
        Me.btnPriceLocks.Text = "📋 Manage Price Locks..."
        Me.btnPriceLocks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPriceLocks.UseVisualStyleBackColor = True
        '
        'btnComparisonReport
        '
        Me.btnComparisonReport.Location = New System.Drawing.Point(30, 125)
        Me.btnComparisonReport.Name = "btnComparisonReport"
        Me.btnComparisonReport.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnComparisonReport.Size = New System.Drawing.Size(320, 35)
        Me.btnComparisonReport.TabIndex = 3
        Me.btnComparisonReport.Text = "📊 Comparison Report..."
        Me.btnComparisonReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnComparisonReport.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(20, 175)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(130, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Setup && Configuration"
        '
        'btnBuilders
        '
        Me.btnBuilders.Location = New System.Drawing.Point(30, 200)
        Me.btnBuilders.Name = "btnBuilders"
        Me.btnBuilders.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnBuilders.Size = New System.Drawing.Size(320, 35)
        Me.btnBuilders.TabIndex = 5
        Me.btnBuilders.Text = "🏗️ Builders && Subdivisions..."
        Me.btnBuilders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBuilders.UseVisualStyleBackColor = True
        '
        'btnPlans
        '
        Me.btnPlans.Location = New System.Drawing.Point(30, 240)
        Me.btnPlans.Name = "btnPlans"
        Me.btnPlans.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnPlans.Size = New System.Drawing.Size(320, 35)
        Me.btnPlans.TabIndex = 6
        Me.btnPlans.Text = "🏠 Plans && Elevations..."
        Me.btnPlans.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPlans.UseVisualStyleBackColor = True
        '
        'btnOptions
        '
        Me.btnOptions.Location = New System.Drawing.Point(30, 280)
        Me.btnOptions.Name = "btnOptions"
        Me.btnOptions.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnOptions.Size = New System.Drawing.Size(320, 35)
        Me.btnOptions.TabIndex = 7
        Me.btnOptions.Text = "⚙️ Structural Options..."
        Me.btnOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOptions.UseVisualStyleBackColor = True
        '
        'btnMaterialCategories
        '
        Me.btnMaterialCategories.Location = New System.Drawing.Point(30, 320)
        Me.btnMaterialCategories.Name = "btnMaterialCategories"
        Me.btnMaterialCategories.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnMaterialCategories.Size = New System.Drawing.Size(320, 35)
        Me.btnMaterialCategories.TabIndex = 8
        Me.btnMaterialCategories.Text = "📦 Material Categories..."
        Me.btnMaterialCategories.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnMaterialCategories.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(270, 443)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(80, 30)
        Me.btnClose.TabIndex = 9
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnRandomLengthsImport
        '
        Me.btnRandomLengthsImport.Location = New System.Drawing.Point(30, 361)
        Me.btnRandomLengthsImport.Name = "btnRandomLengthsImport"
        Me.btnRandomLengthsImport.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnRandomLengthsImport.Size = New System.Drawing.Size(320, 35)
        Me.btnRandomLengthsImport.TabIndex = 10
        Me.btnRandomLengthsImport.Text = "📦 Random Lengths Import..."
        Me.btnRandomLengthsImport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRandomLengthsImport.UseVisualStyleBackColor = True
        '
        'btnRLMapping
        '
        Me.btnRLMapping.Location = New System.Drawing.Point(30, 402)
        Me.btnRLMapping.Name = "btnRLMapping"
        Me.btnRLMapping.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.btnRLMapping.Size = New System.Drawing.Size(320, 35)
        Me.btnRLMapping.TabIndex = 11
        Me.btnRLMapping.Text = "📦 RL Mapping..."
        Me.btnRLMapping.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRLMapping.UseVisualStyleBackColor = True
        '
        'frmPriceLockAdmin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(384, 481)
        Me.Controls.Add(Me.btnRLMapping)
        Me.Controls.Add(Me.btnRandomLengthsImport)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnMaterialCategories)
        Me.Controls.Add(Me.btnOptions)
        Me.Controls.Add(Me.btnPlans)
        Me.Controls.Add(Me.btnBuilders)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnComparisonReport)
        Me.Controls.Add(Me.btnPriceLocks)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPriceLockAdmin"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "PriceLock Administration"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents btnPriceLocks As Button
    Friend WithEvents btnComparisonReport As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents btnBuilders As Button
    Friend WithEvents btnPlans As Button
    Friend WithEvents btnOptions As Button
    Friend WithEvents btnMaterialCategories As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnRandomLengthsImport As Button
    Friend WithEvents btnRLMapping As Button
End Class