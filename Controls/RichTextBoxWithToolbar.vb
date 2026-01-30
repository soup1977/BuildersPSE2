Imports System.Windows.Forms

Public Class RichTextBoxWithToolbar
    Inherits UserControl

    Private WithEvents rtb As RichTextBox
    Private toolStrip As ToolStrip
    Private WithEvents btnBold As ToolStripButton
    Private WithEvents btnItalic As ToolStripButton
    Private WithEvents btnUnderline As ToolStripButton
    Private WithEvents cboFontSize As ToolStripComboBox

    Public Sub New()
        rtb = New RichTextBox()
        toolStrip = New ToolStrip()
        InitializeControls()
    End Sub

    Private Sub InitializeControls()
        ' Toolbar buttons
        btnBold = New ToolStripButton("B") With {.Font = New Font("Segoe UI", 9, FontStyle.Bold), .ToolTipText = "Bold (Ctrl+B)"}
        btnItalic = New ToolStripButton("I") With {.Font = New Font("Segoe UI", 9, FontStyle.Italic), .ToolTipText = "Italic (Ctrl+I)"}
        btnUnderline = New ToolStripButton("U") With {.Font = New Font("Segoe UI", 9, FontStyle.Underline), .ToolTipText = "Underline (Ctrl+U)"}

        cboFontSize = New ToolStripComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 50, .ToolTipText = "Font Size"}
        cboFontSize.Items.AddRange({"8", "9", "10", "11", "12", "14", "16", "18", "20", "24", "28", "36"})
        cboFontSize.SelectedIndex = 4 ' Default 12pt

        toolStrip.Items.AddRange({btnBold, btnItalic, btnUnderline, New ToolStripSeparator(), cboFontSize})
        toolStrip.Dock = DockStyle.Top

        ' RichTextBox
        rtb.Dock = DockStyle.Fill
        rtb.BorderStyle = BorderStyle.None

        ' Layout
        Me.Controls.Add(rtb)
        Me.Controls.Add(toolStrip)
        Me.BorderStyle = BorderStyle.FixedSingle
    End Sub

    ' Properties to expose
    Public Property Rtf As String
        Get
            Return rtb.Rtf
        End Get
        Set(value As String)
            Try
                If Not String.IsNullOrEmpty(value) AndAlso value.TrimStart().StartsWith("{\rtf") Then
                    rtb.Rtf = value
                Else
                    rtb.Text = If(value, String.Empty)
                End If
            Catch
                rtb.Text = If(value, String.Empty)
            End Try
        End Set
    End Property

    Public Shadows Property Text As String
        Get
            Return rtb.Text
        End Get
        Set(value As String)
            rtb.Text = value
        End Set
    End Property

    ' Button handlers
    Private Sub btnBold_Click(sender As Object, e As EventArgs) Handles btnBold.Click
        ToggleStyle(FontStyle.Bold)
    End Sub

    Private Sub btnItalic_Click(sender As Object, e As EventArgs) Handles btnItalic.Click
        ToggleStyle(FontStyle.Italic)
    End Sub

    Private Sub btnUnderline_Click(sender As Object, e As EventArgs) Handles btnUnderline.Click
        ToggleStyle(FontStyle.Underline)
    End Sub

    Private Sub cboFontSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFontSize.SelectedIndexChanged
        If rtb.SelectionFont IsNot Nothing AndAlso cboFontSize.SelectedItem IsNot Nothing Then
            Dim size As Single = CSng(cboFontSize.SelectedItem)
            rtb.SelectionFont = New Font(rtb.SelectionFont.FontFamily, size, rtb.SelectionFont.Style)
        End If
        rtb.Focus()
    End Sub

    Private Sub ToggleStyle(style As FontStyle)
        If rtb.SelectionFont Is Nothing Then Exit Sub
        Dim currentStyle = rtb.SelectionFont.Style
        Dim newStyle = If((currentStyle And style) = style, currentStyle And Not style, currentStyle Or style)
        rtb.SelectionFont = New Font(rtb.SelectionFont, newStyle)
        rtb.Focus()
    End Sub

    ' Keyboard shortcuts
    Private Sub rtb_KeyDown(sender As Object, e As KeyEventArgs) Handles rtb.KeyDown
        If e.Control Then
            Select Case e.KeyCode
                Case Keys.B : ToggleStyle(FontStyle.Bold) : e.SuppressKeyPress = True
                Case Keys.I : ToggleStyle(FontStyle.Italic) : e.SuppressKeyPress = True
                Case Keys.U : ToggleStyle(FontStyle.Underline) : e.SuppressKeyPress = True
            End Select
        End If
    End Sub
End Class