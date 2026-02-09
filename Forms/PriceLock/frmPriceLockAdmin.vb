' =====================================================
' frmPriceLockAdmin.vb
' Admin Menu for PriceLock Module - Access all management forms
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess

Public Class frmPriceLockAdmin

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess

#End Region

#Region "Constructor"

    Public Sub New()
        _dataAccess = New PriceLockDataAccess()
        InitializeComponent()
    End Sub

#End Region

#Region "Button Events"

    Private Sub btnPriceLocks_Click(sender As Object, e As EventArgs) Handles btnPriceLocks.Click
        Using frm As New frmPriceLockList()
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnComparisonReport_Click(sender As Object, e As EventArgs) Handles btnComparisonReport.Click
        Using frm As New frmPriceLockComparison(_dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnBuilders_Click(sender As Object, e As EventArgs) Handles btnBuilders.Click
        Using frm As New frmBuilderManagement(_dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnPlans_Click(sender As Object, e As EventArgs) Handles btnPlans.Click
        Using frm As New frmPlanManagement(_dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnOptions_Click(sender As Object, e As EventArgs) Handles btnOptions.Click
        Using frm As New frmOptionManagement(_dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnMaterialCategories_Click(sender As Object, e As EventArgs) Handles btnMaterialCategories.Click
        Using frm As New frmMaterialCategoryManagement(_dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnRandomLengthsImport_Click(sender As Object, e As EventArgs) Handles btnRandomLengthsImport.Click
        Using frm As New frmRandomLengthsImportManager()
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnRLMapping_Click(sender As Object, e As EventArgs) Handles btnRLMapping.Click
        Using frm As New frmMaterialCategoryRLMapping(_dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

#End Region

End Class