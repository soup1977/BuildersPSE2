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
        Public Property ArchitectID As Integer?
        Public Property EngineerID As Integer?
        Public Property ProjectNotes As String
        Public Property LastModifiedDate As Date
        Public Property CreatedDate As Date


        Public Property Buildings As New List(Of BuildingModel)

        Public Property Levels As New List(Of LevelModel) ' Direct levels under project
        Public Property Settings As New List(Of ProjectProductSettingsModel) ' Overrides per product type

        Public Property ArchitectName As String
        Public Property EngineerName As String



    End Class
End Namespace