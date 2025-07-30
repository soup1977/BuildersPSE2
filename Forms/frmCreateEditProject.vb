Option Strict On

Imports System.Windows.Forms
Imports BuildersPSE2.BuildersPSE.DataAccess
Imports BuildersPSE2.BuildersPSE.Models


Public Class frmCreateEditProject
    Private da As New DataAccess()
    Private proj As ProjectModel
    Private isNew As Boolean = True

    Public Sub New(Optional selectedProj As ProjectModel = Nothing)
        InitializeComponent()
        ' Populate ComboBoxes with all Customers and Sales
        cboCustomer.DataSource = da.GetCustomers()
        cboCustomer.DisplayMember = "CustomerName"
        cboCustomer.ValueMember = "CustomerID"
        cboSalesman.DataSource = da.GetSales()
        cboSalesman.DisplayMember = "SalesName"
        cboSalesman.ValueMember = "SalesID"
        ' Populate new ComboBoxes for ProjectType and Estimator
        cboProjectType.DataSource = da.GetProjectTypes()
        cboProjectType.DisplayMember = "ProjectTypeName"
        cboProjectType.ValueMember = "ProjectTypeID"
        cboEstimator.DataSource = da.GetEstimators()
        cboEstimator.DisplayMember = "EstimatorName"
        cboEstimator.ValueMember = "EstimatorID"
        If selectedProj IsNot Nothing Then
            proj = selectedProj
            isNew = False
            LoadProjectData()
        Else
            proj = New ProjectModel With {
                .CreatedDate = DateTime.Today,  ' Set CreatedDate to Today for new projects
                .LastModifiedDate = DateTime.Now  ' Set LastModifiedDate to Now for new projects
                }  ' Initialize new ProjectModel instance
            ' Pre-populate settings for new project
            Dim productTypes As List(Of ProductTypeModel) = da.GetProductTypes()
            For Each pt In productTypes
                proj.Settings.Add(New ProjectProductSettingsModel With {
                .ProductTypeID = pt.ProductTypeID,
                .MarginPercent = 0,
                .LumberAdder = 0
            })
            Next
            LoadProjectData()  ' Init tree with root
        End If
        ' Hide non-project tabs until select
        tabControlRight.TabPages.Remove(tabOverrides)
        tabControlRight.TabPages.Remove(tabRollup)
        tabControlRight.TabPages.Remove(tabBuildingInfo)
        tabControlRight.TabPages.Remove(tabLevelInfo)
    End Sub

    Private Sub LoadProjectData()
        tvProjectTree.Nodes.Clear()
        Dim root As New TreeNode(proj.ProjectName) With {.Tag = proj}
        tvProjectTree.Nodes.Add(root)
        For Each bldg In proj.Buildings
            Dim bldgNode As New TreeNode(bldg.BuildingName) With {.Tag = bldg}
            root.Nodes.Add(bldgNode)
            For Each level In bldg.Levels
                Dim levelNode As New TreeNode(level.LevelName & " (" & level.ProductTypeName & ")") With {.Tag = level}  ' Updated: Use ProductTypeName for display
                bldgNode.Nodes.Add(levelNode)
            Next
        Next
        tvProjectTree.ExpandAll()
        tvProjectTree.SelectedNode = root
        tabControlRight.TabPages.Clear()
        tabControlRight.TabPages.Add(tabProjectInfo)
        tabControlRight.TabPages.Add(tabOverrides)
        tabControlRight.TabPages.Add(tabRollup)
        LoadProjectInfo(proj)
        LoadOverrides(proj)

        cmbLevelType.DataSource = da.GetProductTypes()
        cmbLevelType.DisplayMember = "ProductTypeName"
        cmbLevelType.ValueMember = "ProductTypeID"
    End Sub

    Private Sub TreeNode_Selected(sender As Object, e As TreeViewEventArgs)
        Dim selected As Object = tvProjectTree.SelectedNode.Tag
        tabControlRight.TabPages.Clear()
        If TypeOf selected Is ProjectModel Then
            tabControlRight.TabPages.Add(tabProjectInfo)
            tabControlRight.TabPages.Add(tabOverrides)
            tabControlRight.TabPages.Add(tabRollup)
            LoadProjectInfo(CType(selected, ProjectModel))
            LoadOverrides(CType(selected, ProjectModel))
            LoadRollup(selected)
        ElseIf TypeOf selected Is BuildingModel Then
            tabControlRight.TabPages.Add(tabBuildingInfo)
            tabControlRight.TabPages.Add(tabRollup)
            LoadBuildingInfo(CType(selected, BuildingModel))
            LoadRollup(selected)
        ElseIf TypeOf selected Is LevelModel Then
            tabControlRight.TabPages.Add(tabLevelInfo)
            tabControlRight.TabPages.Add(tabRollup)
            LoadLevelInfo(CType(selected, LevelModel))
            LoadRollup(selected)
        End If
    End Sub

    Private Sub TvProjectTree_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvProjectTree.AfterSelect
        TreeNode_Selected(sender, e)
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
        If proj.BidDate.HasValue Then dtpBidDate.Value = proj.BidDate.Value
        If proj.ArchPlansDated.HasValue Then dtpArchPlansDated.Value = proj.ArchPlansDated.Value
        If proj.EngPlansDated.HasValue Then dtpEngPlansDated.Value = proj.EngPlansDated.Value
        nudMilesToJobSite.Value = proj.MilesToJobSite
        If proj.TotalNetSqft.HasValue Then nudTotalNetSqft.Value = proj.TotalNetSqft.Value
        If proj.TotalGrossSqft.HasValue Then nudTotalGrossSqft.Value = proj.TotalGrossSqft.Value
        txtProjectArchitect.Text = proj.ProjectArchitect
        txtProjectEngineer.Text = proj.ProjectEngineer
        txtProjectNotes.Text = proj.ProjectNotes
        dtpCreatedDate.Value = proj.CreatedDate
        dtpLastModified.Value = proj.LastModifiedDate

        ' Load Customers and Salespeople into grids
        dgvCustomerProject.DataSource = proj.Customers
        dgvCustomerProject.Columns("CustomerID").Visible = False
        dgvSalesProject.DataSource = proj.Salespeople
        dgvSalesProject.Columns("SalesID").Visible = False
    End Sub

    Private Sub LoadOverrides(proj As ProjectModel)
        dgvOverrides.DataSource = proj.Settings
        If dgvOverrides.Columns("ProductTypeName") Is Nothing Then
            Dim col As New DataGridViewComboBoxColumn With {
            .DataSource = da.GetProductTypes(),
            .DisplayMember = "ProductTypeName",
            .ValueMember = "ProductTypeID",
            .DataPropertyName = "ProductTypeID",
            .HeaderText = "Product Type",
            .Name = "ProductTypeName",
            .ReadOnly = True  ' Ensure editable
        }
            dgvOverrides.Columns.Add(col)
        End If
        dgvOverrides.Columns("MarginPercent").HeaderText = "Margin %"
        dgvOverrides.Columns("LumberAdder").HeaderText = "Lumber Adder"
        dgvOverrides.AllowUserToAddRows = True ' Enable adding new rows

        ' Hide unwanted columns and lock ProductTypeName (after DataSource is set to ensure all are generated)
        For Each col As DataGridViewColumn In dgvOverrides.Columns
            Select Case col.Name
                Case "MarginPercent", "LumberAdder", "ProductTypeName"
                    col.Visible = True

                Case Else
                    col.Visible = False
                    col.ReadOnly = True ' Hide all others (e.g., SettingID, ProjectID, ProductTypeID)
            End Select
        Next
        ' Format columns (after columns are configured)
        dgvOverrides.Columns("MarginPercent").DefaultCellStyle.Format = "P2"  ' Percentage with 2 decimals (e.g., 20.00%)
        dgvOverrides.Columns("LumberAdder").DefaultCellStyle.Format = "C2"  ' Currency with 2 decimals (e.g., $100.00)
    End Sub

    Private Sub LoadRollup(item As Object)
        dgvRollup.Rows.Clear()
        dgvRollup.Columns.Clear()
        ' Add columns for display
        dgvRollup.Columns.Add("Category", "Category")
        dgvRollup.Columns.Add("Value", "Value")

        If TypeOf item Is ProjectModel Then
            Dim proj As ProjectModel = CType(item, ProjectModel)
            If proj.Buildings Is Nothing OrElse proj.Buildings.Count = 0 Then
                dgvRollup.Rows.Add("Status", "No buildings defined yet. Add buildings and levels for rollup data.")
                Return  ' Skip computations if no data
            End If
            ' Fetch project rollups (e.g., sum extended from buildings)
            ' Dim totalPrice As Decimal = proj.Buildings.Sum(Function(b) CDec(If(b.ExtendedFloorCost, 0D)) + CDec(If(b.ExtendedRoofCost, 0D)) + CDec(If(b.ExtendedWallCost, 0D)))
            'Dim totalCost As Decimal = proj.Buildings.Sum(Function(b) CDec(If(b.OverallCost, 0D)) * b.BldgQty) ' Adjust if OverallCost is per-bldg

            ' Calculate rolled-up Gross SQFT from levels (sum OverallSQFT across all buildings/levels)
            Dim calculatedGrossSqft As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) CDec(If(l.OverallSQFT, 0D))))

            ' New project-level rollups
            Dim extendedSqft As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) CDec(If(l.OverallSQFT, 0D))) * b.BldgQty)
            Dim extendedBdft As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) CDec(If(l.OverallBDFT, 0D))) * b.BldgQty)
            Dim extendedCost As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) CDec(If(l.OverallCost, 0D))) * b.BldgQty)
            Dim extendedDelivery As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) CDec(If(l.DeliveryCost, 0D))) * b.BldgQty)
            Dim extendedSell As Decimal = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) CDec(If(l.OverallPrice, 0D))) * b.BldgQty)

            Dim totalSqft As Integer = If(proj.TotalGrossSqft.HasValue, CInt(proj.TotalGrossSqft.Value), 0) ' User-entered
            Dim totalNetSqft As Integer = If(proj.TotalNetSqft.HasValue, CInt(proj.TotalNetSqft.Value), 0) ' User-entered

            ' Only perform checks if there is calculated data (avoid warnings on empty/new projects)
            If calculatedGrossSqft > 0 Then
                dgvRollup.Rows.Add("Overall Sell", extendedSell.ToString("C2"))
                dgvRollup.Rows.Add("Overall Cost", extendedCost.ToString("C2"))
                dgvRollup.Rows.Add("Overall Delivery", extendedDelivery.ToString("C2"))
                dgvRollup.Rows.Add("Overall SQFT", extendedSqft.ToString("N2"))
                dgvRollup.Rows.Add("Overall BDFT", extendedBdft.ToString("N2"))
                dgvRollup.Rows.Add("Total Gross SQFT (User-Entered)", totalSqft.ToString("N0"))
                dgvRollup.Rows.Add("Calculated Gross SQFT (Rollup)", calculatedGrossSqft.ToString("N0"))

                ' Double-check: Alert if difference >1% threshold
                Dim grossThreshold As Double = 0.01
                If Math.Abs(calculatedGrossSqft - totalSqft) / If(totalSqft = 0, 1, totalSqft) > grossThreshold Then
                    MessageBox.Show("Warning: Calculated Gross SQFT (" & calculatedGrossSqft.ToString("N0") & ") differs from user-entered (" & totalSqft.ToString("N0") & ") by more than 1%. Review data.", "Double-Check Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    dgvRollup.Rows.Add("Gross SQFT Mismatch", "Yes - Review")
                Else
                    dgvRollup.Rows.Add("Gross SQFT Mismatch", "No")
                End If
                dgvRollup.Rows.Add("Margin", (((extendedSell - extendedDelivery) - extendedCost) / (extendedSell - extendedDelivery)).ToString("P1"))
                dgvRollup.Rows.Add("Margin w/ Delivery", ((extendedSell - (extendedCost + extendedDelivery)) / extendedSell).ToString("P1"))
                dgvRollup.Rows.Add("PricePerBDFT", (extendedSell / extendedBdft).ToString("c2"))
                dgvRollup.Rows.Add("PricePerSQFT", (extendedSell / extendedSqft).ToString("c2"))
                ' Add more as needed (e.g., BDFT sum)
            Else
                dgvRollup.Rows.Add("Status", "No rollup data available yet. Assign units to levels for calculations.")
            End If
        ElseIf TypeOf item Is BuildingModel Then
                Dim bldg As BuildingModel = CType(item, BuildingModel)
                dgvRollup.Rows.Add("Floor Cost Per Bldg", CDec(If(bldg.FloorCostPerBldg, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Roof Cost Per Bldg", CDec(If(bldg.RoofCostPerBldg, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Extended Floor Cost", CDec(If(bldg.ExtendedFloorCost, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Extended Roof Cost", CDec(If(bldg.ExtendedRoofCost, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Extended Wall Cost", CDec(If(bldg.ExtendedWallCost, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Overall Price", CDec(If(bldg.OverallPrice, 0D)).ToString("C2"))
                dgvRollup.Rows.Add("Overall Cost", CDec(If(bldg.OverallCost, 0D)).ToString("C2"))
                ' Add BDFT, SQFT sums from levels

            ElseIf TypeOf item Is LevelModel Then
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
            dgvRollup.Rows.Add("Margin", ((CDec(If(level.OverallPrice, 0D)) - CDec(If(level.OverallCost, 0D)) + CDec(If(level.DeliveryCost, 0D))) / CDec(level.OverallPrice)).ToString("P1"))
            ' Add more derived fields as needed

        End If
        dgvRollup.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    End Sub

    Private Sub LoadBuildingInfo(bldg As BuildingModel)
        txtBuildingName.Text = bldg.BuildingName
        nudBldgQty.Value = bldg.BldgQty
        txtBuildingType.Text = If(bldg.BuildingType.HasValue, bldg.BuildingType.Value.ToString(), String.Empty)  ' Updated: Handle null with empty string
        txtResUnits.Text = If(bldg.ResUnits.HasValue, bldg.ResUnits.Value.ToString(), String.Empty)  ' Updated: Handle null with empty string
        nudNbrUnits.Value = bldg.Levels.Sum(Function(l) l.ActualUnitMappings.Sum(Function(m) m.Quantity))
    End Sub

    Private Sub LoadLevelInfo(level As LevelModel)
        txtLevelName.Text = level.LevelName  ' If LevelName is still in use elsewhere; otherwise, remove if fully replaced
        cmbLevelType.SelectedValue = level.ProductTypeID  ' Updated: Use integer ProductTypeID
        nudLevelNumber.Value = level.LevelNumber
    End Sub

    Private Sub btnSaveProjectInfo_Click(sender As Object, e As EventArgs) Handles btnSaveProjectInfo.Click
        proj.JBID = txtJBID.Text
        proj.ProjectType.ProjectTypeID = CInt(cboProjectType.SelectedValue)
        proj.ProjectType.ProjectTypeName = cboProjectType.Text
        proj.ProjectName = txtProjectName.Text
        proj.Estimator.EstimatorID = CInt(cboEstimator.SelectedValue)
        proj.Estimator.EstimatorName = cboEstimator.Text
        proj.Address = txtAddress.Text
        proj.City = txtCity.Text
        proj.State = cboState.Text
        proj.Zip = txtZip.Text
        proj.BidDate = dtpBidDate.Value
        proj.ArchPlansDated = dtpArchPlansDated.Value
        proj.EngPlansDated = dtpEngPlansDated.Value
        proj.MilesToJobSite = CInt(nudMilesToJobSite.Value)
        proj.TotalNetSqft = If(nudTotalNetSqft.Value > 0, CInt(nudTotalNetSqft.Value), Nothing)
        proj.TotalGrossSqft = CInt(nudTotalGrossSqft.Value)
        proj.ProjectArchitect = txtProjectArchitect.Text
        proj.ProjectEngineer = txtProjectEngineer.Text
        proj.ProjectNotes = txtProjectNotes.Text

        ' Update Customers and Salespeople from grids
        proj.Customers = CType(dgvCustomerProject.DataSource, List(Of CustomerModel))
        proj.Salespeople = CType(dgvSalesProject.DataSource, List(Of SalesModel))

        da.SaveProject(proj)
        MessageBox.Show("Project info saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        LoadProjectData()
    End Sub

    Private Sub btnSaveOverrides_Click(sender As Object, e As EventArgs) Handles btnSaveOverrides.Click
        proj.Settings = CType(dgvOverrides.DataSource, List(Of ProjectProductSettingsModel))
        da.SaveProject(proj)
        MessageBox.Show("Overrides saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        LoadOverrides(proj)
    End Sub

    Private Sub btnSaveBuildingInfo_Click(sender As Object, e As EventArgs) Handles btnSaveBuildingInfo.Click
        Dim bldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
        bldg.BuildingName = txtBuildingName.Text
        bldg.BldgQty = CInt(nudBldgQty.Value)
        ' Not used at this time -   bldg.BuildingType = If(String.IsNullOrEmpty(txtBuildingType.Text), Nothing, (txtBuildingType.Text))
        da.SaveBuilding(bldg, proj.ProjectID)
        'MessageBox.Show("Building info saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        LoadProjectData()
    End Sub

    Private Sub btnSaveLevelInfo_Click(sender As Object, e As EventArgs) Handles btnSaveLevelInfo.Click
        Dim level As LevelModel = CType(tvProjectTree.SelectedNode.Tag, LevelModel)
        level.LevelName = txtLevelName.Text  ' If LevelName is still in use; otherwise, remove
        level.ProductTypeID = CInt(cmbLevelType.SelectedValue)  ' Updated: Save integer ProductTypeID
        level.ProductTypeName = CType(cmbLevelType.SelectedItem, ProductTypeModel).ProductTypeName  ' Optional: Save name for display
        level.LevelNumber = CInt(nudLevelNumber.Value)
        da.SaveLevel(level, level.BuildingID, proj.ProjectID)
        'MessageBox.Show("Level info saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        LoadProjectData()
    End Sub

    Private Sub cmsTreeMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles cmsTreeMenu.Opening
        mnuAddBuilding.Visible = TypeOf tvProjectTree.SelectedNode.Tag Is ProjectModel
        mnuAddLevel.Visible = TypeOf tvProjectTree.SelectedNode.Tag Is BuildingModel
        mnuEdit.Visible = True
        mnuDelete.Visible = True
        EditPSEToolStripMenuItem.Visible = True  ' Always visible, regardless of selected node type
    End Sub
    Private Sub mnuAddBuilding_Click(sender As Object, e As EventArgs) Handles mnuAddBuilding.Click
        Dim newBldg As New BuildingModel With {.BuildingName = "New Building", .BldgQty = 1}
        proj.Buildings.Add(newBldg)
        da.SaveBuilding(newBldg, proj.ProjectID)
        LoadProjectData()
    End Sub

    Private Sub mnuAddLevel_Click(sender As Object, e As EventArgs) Handles mnuAddLevel.Click
        Dim selectedBldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
        Dim newLevel As New LevelModel With {
            .LevelName = "New Level",
            .LevelNumber = selectedBldg.Levels.Count + 1,
            .ProductTypeID = 1,  ' Updated: Set default ProductTypeID (e.g., 1 for "Floor"); adjust based on your ProductType table
            .ProductTypeName = "Floor"  ' Optional: Set default name for display
        }
        selectedBldg.Levels.Add(newLevel)
        da.SaveLevel(newLevel, selectedBldg.BuildingID, proj.ProjectID)
        LoadProjectData()
    End Sub

    Private Sub mnuEdit_Click(sender As Object, e As EventArgs) Handles mnuEdit.Click
        ' Handled by selection changing tabs
    End Sub

    Private Sub mnuDelete_Click(sender As Object, e As EventArgs) Handles mnuDelete.Click
        If MessageBox.Show("Delete selected item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim selected As Object = tvProjectTree.SelectedNode.Tag
            Dim da As New DataAccess() ' Instantiate DataAccess (adjust if using a shared instance or dependency injection)
            If TypeOf selected Is BuildingModel Then
                Dim bldg As BuildingModel = CType(selected, BuildingModel)
                da.DeleteBuilding(bldg.BuildingID) ' Delete from DB first to ensure persistence
                proj.Buildings.Remove(bldg) ' Then remove from model
            ElseIf TypeOf selected Is LevelModel Then
                Dim level As LevelModel = CType(selected, LevelModel)
                da.DeleteLevel(level.LevelID) ' Delete from DB first
                Dim bldg As BuildingModel = proj.Buildings.First(Function(b) b.Levels.Contains(level))
                bldg.Levels.Remove(level) ' Then remove from model
            End If
            LoadProjectData() ' Reload to reflect changes
        End If
    End Sub

    Private Sub btnAddCusttoProj_Click(sender As Object, e As EventArgs) Handles btnAddCusttoProj.Click
        If cboCustomer.SelectedIndex <> -1 Then
            Dim selectedCustomer As CustomerModel = CType(cboCustomer.SelectedItem, CustomerModel)
            If Not proj.Customers.Any(Function(c) c.CustomerID = selectedCustomer.CustomerID) Then
                proj.Customers.Add(selectedCustomer)
                dgvCustomerProject.DataSource = Nothing
                dgvCustomerProject.DataSource = proj.Customers
            Else
                MessageBox.Show("Customer already added.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub btnAddSalestoProj_Click(sender As Object, e As EventArgs) Handles btnAddSalestoProj.Click
        If cboSalesman.SelectedIndex <> -1 Then
            Dim selectedSales As SalesModel = CType(cboSalesman.SelectedItem, SalesModel)
            If Not proj.Salespeople.Any(Function(s) s.SalesID = selectedSales.SalesID) Then
                proj.Salespeople.Add(selectedSales)
                dgvSalesProject.DataSource = Nothing
                dgvSalesProject.DataSource = proj.Salespeople
            Else
                MessageBox.Show("Salesperson already added.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub EditPSEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditPSEToolStripMenuItem.Click
        If proj IsNot Nothing AndAlso proj.ProjectID > 0 Then
            Dim pseForm As New FrmPSE(proj.ProjectID)
            pseForm.ShowDialog()  ' Open PSE form modally for focused editing
        Else
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
End Class