Option Strict On
Imports System.Data.SqlClient
Imports System.Globalization
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities
Imports Microsoft.Reporting.WinForms

Public Class frmCreateEditProject
    Private ReadOnly da As New ProjectDataAccess()
    Private currentProject As ProjectModel
    Private currentVersionID As Integer
    Private copiedLevels As List(Of LevelModel)
    Private copiedBuilding As BuildingModel
    Private isNewProject As Boolean = True
    Private isChangingVersion As Boolean = False

    Private Class LumberHistoryDate
        Public Property DisplayText As String
        Public Property CostEffectiveID As Integer
        Public Overrides Function ToString() As String
            Return DisplayText
        End Function
    End Class
    Private ReadOnly _mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)

    Public Sub New(Optional selectedProj As ProjectModel = Nothing, Optional versionID As Integer = 0)
        InitializeComponent()
        InitializeComboBoxes()
        InitializeProject(selectedProj)
        currentVersionID = versionID
        InitializeTabControl()
        LoadVersions()
        LoadProjectData()
        SetFormTitle()
    End Sub

    ' Initializes all combo boxes with their data sources.
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

    ' Initializes the project model for new or existing projects.
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
            txtEngPlanDate.PromptChar = " "c
            txtArchPlanDate.PromptChar = " "c
            txtEngPlanDate.ValidatingType = GetType(Date)
            txtArchPlanDate.ValidatingType = GetType(Date)
        Catch ex As Exception
            UIHelper.Add($"Error initializing project: {ex.Message}")
            MessageBox.Show($"Error initializing project: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Initializes default project settings for a new project.
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

    ' Sets the form title based on project state.
    Private Sub SetFormTitle()
        If isNewProject Then
            Me.Text = "Create Project"
        Else
            Me.Text = $"Edit Project - {currentProject.JBID}"
        End If
    End Sub




    ' Initializes the tab control based on project state.
    Private Sub InitializeTabControl()
        Try
            tabControlRight.TabPages.Clear()
            tabControlRight.TabPages.Add(tabProjectInfo)
            tabControlRight.TabPages.Add(tabRollup)
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

    ' Clears the version combo box and project tree for new projects.
    Private Sub ClearVersionComboAndTree()
        cboVersion.DataSource = New List(Of ProjectVersionModel)()
        UIHelper.Add("Status: Save the project to create a version.")
        tvProjectTree.Nodes.Clear()
        tvProjectTree.Nodes.Add(New TreeNode(currentProject.ProjectName & "-No Version") With {.Tag = currentProject})
    End Sub

    ' Binds project versions to the version combo box and sets the current version.
    Private Sub BindVersionsToCombo()
        isChangingVersion = True
        Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(currentProject.ProjectID)
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

    ' Loads cost-effective dates into the combo box.
    Private Sub LoadCostEffectiveDates()
        Dim dtCostEffective As DataTable = LumberDataAccess.GetAllLumberCostEffective()
        cboCostEffective.DataSource = dtCostEffective
        cboCostEffective.DisplayMember = "CosteffectiveDate"
        cboCostEffective.ValueMember = "CostEffectiveID"
        If dtCostEffective.Rows.Count > 0 Then
            cboCostEffective.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadProjectData()
        Try
            LoadProjectInfo(currentProject)
            LoadVersionSpecificData()
            LoadRollupForSelectedNode()
            UIHelper.Add($"Loaded data for {(If(isNewProject, "new project", $"project ID {currentProject.ProjectID}"))}")
        Catch ex As Exception
            UIHelper.Add($"Error loading project data: {ex.Message}")
            MessageBox.Show($"Error loading project data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Loads rollup data for the currently selected tree node or defaults to project.
    Private Sub LoadRollupForSelectedNode()
        If tvProjectTree.SelectedNode IsNot Nothing Then
            LoadRollup(tvProjectTree.SelectedNode.Tag)
        Else
            LoadRollup(currentProject)
        End If
    End Sub

    Private Sub LoadVersionSpecificData()
        Try
            UIHelper.Add("Loading version...")
            LoadProjectSettingsAndOverrides()
            LoadProjectBuildings()
            RefreshProjectTree()
            SetVersionComboBoxes()
            LoadAverageSPFPrices()
            LoadActiveSPFPrices()
            LoadLumberHistory()
            LoadProjectRollup()
            Dim selectedVersion As ProjectVersionModel = If(currentVersionID > 0, ProjVersionDataAccess.GetProjectVersions(currentProject.ProjectID).FirstOrDefault(Function(v) v.VersionID = currentVersionID), Nothing)
            UIHelper.Add($"Loaded version {(If(selectedVersion IsNot Nothing, selectedVersion.VersionName, "No Version"))}")
            LoadFuturesIntoListBox(currentVersionID)
        Catch ex As Exception
            UIHelper.Add($"Error loading version-specific data: {ex.Message}")
            MessageBox.Show("Error loading version-specific data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Loads project settings and applies overrides.
    Private Sub LoadProjectSettingsAndOverrides()
        currentProject.Settings = ProjectDataAccess.GetProjectProductSettings(currentVersionID)
        LoadOverrides(currentProject.Settings)
    End Sub

    ' Loads buildings for the current version.
    Public Sub LoadProjectBuildings()
        currentProject.Buildings = ProjectDataAccess.GetBuildingsByVersionID(currentVersionID)
    End Sub

    ' Refreshes the project tree view with current project data.
    Public Sub RefreshProjectTree()
        tvProjectTree.Nodes.Clear()
        Dim selectedVersion As ProjectVersionModel = If(currentVersionID > 0, ProjVersionDataAccess.GetProjectVersions(currentProject.ProjectID).FirstOrDefault(Function(v) v.VersionID = currentVersionID), Nothing)
        Dim rootText As String = If(selectedVersion IsNot Nothing, $"{currentProject.ProjectName}-{selectedVersion.VersionName}", currentProject.ProjectName & "-No Version")
        Dim root As New TreeNode(rootText) With {.Tag = currentProject}
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
        tvProjectTree.SelectedNode = root
    End Sub

    ' Sets combo box values based on the selected version.
    Private Sub SetVersionComboBoxes()
        Dim selectedVersion As ProjectVersionModel = If(currentVersionID > 0, ProjVersionDataAccess.GetProjectVersions(currentProject.ProjectID).FirstOrDefault(Function(v) v.VersionID = currentVersionID), Nothing)
        If selectedVersion IsNot Nothing Then
            cboCustomer.SelectedValue = If(selectedVersion.CustomerID, 0)
            cboSalesman.SelectedValue = If(selectedVersion.SalesID, 0)
            cboVersion.SelectedValue = currentVersionID
            txtMondayItemId.Text = selectedVersion.MondayID
        End If
    End Sub

    ' Loads average SPFNo2 prices for floors and roofs.
    Private Sub LoadAverageSPFPrices()
        txtRawFloorSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetAverageSPFNo2ByProductType(currentVersionID, "Floor").ToString("F2"), "0.00")
        txtRawRoofSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetAverageSPFNo2ByProductType(currentVersionID, "Roof").ToString("F2"), "0.00")
    End Sub

    ' Loads active SPFNo2 prices for floors and roofs.
    Private Sub LoadActiveSPFPrices()
        txtActiveFloorSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetActiveSPFNo2ByProductType(currentVersionID, "Floor").ToString("F2"), "0.00")
        txtActiveRoofSPFPrice.Text = If(currentVersionID > 0, LumberDataAccess.GetActiveSPFNo2ByProductType(currentVersionID, "Roof").ToString("F2"), "0.00")
    End Sub

    ' Loads lumber history dates into the list box.
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

    ' Loads project rollup data.
    Private Sub LoadProjectRollup()
        LoadRollup(currentProject)
    End Sub

    Private Sub cboVersion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboVersion.SelectedIndexChanged
        If cboVersion.SelectedValue IsNot Nothing AndAlso Not isChangingVersion Then
            isChangingVersion = True
            currentVersionID = CInt(cboVersion.SelectedValue)
            LoadVersionSpecificData()
            isChangingVersion = False
        End If
    End Sub

    Private Sub LoadProjectInfo(proj As ProjectModel)
        Try
            PopulateProjectInfoControls(proj)
        Catch ex As Exception
            UIHelper.Add($"Error loading project info: {ex.Message}")
            MessageBox.Show($"Error loading project info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Populates project info controls with project model data.
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
            txtArchPlanDate.Text = If(proj.ArchPlansDated.HasValue,
                          proj.ArchPlansDated.Value.ToString("MMddyyyy"),
                          String.Empty)
        End If
        If proj.EngPlansDated.HasValue Then
            txtEngPlanDate.Text = If(proj.EngPlansDated.HasValue,
                         proj.EngPlansDated.Value.ToString("MMddyyyy"),
                         String.Empty)
        End If
        nudMilesToJobSite.Value = proj.MilesToJobSite
        If proj.TotalNetSqft.HasValue Then
            nudTotalNetSqft.Value = proj.TotalNetSqft.Value
        Else
            nudTotalNetSqft.Value = 0
        End If
        If proj.TotalGrossSqft.HasValue Then
            nudTotalGrossSqft.Value = proj.TotalGrossSqft.Value
        Else
            nudTotalGrossSqft.Value = 0
        End If
        cboProjectArchitect.SelectedValue = If(proj.ArchitectID, 0)
        cboProjectEngineer.SelectedValue = If(proj.EngineerID, 0)
        txtProjectNotes.Text = proj.ProjectNotes
        dtpCreatedDate.Value = proj.CreatedDate
        dtpLastModified.Value = proj.LastModifiedDate
    End Sub

    Private Sub LoadOverrides(settings As List(Of ProjectProductSettingsModel))
        Try
            BindOverridesGrid(settings)
            ConfigureOverridesGridColumns()
        Catch ex As Exception
            UIHelper.Add($"Error loading overrides: {ex.Message}")
            MessageBox.Show($"Error loading overrides: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Binds settings to the overrides grid.
    Private Sub BindOverridesGrid(settings As List(Of ProjectProductSettingsModel))
        dgvOverrides.DataSource = settings
    End Sub

    ' Configures columns for the overrides grid.
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

    Private Sub LoadRollup(item As Object)
        Try
            InitializeRollupGrid()
            Select Case True
                Case TypeOf item Is ProjectModel
                    LoadProjectRollupData(item)
                Case TypeOf item Is BuildingModel
                    LoadBuildingRollupData(item)
                Case TypeOf item Is LevelModel
                    LoadLevelRollupData(item)
            End Select
            dgvRollup.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Catch ex As Exception
            UIHelper.Add($"Error loading rollup: {ex.Message}")
            MessageBox.Show($"Error loading rollup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Initializes the rollup grid columns.
    Private Sub InitializeRollupGrid()
        dgvRollup.Rows.Clear()
        dgvRollup.Columns.Clear()
        dgvRollup.Columns.Add("Category", "Category")
        dgvRollup.Columns.Add("Value", "Value")
    End Sub

    ' Loads rollup data for a project.
    Private Sub LoadProjectRollupData(item As Object)
        Dim proj As ProjectModel = CType(item, ProjectModel)
        If proj.Buildings Is Nothing OrElse proj.Buildings.Count = 0 Then
            dgvRollup.Rows.Add("Status", "No buildings defined yet. Add buildings and levels for rollup data.")
            Return
        End If
        Dim calculatedGrossSqft As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallSQFT, 0D)))
        Dim extendedSqft As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallSQFT, 0D)) * b.BldgQty)
        Dim extendedBdft As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallBDFT, 0D)) * b.BldgQty)
        Dim extendedCost As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallCost, 0D)) * b.BldgQty)
        Dim extendedDelivery As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.DeliveryCost, 0D)) * b.BldgQty)
        Dim extendedSell As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallPrice, 0D)) * b.BldgQty)
        Dim totalSqft As Integer = If(proj.TotalGrossSqft.HasValue, CInt(proj.TotalGrossSqft.Value), 0)
        Dim totalNetSqft As Integer = If(proj.TotalNetSqft.HasValue, CInt(proj.TotalNetSqft.Value), 0)
        If calculatedGrossSqft > 0 Then
            dgvRollup.Rows.Add("Overall Sell", extendedSell.ToString("C2"))
            dgvRollup.Rows.Add("Overall Cost", extendedCost.ToString("C2"))
            dgvRollup.Rows.Add("Overall Delivery", extendedDelivery.ToString("C2"))
            dgvRollup.Rows.Add("Overall SQFT", extendedSqft.ToString("N2"))
            dgvRollup.Rows.Add("Overall BDFT", extendedBdft.ToString("N2"))
            dgvRollup.Rows.Add("Total Gross SQFT (User-Entered)", totalSqft.ToString("N0"))
            dgvRollup.Rows.Add("Calculated Gross SQFT (Rollup)", calculatedGrossSqft.ToString("N0"))
            Dim grossThreshold As Double = 0.01
            If Math.Abs(calculatedGrossSqft - totalSqft) / If(totalSqft = 0, 1, totalSqft) > grossThreshold Then
                dgvRollup.Rows.Add("Gross SQFT Mismatch", "Yes - Review")
            Else
                dgvRollup.Rows.Add("Gross SQFT Mismatch", "No")
            End If
            Dim margin As Decimal = If(extendedSell - extendedDelivery = 0, 0D, ((extendedSell - extendedDelivery) - extendedCost) / (extendedSell - extendedDelivery))
            dgvRollup.Rows.Add("Margin", margin.ToString("P1"))
            Dim marginWithDelivery As Decimal = If(extendedSell = 0, 0D, (extendedSell - (extendedCost + extendedDelivery)) / extendedSell)
            dgvRollup.Rows.Add("Margin w/ Delivery", marginWithDelivery.ToString("P1"))
            Dim pricePerBDFT As Decimal = If(extendedBdft = 0, 0D, extendedSell / extendedBdft)
            dgvRollup.Rows.Add("PricePerBDFT", pricePerBDFT.ToString("C2"))
            Dim pricePerSQFT As Decimal = If(extendedSqft = 0, 0D, extendedSell / extendedSqft)
            dgvRollup.Rows.Add("PricePerSQFT", pricePerSQFT.ToString("C2"))
        Else
            dgvRollup.Rows.Add("Status", "No rollup data available yet. Assign units to levels for calculations.")
        End If
    End Sub

    ' Loads rollup data for a building.
    Private Sub LoadBuildingRollupData(item As Object)
        Dim bldg As BuildingModel = CType(item, BuildingModel)

        If bldg.Levels Is Nothing OrElse bldg.Levels.Count = 0 Then
            dgvRollup.Rows.Add("Status", "No levels defined yet. Add levels and assign units for rollup data.")
            Return
        End If

        ' Bottom-up from levels – identical to project logic
        Dim perBldgSqft As Decimal = bldg.Levels.Sum(Function(l) If(l.OverallSQFT, 0D))
        Dim perBldgBdft As Decimal = bldg.Levels.Sum(Function(l) If(l.OverallBDFT, 0D))
        Dim perBldgCost As Decimal = bldg.Levels.Sum(Function(l) If(l.OverallCost, 0D))
        Dim perBldgDelivery As Decimal = bldg.Levels.Sum(Function(l) If(l.DeliveryCost, 0D))
        Dim perBldgSell As Decimal = bldg.Levels.Sum(Function(l) If(l.OverallPrice, 0D))

        Dim extendedSqft As Decimal = perBldgSqft * bldg.BldgQty
        Dim extendedBdft As Decimal = perBldgBdft * bldg.BldgQty
        Dim extendedCost As Decimal = perBldgCost * bldg.BldgQty
        Dim extendedDelivery As Decimal = perBldgDelivery * bldg.BldgQty
        Dim extendedSell As Decimal = perBldgSell * bldg.BldgQty

        If perBldgSqft > 0 OrElse perBldgBdft > 0 OrElse perBldgCost > 0 OrElse perBldgSell > 0 Then
            dgvRollup.Rows.Add("Ext. Overall Sell", extendedSell.ToString("C2"))
            dgvRollup.Rows.Add("Ext. Overall Cost", extendedCost.ToString("C2"))
            dgvRollup.Rows.Add("Ext. Overall Delivery", extendedDelivery.ToString("C2"))
            dgvRollup.Rows.Add("Ext. Overall SQFT", extendedSqft.ToString("N2"))
            dgvRollup.Rows.Add("Ext. Overall BDFT", extendedBdft.ToString("N2"))
            dgvRollup.Rows.Add("Calculated Gross SQFT (Per Bldg)", perBldgSqft.ToString("N0"))

            Dim margin As Decimal = If(extendedSell - extendedDelivery = 0D, 0D, (extendedSell - extendedDelivery - extendedCost) / (extendedSell - extendedDelivery))
            dgvRollup.Rows.Add("Margin", margin.ToString("P1"))

            Dim marginWithDelivery As Decimal = If(extendedSell = 0D, 0D, (extendedSell - extendedCost - extendedDelivery) / extendedSell)
            dgvRollup.Rows.Add("Margin w/ Delivery", marginWithDelivery.ToString("P1"))

            Dim pricePerBDFT As Decimal = If(extendedBdft = 0D, 0D, extendedSell / extendedBdft)
            dgvRollup.Rows.Add("PricePerBDFT", pricePerBDFT.ToString("C2"))

            Dim pricePerSQFT As Decimal = If(extendedSqft = 0D, 0D, extendedSell / extendedSqft)
            dgvRollup.Rows.Add("PricePerSQFT", pricePerSQFT.ToString("C2"))
        Else
            dgvRollup.Rows.Add("Status", "No rollup data available yet. Assign units to levels for calculations.")
        End If
    End Sub

    ' Loads rollup data for a level.
    Private Sub LoadLevelRollupData(item As Object)
        Dim level As LevelModel = CType(item, LevelModel)
        dgvRollup.Rows.Add("Overall Price", CDec(If(level.OverallPrice, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Overall Cost", CDec(If(level.OverallCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Overall SQFT", CDec(If(level.OverallSQFT, 0D)).ToString("N2"))
        dgvRollup.Rows.Add("Overall LF", CDec(If(level.OverallLF, 0D)).ToString("N2"))
        dgvRollup.Rows.Add("Overall BDFT", CDec(If(level.OverallBDFT, 0D)).ToString("N2"))
        dgvRollup.Rows.Add("Lumber Cost", CDec(If(level.LumberCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Plate Cost", CDec(If(level.PlateCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Labor Cost", CDec(If(level.LaborCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Design Cost", CDec(If(level.DesignCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("MGMT Cost", CDec(If(level.MGMTCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Job Supplies Cost", CDec(If(level.JobSuppliesCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Items Cost", CDec(If(level.ItemsCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("Delivery Cost", CDec(If(level.DeliveryCost, 0D)).ToString("C2"))
        dgvRollup.Rows.Add("MGMT Material Cost comparison", (CDec(If(level.LumberCost, 0D)) + CDec(If(level.PlateCost, 0D))).ToString("C2"))
        dgvRollup.Rows.Add("MGMT Labor Cost comparison", (CDec(If(level.LaborCost, 0D)) + CDec(If(level.DesignCost, 0D)) + CDec(If(level.MGMTCost, 0D)) + CDec(If(level.JobSuppliesCost, 0D))).ToString("C2"))
        Dim margin As Decimal = If(level.OverallPrice = 0, 0D, ((CDec(If(level.OverallPrice, 0D)) - CDec(If(level.DeliveryCost, 0D))) - CDec(If(level.OverallCost, 0D))) / CDec(If(level.OverallPrice, 0D) - CDec(If(level.DeliveryCost, 0D))))
        dgvRollup.Rows.Add("Margin", margin.ToString("P1"))
        Dim pricePerBDFT As Decimal = CDec(If(level.OverallBDFT = 0, 0D, level.OverallPrice / level.OverallBDFT))
        dgvRollup.Rows.Add("PricePerBDFT", pricePerBDFT.ToString("C2"))
        Dim pricePerSQFT As Decimal = CDec(If(level.OverallSQFT = 0, 0D, level.OverallPrice / level.OverallSQFT))
        dgvRollup.Rows.Add("PricePerSQFT", pricePerSQFT.ToString("C2"))

    End Sub

    Private Sub LoadBuildingInfo(bldg As BuildingModel)
        Try
            PopulateBuildingInfoControls(bldg)
        Catch ex As Exception
            UIHelper.Add($"Error loading building info: {ex.Message}")
            MessageBox.Show($"Error loading building info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Populates building info controls with building model data.
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

    Private Sub LoadLevelInfo(level As LevelModel)
        Try
            PopulateLevelInfoControls(level)
        Catch ex As Exception
            UIHelper.Add($"Error loading level info: {ex.Message}")
            MessageBox.Show($"Error loading level info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Populates level info controls with level model data.
    Private Sub PopulateLevelInfoControls(level As LevelModel)
        txtLevelName.Text = level.LevelName
        cmbLevelType.SelectedValue = level.ProductTypeID
        nudLevelNumber.Value = level.LevelNumber
    End Sub

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

    ' Opens a customer dialog for adding a new customer.
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

    ' Edits the selected customer in the specified combo box.
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

    ' Refreshes all customer-related combo boxes while preserving current selections.
    Private Sub RefreshCustomerComboboxes()
        ' Store current selections
        Dim currentCustomerID As Object = cboCustomer.SelectedValue
        Dim currentArchitectID As Object = cboProjectArchitect.SelectedValue
        Dim currentEngineerID As Object = cboProjectEngineer.SelectedValue

        ' Refresh data sources
        Dim customers As List(Of CustomerModel) = HelperDataAccess.GetCustomers(customerType:=1)
        Dim architects As List(Of CustomerModel) = HelperDataAccess.GetCustomers(customerType:=2)
        Dim engineers As List(Of CustomerModel) = HelperDataAccess.GetCustomers(customerType:=3)

        ' Rebind
        cboCustomer.DataSource = customers
        cboProjectArchitect.DataSource = architects
        cboProjectEngineer.DataSource = engineers

        ' Restore selections if the ID still exists in the new list
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

        ' Fallback: if not found (e.g. deleted), select first item gracefully
        If cboCustomer.SelectedValue Is Nothing AndAlso customers.Any() Then cboCustomer.SelectedIndex = 0
        If cboProjectArchitect.SelectedValue Is Nothing AndAlso architects.Any() Then cboProjectArchitect.SelectedIndex = 0
        If cboProjectEngineer.SelectedValue Is Nothing AndAlso engineers.Any() Then cboProjectEngineer.SelectedIndex = 0
    End Sub

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
    Private Function TextToNullableDate(mtb As MaskedTextBox) As Date?
        If String.IsNullOrEmpty(mtb.Text) OrElse mtb.Text.Length <> 8 Then
            Return Nothing
        End If

        If DateTime.TryParseExact(mtb.Text, "MMddyyyy", Nothing, DateTimeStyles.None, Nothing) Then
            Return DateTime.ParseExact(mtb.Text, "MMddyyyy", Nothing).Date
        Else
            Return Nothing  ' invalid date → treat as null
        End If
    End Function
    ' Updates the project model with data from UI controls.
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

    ' Saves the project and creates/updates the version.
    Private Sub SaveProjectAndVersion()
        da.SaveProject(currentProject)
        Dim customerID As Integer? = If(cboCustomer.SelectedValue IsNot DBNull.Value, CInt(cboCustomer.SelectedValue), Nothing)
        Dim salesID As Integer? = If(cboSalesman.SelectedValue IsNot DBNull.Value, CInt(cboSalesman.SelectedValue), Nothing)
        If isNewProject AndAlso currentVersionID = 0 Then
            CreateInitialVersion(customerID, salesID)
        ElseIf currentVersionID > 0 Then
            ProjVersionDataAccess.UpdateProjectVersion(currentVersionID, cboVersion.Text, txtMondayItemId.Text, customerID, salesID)
        End If
    End Sub

    ' Creates the initial project version for a new project.
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

    ' Saves the overrides from the grid to the database.
    Private Sub SaveOverrides()
        currentProject.Settings = CType(dgvOverrides.DataSource, List(Of ProjectProductSettingsModel))
        For Each setting In currentProject.Settings
            setting.VersionID = currentVersionID
            da.SaveProjectProductSetting(setting, currentVersionID)
        Next
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

    ' Updates the building model with data from UI controls.
    Private Sub UpdateBuildingFromControls(bldg As BuildingModel)
        bldg.BuildingName = txtBuildingName.Text
        bldg.BldgQty = CInt(nudBldgQty.Value)
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

    ' Updates the level model with data from UI controls.
    Private Sub UpdateLevelFromControls(level As LevelModel)
        level.LevelName = txtLevelName.Text
        level.ProductTypeID = CInt(cmbLevelType.SelectedValue)
        level.ProductTypeName = CType(cmbLevelType.SelectedItem, ProductTypeModel).ProductTypeName
        level.LevelNumber = CInt(nudLevelNumber.Value)
    End Sub

    Private Sub TvProjectTree_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvProjectTree.AfterSelect
        Try
            UpdateTabsForSelectedNode()
            RefreshLevelVariance()
            RefreshBuildingVariance()

        Catch ex As Exception
            UIHelper.Add($"Error selecting tree node: {ex.Message}")
            MessageBox.Show("Error selecting tree node: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Updates tabs and data based on the selected tree node.
    Private Sub UpdateTabsForSelectedNode()
        Dim selected As Object = tvProjectTree.SelectedNode?.Tag
        If selected Is Nothing Then
            tabControlRight.TabPages.Clear()
            Return
        End If

        ' Remember the currently selected tab (by name, not reference – safer)
        Dim previouslySelectedTabName As String = If(tabControlRight.SelectedTab?.Name, String.Empty)

        ' Clear and rebuild tabs based on selected node type
        tabControlRight.TabPages.Clear()

        Select Case True
            Case TypeOf selected Is ProjectModel
                tabControlRight.TabPages.Add(tabProjectInfo)
                tabControlRight.TabPages.Add(tabOverrides)
                tabControlRight.TabPages.Add(tabRollup)

                LoadProjectInfo(CType(selected, ProjectModel))
                LoadOverrides(currentProject.Settings)
                LoadRollup(selected)

                ' Prefer Rollup if it was selected before, else default to ProjectInfo
                tabControlRight.SelectedTab = If(tabControlRight.TabPages.Contains(tabRollup) AndAlso
                                           previouslySelectedTabName = tabRollup.Name,
                                           tabRollup, tabProjectInfo)

            Case TypeOf selected Is BuildingModel
                tabControlRight.TabPages.Add(tabBuildingInfo)
                tabControlRight.TabPages.Add(tabRollup)

                LoadBuildingInfo(CType(selected, BuildingModel))
                LoadRollup(selected)

                ' Try to restore previous tab: Rollup if available and was selected
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

                ' Try to restore previous tab: Rollup if available and was selected
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

    Private Sub cmsTreeMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsTreeMenu.Opening
        ConfigureTreeMenuVisibility()
    End Sub

    ' Configures visibility of context menu items based on the selected node.
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

    Private Sub mnuAddBuilding_Click(sender As Object, e As EventArgs) Handles mnuAddBuilding.Click
        Try
            AddNewBuilding()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error adding building: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Adds a new building to the project.
    Private Sub AddNewBuilding()
        Dim newBldg As New BuildingModel With {.BuildingName = "New Building", .BldgQty = 1}
        currentProject.Buildings.Add(newBldg)
        ProjectDataAccess.SaveBuilding(newBldg, currentVersionID)
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

    ' Adds a new level to the selected building.
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

    ' Deletes the selected building or level.
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

    Private Sub mnuCopyLevels_Click(sender As Object, e As EventArgs) Handles mnuCopyLevels.Click
        Try
            CopyLevels()
        Catch ex As Exception
            UIHelper.Add($"Error copying levels: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Copies levels from the selected building.
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

    Private Sub mnuPasteLevels_Click(sender As Object, e As EventArgs) Handles mnuPasteLevels.Click
        Try
            PasteLevels()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error pasting levels: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Pastes copied levels into the selected building.
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

    Private Sub EditPSEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditPSEToolStripMenuItem.Click
        OpenPSEForm()
    End Sub

    ' Opens the PSE form for the current project and version.
    Private Sub OpenPSEForm()
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            Try
                If currentVersionID <= 0 OrElse cboVersion.SelectedValue Is Nothing Then
                    UIHelper.Add($"No version selected for ProjectID {currentProject.ProjectID}")
                    MessageBox.Show("No version selected. Please select or create a version first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                _mainForm.AddFormToTabControl(GetType(FrmPSE), $"PSE_{currentProject.ProjectID}_{currentVersionID}", New Object() {currentProject.ProjectID, currentVersionID})
                UIHelper.Add($"Opened PSE form for ProjectID {currentProject.ProjectID}, VersionID {currentVersionID}")
            Catch ex As Exception
                UIHelper.Add($"Error opening PSE form: {ex.Message}")
                MessageBox.Show("Error opening PSE form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            UIHelper.Add("No valid project selected for PSE")
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub RefreshRollupData()
        Try
            UpdateVersionAndSettings()
            LoadVersionSpecificData()
        Catch ex As Exception
            UIHelper.Add($"Error refreshing rollup data: {ex.Message}")
            MessageBox.Show($"Error refreshing rollup data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Updates version and settings data for rollup refresh.
    Private Sub UpdateVersionAndSettings()
        Dim selectedVersion As ProjectVersionModel = If(currentVersionID > 0, ProjVersionDataAccess.GetProjectVersions(currentProject.ProjectID).FirstOrDefault(Function(v) v.VersionID = currentVersionID), Nothing)
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

    Private Sub btnRecalcRollup_Click(sender As Object, e As EventArgs) Handles btnRecalcRollup.Click
        Try
            Dim currentTab As TabPage = tabControlRight.SelectedTab
            Dim selectedNode As TreeNode = tvProjectTree.SelectedNode
            UIHelper.Add("Recalculating rollup...")
            Me.Cursor = Cursors.WaitCursor
            SaveOverrides()
            RollupDataAccess.RecalculateVersion(currentVersionID)
            RefreshRollupData()
            RestoreRollupGrid(currentTab, selectedNode)
            UIHelper.Add("Rollup recalculated successfully")
        Catch ex As Exception
            UIHelper.Add($"Error recalculating rollup: {ex.Message}")
            MessageBox.Show($"Error recalculating rollup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ' Restores the rollup grid and selected tab/node after recalculation.
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

    ' Creates a new project version and refreshes the UI.
    Private Sub CreateNewVersion(versionName As String, description As String)
        Dim newVersionID As Integer = ProjVersionDataAccess.CreateProjectVersion(currentProject.ProjectID, versionName, description, Nothing, Nothing)
        currentVersionID = newVersionID
        Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(currentProject.ProjectID)
        cboVersion.DataSource = versions
        cboVersion.DisplayMember = "VersionName"
        cboVersion.ValueMember = "VersionID"
        cboVersion.SelectedValue = currentVersionID
        LoadVersionSpecificData()
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

    ' Duplicates the current version and refreshes the UI.
    Private Sub DuplicateVersion(versionName As String, description As String)
        ProjVersionDataAccess.DuplicateProjectVersion(currentVersionID, versionName, description, currentProject.ProjectID)
        Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(currentProject.ProjectID)
        currentVersionID = CInt((versions.FirstOrDefault(Function(v) v.VersionName = versionName)?.VersionID))
        cboVersion.DataSource = versions
        cboVersion.DisplayMember = "VersionName"
        cboVersion.ValueMember = "VersionID"
        cboVersion.SelectedValue = currentVersionID
        LoadVersionSpecificData()
    End Sub

    Private Sub btnOpenPSE_Click(sender As Object, e As EventArgs) Handles btnOpenPSE.Click
        OpenPSEForm()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Try
            CloseForm()
        Catch ex As Exception
            UIHelper.Add($"Error closing tab: {ex.Message}")
            MessageBox.Show($"Error closing tab: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Closes the form and removes it from the tab control.
    Private Sub CloseForm()
        Dim tagValue As String = Me.Tag?.ToString()
        If String.IsNullOrEmpty(tagValue) Then
            Throw New Exception("Tab tag not found.")
        End If


        _mainForm.RemoveTabFromTabControl(tagValue)
        UIHelper.Add($"Closed tab {tagValue} at {DateTime.Now:HH:mm:ss}")
    End Sub

    Private Sub btnIEOpen_Click(sender As Object, e As EventArgs) Handles btnIEOpen.Click
        Try
            OpenInclusionsExclusionsForm()
        Catch ex As Exception
            UIHelper.Add($"Error opening Inclusions/Exclusions form: {ex.Message}")
            MessageBox.Show("Error opening Inclusions/Exclusions form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Opens the Inclusions/Exclusions form.
    Private Sub OpenInclusionsExclusionsForm()
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            _mainForm.AddFormToTabControl(GetType(frmInclusionsExclusions), $"IE_{currentProject.ProjectID}", New Object() {currentProject.ProjectID})
            UIHelper.Add($"Opened Inclusions/Exclusions form for ProjectID {currentProject.ProjectID}")
        Else
            UIHelper.Add("No valid project selected for Inclusions/Exclusions")
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub mnuCopyBuilding_Click(sender As Object, e As EventArgs) Handles mnuCopyBuilding.Click
        Try
            CopyBuilding()
        Catch ex As Exception
            UIHelper.Add($"Error copying building: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Copies the selected building and its levels.
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

    Private Sub mnuPasteBuilding_Click(sender As Object, e As EventArgs) Handles mnuPasteBuilding.Click
        Try
            PasteBuilding()
            LoadProjectData()
        Catch ex As Exception
            UIHelper.Add($"Error pasting building: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Pastes the copied building and its levels.
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

    Private Sub tvProjectTree_MouseDown(sender As Object, e As MouseEventArgs) Handles tvProjectTree.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim node As TreeNode = tvProjectTree.GetNodeAt(e.X, e.Y)
            If node IsNot Nothing Then
                tvProjectTree.SelectedNode = node
            End If
        End If
    End Sub

    Private Sub btnDeleteProject_Click(sender As Object, e As EventArgs) Handles btnDeleteProject.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If
        Try
            If MessageBox.Show("Are you really sure you want to delete this project? This action cannot be undone.", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
                DeleteProject()
            End If
        Catch ex As Exception
            UIHelper.Add($"Error deleting project: {ex.Message}")
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Deletes the current project and closes the form.
    Private Sub DeleteProject()
        Dim notification As String = String.Empty
        da.DeleteProject(currentProject.ProjectID, notification)
        If Not String.IsNullOrEmpty(notification) Then
            MessageBox.Show(notification, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        btnClose.PerformClick()
    End Sub

    Private Sub btnGenerateProjectReport_Click(sender As Object, e As EventArgs) Handles btnGenerateProjectReport.Click

        Try
            GenerateProjectReport()
        Catch ex As Exception
            UIHelper.Add($"Error generating project report: {ex.Message}")
            MessageBox.Show("Error generating project report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Generates and displays the project summary report.
    Private Sub GenerateProjectReport()
        If currentProject Is Nothing OrElse currentProject.ProjectID <= 0 OrElse currentVersionID <= 0 Then
            UIHelper.Add("No valid project or version selected for report")
            MessageBox.Show("No valid project or version selected. Save the project and select a version first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        UIHelper.Add("Opening Project Summary Preview...")
        Dim dataSources As New List(Of ReportDataSource) From {
            New ReportDataSource("ProjectSummaryDataSet", ReportsDataAccess.GetProjectSummaryData(currentProject.ProjectID, currentVersionID))
        }
        Dim previewForm As New frmReportPreview("BuildersPSE2.ProjectSummary.rdlc", dataSources)
        previewForm.ShowDialog()
        UIHelper.Add("Project Summary Preview opened")
    End Sub

    Private Sub btnPreviewIncExc_Click(sender As Object, e As EventArgs) Handles btnPreviewIncExc.Click
        Try
            GenerateInclusionsExclusionsReport()
        Catch ex As Exception
            UIHelper.Add($"Error generating Inclusions/Exclusions report: {ex.Message}")
            MessageBox.Show("Error generating Inclusions/Exclusions report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Generates and displays the Inclusions/Exclusions report.
    Private Sub GenerateInclusionsExclusionsReport()
        If currentProject Is Nothing OrElse currentProject.ProjectID <= 0 OrElse currentVersionID <= 0 Then
            UIHelper.Add("No valid project or version selected for report")
            MessageBox.Show("No valid project or version selected. Save the project and select a version first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
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

    ' Updates lumber prices for the selected cost-effective date.
    Private Sub UpdateLumberPrices()
        If currentVersionID <= 0 Then
            UIHelper.Add("No valid version selected for lumber update")
            MessageBox.Show("No valid version selected. Please select or create a version first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        If cboCostEffective.SelectedValue Is Nothing OrElse CInt(cboCostEffective.SelectedValue) <= 0 Then
            UIHelper.Add("No cost-effective date selected for lumber update")
            MessageBox.Show("Please select a cost-effective date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If
        Dim costEffectiveID As Integer = CInt(cboCostEffective.SelectedValue)
        LumberDataAccess.UpdateLumberPrices(currentVersionID, costEffectiveID)
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

    ' Sets the selected lumber history as active.
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
                    Dim deactivateParams As New Dictionary(Of String, Object) From {
                        {"@VersionID", currentVersionID}
                    }
                    SqlConnectionManager.Instance.ExecuteNonQueryTransactional("UPDATE RawUnitLumberHistory SET IsActive = 0 WHERE VersionID = @VersionID", HelperDataAccess.BuildParameters(deactivateParams), conn, transaction)
                    Dim activateParams As New Dictionary(Of String, Object) From {
                        {"@CostEffectiveDateID", costEffectiveID},
                        {"@VersionID", currentVersionID}
                    }
                    SqlConnectionManager.Instance.ExecuteNonQueryTransactional("UPDATE RawUnitLumberHistory SET IsActive = 1, UpdateDate = GETDATE() WHERE CostEffectiveDateID = @CostEffectiveDateID AND VersionID = @VersionID", HelperDataAccess.BuildParameters(activateParams), conn, transaction)
                    transaction.Commit()
                    UIHelper.Add($"Set CostEffectiveDateID {costEffectiveID} as active for VersionID {currentVersionID}")
                    RollupDataAccess.RecalculateVersion(currentVersionID)
                    LoadVersionSpecificData()
                Catch ex As Exception
                    transaction.Rollback()
                    UIHelper.Add($"Error setting active lumber history: {ex.Message}")
                    Throw
                End Try
            End Using
        End Using
    End Sub

    Private Sub btnDeleteLumberHistory_Click(sender As Object, e As EventArgs) Handles btnDeleteLumberHistory.Click
        Try
            DeleteLumberHistory()
        Catch ex As Exception
            UIHelper.Add($"Error deleting lumber history: {ex.Message}")
            MessageBox.Show($"Error deleting lumber history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Deletes the selected lumber history.
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
        If MessageBox.Show($"Are you sure you want to delete all lumber history records for Cost Effective Date ID {costEffectiveID}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Dim params As New Dictionary(Of String, Object) From {
                {"@CostEffectiveDateID", costEffectiveID},
                {"@VersionID", currentVersionID}
            }
            SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteLumberHistory, HelperDataAccess.BuildParameters(params))
            UIHelper.Add($"Deleted lumber history for CostEffectiveDateID {costEffectiveID}")
            RollupDataAccess.RecalculateVersion(currentVersionID)
            LoadVersionSpecificData()
        End If
    End Sub

    Private Sub btnOpenProjectBuilder_Click(sender As Object, e As EventArgs) Handles btnOpenProjectBuilder.Click
        Try
            OpenProjectBuilderForm()
        Catch ex As Exception
            UIHelper.Add($"Error opening Project Builder form: {ex.Message}")
            MessageBox.Show("Error opening Project Builder form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Opens the Project Builder form.
    Private Sub OpenProjectBuilderForm()
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            If currentVersionID <= 0 OrElse cboVersion.SelectedValue Is Nothing Then
                UIHelper.Add($"No version selected for ProjectID {currentProject.ProjectID}")
                MessageBox.Show("No version selected. Please select or create a version first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            _mainForm.AddFormToTabControl(GetType(ProjectBuilderForm), $"ProjectBuilder_{currentVersionID}", New Object() {currentVersionID})
            UIHelper.Add($"Opened Project Builder form for ProjectID {currentProject.ProjectID}, VersionID {currentVersionID}")
        Else
            UIHelper.Add("No valid project selected for Project Builder")
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub btnPullFutures_Click(sender As Object, e As EventArgs) Handles btnPullFutures.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If
        Dim versionId As Integer = currentVersionID

        If versionId <= 0 Then
            MessageBox.Show("No valid version selected. Save the project and select a version first.", "Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim futures = LumberDataAccess.GetLumberFutures()
        If futures.Count = 0 Then
            Debug.WriteLine("No lumber futures data returned.")
            MessageBox.Show("No data returned – check internet or page layout.", "Futures", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Using conn As SqlConnection = SqlConnectionManager.Instance.GetConnection()
            ' <<< NEW: open only when really needed >>>
            If conn.State <> ConnectionState.Open Then
                conn.Open()
            End If

            Using tran = conn.BeginTransaction()
                Try
                    LumberDataAccess.SaveLumberFutures(versionId, futures, conn, tran)
                    tran.Commit()
                    Debug.WriteLine($"Successfully saved {futures.Count} lumber futures for VersionID {versionId}")
                Catch ex As Exception
                    tran.Rollback()
                    Debug.WriteLine("Transaction rollback: " & ex.Message)
                    Throw                              ' safe – Using blocks still dispose everything
                End Try
            End Using
        End Using                                     ' <<< conn.Dispose() always runs >>>

        LoadFuturesIntoListBox(versionId)
    End Sub


    Private Sub LoadFuturesIntoListBox(versionId As Integer)

        lvFutures.Items.Clear()
        lvFutures.Groups.Clear()
        lvFutures.BeginUpdate()          ' ← prevents flickering
        Try
            If versionId <= 0 Then Return

            Dim futures = LumberDataAccess.GetFuturesForVersion(versionId)
            If futures.Count = 0 Then Return

            ' Group by PullDate (date only)
            Dim groups = futures.GroupBy(Function(f) f.PullDate.Date)

            For Each grp In groups.OrderByDescending(Function(g) g.Key)   ' newest first
                Dim groupDateStr = grp.Key.ToString("MMMM d, yyyy")
                Dim header = $"Pulled on {groupDateStr} ({grp.Count()} contracts)"

                Dim lvg As New ListViewGroup(grp.Key.ToString("yyyyMMdd"), header)
                lvFutures.Groups.Add(lvg)

                For Each f In grp.OrderBy(Function(x) LumberDataAccess.MonthNameToNumber(x.ContractMonth.Split(" "c)(0)) * 100 + CInt(x.ContractMonth.Split(" "c)(1)))
                    ' *** THIS IS THE KEY FIX ***
                    Dim lvi As New ListViewItem(New String() {
                    f.ContractMonth,                                           ' Column 0
                    If(f.PriorSettle.HasValue, f.PriorSettle.Value.ToString("N2"), "—"), ' Column 1
                    f.PullDate.ToString("MM/dd/yyyy")                          ' Column 2
                }) With {
                        .Tag = f,
                        .Group = lvg
                }
                    lvFutures.Items.Add(lvi)
                Next
            Next

            ' Highlight today's pull (optional – makes it pop)
            If lvFutures.Groups.Count > 0 Then
                lvFutures.Groups(0).Header = lvFutures.Groups(0).Header.Substring(9)
            End If

            ' Auto-select nearest expiry (unchanged logic)
            If lvFutures.Items.Count > 0 Then
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
            End If

        Finally
            lvFutures.EndUpdate()
        End Try

        'lstFuturesold.Items.Clear()

        'If versionId <= 0 Then
        '    Debug.WriteLine("LoadFuturesIntoListBox: Invalid versionId")
        '    Return
        'End If

        'Dim futures = LumberDataAccess.GetFuturesForVersion(versionId)

        '' ---- Add the LumberFutures objects directly ----
        'For Each f As LumberFutures In futures
        '    lstFuturesold.Items.Add(f)          ' <-- ListBox now holds LumberFutures
        'Next

        '' ---- Auto-select nearest expiry (no late binding) ----
        'If futures.Count > 0 Then
        '    Dim today = DateTime.Today
        '    Dim bestIdx = 0
        '    Dim bestDiff = Double.MaxValue

        '    For i = 0 To futures.Count - 1
        '        Dim f = futures(i)                     ' <-- typed reference
        '        Dim parts = f.ContractMonth.Split(" "c)
        '        If parts.Length >= 2 Then
        '            Dim m = LumberDataAccess.MonthNameToNumber(parts(0))
        '            Dim y = CInt(parts(1))
        '            Dim yr = If(y < 50, 2000 + y, 1900 + y)
        '            If m > 0 Then
        '                Dim dt = New DateTime(yr, m, 1)
        '                Dim diff = Math.Abs((dt - today).TotalDays)
        '                If diff < bestDiff Then
        '                    bestDiff = diff
        '                    bestIdx = i
        '                End If
        '            End If
        '        End If
        '    Next
        '    lstFuturesold.SelectedIndex = bestIdx
        'End If
    End Sub
    ''' <summary>
    ''' Gets the current LevelID from the selected tree node
    ''' </summary>
    Private Function GetCurrentLevelID() As Integer
        If tvProjectTree.SelectedNode Is Nothing Then Return 0
        If TypeOf tvProjectTree.SelectedNode.Tag Is LevelModel Then
            Return CType(tvProjectTree.SelectedNode.Tag, LevelModel).LevelID
        End If
        Return 0
    End Function
    Private Function GetCurrentBuildingID() As Integer
        If tvProjectTree.SelectedNode Is Nothing Then Return 0
        If TypeOf tvProjectTree.SelectedNode.Tag Is BuildingModel Then
            Return CType(tvProjectTree.SelectedNode.Tag, BuildingModel).BuildingID
        End If
        Return 0
    End Function


    ''' <summary>
    ''' Refreshes the per-level variance grid — PIVOTED, MARGIN %, COLOR CODING, FORMATTING PRESERVED
    ''' </summary>
    Private Sub RefreshLevelVariance()
        Dim levelID As Integer = GetCurrentLevelID()
        If levelID <= 0 OrElse currentVersionID <= 0 Then
            dgvLevelVariance.DataSource = Nothing
            Exit Sub
        End If

        Dim dt As New DataTable()

        Try
            ' 1. GET ESTIMATE + PRODUCT TYPE
            Dim estimateRow As DataRow = Nothing
            Dim productTypeName As String = ""
            Using reader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectLevelEstimateWithProductType, {
            New SqlParameter("@LevelID", levelID),
            New SqlParameter("@VersionID", currentVersionID)
        })
                Dim estTable As New DataTable()
                estTable.Load(reader)
                If estTable.Rows.Count > 0 Then
                    estimateRow = estTable.Rows(0)
                    productTypeName = estimateRow("ProductTypeName").ToString()
                End If
            End Using

            If estimateRow Is Nothing Then
                dgvLevelVariance.DataSource = Nothing
                Exit Sub
            End If

            ' 2. GET AvgSPFNo2
            Dim avgSPFNo2 As Decimal = LumberDataAccess.GetActiveSPFNo2ByProductType(currentVersionID, productTypeName)

            ' 3. BUILD ESTIMATE DATA
            Dim estData As New Dictionary(Of String, Decimal) From {
            {"Sell Price", CDec(estimateRow("OverallPrice"))},
            {"BDFT", CDec(estimateRow("OverallBDFT"))},
            {"AvgSPF2", avgSPFNo2},
            {"LumberCost", CDec(estimateRow("LumberCost"))},
            {"PlateCost", CDec(estimateRow("PlateCost"))},
            {"ManufLaborCost", CDec(estimateRow("LaborCost"))},
            {"ManufLaborMH", CDec(estimateRow("LaborMH"))},
            {"ItemCost", CDec(estimateRow("ItemsCost"))},
            {"DeliveryCost", CDec(estimateRow("DeliveryCost"))},
            {"MiscLaborCost", CDec(estimateRow("DesignCost")) + CDec(estimateRow("MGMTCost")) + CDec(estimateRow("JobSuppliesCost"))},
            {"Total Cost", CDec(estimateRow("OverallCost"))}
        }

            ' Initialize DataTable
            dt.Columns.Add("Metric", GetType(String))
            dt.Columns.Add("Estimate", GetType(Decimal))

            For Each kvp In estData
                Dim row As DataRow = dt.NewRow()
                row("Metric") = kvp.Key
                row("Estimate") = kvp.Value
                dt.Rows.Add(row)
            Next

            ' 4. GET ALL ACTUALS — WITH REAL JOB/SO HEADERS
            Dim actuals As New List(Of (Source As String, Data As Dictionary(Of String, Decimal), Header As String))()
            Using reader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectAllActualsForLevel, {
    New SqlParameter("@LevelID", levelID),
    New SqlParameter("@VersionID", currentVersionID)
})
                Dim indexDesign As Integer = 1
                Dim indexInvoice As Integer = 1

                While reader.Read()
                    Dim stageType As Integer = CInt(reader("StageType"))
                    Dim miTekJobNumber As String = If(reader("MiTekJobNumber") Is DBNull.Value, "", reader("MiTekJobNumber").ToString().Trim())
                    Dim bisTrackSO As String = If(reader("BisTrackSalesOrder") Is DBNull.Value, "", reader("BisTrackSalesOrder").ToString().Trim())

                    ' BUILD HEADER
                    Dim header As String = ""
                    If stageType = 1 Then ' Design
                        header = If(String.IsNullOrEmpty(miTekJobNumber), $"Design{indexDesign}", $"Des - {miTekJobNumber}")
                        indexDesign += 1
                    Else ' Invoice
                        header = If(String.IsNullOrEmpty(bisTrackSO), $"Invoice{indexInvoice}", $"BT - {bisTrackSO}")
                        indexInvoice += 1
                    End If

                    Dim data As New Dictionary(Of String, Decimal) From {
            {"Sell Price", If(reader("ActualSoldAmount") Is DBNull.Value, 0D, CDec(reader("ActualSoldAmount")))},
            {"BDFT", If(reader("ActualBDFT") Is DBNull.Value, 0D, CDec(reader("ActualBDFT")))},
            {"AvgSPF2", If(reader("AvgSPFNo2Actual") Is DBNull.Value, 0D, CDec(reader("AvgSPFNo2Actual")))},
            {"LumberCost", If(reader("ActualLumberCost") Is DBNull.Value, 0D, CDec(reader("ActualLumberCost")))},
            {"PlateCost", If(reader("ActualPlateCost") Is DBNull.Value, 0D, CDec(reader("ActualPlateCost")))},
            {"ManufLaborCost", If(reader("ActualManufLaborCost") Is DBNull.Value, 0D, CDec(reader("ActualManufLaborCost")))},
            {"ManufLaborMH", If(reader("ActualManufMH") Is DBNull.Value, 0D, CDec(reader("ActualManufMH")))},
            {"ItemCost", If(reader("ActualItemCost") Is DBNull.Value, 0D, CDec(reader("ActualItemCost")))},
            {"DeliveryCost", If(reader("ActualDeliveryCost") Is DBNull.Value, 0D, CDec(reader("ActualDeliveryCost")))},
            {"MiscLaborCost", If(reader("ActualMiscLaborCost") Is DBNull.Value, 0D, CDec(reader("ActualMiscLaborCost")))},
            {"Total Cost", If(reader("ActualTotalCost") Is DBNull.Value, 0D, CDec(reader("ActualTotalCost")))}
        }

                    actuals.Add((header, data, header)) ' Source and Header are the same
                End While
            End Using

            ' Add actual columns using REAL HEADERS
            For Each act In actuals
                If Not dt.Columns.Contains(act.Header) Then
                    dt.Columns.Add(act.Header, GetType(Decimal))
                End If
                For Each row As DataRow In dt.Rows
                    Dim metric = row("Metric").ToString()
                    If act.Data.ContainsKey(metric) Then
                        row(act.Header) = act.Data(metric)
                    End If
                Next
            Next

            ' === ADD MARGIN % ROW (BEFORE TOTAL COST) ===
            Dim marginRow As DataRow = dt.NewRow()
            marginRow("Metric") = "Margin %"
            dt.Rows.InsertAt(marginRow, dt.Rows.Count)

            ' === CALCULATE MARGIN % FOR ALL COLUMNS (INCLUDING ESTIMATE) ===
            For i = 1 To dt.Columns.Count - 1
                Dim colName = dt.Columns(i).ColumnName

                ' Get Sell Price and Total Cost
                Dim sellPrice As Decimal = 0D
                Dim totalCost As Decimal = 0D

                Dim sellRow = dt.AsEnumerable().FirstOrDefault(Function(r) r.Field(Of String)("Metric") = "Sell Price")
                If sellRow IsNot Nothing AndAlso Not sellRow.IsNull(colName) Then
                    sellPrice = CDec(sellRow(colName))
                End If

                Dim costRow = dt.AsEnumerable().FirstOrDefault(Function(r) r.Field(Of String)("Metric") = "Total Cost")
                If costRow IsNot Nothing AndAlso Not costRow.IsNull(colName) Then
                    totalCost = CDec(costRow(colName))
                End If

                ' Calculate Margin %
                Dim marginPct As Decimal = If(sellPrice = 0, 0D, (sellPrice - totalCost) / sellPrice)
                marginRow(colName) = marginPct
            Next

            ' === BIND TO GRID FIRST ===
            dgvLevelVariance.DataSource = dt

            ' === FORMATTING (RESTORED) ===
            For Each row As DataGridViewRow In dgvLevelVariance.Rows
                If row.Cells("Metric").Value Is Nothing Then Continue For
                Dim metric = row.Cells("Metric").Value.ToString()

                For i = 1 To dgvLevelVariance.Columns.Count - 1
                    Dim cell = row.Cells(i)
                    If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then
                        cell.Value = 0
                    End If

                    Select Case metric
                        Case "Margin %"
                            cell.Style.Format = "P2"
                        Case "Sell Price", "LumberCost", "PlateCost", "ManufLaborCost", "ItemCost",
                         "DeliveryCost", "MiscLaborCost", "Total Cost"
                            cell.Style.Format = "C2"
                        Case "BDFT"
                            cell.Style.Format = "N0"
                        Case Else
                            cell.Style.Format = "N2"
                    End Select
                Next
            Next

            ' Right-align numeric columns
            For i = 1 To dgvLevelVariance.Columns.Count - 1
                dgvLevelVariance.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                dgvLevelVariance.Columns(i).MinimumWidth = 100
            Next

            ' Bold Metric column
            If dgvLevelVariance.Columns.Contains("Metric") Then
                dgvLevelVariance.Columns("Metric").DefaultCellStyle.Font = New Font(dgvLevelVariance.Font, FontStyle.Bold)
                dgvLevelVariance.Columns("Metric").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                dgvLevelVariance.Columns("Metric").MinimumWidth = 140
                dgvLevelVariance.Columns("Metric").DefaultCellStyle.Padding = New Padding(8, 0, 8, 0)
            End If

            ' === RESIZING ===
            dgvLevelVariance.AutoResizeColumns()
            Dim totalWidth As Integer = dgvLevelVariance.ClientSize.Width
            Dim metricWidth As Integer = If(dgvLevelVariance.Columns.Contains("Metric"), dgvLevelVariance.Columns("Metric").Width, 140)
            Dim remainingWidth As Integer = totalWidth - metricWidth - SystemInformation.VerticalScrollBarWidth
            Dim valueColumnCount As Integer = dgvLevelVariance.Columns.Count - 1
            If valueColumnCount > 0 Then
                Dim idealWidth As Integer = remainingWidth \ valueColumnCount
                For i = 1 To dgvLevelVariance.Columns.Count - 1
                    dgvLevelVariance.Columns(i).Width = Math.Max(idealWidth, 100)
                Next
            End If

            ' === COLOR CODING: > +5% RED, < -5% GREEN ===
            For Each row As DataGridViewRow In dgvLevelVariance.Rows
                If row.Cells("Metric").Value Is Nothing Then Continue For
                Dim metric = row.Cells("Metric").Value.ToString()


                Dim estimateValue As Decimal = 0D
                If row.Cells(1).Value IsNot DBNull.Value Then
                    estimateValue = CDec(row.Cells(1).Value)
                End If
                If estimateValue = 0 Then Continue For

                For colIndex = 2 To dgvLevelVariance.Columns.Count - 1
                    Dim cell = row.Cells(colIndex)
                    If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then
                        cell.Style.BackColor = Color.White
                        Continue For
                    End If

                    Dim actualValue As Decimal = CDec(cell.Value)
                    Dim pctDiff As Decimal = (actualValue - estimateValue) / estimateValue

                    ' REVERSE LOGIC FOR MARGIN %
                    If metric = "Margin %" Then
                        If pctDiff < -0.05D Then
                            cell.Style.BackColor = Color.LightCoral   ' Margin DOWN >5% = RED
                        ElseIf pctDiff > 0.05D Then
                            cell.Style.BackColor = Color.LightGreen   ' Margin UP >5% = GREEN
                        Else
                            cell.Style.BackColor = Color.White
                        End If
                    Else
                        ' NORMAL LOGIC FOR ALL OTHER METRICS
                        If pctDiff > 0.05D Then
                            cell.Style.BackColor = Color.LightCoral   ' Cost UP >5% = RED
                        ElseIf pctDiff < -0.05D Then
                            cell.Style.BackColor = Color.LightGreen   ' Cost DOWN >5% = GREEN
                        Else
                            cell.Style.BackColor = Color.White
                        End If
                    End If

                Next
            Next
            'Freeze first two columns of datagrid
            dgvLevelVariance.Columns(1).Frozen = True

        Catch ex As Exception
            Dim stackTrace As New Diagnostics.StackTrace(ex, True)
            Dim frame As Diagnostics.StackFrame = stackTrace.GetFrame(0)
            Dim lineNumber As Integer = If(frame IsNot Nothing, frame.GetFileLineNumber(), 0)
            Dim methodName As String = If(frame IsNot Nothing, frame.GetMethod().Name, "Unknown")

            Dim fullError As String = $"ERROR in {methodName}() at line {lineNumber}:{vbCrLf}{ex.Message}{vbCrLf}{vbCrLf}Stack:{vbCrLf}{ex.StackTrace}"

            dgvLevelVariance.DataSource = Nothing
            MessageBox.Show(fullError, "Variance Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Try
                IO.File.AppendAllText(
                IO.Path.Combine(Application.StartupPath, "VarianceErrors.log"),
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {fullError}{vbCrLf}{vbCrLf}")
            Catch
            End Try
        End Try
    End Sub
    ''' <summary>
    ''' Building-level variance – ONE estimate column + ONE cumulative ACTUAL column
    ''' Now uses the new BuildingID column in LevelActuals → faster, simpler, rock-solid
    ''' Shows: Estimate vs Total Invoiced Actuals (StageType = 2) for the entire building
    ''' Includes Level Count row and proper color logic
    ''' </summary>
    Private Sub RefreshBuildingVariance()
        Dim buildingID As Integer = GetCurrentBuildingID()
        If buildingID <= 0 OrElse currentVersionID <= 0 Then
            dgvBuildingVariance.DataSource = Nothing
            Exit Sub
        End If

        Dim dt As New DataTable()
        Try
            '====================================================================
            ' 1. GET BldgQty + ESTIMATE ROLLUP (unchanged – perfect)
            '====================================================================
            Dim bldgQty As Integer = 1
            Using r = SqlConnectionManager.Instance.ExecuteReader(
            "SELECT BldgQty FROM Buildings WHERE BuildingID = @BuildingID AND VersionID = @VersionID",
            {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", currentVersionID)})
                If r.Read() Then bldgQty = CInt(r("BldgQty"))
            End Using

            Dim levelCount As Integer = 0
            Dim est As New Dictionary(Of String, Decimal) From {
            {"Sell Price", 0D}, {"BDFT", 0D}, {"Level Count", 0D}, {"AvgSPF2", 0D},
            {"LumberCost", 0D}, {"PlateCost", 0D}, {"ManufLaborCost", 0D},
            {"ManufLaborMH", 0D}, {"ItemCost", 0D}, {"DeliveryCost", 0D},
            {"MiscLaborCost", 0D}, {"Total Cost", 0D}
        }

            Dim weightedEstSPF As Decimal = 0D, totalEstBDFT As Decimal = 0D

            Using r = SqlConnectionManager.Instance.ExecuteReader(
            Queries.SelectLevelsByBuilding & " AND l.VersionID = @VersionID",
            {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", currentVersionID)})

                While r.Read()
                    levelCount += 1
                    Dim b As Decimal = If(r("OverallBDFT") Is DBNull.Value, 0D, CDec(r("OverallBDFT")))
                    Dim spf As Decimal = LumberDataAccess.GetActiveSPFNo2ByProductType(currentVersionID, r("ProductTypeName").ToString())

                    weightedEstSPF += spf * b
                    totalEstBDFT += b

                    est("Sell Price") += CDec(If(r("OverallPrice") Is DBNull.Value, 0D, r("OverallPrice")))
                    est("BDFT") += b
                    est("LumberCost") += CDec(If(r("LumberCost") Is DBNull.Value, 0D, r("LumberCost")))
                    est("PlateCost") += CDec(If(r("PlateCost") Is DBNull.Value, 0D, r("PlateCost")))
                    est("ManufLaborCost") += CDec(If(r("LaborCost") Is DBNull.Value, 0D, r("LaborCost")))
                    est("ManufLaborMH") += CDec(If(r("LaborMH") Is DBNull.Value, 0D, r("LaborMH")))
                    est("ItemCost") += CDec(If(r("ItemsCost") Is DBNull.Value, 0D, r("ItemsCost")))
                    est("DeliveryCost") += CDec(If(r("DeliveryCost") Is DBNull.Value, 0D, r("DeliveryCost")))
                    est("MiscLaborCost") += CDec(If(r("DesignCost") Is DBNull.Value, 0D, r("DesignCost"))) _
                                        + CDec(If(r("MGMTCost") Is DBNull.Value, 0D, r("MGMTCost"))) _
                                        + CDec(If(r("JobSuppliesCost") Is DBNull.Value, 0D, r("JobSuppliesCost")))
                    est("Total Cost") += CDec(If(r("OverallCost") Is DBNull.Value, 0D, r("OverallCost")))
                End While
            End Using

            If levelCount = 0 Then
                dgvBuildingVariance.DataSource = Nothing
                Exit Sub
            End If

            If totalEstBDFT > 0 Then est("AvgSPF2") = weightedEstSPF / totalEstBDFT


            For Each k In est.Keys.Where(Function(x) x <> "AvgSPF2").ToList()
                est(k) *= bldgQty
            Next
            est("Level Count") = levelCount * bldgQty
            '====================================================================
            ' 2. ACTUALS – NOW USING BuildingID directly (MUCH FASTER & CLEANER)
            '====================================================================
            Dim act As New Dictionary(Of String, Decimal) From {
            {"Sell Price", 0D}, {"BDFT", 0D}, {"Level Count", 0D}, {"AvgSPF2", 0D},
            {"LumberCost", 0D}, {"PlateCost", 0D}, {"ManufLaborCost", 0D},
            {"ManufLaborMH", 0D}, {"ItemCost", 0D}, {"DeliveryCost", 0D},
            {"MiscLaborCost", 0D}, {"Total Cost", 0D}
        }

            Dim actualLevelCount As Integer = 0
            Dim weightedActSPF As Decimal = 0D, totalActBDFT As Decimal = 0D

            ' NEW: Direct filter using BuildingID in LevelActuals
            Dim sqlActuals As String = "
            SELECT la.*
            FROM LevelActuals la
            WHERE la.BuildingID = @BuildingID
              AND la.VersionID = @VersionID
              AND la.StageType = 1"

            Using r = SqlConnectionManager.Instance.ExecuteReader(sqlActuals,
            {New SqlParameter("@BuildingID", buildingID),
             New SqlParameter("@VersionID", currentVersionID)})

                While r.Read()
                    actualLevelCount += 1

                    Dim b As Decimal = If(r("ActualBDFT") Is DBNull.Value, 0D, CDec(r("ActualBDFT")))
                    Dim spf As Decimal = If(r("AvgSPFNo2Actual") Is DBNull.Value, 0D, CDec(r("AvgSPFNo2Actual")))

                    totalActBDFT += b
                    weightedActSPF += spf * b

                    act("Sell Price") += CDec(If(r("ActualSoldAmount") Is DBNull.Value, 0D, r("ActualSoldAmount")))
                    act("BDFT") += b
                    act("LumberCost") += CDec(If(r("ActualLumberCost") Is DBNull.Value, 0D, r("ActualLumberCost")))
                    act("PlateCost") += CDec(If(r("ActualPlateCost") Is DBNull.Value, 0D, r("ActualPlateCost")))
                    act("ManufLaborCost") += CDec(If(r("ActualManufLaborCost") Is DBNull.Value, 0D, r("ActualManufLaborCost")))
                    act("ManufLaborMH") += CDec(If(r("ActualManufMH") Is DBNull.Value, 0D, r("ActualManufMH")))
                    act("ItemCost") += CDec(If(r("ActualItemCost") Is DBNull.Value, 0D, r("ActualItemCost")))
                    act("DeliveryCost") += CDec(If(r("ActualDeliveryCost") Is DBNull.Value, 0D, r("ActualDeliveryCost")))
                    act("MiscLaborCost") += CDec(If(r("ActualMiscLaborCost") Is DBNull.Value, 0D, r("ActualMiscLaborCost")))
                    act("Total Cost") += CDec(If(r("ActualTotalCost") Is DBNull.Value, 0D, r("ActualTotalCost")))
                End While
            End Using

            If totalActBDFT > 0 Then act("AvgSPF2") = weightedActSPF / totalActBDFT
            act("Level Count") = actualLevelCount

            '====================================================================
            ' 3. BUILD FINAL DATATABLE
            '====================================================================
            dt.Columns.Add("Metric", GetType(String))
            dt.Columns.Add("Estimate", GetType(Decimal))
            dt.Columns.Add("Actual", GetType(Decimal))

            For Each kv In est
                Dim row = dt.NewRow()
                row("Metric") = kv.Key
                row("Estimate") = kv.Value
                row("Actual") = If(act.ContainsKey(kv.Key), act(kv.Key), 0D)
                dt.Rows.Add(row)
            Next

            ' Insert Margin % row before Total Cost
            Dim marginRow = dt.NewRow()
            marginRow("Metric") = "Margin %"
            dt.Rows.InsertAt(marginRow, dt.Rows.Count - 1)

            For i = 1 To 2
                Dim col = dt.Columns(i).ColumnName
                Dim sell = CDec(dt.AsEnumerable().FirstOrDefault(Function(r) r.Field(Of String)("Metric") = "Sell Price")(col))
                Dim cost = CDec(dt.AsEnumerable().FirstOrDefault(Function(r) r.Field(Of String)("Metric") = "Total Cost")(col))
                marginRow(col) = If(sell = 0D, 0D, (sell - cost) / sell)
            Next

            '====================================================================
            ' 4. BIND + FORMATTING + COLOR CODING
            '====================================================================
            dgvBuildingVariance.DataSource = dt

            For Each row As DataGridViewRow In dgvBuildingVariance.Rows
                If row.Cells("Metric").Value Is Nothing Then Continue For
                Dim metric = row.Cells("Metric").Value.ToString()
                For i = 1 To 2
                    Dim cell = row.Cells(i)
                    cell.Value = If(cell.Value Is DBNull.Value OrElse cell.Value Is Nothing, 0D, cell.Value)

                    Select Case metric
                        Case "Margin %"
                            cell.Style.Format = "P2"
                        Case "Sell Price", "LumberCost", "PlateCost", "ManufLaborCost", "ItemCost",
                         "DeliveryCost", "MiscLaborCost", "Total Cost"
                            cell.Style.Format = "C2"
                        Case "BDFT", "Level Count"
                            cell.Style.Format = "N0"
                        Case Else
                            cell.Style.Format = "N2"
                    End Select
                Next
            Next

            For i = 1 To 2
                dgvBuildingVariance.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                dgvBuildingVariance.Columns(i).MinimumWidth = 130
            Next

            With dgvBuildingVariance.Columns("Metric")
                .DefaultCellStyle.Font = New Font(dgvBuildingVariance.Font, FontStyle.Bold)
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .MinimumWidth = 160
            End With

            dgvBuildingVariance.AutoResizeColumns()

            ' Color coding: higher good for Margin%, Sell Price, Level Count
            For Each row As DataGridViewRow In dgvBuildingVariance.Rows
                Dim metric = If(row.Cells("Metric").Value?.ToString(), "")
                Dim estVal = If(row.Cells("Estimate").Value Is DBNull.Value, 0D, CDec(row.Cells("Estimate").Value))
                Dim actVal = If(row.Cells("Actual").Value Is DBNull.Value, 0D, CDec(row.Cells("Actual").Value))
                Dim cell = row.Cells("Actual")

                If estVal = 0 Then
                    cell.Style.BackColor = Color.White
                    Continue For
                End If

                Dim diff = (actVal - estVal) / estVal

                If metric = "Margin %" OrElse metric = "Sell Price" OrElse metric = "Level Count" Then
                    cell.Style.BackColor = If(diff > 0.05D, Color.LightGreen,
                                         If(diff < -0.05D, Color.LightCoral, Color.White))
                Else
                    cell.Style.BackColor = If(diff > 0.05D, Color.LightCoral,
                                         If(diff < -0.05D, Color.LightGreen, Color.White))
                End If
            Next

            ' Freeze Estimate column
            If dgvBuildingVariance.Columns.Count > 1 Then
                dgvBuildingVariance.Columns(1).Frozen = True
            End If

        Catch ex As Exception
            MessageBox.Show("Building variance failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            dgvBuildingVariance.DataSource = Nothing
        End Try
    End Sub

    ''' <summary>
    ''' Refreshes the project-level variance summary — SAFE FOR NO RECORDS
    ''' </summary>



    Private Sub ColorVarianceCells()
        If dgvLevelVariance.DataSource Is Nothing Then Exit Sub

        ' Find Estimate column index (always 1)
        Dim estimateColIndex As Integer = 1 ' "Estimate" is column 1

        ' Loop through all rows
        For Each row As DataGridViewRow In dgvLevelVariance.Rows
            If row.Cells("Metric").Value Is Nothing Then Continue For
            Dim metric = row.Cells("Metric").Value.ToString()

            ' Only color numeric rows (not headers or variance rows)
            If metric.Contains("Variance") OrElse metric.Contains("Margin") OrElse metric = "Metric" Then
                Continue For
            End If

            Dim estimateValue As Decimal = 0D
            If row.Cells(estimateColIndex).Value IsNot DBNull.Value Then
                estimateValue = CDec(row.Cells(estimateColIndex).Value)
            End If

            If estimateValue = 0 Then
                ' Skip if estimate is zero (avoid divide-by-zero)
                Continue For
            End If

            ' Loop through all value columns (skip Metric and Estimate)
            For colIndex = 2 To dgvLevelVariance.Columns.Count - 1
                Dim cell = row.Cells(colIndex)
                If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then
                    cell.Style.BackColor = Color.White
                    Continue For
                End If

                Dim actualValue As Decimal = CDec(cell.Value)
                Dim pctDiff As Decimal = (actualValue - estimateValue) / estimateValue

                If pctDiff > 0.05D Then
                    cell.Style.BackColor = Color.LightCoral ' +5% or more = RED
                ElseIf pctDiff < -0.05D Then
                    cell.Style.BackColor = Color.LightGreen ' -5% or more = GREEN
                Else
                    cell.Style.BackColor = Color.White ' Within ±5% = WHITE
                End If
            Next
        Next
    End Sub

    ' REPLACE YOUR OLD btnImportMiTek_Click
    Private Sub btnImportMiTek_Click(sender As Object, e As EventArgs) Handles btnImportMiTek.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If
        Using dlg As New OpenFileDialog With {.Filter = "CSV files|*.csv", .Title = "Select MiTek Design Export"}
            If dlg.ShowDialog() = DialogResult.OK Then
                Dim frm As New frmActualsMatcher(currentVersionID, 1, dlg.FileName) ' 1 = Design
                If frm.ShowDialog() = DialogResult.OK Then
                    RefreshLevelVariance()

                    MessageBox.Show("MiTek actuals imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End Using
    End Sub

    ' REPLACE YOUR OLD btnImportBisTrack_Click
    Private Sub btnImportBisTrack_Click(sender As Object, e As EventArgs) Handles btnImportBisTrack.Click
        Using dlg As New OpenFileDialog With {.Filter = "CSV files|*.csv", .Title = "Select BisTrack Invoice Export"}
            If dlg.ShowDialog() = DialogResult.OK Then
                Dim frm As New frmActualsMatcher(currentVersionID, 2, dlg.FileName) ' 2 = Invoice
                If frm.ShowDialog() = DialogResult.OK Then
                    RefreshLevelVariance()

                    MessageBox.Show("BisTrack invoice imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End Using
    End Sub

    Private Sub btnImportWalls_Click(sender As Object, e As EventArgs) Handles btnImportWalls.Click
        ' ------------------------------------------------------------------
        ' 1. Make sure we have a valid VersionID to import into
        ' ------------------------------------------------------------------
        Dim versionID As Integer

        If currentVersionID <= 0 OrElse cboVersion.SelectedValue Is Nothing Then
            MessageBox.Show("Please select or create a project version first.", "Wall Import", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        versionID = currentVersionID  ' <-- however you expose the current version

        ' ------------------------------------------------------------------
        ' 2. Let the user pick the CSV file
        ' ------------------------------------------------------------------
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select Wall Costing CSV Export"
            ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            ofd.Multiselect = False

            If ofd.ShowDialog() <> DialogResult.OK Then
                Return ' user cancelled
            End If

            Dim csvFilePath As String = ofd.FileName

            ' ------------------------------------------------------------------
            ' 3. Run the import (your new function)
            ' ------------------------------------------------------------------
            Try
                Dim summary As ImportWallSummary = ExternalImportDataAccess.ImportWallsInteractive(csvFilePath, versionID)

                If summary.WasCancelled Then
                    ' Nothing to do – user hit Cancel in the mapping form
                    Return
                End If

                If summary.Success Then
                    MessageBox.Show("Walls imported successfully!" & vbCrLf & vbCrLf & summary.GetSummaryText(),
                                "Wall Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Wall import failed. See details below:" & vbCrLf & vbCrLf & summary.GetSummaryText(),
                                "Wall Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                ' ------------------------------------------------------------------
                ' 4. Refresh whatever UI you have (grid, tree, etc.)
                ' ------------------------------------------------------------------
                RefreshRollupData()
                SqlConnectionManager.CloseAllDataReaders()
            Catch ex As Exception
                MessageBox.Show("Unexpected error during wall import:" & vbCrLf & ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub btnLinkMonday_Click(sender As Object, e As EventArgs) Handles btnLinkMonday.Click
        If Not CurrentUser.IsAdmin Then
            MsgBox("Admin only")
            Exit Sub
        End If
        Using searchForm As New frmMondaySearch With {
        .InitialSearchText = txtProjectName.Text.Trim()
    }
            If searchForm.ShowDialog() = DialogResult.OK AndAlso Not String.IsNullOrEmpty(searchForm.SelectedMondayItemId) Then
                txtMondayItemId.Text = searchForm.SelectedMondayItemId
                MessageBox.Show("Successfully linked to monday.com item: " & searchForm.SelectedMondayItemId, "Linked!", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        LoadProjectBuildings()
        RefreshProjectTree()
    End Sub

End Class