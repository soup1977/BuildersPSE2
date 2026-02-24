<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmAdminUnlockDialog
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
        Me.lblWarning = New System.Windows.Forms.Label()
        Me.txtReason = New System.Windows.Forms.TextBox()
        Me.lblReason = New System.Windows.Forms.Label()
        Me.btnUnlock = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblWarning
        '
        Me.lblWarning.Location = New System.Drawing.Point(12, 9)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(460, 120)
        Me.lblWarning.TabIndex = 0
        Me.lblWarning.Text = "Warning text here"
        '
        'txtReason
        '
        Me.txtReason.Location = New System.Drawing.Point(15, 156)
        Me.txtReason.Multiline = True
        Me.txtReason.Name = "txtReason"
        Me.txtReason.Size = New System.Drawing.Size(457, 80)
        Me.txtReason.TabIndex = 2
        '
        'lblReason
        '
        Me.lblReason.AutoSize = True
        Me.lblReason.Location = New System.Drawing.Point(12, 140)
        Me.lblReason.Name = "lblReason"
        Me.lblReason.Size = New System.Drawing.Size(158, 13)
        Me.lblReason.TabIndex = 1
        Me.lblReason.Text = "Reason for unlocking (required):"
        '
        'btnUnlock
        '
        Me.btnUnlock.BackColor = System.Drawing.Color.OrangeRed
        Me.btnUnlock.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUnlock.ForeColor = System.Drawing.Color.White
        Me.btnUnlock.Location = New System.Drawing.Point(285, 250)
        Me.btnUnlock.Name = "btnUnlock"
        Me.btnUnlock.Size = New System.Drawing.Size(90, 30)
        Me.btnUnlock.TabIndex = 3
        Me.btnUnlock.Text = "UNLOCK"
        Me.btnUnlock.UseVisualStyleBackColor = False
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(382, 250)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(90, 30)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmAdminUnlockDialog
        '
        Me.ClientSize = New System.Drawing.Size(484, 292)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnUnlock)
        Me.Controls.Add(Me.txtReason)
        Me.Controls.Add(Me.lblReason)
        Me.Controls.Add(Me.lblWarning)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAdminUnlockDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Admin Override - Unlock Version"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblWarning As Label
    Friend WithEvents txtReason As TextBox
    Friend WithEvents lblReason As Label
    Friend WithEvents btnUnlock As Button
    Friend WithEvents btnCancel As Button
End Class
