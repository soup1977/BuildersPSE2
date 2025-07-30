Option Strict On
Imports System.Windows.Forms
Imports BuildersPSE2.BuildersPSE.DataAccess
Imports BuildersPSE2.BuildersPSE.Models

Public Class frmCreateEditProject
    Private ReadOnly da As New DataAccess()
    Private currentProject As ProjectModel
    Private copiedLevels As List(Of LevelModel)
    Private isNewProject As Boolean = True

    Public Sub New(Optional selectedProj As ProjectModel = Nothing)
        InitializeComponent()
        cboCustomer.DataSource = da.GetCustomers()
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
        If selectedProj IsNot Nothing Then
            currentProject = selectedProj
            isNewProject = False
            LoadProjectData()
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
                    .LumberAdder = 0
                })
            Next
            LoadProjectData()

        End If
        tabControlRight.TabPages.Remove(tabOverrides)
        tabControlRight.TabPages.Remove(tabRollup)
        tabControlRight.TabPages.Remove(tabBuildingInfo)
        tabControlRight.TabPages.Remove(tabLevelInfo)
    End Sub

    Private Sub LoadProjectData()
        tvProjectTree.Nodes.Clear()
        Dim root As New TreeNode(currentProject.ProjectName) With {.Tag = currentProject}
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
        tabControlRight.TabPages.Add(tabProjectInfo)
        tabControlRight.TabPages.Add(tabOverrides)
        tabControlRight.TabPages.Add(tabRollup)
        LoadProjectInfo(currentProject)
        LoadOverrides(currentProject)
        LoadRollup(currentProject)

        cmbLevelType.DataSource = da.GetProductTypes()
        cmbLevelType.DisplayMember = "ProductTypeName"
        cmbLevelType.ValueMember = "ProductTypeID"

    End Sub

    Private Sub TvProjectTree_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvProjectTree.AfterSelect
        Dim selected As Object = tvProjectTree.SelectedNode.Tag
        If selected Is Nothing Then Exit Sub
        tabControlRight.TabPages.Clear()
        Select Case True
            Case TypeOf selected Is ProjectModel
                tabControlRight.TabPages.Add(tabProjectInfo)
                tabControlRight.TabPages.Add(tabOverrides)
                tabControlRight.TabPages.Add(tabRollup)
                LoadProjectInfo(CType(selected, ProjectModel))
                LoadOverrides(CType(selected, ProjectModel))
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
        txtProjectArchitect.Text = proj.ProjectArchitect
        txtProjectEngineer.Text = proj.ProjectEngineer
        txtProjectNotes.Text = proj.ProjectNotes
        dtpCreatedDate.Value = proj.CreatedDate
        dtpLastModified.Value = proj.LastModifiedDate

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
            currentProject.ProjectArchitect = txtProjectArchitect.Text
            currentProject.ProjectEngineer = txtProjectEngineer.Text
            currentProject.ProjectNotes = txtProjectNotes.Text
            currentProject.Customers = CType(dgvCustomerProject.DataSource, List(Of CustomerModel))
            currentProject.Salespeople = CType(dgvSalesProject.DataSource, List(Of SalesModel))
            da.SaveProject(currentProject)
            MessageBox.Show("Project info saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            LoadOverrides(currentProject)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveBuildingInfo_Click(sender As Object, e As EventArgs) Handles btnSaveBuildingInfo.Click
        Try
            Dim bldg As BuildingModel = CType(tvProjectTree.SelectedNode.Tag, BuildingModel)
            bldg.BuildingName = txtBuildingName.Text
            bldg.BldgQty = CInt(nudBldgQty.Value)
            da.SaveBuilding(bldg, currentProject.ProjectID)
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
            da.SaveLevel(level, level.BuildingID, currentProject.ProjectID)
            LoadProjectData()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
        da.SaveBuilding(newBldg, currentProject.ProjectID)
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
        da.SaveLevel(newLevel, selectedBldg.BuildingID, currentProject.ProjectID)
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
            da.SaveLevel(newLevel, targetBldg.BuildingID, currentProject.ProjectID)
        Next
        LoadProjectData()
    End Sub
    Private Sub btnAddCusttoProj_Click(sender As Object, e As EventArgs) Handles btnAddCusttoProj.Click
        If cboCustomer.SelectedIndex <> -1 Then
            Dim selectedCustomer As CustomerModel = CType(cboCustomer.SelectedItem, CustomerModel)
            If Not currentProject.Customers.Any(Function(c) c.CustomerID = selectedCustomer.CustomerID) Then
                currentProject.Customers.Add(selectedCustomer)
                dgvCustomerProject.DataSource = Nothing
                dgvCustomerProject.DataSource = currentProject.Customers
            Else
                MessageBox.Show("Customer already added.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub btnAddSalestoProj_Click(sender As Object, e As EventArgs) Handles btnAddSalestoProj.Click
        If cboSalesman.SelectedIndex <> -1 Then
            Dim selectedSales As SalesModel = CType(cboSalesman.SelectedItem, SalesModel)
            If Not currentProject.Salespeople.Any(Function(s) s.SalesID = selectedSales.SalesID) Then
                currentProject.Salespeople.Add(selectedSales)
                dgvSalesProject.DataSource = Nothing
                dgvSalesProject.DataSource = currentProject.Salespeople
            Else
                MessageBox.Show("Salesperson already added.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub EditPSEToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditPSEToolStripMenuItem.Click
        If currentProject IsNot Nothing AndAlso currentProject.ProjectID > 0 Then
            Dim pseForm As New FrmPSE(currentProject.ProjectID)
            pseForm.ShowDialog()
        Else
            MessageBox.Show("No valid project selected or project ID not available. Please save the project first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub btnRecalcRollup_Click(sender As Object, e As EventArgs) Handles btnRecalcRollup.Click

    End Sub
End Class