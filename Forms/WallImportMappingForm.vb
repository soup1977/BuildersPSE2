Option Strict On
Imports System.ComponentModel

Public Class WallImportMappingForm

    ' Holds the final user-approved mapping: CSV Key → BuildingID (-1 = create new)
    Public ReadOnly Property FinalMapping As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
    Public Property CreateMissingLevels As Boolean = True

    Private ReadOnly _csvKeys As HashSet(Of String)
    Private ReadOnly _existingBuildings As List(Of BuildingOption)
    Private ReadOnly _autoMatchDict As Dictionary(Of String, Integer)

    Private Class BuildingOption
        Public Property BuildingID As Integer
        Public Property BuildingName As String

        Public Overrides Function ToString() As String
            Return BuildingName
        End Function
    End Class

    Public Sub New(csvKeys As HashSet(Of String),
                   existingBuildingList As List(Of ValueTuple(Of Integer, String)),
                   autoMatchLookup As Dictionary(Of String, Integer),
                   wallRowCount As Integer)

        InitializeComponent()

        _csvKeys = csvKeys
        _autoMatchDict = autoMatchLookup

        ' Build full list of target options
        _existingBuildings = New List(Of BuildingOption) From {
            New BuildingOption With {.BuildingID = -1, .BuildingName = "[Create New Building...]"}
        }
        For Each b In existingBuildingList
            _existingBuildings.Add(New BuildingOption With {
                .BuildingID = b.Item1,
                .BuildingName = b.Item2
            })
        Next

        lblHeader.Text = $"Found {csvKeys.Count} unique building group(s) across {wallRowCount} wall rows. " &
                         "Please confirm or adjust where each should be imported:"

        SetupDataGrid()
        PopulateGrid()
    End Sub

    Private Sub SetupDataGrid()
        dgvMapping.AutoGenerateColumns = False
        dgvMapping.Columns.Clear()

        ' Column 0 - CSV Key (read-only)
        Dim colKey As New DataGridViewTextBoxColumn With {
            .Name = "colCsvKey",
            .DataPropertyName = "CsvKey",
            .HeaderText = "CSV Building Key",
            .Width = 200,
            .ReadOnly = True
        }

        ' Column 1 - Arrow (read-only)
        Dim colArrow As New DataGridViewTextBoxColumn With {
            .Name = "colArrow",
            .HeaderText = "→",
            .Width = 50,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle With {.ForeColor = Color.Gray, .Font = New Font(dgvMapping.Font, FontStyle.Bold)}
        }

        ' Column 2 - Target ComboBox
        Dim colTarget As New DataGridViewComboBoxColumn With {
            .Name = "colTarget",
            .DataPropertyName = "TargetBuildingID",
            .HeaderText = "Target Building",
            .DataSource = _existingBuildings,
            .DisplayMember = "BuildingName",
            .ValueMember = "BuildingID",
            .FlatStyle = FlatStyle.Flat,
            .DropDownWidth = 320
        }

        dgvMapping.Columns.AddRange(colKey, colArrow, colTarget)
        dgvMapping.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub

    Private Sub PopulateGrid()
        Dim dt As New DataTable()
        dt.Columns.Add("CsvKey", GetType(String))
        dt.Columns.Add("TargetBuildingID", GetType(Integer))

        For Each key In _csvKeys
            Dim suggestedID As Integer = -1 ' default = create new

            If _autoMatchDict.ContainsKey(key.Split("_"c).Last()) Then
                suggestedID = _autoMatchDict(key.Split("_"c).Last())
            ElseIf _autoMatchDict.ContainsKey(key) Then
                suggestedID = _autoMatchDict(key)
            End If

            dt.Rows.Add(key, suggestedID)
        Next

        dgvMapping.DataSource = dt

        ' Format arrow column
        For Each row As DataGridViewRow In dgvMapping.Rows
            row.Cells("colArrow").Value = "→"
        Next

        ' Auto-select "Create New" if no match
        For Each row As DataGridViewRow In dgvMapping.Rows
            Dim cell = DirectCast(row.Cells("colTarget"), DataGridViewComboBoxCell)
            If CInt(cell.Value) = -1 Then
                cell.Value = -1 ' force [Create New]
            End If
        Next
    End Sub

    Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
        dgvMapping.EndEdit()

        FinalMapping.Clear()
        For Each row As DataGridViewRow In dgvMapping.Rows
            If row.IsNewRow Then Continue For

            Dim csvKey = CStr(row.Cells("colCsvKey").Value)
            Dim targetID As Integer = CInt(row.Cells("colTarget").Value)

            FinalMapping(csvKey) = targetID
        Next

        CreateMissingLevels = chkCreateLevels.Checked
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub WallImportMappingForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        chkCreateLevels.Checked = True
    End Sub
End Class