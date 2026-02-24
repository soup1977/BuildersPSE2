Option Strict On
Imports System.Data.SqlClient
Imports System.Globalization
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Services
Imports BuildersPSE2.Utilities
Imports Microsoft.Reporting.WinForms

''' <summary>
''' Form for creating new projects and editing existing project data.
''' Manages project information, versions, buildings, levels, and associated data.
''' </summary>
Public Class frmCreateEditProject

#Region "Fields"

    ' Data Access
    Private ReadOnly da As New ProjectDataAccess()

    ' State Management
    Private currentProject As ProjectModel
    Private currentVersionID As Integer
    Private isNewProject As Boolean = True
    Private isChangingVersion As Boolean = False
    Private isLoadingData As Boolean = False
    Private rollupCleared As Boolean = False

    ' Services
    Private ReadOnly rollupService As New RollupCalculationService()
    Private ReadOnly rollupFormatter As New RollupFormatter()
    Private varianceService As VarianceGridService
    Private ReadOnly varianceFormatter As New VarianceGridFormatter()

    ' Clipboard
    Private copiedLevels As List(Of LevelModel)
    Private copiedBuilding As BuildingModel

    ' Caching
    Private projectVersions As New List(Of ProjectVersionModel)()
    Private projectVersionsProjectId As Integer

    ' Form References
    Private ReadOnly _mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)

    ' Version Lock State
    Private isVersionLocked As Boolean = False
    Private btnAdminUnlock As Button ' Will be created dynamically

#End Region

#Region "Nested Classes"

    ''' <summary>
    ''' Represents a lumber history date entry for display in the list box.
    ''' </summary>
    Private Class LumberHistoryDate
        Public Property DisplayText As String
        Public Property CostEffectiveID As Integer

        Public Overrides Function ToString() As String
            Return DisplayText
        End Function
    End Class

#End Region

#Region "Constructor"

    ''' <summary>
    ''' Initializes a new instance of the form for creating or editing a project.
    ''' </summary>
    ''' <param name="selectedProj">Existing project to edit, or Nothing to create a new project.</param>
    ''' <param name="versionID">Version ID to load, or 0 for default version.</param>
    Public Sub New(Optional selectedProj As ProjectModel = Nothing, Optional versionID As Integer = 0)
        InitializeComponent()
        InitializeComboBoxes()
        InitializeProject(selectedProj)
        currentVersionID = versionID
        InitializeTabControl()
        LoadVersions()
        LoadProjectData()
        SetFormTitle()

        ' CHECK LOCK STATE ON FORM LOAD
        ApplyVersionLockState()

    End Sub

#End Region

#Region "Initialization"

    ''' <summary>
    ''' Initializes all combo boxes with their data sources.
    ''' </summary>
    Private Sub InitializeComboBoxes()
        Try
            cboCustomer.DataSource = HelperDataAccess.GetCustomers(customerType:=1)
            cboCustomer.DisplayMember = "CustomerName"
            cboCustomer.ValueMember = "CustomerID"

            cboSalesman.DataSource = HelperDataAccess.GetSales()
            cboSalesman.DisplayMember = "SalesName"
            cboSalesman.ValueMember = "SalesID"

            cboProjectType.DataSource = da.GetProjectTypes()
            cboProjectType.DisplayMember = "ProjectTypeName"
            cboProjectType.ValueMember = "ProjectTypeID"

            cboProjectStatus.DataSource = da.GetProjectVersionStatus()
            cboProjectStatus.DisplayMember = "ProjVersionStatus"
            cboProjectStatus.ValueMember = "ProjVersionStatusID"

            cboEstimator.DataSource = HelperDataAccess.GetEstimators()
            cboEstimator.DisplayMember = "EstimatorName"
            cboEstimator.ValueMember = "EstimatorID"

            cboProjectArchitect.DataSource = HelperDataAccess.GetCustomers(customerType:=2)
            cboProjectArchitect.DisplayMember = "CustomerName"
            cboProjectArchitect.ValueMember = "CustomerID"

            cboProjectEngineer.DataSource = HelperDataAccess.GetCustomers(customerType:=3)
            cboProjectEngineer.DisplayMember = "CustomerName"
            cboProjectEngineer.ValueMember = "CustomerID"

            cboVersion.DataSource = New List(Of ProjectVersionModel)()
            cboVersion.DisplayMember = "VersionName"
            cboVersion.ValueMember = "VersionID"

            cmbLevelType.DataSource = da.GetProductTypes()
            cmbLevelType.DisplayMember = "ProductTypeName"
            cmbLevelType.ValueMember = "ProductTypeID"
        Catch ex As Exception
            UIHelper.Add($"Error initializing combo boxes: {ex.Message}")
            MessageBox.Show($"Error initializing combo boxes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the project model for new or existing projects.
    ''' </summary>
    ''' <param name="selectedProj">The project to edit, or Nothing for a new project.</param>
    Private Sub InitializeProject(selectedProj As ProjectModel)
        Try
            If selectedProj IsNot Nothing Then
                currentProject = selectedProj
                isNewProject = False
            Else
                currentProject = New ProjectModel With {
                    .CreatedDate = DateTime.Today,
                    .LastModifiedDate = DateTime.Now
                }
                InitializeProjectSettings()
            End If

            ' Configure date pickers
            txtEngPlanDate.PromptChar = " "c
            txtArchPlanDate.PromptChar = " "c
            txtEngPlanDate.ValidatingType = GetType(Date)
            txtArchPlanDate.ValidatingType = GetType(Date)

            ' Configure admin-only features
            If CurrentUser.IsAdmin Then
                btnImportBisTrack.Visible = True
                btnImportMiTek.Visible = True
                btnLinkMonday.Visible = True
                tabOverrides.Visible = True
            Else
                btnImportBisTrack.Visible = False
                btnImportMiTek.Visible = False
                btnLinkMonday.Visible = False
                tabOverrides.Visible = False
            End If
        Catch ex As Exception
            UIHelper.Add($"Error initializing project: {ex.Message}")
            MessageBox.Show($"Error initializing project: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes default project settings for a new project.
    ''' Creates settings entries for each product type with default values.
    ''' </summary>
    Private Sub InitializeProjectSettings()
        Dim productTypes As List(Of ProductTypeModel) = da.GetProductTypes()
        For Each pt In productTypes
            currentProject.Settings.Add(New ProjectProductSettingsModel With {
                .ProductTypeID = pt.ProductTypeID,
                .MarginPercent = 0,
                .LumberAdder = 0,
                .VersionID = 0
            })
        Next
    End Sub

    ''' <summary>
    ''' Initializes the tab control based on project state (new vs existing).
    ''' </summary>
    Private Sub InitializeTabControl()
        Try
            tabControlRight.TabPages.Clear()
            tabControlRight.TabPages.Add(tabProjectInfo)
            tabControlRight.TabPages.Add(tabRollup)

            If CurrentUser.IsAdmin Then
                dgvLevelVariance.Visible = True
            End If

            If Not isNewProject Then
                tabControlRight.TabPages.Add(tabOverrides)
                tabControlRight.TabPages.Add(tabBuildingInfo)
                tabControlRight.TabPages.Add(tabLevelInfo)
            End If

            UIHelper.Add($"Initialized {tabControlRight.TabPages.Count} tab(s) for {(If(isNewProject, "new project", $"project ID {currentProject.ProjectID}"))}")
        Catch ex As Exception
            UIHelper.Add($"Error initializing tab control: {ex.Message}")
            MessageBox.Show($"Error initializing tab control: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Sets the form title based on whether this is a new or existing project.
    ''' </summary>
    Private Sub SetFormTitle()
        If isNewProject Then
            Me.Text = "Create Project"
        Else
            Me.Text = $"Edit Project - {currentProject.JBID}"
        End If
    End Sub

#End Region

#Region "Data Loading - Project & Versions"

    ''' <summary>
    ''' Loads project data and associated information into the form.
    ''' </summary>
    Private Sub LoadProjectData()
        Try
            LoadProjectInfo(currentProject)
            LoadVersionSpecificData()
            UIHelper.Add($"Loaded data for {(If(isNewProject, "new project", $"project ID {currentProject.ProjectID}"))}")
        Catch ex As Exception
            UIHelper.Add($"Error loading project data: {ex.Message}")
            MessageBox.Show($"Error loading project data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Loads project information into the UI controls.
    ''' </summary>
    ''' <param name="proj">The project model to load.</param>
    Private Sub LoadProjectInfo(proj As ProjectModel)
        Try
            PopulateProjectInfoControls(proj)
        Catch ex As Exception
            UIHelper.Add($"Error loading project info: {ex.Message}")
            MessageBox.Show($"Error loading project info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Populates project info controls with data from the project model.
    ''' </summary>
    ''' <param name="proj">The project model containing the data.</param>
    Private Sub PopulateProjectInfoControls(proj As ProjectModel)
        txtJBID.Text = proj.JBID
        cboProjectType.SelectedValue = proj.ProjectType.ProjectTypeID
        txtProjectName.Text = proj.ProjectName
        cboEstimator.SelectedValue = proj.Estimator.EstimatorID
        txtAddress.Text = proj.Address
        txtCity.Text = proj.City
        cboState.Text = proj.State
        txtZip.Text = proj.Zip

        If proj.BidDate.HasValue Then
            dtpBidDate.Value = proj.BidDate.Value
        End If

        If proj.ArchPlansDated.HasValue Then
            txtArchPlanDate.Text = proj.ArchPlansDated.Value.ToString("MMddyyyy")
        End If

        If proj.EngPlansDated.HasValue Then
            txtEngPlanDate.Text = proj.EngPlansDated.Value.ToString("MMddyyyy")
        End If

        nudMilesToJobSite.Value = proj.MilesToJobSite
        nudTotalNetSqft.Value = If(proj.TotalNetSqft, 0)
        nudTotalGrossSqft.Value = If(proj.TotalGrossSqft, 0)
        cboProjectArchitect.SelectedValue = If(proj.ArchitectID, 0)
        cboProjectEngineer.SelectedValue = If(proj.EngineerID, 0)
        txtProjectNotes.Text = proj.ProjectNotes
        dtpCreatedDate.Value = proj.CreatedDate
        dtpLastModified.Value = proj.LastModifiedDate
    End Sub

    ''' <summary>
    ''' Loads all project versions and initializes version-specific data.
    ''' </summary>
    Private Sub LoadVersions()
        Try
            If currentProject.ProjectID = 0 Then
                ClearVersionComboAndTree()
                Return
            End If

            BindVersionsToCombo()
            LoadCostEffectiveDates()
            LoadVersionSpecificData()
        Catch ex As Exception
            isChangingVersion = False
            UIHelper.Add("Status: Error loading versions: " & ex.Message)
            MessageBox.Show("Error loading versions: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Loads all data specific to the current version.
    ''' </summary>
    Private Sub LoadVersionSpecificData()
        Try
            UIHelper.Add("Loading version...")
            varianceService = New VarianceGridService(currentVersionID)

            LoadProjectSettingsAndOverrides()
            LoadProjectBuildings()

            isLoadingData = True
            RefreshProjectTree()
            isLoadingData = False

            SetVersionComboBoxes()
            LoadAverageSPFPrices()
            LoadActiveSPFPrices()
            LoadLumberHistory()

            Dim selectedVersion As ProjectVersionModel = GetCurrentVersionModel()
            UIHelper.Add($"Loaded version {(If(selectedVersion IsNot Nothing, selectedVersion.VersionName, "No Version"))}")

            LoadFuturesIntoListBox(currentVersionID)

            ' APPLY LOCK STATE AFTER LOADING DATA
            ApplyVersionLockState()

        Catch ex As Exception
            isLoadingData = False
            UIHelper.Add($"Error loading version-specific data: {ex.Message}")
            MessageBox.Show("Error loading version-specific data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Clears the version combo box and project tree for new projects.
    ''' </summary>
    Private Sub ClearVersionComboAndTree()
        cboVersion.DataSource = New List(Of ProjectVersionModel)()
        UIHelper.Add("Status: Save the project to create a version.")
        tvProjectTree.Nodes.Clear()
        tvProjectTree.Nodes.Add(New TreeNode(currentProject.ProjectName & "-No Version") With {.Tag = currentProject})
    End Sub

    ''' <summary>
    ''' Binds project versions to the version combo box and sets the current selection.
    ''' </summary>
    Private Sub BindVersionsToCombo()
        isChangingVersion = True
        Dim versions As List(Of ProjectVersionModel) = GetProjectVersions(True)
        cboVersion.DataSource = versions
        cboVersion.DisplayMember = "VersionName"
        cboVersion.ValueMember = "VersionID"

        If currentVersionID > 0 Then
            cboVersion.SelectedValue = currentVersionID
        ElseIf versions.Any() Then
            cboVersion.SelectedIndex = 0
            currentVersionID = CInt(cboVersion.SelectedValue)
        Else
            currentVersionID = 0
            UIHelper.Add("Status: No versions found. Save the project to create a base version.")
        End If

        isChangingVersion = False
    End Sub

    ''' <summary>
    ''' Sets combo box values based on the selected version.
    ''' </summary>
    Private Sub SetVersionComboBoxes()
        Dim selectedVersion As ProjectVersionModel = GetCurrentVersionModel()
        If selectedVersion IsNot Nothing Then
            cboCustomer.SelectedValue = If(selectedVersion.CustomerID, 0)
            cboSalesman.SelectedValue = If(selectedVersion.SalesID, 0)
            cboVersion.SelectedValue = currentVersionID
            txtMondayItemId.Text = selectedVersion.MondayID
            cboProjectStatus.SelectedValue = selectedVersion.ProjVersionStatusID
        End If
    End Sub

    ''' <summary>
    ''' Loads project settings and applies overrides for the current version.
    ''' </summary>
    Private Sub LoadProjectSettingsAndOverrides()
        currentProject.Settings = ProjectDataAccess.GetProjectProductSettings(currentVersionID)
        LoadOverrides(currentProject.Settings)
    End Sub

    ''' <summary>
    ''' Loads buildings for the current version.
    ''' </summary>
    Public Sub LoadProjectBuildings()
        currentProject.Buildings = ProjectDataAccess.GetBuildingsByVersionID(currentVersionID)
    End Sub

#End Region

#Region "Data Loading - Buildings & Levels"

    ''' <summary>
    ''' Loads building information into the UI controls.
    ''' </summary>
    ''' <param name="bldg">The building model to load.</param>
    Private Sub LoadBuildingInfo(bldg As BuildingModel)
        Try
            PopulateBuildingInfoControls(bldg)
        Catch ex As Exception
            UIHelper.Add($"Error loading building info: {ex.Message}")
            MessageBox.Show($"Error loading building info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Populates building info controls with data from the building model.
    ''' </summary>
    ''' <param name="bldg">The building model containing the data.</param>
    Private Sub PopulateBuildingInfoControls(bldg As BuildingModel)
        txtBuildingName.Text = bldg.BuildingName
        nudBldgQty.Value = bldg.BldgQty
        txtBuildingType.Text = If(bldg.BuildingType.HasValue, bldg.BuildingType.Value.ToString(), String.Empty)
        txtResUnits.Text = If(bldg.ResUnits.HasValue, bldg.ResUnits.Value.ToString(), String.Empty)

        If bldg.Levels IsNot Nothing Then
            nudNbrUnits.Value = bldg.Levels.Sum(Function(l) l.ActualUnitMappings.Sum(Function(m) m.Quantity))
        Else
            nudNbrUnits.Value = 0
        End If
    End Sub

    ''' <summary>
    ''' Loads level information into the UI controls.
    ''' </summary>
    ''' <param name="level">The level model to load.</param>
    Private Sub LoadLevelInfo(level As LevelModel)
        Try
            PopulateLevelInfoControls(level)
        Catch ex As Exception
            UIHelper.Add($"Error loading level info: {ex.Message}")
            MessageBox.Show($"Error loading level info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Populates level info controls with data from the level model.
    ''' </summary>
    ''' <param name="level">The level model containing the data.</param>
    Private Sub PopulateLevelInfoControls(level As LevelModel)
        txtLevelName.Text = level.LevelName
        cmbLevelType.SelectedValue = level.ProductTypeID
        nudLevelNumber.Value = level.LevelNumber
    End Sub

#End Region

#Region "Data Loading - Overrides & Pricing"

    ''' <summary>
    ''' Loads override settings into the grid.
    ''' </summary>
    ''' <param name="settings">The settings to display.</param>
    Private Sub LoadOverrides(settings As List(Of ProjectProductSettingsModel))
        Try
            BindOverridesGrid(settings)
            ConfigureOverridesGridColumns()
        Catch ex As Exception
            UIHelper.Add($"Error loading overrides: {ex.Message}")
            MessageBox.Show($"Error loading overrides: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Binds settings to the overrides data grid.
    ''' </summary>
    ''' <param name="settings">The settings to bind.</param>
    Private Sub BindOverridesGrid(settings As List(Of ProjectProductSettingsModel))
        dgvOverrides.DataSource = settings
    End Sub

    ''' <summary>
    ''' Configures columns for the overrides grid including formatting and visibility.
    ''' </summary>
    Private Sub ConfigureOverridesGridColumns()
        If dgvOverrides.Columns("ProductTypeName") Is Nothing Then
            Dim col As New DataGridViewComboBoxColumn With {
                .DataSource = da.GetProductTypes(),
                .DisplayMember = "ProductTypeName",
                .ValueMember = "ProductTypeID",
                .DataPropertyName = "ProductTypeID",
                .HeaderText = "Product Type",
                .Name = "ProductTypeName"
            }
            dgvOverrides.Columns.Add(col)
        End If

        For Each col As DataGridViewColumn In dgvOverrides.Columns
            Select Case col.Name
                Case "MarginPercent"
                    col.HeaderText = "Margin %"
                    col.DefaultCellStyle.Format = "P2"
                    col.Visible = True
                Case "LumberAdder"
                    col.HeaderText = "Futures Adder/MBF"
                    col.DefaultCellStyle.Format = "C2"
                    col.Visible = True
                    col.Width = 150
                Case "ProductTypeName"
                    col.Visible = True
                Case Else
                    col.Visible = False
                    col.ReadOnly = True
            End Select
        Next

        dgvOverrides.AllowUserToAddRows = True
    End Sub

    ''' <summary>
    ''' Loads average SPF#2 prices for floors and roofs.
    ''' </summary>
    Private Sub LoadAverageSPFPrices()
        txtRawFloorSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetAverageSPFNo2ByProductType(currentVersionID, "Floor").ToString("F2"), "0.00")
        txtRawRoofSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetAverageSPFNo2ByProductType(currentVersionID, "Roof").ToString("F2"), "0.00")
    End Sub

    ''' <summary>
    ''' Loads active SPF#2 prices for floors and roofs.
    ''' </summary>
    Private Sub LoadActiveSPFPrices()
        txtActiveFloorSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetActiveSPFNo2ByProductType(currentVersionID, "Floor").ToString("F2"), "0.00")
        txtActiveRoofSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetActiveSPFNo2ByProductType(currentVersionID, "Roof").ToString("F2"), "0.00")
    End Sub

    ''' <summary>
    ''' Loads cost-effective dates into the combo box.
    ''' </summary>
    Private Sub LoadCostEffectiveDates()
        Dim dtCostEffective As DataTable = LumberDataAccess.GetAllLumberCostEffective()
        cboCostEffective.DataSource = dtCostEffective
        cboCostEffective.DisplayMember = "CosteffectiveDate"
        cboCostEffective.ValueMember = "CostEffectiveID"

        If dtCostEffective.Rows.Count > 0 Then
            cboCostEffective.SelectedIndex = 0
        End If
    End Sub

    ''' <summary>
    ''' Loads lumber history dates into the list box.
    ''' </summary>
    Private Sub LoadLumberHistory()
        If currentVersionID > 0 Then
            Dim dtHistory As New DataTable()
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", currentVersionID)}

            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Dim cmd As New SqlCommand(Queries.SelectDistinctLumberHistoryDates, conn)
                cmd.Parameters.AddRange(params)
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dtHistory)
            End Using

            lstLumberHistory.Items.Clear()

            For Each row As DataRow In dtHistory.Rows
                Dim costEffectiveID As Integer = CInt(row("CostEffectiveDateID"))
                Dim costEffectiveDate As Date = CDate(row("CosteffectiveDate"))
                Dim isActive As Boolean = CBool(row("IsActive"))
                Dim updateDate As Date = CDate(row("UpdateDate"))
                Dim displayText As String = $"{costEffectiveDate:yyyy-MM-dd}{(If(isActive, " (Active)", ""))} (Updated: {updateDate:yyyy-MM-dd HH:mm:ss})"

                lstLumberHistory.Items.Add(New LumberHistoryDate With {
                    .DisplayText = displayText,
                    .CostEffectiveID = costEffectiveID
                })
            Next

            If lstLumberHistory.Items.Count > 0 Then
                lstLumberHistory.SelectedIndex = 0
            End If
        Else
            lstLumberHistory.Items.Clear()
        End If
    End Sub

#End Region

#Region "Data Loading - Rollup & Variance"

    ''' <summary>
    ''' Loads rollup data for the currently selected tree node or defaults to project.
    ''' </summary>
    Private Sub LoadRollupForSelectedNode()
        If tvProjectTree.SelectedNode IsNot Nothing Then
            LoadRollup(tvProjectTree.SelectedNode.Tag)
        Else
            LoadRollup(currentProject)
        End If
    End Sub

    ''' <summary>
    ''' Loads and displays rollup data for a project, building, or level.
    ''' </summary>
    ''' <param name="item">The project, building, or level to calculate rollup for.</param>
    Private Sub LoadRollup(item As Object)
        If rollupCleared Then Exit Sub

        Try
            Dim result As RollupResult = Nothing

            Select Case True
                Case TypeOf item Is ProjectModel
                    Dim currentVersion As ProjectVersionModel = GetCurrentVersionModel()
                    result = rollupService.CalculateProjectRollup(CType(item, ProjectModel), currentVersion)
                Case TypeOf item Is BuildingModel
                    result = rollupService.CalculateBuildingRollup(CType(item, BuildingModel))
                Case TypeOf item Is LevelModel
                    Dim level As LevelModel = CType(item, LevelModel)
                    Dim parentBldg As BuildingModel = currentProject.Buildings.FirstOrDefault(Function(b) b.Levels.Contains(level))
                    Dim bldgQty As Integer = If(parentBldg IsNot Nothing, parentBldg.BldgQty, 1)
                    result = rollupService.CalculateLevelRollup(level, bldgQty)
                Case Else
                    result = New RollupResult With {
                        .Type = RollupType.Project,
                        .HasData = False,
                        .StatusMessage = "Unsupported selection."
                    }
            End Select

            RenderRollup(result)
        Catch ex As Exception
            UIHelper.Add($"Error loading rollup: {ex.Message}")
            MessageBox.Show($"Error loading rollup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Renders rollup results to the data grid.
    ''' </summary>
    ''' <param name="result">The rollup result to display.</param>
    Private Sub RenderRollup(result As RollupResult)
        Dim formattedGrid As DataGridView = rollupFormatter.FormatForGrid(result)

        dgvRollup.Rows.Clear()
        dgvRollup.Columns.Clear()

        For Each sourceColumn As DataGridViewColumn In formattedGrid.Columns
            Dim cloned As DataGridViewColumn = CType(sourceColumn.Clone(), DataGridViewColumn)
            cloned.HeaderText = sourceColumn.HeaderText
            cloned.Name = sourceColumn.Name
            dgvRollup.Columns.Add(cloned)
        Next

        For Each sourceRow As DataGridViewRow In formattedGrid.Rows
            If sourceRow.IsNewRow Then Continue For
            Dim values(sourceRow.Cells.Count - 1) As Object
            For i As Integer = 0 To sourceRow.Cells.Count - 1
                values(i) = sourceRow.Cells(i).Value
            Next
            dgvRollup.Rows.Add(values)
        Next

        dgvRollup.AutoSizeColumnsMode = formattedGrid.AutoSizeColumnsMode
    End Sub

    ''' <summary>
    ''' Refreshes the level variance grid for the currently selected level.
    ''' </summary>
    Private Sub RefreshLevelVariance()
        Dim levelID As Integer = GetCurrentLevelID()
        If levelID <= 0 OrElse currentVersionID <= 0 Then
            dgvLevelVariance.DataSource = Nothing
            Exit Sub
        End If

        Try
            Dim dt = varianceService.BuildLevelVarianceTable(levelID)
            If dt Is Nothing Then
                dgvLevelVariance.DataSource = Nothing
                Exit Sub
            End If

            dgvLevelVariance.DataSource = dt
            varianceFormatter.FormatVarianceGrid(dgvLevelVariance)
        Catch ex As Exception
            UIHelper.Add($"Error loading level variance: {ex.Message}")
            dgvLevelVariance.DataSource = Nothing
        End Try
    End Sub

    ''' <summary>
    ''' Refreshes the building variance grid for the currently selected building.
    ''' Only displays data if all actuals are complete (LevelActuals count = Bistrack matched count = Expected count).
    ''' </summary>
    Private Sub RefreshBuildingVariance()
        Dim buildingID As Integer = GetCurrentBuildingID()
        If buildingID <= 0 OrElse currentVersionID <= 0 Then
            dgvBuildingVariance.DataSource = Nothing
            dgvBuildingVariance.Visible = False
            lblBuildingVarianceStatus.Visible = False
            Exit Sub
        End If

        Try
            ' Validate if we can show building variance
            'Dim validationResult = BisTrackDataAccess.GetBuildingVarianceValidationInfo(buildingID, currentVersionID)
            Dim validationResult = New With {.IsValid = True}


            If Not validationResult.IsValid Then
                ' Show status message instead of grid
                dgvBuildingVariance.DataSource = Nothing
                dgvBuildingVariance.Visible = False
                ' lblBuildingVarianceStatus.Text = validationResult.GetStatusMessage()
                lblBuildingVarianceStatus.Visible = True
                'UIHelper.Add($"Building variance hidden: {validationResult.GetStatusMessage()}")
                Exit Sub
            End If

            ' All counts match - build and display the variance table
            Dim dt = varianceService.BuildBuildingVarianceTable(buildingID)
            If dt Is Nothing Then
                dgvBuildingVariance.DataSource = Nothing
                dgvBuildingVariance.Visible = False
                lblBuildingVarianceStatus.Text = "No variance data available"
                lblBuildingVarianceStatus.Visible = True
                Exit Sub
            End If

            dgvBuildingVariance.DataSource = dt
            varianceFormatter.FormatVarianceGrid(dgvBuildingVariance)
            dgvBuildingVariance.Refresh()
            dgvBuildingVariance.Visible = True
            lblBuildingVarianceStatus.Visible = False

            ' UIHelper.Add($"Building variance displayed: {validationResult.ExpectedCount} levels complete")

        Catch ex As Exception
            UIHelper.Add($"Error loading building variance: {ex.Message}")
            dgvBuildingVariance.DataSource = Nothing
            dgvBuildingVariance.Visible = False
            lblBuildingVarianceStatus.Text = $"Error: {ex.Message}"
            lblBuildingVarianceStatus.Visible = True
        End Try
    End Sub

    ''' <summary>
    ''' Clears the rollup display to prevent showing stale data.
    ''' </summary>
    Private Sub ClearRollupDisplay()
        rollupCleared = True
        dgvRollup.SuspendLayout()
        dgvRollup.DataSource = Nothing
        dgvRollup.Rows.Clear()
        dgvRollup.Columns.Clear()
        dgvRollup.ResumeLayout()
        dgvLevelVariance.DataSource = Nothing
        dgvBuildingVariance.DataSource = Nothing
        UIHelper.Add("Rollup display cleared - data may be modified in PSE form.")
    End Sub

#End Region

#Region "Data Loading - Lumber Futures"

    ''' <summary>
    ''' Loads lumber futures data into the list view for the specified version.
    ''' </summary>
    ''' <param name="versionId">The version ID to load futures for.</param>
    Private Sub LoadFuturesIntoListBox(versionId As Integer)
        lvFutures.Items.Clear()
        lvFutures.Groups.Clear()
        lvFutures.BeginUpdate()

        Try
            If versionId <= 0 Then Return

            Dim futures = LumberDataAccess.GetFuturesForVersion(versionId)
            If futures.Count = 0 Then Return

            Dim groups = futures.GroupBy(Function(f) f.PullDate.Date)

            For Each grp In groups.OrderByDescending(Function(g) g.Key)
                Dim groupDateStr = grp.Key.ToString("MMMM d, yyyy")
                Dim header = $"Pulled on {groupDateStr} ({grp.Count()} contracts)"
                Dim lvg As New ListViewGroup(grp.Key.ToString("yyyyMMdd"), header)
                lvFutures.Groups.Add(lvg)

                For Each f In grp.OrderBy(Function(x) LumberDataAccess.MonthNameToNumber(x.ContractMonth.Split(" "c)(0)) * 100 + CInt(x.ContractMonth.Split(" "c)(1)))
                    Dim lvi As New ListViewItem(New String() {
                        f.ContractMonth,
                        If(f.PriorSettle.HasValue, f.PriorSettle.Value.ToString("N2"), "—"),
                        f.PullDate.ToString("MM/dd/yyyy"),
                        If(f.Active, "x", "")
                    }) With {
                        .Tag = f,
                        .Group = lvg
                    }

                    If f.Active Then
                        lvi.BackColor = Color.LightGreen
                        lvi.Font = New Font(lvi.Font, FontStyle.Bold)
                    End If

                    lvFutures.Items.Add(lvi)
                Next
            Next

            If lvFutures.Groups.Count > 0 Then
                lvFutures.Groups(0).Header = lvFutures.Groups(0).Header.Substring(9)
            End If

            SelectNearestExpiryContract()
        Finally
            lvFutures.EndUpdate()
        End Try
    End Sub

    ''' <summary>
    ''' Auto-selects the nearest expiry contract in the futures list.
    ''' </summary>
    Private Sub SelectNearestExpiryContract()
        If lvFutures.Items.Count = 0 Then Return

        Dim today = DateTime.Today
        Dim bestItem As ListViewItem = Nothing
        Dim bestDiff As Double = Double.MaxValue

        For Each item As ListViewItem In lvFutures.Items
            Dim f As LumberFutures = CType(item.Tag, LumberFutures)
            Dim parts = f.ContractMonth.Split(" "c)

            If parts.Length = 2 AndAlso Integer.TryParse(parts(1), Nothing) Then
                Dim m = LumberDataAccess.MonthNameToNumber(parts(0))
                Dim y = CInt(parts(1))
                Dim yr = If(y < 50, 2000 + y, 1900 + y)

                If m > 0 Then
                    Dim contractDate = New DateTime(yr, m, 1)
                    Dim diff = Math.Abs((contractDate - today).TotalDays)

                    If diff < bestDiff Then
                        bestDiff = diff
                        bestItem = item
                    End If
                End If
            End If
        Next

        If bestItem IsNot Nothing Then
            bestItem.Selected = True
            bestItem.EnsureVisible()
        End If
    End Sub

#End Region

#Region "Data Saving"

    ''' <summary>
    ''' Updates the project model with data from UI controls and saves to database.
    ''' </summary>
    Private Sub UpdateProjectFromControls()
        currentProject.JBID = txtJBID.Text
        currentProject.ProjectType.ProjectTypeID = CInt(cboProjectType.SelectedValue)
        currentProject.ProjectType.ProjectTypeName = cboProjectType.Text
        currentProject.ProjectName = txtProjectName.Text
        currentProject.Estimator.EstimatorID = CInt(cboEstimator.SelectedValue)
        currentProject.Estimator.EstimatorName = cboEstimator.Text
        currentProject.Address = txtAddress.Text
        currentProject.City = txtCity.Text
        currentProject.State = cboState.Text
        currentProject.Zip = txtZip.Text
        currentProject.BidDate = dtpBidDate.Value
        currentProject.ArchPlansDated = TextToNullableDate(txtArchPlanDate)
        currentProject.EngPlansDated = TextToNullableDate(txtEngPlanDate)
        currentProject.MilesToJobSite = CInt(nudMilesToJobSite.Value)
        currentProject.TotalNetSqft = If(nudTotalNetSqft.Value > 0, CInt(nudTotalNetSqft.Value), Nothing)
        currentProject.TotalGrossSqft = CInt(nudTotalGrossSqft.Value)
        currentProject.ArchitectID = If(cboProjectArchitect.SelectedValue IsNot DBNull.Value, CInt(cboProjectArchitect.SelectedValue), Nothing)
        currentProject.EngineerID = If(cboProjectEngineer.SelectedValue IsNot DBNull.Value, CInt(cboProjectEngineer.SelectedValue), Nothing)
        currentProject.ProjectNotes = txtProjectNotes.Text
    End Sub

    ''' <summary>
    ''' Saves the project and creates/updates the version.
    ''' </summary>
    Private Sub SaveProjectAndVersion()
        da.SaveProject(currentProject)

        Dim customerID As Integer? = If(cboCustomer.SelectedValue IsNot DBNull.Value, CInt(cboCustomer.SelectedValue), Nothing)
        Dim salesID As Integer? = If(cboSalesman.SelectedValue IsNot DBNull.Value, CInt(cboSalesman.SelectedValue), Nothing)
        Dim projStatusID As Integer = If(cboProjectStatus.SelectedValue IsNot DBNull.Value, CInt(cboProjectStatus.SelectedValue), Nothing)

        If isNewProject AndAlso currentVersionID = 0 Then
            CreateInitialVersion(customerID, salesID)
        ElseIf currentVersionID > 0 Then
            ProjVersionDataAccess.UpdateProjectVersion(currentVersionID, cboVersion.Text, txtMondayItemId.Text, projStatusID, customerID, salesID)
        End If
    End Sub

    ''' <summary>
    ''' Creates the initial project version for a new project.
    ''' </summary>
    ''' <param name="customerID">The customer ID for the version.</param>
    ''' <param name="salesID">The salesperson ID for the version.</param>
    Private Sub CreateInitialVersion(customerID As Integer?, salesID As Integer?)
        currentVersionID = ProjVersionDataAccess.CreateProjectVersion(currentProject.ProjectID, "v1", "Base Version", customerID, salesID)

        Dim productTypes As List(Of ProductTypeModel) = da.GetProductTypes()
        For Each pt In productTypes
            Dim setting As New ProjectProductSettingsModel With {
                .VersionID = currentVersionID,
                .ProductTypeID = pt.ProductTypeID,
                .MarginPercent = 0,
                .LumberAdder = 0
            }
            da.SaveProjectProductSetting(setting, currentVersionID)
        Next

        isNewProject = False
    End Sub

    ''' <summary>
    ''' Saves the overrides from the grid to the database.
    ''' </summary>
    Private Sub SaveOverrides()
        currentProject.Settings = CType(dgvOverrides.DataSource, List(Of ProjectProductSettingsModel))
        For Each setting In currentProject.Settings
            setting.VersionID = currentVersionID
            da.SaveProjectProductSetting(setting, currentVersionID)
        Next
    End Sub

    ''' <summary>
    ''' Updates the building model with data from UI controls.
    ''' </summary>
    ''' <param name="bldg">The building model to update.</param>
    Private Sub UpdateBuildingFromControls(bldg As BuildingModel)
        bldg.BuildingName = txtBuildingName.Text
        bldg.BldgQty = CInt(nudBldgQty.Value)
    End Sub

    ''' <summary>
    ''' Updates the level model with data from UI controls.
    ''' </summary>
    ''' <param name="level">The level model to update.</param>
    Private Sub UpdateLevelFromControls(level As LevelModel)
        level.LevelName = txtLevelName.Text
        level.ProductTypeID = CInt(cmbLevelType.SelectedValue)
        level.ProductTypeName = CType(cmbLevelType.SelectedItem, ProductTypeModel).ProductTypeName
        level.LevelNumber = CInt(nudLevelNumber.Value)
    End Sub

    ''' <summary>
    ''' Saves the FuturesAdderAmt and FuturesAdderProjTotal to the database.
    ''' </summary>
    ''' <param name="futuresAdderAmt">The adder amount per MBF.</param>
    ''' <param name="futuresAdderProjTotal">The total project adder amount.</param>
    Private Sub SaveFuturesAdderValues(futuresAdderAmt As Decimal, futuresAdderProjTotal As Decimal)
        Dim currentVersion As ProjectVersionModel = GetCurrentVersionModel()
        If currentVersion Is Nothing Then
            Throw New InvalidOperationException("No current version selected.")
        End If

        Dim customerID As Integer? = If(cboCustomer.SelectedValue IsNot DBNull.Value AndAlso cboCustomer.SelectedValue IsNot Nothing,
                                        CInt(cboCustomer.SelectedValue), Nothing)
        Dim salesID As Integer? = If(cboSalesman.SelectedValue IsNot DBNull.Value AndAlso cboSalesman.SelectedValue IsNot Nothing,
                                      CInt(cboSalesman.SelectedValue), Nothing)
        Dim projStatusID As Integer? = If(cboProjectStatus.SelectedValue IsNot DBNull.Value AndAlso cboProjectStatus.SelectedValue IsNot Nothing,
                                           CInt(cboProjectStatus.SelectedValue), Nothing)

        ProjVersionDataAccess.UpdateProjectVersion(
            currentVersionID,
            cboVersion.Text,
            txtMondayItemId.Text,
            projStatusID,
            customerID,
            salesID,
            futuresAdderAmt,
            futuresAdderProjTotal)
    End Sub

#End Region

#Region "UI Management - Tree & Tabs"

    ''' <summary>
    ''' Refreshes the project tree view with current project data.
    ''' </summary>
    Public Sub RefreshProjectTree()
        tvProjectTree.Nodes.Clear()

        Dim selectedVersion As ProjectVersionModel = GetCurrentVersionModel()
        Dim versionName As String = selectedVersion?.VersionName
        Dim rootSuffix As String = If(String.IsNullOrWhiteSpace(versionName), "-No Version", "-" & versionName)
        Dim root As New TreeNode($"{currentProject.ProjectName}{rootSuffix}") With {.Tag = currentProject}

        tvProjectTree.Nodes.Add(root)

        For Each bldg In currentProject.Buildings
            Dim bldgNode As New TreeNode(String.Format("{0} ({1})", bldg.BuildingName, "x" & bldg.BldgQty)) With {.Tag = bldg}
            root.Nodes.Add(bldgNode)

            For Each level In bldg.Levels
                Dim levelNode As New TreeNode(String.Format("{0} ({1})", level.LevelName, level.ProductTypeName)) With {.Tag = level}
                bldgNode.Nodes.Add(levelNode)
            Next
        Next

        tvProjectTree.ExpandAll()

        Dim wasLoading As Boolean = isLoadingData
        isLoadingData = True
        tvProjectTree.SelectedNode = root
        isLoadingData = wasLoading

        If Not isLoadingData Then
            LoadRollup(currentProject)
        End If
    End Sub

    ''' <summary>
    ''' Updates tabs and loads data based on the selected tree node.
    ''' </summary>
    Private Sub UpdateTabsForSelectedNode()
        Dim selected As Object = tvProjectTree.SelectedNode?.Tag
        If selected Is Nothing Then
            tabControlRight.TabPages.Clear()
            Return
        End If

        Dim previouslySelectedTabName As String = If(tabControlRight.SelectedTab?.Name, String.Empty)
        tabControlRight.TabPages.Clear()

        Select Case True
            Case TypeOf selected Is ProjectModel
                tabControlRight.TabPages.Add(tabProjectInfo)
                tabControlRight.TabPages.Add(tabOverrides)
                tabControlRight.TabPages.Add(tabRollup)

                LoadProjectInfo(CType(selected, ProjectModel))
                LoadOverrides(currentProject.Settings)
                LoadRollup(selected)

                tabControlRight.SelectedTab = If(tabControlRight.TabPages.Contains(tabRollup) AndAlso
                                                  previouslySelectedTabName = tabRollup.Name,
                                                  tabRollup, tabProjectInfo)

            Case TypeOf selected Is BuildingModel
                tabControlRight.TabPages.Add(tabBuildingInfo)
                tabControlRight.TabPages.Add(tabRollup)

                LoadBuildingInfo(CType(selected, BuildingModel))
                LoadRollup(selected)

                If previouslySelectedTabName = tabRollup.Name AndAlso tabControlRight.TabPages.Contains(tabRollup) Then
                    tabControlRight.SelectedTab = tabRollup
                Else
                    tabControlRight.SelectedTab = tabBuildingInfo
                    Me.BeginInvoke(Sub()
                                       txtBuildingName.Focus()
                                       txtBuildingName.SelectAll()
                                   End Sub)
                End If

            Case TypeOf selected Is LevelModel
                tabControlRight.TabPages.Add(tabLevelInfo)
                tabControlRight.TabPages.Add(tabRollup)

                LoadLevelInfo(CType(selected, LevelModel))
                LoadRollup(selected)

                If previouslySelectedTabName = tabRollup.Name AndAlso tabControlRight.TabPages.Contains(tabRollup) Then
                    tabControlRight.SelectedTab = tabRollup
                Else
                    tabControlRight.SelectedTab = tabLevelInfo
                    Me.BeginInvoke(Sub()
                                       txtLevelName.Focus()
                                       txtLevelName.SelectAll()
                                   End Sub)
                End If
        End Select
    End Sub

    ''' <summary>
    ''' Configures visibility of context menu items based on the selected node.
    ''' </summary>
    Private Sub ConfigureTreeMenuVisibility()
        mnuAddBuilding.Visible = TypeOf tvProjectTree.SelectedNode.Tag Is ProjectModel
        mnuAddLevel.Visible = TypeOf tvProjectTree.SelectedNode.Tag Is BuildingModel
        mnuCopyBuilding.Visible = TypeOf tvProjectTree.SelectedNode.Tag Is BuildingModel
        mnuPasteBuilding.Visible = (TypeOf tvProjectTree.SelectedNode.Tag Is ProjectModel) AndAlso (copiedBuilding IsNot Nothing)
        mnuDelete.Visible = True
        EditPSEToolStripMenuItem.Visible = True
        mnuCopyLevels.Visible = False
        mnuPasteLevels.Visible = False

        If TypeOf tvProjectTree.SelectedNode.Tag Is BuildingModel Then
            Dim selectedBldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
            If selectedBldg.Levels.Count > 0 Then
                mnuCopyLevels.Visible = True
            ElseIf copiedLevels IsNot Nothing AndAlso copiedLevels.Count > 0 Then
                mnuPasteLevels.Visible = True
            End If
        End If
    End Sub

#End Region

#Region "UI Management - Customer Dialogs"

    ''' <summary>
    ''' Opens a customer dialog for adding a new customer.
    ''' </summary>
    ''' <param name="defaultTypeID">The customer type (1=Customer, 2=Architect, 3=Engineer).</param>
    ''' <param name="role">Display name for the customer type.</param>
    Private Sub OpenCustomerDialog(defaultTypeID As Integer, role As String)
        Try
            Using frm As New FrmCustomerDialog(defaultTypeID:=defaultTypeID)
                UIHelper.Add($"Opened {role} dialog for new {role}")
                If frm.ShowDialog() = DialogResult.OK Then
                    RefreshCustomerComboboxes()
                    UIHelper.Add($"{role} added")
                End If
            End Using
        Catch ex As Exception
            UIHelper.Add($"Error opening {role} dialog: {ex.Message}")
            MessageBox.Show($"Error opening {role} dialog: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Edits the selected customer in the specified combo box.
    ''' </summary>
    ''' <param name="comboBox">The combo box containing the customer.</param>
    ''' <param name="customerType">The customer type (1=Customer, 2=Architect, 3=Engineer).</param>
    ''' <param name="role">Display name for the customer type.</param>
    Private Sub EditSelectedCustomer(comboBox As ComboBox, customerType As Integer, role As String)
        Try
            If comboBox.SelectedValue IsNot Nothing AndAlso comboBox.SelectedValue IsNot DBNull.Value Then
                Dim selectedCustomerID As Integer = CInt(comboBox.SelectedValue)
                Dim selectedCustomer As CustomerModel = HelperDataAccess.GetCustomers(customerType:=customerType).FirstOrDefault(Function(c) c.CustomerID = selectedCustomerID)

                If selectedCustomer IsNot Nothing Then
                    Using frm As New FrmCustomerDialog(selectedCustomer)
                        UIHelper.Add($"Opened {role} dialog for CustomerID {selectedCustomerID}")
                        If frm.ShowDialog() = DialogResult.OK Then
                            RefreshCustomerComboboxes()
                            UIHelper.Add($"{role} updated")
                        End If
                    End Using
                Else
                    UIHelper.Add($"No {role} found for CustomerID {selectedCustomerID}")
                    MessageBox.Show($"No {role} found for the selected ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                UIHelper.Add($"No {role} selected for editing")
                MessageBox.Show($"Select a {role} to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            UIHelper.Add($"Error editing {role}: {ex.Message}")
            MessageBox.Show($"Error editing {role}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Refreshes all customer-related combo boxes while preserving current selections.
    ''' </summary>
    Private Sub RefreshCustomerComboboxes()
        Dim currentCustomerID As Object = cboCustomer.SelectedValue
        Dim currentArchitectID As Object = cboProjectArchitect.SelectedValue
        Dim currentEngineerID As Object = cboProjectEngineer.SelectedValue

        Dim customers As List(Of CustomerModel) = HelperDataAccess.GetCustomers(customerType:=1)
        Dim architects As List(Of CustomerModel) = HelperDataAccess.GetCustomers(customerType:=2)
        Dim engineers As List(Of CustomerModel) = HelperDataAccess.GetCustomers(customerType:=3)

        cboCustomer.DataSource = customers
        cboProjectArchitect.DataSource = architects
        cboProjectEngineer.DataSource = engineers

        If currentCustomerID IsNot Nothing AndAlso currentCustomerID IsNot DBNull.Value Then
            If customers.Any(Function(c) c.CustomerID = CInt(currentCustomerID)) Then
                cboCustomer.SelectedValue = currentCustomerID
            End If
        End If

        If currentArchitectID IsNot Nothing AndAlso currentArchitectID IsNot DBNull.Value Then
            If architects.Any(Function(c) c.CustomerID = CInt(currentArchitectID)) Then
                cboProjectArchitect.SelectedValue = currentArchitectID
            End If
        End If

        If currentEngineerID IsNot Nothing AndAlso currentEngineerID IsNot DBNull.Value Then
            If engineers.Any(Function(c) c.CustomerID = CInt(currentEngineerID)) Then
                cboProjectEngineer.SelectedValue = currentEngineerID
            End If
        End If

        If cboCustomer.SelectedValue Is Nothing AndAlso customers.Any() Then cboCustomer.SelectedIndex = 0
        If cboProjectArchitect.SelectedValue Is Nothing AndAlso architects.Any() Then cboProjectArchitect.SelectedIndex = 0
        If cboProjectEngineer.SelectedValue Is Nothing AndAlso engineers.Any() Then cboProjectEngineer.SelectedIndex = 0
    End Sub

#End Region

#Region "Building & Level Operations"

    ''' <summary>
    ''' Adds a new building to the project.
    ''' </summary>
    Private Sub AddNewBuilding()
        Dim newBldg As New BuildingModel With {.BuildingName = "New Building", .BldgQty = 1}
        currentProject.Buildings.Add(newBldg)
        ProjectDataAccess.SaveBuilding(newBldg, currentVersionID)
    End Sub

    ''' <summary>
    ''' Adds a new level to the selected building.
    ''' </summary>
    Private Sub AddNewLevel()
        Dim selectedBldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
        Dim newLevel As New LevelModel With {
            .LevelName = "New Level",
            .LevelNumber = selectedBldg.Levels.Count + 1,
            .ProductTypeID = 1,
            .ProductTypeName = "Floor"
        }
        selectedBldg.Levels.Add(newLevel)
        ProjectDataAccess.SaveLevel(newLevel, selectedBldg.BuildingID, currentVersionID)
    End Sub

    ''' <summary>
    ''' Deletes the selected building or level.
    ''' </summary>
    Private Sub DeleteSelectedItem()
        Dim selected As Object = tvProjectTree.SelectedNode.Tag

        If TypeOf selected Is BuildingModel Then
            Dim bldg As BuildingModel = CType(selected, BuildingModel)
            da.DeleteBuilding(bldg.BuildingID)
            currentProject.Buildings.Remove(bldg)
        ElseIf TypeOf selected Is LevelModel Then
            Dim level As LevelModel = CType(selected, LevelModel)
            da.DeleteLevel(level.LevelID)
            Dim bldg As BuildingModel = currentProject.Buildings.First(Function(b) b.Levels.Contains(level))
            bldg.Levels.Remove(level)
        End If
    End Sub

#End Region

#Region "Copy/Paste Operations"

    ''' <summary>
    ''' Copies the selected building and its levels.
    ''' </summary>
    Private Sub CopyBuilding()
        Dim selectedBldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
        copiedBuilding = New BuildingModel With {
            .BuildingName = selectedBldg.BuildingName & " Copy",
            .BuildingType = selectedBldg.BuildingType,
            .BldgQty = selectedBldg.BldgQty,
            .ResUnits = selectedBldg.ResUnits,
            .Levels = New List(Of LevelModel)
        }

        For Each level In selectedBldg.Levels
            Dim clonedLevel As New LevelModel With {
                .LevelName = level.LevelName,
                .LevelNumber = level.LevelNumber,
                .ProductTypeID = level.ProductTypeID,
                .ProductTypeName = level.ProductTypeName
            }
            copiedBuilding.Levels.Add(clonedLevel)
        Next
    End Sub

    ''' <summary>
    ''' Pastes the copied building and its levels.
    ''' </summary>
    Private Sub PasteBuilding()
        If copiedBuilding Is Nothing Then
            MessageBox.Show("No building copied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim newBldg As New BuildingModel With {
            .BuildingName = copiedBuilding.BuildingName,
            .BuildingType = copiedBuilding.BuildingType,
            .BldgQty = copiedBuilding.BldgQty,
            .ResUnits = copiedBuilding.ResUnits
        }
        currentProject.Buildings.Add(newBldg)
        ProjectDataAccess.SaveBuilding(newBldg, currentVersionID)

        newBldg.Levels = New List(Of LevelModel)
        For Each clonedLevel In copiedBuilding.Levels
            Dim newLevel As New LevelModel With {
                .LevelName = clonedLevel.LevelName,
                .LevelNumber = clonedLevel.LevelNumber,
                .ProductTypeID = clonedLevel.ProductTypeID,
                .ProductTypeName = clonedLevel.ProductTypeName
            }
            newBldg.Levels.Add(newLevel)
            ProjectDataAccess.SaveLevel(newLevel, newBldg.BuildingID, currentVersionID)
        Next
    End Sub

    ''' <summary>
    ''' Copies levels from the selected building.
    ''' </summary>
    Private Sub CopyLevels()
        Dim selectedBldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
        copiedLevels = New List(Of LevelModel)

        For Each level In selectedBldg.Levels
            Dim clonedLevel As New LevelModel With {
                .LevelName = level.LevelName,
                .LevelNumber = level.LevelNumber,
                .ProductTypeID = level.ProductTypeID,
                .ProductTypeName = level.ProductTypeName
            }
            copiedLevels.Add(clonedLevel)
        Next
    End Sub

    ''' <summary>
    ''' Pastes copied levels into the selected building.
    ''' </summary>
    Private Sub PasteLevels()
        Dim targetBldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)

        If targetBldg.Levels.Count > 0 Then
            MessageBox.Show("Cannot paste to a building with existing levels.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If copiedLevels Is Nothing OrElse copiedLevels.Count = 0 Then
            MessageBox.Show("No levels copied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        For Each clonedLevel In copiedLevels
            Dim newLevel As New LevelModel With {
                .LevelName = clonedLevel.LevelName,
                .LevelNumber = clonedLevel.LevelNumber,
                .ProductTypeID = clonedLevel.ProductTypeID,
                .ProductTypeName = clonedLevel.ProductTypeName
            }
            targetBldg.Levels.Add(newLevel)
            ProjectDataAccess.SaveLevel(newLevel, targetBldg.BuildingID, currentVersionID)
        Next
    End Sub

#End Region

#Region "Version Management"

    ''' <summary>
    ''' Creates a new project version and refreshes the UI.
    ''' </summary>
    ''' <param name="versionName">The name for the new version.</param>
    ''' <param name="description">Description of the new version.</param>
    Private Sub CreateNewVersion(versionName As String, description As String)
        Dim newVersionID As Integer = ProjVersionDataAccess.CreateProjectVersion(currentProject.ProjectID, versionName, description, Nothing, Nothing)
        currentVersionID = newVersionID

        Dim versions As List(Of ProjectVersionModel) = GetProjectVersions(True)
        cboVersion.DataSource = versions
        cboVersion.DisplayMember = "VersionName"
        cboVersion.ValueMember = "VersionID"
        cboVersion.SelectedValue = currentVersionID

        LoadVersionSpecificData()
    End Sub

    ''' <summary>
    ''' Duplicates the current version and refreshes the UI.
    ''' </summary>
    ''' <param name="versionName">The name for the duplicated version.</param>
    ''' <param name="description">Description of the duplicated version.</param>
    Private Sub DuplicateVersion(versionName As String, description As String)
        ProjVersionDataAccess.DuplicateProjectVersion(currentVersionID, versionName, description, currentProject.ProjectID)

        Dim versions As List(Of ProjectVersionModel) = GetProjectVersions(True)
        currentVersionID = CInt((versions.FirstOrDefault(Function(v) v.VersionName = versionName)?.VersionID))

        cboVersion.DataSource = versions
        cboVersion.DisplayMember = "VersionName"
        cboVersion.ValueMember = "VersionID"
        cboVersion.SelectedValue = currentVersionID

        LoadVersionSpecificData()
    End Sub

    ''' <summary>
    ''' Refreshes rollup data after version changes or recalculations.
    ''' </summary>
    Private Sub RefreshRollupData()
        Try
            UpdateVersionAndSettings()
            LoadVersionSpecificData()
        Catch ex As Exception
            UIHelper.Add($"Error refreshing rollup data: {ex.Message}")
            MessageBox.Show($"Error refreshing rollup data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Updates version and settings data for rollup refresh.
    ''' </summary>
    Private Sub UpdateVersionAndSettings()
        Dim selectedVersion As ProjectVersionModel = GetCurrentVersionModel()
        currentProject.Settings = ProjectDataAccess.GetProjectProductSettings(currentVersionID)
        LoadOverrides(currentProject.Settings)
        currentProject.Buildings = ProjectDataAccess.GetBuildingsByVersionID(currentVersionID)

        If selectedVersion IsNot Nothing Then
            cboCustomer.SelectedValue = If(selectedVersion.CustomerID, 0)
            cboSalesman.SelectedValue = If(selectedVersion.SalesID, 0)
            cboVersion.SelectedValue = currentVersionID
            txtMondayItemId.Text = selectedVersion.MondayID
        End If
    End Sub

    ''' <summary>
    ''' Restores the rollup grid and selected tab/node after recalculation.
    ''' </summary>
    ''' <param name="currentTab">The tab that was selected before recalculation.</param>
    ''' <param name="selectedNode">The tree node that was selected before recalculation.</param>
    Private Sub RestoreRollupGrid(currentTab As TabPage, selectedNode As TreeNode)
        dgvRollup.DataSource = Nothing

        If selectedNode IsNot Nothing Then
            tvProjectTree.SelectedNode = selectedNode
            LoadRollup(selectedNode.Tag)
        Else
            LoadRollup(currentProject)
        End If

        tabControlRight.SelectedTab = currentTab
    End Sub

#End Region

#Region "Lumber Operations"

    ''' <summary>
    ''' Updates lumber prices for the selected cost-effective date.
    ''' </summary>
    Private Sub UpdateLumberPrices()
        If Not EnsureProjectAndVersion("Lumber Update") Then Exit Sub

        If cboCostEffective.SelectedValue Is Nothing OrElse CInt(cboCostEffective.SelectedValue) <= 0 Then
            UIHelper.Add("No cost-effective date selected for lumber update")
            MessageBox.Show("Please select a cost-effective date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim costEffectiveID As Integer = CInt(cboCostEffective.SelectedValue)
        LumberDataAccess.UpdateLumberPrices(currentVersionID, costEffectiveID)
    End Sub

    ''' <summary>
    ''' Sets the selected lumber history as active.
    ''' </summary>
    Private Sub SetActiveLumberHistory()
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If

        If lstLumberHistory.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a cost-effective date to set as active.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim selectedItem As LumberHistoryDate = TryCast(lstLumberHistory.SelectedItem, LumberHistoryDate)
        If selectedItem Is Nothing Then
            MessageBox.Show("Invalid selection in lumber history.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim costEffectiveID As Integer = selectedItem.CostEffectiveID

        Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
            conn.Open()
            Using transaction As SqlTransaction = conn.BeginTransaction()
                Try
                    Dim deactivateParams As New Dictionary(Of String, Object) From {{"@VersionID", currentVersionID}}
                    SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                        "UPDATE RawUnitLumberHistory SET IsActive = 0 WHERE VersionID = @VersionID",
                        HelperDataAccess.BuildParameters(deactivateParams), conn, transaction)

                    Dim activateParams As New Dictionary(Of String, Object) From {
                        {"@CostEffectiveDateID", costEffectiveID},
                        {"@VersionID", currentVersionID}
                    }
                    SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                        "UPDATE RawUnitLumberHistory SET IsActive = 1, UpdateDate = GETDATE() WHERE CostEffectiveDateID = @CostEffectiveDateID AND VersionID = @VersionID",
                        HelperDataAccess.BuildParameters(activateParams), conn, transaction)

                    transaction.Commit()
                    UIHelper.Add($"Set CostEffectiveDateID {costEffectiveID} as active for VersionID {currentVersionID}")

                    RollupCalculationService.RecalculateVersion(currentVersionID)
                    LoadVersionSpecificData()
                Catch ex As Exception
                    transaction.Rollback()
                    UIHelper.Add($"Error setting active lumber history: {ex.Message}")
                    Throw
                End Try
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Deletes the selected lumber history.
    ''' </summary>
    Private Sub DeleteLumberHistory()
        If lstLumberHistory.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a cost-effective date to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim selectedItem As LumberHistoryDate = TryCast(lstLumberHistory.SelectedItem, LumberHistoryDate)
        If selectedItem Is Nothing Then
            MessageBox.Show("Invalid selection in lumber history.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim costEffectiveID As Integer = selectedItem.CostEffectiveID

        If MessageBox.Show($"Are you sure you want to delete all lumber history records for Cost Effective Date ID {costEffectiveID}?",
                          "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Dim params As New Dictionary(Of String, Object) From {
                {"@CostEffectiveDateID", costEffectiveID},
                {"@VersionID", currentVersionID}
            }
            SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteLumberHistory, HelperDataAccess.BuildParameters(params))
            UIHelper.Add($"Deleted lumber history for CostEffectiveDateID {costEffectiveID}")

            RollupCalculationService.RecalculateVersion(currentVersionID)
            LoadVersionSpecificData()
        End If
    End Sub

#End Region

#Region "Futures Operations"

    ''' <summary>
    ''' Prompts user to set the selected contract as active.
    ''' </summary>
    ''' <param name="f">The lumber futures contract.</param>
    ''' <returns>True if the operation was handled, False otherwise.</returns>
    Private Function HandleSetActiveContract(f As LumberFutures) As Boolean
        Dim msg = $"Set {f.ContractMonth} as the active lumber futures contract?" & vbCrLf & vbCrLf &
                  "This will make it the default price source for this version."

        If MessageBox.Show(msg, "Confirm Active Contract", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                LumberDataAccess.SetActiveLumberFuture(currentVersionID, f.LumberFutureID)
                LoadFuturesIntoListBox(currentVersionID)
                MessageBox.Show($"{f.ContractMonth} is now the active contract.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Could not set active contract:" & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Calculates and saves futures adder values based on the selected contract.
    ''' </summary>
    ''' <param name="f">The lumber futures contract.</param>
    Private Sub HandleFuturesAdderCalculation(f As LumberFutures)
        If Not f.PriorSettle.HasValue Then
            MessageBox.Show("Selected futures contract has no settle price.", "Cannot Calculate", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            Dim calculator As New FuturesAdderCalculator(currentVersionID, rollupService)
            Dim currentVersion = GetCurrentVersionModel()
            Dim result = calculator.Calculate(f.PriorSettle.Value, currentProject, currentVersion)

            If Not result.Success Then
                MessageBox.Show(result.ErrorMessage, "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If MessageBox.Show(result.GetConfirmationMessage(f.ContractMonth), "Confirm Futures Adder Calculation",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                SaveFuturesAdderValues(result.FuturesAdderAmt, result.FuturesAdderProjTotal)

                GetProjectVersions(True)
                LoadVersionSpecificData()
                LoadRollupForSelectedNode()

                MessageBox.Show(result.GetSuccessMessage(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("Error calculating futures adder:" & vbCrLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Import Operations"

    ''' <summary>
    ''' Imports MiTek design actuals from a CSV file.
    ''' </summary>
    Private Sub ImportMiTekDesignActuals()
        Using dlg As New OpenFileDialog With {.Filter = "CSV files|*.csv", .Title = "Select MiTek Design Export"}
            If dlg.ShowDialog() = DialogResult.OK Then
                Dim frm As New frmActualsMatcher(currentVersionID, 1, dlg.FileName)
                If frm.ShowDialog() = DialogResult.OK Then
                    RefreshLevelVariance()
                    MessageBox.Show("MiTek actuals imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' Imports BisTrack invoice actuals from a CSV file.
    ''' </summary>
    Private Sub ImportBisTrackInvoiceActuals()
        Using dlg As New OpenFileDialog With {.Filter = "CSV files|*.csv", .Title = "Select BisTrack Invoice Export"}
            If dlg.ShowDialog() = DialogResult.OK Then
                Dim frm As New frmActualsMatcher(currentVersionID, 2, dlg.FileName)
                If frm.ShowDialog() = DialogResult.OK Then
                    RefreshLevelVariance()
                    MessageBox.Show("BisTrack invoice imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' Imports wall costing data from a CSV file.
    ''' </summary>
    Private Sub ImportWallData()
        If Not EnsureProjectAndVersion("Wall Import") Then Return

        Using ofd As New OpenFileDialog() With {
        .Title = "Select Wall Costing CSV Export",
        .Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
        .Multiselect = False
    }
            If ofd.ShowDialog() <> DialogResult.OK Then Return

            Try
                Dim summary As ImportWallSummary = ExternalImportDataAccess.ImportWallsInteractive(ofd.FileName, currentVersionID)

                If summary.WasCancelled Then Return

                If summary.Success Then
                    MessageBox.Show("Walls imported successfully!" & vbCrLf & vbCrLf & summary.GetSummaryText(),
                               "Wall Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Wall import failed. See details below:" & vbCrLf & vbCrLf & summary.GetSummaryText(),
                               "Wall Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                RefreshRollupData()
                SqlConnectionManager.CloseAllDataReaders()
            Catch ex As Exception
                MessageBox.Show("Unexpected error during wall import:" & vbCrLf & ex.Message,
                           "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

#End Region

#Region "Report Generation"

    ''' <summary>
    ''' Generates and displays the project summary report.
    ''' Prompts user to confirm submission and version locking before proceeding.
    ''' </summary>
    Private Sub GenerateProjectReport()
        If Not EnsureProjectAndVersion("Project Summary Preview") Then Exit Sub

        ' Get current version to check status
        Dim currentVersion As ProjectVersionModel = GetCurrentVersionModel()
        If currentVersion Is Nothing Then
            MessageBox.Show("Unable to load current version information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Check if version is already locked
        If ProjVersionDataAccess.IsVersionLocked(currentVersionID) Then
            Dim lockedMsg As String = $"This version is LOCKED and cannot be modified." & vbCrLf & vbCrLf &
                                       $"Locked on: {currentVersion.LockedDate:g}" & vbCrLf &
                                       $"Locked by: {currentVersion.LockedBy}" & vbCrLf & vbCrLf &
                                       "A report has already been generated for this version." & vbCrLf &
                                       "Create a new version to make changes."

            MessageBox.Show(lockedMsg, "Version Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Show confirmation dialog with all the consequences
        Dim confirmationMessage As String = BuildReportConfirmationMessage(currentVersion)

        Dim result = MessageBox.Show(
            confirmationMessage,
            "Confirm Project Report Generation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2)

        If result <> DialogResult.Yes Then
            UIHelper.Add("Project report generation cancelled by user")
            Exit Sub
        End If

        Try
            ' 1. Change status to "Submitted" (ProjVersionStatusID = 2) and lock the version
            Const SUBMITTED_STATUS_ID As Integer = 2
            ProjVersionDataAccess.LockVersion(currentVersionID, SUBMITTED_STATUS_ID, CurrentUser.DisplayName)

            ' 2. Capture price history snapshot
            CapturePriceHistorySnapshot("ProjectSummary")

            ' 3. Refresh the UI to reflect locked state
            GetProjectVersions(True)
            LoadVersionSpecificData()

            ' APPLY LOCK STATE IMMEDIATELY AFTER LOCKING
            ApplyVersionLockState()

            UIHelper.Add($"Version {currentVersion.VersionName} locked and status changed to 'Submitted' (StatusID: {SUBMITTED_STATUS_ID})")
        Catch ex As Exception
            UIHelper.Add($"Error locking version: {ex.Message}")
            MessageBox.Show($"Error locking version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try

        ' 4. Generate the report
        UIHelper.Add("Opening Project Summary Preview...")
        Dim dataSources As New List(Of ReportDataSource) From {
            New ReportDataSource("ProjectSummaryDataSet", ReportsDataAccess.GetProjectSummaryData(currentProject.ProjectID, currentVersionID))
        }

        Dim previewForm As New frmReportPreview("BuildersPSE2.ProjectSummary.rdlc", dataSources)
        previewForm.ShowDialog()
        UIHelper.Add("Project Summary Preview opened")
    End Sub

    ''' <summary>
    ''' Builds the confirmation message showing all consequences of generating the report.
    ''' </summary>
    ''' <param name="currentVersion">The current project version.</param>
    ''' <returns>Formatted confirmation message.</returns>
    Private Function BuildReportConfirmationMessage(currentVersion As ProjectVersionModel) As String
        Dim statusName As String = cboProjectStatus.Text

        Dim message As New System.Text.StringBuilder()
        message.AppendLine("⚠️ Are you sure you want to generate the Project Report?")
        message.AppendLine()
        message.AppendLine("The following actions will occur:")
        message.AppendLine()
        message.AppendLine("1. ✓ Status will change to 'Submitted'")
        message.AppendLine($"   Current Status: {statusName}")
        message.AppendLine()
        message.AppendLine("2. 🔒 This version will be LOCKED")
        message.AppendLine("   • All fields will become read-only")
        message.AppendLine("   • No data changes will be allowed")
        message.AppendLine("   • Buildings and levels cannot be modified")
        message.AppendLine()
        message.AppendLine("3. 📋 A new version must be created for any future changes")
        message.AppendLine()
        message.AppendLine("4. 📸 A price history snapshot will be captured")
        message.AppendLine()
        message.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        message.AppendLine($"Project: {currentProject.JBID} - {currentProject.ProjectName}")
        message.AppendLine($"Version: {currentVersion.VersionName}")
        message.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
        message.AppendLine()
        message.AppendLine("This action cannot be undone (except by admin override).")
        message.AppendLine()
        message.AppendLine("Continue with report generation?")

        Return message.ToString()
    End Function

    ''' <summary>
    ''' Generates and displays the Inclusions/Exclusions report.
    ''' </summary>
    Private Sub GenerateInclusionsExclusionsReport()
        If Not EnsureProjectAndVersion("Inclusions/Exclusions Preview") Then Exit Sub

        UIHelper.Add("Opening Inclusions/Exclusions Preview...")
        Dim dataSources As New List(Of ReportDataSource) From {
            New ReportDataSource("ProjectHeaderDataSet", ReportsDataAccess.GetProjectHeaderData(currentProject.ProjectID, currentVersionID)),
            New ReportDataSource("IncExcItemsDataSet", IEDataAccess.GetProjectItems(currentProject.ProjectID)),
            New ReportDataSource("ProjectLoadsDataSet", IEDataAccess.GetProjectLoadsData(currentProject.ProjectID)),
            New ReportDataSource("ProjectBearingStylesDataSet", IEDataAccess.GetProjectBearingStylesData(currentProject.ProjectID))
        }

        Dim previewForm As New frmReportPreview("BuildersPSE2.InclusionsExclusions.rdlc", dataSources)
        previewForm.ShowDialog()
        UIHelper.Add("Inclusions/Exclusions Preview opened")
    End Sub

    ''' <summary>
    ''' Captures a price history snapshot with duplicate checking.
    ''' </summary>
    ''' <param name="reportType">The type of report being generated.</param>
    Private Sub CapturePriceHistorySnapshot(reportType As String)
        Const DUPLICATE_CHECK_MINUTES As Integer = 5

        Dim recentHistory = PriceHistoryDataAccess.GetRecentPriceHistory(currentProject.ProjectID, currentVersionID, DUPLICATE_CHECK_MINUTES)

        If recentHistory IsNot Nothing Then
            Dim msg = $"A price history snapshot was already recorded {FormatTimeAgo(recentHistory.CreatedDate)}." & vbCrLf & vbCrLf &
                      $"Recorded by: {recentHistory.CreatedBy}" & vbCrLf &
                      $"Sell: {recentHistory.ExtendedSell:C2}  |  Cost: {recentHistory.ExtendedCost:C2}  |  Margin: {recentHistory.Margin:P1}" & vbCrLf & vbCrLf &
                      "Would you like to:" & vbCrLf &
                      "• Yes - Record a NEW snapshot" & vbCrLf &
                      "• No - OVERWRITE the existing snapshot" & vbCrLf &
                      "• Cancel - Skip recording"

            Dim result = MessageBox.Show(msg, "Recent Price History Found", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            Select Case result
                Case DialogResult.Cancel
                    UIHelper.Add("Price history capture skipped by user")
                    Return
                Case DialogResult.No
                    Dim service As New PriceHistoryService()
                    Dim currentVersion = GetCurrentVersionModel()
                    Dim model = service.CapturePriceHistory(currentProject, currentVersion, reportType)
                    PriceHistoryDataAccess.OverwritePriceHistory(recentHistory.PriceHistoryID, model)
                    UIHelper.Add($"Price history overwritten (ID: {recentHistory.PriceHistoryID})")
                    Return
            End Select
        End If

        Dim priceService As New PriceHistoryService()
        Dim version = GetCurrentVersionModel()
        Dim historyModel = priceService.CapturePriceHistory(currentProject, version, reportType)
        Dim newID = PriceHistoryDataAccess.SavePriceHistory(historyModel)
        UIHelper.Add($"Price history captured (ID: {newID})")
    End Sub

#End Region

#Region "Form Navigation"

    ''' <summary>
    ''' Opens the PSE form for the current project and version.
    ''' </summary>
    Private Sub OpenPSEForm()
        If Not EnsureProjectAndVersion("PSE") Then Return

        Try
            ClearRollupDisplay()

            _mainForm.AddFormToTabControl(GetType(FrmPSE), $"PSE_{currentProject.ProjectID}_{currentVersionID}",
                                      New Object() {currentProject.ProjectID, currentVersionID})
            UIHelper.Add($"Opened PSE form for ProjectID {currentProject.ProjectID}, VersionID {currentVersionID}")
        Catch ex As Exception
            UIHelper.Add($"Error opening PSE form: {ex.Message}")
            MessageBox.Show("Error opening PSE form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Opens the Inclusions/Exclusions form.
    ''' </summary>
    Private Sub OpenInclusionsExclusionsForm()
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            _mainForm.AddFormToTabControl(GetType(frmInclusionsExclusions), $"IE_{currentProject.ProjectID}",
                                          New Object() {currentProject.ProjectID})
            UIHelper.Add($"Opened Inclusions/Exclusions form for ProjectID {currentProject.ProjectID}")
        Else
            UIHelper.Add("No valid project selected for Inclusions/Exclusions")
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.",
                           "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ''' <summary>
    ''' Opens the Project Builder form.
    ''' </summary>
    Private Sub OpenProjectBuilderForm()
        If Not EnsureProjectAndVersion("Project Builder") Then Return

        Try
            _mainForm.AddFormToTabControl(GetType(ProjectBuilderForm), $"ProjectBuilder_{currentVersionID}",
                                       New Object() {currentVersionID})
            UIHelper.Add($"Opened Project Builder form for ProjectID {currentProject.ProjectID}, VersionID {currentVersionID}")
        Catch ex As Exception
            UIHelper.Add($"Error opening Project Builder form: {ex.Message}")
            MessageBox.Show("Error opening Project Builder form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Closes the form and removes it from the tab control.
    ''' </summary>
    Private Sub CloseForm()
        Dim tagValue As String = Me.Tag?.ToString()
        If String.IsNullOrEmpty(tagValue) Then
            Throw New Exception("Tab tag not found.")
        End If

        _mainForm.RemoveTabFromTabControl(tagValue)
        UIHelper.Add($"Closed tab {tagValue} at {DateTime.Now:HH:mm:ss}")
    End Sub

#End Region

#Region "Project Operations"

    ''' <summary>
    ''' Deletes the current project and closes the form.
    ''' </summary>
    Private Sub DeleteProject()
        Dim notification As String = String.Empty
        da.DeleteProject(currentProject.ProjectID, notification)

        If Not String.IsNullOrEmpty(notification) Then
            MessageBox.Show(notification, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        btnClose.PerformClick()
    End Sub

#End Region

#Region "Helper Methods"

    ''' <summary>
    ''' Gets project versions with optional caching.
    ''' </summary>
    ''' <param name="forceRefresh">Whether to force a refresh from the database.</param>
    ''' <returns>List of project versions.</returns>
    Private Function GetProjectVersions(Optional forceRefresh As Boolean = False) As List(Of ProjectVersionModel)
        Dim projectId As Integer = If(currentProject IsNot Nothing, currentProject.ProjectID, 0)
        Dim requiresReload As Boolean = forceRefresh OrElse projectVersions Is Nothing OrElse projectVersionsProjectId <> projectId

        If requiresReload Then
            If projectId <= 0 Then
                projectVersions = New List(Of ProjectVersionModel)()
            Else
                projectVersions = ProjVersionDataAccess.GetProjectVersions(projectId)
            End If
            projectVersionsProjectId = projectId
        End If

        Return projectVersions
    End Function

    ''' <summary>
    ''' Gets the current version model for the selected version ID.
    ''' </summary>
    ''' <returns>The current version model, or Nothing if not found.</returns>
    Private Function GetCurrentVersionModel() As ProjectVersionModel
        If currentVersionID <= 0 Then Return Nothing

        Dim versions As List(Of ProjectVersionModel) = GetProjectVersions()
        Return versions.FirstOrDefault(Function(v) v.VersionID = currentVersionID)
    End Function

    ''' <summary>
    ''' Ensures a valid project and version are selected before proceeding.
    ''' </summary>
    ''' <param name="operationName">The name of the operation requiring validation.</param>
    ''' <returns>True if valid, False otherwise.</returns>
    Private Function EnsureProjectAndVersion(operationName As String) As Boolean
        If currentProject Is Nothing OrElse currentProject.ProjectID <= 0 OrElse currentVersionID <= 0 Then
            UIHelper.Add($"No valid project or version selected for {operationName}.")
            MessageBox.Show("No valid project or version selected. Save the project and select a version first.",
                           operationName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' Gets the current level ID from the selected tree node.
    ''' </summary>
    ''' <returns>The level ID, or 0 if no level is selected.</returns>
    Private Function GetCurrentLevelID() As Integer
        If tvProjectTree.SelectedNode Is Nothing Then Return 0
        If TypeOf tvProjectTree.SelectedNode.Tag Is LevelModel Then
            Return CType(tvProjectTree.SelectedNode.Tag, LevelModel).LevelID
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Gets the current building ID from the selected tree node.
    ''' </summary>
    ''' <returns>The building ID, or 0 if no building is selected.</returns>
    Private Function GetCurrentBuildingID() As Integer
        If tvProjectTree.SelectedNode Is Nothing Then Return 0
        If TypeOf tvProjectTree.SelectedNode.Tag Is BuildingModel Then
            Return CType(tvProjectTree.SelectedNode.Tag, BuildingModel).BuildingID
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Converts a masked textbox value to a nullable date.
    ''' </summary>
    ''' <param name="mtb">The masked textbox to convert.</param>
    ''' <returns>A nullable date value.</returns>
    Private Function TextToNullableDate(mtb As MaskedTextBox) As Date?
        If String.IsNullOrEmpty(mtb.Text) OrElse mtb.Text.Length <> 8 Then
            Return Nothing
        End If

        If DateTime.TryParseExact(mtb.Text, "MMddyyyy", Nothing, DateTimeStyles.None, Nothing) Then
            Return DateTime.ParseExact(mtb.Text, "MMddyyyy", Nothing).Date
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Formats a DateTime as a human-readable "time ago" string.
    ''' </summary>
    ''' <param name="dt">The DateTime to format.</param>
    ''' <returns>A formatted string like "5 minutes ago".</returns>
    Private Function FormatTimeAgo(dt As DateTime) As String
        Dim diff = DateTime.Now - dt
        If diff.TotalMinutes < 1 Then Return "just now"
        If diff.TotalMinutes < 60 Then Return $"{CInt(diff.TotalMinutes)} minutes ago"
        If diff.TotalHours < 24 Then Return $"{CInt(diff.TotalHours)} hours ago"
        Return dt.ToString("g")
    End Function

    ''' <summary>
    ''' Checks if the current version is locked before allowing destructive operations.
    ''' Shows warning message if locked.
    ''' </summary>
    ''' <param name="operationName">Name of the operation being attempted.</param>
    ''' <returns>True if operation should proceed, False if locked.</returns>
    Private Function CheckVersionNotLocked(operationName As String) As Boolean
        If isVersionLocked Then
            MessageBox.Show($"Cannot perform '{operationName}' - this version is locked." & vbCrLf & vbCrLf &
                       "Create a new version to make changes.",
                       "Version Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function



#End Region

#Region "Version Lock Management"

    ''' <summary>
    ''' Checks if the current version is locked and applies UI restrictions accordingly.
    ''' Should be called whenever version changes or after locking/unlocking operations.
    ''' </summary>
    Private Sub ApplyVersionLockState()
        Try
            ' Check if version is locked
            Dim currentVersion = GetCurrentVersionModel()
            If currentVersion IsNot Nothing Then
                isVersionLocked = currentVersion.IsLocked
            Else
                isVersionLocked = False
            End If

            ' Apply UI lockdown
            If isVersionLocked Then
                LockFormControls()
                ShowLockedIndicators(currentVersion)
                UIHelper.Add($"Version {currentVersion?.VersionName} is LOCKED - UI controls disabled")
            Else
                UnlockFormControls()
                HideLockedIndicators()
            End If

            ' Show/hide admin unlock button
            ConfigureAdminUnlockButton()

        Catch ex As Exception
            UIHelper.Add($"Error applying version lock state: {ex.Message}")
        End Try
    End Sub

    ''' <summary>
    ''' Disables all editing controls when version is locked.
    ''' </summary>
    Private Sub LockFormControls()
        ' === SAVE BUTTONS ===
        btnSaveProjectInfo.Enabled = False
        btnSaveOverrides.Enabled = False
        btnSaveBuildingInfo.Enabled = False
        btnSaveLevelInfo.Enabled = False

        ' === TREE CONTEXT MENU ===
        mnuAddBuilding.Enabled = False
        mnuAddLevel.Enabled = False
        mnuDelete.Enabled = False
        mnuCopyBuilding.Enabled = False
        mnuCopyLevels.Enabled = False
        mnuPasteBuilding.Enabled = False
        mnuPasteLevels.Enabled = False
        EditPSEToolStripMenuItem.Enabled = False

        ' === LUMBER OPERATIONS ===
        btnUpdateLumber.Enabled = False
        btnSetActive.Enabled = False
        btnDeleteLumberHistory.Enabled = False
        btnPullFutures.Enabled = False
        cboCostEffective.Enabled = False
        lstLumberHistory.Enabled = False

        ' === PSE FORM OPENING ===
        btnOpenPSE.Enabled = False

        ' === IMPORT OPERATIONS ===
        btnImportMiTek.Enabled = False
        btnImportBisTrack.Enabled = False
        btnImportWalls.Enabled = False

        ' === PROJECT INFO TAB ===
        txtJBID.ReadOnly = True
        txtProjectName.ReadOnly = True
        txtAddress.ReadOnly = True
        txtCity.ReadOnly = True
        txtZip.ReadOnly = True
        txtProjectNotes.ReadOnly = True
        txtEngPlanDate.ReadOnly = True
        txtArchPlanDate.ReadOnly = True
        cboProjectType.Enabled = False
        cboEstimator.Enabled = False
        cboState.Enabled = False
        cboProjectArchitect.Enabled = False
        cboProjectEngineer.Enabled = False
        cboCustomer.Enabled = False
        cboSalesman.Enabled = False
        cboProjectStatus.Enabled = False
        dtpBidDate.Enabled = False
        nudMilesToJobSite.Enabled = False
        nudTotalNetSqft.Enabled = False
        nudTotalGrossSqft.Enabled = False

        ' === BUILDING INFO TAB ===
        txtBuildingName.ReadOnly = True
        txtBuildingType.ReadOnly = True
        txtResUnits.ReadOnly = True
        nudBldgQty.Enabled = False
        nudNbrUnits.Enabled = False

        ' === LEVEL INFO TAB ===
        txtLevelName.ReadOnly = True
        cmbLevelType.Enabled = False
        nudLevelNumber.Enabled = False

        ' === OVERRIDES GRID ===
        If dgvOverrides IsNot Nothing Then
            dgvOverrides.ReadOnly = True
            dgvOverrides.AllowUserToAddRows = False
            dgvOverrides.AllowUserToDeleteRows = False
        End If

        ' === CUSTOMER BUTTONS ===
        btnAddCustomer.Enabled = False
        btnEditCustomer.Enabled = False
        btnAddArchitect.Enabled = False
        btnEditArchitect.Enabled = False
        btnAddEngineer.Enabled = False
        btnEditEngineer.Enabled = False

        ' === MONDAY.COM ===
        btnLinkMonday.Enabled = False
        txtMondayItemId.ReadOnly = True
    End Sub

    ''' <summary>
    ''' Re-enables all editing controls when version is unlocked.
    ''' </summary>
    Private Sub UnlockFormControls()
        ' === SAVE BUTTONS ===
        btnSaveProjectInfo.Enabled = True
        btnSaveOverrides.Enabled = CurrentUser.IsAdmin
        btnSaveBuildingInfo.Enabled = True
        btnSaveLevelInfo.Enabled = True

        ' === TREE CONTEXT MENU ===
        mnuAddBuilding.Enabled = True
        mnuAddLevel.Enabled = True
        mnuDelete.Enabled = True
        mnuCopyBuilding.Enabled = True
        mnuCopyLevels.Enabled = True
        mnuPasteBuilding.Enabled = True
        mnuPasteLevels.Enabled = True
        EditPSEToolStripMenuItem.Enabled = True

        ' === LUMBER OPERATIONS ===
        btnUpdateLumber.Enabled = CurrentUser.IsAdmin
        btnSetActive.Enabled = CurrentUser.IsAdmin
        btnDeleteLumberHistory.Enabled = True
        btnPullFutures.Enabled = CurrentUser.IsAdmin
        cboCostEffective.Enabled = True
        lstLumberHistory.Enabled = True

        ' === PSE FORM OPENING ===
        btnOpenPSE.Enabled = True

        ' === IMPORT OPERATIONS ===
        btnImportMiTek.Enabled = CurrentUser.IsAdmin
        btnImportBisTrack.Enabled = True
        btnImportWalls.Enabled = True

        ' === PROJECT INFO TAB ===
        txtJBID.ReadOnly = False
        txtProjectName.ReadOnly = False
        txtAddress.ReadOnly = False
        txtCity.ReadOnly = False
        txtZip.ReadOnly = False
        txtProjectNotes.ReadOnly = False
        txtEngPlanDate.ReadOnly = False
        txtArchPlanDate.ReadOnly = False
        cboProjectType.Enabled = True
        cboEstimator.Enabled = True
        cboState.Enabled = True
        cboProjectArchitect.Enabled = True
        cboProjectEngineer.Enabled = True
        cboCustomer.Enabled = True
        cboSalesman.Enabled = True
        cboProjectStatus.Enabled = True
        dtpBidDate.Enabled = True
        nudMilesToJobSite.Enabled = True
        nudTotalNetSqft.Enabled = True
        nudTotalGrossSqft.Enabled = True

        ' === BUILDING INFO TAB ===
        txtBuildingName.ReadOnly = False
        txtBuildingType.ReadOnly = False
        txtResUnits.ReadOnly = False
        nudBldgQty.Enabled = True
        nudNbrUnits.Enabled = True

        ' === LEVEL INFO TAB ===
        txtLevelName.ReadOnly = False
        cmbLevelType.Enabled = True
        nudLevelNumber.Enabled = True

        ' === OVERRIDES GRID ===
        If dgvOverrides IsNot Nothing Then
            dgvOverrides.ReadOnly = False
            dgvOverrides.AllowUserToAddRows = True
            dgvOverrides.AllowUserToDeleteRows = True
        End If

        ' === CUSTOMER BUTTONS ===
        btnAddCustomer.Enabled = True
        btnEditCustomer.Enabled = True
        btnAddArchitect.Enabled = True
        btnEditArchitect.Enabled = True
        btnAddEngineer.Enabled = True
        btnEditEngineer.Enabled = True

        ' === MONDAY.COM ===
        btnLinkMonday.Enabled = CurrentUser.IsAdmin
        txtMondayItemId.ReadOnly = False
    End Sub

    ''' <summary>
    ''' Shows visual indicators that the version is locked.
    ''' </summary>
    Private Sub ShowLockedIndicators(currentVersion As ProjectVersionModel)
        ' === OPTION C: Change version combo background color ===
        cboVersion.BackColor = Color.LightCoral
        cboVersion.ForeColor = Color.White

        ' === OPTION D: Add locked icon/label in status area (form title) ===
        Me.Text = $"🔒 LOCKED - Edit Project - {currentProject.JBID} - {currentVersion?.VersionName}"

        ' Optional: Show lock details in tooltip
        If currentVersion IsNot Nothing AndAlso currentVersion.LockedDate.HasValue Then
            Dim tooltip = $"🔒 VERSION LOCKED" & vbCrLf &
                         $"Locked by: {currentVersion.LockedBy}" & vbCrLf &
                         $"Locked on: {currentVersion.LockedDate:g}" & vbCrLf &
                         "Create a new version to make changes."

            Dim tt As New ToolTip()
            tt.SetToolTip(cboVersion, tooltip)
        End If
    End Sub

    ''' <summary>
    ''' Hides locked indicators when version is unlocked.
    ''' </summary>
    Private Sub HideLockedIndicators()
        ' Restore normal colors
        cboVersion.BackColor = SystemColors.Window
        cboVersion.ForeColor = SystemColors.WindowText

        ' Restore original title
        SetFormTitle()
    End Sub

    ''' <summary>
    ''' Shows/hides the admin unlock button based on locked state and user permissions.
    ''' </summary>
    Private Sub ConfigureAdminUnlockButton()
        If Not CurrentUser.IsAdmin Then
            ' Non-admins never see the unlock button
            If btnAdminUnlock IsNot Nothing Then
                Me.Controls.Remove(btnAdminUnlock)
                btnAdminUnlock.Dispose()
                btnAdminUnlock = Nothing
            End If
            Return
        End If

        If isVersionLocked Then
            ' Create and show admin unlock button
            If btnAdminUnlock Is Nothing Then
                btnAdminUnlock = New Button With {
                    .Text = "🔓 Admin Unlock",
                    .BackColor = Color.OrangeRed,
                    .ForeColor = Color.White,
                    .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                    .Size = New Size(120, 28),
                    .Location = New Point(580, 14),
                    .Cursor = Cursors.Hand
                }
                Me.pnlProjectInfo.Controls.Add(Me.btnAdminUnlock)

                AddHandler btnAdminUnlock.Click, AddressOf btnAdminUnlock_Click
                Me.pnlProjectInfo.Controls.Add(Me.btnAdminUnlock)
                btnAdminUnlock.BringToFront()
            End If
            btnAdminUnlock.Visible = True
        Else
            ' Hide unlock button when not locked
            If btnAdminUnlock IsNot Nothing Then
                btnAdminUnlock.Visible = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handles admin unlock button click - shows audit dialog and unlocks version.
    ''' </summary>
    Private Sub btnAdminUnlock_Click(sender As Object, e As EventArgs)
        Try
            Dim currentVersion = GetCurrentVersionModel()
            If currentVersion Is Nothing Then Return

            ' Show admin unlock dialog with audit requirement
            Using dlg As New frmAdminUnlockDialog(
                currentVersion.VersionName,
                currentVersion.LockedBy,
                currentVersion.LockedDate)

                If dlg.ShowDialog() = DialogResult.OK Then
                    ' Perform unlock with audit logging
                    ProjVersionDataAccess.UnlockVersionWithAudit(
                        currentVersionID,
                        CurrentUser.DisplayName,
                        dlg.UnlockReason,
                        currentVersion.LockedDate,
                        currentVersion.LockedBy)

                    ' Refresh version data and UI
                    GetProjectVersions(True)
                    LoadVersionSpecificData()
                    ApplyVersionLockState()

                    MessageBox.Show($"Version '{currentVersion.VersionName}' has been unlocked." & vbCrLf & vbCrLf &
                                   "This action has been logged for audit purposes.",
                                   "Version Unlocked", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    UIHelper.Add($"ADMIN OVERRIDE: Version {currentVersionID} unlocked by {CurrentUser.DisplayName}")
                End If
            End Using

        Catch ex As Exception
            UIHelper.Add($"Error unlocking version: {ex.Message}")
            MessageBox.Show($"Error unlocking version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Event Handlers - Save Buttons"

    Private Sub btnSaveProjectInfo_Click(sender As Object, e As EventArgs) Handles btnSaveProjectInfo.Click
        Try
            UpdateProjectFromControls()
            SaveProjectAndVersion()
            MessageBox.Show("Project and version info saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadVersions()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error saving project info: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveOverrides_Click(sender As Object, e As EventArgs) Handles btnSaveOverrides.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If

        Try
            SaveOverrides()
            MessageBox.Show("Overrides saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadOverrides(currentProject.Settings)
            btnRecalcRollup.PerformClick()
        Catch ex As Exception
            UIHelper.Add($"Error saving overrides: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveBuildingInfo_Click(sender As Object, e As EventArgs) Handles btnSaveBuildingInfo.Click
        Try
            Dim bldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
            UpdateBuildingFromControls(bldg)
            ProjectDataAccess.SaveBuilding(bldg, currentVersionID)
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error saving building info: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveLevelInfo_Click(sender As Object, e As EventArgs) Handles btnSaveLevelInfo.Click
        Try
            Dim level As LevelModel = CType(tvProjectTree.SelectedNode.Tag, LevelModel)
            UpdateLevelFromControls(level)
            ProjectDataAccess.SaveLevel(level, level.BuildingID, currentVersionID)
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error saving level info: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Event Handlers - Customer Buttons"

    Private Sub btnAddCustomer_Click(sender As Object, e As EventArgs) Handles btnAddCustomer.Click
        OpenCustomerDialog(defaultTypeID:=1, "customer")
    End Sub

    Private Sub btnEditCustomer_Click(sender As Object, e As EventArgs) Handles btnEditCustomer.Click
        EditSelectedCustomer(cboCustomer, customerType:=1, "customer")
    End Sub

    Private Sub btnAddArchitect_Click(sender As Object, e As EventArgs) Handles btnAddArchitect.Click
        OpenCustomerDialog(defaultTypeID:=2, "architect")
    End Sub

    Private Sub btnEditArchitect_Click(sender As Object, e As EventArgs) Handles btnEditArchitect.Click
        EditSelectedCustomer(cboProjectArchitect, customerType:=2, "architect")
    End Sub

    Private Sub btnAddEngineer_Click(sender As Object, e As EventArgs) Handles btnAddEngineer.Click
        OpenCustomerDialog(defaultTypeID:=3, "engineer")
    End Sub

    Private Sub btnEditEngineer_Click(sender As Object, e As EventArgs) Handles btnEditEngineer.Click
        EditSelectedCustomer(cboProjectEngineer, customerType:=3, "engineer")
    End Sub

#End Region

#Region "Event Handlers - Version Operations"

    Private Sub cboVersion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboVersion.SelectedIndexChanged
        If cboVersion.SelectedValue IsNot Nothing AndAlso Not isChangingVersion Then
            isChangingVersion = True
            Dim previousVersionID As Integer = currentVersionID
            currentVersionID = CInt(cboVersion.SelectedValue)

            If previousVersionID > 0 AndAlso previousVersionID <> currentVersionID Then
                GetProjectVersions(True)

                Dim result = MessageBox.Show(
                    "Version changed. Do you want to recalculate rollup data for the new version?" & vbCrLf & vbCrLf &
                    "This will ensure all pricing and totals are up-to-date.",
                    "Recalculate Rollup?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1)

                If result = DialogResult.Yes Then
                    Try
                        UIHelper.ShowBusy(_mainForm, "Recalculating version rollups...")
                        RollupCalculationService.RecalculateVersion(currentVersionID)
                        GetProjectVersions(True)
                        UIHelper.Add($"Rollup recalculated for version {currentVersionID}")
                    Catch ex As Exception
                        MessageBox.Show($"Error recalculating rollup: {ex.Message}", "Recalculation Error",
                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Finally
                        UIHelper.HideBusy(_mainForm)
                    End Try
                End If
            Else
                GetProjectVersions(True)
            End If

            LoadVersionSpecificData()

            ' APPLY LOCK STATE AFTER VERSION CHANGE
            ApplyVersionLockState()

            isChangingVersion = False
        End If
    End Sub

    Private Sub btnCreateVersion_Click(sender As Object, e As EventArgs) Handles btnCreateVersion.Click
        Try
            Using frm As New frmVersionDialog()
                If frm.ShowDialog() = DialogResult.OK Then
                    CreateNewVersion(frm.VersionName, frm.Description)
                    MessageBox.Show("Version created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            UIHelper.Add($"Error creating version: {ex.Message}")
            MessageBox.Show("Error creating version: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnDuplicateVersion_Click(sender As Object, e As EventArgs) Handles btnDuplicateVersion.Click
        Try
            Using frm As New frmVersionDialog()
                If frm.ShowDialog() = DialogResult.OK Then
                    DuplicateVersion(frm.VersionName, frm.Description)
                    MessageBox.Show("Version duplicated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As ArgumentException
            UIHelper.Add($"Cannot duplicate version: {ex.Message}")
            MessageBox.Show($"Cannot duplicate version: {ex.Message}", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            UIHelper.Add($"Error duplicating version: {ex.Message}")
            MessageBox.Show($"Error duplicating version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnRecalcRollup_Click(sender As Object, e As EventArgs) Handles btnRecalcRollup.Click
        Try
            UIHelper.ShowBusy(frmMain)
            Dim currentTab As TabPage = tabControlRight.SelectedTab
            Dim selectedNode As TreeNode = tvProjectTree.SelectedNode
            UIHelper.Add("Recalculating rollup...")
            Me.Cursor = Cursors.WaitCursor

            SaveOverrides()
            RollupCalculationService.RecalculateVersion(currentVersionID)
            RefreshRollupData()
            RestoreRollupGrid(currentTab, selectedNode)

            UIHelper.Add("Rollup recalculated successfully")
        Catch ex As Exception
            UIHelper.Add($"Error recalculating rollup: {ex.Message}")
            MessageBox.Show($"Error recalculating rollup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Cursor = Cursors.Default
            UIHelper.HideBusy(frmMain)
        End Try
    End Sub

#End Region

#Region "Event Handlers - Tree Operations"

    Private Sub TvProjectTree_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvProjectTree.AfterSelect
        Try
            If isLoadingData Then Exit Sub

            UpdateTabsForSelectedNode()
            RefreshLevelVariance()
            RefreshBuildingVariance()
        Catch ex As Exception
            UIHelper.Add($"Error selecting tree node: {ex.Message}")
            MessageBox.Show("Error selecting tree node: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub tvProjectTree_MouseDown(sender As Object, e As MouseEventArgs) Handles tvProjectTree.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim node As TreeNode = tvProjectTree.GetNodeAt(e.X, e.Y)
            If node IsNot Nothing Then
                tvProjectTree.SelectedNode = node
            End If
        End If
    End Sub

    Private Sub cmsTreeMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsTreeMenu.Opening
        ConfigureTreeMenuVisibility()
    End Sub

#End Region

#Region "Event Handlers - Context Menu"

    Private Sub mnuAddBuilding_Click(sender As Object, e As EventArgs) Handles mnuAddBuilding.Click
        Try
            AddNewBuilding()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error adding building: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuAddLevel_Click(sender As Object, e As EventArgs) Handles mnuAddLevel.Click
        Try
            AddNewLevel()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error adding level: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuDelete_Click(sender As Object, e As EventArgs) Handles mnuDelete.Click
        Try
            If MessageBox.Show("Delete selected item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                DeleteSelectedItem()
                LoadProjectData()
            End If
        Catch ex As Exception
            UIHelper.Add($"Error deleting item: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuCopyLevels_Click(sender As Object, e As EventArgs) Handles mnuCopyLevels.Click
        Try
            CopyLevels()
        Catch ex As Exception
            UIHelper.Add($"Error copying levels: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuPasteLevels_Click(sender As Object, e As EventArgs) Handles mnuPasteLevels.Click
        Try
            PasteLevels()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error pasting levels: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuCopyBuilding_Click(sender As Object, e As EventArgs) Handles mnuCopyBuilding.Click
        Try
            CopyBuilding()
        Catch ex As Exception
            UIHelper.Add($"Error copying building: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuPasteBuilding_Click(sender As Object, e As EventArgs) Handles mnuPasteBuilding.Click
        Try
            PasteBuilding()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error pasting building: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub EditPSEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditPSEToolStripMenuItem.Click
        OpenPSEForm()
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        LoadProjectBuildings()
        RefreshProjectTree()
    End Sub

#End Region

#Region "Event Handlers - Navigation Buttons"

    Private Sub btnOpenPSE_Click(sender As Object, e As EventArgs) Handles btnOpenPSE.Click
        OpenPSEForm()
    End Sub

    Private Sub btnIEOpen_Click(sender As Object, e As EventArgs) Handles btnIEOpen.Click
        Try
            OpenInclusionsExclusionsForm()
        Catch ex As Exception
            UIHelper.Add($"Error opening Inclusions/Exclusions form: {ex.Message}")
            MessageBox.Show("Error opening Inclusions/Exclusions form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnOpenProjectBuilder_Click(sender As Object, e As EventArgs) Handles btnOpenProjectBuilder.Click
        Try
            OpenProjectBuilderForm()
        Catch ex As Exception
            UIHelper.Add($"Error opening Project Builder form: {ex.Message}")
            MessageBox.Show("Error opening Project Builder form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Try
            CloseForm()
        Catch ex As Exception
            UIHelper.Add($"Error closing tab: {ex.Message}")
            MessageBox.Show($"Error closing tab: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Event Handlers - Project Operations"

    Private Sub btnDeleteProject_Click(sender As Object, e As EventArgs) Handles btnDeleteProject.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If

        Try
            If MessageBox.Show("Are you really sure you want to delete this project? This action cannot be undone.",
                              "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                DeleteProject()
            End If
        Catch ex As Exception
            UIHelper.Add($"Error deleting project: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Event Handlers - Report Buttons"

    Private Sub btnGenerateProjectReport_Click(sender As Object, e As EventArgs) Handles btnGenerateProjectReport.Click
        Try
            GenerateProjectReport()
        Catch ex As Exception
            UIHelper.Add($"Error generating project report: {ex.Message}")
            MessageBox.Show("Error generating project report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnPreviewIncExc_Click(sender As Object, e As EventArgs) Handles btnPreviewIncExc.Click
        Try
            GenerateInclusionsExclusionsReport()
        Catch ex As Exception
            UIHelper.Add($"Error generating Inclusions/Exclusions report: {ex.Message}")
            MessageBox.Show("Error generating Inclusions/Exclusions report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnViewPriceHistory_Click(sender As Object, e As EventArgs) Handles btnViewPriceHistory.Click
        If Not EnsureProjectAndVersion("Price History Viewer") Then Return

        Try
            Using frm As New frmPriceHistoryViewer(currentProject.ProjectID, currentProject.ProjectName)
                frm.ShowDialog()
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error opening price history viewer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Event Handlers - Lumber Operations"

    Private Sub btnUpdateLumber_Click(sender As Object, e As EventArgs) Handles btnUpdateLumber.Click
        If CurrentUser.IsAdmin Then
            UIHelper.ShowBusy(frmMain, "Updating lumber prices")
            Try
                UpdateLumberPrices()
                UIHelper.Add("Lumber prices updated successfully for version " & currentVersionID)
                MessageBox.Show("Lumber prices updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadVersionSpecificData()
            Catch ex As Exception
                UIHelper.Add($"Error updating lumber prices: {ex.Message}")
                MessageBox.Show("Error updating lumber prices: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                UIHelper.HideBusy(frmMain)
            End Try
        Else
            MsgBox("Admin only")
        End If
    End Sub

    Private Sub btnSetActive_Click(sender As Object, e As EventArgs) Handles btnSetActive.Click
        If CurrentUser.IsAdmin Then
            Try
                SetActiveLumberHistory()
            Catch ex As Exception
                UIHelper.Add($"Error setting active lumber history: {ex.Message}")
                MessageBox.Show($"Error setting active lumber history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MsgBox("Admin only")
        End If
    End Sub

    Private Sub btnDeleteLumberHistory_Click(sender As Object, e As EventArgs) Handles btnDeleteLumberHistory.Click
        Try
            DeleteLumberHistory()
        Catch ex As Exception
            UIHelper.Add($"Error deleting lumber history: {ex.Message}")
            MessageBox.Show($"Error deleting lumber history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnPullFutures_Click(sender As Object, e As EventArgs) Handles btnPullFutures.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If

        If currentVersionID <= 0 Then
            MessageBox.Show("No valid version selected. Save the project and select a version first.",
                           "Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim futures = LumberDataAccess.GetLumberFutures()
        If futures.Count = 0 Then
            Debug.WriteLine("No lumber futures data returned.")
            MessageBox.Show("No data returned – check internet or page layout.", "Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using conn As SqlConnection = SqlConnectionManager.Instance.GetConnection()
            If conn.State <> ConnectionState.Open Then
                conn.Open()
            End If

            Using tran = conn.BeginTransaction()
                Try
                    LumberDataAccess.SaveLumberFutures(currentVersionID, futures, conn, tran)
                    tran.Commit()
                    Debug.WriteLine($"Successfully saved {futures.Count} lumber futures for VersionID {currentVersionID}")
                Catch ex As Exception
                    tran.Rollback()
                    Debug.WriteLine("Transaction rollback: " & ex.Message)
                    Throw
                End Try
            End Using
        End Using

        LoadFuturesIntoListBox(currentVersionID)
    End Sub

    Private Sub lvFutures_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lvFutures.MouseDoubleClick
        If e.Button <> MouseButtons.Left Then Exit Sub

        Dim hit = lvFutures.HitTest(e.Location)
        If hit.Item Is Nothing Then Exit Sub

        Dim f = TryCast(hit.Item.Tag, LumberFutures)
        If f Is Nothing Then Exit Sub

        If Not f.Active Then
            If HandleSetActiveContract(f) Then Return
        End If

        HandleFuturesAdderCalculation(f)
    End Sub

#End Region

#Region "Event Handlers - Import Operations"

    Private Sub btnImportMiTek_Click(sender As Object, e As EventArgs) Handles btnImportMiTek.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If
        ImportMiTekDesignActuals()
    End Sub

    Private Sub btnImportBisTrack_Click(sender As Object, e As EventArgs) Handles btnImportBisTrack.Click
        ImportBisTrackInvoiceActuals()
    End Sub

    Private Sub btnImportWalls_Click(sender As Object, e As EventArgs) Handles btnImportWalls.Click
        ImportWallData()
    End Sub

#End Region

#Region "Event Handlers - Monday.com Integration"

    Private Sub btnLinkMonday_Click(sender As Object, e As EventArgs) Handles btnLinkMonday.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If

        Using searchForm As New frmMondaySearch With {.InitialSearchText = txtProjectName.Text.Trim()}
            If searchForm.ShowDialog() = DialogResult.OK AndAlso Not String.IsNullOrEmpty(searchForm.SelectedMondayItemId) Then
                txtMondayItemId.Text = searchForm.SelectedMondayItemId
                MessageBox.Show("Successfully linked to monday.com item: " & searchForm.SelectedMondayItemId,
                               "Linked!", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub btnViewMonday_Click(sender As Object, e As EventArgs) Handles btnViewMonday.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If

        Dim itemId As String = txtMondayItemId.Text.Trim()

        If String.IsNullOrWhiteSpace(itemId) OrElse Not IsNumeric(itemId) Then
            MessageBox.Show("No valid Monday.com Item ID found.", "Nothing to open", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Try
            Dim url As String = MondaycomAccess.GetMondayItemUrl(itemId)
            Process.Start(New ProcessStartInfo(url) With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show("Could not open browser: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Event Handlers - Form Events"

    Private Sub frmCreateEditProject_Enter(sender As Object, e As EventArgs) Handles Me.Enter
        If rollupCleared Then
            rollupCleared = False
        End If
    End Sub



#End Region

End Class