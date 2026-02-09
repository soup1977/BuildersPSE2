' =====================================================
' PriceLockDataAccess.vb
' Data Access Layer for PriceLock Module
' BuildersPSE2 Extension
' =====================================================

Option Strict On
Option Explicit On

Imports System.Data
Imports System.Data.SqlClient
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Namespace DataAccess

    ''' <summary>
    ''' Provides data access methods for all PriceLock entities.
    ''' Uses parameterized queries from PriceLockQueries class.
    ''' </summary>
    Public Class PriceLockDataAccess

        Private ReadOnly _connectionString As String

        Public Sub New()
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings("DbConn").ConnectionString
        End Sub

        Public Sub New(connectionString As String)
            _connectionString = connectionString
        End Sub

#Region "Builders"

        Public Function GetBuilders() As List(Of PLBuilder)
            Dim builders As New List(Of PLBuilder)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectBuilders, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            builders.Add(MapBuilder(reader))
                        End While
                    End Using
                End Using
            End Using
            Return builders
        End Function

        Public Function GetBuilderByID(builderID As Integer) As PLBuilder
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectBuilderByID, conn)
                    cmd.Parameters.AddWithValue("@BuilderID", builderID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapBuilder(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function InsertBuilder(builder As PLBuilder) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertBuilder, conn)
                    cmd.Parameters.AddWithValue("@BuilderName", builder.BuilderName)
                    cmd.Parameters.AddWithValue("@BuilderCode", If(builder.BuilderCode, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdateBuilder(builder As PLBuilder)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateBuilder, conn)
                    cmd.Parameters.AddWithValue("@BuilderID", builder.BuilderID)
                    cmd.Parameters.AddWithValue("@BuilderName", builder.BuilderName)
                    cmd.Parameters.AddWithValue("@BuilderCode", If(builder.BuilderCode, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@IsActive", builder.IsActive)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Function MapBuilder(reader As SqlDataReader) As PLBuilder
            Return New PLBuilder() With {
                .BuilderID = reader.GetInt32(reader.GetOrdinal("BuilderID")),
                .BuilderName = reader.GetString(reader.GetOrdinal("BuilderName")),
                .BuilderCode = If(reader.IsDBNull(reader.GetOrdinal("BuilderCode")), Nothing, reader.GetString(reader.GetOrdinal("BuilderCode"))),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            }
        End Function

#End Region

#Region "Subdivisions"

        Public Function GetSubdivisionsByBuilder(builderID As Integer) As List(Of PLSubdivision)
            Dim subdivisions As New List(Of PLSubdivision)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectSubdivisionsByBuilder, conn)
                    cmd.Parameters.AddWithValue("@BuilderID", builderID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            subdivisions.Add(MapSubdivision(reader))
                        End While
                    End Using
                End Using
            End Using
            Return subdivisions
        End Function

        Public Function GetAllActiveSubdivisions() As List(Of PLSubdivision)
            Dim subdivisions As New List(Of PLSubdivision)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectAllActiveSubdivisions, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            subdivisions.Add(MapSubdivision(reader))
                        End While
                    End Using
                End Using
            End Using
            Return subdivisions
        End Function

        Public Function GetSubdivisionByID(subdivisionID As Integer) As PLSubdivision
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectSubdivisionByID, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapSubdivision(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function InsertSubdivision(subdivision As PLSubdivision) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertSubdivision, conn)
                    cmd.Parameters.AddWithValue("@BuilderID", subdivision.BuilderID)
                    cmd.Parameters.AddWithValue("@SubdivisionName", subdivision.SubdivisionName)
                    cmd.Parameters.AddWithValue("@SubdivisionCode", If(subdivision.SubdivisionCode, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdateSubdivision(subdivision As PLSubdivision)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateSubdivision, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivision.SubdivisionID)
                    cmd.Parameters.AddWithValue("@BuilderID", subdivision.BuilderID)
                    cmd.Parameters.AddWithValue("@SubdivisionName", subdivision.SubdivisionName)
                    cmd.Parameters.AddWithValue("@SubdivisionCode", If(subdivision.SubdivisionCode, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@IsActive", subdivision.IsActive)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Function MapSubdivision(reader As SqlDataReader) As PLSubdivision
            Return New PLSubdivision() With {
                .SubdivisionID = reader.GetInt32(reader.GetOrdinal("SubdivisionID")),
                .BuilderID = reader.GetInt32(reader.GetOrdinal("BuilderID")),
                .SubdivisionName = reader.GetString(reader.GetOrdinal("SubdivisionName")),
                .SubdivisionCode = If(reader.IsDBNull(reader.GetOrdinal("SubdivisionCode")), Nothing, reader.GetString(reader.GetOrdinal("SubdivisionCode"))),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                .BuilderName = reader.GetString(reader.GetOrdinal("BuilderName"))
            }
        End Function

#End Region

#Region "Plans"

        Public Function GetPlans() As List(Of PLPlan)
            Dim plans As New List(Of PLPlan)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPlans, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            plans.Add(MapPlan(reader))
                        End While
                    End Using
                End Using
            End Using
            Return plans
        End Function

        Public Function GetPlansBySubdivision(subdivisionID As Integer) As List(Of PLPlan)
            Dim plans As New List(Of PLPlan)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPlansBySubdivision, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim plan = MapPlan(reader)
                            plan.SubdivisionPlanID = reader.GetInt32(reader.GetOrdinal("SubdivisionPlanID"))
                            plans.Add(plan)
                        End While
                    End Using
                End Using
            End Using
            Return plans
        End Function

        Public Function GetPlanByID(planID As Integer) As PLPlan
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPlanByID, conn)
                    cmd.Parameters.AddWithValue("@PlanID", planID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapPlan(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function InsertPlan(plan As PLPlan) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertPlan, conn)
                    cmd.Parameters.AddWithValue("@PlanName", plan.PlanName)
                    cmd.Parameters.AddWithValue("@PlanDescription", If(plan.PlanDescription, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@SquareFootage", If(plan.SquareFootage, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdatePlan(plan As PLPlan)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdatePlan, conn)
                    cmd.Parameters.AddWithValue("@PlanID", plan.PlanID)
                    cmd.Parameters.AddWithValue("@PlanName", plan.PlanName)
                    cmd.Parameters.AddWithValue("@PlanDescription", If(plan.PlanDescription, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@SquareFootage", If(plan.SquareFootage, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@IsActive", plan.IsActive)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function AssignPlanToSubdivision(subdivisionID As Integer, planID As Integer) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertSubdivisionPlan, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                    cmd.Parameters.AddWithValue("@PlanID", planID)
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Private Function MapPlan(reader As SqlDataReader) As PLPlan
            Return New PLPlan() With {
                .PlanID = reader.GetInt32(reader.GetOrdinal("PlanID")),
                .PlanName = reader.GetString(reader.GetOrdinal("PlanName")),
                .PlanDescription = If(reader.IsDBNull(reader.GetOrdinal("PlanDescription")), Nothing, reader.GetString(reader.GetOrdinal("PlanDescription"))),
                .SquareFootage = If(reader.IsDBNull(reader.GetOrdinal("SquareFootage")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("SquareFootage")), Integer?)),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            }
        End Function

#End Region

#Region "Elevations"

        Public Function GetElevationsByPlan(planID As Integer) As List(Of PLElevation)
            Dim elevations As New List(Of PLElevation)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectElevationsByPlan, conn)
                    cmd.Parameters.AddWithValue("@PlanID", planID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            elevations.Add(MapElevation(reader))
                        End While
                    End Using
                End Using
            End Using
            Return elevations
        End Function

        Public Function InsertElevation(elevation As PLElevation) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertElevation, conn)
                    cmd.Parameters.AddWithValue("@PlanID", elevation.PlanID)
                    cmd.Parameters.AddWithValue("@ElevationName", elevation.ElevationName)
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdateElevation(elevation As PLElevation)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateElevation, conn)
                    cmd.Parameters.AddWithValue("@ElevationID", elevation.ElevationID)
                    cmd.Parameters.AddWithValue("@ElevationName", elevation.ElevationName)
                    cmd.Parameters.AddWithValue("@IsActive", elevation.IsActive)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Function MapElevation(reader As SqlDataReader) As PLElevation
            Return New PLElevation() With {
                .ElevationID = reader.GetInt32(reader.GetOrdinal("ElevationID")),
                .PlanID = reader.GetInt32(reader.GetOrdinal("PlanID")),
                .ElevationName = reader.GetString(reader.GetOrdinal("ElevationName")),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            }
        End Function

#End Region

#Region "Options"

        Public Function GetOptions() As List(Of PLOption)
            Dim options As New List(Of PLOption)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectOptions, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            options.Add(MapOption(reader))
                        End While
                    End Using
                End Using
            End Using
            Return options
        End Function

        Public Function InsertOption([option] As PLOption) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertOption, conn)
                    cmd.Parameters.AddWithValue("@OptionName", [option].OptionName)
                    cmd.Parameters.AddWithValue("@OptionDescription", If([option].OptionDescription, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdateOption([option] As PLOption)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateOption, conn)
                    cmd.Parameters.AddWithValue("@OptionID", [option].OptionID)
                    cmd.Parameters.AddWithValue("@OptionName", [option].OptionName)
                    cmd.Parameters.AddWithValue("@OptionDescription", If([option].OptionDescription, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@IsActive", [option].IsActive)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function FindOrCreateOption(optionName As String, Optional description As String = Nothing) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.FindOrCreateOption, conn)
                    cmd.Parameters.AddWithValue("@OptionName", optionName)
                    cmd.Parameters.AddWithValue("@OptionDescription", If(description, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Merges a source option into a target option by reassigning all component pricing
        ''' references and deactivating the source option.
        ''' </summary>
        ''' <param name="sourceOptionID">The duplicate option to be merged away</param>
        ''' <param name="targetOptionID">The option to keep</param>
        ''' <param name="modifiedBy">User performing the merge</param>
        ''' <returns>Number of component pricing records that were reassigned</returns>
        Public Function MergeOptions(sourceOptionID As Integer, targetOptionID As Integer, modifiedBy As String) As Integer
            If sourceOptionID = targetOptionID Then
                Throw New ArgumentException("Source and target options cannot be the same.")
            End If

            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.MergeOptions, conn)
                    cmd.Parameters.AddWithValue("@SourceOptionID", sourceOptionID)
                    cmd.Parameters.AddWithValue("@TargetOptionID", targetOptionID)
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(modifiedBy, CObj(DBNull.Value)))
                    conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    Return If(result IsNot Nothing AndAlso result IsNot DBNull.Value, CInt(result), 0)
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Gets the count of component pricing records that reference this option
        ''' </summary>
        Public Function GetOptionUsageCount(optionID As Integer) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.GetOptionUsageCount, conn)
                    cmd.Parameters.AddWithValue("@OptionID", optionID)
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Private Function MapOption(reader As SqlDataReader) As PLOption
            Return New PLOption() With {
                .OptionID = reader.GetInt32(reader.GetOrdinal("OptionID")),
                .OptionName = reader.GetString(reader.GetOrdinal("OptionName")),
                .OptionDescription = If(reader.IsDBNull(reader.GetOrdinal("OptionDescription")), Nothing, reader.GetString(reader.GetOrdinal("OptionDescription"))),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            }
        End Function

#End Region

#Region "Product Types and Material Categories"

        Public Function GetProductTypes() As List(Of PLProductType)
            Dim types As New List(Of PLProductType)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectProductTypes, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            types.Add(New PLProductType() With {
                                .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                .ProductTypeName = reader.GetString(reader.GetOrdinal("ProductTypeName"))
                            })
                        End While
                    End Using
                End Using
            End Using
            Return types
        End Function

        Public Function GetMaterialCategories() As List(Of PLMaterialCategory)
            Dim categories As New List(Of PLMaterialCategory)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectMaterialCategories, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            categories.Add(MapMaterialCategory(reader))
                        End While
                    End Using
                End Using
            End Using
            Return categories
        End Function

        Public Function InsertMaterialCategory(category As PLMaterialCategory) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertMaterialCategory, conn)
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName)
                    cmd.Parameters.AddWithValue("@CategoryCode", If(category.CategoryCode, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@DisplayOrder", category.DisplayOrder)
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdateMaterialCategory(category As PLMaterialCategory)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateMaterialCategory, conn)
                    cmd.Parameters.AddWithValue("@MaterialCategoryID", category.MaterialCategoryID)
                    cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName)
                    cmd.Parameters.AddWithValue("@CategoryCode", If(category.CategoryCode, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@DisplayOrder", category.DisplayOrder)
                    cmd.Parameters.AddWithValue("@IsActive", category.IsActive)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Function MapMaterialCategory(reader As SqlDataReader) As PLMaterialCategory
            Return New PLMaterialCategory() With {
                .MaterialCategoryID = reader.GetInt32(reader.GetOrdinal("MaterialCategoryID")),
                .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                .CategoryCode = If(reader.IsDBNull(reader.GetOrdinal("CategoryCode")), Nothing, reader.GetString(reader.GetOrdinal("CategoryCode"))),
                .DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
            }
        End Function

#End Region



#Region "Price Locks"

        Public Function GetPriceLocksBySubdivision(subdivisionID As Integer) As List(Of PLPriceLock)
            Dim locks As New List(Of PLPriceLock)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPriceLocksBySubdivision, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            locks.Add(MapPriceLock(reader))
                        End While
                    End Using
                End Using
            End Using
            Return locks
        End Function

        Public Function GetPriceLocksFiltered(Optional builderID As Integer? = Nothing,
                                               Optional subdivisionID As Integer? = Nothing,
                                               Optional status As String = Nothing,
                                               Optional startDate As Date? = Nothing,
                                               Optional endDate As Date? = Nothing) As List(Of PLPriceLock)
            Dim locks As New List(Of PLPriceLock)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPriceLocksFiltered, conn)
                    cmd.Parameters.AddWithValue("@BuilderID", If(builderID, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@SubdivisionID", If(subdivisionID, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@Status", If(status, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@StartDate", If(startDate, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@EndDate", If(endDate, CObj(DBNull.Value)))
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            locks.Add(MapPriceLock(reader))
                        End While
                    End Using
                End Using
            End Using
            Return locks
        End Function

        Public Function GetPriceLockByID(priceLockID As Integer) As PLPriceLock
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPriceLockByID, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapPriceLock(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function GetLatestPriceLock(subdivisionID As Integer) As PLPriceLock
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectLatestPriceLock, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapPriceLock(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function GetPreviousPriceLock(subdivisionID As Integer, currentLockDate As Date) As PLPriceLock
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPreviousPriceLock, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                    cmd.Parameters.AddWithValue("@CurrentLockDate", currentLockDate)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New PLPriceLock() With {
                                .PriceLockID = reader.GetInt32(reader.GetOrdinal("PriceLockID")),
                                .SubdivisionID = reader.GetInt32(reader.GetOrdinal("SubdivisionID")),
                                .PriceLockDate = reader.GetDateTime(reader.GetOrdinal("PriceLockDate")),
                                .PriceLockName = If(reader.IsDBNull(reader.GetOrdinal("PriceLockName")), Nothing, reader.GetString(reader.GetOrdinal("PriceLockName"))),
                                .Status = reader.GetString(reader.GetOrdinal("Status"))
                            }
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function InsertPriceLock(priceLock As PLPriceLock) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertPriceLock, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", priceLock.SubdivisionID)
                    cmd.Parameters.AddWithValue("@PriceLockDate", priceLock.PriceLockDate)
                    cmd.Parameters.AddWithValue("@PriceLockName", If(priceLock.PriceLockName, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@BaseMgmtMargin", If(priceLock.BaseMgmtMargin, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@AdjustedMarginBaseModels", If(priceLock.AdjustedMarginBaseModels, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@OptionMargin", If(priceLock.OptionMargin, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@Status", priceLock.Status)
                    cmd.Parameters.AddWithValue("@CreatedBy", If(priceLock.CreatedBy, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdatePriceLock(priceLock As PLPriceLock)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdatePriceLock, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLock.PriceLockID)
                    cmd.Parameters.AddWithValue("@PriceLockDate", priceLock.PriceLockDate)
                    cmd.Parameters.AddWithValue("@PriceLockName", If(priceLock.PriceLockName, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@BaseMgmtMargin", If(priceLock.BaseMgmtMargin, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@AdjustedMarginBaseModels", If(priceLock.AdjustedMarginBaseModels, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@OptionMargin", If(priceLock.OptionMargin, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@Status", priceLock.Status)
                    cmd.Parameters.AddWithValue("@BaselineRLImportID", If(priceLock.BaselineRLImportID, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@CurrentRLImportID", If(priceLock.CurrentRLImportID, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub UpdatePriceLockStatus(priceLockID As Integer, status As String, Optional approvedBy As String = Nothing)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdatePriceLockStatus, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    cmd.Parameters.AddWithValue("@Status", status)
                    cmd.Parameters.AddWithValue("@ApprovedBy", If(approvedBy, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function DeletePriceLock(priceLockID As Integer) As Boolean
            ' First delete child records
            DeleteComponentPricingByLock(priceLockID)
            DeleteMaterialPricingByLock(priceLockID)

            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeletePriceLock, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    conn.Open()
                    Return cmd.ExecuteNonQuery() > 0
                End Using
            End Using
        End Function

        Public Function CopyPriceLock(sourcePriceLockID As Integer, newSubdivisionID As Integer, newDate As Date, createdBy As String) As Integer
            Dim source = GetPriceLockByID(sourcePriceLockID)
            If source Is Nothing Then Return -1

            Using conn As New SqlConnection(_connectionString)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    Try
                        ' 1. Create the new price lock header
                        Dim newLock As New PLPriceLock() With {
                    .SubdivisionID = newSubdivisionID,
                    .PriceLockDate = newDate,
                    .PriceLockName = $"Copy of {source.PriceLockName}",
                    .BaseMgmtMargin = source.BaseMgmtMargin,
                    .AdjustedMarginBaseModels = source.AdjustedMarginBaseModels,
                    .OptionMargin = source.OptionMargin,
                    .BaselineRLImportID = source.BaselineRLImportID,
                    .CurrentRLImportID = source.CurrentRLImportID,
                    .Status = PLPriceLockStatus.Draft,
                    .CreatedBy = createdBy
                }

                        Dim newPriceLockID = InsertPriceLock(newLock)

                        ' 2. Copy component pricing with BaseComponentPricingID remapping
                        CopyComponentPricingWithRemapping(sourcePriceLockID, newPriceLockID, createdBy, conn, tran)

                        ' 3. Copy material pricing with all RL fields
                        CopyMaterialPricingComplete(sourcePriceLockID, newPriceLockID, createdBy, conn, tran)

                        tran.Commit()
                        Return newPriceLockID

                    Catch ex As Exception
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Copy component pricing with proper BaseComponentPricingID remapping
        ''' </summary>
        Private Sub CopyComponentPricingWithRemapping(sourcePriceLockID As Integer, newPriceLockID As Integer,
                                               modifiedBy As String, conn As SqlConnection, tran As SqlTransaction)
            ' Step 1: Copy all records WITHOUT remapping BaseComponentPricingID yet
            Dim sqlCopy = "
        INSERT INTO PL_ComponentPricing (
            PriceLockID, PlanID, ElevationID, OptionID, ProductTypeID,
            Cost, MgmtSellPrice, CalculatedPrice, AppliedMargin,
            FinalPrice, PriceSentToSales, PriceSentToBuilder,
            IsAdder, BaseElevationID, BaseComponentPricingID, PriceNote, MarginSource,
            CreatedDate, ModifiedDate, ModifiedBy
        )
        SELECT 
            @NewPriceLockID, PlanID, ElevationID, OptionID, ProductTypeID,
            Cost, MgmtSellPrice, CalculatedPrice, AppliedMargin,
            FinalPrice, PriceSentToSales, PriceSentToBuilder,
            IsAdder, BaseElevationID, 
            NULL, -- Will be fixed in step 2
            PriceNote, MarginSource,
            GETDATE(), GETDATE(), @ModifiedBy
        FROM PL_ComponentPricing
        WHERE PriceLockID = @SourcePriceLockID
    "

            Using cmd As New SqlCommand(sqlCopy, conn, tran)
                cmd.Parameters.AddWithValue("@SourcePriceLockID", sourcePriceLockID)
                cmd.Parameters.AddWithValue("@NewPriceLockID", newPriceLockID)
                cmd.Parameters.AddWithValue("@ModifiedBy", If(modifiedBy, CObj(DBNull.Value)))
                cmd.ExecuteNonQuery()
            End Using

            ' Step 2: Remap BaseComponentPricingID by matching business keys
            Dim sqlRemap = "
        -- Update BaseComponentPricingID to point to the new copied records
        UPDATE newcp
        SET BaseComponentPricingID = basecp.ComponentPricingID
        FROM PL_ComponentPricing newcp
        INNER JOIN PL_ComponentPricing oldcp 
            ON oldcp.PriceLockID = @SourcePriceLockID
            AND oldcp.PlanID = newcp.PlanID
            AND oldcp.ElevationID = newcp.ElevationID
            AND ISNULL(oldcp.OptionID, 0) = ISNULL(newcp.OptionID, 0)
            AND oldcp.ProductTypeID = newcp.ProductTypeID
        INNER JOIN PL_ComponentPricing oldBase
            ON oldBase.ComponentPricingID = oldcp.BaseComponentPricingID
        INNER JOIN PL_ComponentPricing basecp
            ON basecp.PriceLockID = @NewPriceLockID
            AND basecp.PlanID = oldBase.PlanID
            AND basecp.ElevationID = oldBase.ElevationID
            AND ISNULL(basecp.OptionID, 0) = ISNULL(oldBase.OptionID, 0)
            AND basecp.ProductTypeID = oldBase.ProductTypeID
        WHERE newcp.PriceLockID = @NewPriceLockID
          AND oldcp.BaseComponentPricingID IS NOT NULL
    "

            Using cmd As New SqlCommand(sqlRemap, conn, tran)
                cmd.Parameters.AddWithValue("@SourcePriceLockID", sourcePriceLockID)
                cmd.Parameters.AddWithValue("@NewPriceLockID", newPriceLockID)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        ''' <summary>
        ''' Copy material pricing with all RL calculation fields
        ''' </summary>
        Private Sub CopyMaterialPricingComplete(sourcePriceLockID As Integer, newPriceLockID As Integer,
                                        modifiedBy As String, conn As SqlConnection, tran As SqlTransaction)
            Dim sql = "
        INSERT INTO PL_MaterialPricing (
            PriceLockID, MaterialCategoryID,
            RandomLengthsDate, RandomLengthsPrice,
            CalculatedPrice, PriceSentToSales, PriceSentToBuilder,
            PctChangeFromPrevious, PriceNote,
            BaselineRLPrice, CurrentRLPrice, OriginalSellPrice, RLPercentChange,
            CreatedDate, ModifiedDate, ModifiedBy
        )
        SELECT 
            @NewPriceLockID, MaterialCategoryID,
            RandomLengthsDate, RandomLengthsPrice,
            CalculatedPrice, PriceSentToSales, PriceSentToBuilder,
            PctChangeFromPrevious, PriceNote,
            BaselineRLPrice, CurrentRLPrice, OriginalSellPrice, RLPercentChange,
            GETDATE(), GETDATE(), @ModifiedBy
        FROM PL_MaterialPricing
        WHERE PriceLockID = @SourcePriceLockID
    "

            Using cmd As New SqlCommand(sql, conn, tran)
                cmd.Parameters.AddWithValue("@SourcePriceLockID", sourcePriceLockID)
                cmd.Parameters.AddWithValue("@NewPriceLockID", newPriceLockID)
                cmd.Parameters.AddWithValue("@ModifiedBy", If(modifiedBy, CObj(DBNull.Value)))
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Function MapPriceLock(reader As SqlDataReader) As PLPriceLock
            Dim pl As New PLPriceLock() With {
        .PriceLockID = reader.GetInt32(reader.GetOrdinal("PriceLockID")),
        .SubdivisionID = reader.GetInt32(reader.GetOrdinal("SubdivisionID")),
        .PriceLockDate = reader.GetDateTime(reader.GetOrdinal("PriceLockDate")),
        .Status = reader.GetString(reader.GetOrdinal("Status")),
        .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
        .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
        .SubdivisionName = reader.GetString(reader.GetOrdinal("SubdivisionName")),
        .BuilderID = reader.GetInt32(reader.GetOrdinal("BuilderID")),
        .BuilderName = reader.GetString(reader.GetOrdinal("BuilderName"))
    }

            ' Nullable fields
            If Not reader.IsDBNull(reader.GetOrdinal("PriceLockName")) Then pl.PriceLockName = reader.GetString(reader.GetOrdinal("PriceLockName"))
            If Not reader.IsDBNull(reader.GetOrdinal("SubdivisionCode")) Then pl.SubdivisionCode = reader.GetString(reader.GetOrdinal("SubdivisionCode"))
            If Not reader.IsDBNull(reader.GetOrdinal("BaseMgmtMargin")) Then pl.BaseMgmtMargin = reader.GetDecimal(reader.GetOrdinal("BaseMgmtMargin"))
            If Not reader.IsDBNull(reader.GetOrdinal("AdjustedMarginBaseModels")) Then pl.AdjustedMarginBaseModels = reader.GetDecimal(reader.GetOrdinal("AdjustedMarginBaseModels"))
            If Not reader.IsDBNull(reader.GetOrdinal("OptionMargin")) Then pl.OptionMargin = reader.GetDecimal(reader.GetOrdinal("OptionMargin"))
            If Not reader.IsDBNull(reader.GetOrdinal("CreatedBy")) Then pl.CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy"))
            If Not reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) Then pl.ApprovedBy = reader.GetString(reader.GetOrdinal("ApprovedBy"))
            If Not reader.IsDBNull(reader.GetOrdinal("ApprovedDate")) Then pl.ApprovedDate = reader.GetDateTime(reader.GetOrdinal("ApprovedDate"))
            If Not reader.IsDBNull(reader.GetOrdinal("SentToBuilderDate")) Then pl.SentToBuilderDate = reader.GetDateTime(reader.GetOrdinal("SentToBuilderDate"))

            ' *** NEW: Read RL Import IDs ***
            If HasColumn(reader, "BaselineRLImportID") AndAlso Not reader.IsDBNull(reader.GetOrdinal("BaselineRLImportID")) Then
                pl.BaselineRLImportID = reader.GetInt32(reader.GetOrdinal("BaselineRLImportID"))
            End If
            If HasColumn(reader, "CurrentRLImportID") AndAlso Not reader.IsDBNull(reader.GetOrdinal("CurrentRLImportID")) Then
                pl.CurrentRLImportID = reader.GetInt32(reader.GetOrdinal("CurrentRLImportID"))
            End If

            Return pl
        End Function

#End Region

#Region "Component Pricing"

        ' =====================================================
        ' MODIFY GetComponentPricingByLock function
        ' Change the query constant reference (around line 765)
        ' =====================================================

        Public Function GetComponentPricingByLock(priceLockID As Integer) As List(Of PLComponentPricing)
            Dim pricing As New List(Of PLComponentPricing)()
            Using conn As New SqlConnection(_connectionString)
                ' CHANGE THIS LINE: Use the new query with previous price lock data
                Using cmd As New SqlCommand(PriceLockQueries.SelectComponentPricingByLockWithPrevious, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            pricing.Add(MapComponentPricing(reader))
                        End While
                    End Using
                End Using
            End Using
            Return pricing
        End Function

        Public Function GetComponentPricingByID(componentPricingID As Integer) As PLComponentPricing
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectComponentPricingByID, conn)
                    cmd.Parameters.AddWithValue("@ComponentPricingID", componentPricingID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapComponentPricing(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function InsertComponentPricing(pricing As PLComponentPricing) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertComponentPricing, conn)
                    AddComponentPricingParameters(cmd, pricing)
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdateComponentPricing(pricing As PLComponentPricing)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateComponentPricing, conn)
                    cmd.Parameters.AddWithValue("@ComponentPricingID", pricing.ComponentPricingID)
                    AddComponentPricingParameters(cmd, pricing)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub UpdateComponentPricingPrices(componentPricingID As Integer, finalPrice As Decimal, priceSentToBuilder As Decimal, priceNote As String, modifiedBy As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateComponentPricingPrices, conn)
                    cmd.Parameters.AddWithValue("@ComponentPricingID", componentPricingID)
                    cmd.Parameters.AddWithValue("@FinalPrice", finalPrice)
                    cmd.Parameters.AddWithValue("@PriceSentToBuilder", priceSentToBuilder)
                    cmd.Parameters.AddWithValue("@PriceNote", If(priceNote, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(modifiedBy, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub DeleteComponentPricing(componentPricingID As Integer)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeleteComponentPricing, conn)
                    cmd.Parameters.AddWithValue("@ComponentPricingID", componentPricingID)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub DeleteComponentPricingByLock(priceLockID As Integer)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeleteComponentPricingByLock, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub CopyComponentPricing(sourcePriceLockID As Integer, newPriceLockID As Integer, modifiedBy As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.CopyComponentPricing, conn)
                    cmd.Parameters.AddWithValue("@SourcePriceLockID", sourcePriceLockID)
                    cmd.Parameters.AddWithValue("@NewPriceLockID", newPriceLockID)
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(modifiedBy, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' (Keep all existing code, only showing the methods that need changes)

        Private Sub AddComponentPricingParameters(cmd As SqlCommand, pricing As PLComponentPricing)
            cmd.Parameters.AddWithValue("@PriceLockID", pricing.PriceLockID)
            cmd.Parameters.AddWithValue("@PlanID", pricing.PlanID)
            cmd.Parameters.AddWithValue("@ElevationID", pricing.ElevationID)
            cmd.Parameters.AddWithValue("@OptionID", If(pricing.OptionID, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@ProductTypeID", pricing.ProductTypeID)
            cmd.Parameters.AddWithValue("@Cost", If(pricing.Cost, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@MgmtSellPrice", If(pricing.MgmtSellPrice, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@CalculatedPrice", If(pricing.CalculatedPrice, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@AppliedMargin", If(pricing.AppliedMargin, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@FinalPrice", If(pricing.FinalPrice, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@PriceSentToSales", If(pricing.PriceSentToSales, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@PriceSentToBuilder", If(pricing.PriceSentToBuilder, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@IsAdder", pricing.IsAdder)
            cmd.Parameters.AddWithValue("@BaseElevationID", If(pricing.BaseElevationID, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@BaseComponentPricingID", If(pricing.BaseComponentPricingID, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@PriceNote", If(pricing.PriceNote, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@MarginSource", If(pricing.MarginSource, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@ModifiedBy", If(pricing.ModifiedBy, CObj(DBNull.Value)))
        End Sub

        Private Function MapComponentPricing(reader As SqlDataReader) As PLComponentPricing
            Dim cp As New PLComponentPricing() With {
                .ComponentPricingID = reader.GetInt32(reader.GetOrdinal("ComponentPricingID")),
                .PriceLockID = reader.GetInt32(reader.GetOrdinal("PriceLockID")),
                .PlanID = reader.GetInt32(reader.GetOrdinal("PlanID")),
                .ElevationID = reader.GetInt32(reader.GetOrdinal("ElevationID")),
                .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                .IsAdder = reader.GetBoolean(reader.GetOrdinal("IsAdder")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                .PlanName = reader.GetString(reader.GetOrdinal("PlanName")),
                .ElevationName = reader.GetString(reader.GetOrdinal("ElevationName")),
                .ProductTypeName = reader.GetString(reader.GetOrdinal("ProductTypeName"))
            }

            If Not reader.IsDBNull(reader.GetOrdinal("OptionID")) Then cp.OptionID = reader.GetInt32(reader.GetOrdinal("OptionID"))
            If Not reader.IsDBNull(reader.GetOrdinal("Cost")) Then
                cp.Cost = reader.GetDecimal(reader.GetOrdinal("Cost"))
                cp.TrueCost = cp.Cost ' For compatibility with import code
            End If
            If Not reader.IsDBNull(reader.GetOrdinal("MgmtSellPrice")) Then cp.MgmtSellPrice = reader.GetDecimal(reader.GetOrdinal("MgmtSellPrice"))
            If Not reader.IsDBNull(reader.GetOrdinal("CalculatedPrice")) Then cp.CalculatedPrice = reader.GetDecimal(reader.GetOrdinal("CalculatedPrice"))
            If Not reader.IsDBNull(reader.GetOrdinal("AppliedMargin")) Then cp.AppliedMargin = reader.GetDecimal(reader.GetOrdinal("AppliedMargin"))
            If Not reader.IsDBNull(reader.GetOrdinal("FinalPrice")) Then cp.FinalPrice = reader.GetDecimal(reader.GetOrdinal("FinalPrice"))
            If Not reader.IsDBNull(reader.GetOrdinal("PriceSentToSales")) Then cp.PriceSentToSales = reader.GetDecimal(reader.GetOrdinal("PriceSentToSales"))
            If Not reader.IsDBNull(reader.GetOrdinal("PriceSentToBuilder")) Then cp.PriceSentToBuilder = reader.GetDecimal(reader.GetOrdinal("PriceSentToBuilder"))
            If Not reader.IsDBNull(reader.GetOrdinal("BaseElevationID")) Then cp.BaseElevationID = reader.GetInt32(reader.GetOrdinal("BaseElevationID"))
            If Not reader.IsDBNull(reader.GetOrdinal("BaseComponentPricingID")) Then cp.BaseComponentPricingID = reader.GetInt32(reader.GetOrdinal("BaseComponentPricingID"))
            If Not reader.IsDBNull(reader.GetOrdinal("PriceNote")) Then cp.PriceNote = reader.GetString(reader.GetOrdinal("PriceNote"))
            If Not reader.IsDBNull(reader.GetOrdinal("MarginSource")) Then cp.MarginSource = reader.GetString(reader.GetOrdinal("MarginSource"))
            If Not reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) Then cp.ModifiedBy = reader.GetString(reader.GetOrdinal("ModifiedBy"))
            If Not reader.IsDBNull(reader.GetOrdinal("OptionName")) Then cp.OptionName = reader.GetString(reader.GetOrdinal("OptionName"))

            ' NEW: Load base component info for adders
            If HasColumn(reader, "BaseComponentCost") AndAlso Not reader.IsDBNull(reader.GetOrdinal("BaseComponentCost")) Then
                cp.BaseComponentCost = reader.GetDecimal(reader.GetOrdinal("BaseComponentCost"))
            End If
            If HasColumn(reader, "BaseComponentDescription") AndAlso Not reader.IsDBNull(reader.GetOrdinal("BaseComponentDescription")) Then
                cp.BaseComponentDescription = reader.GetString(reader.GetOrdinal("BaseComponentDescription"))
            End If

            ' Set a display-friendly margin name if MarginSource is populated
            If Not String.IsNullOrEmpty(cp.MarginSource) Then
                cp.MarginName = GetFriendlyMarginName(cp.MarginSource)
            Else
                cp.MarginName = String.Empty
            End If
            ' =====================================================
            ' ADD THESE LINES to MapComponentPricing function
            ' Insert BEFORE the "Return cp" statement (around line 920)
            ' =====================================================

            ' Previous Price Lock Comparison Fields
            If HasColumn(reader, "PreviousPriceSentToSales") AndAlso Not reader.IsDBNull(reader.GetOrdinal("PreviousPriceSentToSales")) Then
                cp.PreviousPriceSentToSales = reader.GetDecimal(reader.GetOrdinal("PreviousPriceSentToSales"))
            End If
            If HasColumn(reader, "PreviousPriceSentToBuilder") AndAlso Not reader.IsDBNull(reader.GetOrdinal("PreviousPriceSentToBuilder")) Then
                cp.PreviousPriceSentToBuilder = reader.GetDecimal(reader.GetOrdinal("PreviousPriceSentToBuilder"))
            End If
            If HasColumn(reader, "PctChangeSentToSales") AndAlso Not reader.IsDBNull(reader.GetOrdinal("PctChangeSentToSales")) Then
                cp.PctChangeSentToSales = reader.GetDecimal(reader.GetOrdinal("PctChangeSentToSales"))
            End If
            If HasColumn(reader, "PctChangeSentToBuilder") AndAlso Not reader.IsDBNull(reader.GetOrdinal("PctChangeSentToBuilder")) Then
                cp.PctChangeSentToBuilder = reader.GetDecimal(reader.GetOrdinal("PctChangeSentToBuilder"))
            End If

            Return cp
        End Function

        ''' <summary>Helper to check if a column exists in the reader</summary>
        Private Function HasColumn(reader As SqlDataReader, columnName As String) As Boolean
            For i As Integer = 0 To reader.FieldCount - 1
                If reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary>
        ''' Converts MarginSource code to friendly display name
        ''' </summary>
        Private Function GetFriendlyMarginName(marginSource As String) As String
            Select Case marginSource?.ToUpper()
                Case "0"
                    Return "Adjusted"
                Case "1"
                    Return "Option"

                Case "CUSTOM"
                    Return "Custom"
                Case Else
                    Return marginSource ' Return as-is if unknown
            End Select
        End Function

        ''' <summary>
        ''' Updates all component pricing records when margin changes
        ''' </summary>
        Public Sub RecalculatePricesForMarginChange(priceLockID As Integer, marginType As String, newMargin As Decimal, userName As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateComponentPricingForMarginChange, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    cmd.Parameters.AddWithValue("@MarginType", marginType) ' "Adjusted" or "Option"
                    cmd.Parameters.AddWithValue("@NewMargin", newMargin)
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(userName, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Updates the MarginSource for a specific component pricing record
        ''' </summary>
        Public Sub UpdateComponentMarginSource(componentPricingID As Integer, marginSource As String, userName As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateComponentMarginSource, conn)
                    cmd.Parameters.AddWithValue("@ComponentPricingID", componentPricingID)
                    cmd.Parameters.AddWithValue("@MarginSource", marginSource)
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(userName, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Gets potential reference units for an adder (same plan, same product type, base elevations only)
        ''' </summary>
        Public Function GetPotentialReferenceUnits(priceLockID As Integer, planID As Integer, productTypeID As Integer, Optional excludeComponentPricingID As Integer = 0) As List(Of PLReferenceUnit)
            Dim units As New List(Of PLReferenceUnit)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPotentialReferenceUnits, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    cmd.Parameters.AddWithValue("@PlanID", planID)
                    cmd.Parameters.AddWithValue("@ProductTypeID", productTypeID)
                    cmd.Parameters.AddWithValue("@ExcludeComponentPricingID", If(excludeComponentPricingID = 0, CObj(DBNull.Value), excludeComponentPricingID))
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            units.Add(New PLReferenceUnit() With {
                                .ComponentPricingID = reader.GetInt32(reader.GetOrdinal("ComponentPricingID")),
                                .PlanID = reader.GetInt32(reader.GetOrdinal("PlanID")),
                                .ElevationID = reader.GetInt32(reader.GetOrdinal("ElevationID")),
                                .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                .Cost = If(reader.IsDBNull(reader.GetOrdinal("Cost")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("Cost")), Decimal?)),
                                .FinalPrice = If(reader.IsDBNull(reader.GetOrdinal("FinalPrice")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("FinalPrice")), Decimal?)),
                                .PlanName = reader.GetString(reader.GetOrdinal("PlanName")),
                                .ElevationName = reader.GetString(reader.GetOrdinal("ElevationName")),
                                .ProductTypeName = reader.GetString(reader.GetOrdinal("ProductTypeName")),
                                .Description = reader.GetString(reader.GetOrdinal("Description"))
                            })
                        End While
                    End Using
                End Using
            End Using
            Return units
        End Function

        ''' <summary>
        ''' Gets the cost of a base component for adder calculation
        ''' </summary>
        Public Function GetBaseComponentCost(baseComponentPricingID As Integer) As Decimal?
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectBaseComponentCost, conn)
                    cmd.Parameters.AddWithValue("@BaseComponentPricingID", baseComponentPricingID)
                    conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then Return CDec(result)
                End Using
            End Using
            Return Nothing
        End Function


#End Region

#Region "Material Pricing"

        Public Function GetMaterialPricingByLock(priceLockID As Integer) As List(Of PLMaterialPricing)
            Dim pricing As New List(Of PLMaterialPricing)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectMaterialPricingByLock, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            pricing.Add(MapMaterialPricing(reader))
                        End While
                    End Using
                End Using
            End Using
            Return pricing
        End Function

        Public Function GetMaterialPricingByID(materialPricingID As Integer) As PLMaterialPricing
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectMaterialPricingByID, conn)
                    cmd.Parameters.AddWithValue("@MaterialPricingID", materialPricingID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapMaterialPricing(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        Public Function InsertMaterialPricing(pricing As PLMaterialPricing) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertMaterialPricing, conn)
                    AddMaterialPricingParameters(cmd, pricing)
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub UpdateMaterialPricing(pricing As PLMaterialPricing)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateMaterialPricing, conn)
                    cmd.Parameters.AddWithValue("@MaterialPricingID", pricing.MaterialPricingID)
                    AddMaterialPricingParameters(cmd, pricing)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub UpdateMaterialPricingPrices(materialPricingID As Integer, calculatedPrice As Decimal, priceSentToBuilder As Decimal, priceNote As String, modifiedBy As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateMaterialPricingPrices, conn)
                    cmd.Parameters.AddWithValue("@MaterialPricingID", materialPricingID)
                    cmd.Parameters.AddWithValue("@CalculatedPrice", calculatedPrice)
                    cmd.Parameters.AddWithValue("@PriceSentToBuilder", priceSentToBuilder)
                    cmd.Parameters.AddWithValue("@PriceNote", If(priceNote, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(modifiedBy, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub DeleteMaterialPricing(materialPricingID As Integer)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeleteMaterialPricing, conn)
                    cmd.Parameters.AddWithValue("@MaterialPricingID", materialPricingID)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub DeleteMaterialPricingByLock(priceLockID As Integer)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeleteMaterialPricingByLock, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub CopyMaterialPricing(sourcePriceLockID As Integer, newPriceLockID As Integer, modifiedBy As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.CopyMaterialPricing, conn)
                    cmd.Parameters.AddWithValue("@SourcePriceLockID", sourcePriceLockID)
                    cmd.Parameters.AddWithValue("@NewPriceLockID", newPriceLockID)
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(modifiedBy, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function GetPreviousMaterialPrice(subdivisionID As Integer, materialCategoryID As Integer, currentLockDate As Date) As Decimal?
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPreviousMaterialPrice, conn)
                    cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                    cmd.Parameters.AddWithValue("@MaterialCategoryID", materialCategoryID)
                    cmd.Parameters.AddWithValue("@CurrentLockDate", currentLockDate)
                    conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then Return CDec(result)
                End Using
            End Using
            Return Nothing
        End Function

        Private Sub AddMaterialPricingParameters(cmd As SqlCommand, pricing As PLMaterialPricing)
            cmd.Parameters.AddWithValue("@PriceLockID", pricing.PriceLockID)
            cmd.Parameters.AddWithValue("@MaterialCategoryID", pricing.MaterialCategoryID)
            cmd.Parameters.AddWithValue("@RandomLengthsDate", If(pricing.RandomLengthsDate, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@RandomLengthsPrice", If(pricing.RandomLengthsPrice, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@CalculatedPrice", If(pricing.CalculatedPrice, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@PriceSentToSales", If(pricing.PriceSentToSales, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@PriceSentToBuilder", If(pricing.PriceSentToBuilder, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@PctChangeFromPrevious", If(pricing.PctChangeFromPrevious, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@PriceNote", If(pricing.PriceNote, CObj(DBNull.Value)))
            cmd.Parameters.AddWithValue("@ModifiedBy", If(pricing.ModifiedBy, CObj(DBNull.Value)))
        End Sub

        Private Function MapMaterialPricing(reader As SqlDataReader) As PLMaterialPricing
            Dim mp As New PLMaterialPricing() With {
                .MaterialPricingID = reader.GetInt32(reader.GetOrdinal("MaterialPricingID")),
                .PriceLockID = reader.GetInt32(reader.GetOrdinal("PriceLockID")),
                .MaterialCategoryID = reader.GetInt32(reader.GetOrdinal("MaterialCategoryID")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate")),
                .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                .DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder"))
            }

            If Not reader.IsDBNull(reader.GetOrdinal("RandomLengthsDate")) Then mp.RandomLengthsDate = reader.GetDateTime(reader.GetOrdinal("RandomLengthsDate"))
            If Not reader.IsDBNull(reader.GetOrdinal("RandomLengthsPrice")) Then mp.RandomLengthsPrice = reader.GetDecimal(reader.GetOrdinal("RandomLengthsPrice"))
            If Not reader.IsDBNull(reader.GetOrdinal("CalculatedPrice")) Then mp.CalculatedPrice = reader.GetDecimal(reader.GetOrdinal("CalculatedPrice"))
            If Not reader.IsDBNull(reader.GetOrdinal("PriceSentToSales")) Then mp.PriceSentToSales = reader.GetDecimal(reader.GetOrdinal("PriceSentToSales"))
            If Not reader.IsDBNull(reader.GetOrdinal("PriceSentToBuilder")) Then mp.PriceSentToBuilder = reader.GetDecimal(reader.GetOrdinal("PriceSentToBuilder"))
            If Not reader.IsDBNull(reader.GetOrdinal("PctChangeFromPrevious")) Then mp.PctChangeFromPrevious = reader.GetDecimal(reader.GetOrdinal("PctChangeFromPrevious"))
            If Not reader.IsDBNull(reader.GetOrdinal("PriceNote")) Then mp.PriceNote = reader.GetString(reader.GetOrdinal("PriceNote"))
            If Not reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) Then mp.ModifiedBy = reader.GetString(reader.GetOrdinal("ModifiedBy"))
            If Not reader.IsDBNull(reader.GetOrdinal("CategoryCode")) Then mp.CategoryCode = reader.GetString(reader.GetOrdinal("CategoryCode"))

            Return mp
        End Function

#End Region

#Region "Comparison Reports"

        Public Function CompareComponentPricing(currentPriceLockID As Integer, previousPriceLockID As Integer) As List(Of PLComponentPriceComparison)
            Dim comparisons As New List(Of PLComponentPriceComparison)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.CompareComponentPricing, conn)
                    cmd.Parameters.AddWithValue("@CurrentPriceLockID", currentPriceLockID)
                    cmd.Parameters.AddWithValue("@PreviousPriceLockID", previousPriceLockID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim comp As New PLComponentPriceComparison() With {
                                .PlanName = reader.GetString(reader.GetOrdinal("PlanName")),
                                .ElevationName = reader.GetString(reader.GetOrdinal("ElevationName")),
                                .ProductTypeName = reader.GetString(reader.GetOrdinal("ProductTypeName"))
                            }
                            If Not reader.IsDBNull(reader.GetOrdinal("OptionName")) Then comp.OptionName = reader.GetString(reader.GetOrdinal("OptionName"))
                            If Not reader.IsDBNull(reader.GetOrdinal("CurrentFinalPrice")) Then comp.CurrentFinalPrice = reader.GetDecimal(reader.GetOrdinal("CurrentFinalPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("CurrentBuilderPrice")) Then comp.CurrentBuilderPrice = reader.GetDecimal(reader.GetOrdinal("CurrentBuilderPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PreviousFinalPrice")) Then comp.PreviousFinalPrice = reader.GetDecimal(reader.GetOrdinal("PreviousFinalPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PreviousBuilderPrice")) Then comp.PreviousBuilderPrice = reader.GetDecimal(reader.GetOrdinal("PreviousBuilderPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("FinalPriceDiff")) Then comp.FinalPriceDiff = reader.GetDecimal(reader.GetOrdinal("FinalPriceDiff"))
                            If Not reader.IsDBNull(reader.GetOrdinal("FinalPricePctChange")) Then comp.FinalPricePctChange = reader.GetDecimal(reader.GetOrdinal("FinalPricePctChange"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PriceNote")) Then comp.PriceNote = reader.GetString(reader.GetOrdinal("PriceNote"))
                            comparisons.Add(comp)
                        End While
                    End Using
                End Using
            End Using
            Return comparisons
        End Function

        Public Function CompareMaterialPricing(currentPriceLockID As Integer, previousPriceLockID As Integer) As List(Of PLMaterialPriceComparison)
            Dim comparisons As New List(Of PLMaterialPriceComparison)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.CompareMaterialPricing, conn)
                    cmd.Parameters.AddWithValue("@CurrentPriceLockID", currentPriceLockID)
                    cmd.Parameters.AddWithValue("@PreviousPriceLockID", previousPriceLockID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim comp As New PLMaterialPriceComparison() With {
                                .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                            }
                            If Not reader.IsDBNull(reader.GetOrdinal("CategoryCode")) Then comp.CategoryCode = reader.GetString(reader.GetOrdinal("CategoryCode"))
                            If Not reader.IsDBNull(reader.GetOrdinal("CurrentPrice")) Then comp.CurrentPrice = reader.GetDecimal(reader.GetOrdinal("CurrentPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("CurrentBuilderPrice")) Then comp.CurrentBuilderPrice = reader.GetDecimal(reader.GetOrdinal("CurrentBuilderPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PreviousPrice")) Then comp.PreviousPrice = reader.GetDecimal(reader.GetOrdinal("PreviousPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PreviousBuilderPrice")) Then comp.PreviousBuilderPrice = reader.GetDecimal(reader.GetOrdinal("PreviousBuilderPrice"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PriceDiff")) Then comp.PriceDiff = reader.GetDecimal(reader.GetOrdinal("PriceDiff"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PctChangeFromPrevious")) Then comp.PctChangeFromPrevious = reader.GetDecimal(reader.GetOrdinal("PctChangeFromPrevious"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PriceNote")) Then comp.PriceNote = reader.GetString(reader.GetOrdinal("PriceNote"))
                            comparisons.Add(comp)
                        End While
                    End Using
                End Using
            End Using
            Return comparisons
        End Function

        Public Function GetSubdivisionsNeedingRenewal() As List(Of PLSubdivisionRenewalStatus)
            Dim results As New List(Of PLSubdivisionRenewalStatus)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectSubdivisionsNeedingRenewal, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim status As New PLSubdivisionRenewalStatus() With {
                                .SubdivisionID = reader.GetInt32(reader.GetOrdinal("SubdivisionID")),
                                .SubdivisionName = reader.GetString(reader.GetOrdinal("SubdivisionName")),
                                .BuilderID = reader.GetInt32(reader.GetOrdinal("BuilderID")),
                                .BuilderName = reader.GetString(reader.GetOrdinal("BuilderName"))
                            }
                            If Not reader.IsDBNull(reader.GetOrdinal("SubdivisionCode")) Then status.SubdivisionCode = reader.GetString(reader.GetOrdinal("SubdivisionCode"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PriceLockID")) Then status.PriceLockID = reader.GetInt32(reader.GetOrdinal("PriceLockID"))
                            If Not reader.IsDBNull(reader.GetOrdinal("PriceLockDate")) Then status.PriceLockDate = reader.GetDateTime(reader.GetOrdinal("PriceLockDate"))
                            If Not reader.IsDBNull(reader.GetOrdinal("DaysSinceLock")) Then status.DaysSinceLock = reader.GetInt32(reader.GetOrdinal("DaysSinceLock"))
                            results.Add(status)
                        End While
                    End Using
                End Using
            End Using
            Return results
        End Function

#End Region

#Region "Audit History"

        Public Sub LogPriceChange(tableName As String, recordID As Integer, fieldName As String, oldValue As String, newValue As String, changedBy As String, Optional changeReason As String = Nothing)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertPriceChangeHistory, conn)
                    cmd.Parameters.AddWithValue("@TableName", tableName)
                    cmd.Parameters.AddWithValue("@RecordID", recordID)
                    cmd.Parameters.AddWithValue("@FieldName", fieldName)
                    cmd.Parameters.AddWithValue("@OldValue", If(oldValue, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@NewValue", If(newValue, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@ChangedBy", If(changedBy, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@ChangeReason", If(changeReason, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function GetPriceChangeHistory(tableName As String, recordID As Integer) As List(Of PLPriceChangeHistory)
            Dim history As New List(Of PLPriceChangeHistory)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectPriceChangeHistory, conn)
                    cmd.Parameters.AddWithValue("@TableName", tableName)
                    cmd.Parameters.AddWithValue("@RecordID", recordID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            history.Add(New PLPriceChangeHistory() With {
                                .HistoryID = reader.GetInt32(reader.GetOrdinal("HistoryID")),
                                .TableName = reader.GetString(reader.GetOrdinal("TableName")),
                                .RecordID = reader.GetInt32(reader.GetOrdinal("RecordID")),
                                .FieldName = reader.GetString(reader.GetOrdinal("FieldName")),
                                .OldValue = If(reader.IsDBNull(reader.GetOrdinal("OldValue")), Nothing, reader.GetString(reader.GetOrdinal("OldValue"))),
                                .NewValue = If(reader.IsDBNull(reader.GetOrdinal("NewValue")), Nothing, reader.GetString(reader.GetOrdinal("NewValue"))),
                                .ChangedBy = If(reader.IsDBNull(reader.GetOrdinal("ChangedBy")), Nothing, reader.GetString(reader.GetOrdinal("ChangedBy"))),
                                .ChangedDate = reader.GetDateTime(reader.GetOrdinal("ChangedDate")),
                                .ChangeReason = If(reader.IsDBNull(reader.GetOrdinal("ChangeReason")), Nothing, reader.GetString(reader.GetOrdinal("ChangeReason")))
                            })
                        End While
                    End Using
                End Using
            End Using
            Return history
        End Function

#End Region

        ' =====================================================
        ' PriceLockDataAccess_RandomLengths_Addition.vb
        ' 
        ' ADD THESE REGIONS TO YOUR EXISTING PriceLockDataAccess.vb
        ' Insert before the "End Class" at the bottom of the file
        ' (around line 1249)
        ' =====================================================

#Region "Random Lengths Import"

        ''' <summary>Get all active Random Lengths imports</summary>
        Public Function GetRandomLengthsImports() As List(Of PLRandomLengthsImport)
            Dim rlImports As New List(Of PLRandomLengthsImport)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRandomLengthsImports, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            rlImports.Add(MapRandomLengthsImport(reader))
                        End While
                    End Using
                End Using
            End Using
            Return rlImports
        End Function

        ''' <summary>Get all Random Lengths imports including inactive</summary>
        Public Function GetAllRandomLengthsImports() As List(Of PLRandomLengthsImport)
            Dim rlImports As New List(Of PLRandomLengthsImport)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectAllRandomLengthsImports, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            rlImports.Add(MapRandomLengthsImport(reader))
                        End While
                    End Using
                End Using
            End Using
            Return rlImports
        End Function

        ''' <summary>Get Random Lengths import by ID</summary>
        Public Function GetRandomLengthsImportByID(randomLengthsImportID As Integer) As PLRandomLengthsImport
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRandomLengthsImportByID, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", randomLengthsImportID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapRandomLengthsImport(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        ''' <summary>Get Random Lengths imports within date range</summary>
        Public Function GetRandomLengthsImportsByDateRange(startDate As Date, endDate As Date) As List(Of PLRandomLengthsImport)
            Dim rlImports As New List(Of PLRandomLengthsImport)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRandomLengthsImportsByDateRange, conn)
                    cmd.Parameters.AddWithValue("@StartDate", startDate)
                    cmd.Parameters.AddWithValue("@EndDate", endDate)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            rlImports.Add(MapRandomLengthsImport(reader))
                        End While
                    End Using
                End Using
            End Using
            Return rlImports
        End Function

        ''' <summary>Insert new Random Lengths import</summary>
        Public Function InsertRandomLengthsImport(rlImport As PLRandomLengthsImport) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertRandomLengthsImport, conn)
                    cmd.Parameters.AddWithValue("@ReportDate", rlImport.ReportDate)
                    cmd.Parameters.AddWithValue("@ReportName", If(rlImport.ReportName, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@SourceFileName", If(rlImport.SourceFileName, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@ImportMethod", rlImport.ImportMethod)
                    cmd.Parameters.AddWithValue("@ImportedBy", rlImport.ImportedBy)
                    cmd.Parameters.AddWithValue("@Notes", If(rlImport.Notes, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(rlImport.ModifiedBy, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        ''' <summary>Update Random Lengths import</summary>
        Public Sub UpdateRandomLengthsImport(rlImport As PLRandomLengthsImport)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateRandomLengthsImport, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", rlImport.RandomLengthsImportID)
                    cmd.Parameters.AddWithValue("@ReportDate", rlImport.ReportDate)
                    cmd.Parameters.AddWithValue("@ReportName", If(rlImport.ReportName, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@SourceFileName", If(rlImport.SourceFileName, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@Notes", If(rlImport.Notes, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@ModifiedBy", If(rlImport.ModifiedBy, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Soft delete Random Lengths import</summary>
        Public Sub DeactivateRandomLengthsImport(randomLengthsImportID As Integer, modifiedBy As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeactivateRandomLengthsImport, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", randomLengthsImportID)
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Check if import exists for date</summary>
        Public Function CheckRandomLengthsImportExists(reportDate As Date, Optional excludeID As Integer? = Nothing) As Boolean
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.CheckRandomLengthsImportExists, conn)
                    cmd.Parameters.AddWithValue("@ReportDate", reportDate)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", If(excludeID, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

        Private Function MapRandomLengthsImport(reader As SqlDataReader) As PLRandomLengthsImport
            Dim rli As New PLRandomLengthsImport() With {
                .RandomLengthsImportID = reader.GetInt32(reader.GetOrdinal("RandomLengthsImportID")),
                .ReportDate = reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                .ImportMethod = reader.GetString(reader.GetOrdinal("ImportMethod")),
                .ImportedBy = reader.GetString(reader.GetOrdinal("ImportedBy")),
                .ImportedDate = reader.GetDateTime(reader.GetOrdinal("ImportedDate")),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                .ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            }

            ' Nullable fields
            If Not reader.IsDBNull(reader.GetOrdinal("ReportName")) Then rli.ReportName = reader.GetString(reader.GetOrdinal("ReportName"))
            If Not reader.IsDBNull(reader.GetOrdinal("SourceFileName")) Then rli.SourceFileName = reader.GetString(reader.GetOrdinal("SourceFileName"))
            If Not reader.IsDBNull(reader.GetOrdinal("Notes")) Then rli.Notes = reader.GetString(reader.GetOrdinal("Notes"))
            If Not reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) Then rli.ModifiedBy = reader.GetString(reader.GetOrdinal("ModifiedBy"))

            ' Price count from aggregate
            If HasColumn(reader, "PriceCount") AndAlso Not reader.IsDBNull(reader.GetOrdinal("PriceCount")) Then
                rli.PriceCount = reader.GetInt32(reader.GetOrdinal("PriceCount"))
            End If

            Return rli
        End Function

#End Region

#Region "Random Lengths Pricing Type"

        ''' <summary>Get all active RL pricing types</summary>
        Public Function GetRLPricingTypes() As List(Of PLRandomLengthsPricingType)
            Dim types As New List(Of PLRandomLengthsPricingType)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRLPricingTypes, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            types.Add(MapRLPricingType(reader))
                        End While
                    End Using
                End Using
            End Using
            Return types
        End Function

        ''' <summary>Get RL pricing types by category group</summary>
        Public Function GetRLPricingTypesByGroup(categoryGroup As String) As List(Of PLRandomLengthsPricingType)
            Dim types As New List(Of PLRandomLengthsPricingType)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRLPricingTypesByGroup, conn)
                    cmd.Parameters.AddWithValue("@CategoryGroup", categoryGroup)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            types.Add(MapRLPricingType(reader))
                        End While
                    End Using
                End Using
            End Using
            Return types
        End Function

        ''' <summary>Get RL pricing type by ID</summary>
        Public Function GetRLPricingTypeByID(rlPricingTypeID As Integer) As PLRandomLengthsPricingType
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRLPricingTypeByID, conn)
                    cmd.Parameters.AddWithValue("@RLPricingTypeID", rlPricingTypeID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapRLPricingType(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        ''' <summary>Get RL pricing type by code</summary>
        Public Function GetRLPricingTypeByCode(typeCode As String) As PLRandomLengthsPricingType
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRLPricingTypeByCode, conn)
                    cmd.Parameters.AddWithValue("@TypeCode", typeCode)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapRLPricingType(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        ''' <summary>Get distinct category groups</summary>
        Public Function GetRLPricingTypeCategoryGroups() As List(Of String)
            Dim groups As New List(Of String)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRLPricingTypeCategoryGroups, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            groups.Add(reader.GetString(0))
                        End While
                    End Using
                End Using
            End Using
            Return groups
        End Function

        Private Function MapRLPricingType(reader As SqlDataReader) As PLRandomLengthsPricingType
            Return New PLRandomLengthsPricingType() With {
                .RLPricingTypeID = reader.GetInt32(reader.GetOrdinal("RLPricingTypeID")),
                .TypeCode = reader.GetString(reader.GetOrdinal("TypeCode")),
                .TypeName = reader.GetString(reader.GetOrdinal("TypeName")),
                .TypeDescription = If(reader.IsDBNull(reader.GetOrdinal("TypeDescription")), Nothing, reader.GetString(reader.GetOrdinal("TypeDescription"))),
                .CategoryGroup = reader.GetString(reader.GetOrdinal("CategoryGroup")),
                .DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder")),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            }
        End Function

#End Region

#Region "Random Lengths Pricing"

        ''' <summary>Get all pricing for a specific RL import</summary>
        Public Function GetRandomLengthsPricingByImport(randomLengthsImportID As Integer) As List(Of PLRandomLengthsPricing)
            Dim pricing As New List(Of PLRandomLengthsPricing)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRandomLengthsPricingByImport, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", randomLengthsImportID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            pricing.Add(MapRandomLengthsPricing(reader))
                        End While
                    End Using
                End Using
            End Using
            Return pricing
        End Function

        ''' <summary>Get single pricing record by ID</summary>
        Public Function GetRandomLengthsPricingByID(randomLengthsPricingID As Integer) As PLRandomLengthsPricing
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRandomLengthsPricingByID, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsPricingID", randomLengthsPricingID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then Return MapRandomLengthsPricing(reader)
                    End Using
                End Using
            End Using
            Return Nothing
        End Function

        ''' <summary>Insert new RL pricing record</summary>
        Public Function InsertRandomLengthsPricing(pricing As PLRandomLengthsPricing) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertRandomLengthsPricing, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", pricing.RandomLengthsImportID)
                    cmd.Parameters.AddWithValue("@RLPricingTypeID", pricing.RLPricingTypeID)
                    cmd.Parameters.AddWithValue("@Price", pricing.Price)
                    cmd.Parameters.AddWithValue("@PriceSource", If(pricing.PriceSource, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@Notes", If(pricing.Notes, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        ''' <summary>Update RL pricing record</summary>
        Public Sub UpdateRandomLengthsPricing(pricing As PLRandomLengthsPricing)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateRandomLengthsPricing, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsPricingID", pricing.RandomLengthsPricingID)
                    cmd.Parameters.AddWithValue("@Price", pricing.Price)
                    cmd.Parameters.AddWithValue("@PriceSource", If(pricing.PriceSource, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@Notes", If(pricing.Notes, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Insert or update RL pricing (upsert)</summary>
        Public Function UpsertRandomLengthsPricing(pricing As PLRandomLengthsPricing) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpsertRandomLengthsPricing, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", pricing.RandomLengthsImportID)
                    cmd.Parameters.AddWithValue("@RLPricingTypeID", pricing.RLPricingTypeID)
                    cmd.Parameters.AddWithValue("@Price", pricing.Price)
                    cmd.Parameters.AddWithValue("@PriceSource", If(pricing.PriceSource, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@Notes", If(pricing.Notes, CObj(DBNull.Value)))
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        ''' <summary>Delete RL pricing record</summary>
        Public Sub DeleteRandomLengthsPricing(randomLengthsPricingID As Integer)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeleteRandomLengthsPricing, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsPricingID", randomLengthsPricingID)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Delete all pricing for an import</summary>
        Public Sub DeleteRandomLengthsPricingByImport(randomLengthsImportID As Integer)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeleteRandomLengthsPricingByImport, conn)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", randomLengthsImportID)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Function MapRandomLengthsPricing(reader As SqlDataReader) As PLRandomLengthsPricing
            Dim rlp As New PLRandomLengthsPricing() With {
                .RandomLengthsPricingID = reader.GetInt32(reader.GetOrdinal("RandomLengthsPricingID")),
                .RandomLengthsImportID = reader.GetInt32(reader.GetOrdinal("RandomLengthsImportID")),
                .RLPricingTypeID = reader.GetInt32(reader.GetOrdinal("RLPricingTypeID")),
                .Price = reader.GetDecimal(reader.GetOrdinal("Price"))
            }

            ' Nullable fields
            If Not reader.IsDBNull(reader.GetOrdinal("PriceSource")) Then rlp.PriceSource = reader.GetString(reader.GetOrdinal("PriceSource"))
            If Not reader.IsDBNull(reader.GetOrdinal("Notes")) Then rlp.Notes = reader.GetString(reader.GetOrdinal("Notes"))

            ' Navigation properties from JOIN
            If HasColumn(reader, "TypeCode") AndAlso Not reader.IsDBNull(reader.GetOrdinal("TypeCode")) Then
                rlp.TypeCode = reader.GetString(reader.GetOrdinal("TypeCode"))
            End If
            If HasColumn(reader, "TypeName") AndAlso Not reader.IsDBNull(reader.GetOrdinal("TypeName")) Then
                rlp.TypeName = reader.GetString(reader.GetOrdinal("TypeName"))
            End If
            If HasColumn(reader, "CategoryGroup") AndAlso Not reader.IsDBNull(reader.GetOrdinal("CategoryGroup")) Then
                rlp.CategoryGroup = reader.GetString(reader.GetOrdinal("CategoryGroup"))
            End If
            If HasColumn(reader, "DisplayOrder") AndAlso Not reader.IsDBNull(reader.GetOrdinal("DisplayOrder")) Then
                rlp.DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder"))
            End If

            Return rlp
        End Function

#End Region

#Region "Material Category RL Mapping"

        ''' <summary>Get all active Material Category to RL mappings</summary>
        Public Function GetMaterialCategoryRLMappings() As List(Of PLMaterialCategoryRLMapping)
            Dim mappings As New List(Of PLMaterialCategoryRLMapping)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectMaterialCategoryRLMappings, conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            mappings.Add(MapMaterialCategoryRLMapping(reader))
                        End While
                    End Using
                End Using
            End Using
            Return mappings
        End Function

        ''' <summary>Get mappings for a specific material category</summary>
        Public Function GetMaterialCategoryRLMappingsByCategory(materialCategoryID As Integer) As List(Of PLMaterialCategoryRLMapping)
            Dim mappings As New List(Of PLMaterialCategoryRLMapping)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectMaterialCategoryRLMappingsByCategory, conn)
                    cmd.Parameters.AddWithValue("@MaterialCategoryID", materialCategoryID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            mappings.Add(MapMaterialCategoryRLMapping(reader))
                        End While
                    End Using
                End Using
            End Using
            Return mappings
        End Function

        ''' <summary>Get weighted RL price for a material category from a specific import</summary>
        Public Function GetMaterialCategoryWeightedRLPrice(materialCategoryID As Integer, randomLengthsImportID As Integer) As Decimal?
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectMaterialCategoryWeightedRLPrice, conn)
                    cmd.Parameters.AddWithValue("@MaterialCategoryID", materialCategoryID)
                    cmd.Parameters.AddWithValue("@RandomLengthsImportID", randomLengthsImportID)
                    conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        Return CDec(result)
                    End If
                End Using
            End Using
            Return Nothing
        End Function

        Private Function MapMaterialCategoryRLMapping(reader As SqlDataReader) As PLMaterialCategoryRLMapping
            Return New PLMaterialCategoryRLMapping() With {
                .MappingID = reader.GetInt32(reader.GetOrdinal("MappingID")),
                .MaterialCategoryID = reader.GetInt32(reader.GetOrdinal("MaterialCategoryID")),
                .RLPricingTypeID = reader.GetInt32(reader.GetOrdinal("RLPricingTypeID")),
                .WeightFactor = reader.GetDecimal(reader.GetOrdinal("WeightFactor")),
                .IsPrimary = reader.GetBoolean(reader.GetOrdinal("IsPrimary")),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                .RLTypeCode = reader.GetString(reader.GetOrdinal("TypeCode")),
                .RLTypeName = reader.GetString(reader.GetOrdinal("TypeName"))
            }
        End Function
        ' ADD after line 1780 (after MapMaterialCategoryRLMapping function):

        ''' <summary>Insert new Material Category RL mapping</summary>
        Public Function InsertMaterialCategoryRLMapping(mapping As PLMaterialCategoryRLMapping) As Integer
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.InsertMaterialCategoryRLMapping, conn)
                    cmd.Parameters.AddWithValue("@MaterialCategoryID", mapping.MaterialCategoryID)
                    cmd.Parameters.AddWithValue("@RLPricingTypeID", mapping.RLPricingTypeID)
                    cmd.Parameters.AddWithValue("@WeightFactor", mapping.WeightFactor)
                    cmd.Parameters.AddWithValue("@IsPrimary", mapping.IsPrimary)
                    cmd.Parameters.AddWithValue("@IsActive", mapping.IsActive)
                    conn.Open()
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        ''' <summary>Update Material Category RL mapping</summary>
        Public Sub UpdateMaterialCategoryRLMapping(mapping As PLMaterialCategoryRLMapping)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateMaterialCategoryRLMapping, conn)
                    cmd.Parameters.AddWithValue("@MappingID", mapping.MappingID)
                    cmd.Parameters.AddWithValue("@IsPrimary", mapping.IsPrimary)
                    cmd.Parameters.AddWithValue("@IsActive", mapping.IsActive)
                    cmd.Parameters.AddWithValue("@WeightFactor", mapping.WeightFactor)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Delete Material Category RL mapping</summary>
        Public Sub DeleteMaterialCategoryRLMapping(mappingID As Integer)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.DeleteMaterialCategoryRLMapping, conn)
                    cmd.Parameters.AddWithValue("@MappingID", mappingID)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

#End Region

#Region "Price Lock RL References"

        ''' <summary>Update price lock baseline RL import</summary>
        Public Sub UpdatePriceLockBaselineRL(priceLockID As Integer, baselineRLImportID As Integer?)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdatePriceLockBaselineRL, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    cmd.Parameters.AddWithValue("@BaselineRLImportID", If(baselineRLImportID, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Update price lock current RL import</summary>
        Public Sub UpdatePriceLockCurrentRL(priceLockID As Integer, currentRLImportID As Integer?)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdatePriceLockCurrentRL, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    cmd.Parameters.AddWithValue("@CurrentRLImportID", If(currentRLImportID, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Update both price lock RL imports</summary>
        Public Sub UpdatePriceLockRLImports(priceLockID As Integer, baselineRLImportID As Integer?, currentRLImportID As Integer?)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdatePriceLockRLImports, conn)
                    cmd.Parameters.AddWithValue("@PriceLockID", priceLockID)
                    cmd.Parameters.AddWithValue("@BaselineRLImportID", If(baselineRLImportID, CObj(DBNull.Value)))
                    cmd.Parameters.AddWithValue("@CurrentRLImportID", If(currentRLImportID, CObj(DBNull.Value)))
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

#End Region

#Region "RL-Based Material Pricing Calculations"

        ''' <summary>
        ''' Calculate percent change for all material categories between two RL imports
        ''' </summary>
        Public Function GetRLPercentChangeAllCategories(baselineRLImportID As Integer, currentRLImportID As Integer) As List(Of PLRLPriceChangeResult)
            Dim results As New List(Of PLRLPriceChangeResult)()
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.SelectRLPercentChangeAllCategories, conn)
                    cmd.Parameters.AddWithValue("@BaselineRLImportID", baselineRLImportID)
                    cmd.Parameters.AddWithValue("@CurrentRLImportID", currentRLImportID)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim result As New PLRLPriceChangeResult() With {
                                .MaterialCategoryID = reader.GetInt32(reader.GetOrdinal("MaterialCategoryID")),
                                .CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                            }
                            If Not reader.IsDBNull(reader.GetOrdinal("BaselineRLPrice")) Then
                                result.BaselineRLPrice = reader.GetDecimal(reader.GetOrdinal("BaselineRLPrice"))
                            End If
                            If Not reader.IsDBNull(reader.GetOrdinal("CurrentRLPrice")) Then
                                result.CurrentRLPrice = reader.GetDecimal(reader.GetOrdinal("CurrentRLPrice"))
                            End If
                            If Not reader.IsDBNull(reader.GetOrdinal("PercentChange")) Then
                                result.PercentChange = reader.GetDecimal(reader.GetOrdinal("PercentChange"))
                            End If
                            results.Add(result)
                        End While
                    End Using
                End Using
            End Using
            Return results
        End Function


        ''' <summary>
        ''' Calculate material pricing based on Random Lengths percent change (Option B logic)
        ''' Formula: New Price = Previous Approved Price × (1 + RL % Change)
        ''' Where: RL % Change = (Current RL Price - Previous RL Price) / Previous RL Price
        ''' </summary>
        Public Sub CalculateMaterialPricingFromRL(priceLockID As Integer, userName As String)
            Using conn As New SqlConnection(_connectionString)
                Using tran = conn.BeginTransaction()
                    Try
                        ' 1. Get the price lock
                        Dim priceLock = GetPriceLockByID(priceLockID)
                        If priceLock Is Nothing Then
                            Throw New Exception("Price lock not found.")
                        End If

                        ' 2. Validate RL imports are selected
                        If Not priceLock.BaselineRLImportID.HasValue OrElse Not priceLock.CurrentRLImportID.HasValue Then
                            Throw New Exception("Both Baseline and Current RL imports must be selected.")
                        End If

                        Dim baselineImportID = priceLock.BaselineRLImportID.Value
                        Dim currentImportID = priceLock.CurrentRLImportID.Value

                        ' 3. Get all material pricing records for this lock
                        Dim materialPricing = GetMaterialPricingByLock(priceLockID)
                        If materialPricing.Count = 0 Then
                            Throw New Exception("No material pricing records found for this price lock.")
                        End If

                        ' 4. Get material category → RL pricing type mappings
                        Dim categoryMappings = GetMaterialCategoryRLMappings(conn, tran)

                        ' 5. Get RL prices for baseline and current imports
                        Dim baselinePrices = GetRandomLengthsPricingByImport(baselineImportID, conn, tran)
                        Dim currentPrices = GetRandomLengthsPricingByImport(currentImportID, conn, tran)

                        ' 6. Get previous price lock for this subdivision (to get previous approved prices)
                        Dim previousLock = GetPreviousPriceLock(priceLock.SubdivisionID, priceLock.PriceLockDate, conn, tran)

                        Dim updatedCount = 0
                        Dim skippedCount = 0

                        ' 7. Calculate new prices for each material category
                        For Each matPrice In materialPricing
                            Try
                                ' Get RL pricing type for this material category
                                Dim mapping = categoryMappings.FirstOrDefault(Function(m) m.MaterialCategoryID = matPrice.MaterialCategoryID AndAlso m.IsPrimary)
                                If mapping Is Nothing Then
                                    skippedCount += 1
                                    Continue For ' No RL mapping defined
                                End If

                                ' Get baseline and current RL prices
                                Dim baselineRL = baselinePrices.FirstOrDefault(Function(p) p.RLPricingTypeID = mapping.RLPricingTypeID)
                                Dim currentRL = currentPrices.FirstOrDefault(Function(p) p.RLPricingTypeID = mapping.RLPricingTypeID)

                                If baselineRL Is Nothing OrElse currentRL Is Nothing Then
                                    skippedCount += 1
                                    Continue For ' RL prices not available
                                End If

                                ' Calculate RL percent change
                                If baselineRL.Price = 0 Then
                                    skippedCount += 1
                                    Continue For ' Can't divide by zero
                                End If

                                Dim rlPctChange = (currentRL.Price - baselineRL.Price) / baselineRL.Price

                                ' Get previous approved price (OPTION B LOGIC)
                                Dim previousApprovedPrice As Decimal? = Nothing
                                If previousLock IsNot Nothing Then
                                    previousApprovedPrice = GetPreviousMaterialPrice(
                                priceLock.SubdivisionID,
                                matPrice.MaterialCategoryID,
                                priceLock.PriceLockDate,
                                conn,
                                tran)
                                End If

                                ' If no previous approved price, check if OriginalSellPrice is set
                                If Not previousApprovedPrice.HasValue Then
                                    If matPrice.OriginalSellPrice.HasValue AndAlso matPrice.OriginalSellPrice.Value > 0 Then
                                        previousApprovedPrice = matPrice.OriginalSellPrice.Value
                                    Else
                                        skippedCount += 1
                                        Continue For ' No baseline price to work from
                                    End If
                                End If

                                ' CALCULATE NEW PRICE (Option B Formula)
                                Dim newCalculatedPrice = previousApprovedPrice.Value * (1 + rlPctChange)

                                ' Update the material pricing record
                                matPrice.BaselineRLPrice = baselineRL.Price
                                matPrice.CurrentRLPrice = currentRL.Price
                                matPrice.RLPercentChange = rlPctChange
                                matPrice.CalculatedPrice = newCalculatedPrice
                                matPrice.PriceSentToSales = newCalculatedPrice  ' Default to calculated
                                matPrice.PriceSentToBuilder = newCalculatedPrice ' Default to calculated
                                matPrice.ModifiedBy = userName
                                matPrice.ModifiedDate = DateTime.Now

                                ' Update in database
                                UpdateMaterialPricing(matPrice)
                                updatedCount += 1

                            Catch ex As Exception
                                ' Log but continue with other categories
                                System.Diagnostics.Debug.WriteLine($"Error calculating price for category {matPrice.MaterialCategoryID}: {ex.Message}")
                                skippedCount += 1
                            End Try
                        Next

                        tran.Commit()

                        If skippedCount > 0 Then
                            System.Diagnostics.Debug.WriteLine($"RL Calculation complete: {updatedCount} updated, {skippedCount} skipped")
                        End If

                    Catch ex As Exception
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Sub

        ' HELPER METHOD: Get material category → RL pricing type mappings
        Private Function GetMaterialCategoryRLMappings(conn As SqlConnection, tran As SqlTransaction) As List(Of PLMaterialCategoryRLMapping)
            Dim mappings As New List(Of PLMaterialCategoryRLMapping)()

            Dim sql = "SELECT MappingID, MaterialCategoryID, RLPricingTypeID, WeightFactor, IsPrimary, IsActive 
               FROM PL_MaterialCategoryRLMapping 
               WHERE IsActive = 1"

            Using cmd As New SqlCommand(sql, conn, tran)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        mappings.Add(New PLMaterialCategoryRLMapping() With {
                    .MappingID = CInt(reader("MappingID")),
                    .MaterialCategoryID = CInt(reader("MaterialCategoryID")),
                    .RLPricingTypeID = CInt(reader("RLPricingTypeID")),
                    .WeightFactor = CDec(reader("WeightFactor")),
                    .IsPrimary = CBool(reader("IsPrimary")),
                    .IsActive = CBool(reader("IsActive"))
                })
                    End While
                End Using
            End Using

            Return mappings
        End Function

        ' HELPER METHOD: Get RL pricing by import (with transaction)
        Private Function GetRandomLengthsPricingByImport(importID As Integer, conn As SqlConnection, tran As SqlTransaction) As List(Of PLRandomLengthsPricing)
            Dim pricing As New List(Of PLRandomLengthsPricing)()

            Dim sql = "SELECT RandomLengthsPricingID, RandomLengthsImportID, RLPricingTypeID, Price, PriceSource, Notes 
               FROM PL_RandomLengthsPricing 
               WHERE RandomLengthsImportID = @ImportID"

            Using cmd As New SqlCommand(sql, conn, tran)
                cmd.Parameters.AddWithValue("@ImportID", importID)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        pricing.Add(New PLRandomLengthsPricing() With {
                    .RandomLengthsPricingID = CInt(reader("RandomLengthsPricingID")),
                    .RandomLengthsImportID = CInt(reader("RandomLengthsImportID")),
                    .RLPricingTypeID = CInt(reader("RLPricingTypeID")),
                    .Price = CDec(reader("Price")),
                    .PriceSource = If(reader("PriceSource") Is DBNull.Value, Nothing, CStr(reader("PriceSource"))),
                    .Notes = If(reader("Notes") Is DBNull.Value, Nothing, CStr(reader("Notes")))
                })
                    End While
                End Using
            End Using

            Return pricing
        End Function

        ' HELPER METHOD: Get previous price lock (with transaction)
        Private Function GetPreviousPriceLock(subdivisionID As Integer, currentDate As Date, conn As SqlConnection, tran As SqlTransaction) As PLPriceLock
            Dim sql = "SELECT TOP 1 PriceLockID, SubdivisionID, PriceLockDate, PriceLockName, Status 
               FROM PL_PriceLocks 
               WHERE SubdivisionID = @SubdivisionID 
                 AND PriceLockDate < @CurrentDate 
               ORDER BY PriceLockDate DESC"

            Using cmd As New SqlCommand(sql, conn, tran)
                cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                cmd.Parameters.AddWithValue("@CurrentDate", currentDate)

                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Return New PLPriceLock() With {
                    .PriceLockID = CInt(reader("PriceLockID")),
                    .SubdivisionID = CInt(reader("SubdivisionID")),
                    .PriceLockDate = CDate(reader("PriceLockDate")),
                    .PriceLockName = If(reader("PriceLockName") Is DBNull.Value, Nothing, CStr(reader("PriceLockName"))),
                    .Status = CStr(reader("Status"))
                }
                    End If
                End Using
            End Using

            Return Nothing
        End Function

        ' HELPER METHOD: Get previous material price (with transaction overload)
        Private Function GetPreviousMaterialPrice(subdivisionID As Integer, categoryID As Integer, currentDate As Date,
                                          conn As SqlConnection, tran As SqlTransaction) As Decimal?
            Dim sql = "SELECT TOP 1 PriceSentToSales 
               FROM PL_MaterialPricing mp
               JOIN PL_PriceLocks pl ON mp.PriceLockID = pl.PriceLockID
               WHERE pl.SubdivisionID = @SubdivisionID
                 AND mp.MaterialCategoryID = @CategoryID
                 AND pl.PriceLockDate < @CurrentDate
                 AND mp.PriceSentToSales IS NOT NULL
               ORDER BY pl.PriceLockDate DESC"

            Using cmd As New SqlCommand(sql, conn, tran)
                cmd.Parameters.AddWithValue("@SubdivisionID", subdivisionID)
                cmd.Parameters.AddWithValue("@CategoryID", categoryID)
                cmd.Parameters.AddWithValue("@CurrentDate", currentDate)

                Dim result = cmd.ExecuteScalar()
                Return If(result IsNot Nothing AndAlso result IsNot DBNull.Value, CType(CDec(result), Decimal?), Nothing)
            End Using
        End Function

        ''' <summary>
        ''' Set the original sell price for a material pricing record
        ''' </summary>
        Public Sub UpdateMaterialPricingOriginalSellPrice(materialPricingID As Integer, originalSellPrice As Decimal, modifiedBy As String)
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(PriceLockQueries.UpdateMaterialPricingOriginalSellPrice, conn)
                    cmd.Parameters.AddWithValue("@MaterialPricingID", materialPricingID)
                    cmd.Parameters.AddWithValue("@OriginalSellPrice", originalSellPrice)
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

#End Region

    End Class

End Namespace