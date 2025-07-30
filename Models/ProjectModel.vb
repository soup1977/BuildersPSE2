Option Strict On
Imports System.Collections.Generic

Namespace BuildersPSE.Models
    Public Class ProjectModel
        Public Property ProjectID As Integer
        Public Property JBID As String
        Public Property ProjectType As ProjectTypeModel = New ProjectTypeModel()

        Public Property ProjectName As String

        Public Property Estimator As EstimatorModel = New EstimatorModel()
        Public Property Address As String
        Public Property City As String
        Public Property State As String
        Public Property Zip As String
        Public Property BidDate As Date?
        Public Property ArchPlansDated As Date?
        Public Property EngPlansDated As Date?

        Public Property MilesToJobSite As Integer
        Public Property TotalNetSqft As Integer?
        Public Property TotalGrossSqft As Integer?
        Public Property ProjectArchitect As String
        Public Property ProjectEngineer As String
        Public Property ProjectNotes As String
        Public Property LastModifiedDate As Date
        Public Property CreatedDate As Date

        ' Collections for hierarchy

        Public Property Customers As List(Of CustomerModel) = New List(Of CustomerModel)()
        Public Property Salespeople As List(Of SalesModel) = New List(Of SalesModel)()
        Public Property Buildings As New List(Of BuildingModel)
        Public Property ActualUnits As New List(Of ActualUnitModel) ' Project-level units
        Public Property Levels As New List(Of LevelModel) ' Direct levels under project
        Public Property Settings As New List(Of ProjectProductSettingsModel) ' Overrides per product type

        Public ReadOnly Property PrimarySalesman As String
            Get
                If Salespeople.Count > 0 Then
                    Return Salespeople(0).SalesName
                Else
                    Return String.Empty
                End If
            End Get
        End Property

        Public ReadOnly Property PrimaryCustomer As String
            Get
                If Customers.Count > 0 Then
                    Return Customers(0).CustomerName
                Else
                    Return String.Empty
                End If
            End Get
        End Property

    End Class
End Namespace
