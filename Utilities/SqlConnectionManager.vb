Option Strict On

Imports System.Configuration
Imports System.Data.SqlClient

Namespace BuildersPSE.Utilities
    Public Class SqlConnectionManager
        Private Shared _instance As SqlConnectionManager
        Private Shared ReadOnly _lock As New Object()
        Private ReadOnly _connectionString As String

        Private Sub New()
            Dim connName As String = "DbConn" ' Or "BuildersPSE2.My.MySettings.DbConn" if sticking to generated
            Dim connSetting As ConnectionStringSettings = ConfigurationManager.ConnectionStrings(connName)
            If connSetting Is Nothing OrElse String.IsNullOrEmpty(connSetting.ConnectionString) Then
                Throw New InvalidOperationException("Connection string '" & connName & "' not found in App.config or invalid.")
            End If
            _connectionString = connSetting.ConnectionString
        End Sub

        Public Shared ReadOnly Property Instance As SqlConnectionManager
            Get
                If _instance Is Nothing Then
                    SyncLock _lock
                        If _instance Is Nothing Then
                            _instance = New SqlConnectionManager()
                        End If
                    End SyncLock
                End If
                Return _instance
            End Get
        End Property

        Public ReadOnly Property ConnectionString As String
            Get
                Return _connectionString
            End Get
        End Property

        Public Function GetConnection() As SqlConnection
            Dim conn As New SqlConnection(_connectionString)
            conn.Open()
            Return conn
        End Function

        ' Original ExecuteNonQuery (unchanged for compatibility)
        Public Sub ExecuteNonQuery(query As String, params As SqlParameter())
            Using conn As New SqlConnection(ConnectionString)
                conn.Open()
                Using cmd As New SqlCommand(query, conn)
                    If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' New transactional overload for specific use cases
        Public Sub ExecuteNonQueryTransactional(query As String, params As SqlParameter(), conn As SqlConnection, Optional transaction As SqlTransaction = Nothing)
            Using cmd As New SqlCommand(query, conn)
                If transaction IsNot Nothing Then cmd.Transaction = transaction
                If params IsNot Nothing Then cmd.Parameters.AddRange(params)
                cmd.ExecuteNonQuery()
            End Using
        End Sub




        Public Function ExecuteScalar(Of T)(sql As String, Optional parameters As SqlParameter() = Nothing) As T
            Using conn As SqlConnection = GetConnection()
                Using cmd As New SqlCommand(sql, conn)
                    If parameters IsNot Nothing Then
                        cmd.Parameters.AddRange(parameters)
                    End If
                    Return CType(cmd.ExecuteScalar(), T)
                End Using
            End Using
        End Function

        Public Function ExecuteReader(sql As String, Optional parameters As SqlParameter() = Nothing) As SqlDataReader
            Dim conn As SqlConnection = GetConnection()
            Dim cmd As New SqlCommand(sql, conn)
            If parameters IsNot Nothing Then
                cmd.Parameters.AddRange(parameters)
            End If
            Return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection) ' Auto-closes conn on reader close
        End Function

        ' Added: Execute reader within a transaction
        ' Execute reader within a transaction
        Public Function ExecuteReaderTransactional(sql As String, parameters As SqlParameter(), conn As SqlConnection, Optional transaction As SqlTransaction = Nothing) As SqlDataReader
            Dim cmd As New SqlCommand(sql, conn)
            If transaction IsNot Nothing Then cmd.Transaction = transaction
            If parameters IsNot Nothing Then cmd.Parameters.AddRange(parameters)
            Return cmd.ExecuteReader() ' Remove CommandBehavior.CloseConnection to keep connection open
        End Function

        ' Added: Helper method for executing queries with error handling (moved from DataAccess.vb)
        Public Sub ExecuteWithErrorHandling(action As Action, errorMessage As String)
            Try
                action()
            Catch ex As Exception
                Throw New ApplicationException(errorMessage & ": " & ex.Message)
            End Try
        End Sub

        ' Added: Helper for executing scalar with transaction (moved from DataAccess.vb)
        Public Function ExecuteScalarTransactional(Of T)(query As String, params As SqlParameter(), conn As SqlConnection, transaction As SqlTransaction) As T
            Using cmd As New SqlCommand(query, conn, transaction)
                cmd.Parameters.AddRange(params)
                Return CType(cmd.ExecuteScalar(), T)
            End Using
        End Function


    End Class
End Namespace