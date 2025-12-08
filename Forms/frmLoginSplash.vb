Option Strict On

Imports System.Data.SqlClient
Imports System.Security.Principal
Imports BuildersPSE2.DataAccess      ' Queries
Imports BuildersPSE2.Utilities       ' SqlConnectionManager & CurrentUser

Public Class frmLoginSplash

    Private Sub frmLoginSplash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblStatus.Text = "Authenticating..."
        lblStatus.Refresh()

        Dim currentLogon As String = WindowsIdentity.GetCurrent().Name.Trim()
        Dim defaultName As String = currentLogon.Substring(currentLogon.IndexOf("\"c) + 1)

        Try
            Using conn As SqlConnection = SqlConnectionManager.Instance.GetConnection()

                ' Look for existing user
                Using cmd As New SqlCommand(Queries.SelectUserByWindowsLogon, conn)
                    cmd.Parameters.AddWithValue("@WindowsLogon", currentLogon)
                    Using r As SqlDataReader = cmd.ExecuteReader()
                        If r.Read() Then
                            If Not CBool(r("IsActive")) Then
                                MessageBox.Show("Account is inactive.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                                Application.Exit()
                                Return
                            End If

                            CurrentUser.SetCurrentUser(
                                CInt(r("UserID")),
                                currentLogon,
                                CStr(r("DisplayName")),
                                CBool(r("IsAdmin"))
                            )
                            lblStatus.Text = "Welcome back, " & CurrentUser.DisplayName & "!"
                        Else
                            ' Auto-create new user
                            r.Close()
                            Using ins As New SqlCommand(Queries.InsertUser, conn)
                                ins.Parameters.AddWithValue("@WindowsLogon", currentLogon)
                                ins.Parameters.AddWithValue("@DisplayName", defaultName)
                                ins.Parameters.AddWithValue("@EstimatorID", DBNull.Value)
                                Dim newId As Integer = CInt(ins.ExecuteScalar())
                                CurrentUser.SetCurrentUser(newId, currentLogon, defaultName, False)
                            End Using
                            lblStatus.Text = "New user created: " & defaultName
                        End If
                    End Using
                End Using

                ' Mark as logged in
                Using cmd As New SqlCommand(Queries.UpdateUserLoginStatus, conn)
                    cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserID)
                    cmd.Parameters.AddWithValue("@IsLoggedIn", True)
                    cmd.Parameters.AddWithValue("@LastLogin", Now)
                    cmd.Parameters.AddWithValue("@LastLogout", DBNull.Value)
                    cmd.ExecuteNonQuery()
                End Using

            End Using

            ' Done! Enable the button
            btnLogin.Enabled = True
            btnLogin.Text = "Continue →"
            btnLogin.Focus()

        Catch ex As Exception
            MessageBox.Show("Login failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Application.Exit()
        End Try
    End Sub

    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Me.Hide()
        Using main As New frmMain()
            main.ShowDialog()     ' When frmMain closes, we come back here
        End Using
        Me.Close()                ' Splash closes → app ends perfectly
    End Sub



End Class