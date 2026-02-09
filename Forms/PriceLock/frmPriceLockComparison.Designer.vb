' =====================================================
' frmPriceLockComparison.Designer.vb
' Designer file for Price Lock Comparison Form
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmPriceLockComparison

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

    ' Filter Controls
    Friend WithEvents cboBuilder As ComboBox
    Friend WithEvents cboSubdivision As ComboBox
    Friend WithEvents cboLockFrom As ComboBox
    Friend WithEvents cboLockTo As ComboBox
    Friend WithEvents btnRunReport As Button

    ' Results
    Friend WithEvents tabResults As TabControl
    Friend WithEvents tabComponents As TabPage
    Friend WithEvents tabMaterials As TabPage
    Friend WithEvents dgvComponents As DataGridView
    Friend WithEvents dgvMaterials As DataGridView

    ' Summary
    Friend WithEvents lblComponentSummary As Label
    Friend WithEvents lblMaterialSummary As Label

    ' Buttons
    Friend WithEvents btnExport As Button
    Friend WithEvents btnClose As Button

    ' Panels
    Friend WithEvents pnlFilters As Panel
    Friend WithEvents pnlSummary As Panel
    Friend WithEvents pnlButtons As Panel

    ' Labels
    Friend WithEvents lblBuilder As Label
    Friend WithEvents lblSubdivision As Label
    Friend WithEvents lblCompare As Label
    Friend WithEvents lblVs As Label

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()

        Me.pnlFilters = New Panel()
        Me.lblBuilder = New Label()
        Me.cboBuilder = New ComboBox()
        Me.lblSubdivision = New Label()
        Me.cboSubdivision = New ComboBox()
        Me.lblCompare = New Label()
        Me.cboLockFrom = New ComboBox()
        Me.lblVs = New Label()
        Me.cboLockTo = New ComboBox()
        Me.btnRunReport = New Button()

        Me.pnlSummary = New Panel()
        Me.lblComponentSummary = New Label()
        Me.lblMaterialSummary = New Label()

        Me.tabResults = New TabControl()
        Me.tabComponents = New TabPage()
        Me.dgvComponents = New DataGridView()
        Me.tabMaterials = New TabPage()
        Me.dgvMaterials = New DataGridView()

        Me.pnlButtons = New Panel()
        Me.btnExport = New Button()
        Me.btnClose = New Button()

        Me.pnlFilters.SuspendLayout()
        Me.pnlSummary.SuspendLayout()
        Me.tabResults.SuspendLayout()
        Me.tabComponents.SuspendLayout()
        CType(Me.dgvComponents, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabMaterials.SuspendLayout()
        CType(Me.dgvMaterials, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()

        '
        ' pnlFilters
        '
        Me.pnlFilters.Controls.Add(Me.lblBuilder)
        Me.pnlFilters.Controls.Add(Me.cboBuilder)
        Me.pnlFilters.Controls.Add(Me.lblSubdivision)
        Me.pnlFilters.Controls.Add(Me.cboSubdivision)
        Me.pnlFilters.Controls.Add(Me.lblCompare)
        Me.pnlFilters.Controls.Add(Me.cboLockFrom)
        Me.pnlFilters.Controls.Add(Me.lblVs)
        Me.pnlFilters.Controls.Add(Me.cboLockTo)
        Me.pnlFilters.Controls.Add(Me.btnRunReport)
        Me.pnlFilters.Dock = DockStyle.Top
        Me.pnlFilters.Height = 90
        Me.pnlFilters.Padding = New Padding(10)
        Me.pnlFilters.Name = "pnlFilters"

        '
        ' lblBuilder
        '
        Me.lblBuilder.AutoSize = True
        Me.lblBuilder.Location = New Point(10, 15)
        Me.lblBuilder.Name = "lblBuilder"
        Me.lblBuilder.Text = "Builder:"

        '
        ' cboBuilder
        '
        Me.cboBuilder.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboBuilder.Location = New Point(70, 12)
        Me.cboBuilder.Name = "cboBuilder"
        Me.cboBuilder.Width = 180

        '
        ' lblSubdivision
        '
        Me.lblSubdivision.AutoSize = True
        Me.lblSubdivision.Location = New Point(270, 15)
        Me.lblSubdivision.Name = "lblSubdivision"
        Me.lblSubdivision.Text = "Subdivision:"

        '
        ' cboSubdivision
        '
        Me.cboSubdivision.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboSubdivision.Location = New Point(350, 12)
        Me.cboSubdivision.Name = "cboSubdivision"
        Me.cboSubdivision.Width = 220

        '
        ' lblCompare
        '
        Me.lblCompare.AutoSize = True
        Me.lblCompare.Location = New Point(10, 47)
        Me.lblCompare.Name = "lblCompare"
        Me.lblCompare.Text = "Compare:"

        '
        ' cboLockFrom
        '
        Me.cboLockFrom.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboLockFrom.Location = New Point(70, 44)
        Me.cboLockFrom.Name = "cboLockFrom"
        Me.cboLockFrom.Width = 250

        '
        ' lblVs
        '
        Me.lblVs.AutoSize = True
        Me.lblVs.Location = New Point(330, 47)
        Me.lblVs.Name = "lblVs"
        Me.lblVs.Text = "vs"

        '
        ' cboLockTo
        '
        Me.cboLockTo.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboLockTo.Location = New Point(350, 44)
        Me.cboLockTo.Name = "cboLockTo"
        Me.cboLockTo.Width = 250

        '
        ' btnRunReport
        '
        Me.btnRunReport.Height = 28
        Me.btnRunReport.Location = New Point(620, 42)
        Me.btnRunReport.Name = "btnRunReport"
        Me.btnRunReport.Text = "Run Report"
        Me.btnRunReport.Width = 100

        '
        ' pnlSummary
        '
        Me.pnlSummary.BackColor = Color.FromArgb(240, 240, 245)
        Me.pnlSummary.Controls.Add(Me.lblComponentSummary)
        Me.pnlSummary.Controls.Add(Me.lblMaterialSummary)
        Me.pnlSummary.Dock = DockStyle.Top
        Me.pnlSummary.Height = 50
        Me.pnlSummary.Name = "pnlSummary"
        Me.pnlSummary.Padding = New Padding(10)

        '
        ' lblComponentSummary
        '
        Me.lblComponentSummary.AutoSize = True
        Me.lblComponentSummary.Font = New Font(Me.Font.FontFamily, 10, FontStyle.Bold)
        Me.lblComponentSummary.Location = New Point(10, 15)
        Me.lblComponentSummary.Name = "lblComponentSummary"
        Me.lblComponentSummary.Text = "Component Changes: --"

        '
        ' lblMaterialSummary
        '
        Me.lblMaterialSummary.AutoSize = True
        Me.lblMaterialSummary.Font = New Font(Me.Font.FontFamily, 10, FontStyle.Bold)
        Me.lblMaterialSummary.Location = New Point(350, 15)
        Me.lblMaterialSummary.Name = "lblMaterialSummary"
        Me.lblMaterialSummary.Text = "Material Changes: --"

        '
        ' tabResults
        '
        Me.tabResults.Controls.Add(Me.tabComponents)
        Me.tabResults.Controls.Add(Me.tabMaterials)
        Me.tabResults.Dock = DockStyle.Fill
        Me.tabResults.Name = "tabResults"

        '
        ' tabComponents
        '
        Me.tabComponents.Controls.Add(Me.dgvComponents)
        Me.tabComponents.Name = "tabComponents"
        Me.tabComponents.Text = "Component Price Changes"

        '
        ' dgvComponents
        '
        Me.dgvComponents.AllowUserToAddRows = False
        Me.dgvComponents.AllowUserToDeleteRows = False
        Me.dgvComponents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvComponents.BackgroundColor = SystemColors.Window
        Me.dgvComponents.Dock = DockStyle.Fill
        Me.dgvComponents.MultiSelect = False
        Me.dgvComponents.Name = "dgvComponents"
        Me.dgvComponents.ReadOnly = True
        Me.dgvComponents.RowHeadersVisible = False
        Me.dgvComponents.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        '
        ' tabMaterials
        '
        Me.tabMaterials.Controls.Add(Me.dgvMaterials)
        Me.tabMaterials.Name = "tabMaterials"
        Me.tabMaterials.Text = "Material Price Changes"

        '
        ' dgvMaterials
        '
        Me.dgvMaterials.AllowUserToAddRows = False
        Me.dgvMaterials.AllowUserToDeleteRows = False
        Me.dgvMaterials.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvMaterials.BackgroundColor = SystemColors.Window
        Me.dgvMaterials.Dock = DockStyle.Fill
        Me.dgvMaterials.MultiSelect = False
        Me.dgvMaterials.Name = "dgvMaterials"
        Me.dgvMaterials.ReadOnly = True
        Me.dgvMaterials.RowHeadersVisible = False
        Me.dgvMaterials.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        '
        ' pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnExport)
        Me.pnlButtons.Controls.Add(Me.btnClose)
        Me.pnlButtons.Dock = DockStyle.Bottom
        Me.pnlButtons.Height = 50
        Me.pnlButtons.Name = "pnlButtons"

        '
        ' btnExport
        '
        Me.btnExport.Anchor = AnchorStyles.Right Or AnchorStyles.Top
        Me.btnExport.Enabled = False
        Me.btnExport.Height = 30
        Me.btnExport.Location = New Point(780, 10)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Text = "Export to Excel"
        Me.btnExport.Width = 100

        '
        ' btnClose
        '
        Me.btnClose.Anchor = AnchorStyles.Right Or AnchorStyles.Top
        Me.btnClose.Height = 30
        Me.btnClose.Location = New Point(900, 10)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Text = "Close"
        Me.btnClose.Width = 80

        '
        ' frmPriceLockComparison
        '
        Me.ClientSize = New Size(1000, 700)
        Me.Controls.Add(Me.tabResults)
        Me.Controls.Add(Me.pnlSummary)
        Me.Controls.Add(Me.pnlFilters)
        Me.Controls.Add(Me.pnlButtons)
        Me.MinimumSize = New Size(800, 500)
        Me.Name = "frmPriceLockComparison"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Price Lock Comparison Report"

        Me.pnlFilters.ResumeLayout(False)
        Me.pnlFilters.PerformLayout()
        Me.pnlSummary.ResumeLayout(False)
        Me.pnlSummary.PerformLayout()
        Me.tabResults.ResumeLayout(False)
        Me.tabComponents.ResumeLayout(False)
        CType(Me.dgvComponents, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabMaterials.ResumeLayout(False)
        CType(Me.dgvMaterials, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class