Option Strict On
Imports System.Windows.Forms
Imports BuildersPSE2.BuildersPSE.DataAccess
Imports BuildersPSE2.BuildersPSE.Models

Public Class frmCreateEditProject
    Private ReadOnly da As New DataAccess()
    Private currentProject As ProjectModel
    Private currentVersionID As Integer
    Private copiedLevels As List(Of LevelModel)
    Private isNewProject As Boolean = True
    Private isChangingVersion As Boolean = False

    Public Sub New(Optional selectedProj As ProjectModel = Nothing, Optional versionID As Integer = 0)
        InitializeComponent()
        cboCustomer.DataSource = da.GetCustomers(customerType:=1)
        cboCustomer.DisplayMember = "CustomerName"
        cboCustomer.ValueMember = "CustomerID"
        cboSalesman.DataSource = da.GetSales()
        cboSalesman.DisplayMember = "SalesName"
        cboSalesman.ValueMember = "SalesID"
        cboProjectType.DataSource = da.GetProjectTypes()
        cboProjectType.DisplayMember = "ProjectTypeName"
        cboProjectType.ValueMember = "ProjectTypeID"
        cboEstimator.DataSource = da.GetEstimators()
        cboEstimator.DisplayMember = "EstimatorName"
        cboEstimator.ValueMember = "EstimatorID"
        cboProjectArchitect.DataSource = da.GetCustomers(customerType:=2)
        cboProjectArchitect.DisplayMember = "CustomerName"
        cboProjectArchitect.ValueMember = "CustomerID"
        cboProjectEngineer.DataSource = da.GetCustomers(customerType:=3)
        cboProjectEngineer.DisplayMember = "CustomerName"
        cboProjectEngineer.ValueMember = "CustomerID"
        cboVersion.DataSource = New List(Of ProjectVersionModel)() ' Initialize to avoid null issues
        cboVersion.DisplayMember = "VersionName"
        cboVersion.ValueMember = "VersionID"
        cmbLevelType.DataSource = da.GetProductTypes()
        cmbLevelType.DisplayMember = "ProductTypeName"
        cmbLevelType.ValueMember = "ProductTypeID"

        If selectedProj IsNot Nothing Then
            currentProject = selectedProj
            isNewProject = False
        Else
            currentProject = New ProjectModel With {
                .CreatedDate = DateTime.Today,
                .LastModifiedDate = DateTime.Now
            }
            Dim productTypes As List(Of ProductTypeModel) = da.GetProductTypes()
            For Each pt In productTypes
                currentProject.Settings.Add(New ProjectProductSettingsModel With {
                    .ProductTypeID = pt.ProductTypeID,
                    .MarginPercent = 0,
                    .LumberAdder = 0,
                    .VersionID = 0
                })
            Next
        End If
        currentVersionID = versionID
        LoadVersions()
        LoadProjectData()
    End Sub
    Private Sub UpdateStatus(message As String)
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        If mdiParent IsNot Nothing Then
            mdiParent.StatusLabel.Text = $"{message} at {DateTime.Now:HH:mm:ss}"
        End If
    End Sub
    Private Sub LoadVersions()
        Try
            If currentProject.ProjectID = 0 Then
                cboVersion.DataSource = New List(Of ProjectVersionModel)()
                UpdateStatus("Status: Save the project to create a version.")
                tvProjectTree.Nodes.Clear()
                tvProjectTree.Nodes.Add(New TreeNode(currentProject.ProjectName & "-No Version") With {.Tag = currentProject})
                Return
            End If
            Dim versions As List(Of ProjectVersionModel) = da.GetProjectVersions(currentProject.ProjectID)
            isChangingVersion = True
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
                UpdateStatus("Status: No versions found. Save the project to create a base version.")
            End If
            isChangingVersion = False
            LoadVersionSpecificData()
        Catch ex As Exception
            isChangingVersion = False
            UpdateStatus("Status: Error loading versions: " & ex.Message)
            MessageBox.Show("Error loading versions: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub LoadProjectData()
        Try
            tabControlRight.TabPages.Clear()
            tabControlRight.TabPages.Add(tabProjectInfo)
            If Not isNewProject Then
                tabControlRight.TabPages.Add(tabOverrides)
                tabControlRight.TabPages.Add(tabRollup)
                tabControlRight.TabPages.Add(tabBuildingInfo)
                tabControlRight.TabPages.Add(tabLevelInfo)
            End If
            LoadProjectInfo(currentProject)
            LoadVersionSpecificData()
            UpdateStatus($"Loaded {tabControlRight.TabPages.Count} tab(s) for {(If(isNewProject, "new project", $"project ID {currentProject.ProjectID}"))}")
        Catch ex As Exception
            UpdateStatus($"Error loading project data: {ex.Message}")
            MessageBox.Show("Error loading project data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadVersionSpecificData()
        Try
            UpdateStatus("Loading version...")
            currentProject.Settings = da.GetProjectProductSettings(currentVersionID)
            LoadOverrides(currentProject.Settings)
            currentProject.Buildings = da.GetBuildingsByVersionID(currentVersionID)
            tvProjectTree.Nodes.Clear()
            Dim selectedVersion As ProjectVersionModel = If(currentVersionID > 0, da.GetProjectVersions(currentProject.ProjectID).FirstOrDefault(Function(v) v.VersionID = currentVersionID), Nothing)
            Dim rootText As String = If(selectedVersion IsNot Nothing, $"{currentProject.ProjectName}-{selectedVersion.VersionName}", currentProject.ProjectName & "-No Version")
            Dim root As New TreeNode(rootText) With {.Tag = currentProject}
            tvProjectTree.Nodes.Add(root)
            For Each bldg In currentProject.Buildings
                Dim bldgNode As New TreeNode(bldg.BuildingName) With {.Tag = bldg}
                root.Nodes.Add(bldgNode)
                For Each level In bldg.Levels
                    Dim levelNode As New TreeNode(String.Format("{0} ({1})", level.LevelName, level.ProductTypeName)) With {.Tag = level}
                    bldgNode.Nodes.Add(levelNode)
                Next
            Next
            tvProjectTree.ExpandAll()
            tvProjectTree.SelectedNode = root
            If selectedVersion IsNot Nothing Then
                cboCustomer.SelectedValue = If(selectedVersion.CustomerID, 0)
                cboSalesman.SelectedValue = If(selectedVersion.SalesID, 0)
                cboVersion.SelectedValue = currentVersionID
            End If
            LoadRollup(currentProject)
            UpdateStatus($"Loaded version {(If(selectedVersion IsNot Nothing, selectedVersion.VersionName, "No Version"))}")
        Catch ex As Exception
            UpdateStatus($"Error loading version-specific data: {ex.Message}")
            MessageBox.Show("Error loading version-specific data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cboVersion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboVersion.SelectedIndexChanged
        If cboVersion.SelectedValue IsNot Nothing AndAlso Not isChangingVersion Then
            isChangingVersion = True ' Prevent recursive calls
            currentVersionID = CInt(cboVersion.SelectedValue)
            LoadVersionSpecificData()
            isChangingVersion = False
        End If
    End Sub

    Private Sub LoadProjectInfo(proj As ProjectModel)
        txtJBID.Text = proj.JBID
        cboProjectType.SelectedValue = proj.ProjectType.ProjectTypeID
        txtProjectName.Text = proj.ProjectName
        cboEstimator.SelectedValue = proj.Estimator.EstimatorID
        txtAddress.Text = proj.Address
        txtCity.Text = proj.City
        cboState.Text = proj.State
        txtZip.Text = proj.Zip
        If proj.BidDate.HasValue Then dtpBidDate.Value = proj.BidDate.Value Else dtpBidDate.Value = DateTime.Today
        If proj.ArchPlansDated.HasValue Then dtpArchPlansDated.Value = proj.ArchPlansDated.Value Else dtpArchPlansDated.Value = DateTime.Today
        If proj.EngPlansDated.HasValue Then dtpEngPlansDated.Value = proj.EngPlansDated.Value Else dtpEngPlansDated.Value = DateTime.Today
        nudMilesToJobSite.Value = proj.MilesToJobSite
        If proj.TotalNetSqft.HasValue Then nudTotalNetSqft.Value = proj.TotalNetSqft.Value Else nudTotalNetSqft.Value = 0
        If proj.TotalGrossSqft.HasValue Then nudTotalGrossSqft.Value = proj.TotalGrossSqft.Value Else nudTotalGrossSqft.Value = 0

        cboProjectArchitect.SelectedValue = If(proj.ArchitectID, 0)
        cboProjectEngineer.SelectedValue = If(proj.EngineerID, 0)
        txtProjectNotes.Text = proj.ProjectNotes
        dtpCreatedDate.Value = proj.CreatedDate
        dtpLastModified.Value = proj.LastModifiedDate
    End Sub

    Private Sub LoadOverrides(settings As List(Of ProjectProductSettingsModel))
        dgvOverrides.DataSource = settings
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
                    col.HeaderText = "Lumber Adder"
                    col.DefaultCellStyle.Format = "C2"
                    col.Visible = True
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
        dgvRollup.Rows.Clear()
        dgvRollup.Columns.Clear()
        dgvRollup.Columns.Add("Category", "Category")
        dgvRollup.Columns.Add("Value", "Value")
        Select Case True
            Case TypeOf item Is ProjectModel
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
            Case TypeOf item Is BuildingModel
                Dim bldg As BuildingModel = CType(item, BuildingModel)
                dgvRollup.Rows.Add("Floor Cost Per Bldg", CDec(If(bldg.FloorCostPerBldg, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Roof Cost Per Bldg", CDec(If(bldg.RoofCostPerBldg, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Extended Floor Cost", CDec(If(bldg.ExtendedFloorCost, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Extended Roof Cost", CDec(If(bldg.ExtendedRoofCost, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Extended Wall Cost", CDec(If(bldg.ExtendedWallCost, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Overall Price", CDec(If(bldg.OverallPrice, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Overall Cost", CDec(If(bldg.OverallCost, 0D)).ToString("C2"))
            Case TypeOf item Is LevelModel
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
                Dim margin As Decimal = If(level.OverallPrice = 0, 0D, ((CDec((If(level.OverallPrice, 0D)) - CDec(If(level.DeliveryCost, 0D))) - CDec(If(level.OverallCost, 0D))) / CDec((If(level.OverallPrice, 0D)) - CDec(If(level.DeliveryCost, 0D)))))
                dgvRollup.Rows.Add("Margin", margin.ToString("P1"))
        End Select
        dgvRollup.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub

    Private Sub LoadBuildingInfo(bldg As BuildingModel)
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
        txtLevelName.Text = level.LevelName
        cmbLevelType.SelectedValue = level.ProductTypeID
        nudLevelNumber.Value = level.LevelNumber
    End Sub
    ' Add handlers for Add/Edit buttons
    Private Sub btnAddCustomer_Click(sender As Object, e As EventArgs) Handles btnAddCustomer.Click
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        Using frm As New FrmCustomerDialog(defaultTypeID:=1)
            UpdateStatus("Opened customer dialog for new customer")
            If frm.ShowDialog() = DialogResult.OK Then
                RefreshCustomerComboboxes()
                UpdateStatus("Customer added")
            End If
        End Using
    End Sub

    Private Sub btnEditCustomer_Click(sender As Object, e As EventArgs) Handles btnEditCustomer.Click
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        If cboCustomer.SelectedValue IsNot Nothing AndAlso cboCustomer.SelectedValue IsNot DBNull.Value Then
            Dim selectedCustomerID As Integer = CInt(cboCustomer.SelectedValue)
            Dim selectedCustomer As CustomerModel = da.GetCustomers(customerType:=1).FirstOrDefault(Function(c) c.CustomerID = selectedCustomerID)
            If selectedCustomer IsNot Nothing Then
                Using frm As New FrmCustomerDialog(selectedCustomer)
                    UpdateStatus($"Opened customer dialog for CustomerID {selectedCustomerID}")
                    If frm.ShowDialog() = DialogResult.OK Then
                        RefreshCustomerComboboxes()
                        UpdateStatus("Customer updated")
                    End If
                End Using
            End If
        Else
            UpdateStatus("No customer selected for editing")
            MessageBox.Show("Select a customer to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnAddArchitect_Click(sender As Object, e As EventArgs) Handles btnAddArchitect.Click
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        Using frm As New FrmCustomerDialog(defaultTypeID:=2)
            UpdateStatus("Opened architect dialog for new architect")
            If frm.ShowDialog() = DialogResult.OK Then
                RefreshCustomerComboboxes()
                UpdateStatus("Architect added")
            End If
        End Using
    End Sub

    Private Sub btnEditArchitect_Click(sender As Object, e As EventArgs) Handles btnEditArchitect.Click
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        If cboProjectArchitect.SelectedValue IsNot Nothing AndAlso cboProjectArchitect.SelectedValue IsNot DBNull.Value Then
            Dim selectedCustomerID As Integer = CInt(cboProjectArchitect.SelectedValue)
            Dim selectedCustomer As CustomerModel = da.GetCustomers(customerType:=2).FirstOrDefault(Function(c) c.CustomerID = selectedCustomerID)
            If selectedCustomer IsNot Nothing Then
                Using frm As New FrmCustomerDialog(selectedCustomer)
                    UpdateStatus($"Opened architect dialog for CustomerID {selectedCustomerID}")
                    If frm.ShowDialog() = DialogResult.OK Then
                        RefreshCustomerComboboxes()
                        UpdateStatus("Architect updated")
                    End If
                End Using
            End If
        Else
            UpdateStatus("No architect selected for editing")
            MessageBox.Show("Select an architect to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnAddEngineer_Click(sender As Object, e As EventArgs) Handles btnAddEngineer.Click
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        Using frm As New FrmCustomerDialog(defaultTypeID:=3)
            UpdateStatus("Opened engineer dialog for new engineer")
            If frm.ShowDialog() = DialogResult.OK Then
                RefreshCustomerComboboxes()
                UpdateStatus("Engineer added")
            End If
        End Using
    End Sub

    Private Sub btnEditEngineer_Click(sender As Object, e As EventArgs) Handles btnEditEngineer.Click
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        If cboProjectEngineer.SelectedValue IsNot Nothing AndAlso cboProjectEngineer.SelectedValue IsNot DBNull.Value Then
            Dim selectedCustomerID As Integer = CInt(cboProjectEngineer.SelectedValue)
            Dim selectedCustomer As CustomerModel = da.GetCustomers(customerType:=3).FirstOrDefault(Function(c) c.CustomerID = selectedCustomerID)
            If selectedCustomer IsNot Nothing Then
                Using frm As New FrmCustomerDialog(selectedCustomer)
                    UpdateStatus($"Opened engineer dialog for CustomerID {selectedCustomerID}")
                    If frm.ShowDialog() = DialogResult.OK Then
                        RefreshCustomerComboboxes()
                        UpdateStatus("Engineer updated")
                    End If
                End Using
            End If
        Else
            UpdateStatus("No engineer selected for editing")
            MessageBox.Show("Select an engineer to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' Helper to refresh all customer comboboxes after add/edit
    Private Sub RefreshCustomerComboboxes()
        cboCustomer.DataSource = da.GetCustomers(customerType:=1)
        cboProjectArchitect.DataSource = da.GetCustomers(customerType:=2)
        cboProjectEngineer.DataSource = da.GetCustomers(customerType:=3)
    End Sub
    Private Sub btnSaveProjectInfo_Click(sender As Object, e As EventArgs) Handles btnSaveProjectInfo.Click
        Try
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
            currentProject.ArchPlansDated = dtpArchPlansDated.Value
            currentProject.EngPlansDated = dtpEngPlansDated.Value
            currentProject.MilesToJobSite = CInt(nudMilesToJobSite.Value)
            currentProject.TotalNetSqft = If(nudTotalNetSqft.Value > 0, CInt(nudTotalNetSqft.Value), Nothing)
            currentProject.TotalGrossSqft = CInt(nudTotalGrossSqft.Value)
            currentProject.ArchitectID = If(cboProjectArchitect.SelectedValue IsNot DBNull.Value, CInt(cboProjectArchitect.SelectedValue), Nothing)
            currentProject.EngineerID = If(cboProjectEngineer.SelectedValue IsNot DBNull.Value, CInt(cboProjectEngineer.SelectedValue), Nothing)
            currentProject.ProjectNotes = txtProjectNotes.Text
            da.SaveProject(currentProject)
            Dim customerID As Integer? = If(cboCustomer.SelectedValue IsNot DBNull.Value, CInt(cboCustomer.SelectedValue), Nothing)
            Dim salesID As Integer? = If(cboSalesman.SelectedValue IsNot DBNull.Value, CInt(cboSalesman.SelectedValue), Nothing)
            If isNewProject AndAlso currentVersionID = 0 Then
                currentVersionID = da.CreateProjectVersion(currentProject.ProjectID, "v1", "Base Version", customerID, salesID)
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
            ElseIf currentVersionID > 0 Then
                da.UpdateProjectVersion(currentVersionID, cboVersion.Text, txtProjectNotes.Text, customerID, salesID)
            End If
            MessageBox.Show("Project and version info saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadVersions()
            LoadProjectData()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveOverrides_Click(sender As Object, e As EventArgs) Handles btnSaveOverrides.Click
        Try
            currentProject.Settings = CType(dgvOverrides.DataSource, List(Of ProjectProductSettingsModel))
            da.SaveProject(currentProject)
            MessageBox.Show("Overrides saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadOverrides(currentProject.Settings)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveBuildingInfo_Click(sender As Object, e As EventArgs) Handles btnSaveBuildingInfo.Click
        Try
            Dim bldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
            bldg.BuildingName = txtBuildingName.Text
            bldg.BldgQty = CInt(nudBldgQty.Value)
            da.SaveBuilding(bldg, currentVersionID)
            LoadProjectData()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveLevelInfo_Click(sender As Object, e As EventArgs) Handles btnSaveLevelInfo.Click
        Try
            Dim level As LevelModel = CType(tvProjectTree.SelectedNode.Tag, LevelModel)
            level.LevelName = txtLevelName.Text
            level.ProductTypeID = CInt(cmbLevelType.SelectedValue)
            level.ProductTypeName = CType(cmbLevelType.SelectedItem, ProductTypeModel).ProductTypeName
            level.LevelNumber = CInt(nudLevelNumber.Value)
            da.SaveLevel(level, level.BuildingID, currentVersionID)
            LoadProjectData()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TvProjectTree_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvProjectTree.AfterSelect
        Try
            Dim selected As Object = tvProjectTree.SelectedNode.Tag
            If selected Is Nothing Then
                tabControlRight.TabPages.Clear()
                Return
            End If
            tabControlRight.TabPages.Clear()
            Select Case True
                Case TypeOf selected Is ProjectModel
                    tabControlRight.TabPages.Add(tabProjectInfo)
                    tabControlRight.TabPages.Add(tabOverrides)
                    tabControlRight.TabPages.Add(tabRollup)
                    LoadProjectInfo(CType(selected, ProjectModel))
                    LoadOverrides(currentProject.Settings)
                    LoadRollup(selected)
                Case TypeOf selected Is BuildingModel
                    tabControlRight.TabPages.Add(tabBuildingInfo)
                    tabControlRight.TabPages.Add(tabRollup)
                    LoadBuildingInfo(CType(selected, BuildingModel))
                    LoadRollup(selected)
                    Me.BeginInvoke(Sub()
                                       tabControlRight.SelectedTab = tabBuildingInfo
                                       txtBuildingName.Focus()
                                       txtBuildingName.SelectAll()
                                   End Sub)
                Case TypeOf selected Is LevelModel
                    tabControlRight.TabPages.Add(tabLevelInfo)
                    tabControlRight.TabPages.Add(tabRollup)
                    LoadLevelInfo(CType(selected, LevelModel))
                    LoadRollup(selected)
                    Me.BeginInvoke(Sub()
                                       tabControlRight.SelectedTab = tabLevelInfo
                                       txtLevelName.Focus()
                                       txtLevelName.SelectAll()
                                   End Sub)
            End Select
        Catch ex As Exception
            MessageBox.Show("Error selecting tree node: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub cmsTreeMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsTreeMenu.Opening
        mnuAddBuilding.Visible = TypeOf tvProjectTree.SelectedNode.Tag Is ProjectModel
        mnuAddLevel.Visible = TypeOf tvProjectTree.SelectedNode.Tag Is BuildingModel
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
        Dim newBldg As New BuildingModel With {.BuildingName = "New Building", .BldgQty = 1}
        currentProject.Buildings.Add(newBldg)
        da.SaveBuilding(newBldg, currentVersionID)
        LoadProjectData()
    End Sub

    Private Sub mnuAddLevel_Click(sender As Object, e As EventArgs) Handles mnuAddLevel.Click
        Dim selectedBldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
        Dim newLevel As New LevelModel With {
            .LevelName = "New Level",
            .LevelNumber = selectedBldg.Levels.Count + 1,
            .ProductTypeID = 1,
            .ProductTypeName = "Floor"
        }
        selectedBldg.Levels.Add(newLevel)
        da.SaveLevel(newLevel, selectedBldg.BuildingID, currentVersionID)
        LoadProjectData()
    End Sub

    Private Sub mnuDelete_Click(sender As Object, e As EventArgs) Handles mnuDelete.Click
        If MessageBox.Show("Delete selected item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
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
            LoadProjectData()
        End If
    End Sub

    Private Sub mnuCopyLevels_Click(sender As Object, e As EventArgs) Handles mnuCopyLevels.Click
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
            da.SaveLevel(newLevel, targetBldg.BuildingID, currentVersionID)
        Next
        LoadProjectData()
    End Sub

    Private Sub EditPSEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditPSEToolStripMenuItem.Click
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            Try
                If currentVersionID <= 0 OrElse cboVersion.SelectedValue Is Nothing Then
                    UpdateStatus($"No version selected for ProjectID {currentProject.ProjectID}")
                    MessageBox.Show("No version selected. Please select or create a version first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                AddFormToTabControl(GetType(FrmPSE), $"PSE_{currentProject.ProjectID}_{currentVersionID}", New Object() {currentProject.ProjectID, currentVersionID})
                UpdateStatus($"Opened PSE form for ProjectID {currentProject.ProjectID}, VersionID {currentVersionID}")
            Catch ex As Exception
                UpdateStatus($"Error opening PSE form: {ex.Message}")
                MessageBox.Show("Error opening PSE form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            UpdateStatus("No valid project selected for PSE")
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Private Sub btnRecalcRollup_Click(sender As Object, e As EventArgs) Handles btnRecalcRollup.Click
        Try
            LoadRollup(currentProject)
            UpdateStatus("Rollup recalculated")
        Catch ex As Exception
            UpdateStatus($"Error recalculating rollup: {ex.Message}")
            MessageBox.Show($"Error recalculating rollup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCreateVersion_Click(sender As Object, e As EventArgs) Handles btnCreateVersion.Click
        Using frm As New frmVersionDialog()
            If frm.ShowDialog() = DialogResult.OK Then
                Try
                    Dim newVersionID As Integer = da.CreateProjectVersion(currentProject.ProjectID, frm.VersionName, frm.Description, Nothing, Nothing)
                    currentVersionID = newVersionID
                    Dim versions As List(Of ProjectVersionModel) = da.GetProjectVersions(currentProject.ProjectID)
                    cboVersion.DataSource = versions
                    cboVersion.DisplayMember = "VersionName"
                    cboVersion.ValueMember = "VersionID"
                    cboVersion.SelectedValue = currentVersionID
                    LoadVersionSpecificData()
                    MessageBox.Show("Version created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Error creating version: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub btnDuplicateVersion_Click(sender As Object, e As EventArgs) Handles btnDuplicateVersion.Click
        Using frm As New frmVersionDialog()
            If frm.ShowDialog() = DialogResult.OK Then
                Try
                    da.DuplicateProjectVersion(currentVersionID, frm.VersionName, frm.Description, currentProject.ProjectID)
                    Dim versions As List(Of ProjectVersionModel) = da.GetProjectVersions(currentProject.ProjectID)
                    currentVersionID = CInt((versions.FirstOrDefault(Function(v) v.VersionName = frm.VersionName)?.VersionID))
                    cboVersion.DataSource = versions
                    cboVersion.DisplayMember = "VersionName"
                    cboVersion.ValueMember = "VersionID"
                    cboVersion.SelectedValue = currentVersionID
                    LoadVersionSpecificData()
                    MessageBox.Show("Version duplicated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As ArgumentException
                    MessageBox.Show($"Cannot duplicate version: {ex.Message}", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Catch ex As Exception
                    MessageBox.Show($"Error duplicating version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub btnOpenPSE_Click(sender As Object, e As EventArgs) Handles btnOpenPSE.Click
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            Try
                If currentVersionID <= 0 OrElse cboVersion.SelectedValue Is Nothing Then
                    UpdateStatus($"No version selected for ProjectID {currentProject.ProjectID}")
                    MessageBox.Show("No version selected. Please select or create a version first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                AddFormToTabControl(GetType(FrmPSE), $"PSE_{currentProject.ProjectID}_{currentVersionID}", New Object() {currentProject.ProjectID, currentVersionID})
                UpdateStatus($"Opened PSE form for ProjectID {currentProject.ProjectID}, VersionID {currentVersionID}")
            Catch ex As Exception
                UpdateStatus($"Error opening PSE form: {ex.Message}")
                MessageBox.Show("Error opening PSE form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            UpdateStatus("No valid project selected for PSE")
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Try
            Dim tagValue As String = Me.Tag?.ToString()
            If String.IsNullOrEmpty(tagValue) Then
                Throw New Exception("Tab tag not found.")
            End If
            RemoveTabFromTabControl(tagValue)
            UpdateStatus($"Closed tab {tagValue} at {DateTime.Now:HH:mm:ss}")
        Catch ex As Exception
            UpdateStatus($"Error closing tab: {ex.Message} at {DateTime.Now:HH:mm:ss}")
            MessageBox.Show($"Error closing tab: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnIEOpen_Click(sender As Object, e As EventArgs) Handles btnIEOpen.Click
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            Try
                AddFormToTabControl(GetType(frmInclusionsExclusions), $"IE_{currentProject.ProjectID}", New Object() {currentProject.ProjectID})
                UpdateStatus($"Opened Inclusions/Exclusions form for ProjectID {currentProject.ProjectID}")
            Catch ex As Exception
                UpdateStatus($"Error opening Inclusions/Exclusions form: {ex.Message}")
                MessageBox.Show("Error opening Inclusions/Exclusions form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            UpdateStatus("No valid project selected for Inclusions/Exclusions")
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
End Class