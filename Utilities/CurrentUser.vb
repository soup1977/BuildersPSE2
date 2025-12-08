Option Strict On

Public NotInheritable Class CurrentUser
        Private Sub New()
            ' Prevent instantiation
        End Sub

        Public Shared Property UserID As Integer = 0
        Public Shared Property WindowsLogon As String = String.Empty
        Public Shared Property DisplayName As String = String.Empty
        Public Shared Property IsAdmin As Boolean = False

        ''' <summary>
        ''' Call this once after successful login (from frmLoginSplash)
        ''' </summary>
        Public Shared Sub SetCurrentUser(userID As Integer, windowsLogon As String, displayName As String, isAdmin As Boolean)
            CurrentUser.UserID = userID
            CurrentUser.WindowsLogon = windowsLogon
            CurrentUser.DisplayName = displayName
            CurrentUser.IsAdmin = isAdmin
        End Sub

        ''' <summary>
        ''' Call this on logout / app close
        ''' </summary>
        Public Shared Sub Clear()
            UserID = 0
            WindowsLogon = String.Empty
            DisplayName = String.Empty
            IsAdmin = False
        End Sub
    End Class

