Option Strict On

Imports System.Security.Cryptography
Imports System.Text

Namespace BuildersPSE.Utilities
    Public Module SecurityHelper
        ' Encrypt the API key (run once to store encrypted value)
        Public Function EncryptApiKey(apiKey As String) As String
            If String.IsNullOrEmpty(apiKey) Then Return String.Empty
            Dim data As Byte() = Encoding.UTF8.GetBytes(apiKey)
            Dim encrypted As Byte() = ProtectedData.Protect(data, Nothing, DataProtectionScope.CurrentUser)
            Return Convert.ToBase64String(encrypted)
        End Function

        ' Decrypt the API key for use
        Public Function DecryptApiKey(encryptedKey As String) As String
            If String.IsNullOrEmpty(encryptedKey) Then Return String.Empty
            Try
                Dim encryptedData As Byte() = Convert.FromBase64String(encryptedKey)
                Dim decryptedData As Byte() = ProtectedData.Unprotect(encryptedData, Nothing, DataProtectionScope.CurrentUser)
                Return Encoding.UTF8.GetString(decryptedData)
            Catch ex As Exception
                Throw New Exception("Failed to decrypt API key: " & ex.Message)
            End Try
        End Function
    End Module
End Namespace