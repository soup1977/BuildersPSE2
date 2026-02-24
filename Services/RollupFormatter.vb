Option Strict On
Imports System.Windows.Forms

Namespace Services
    ''' <summary>
    ''' Formats RollupResult data for DataGridView display
    ''' </summary>
    Public Class RollupFormatter

        Public Function FormatForGrid(result As RollupResult) As DataGridView
            Dim dgv As New DataGridView With {
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            }

            dgv.Columns.Add("Category", "Category")

            ' Add appropriate value columns based on rollup type
            Select Case result.Type
                Case RollupType.Project
                    dgv.Columns.Add("Value", "Value")
                Case RollupType.Building, RollupType.Level
                    dgv.Columns.Add("Base", $"Base (x1)")
                    dgv.Columns.Add("Extended", $"Extended (x{result.BldgQty})")
            End Select

            If Not result.HasData Then
                dgv.Rows.Add("Status", result.StatusMessage)
                Return dgv
            End If

            Select Case result.Type
                Case RollupType.Project
                    FormatProjectRows(dgv, result)
                Case RollupType.Building
                    FormatBuildingRows(dgv, result)
                Case RollupType.Level
                    FormatLevelRows(dgv, result)
            End Select

            Return dgv
        End Function

        Private Sub FormatProjectRows(dgv As DataGridView, result As RollupResult)
            dgv.Rows.Add("Overall Sell", result.ExtendedSell.ToString("C2"))
            dgv.Rows.Add("Overall Cost", result.ExtendedCost.ToString("C2"))
            dgv.Rows.Add("Overall Delivery", result.ExtendedDelivery.ToString("C2"))
            dgv.Rows.Add("Overall SQFT", result.ExtendedSqft.ToString("N2"))
            dgv.Rows.Add("Overall BDFT", result.ExtendedBdft.ToString("N2"))
            dgv.Rows.Add("Total Gross SQFT (User-Entered)", result.TotalGrossSqft.ToString("N0"))
            dgv.Rows.Add("Calculated Gross SQFT (Rollup)", result.CalculatedGrossSqft.ToString("N0"))
            dgv.Rows.Add("Gross SQFT Mismatch", If(result.HasGrossSqftMismatch, "Yes - Review", "No"))
            dgv.Rows.Add("Margin", result.Margin.ToString("P1"))
            dgv.Rows.Add("Margin w/ Delivery", result.MarginWithDelivery.ToString("P1"))
            dgv.Rows.Add("PricePerBDFT", result.PricePerBdft.ToString("C2"))
            dgv.Rows.Add("PricePerSQFT", result.PricePerSqft.ToString("C2"))

            ' Add Futures Adder fields (MOVED INSIDE FormatProjectRows)
            dgv.Rows.Add("--- Futures Adder ---", "")  ' Changed from empty row to labeled separator
            dgv.Rows.Add("Futures Adder/MBF", result.FuturesAdderAmt.ToString("C2"))
            dgv.Rows.Add("Futures Adder Project Total", result.FuturesAdderProjTotal.ToString("C2"))
        End Sub

        Private Sub FormatBuildingRows(dgv As DataGridView, result As RollupResult)
            dgv.Rows.Add("Overall Sell", result.BaseSell.ToString("C2"), result.ExtendedSell.ToString("C2"))
            dgv.Rows.Add("Overall Cost", result.BaseCost.ToString("C2"), result.ExtendedCost.ToString("C2"))
            dgv.Rows.Add("Overall Delivery", result.BaseDelivery.ToString("C2"), result.ExtendedDelivery.ToString("C2"))
            dgv.Rows.Add("Overall SQFT", result.BaseSqft.ToString("N2"), result.ExtendedSqft.ToString("N2"))
            dgv.Rows.Add("Overall BDFT", result.BaseBdft.ToString("N2"), result.ExtendedBdft.ToString("N2"))
            dgv.Rows.Add("Calculated Gross SQFT (Per Bldg)", result.CalculatedGrossSqft.ToString("N0"), "-")
            dgv.Rows.Add("Margin", result.Margin.ToString("P1"), "-")
            dgv.Rows.Add("Margin w/ Delivery", result.MarginWithDelivery.ToString("P1"), "-")
            dgv.Rows.Add("PricePerBDFT", result.PricePerBdft.ToString("C2"), "-")
            dgv.Rows.Add("PricePerSQFT", result.PricePerSqft.ToString("C2"), "-")
        End Sub

        Private Sub FormatLevelRows(dgv As DataGridView, result As RollupResult)
            dgv.Rows.Add("Overall Price", result.BaseSell.ToString("C2"), result.ExtendedSell.ToString("C2"))
            dgv.Rows.Add("Overall Cost", result.BaseCost.ToString("C2"), result.ExtendedCost.ToString("C2"))
            dgv.Rows.Add("Overall SQFT", result.BaseSqft.ToString("N2"), result.ExtendedSqft.ToString("N2"))
            dgv.Rows.Add("Overall LF", result.OverallLf.ToString("N2"), result.ExtOverallLf.ToString("N2"))
            dgv.Rows.Add("Overall BDFT", result.BaseBdft.ToString("N2"), result.ExtendedBdft.ToString("N2"))
            dgv.Rows.Add("Lumber Cost", result.LumberCost.ToString("C2"), result.ExtLumberCost.ToString("C2"))
            dgv.Rows.Add("Plate Cost", result.PlateCost.ToString("C2"), result.ExtPlateCost.ToString("C2"))
            dgv.Rows.Add("Labor Cost", result.LaborCost.ToString("C2"), result.ExtLaborCost.ToString("C2"))
            dgv.Rows.Add("Design Cost", result.DesignCost.ToString("C2"), result.ExtDesignCost.ToString("C2"))
            dgv.Rows.Add("MGMT Cost", result.MgmtCost.ToString("C2"), result.ExtMgmtCost.ToString("C2"))
            dgv.Rows.Add("Job Supplies Cost", result.JobSuppliesCost.ToString("C2"), result.ExtJobSuppliesCost.ToString("C2"))
            dgv.Rows.Add("Items Cost", result.ItemsCost.ToString("C2"), result.ExtItemsCost.ToString("C2"))
            dgv.Rows.Add("Delivery Cost", result.BaseDelivery.ToString("C2"), result.ExtendedDelivery.ToString("C2"))
            dgv.Rows.Add("MGMT Material Cost comparison", (result.LumberCost + result.PlateCost).ToString("C2"), (result.ExtLumberCost + result.ExtPlateCost).ToString("C2"))
            dgv.Rows.Add("MGMT Labor Cost comparison",
                        (result.LaborCost + result.DesignCost + result.MgmtCost + result.JobSuppliesCost).ToString("C2"),
                        (result.ExtLaborCost + result.ExtDesignCost + result.ExtMgmtCost + result.ExtJobSuppliesCost).ToString("C2"))
            dgv.Rows.Add("Margin", result.Margin.ToString("P1"), "-")
            dgv.Rows.Add("PricePerBDFT", result.PricePerBdft.ToString("C2"), "-")
            dgv.Rows.Add("PricePerSQFT", result.PricePerSqft.ToString("C2"), "-")
        End Sub

    End Class

End Namespace