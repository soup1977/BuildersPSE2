Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities

Namespace DataAccess

    Public Class HelperDataAccess
        ' Helper method to build parameters from a dictionary
        Public Shared Function BuildParameters(params As IDictionary(Of String, Object)) As SqlParameter()
            Dim sqlParams As New List(Of SqlParameter)
            For Each kvp As KeyValuePair(Of String, Object) In params
                sqlParams.Add(New SqlParameter(kvp.Key, If(kvp.Value, DBNull.Value)))
            Next
            Return sqlParams.ToArray()
        End Function

        ' Create a new project version (CustomerID restricted to CustomerType=1 via UI filtering and validation)
        ' Helper method to validate CustomerType for a given CustomerID
        Public Shared Function ValidateCustomerType(customerID As Integer?, requiredType As Integer) As Boolean
            If Not customerID.HasValue Then Return True ' Allow NULL as valid (nullable field)
            Dim params As SqlParameter() = {New SqlParameter("@CustomerID", customerID.Value)}
            Dim typeObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)("SELECT CustomerType FROM Customer WHERE CustomerID = @CustomerID", params)
            If typeObj Is DBNull.Value OrElse typeObj Is Nothing Then
                Return False ' CustomerID not found
            End If
            Return CInt(typeObj) = requiredType
        End Function

        ' Helper to get or insert CustomerID
        Public Shared Function GetOrInsertCustomer(customerName As String, customerType As Integer, conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(customerName) Then Return Nothing
            Dim params As New Dictionary(Of String, Object) From {
                {"@CustomerName", customerName},
                {"@CustomerType", customerType}
            }
            Dim customerIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT CustomerID FROM Customer WHERE CustomerName = @CustomerName AND CustomerType = @CustomerType order by CustomerName", BuildParameters(params), conn, transaction)
            If customerIDObj Is DBNull.Value OrElse customerIDObj Is Nothing Then
                params = New Dictionary(Of String, Object) From {
                    {"@CustomerName", customerName},
                    {"@CustomerType", customerType}
                }
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertCustomer, BuildParameters(params), conn, transaction)
            End If
            Return CInt(customerIDObj)
        End Function

        ' Helper to get or insert EstimatorID
        Public Shared Function GetOrInsertEstimator(estimatorName As String, conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(estimatorName) Then Return Nothing
            Dim params As New Dictionary(Of String, Object) From {{"@EstimatorName", estimatorName}}
            Dim estimatorIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT EstimatorID FROM Estimator WHERE EstimatorName = @EstimatorName", BuildParameters(params), conn, transaction)
            If estimatorIDObj Is DBNull.Value OrElse estimatorIDObj Is Nothing Then
                params = New Dictionary(Of String, Object) From {{"@EstimatorName", estimatorName}}
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertEstimator, BuildParameters(params), conn, transaction)
            End If
            Return CInt(estimatorIDObj)
        End Function

        ' Helper to get or insert SalesID
        Public Shared Function GetOrInsertSales(salesName As String, conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(salesName) Then Return Nothing
            Dim params As New Dictionary(Of String, Object) From {{"@SalesName", salesName}}
            Dim salesIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT SalesID FROM Sales WHERE SalesName = @SalesName", BuildParameters(params), conn, transaction)
            If salesIDObj Is DBNull.Value OrElse salesIDObj Is Nothing Then
                params = New Dictionary(Of String, Object) From {{"@SalesName", salesName}}
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertSales, BuildParameters(params), conn, transaction)
            End If
            Return CInt(salesIDObj)
        End Function

        ' GetEstimators (unchanged)
        Public Shared Function GetEstimators() As List(Of EstimatorModel)
            Dim estimators As New List(Of EstimatorModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectEstimators)
                                                                           While reader.Read()
                                                                               Dim estimator As New EstimatorModel With {
                                                    .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
                                                    .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("EstimatorName")), reader.GetString(reader.GetOrdinal("EstimatorName")), String.Empty)
                                                }
                                                                               estimators.Add(estimator)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading estimators")
            Return estimators
        End Function

        ' SaveEstimator (unchanged)
        Public Function SaveEstimator(estimator As EstimatorModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@EstimatorName", If(String.IsNullOrEmpty(estimator.EstimatorName), DBNull.Value, CType(estimator.EstimatorName, Object))}
            }

            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If estimator.EstimatorID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertEstimator, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@EstimatorID", estimator.EstimatorID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateEstimator, BuildParameters(paramsDict))
                                                                           result = estimator.EstimatorID
                                                                       End If
                                                                   End Sub, "Error saving estimator")
            Return result
        End Function

        ' GetCustomers (unchanged)
        Public Shared Function GetCustomers(Optional customerType As Integer? = Nothing) As List(Of CustomerModel)
            Dim customers As New List(Of CustomerModel)
            Dim params As SqlParameter() = If(customerType.HasValue, {New SqlParameter("@CustomerType", customerType.Value)}, {New SqlParameter("@CustomerType", DBNull.Value)})
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomers, params)
                                                                           While reader.Read()
                                                                               Dim customer As New CustomerModel With {
                                                                           .CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                                                           .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty),
                                                                           .CustomerTypeID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerType")), reader.GetInt32(reader.GetOrdinal("CustomerType")), Nothing),
                                                                           .CustomerType = New CustomerTypeModel With {
                                                                               .CustomerTypeID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerType")), reader.GetInt32(reader.GetOrdinal("CustomerType")), 0),
                                                                               .CustomerTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerTypeName")), reader.GetString(reader.GetOrdinal("CustomerTypeName")), String.Empty)
                                                                           }
                                                                       }
                                                                               customers.Add(customer)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading customers")
            Return customers
        End Function

        ' SaveCustomer (unchanged)
        Public Shared Function SaveCustomer(customer As CustomerModel, customerTypeID As Integer?) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
        {"@CustomerName", If(String.IsNullOrEmpty(customer.CustomerName), DBNull.Value, CType(customer.CustomerName, Object))},
        {"@CustomerType", If(customerTypeID.HasValue, CType(customerTypeID.Value, Object), DBNull.Value)}
    }

            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If customer.CustomerID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertCustomer, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@CustomerID", customer.CustomerID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateCustomer, BuildParameters(paramsDict))
                                                                           result = customer.CustomerID
                                                                       End If
                                                                   End Sub, "Error saving customer")
            Return result
        End Function
        ' Get all customer types for dropdowns
        Public Shared Function GetCustomerTypes() As List(Of CustomerTypeModel)
            Dim types As New List(Of CustomerTypeModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomerTypes)
                                                                           While reader.Read()
                                                                               Dim type As New CustomerTypeModel With {
                                                                           .CustomerTypeID = reader.GetInt32(reader.GetOrdinal("CustomerTypeID")),
                                                                           .CustomerTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerTypeName")), reader.GetString(reader.GetOrdinal("CustomerTypeName")), String.Empty)
                                                                       }
                                                                               types.Add(type)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading customer types")
            Return types
        End Function

        ' GetSales (unchanged)
        Public Shared Function GetSales() As List(Of SalesModel)
            Dim sales As New List(Of SalesModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectSales)
                                                                           While reader.Read()
                                                                               Dim sale As New SalesModel With {
                                                    .SalesID = reader.GetInt32(reader.GetOrdinal("SalesID")),
                                                    .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
                                                }
                                                                               sales.Add(sale)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading sales")
            Return sales
        End Function

        ' SaveSales (unchanged)
        Public Shared Function SaveSales(sale As SalesModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@SalesName", If(String.IsNullOrEmpty(sale.SalesName), DBNull.Value, CType(sale.SalesName, Object))}
            }

            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If sale.SalesID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertSales, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@SalesID", sale.SalesID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateSales, BuildParameters(paramsDict))
                                                                           result = sale.SalesID
                                                                       End If
                                                                   End Sub, "Error saving sales")
            Return result
        End Function


    End Class
End Namespace