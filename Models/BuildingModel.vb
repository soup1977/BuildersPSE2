Option Strict On
Imports System.Collections.Generic

Namespace BuildersPSE.Models
    Public Class BuildingModel
        Public Property BuildingID As Integer
        Public Property VersionID As Integer ' FK to Projects
        Public Property BuildingName As String
        Public Property BuildingType As Integer? ' e.g., 1, 2, 3
        Public Property ResUnits As Integer? ' e.g., 1, 2, 3
        Public Property BldgQty As Integer

        Public Property FloorCostPerBldg As Decimal? ' Cost per building for the floor
        Public Property RoofCostPerBldg As Decimal? ' Cost per unit for the floor
        Public Property WallCostPerBldg As Decimal? ' Cost per unit for the wall
        Public Property ExtendedFloorCost As Decimal? ' Extended cost for the floor
        Public Property ExtendedRoofCost As Decimal? ' Extended cost for the roof
        Public Property ExtendedWallCost As Decimal? ' Extended cost for the wall
        Public Property OverallPrice As Decimal? ' Overall price for the building
        Public Property OverallCost As Decimal? ' Overall cost for the building
        Public Property LastModifiedDate As DateTime? ' Last modified date for the building
        Public Property createdDate As DateTime? ' Creation date for the building


        ' Collections
        Public Property Levels As New List(Of LevelModel) ' Levels in this building
    End Class
End Namespace
