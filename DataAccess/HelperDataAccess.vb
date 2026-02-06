Option Strict On
Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Namespace DataAccess
    ''' <summary>
    ''' Helper methods for parameter building, lookups, and GetOrInsert operations.
    ''' For Insert/Update/Upsert of main entities, use TableOperations instead.
    ''' </summary>
    Public Class HelperDataAccess

#Region "Parameter Building"

        ''' <summary>Builds SqlParameter array from dictionary. Core utility used everywhere.</summary>
        Public Shared Function BuildParameters(params As IDictionary(Of String, Object)) As SqlParameter()
            Dim sqlParams As New List(Of SqlParameter)
            For Each kvp As KeyValuePair(Of String, Object) In params
                sqlParams.Add(New SqlParameter(kvp.Key, If(kvp.Value, DBNull.Value)))
            Next
            Return sqlParams.ToArray()
        End Function

#End Region

#Region "Validation Helpers"

        ''' <summary>Validates that a CustomerID has the required CustomerType.</summary>
        Public Shared Function ValidateCustomerType(customerID As Integer?, requiredType As Integer) As Boolean
            If Not customerID.HasValue Then Return True
            Dim params As SqlParameter() = {New SqlParameter("@CustomerID", customerID.Value)}
            Dim typeObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                Queries.SelectCustomerTypeByID, params)
            If typeObj Is DBNull.Value OrElse typeObj Is Nothing Then Return False
            Return CInt(typeObj) = requiredType
        End Function

#End Region

#Region "GetOrInsert Helpers"

        ''' <summary>Gets existing CustomerID or inserts new customer and returns ID.</summary>
        Public Shared Function GetOrInsertCustomer(customerName As String, customerType As Integer,
                                                   conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(customerName) Then Return Nothing

            Dim params As New Dictionary(Of String, Object) From {
                {"@CustomerName", customerName},
                {"@CustomerType", customerType}
            }
            Dim customerIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(
                Queries.SelectCustomerIDByNameAndType, BuildParameters(params), conn, transaction)

            If customerIDObj Is DBNull.Value OrElse customerIDObj Is Nothing Then
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                    Queries.InsertCustomer, BuildParameters(params), conn, transaction)
            End If
            Return CInt(customerIDObj)
        End Function

        ''' <summary>Gets existing EstimatorID or inserts new estimator and returns ID.</summary>
        Public Shared Function GetOrInsertEstimator(estimatorName As String,
                                                    conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(estimatorName) Then Return Nothing

            Dim params As New Dictionary(Of String, Object) From {{"@EstimatorName", estimatorName}}
            Dim estimatorIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(
                Queries.SelectEstimatorIDByName, BuildParameters(params), conn, transaction)

            If estimatorIDObj Is DBNull.Value OrElse estimatorIDObj Is Nothing Then
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                    Queries.InsertEstimator, BuildParameters(params), conn, transaction)
            End If
            Return CInt(estimatorIDObj)
        End Function

        ''' <summary>Gets existing SalesID or inserts new sales person and returns ID.</summary>
        Public Shared Function GetOrInsertSales(salesName As String,
                                                conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(salesName) Then Return Nothing

            Dim params As New Dictionary(Of String, Object) From {{"@SalesName", salesName}}
            Dim salesIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(
                Queries.SelectSalesIDByName, BuildParameters(params), conn, transaction)

            If salesIDObj Is DBNull.Value OrElse salesIDObj Is Nothing Then
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                    Queries.InsertSales, BuildParameters(params), conn, transaction)
            End If
            Return CInt(salesIDObj)
        End Function

#End Region

#Region "Read Helpers"

        ''' <summary>Gets all estimators for dropdowns.</summary>
        Public Shared Function GetEstimators() As List(Of EstimatorModel)
            Dim estimators As New List(Of EstimatorModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectEstimators)
                                                                           While reader.Read()
                                                                               estimators.Add(New EstimatorModel With {
                                                                                   .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
                                                                                   .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("EstimatorName")),
                                                                                       reader.GetString(reader.GetOrdinal("EstimatorName")), String.Empty)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading estimators")
            Return estimators
        End Function

        ''' <summary>Gets customers, optionally filtered by type.</summary>
        Public Shared Function GetCustomers(Optional customerType As Integer? = Nothing) As List(Of CustomerModel)
            Dim customers As New List(Of CustomerModel)
            Dim params As SqlParameter() = {New SqlParameter("@CustomerType", If(customerType.HasValue, CObj(customerType.Value), DBNull.Value))}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomers, params)
                                                                           While reader.Read()
                                                                               customers.Add(New CustomerModel With {
                                                                                   .CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                                                                   .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")),
                                                                                       reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty),
                                                                                   .CustomerTypeID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerType")),
                                                                                       reader.GetInt32(reader.GetOrdinal("CustomerType")), Nothing),
                                                                                   .CustomerType = New CustomerTypeModel With {
                                                                                       .CustomerTypeID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerType")),
                                                                                           reader.GetInt32(reader.GetOrdinal("CustomerType")), 0),
                                                                                       .CustomerTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerTypeName")),
                                                                                           reader.GetString(reader.GetOrdinal("CustomerTypeName")), String.Empty)
                                                                                   }
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading customers")
            Return customers
        End Function

        ''' <summary>Gets all customer types for dropdowns.</summary>
        Public Shared Function GetCustomerTypes() As List(Of CustomerTypeModel)
            Dim types As New List(Of CustomerTypeModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomerTypes)
                                                                           While reader.Read()
                                                                               types.Add(New CustomerTypeModel With {
                                                                                   .CustomerTypeID = reader.GetInt32(reader.GetOrdinal("CustomerTypeID")),
                                                                                   .CustomerTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerTypeName")),
                                                                                       reader.GetString(reader.GetOrdinal("CustomerTypeName")), String.Empty)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading customer types")
            Return types
        End Function

        ''' <summary>Gets all sales people for dropdowns.</summary>
        Public Shared Function GetSales() As List(Of SalesModel)
            Dim sales As New List(Of SalesModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectSales)
                                                                           While reader.Read()
                                                                               sales.Add(New SalesModel With {
                                                                                   .SalesID = reader.GetInt32(reader.GetOrdinal("SalesID")),
                                                                                   .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")),
                                                                                       reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading sales")
            Return sales
        End Function

#End Region

#Region "Save Helpers"

        ''' <summary>Saves estimator (insert or update). Returns EstimatorID.</summary>
        Public Function SaveEstimator(estimator As EstimatorModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@EstimatorName", TableOperations.ToDb(estimator.EstimatorName)}
            }
            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If estimator.EstimatorID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                               Queries.InsertEstimator, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@EstimatorID", estimator.EstimatorID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateEstimator, BuildParameters(paramsDict))
                                                                           result = estimator.EstimatorID
                                                                       End If
                                                                   End Sub, "Error saving estimator")
            Return result
        End Function

        ''' <summary>Saves customer (insert or update). Returns CustomerID.</summary>
        Public Shared Function SaveCustomer(customer As CustomerModel, customerTypeID As Integer?) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@CustomerName", TableOperations.ToDb(customer.CustomerName)},
                {"@CustomerType", TableOperations.ToDb(customerTypeID)}
            }
            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If customer.CustomerID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                               Queries.InsertCustomer, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@CustomerID", customer.CustomerID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateCustomer, BuildParameters(paramsDict))
                                                                           result = customer.CustomerID
                                                                       End If
                                                                   End Sub, "Error saving customer")
            Return result
        End Function

        ''' <summary>Saves sales person (insert or update). Returns SalesID.</summary>
        Public Shared Function SaveSales(sale As SalesModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@SalesName", TableOperations.ToDb(sale.SalesName)}
            }
            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If sale.SalesID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                               Queries.InsertSales, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@SalesID", sale.SalesID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateSales, BuildParameters(paramsDict))
                                                                           result = sale.SalesID
                                                                       End If
                                                                   End Sub, "Error saving sales")
            Return result
        End Function

#End Region

    End Class
End Namespace