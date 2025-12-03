Imports System.Net.Http
Imports System.Text
Imports System.Text.Json

Imports BuildersPSE2.BuildersPSE.Models

Public Class MondaycomAccess
    Private Const ApiUrl As String = "https://api.monday.com/v2"
    Private ReadOnly _apiToken As String

    Public Sub New(apiToken As String)
        _apiToken = apiToken
    End Sub

    Public Function GetTop50BoardItems(boardId As String) As List(Of MondayBoardItem)
        Dim query As String = "query { boards(ids: " & boardId & ") { items_page(limit: 50) { items { id name column_values { id text } } } } }"
        Dim requestBody As New Dictionary(Of String, String) From {{"query", query}}

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " & _apiToken)
            client.DefaultRequestHeaders.Add("API-Version", "2023-10")

            Dim content As New StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            Dim response As HttpResponseMessage = client.PostAsync(ApiUrl, content).Result

            If Not response.IsSuccessStatusCode Then
                Throw New Exception("API error: " & response.StatusCode.ToString() & " - " & response.ReasonPhrase)
            End If

            Dim jsonResponse As String = response.Content.ReadAsStringAsync().Result
            Dim items As List(Of MondayBoardItem) = ParseBoardItems(jsonResponse)
            Return items
        End Using
    End Function

    ''' <summary>
    ''' Search monday.com items by text — BoardId is passed in directly
    ''' </summary>
    Public Function SearchMondayItems(boardId As String, searchText As String) As List(Of MondayBoardItem)
        If String.IsNullOrWhiteSpace(searchText) OrElse searchText.Trim.Length < 2 Then
            Return New List(Of MondayBoardItem)()
        End If

        Dim safeText As String = searchText.Trim()

        ' THIS IS THE ONLY QUERY THAT WORKS FOR FULL-TEXT SEARCH IN 2024
        Dim query As String = "query { " &
                          "boards(ids: " & boardId & ") { " &
                              "items_page(limit: 500) { " &
                                  "items { " &
                                      "id " &
                                      "name " &
                                      "column_values { id text } " &
                                  "} " &
                              "} " &
                          "} " &
                          "}"

        Dim requestBody = New Dictionary(Of String, String) From {{"query", query}}
        Using client As New HttpClient()
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " & _apiToken)
            client.DefaultRequestHeaders.Add("API-Version", "2023-10")

            Dim content = New StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            Dim response = client.PostAsync("https://api.monday.com/v2", content).Result
            response.EnsureSuccessStatusCode()

            Dim rawJson As String = response.Content.ReadAsStringAsync().Result

            ' Now filter locally — this is the ONLY reliable way to get partial matches
            Dim allItems As List(Of MondayBoardItem) = ParseBoardItems(rawJson)

            Dim lowerSearch As String = safeText.ToLowerInvariant()
            Dim results As New List(Of MondayBoardItem)

            For Each item As MondayBoardItem In allItems
                Dim found As Boolean = False

                ' Search in item name
                If item.Name IsNot Nothing AndAlso item.Name.ToLowerInvariant().Contains(lowerSearch) Then
                    found = True
                Else
                    ' Search in any column text
                    For Each kvp In item.ColumnValues
                        If kvp.Value IsNot Nothing AndAlso kvp.Value.ToLowerInvariant().Contains(lowerSearch) Then
                            found = True
                            Exit For
                        End If
                    Next
                End If

                If found Then results.Add(item)
            Next

            Return results
        End Using
    End Function


    Private Function ParseBoardItems(json As String) As List(Of MondayBoardItem)
        Dim items As New List(Of MondayBoardItem)
        Try
            Dim jsonDoc As JsonDocument = JsonDocument.Parse(json)
            Dim boardItems As JsonElement = jsonDoc.RootElement.GetProperty("data").GetProperty("boards")(0).GetProperty("items_page").GetProperty("items")

            For Each item As JsonElement In boardItems.EnumerateArray()
                Dim boardItem As New MondayBoardItem With {
                .ItemID = item.GetProperty("id").GetString(),
                .Name = item.GetProperty("name").GetString(),
                .ColumnValues = New Dictionary(Of String, String)
            }

                For Each colVal As JsonElement In item.GetProperty("column_values").EnumerateArray()
                    Dim colId As String = colVal.GetProperty("id").GetString()
                    ' Fixed: Use JsonElement for out param, then GetString() with null handling
                    Dim textElement As JsonElement
                    If colVal.TryGetProperty("text", textElement) AndAlso textElement.ValueKind <> JsonValueKind.Null Then
                        boardItem.ColumnValues.Add(colId, textElement.GetString())
                    Else
                        boardItem.ColumnValues.Add(colId, String.Empty)
                    End If
                Next

                items.Add(boardItem)
            Next
        Catch ex As Exception
            Throw New Exception("Error parsing monday.com response: " & ex.Message)
        End Try
        Return items
    End Function

    Public Shared Function GetMondayItemUrl(itemId As String) As String
        Const BoardId As String = "6930311385"           ' your board
        Const Subdomain As String = "builderswarehouse"       ' ← CHANGE THIS to your actual subdomain
        Return $"https://{Subdomain}.monday.com/boards/{BoardId}/pulses/{itemId}"
    End Function

End Class

