' =====================================================
' frmSelectOption.vb
' Dialog to select an existing option or create new
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.Models
Imports BuildersPSE2.Forms.PriceLock

Public Class frmSelectOption
    Inherits Form

#Region "Fields"

    Private _csvOptionName As String
    Private _existingOptions As List(Of PLOption)
    Private _currentSelectionID As Integer?

#End Region

#Region "Properties"

    Public Property SelectedOption As PLOption
    Public Property CreateNew As Boolean
    Public Property NewOptionName As String

#End Region

#Region "Constructor"

    Public Sub New(csvOptionName As String, existingOptions As List(Of PLOption), currentSelectionID As Integer?)
        _csvOptionName = csvOptionName
        _existingOptions = existingOptions
        _currentSelectionID = currentSelectionID

        InitializeComponent()
        Me.Text = "Select Option Mapping"
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmSelectOption_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblCsvName.Text = $"CSV Option: ""{_csvOptionName}"""
        PopulateOptionList()
        txtNewOptionName.Text = _csvOptionName
    End Sub

    Private Sub rdoExisting_CheckedChanged(sender As Object, e As EventArgs) Handles rdoExisting.CheckedChanged
        lstOptions.Enabled = rdoExisting.Checked
        txtSearch.Enabled = rdoExisting.Checked
        txtNewOptionName.Enabled = Not rdoExisting.Checked
        UpdateOkButton()
    End Sub

    Private Sub rdoCreateNew_CheckedChanged(sender As Object, e As EventArgs) Handles rdoCreateNew.CheckedChanged
        lstOptions.Enabled = rdoExisting.Checked
        txtSearch.Enabled = rdoExisting.Checked
        txtNewOptionName.Enabled = rdoCreateNew.Checked
        UpdateOkButton()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        FilterOptionList()
    End Sub

    Private Sub lstOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstOptions.SelectedIndexChanged
        UpdateOkButton()
    End Sub

    Private Sub txtNewOptionName_TextChanged(sender As Object, e As EventArgs) Handles txtNewOptionName.TextChanged
        UpdateOkButton()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If rdoCreateNew.Checked Then
            CreateNew = True
            NewOptionName = txtNewOptionName.Text.Trim()
            SelectedOption = Nothing
        Else
            CreateNew = False
            SelectedOption = DirectCast(lstOptions.SelectedItem, PLOption)
            NewOptionName = Nothing
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Helper Methods"

    Private Sub PopulateOptionList()
        lstOptions.Items.Clear()
        lstOptions.DisplayMember = "OptionName"

        ' Sort by similarity to CSV name
        Dim sortedOptions = _existingOptions _
            .Select(Function(o) New With {
                .Option = o,
                .Similarity = FuzzyMatcher.SimilarityRatio(
                    FuzzyMatcher.NormalizeForComparison(_csvOptionName),
                    FuzzyMatcher.NormalizeForComparison(o.OptionName))
            }) _
            .OrderByDescending(Function(x) x.Similarity) _
            .Select(Function(x) x.Option) _
            .ToList()

        For Each opt In sortedOptions
            lstOptions.Items.Add(opt)
        Next

        ' Select current if exists
        If _currentSelectionID.HasValue Then
            For i = 0 To lstOptions.Items.Count - 1
                If DirectCast(lstOptions.Items(i), PLOption).OptionID = _currentSelectionID.Value Then
                    lstOptions.SelectedIndex = i
                    rdoExisting.Checked = True
                    Exit For
                End If
            Next
        ElseIf lstOptions.Items.Count > 0 Then
            lstOptions.SelectedIndex = 0
        End If
    End Sub

    Private Sub FilterOptionList()
        Dim searchText = txtSearch.Text.Trim().ToUpperInvariant()

        lstOptions.Items.Clear()

        Dim filtered = _existingOptions _
            .Where(Function(o) String.IsNullOrEmpty(searchText) OrElse
                              o.OptionName.ToUpperInvariant().Contains(searchText)) _
            .OrderBy(Function(o) o.OptionName) _
            .ToList()

        For Each opt In filtered
            lstOptions.Items.Add(opt)
        Next

        If lstOptions.Items.Count > 0 Then
            lstOptions.SelectedIndex = 0
        End If
    End Sub

    Private Sub UpdateOkButton()
        If rdoCreateNew.Checked Then
            btnOK.Enabled = Not String.IsNullOrWhiteSpace(txtNewOptionName.Text)
        Else
            btnOK.Enabled = lstOptions.SelectedItem IsNot Nothing
        End If
    End Sub

#End Region

#Region "Designer"

    Private Sub InitializeComponent()
        Me.lblCsvName = New Label()
        Me.grpChoice = New GroupBox()
        Me.rdoExisting = New RadioButton()
        Me.rdoCreateNew = New RadioButton()
        Me.lblSearch = New Label()
        Me.txtSearch = New TextBox()
        Me.lstOptions = New ListBox()
        Me.lblNewName = New Label()
        Me.txtNewOptionName = New TextBox()
        Me.btnOK = New Button()
        Me.btnCancel = New Button()

        Me.grpChoice.SuspendLayout()
        Me.SuspendLayout()

        ' lblCsvName
        Me.lblCsvName.AutoSize = True
        Me.lblCsvName.Font = New Font("Microsoft Sans Serif", 9.0!, FontStyle.Bold)
        Me.lblCsvName.Location = New Point(12, 15)
        Me.lblCsvName.Name = "lblCsvName"
        Me.lblCsvName.Size = New Size(100, 15)

        ' grpChoice
        Me.grpChoice.Controls.Add(Me.rdoExisting)
        Me.grpChoice.Controls.Add(Me.rdoCreateNew)
        Me.grpChoice.Location = New Point(12, 45)
        Me.grpChoice.Name = "grpChoice"
        Me.grpChoice.Size = New Size(360, 55)
        Me.grpChoice.TabStop = False
        Me.grpChoice.Text = "Choose Action"

        ' rdoExisting
        Me.rdoExisting.AutoSize = True
        Me.rdoExisting.Checked = True
        Me.rdoExisting.Location = New Point(15, 22)
        Me.rdoExisting.Name = "rdoExisting"
        Me.rdoExisting.Size = New Size(130, 17)
        Me.rdoExisting.Text = "Use existing option:"

        ' rdoCreateNew
        Me.rdoCreateNew.AutoSize = True
        Me.rdoCreateNew.Location = New Point(200, 22)
        Me.rdoCreateNew.Name = "rdoCreateNew"
        Me.rdoCreateNew.Size = New Size(130, 17)
        Me.rdoCreateNew.Text = "Create new option"

        ' lblSearch
        Me.lblSearch.AutoSize = True
        Me.lblSearch.Location = New Point(12, 115)
        Me.lblSearch.Name = "lblSearch"
        Me.lblSearch.Text = "Search:"

        ' txtSearch
        Me.txtSearch.Location = New Point(60, 112)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New Size(312, 20)

        ' lstOptions
        Me.lstOptions.Location = New Point(12, 140)
        Me.lstOptions.Name = "lstOptions"
        Me.lstOptions.Size = New Size(360, 160)

        ' lblNewName
        Me.lblNewName.AutoSize = True
        Me.lblNewName.Location = New Point(12, 315)
        Me.lblNewName.Name = "lblNewName"
        Me.lblNewName.Text = "New option name:"

        ' txtNewOptionName
        Me.txtNewOptionName.Enabled = False
        Me.txtNewOptionName.Location = New Point(120, 312)
        Me.txtNewOptionName.Name = "txtNewOptionName"
        Me.txtNewOptionName.Size = New Size(252, 20)

        ' btnOK
        Me.btnOK.Location = New Point(216, 350)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New Size(75, 28)
        Me.btnOK.Text = "OK"

        ' btnCancel
        Me.btnCancel.Location = New Point(297, 350)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New Size(75, 28)
        Me.btnCancel.Text = "Cancel"

        ' frmSelectOption
        Me.ClientSize = New Size(384, 391)
        Me.Controls.Add(Me.lblCsvName)
        Me.Controls.Add(Me.grpChoice)
        Me.Controls.Add(Me.lblSearch)
        Me.Controls.Add(Me.txtSearch)
        Me.Controls.Add(Me.lstOptions)
        Me.Controls.Add(Me.lblNewName)
        Me.Controls.Add(Me.txtNewOptionName)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSelectOption"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Select Option"

        Me.grpChoice.ResumeLayout(False)
        Me.grpChoice.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Private lblCsvName As Label
    Private grpChoice As GroupBox
    Private WithEvents rdoExisting As RadioButton
    Private WithEvents rdoCreateNew As RadioButton
    Private lblSearch As Label
    Private WithEvents txtSearch As TextBox
    Private WithEvents lstOptions As ListBox
    Private lblNewName As Label
    Private WithEvents txtNewOptionName As TextBox
    Private WithEvents btnOK As Button
    Private WithEvents btnCancel As Button

#End Region

End Class