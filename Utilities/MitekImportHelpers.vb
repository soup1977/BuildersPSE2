' =====================================================
' MitekImportHelpers.vb
' Helper classes for smart MiTek CSV import
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Text.RegularExpressions

Namespace Forms.PriceLock

    ''' <summary>
    ''' Provides fuzzy string matching and text normalization utilities
    ''' </summary>
    Public Class FuzzyMatcher

        ''' <summary>
        ''' Calculates Levenshtein distance between two strings
        ''' </summary>
        Public Shared Function LevenshteinDistance(s As String, t As String) As Integer
            If String.IsNullOrEmpty(s) Then Return If(t?.Length, 0)
            If String.IsNullOrEmpty(t) Then Return s.Length

            Dim n = s.Length
            Dim m = t.Length
            Dim d(n, m) As Integer

            For i = 0 To n
                d(i, 0) = i
            Next
            For j = 0 To m
                d(0, j) = j
            Next

            For i = 1 To n
                For j = 1 To m
                    Dim cost = If(s(i - 1) = t(j - 1), 0, 1)
                    d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1), d(i - 1, j - 1) + cost)
                Next
            Next

            Return d(n, m)
        End Function

        ''' <summary>
        ''' Calculates similarity ratio (0 to 1) between two strings
        ''' </summary>
        Public Shared Function SimilarityRatio(s1 As String, s2 As String) As Double
            If String.IsNullOrEmpty(s1) AndAlso String.IsNullOrEmpty(s2) Then Return 1.0
            If String.IsNullOrEmpty(s1) OrElse String.IsNullOrEmpty(s2) Then Return 0.0

            Dim maxLen = Math.Max(s1.Length, s2.Length)
            If maxLen = 0 Then Return 1.0

            Dim distance = LevenshteinDistance(s1.ToUpperInvariant(), s2.ToUpperInvariant())
            Return 1.0 - (distance / CDbl(maxLen))
        End Function

        ''' <summary>
        ''' Normalizes an option name for comparison (removes Opt prefix/suffix, trims, etc.)
        ''' Used ONLY for comparison, not for storage
        ''' </summary>
        Public Shared Function NormalizeForComparison(optionName As String) As String
            If String.IsNullOrWhiteSpace(optionName) Then Return ""

            Dim normalized = optionName.Trim()

            ' Remove "Opt " prefix
            normalized = Regex.Replace(normalized, "^Opt\.?\s+", "", RegexOptions.IgnoreCase)

            ' Remove " Opt" suffix
            normalized = Regex.Replace(normalized, "\s+Opt\.?$", "", RegexOptions.IgnoreCase)

            ' Normalize whitespace
            normalized = Regex.Replace(normalized, "\s+", " ")

            Return normalized.Trim()
        End Function

        ''' <summary>
        ''' Finds potential matches for an option name from a list of existing options
        ''' Returns matches with similarity >= threshold
        ''' </summary>
        Public Shared Function FindSimilarOptions(optionName As String, existingOptions As IEnumerable(Of String), threshold As Double) As List(Of OptionMatch)
            Dim matches As New List(Of OptionMatch)()
            Dim normalizedInput = NormalizeForComparison(optionName)

            For Each existing In existingOptions
                Dim normalizedExisting = NormalizeForComparison(existing)

                ' Check exact match (case-insensitive, normalized)
                If normalizedInput.Equals(normalizedExisting, StringComparison.OrdinalIgnoreCase) Then
                    matches.Add(New OptionMatch() With {
                        .OriginalName = existing,
                        .Similarity = 1.0,
                        .IsExactMatch = True
                    })
                    Continue For
                End If

                ' Check fuzzy match
                Dim similarity = SimilarityRatio(normalizedInput, normalizedExisting)
                If similarity >= threshold Then
                    matches.Add(New OptionMatch() With {
                        .OriginalName = existing,
                        .Similarity = similarity,
                        .IsExactMatch = False
                    })
                End If
            Next

            Return matches.OrderByDescending(Function(m) m.Similarity).ToList()
        End Function
    End Class

    ''' <summary>
    ''' Represents a potential option match
    ''' </summary>
    Public Class OptionMatch
        Public Property OriginalName As String
        Public Property Similarity As Double
        Public Property IsExactMatch As Boolean

        Public ReadOnly Property SimilarityPercent As String
            Get
                Return $"{Similarity:P0}"
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Represents a resolved option mapping (from CSV name to database option)
    ''' </summary>
    Public Class OptionMapping
        Public Property CsvOptionName As String           ' Original name from CSV
        Public Property ResolvedOptionName As String      ' Name to use (might be existing option)
        Public Property ResolvedOptionID As Integer?      ' ID if mapping to existing option
        Public Property IsNewOption As Boolean            ' True if creating new
        Public Property UserConfirmed As Boolean          ' True if user confirmed this mapping
        Public Property OccurrenceCount As Integer        ' How many times this appears in CSV
    End Class

    ''' <summary>
    ''' Utilities for normalizing elevation and description text
    ''' </summary>
    Public Class TextNormalizer

        ''' <summary>
        ''' Normalizes elevation name (trims whitespace, standardizes dash separator)
        ''' </summary>
        Public Shared Function NormalizeElevationName(elevationName As String) As String
            If String.IsNullOrWhiteSpace(elevationName) Then Return ""

            Dim normalized = elevationName.Trim()

            ' Standardize dash separator to " - " (space-dash-space)
            ' Handle cases like "Elev C -Option" or "Elev C- Option" or "Elev C-Option"
            normalized = Regex.Replace(normalized, "\s*-\s*", " - ")

            ' Remove extra whitespace
            normalized = Regex.Replace(normalized, "\s+", " ")

            Return normalized.Trim()
        End Function

        ''' <summary>
        ''' Extracts the base elevation from a full description
        ''' e.g., "Elev A - Outdoor Room Opt" -> "Elev A"
        ''' </summary>
        Public Shared Function ExtractBaseElevation(fullDescription As String) As String
            If String.IsNullOrWhiteSpace(fullDescription) Then Return ""

            Dim normalized = NormalizeElevationName(fullDescription)

            ' Look for " - " separator
            Dim dashIndex = normalized.IndexOf(" - ", StringComparison.Ordinal)
            If dashIndex > 0 Then
                Return normalized.Substring(0, dashIndex).Trim()
            End If

            Return normalized
        End Function

        ''' <summary>
        ''' Extracts the option part from a full description
        ''' e.g., "Elev A - Outdoor Room Opt" -> "Outdoor Room Opt"
        ''' </summary>
        Public Shared Function ExtractOptionPart(fullDescription As String) As String
            If String.IsNullOrWhiteSpace(fullDescription) Then Return Nothing

            Dim normalized = NormalizeElevationName(fullDescription)

            ' Look for " - " separator
            Dim dashIndex = normalized.IndexOf(" - ", StringComparison.Ordinal)
            If dashIndex > 0 AndAlso dashIndex + 3 < normalized.Length Then
                Return normalized.Substring(dashIndex + 3).Trim()
            End If

            Return Nothing ' No option part
        End Function
    End Class

End Namespace
