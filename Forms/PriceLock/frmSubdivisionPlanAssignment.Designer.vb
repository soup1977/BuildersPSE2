' =====================================================
' frmSubdivisionPlanAssignment.Designer.vb
' Designer file for Subdivision Plan Assignment Form
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmSubdivisionPlanAssignment

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Labels
    Friend WithEvents lblAvailablePlans As Label
    Friend WithEvents lblAssignedPlans As Label
    Friend WithEvents lblStatus As Label

    ' List Boxes
    Friend WithEvents lstAvailablePlans As ListBox
    Friend WithEvents lstAssignedPlans As ListBox

    ' Buttons
    Friend WithEvents btnAssign As Button
    Friend WithEvents btnRemove As Button
    Friend WithEvents btnAssignAll As Button
    Friend WithEvents btnRemoveAll As Button
    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnClose As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.lblAvailablePlans = New System.Windows.Forms.Label()
        Me.lstAvailablePlans = New System.Windows.Forms.ListBox()
        Me.btnAssign = New System.Windows.Forms.Button()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.btnAssignAll = New System.Windows.Forms.Button()
        Me.btnRemoveAll = New System.Windows.Forms.Button()
        Me.lblAssignedPlans = New System.Windows.Forms.Label()
        Me.lstAssignedPlans = New System.Windows.Forms.ListBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblAvailablePlans
        '
        Me.lblAvailablePlans.AutoSize = True
        Me.lblAvailablePlans.Location = New System.Drawing.Point(20, 15)
        Me.lblAvailablePlans.Name = "lblAvailablePlans"
        Me.lblAvailablePlans.Size = New System.Drawing.Size(82, 13)
        Me.lblAvailablePlans.TabIndex = 0
        Me.lblAvailablePlans.Text = "Available Plans:"
        '
        'lstAvailablePlans
        '
        Me.lstAvailablePlans.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lstAvailablePlans.Location = New System.Drawing.Point(20, 35)
        Me.lstAvailablePlans.Name = "lstAvailablePlans"
        Me.lstAvailablePlans.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAvailablePlans.Size = New System.Drawing.Size(200, 355)
        Me.lstAvailablePlans.TabIndex = 1
        '
        'btnAssign
        '
        Me.btnAssign.Location = New System.Drawing.Point(240, 120)
        Me.btnAssign.Name = "btnAssign"
        Me.btnAssign.Size = New System.Drawing.Size(40, 30)
        Me.btnAssign.TabIndex = 2
        Me.btnAssign.Text = ">"
        '
        'btnRemove
        '
        Me.btnRemove.Location = New System.Drawing.Point(240, 160)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(40, 30)
        Me.btnRemove.TabIndex = 3
        Me.btnRemove.Text = "<"
        '
        'btnAssignAll
        '
        Me.btnAssignAll.Location = New System.Drawing.Point(240, 210)
        Me.btnAssignAll.Name = "btnAssignAll"
        Me.btnAssignAll.Size = New System.Drawing.Size(40, 30)
        Me.btnAssignAll.TabIndex = 4
        Me.btnAssignAll.Text = ">>"
        '
        'btnRemoveAll
        '
        Me.btnRemoveAll.Location = New System.Drawing.Point(240, 250)
        Me.btnRemoveAll.Name = "btnRemoveAll"
        Me.btnRemoveAll.Size = New System.Drawing.Size(40, 30)
        Me.btnRemoveAll.TabIndex = 5
        Me.btnRemoveAll.Text = "<<"
        '
        'lblAssignedPlans
        '
        Me.lblAssignedPlans.AutoSize = True
        Me.lblAssignedPlans.Location = New System.Drawing.Point(300, 15)
        Me.lblAssignedPlans.Name = "lblAssignedPlans"
        Me.lblAssignedPlans.Size = New System.Drawing.Size(122, 13)
        Me.lblAssignedPlans.TabIndex = 6
        Me.lblAssignedPlans.Text = "Assigned to Subdivision:"
        '
        'lstAssignedPlans
        '
        Me.lstAssignedPlans.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lstAssignedPlans.Location = New System.Drawing.Point(300, 35)
        Me.lstAssignedPlans.Name = "lstAssignedPlans"
        Me.lstAssignedPlans.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAssignedPlans.Size = New System.Drawing.Size(200, 355)
        Me.lstAssignedPlans.TabIndex = 7
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = System.Drawing.Color.Green
        Me.lblStatus.Location = New System.Drawing.Point(20, 340)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 8
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnClose)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 400)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(538, 45)
        Me.pnlButtons.TabIndex = 9
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(446, 3)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(80, 23)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        '
        'frmSubdivisionPlanAssignment
        '
        Me.ClientSize = New System.Drawing.Size(538, 445)
        Me.Controls.Add(Me.lblAvailablePlans)
        Me.Controls.Add(Me.lstAvailablePlans)
        Me.Controls.Add(Me.btnAssign)
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.btnAssignAll)
        Me.Controls.Add(Me.btnRemoveAll)
        Me.Controls.Add(Me.lblAssignedPlans)
        Me.Controls.Add(Me.lstAssignedPlans)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.pnlButtons)
        Me.MinimumSize = New System.Drawing.Size(550, 350)
        Me.Name = "frmSubdivisionPlanAssignment"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Plan Assignment"
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

End Class
