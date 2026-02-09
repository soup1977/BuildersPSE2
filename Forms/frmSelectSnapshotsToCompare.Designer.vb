<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSelectSnapshotsToCompare
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblSnapshot1 = New System.Windows.Forms.Label()
        Me.cboSnapshot1 = New System.Windows.Forms.ComboBox()
        Me.lblSnapshot2 = New System.Windows.Forms.Label()
        Me.cboSnapshot2 = New System.Windows.Forms.ComboBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblInstructions = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblInstructions
        '
        Me.lblInstructions.AutoSize = True
        Me.lblInstructions.Location = New System.Drawing.Point(12, 15)
        Me.lblInstructions.Name = "lblInstructions"
        Me.lblInstructions.Size = New System.Drawing.Size(280, 15)
        Me.lblInstructions.TabIndex = 0
        Me.lblInstructions.Text = "Select two price history snapshots to compare:"
        '
        'lblSnapshot1
        '
        Me.lblSnapshot1.AutoSize = True
        Me.lblSnapshot1.Location = New System.Drawing.Point(12, 50)
        Me.lblSnapshot1.Name = "lblSnapshot1"
        Me.lblSnapshot1.Size = New System.Drawing.Size(70, 15)
        Me.lblSnapshot1.TabIndex = 1
        Me.lblSnapshot1.Text = "Snapshot 1:"
        '
        'cboSnapshot1
        '
        Me.cboSnapshot1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSnapshot1.FormattingEnabled = True
        Me.cboSnapshot1.Location = New System.Drawing.Point(90, 47)
        Me.cboSnapshot1.Name = "cboSnapshot1"
        Me.cboSnapshot1.Size = New System.Drawing.Size(330, 23)
        Me.cboSnapshot1.TabIndex = 2
        '
        'lblSnapshot2
        '
        Me.lblSnapshot2.AutoSize = True
        Me.lblSnapshot2.Location = New System.Drawing.Point(12, 85)
        Me.lblSnapshot2.Name = "lblSnapshot2"
        Me.lblSnapshot2.Size = New System.Drawing.Size(70, 15)
        Me.lblSnapshot2.TabIndex = 3
        Me.lblSnapshot2.Text = "Snapshot 2:"
        '
        'cboSnapshot2
        '
        Me.cboSnapshot2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSnapshot2.FormattingEnabled = True
        Me.cboSnapshot2.Location = New System.Drawing.Point(90, 82)
        Me.cboSnapshot2.Name = "cboSnapshot2"
        Me.cboSnapshot2.Size = New System.Drawing.Size(330, 23)
        Me.cboSnapshot2.TabIndex = 4
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(264, 125)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 30)
        Me.btnOK.TabIndex = 5
        Me.btnOK.Text = "Compare"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(345, 125)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 30)
        Me.btnCancel.TabIndex = 6
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmSelectSnapshotsToCompare
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(434, 171)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.cboSnapshot2)
        Me.Controls.Add(Me.lblSnapshot2)
        Me.Controls.Add(Me.cboSnapshot1)
        Me.Controls.Add(Me.lblSnapshot1)
        Me.Controls.Add(Me.lblInstructions)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSelectSnapshotsToCompare"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Select Snapshots to Compare"
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents lblInstructions As Label
    Friend WithEvents lblSnapshot1 As Label
    Friend WithEvents cboSnapshot1 As ComboBox
    Friend WithEvents lblSnapshot2 As Label
    Friend WithEvents cboSnapshot2 As ComboBox
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button

End Class
