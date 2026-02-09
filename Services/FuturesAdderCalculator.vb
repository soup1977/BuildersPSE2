Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Services

''' <summary>
''' Calculates futures adder values based on contract prices and project data.
''' </summary>
Public Class FuturesAdderCalculator
    Private ReadOnly _versionID As Integer
    Private ReadOnly _rollupService As RollupCalculationService

    Public Sub New(versionID As Integer, rollupService As RollupCalculationService)
        _versionID = versionID
        _rollupService = rollupService
    End Sub

    ''' <summary>
    ''' Result of a futures adder calculation.
    ''' </summary>
    Public Class FuturesAdderResult
        Public Property FuturesPrice As Decimal
        Public Property CurrentSPF As Decimal
        Public Property FuturesAdderShipping As Decimal
        Public Property FuturesAdderAmt As Decimal
        Public Property ExtendedBdft As Decimal
        Public Property FuturesAdderProjTotal As Decimal
        Public Property ErrorMessage As String
        Public Property Success As Boolean = True

        ''' <summary>
        ''' Generates a confirmation message for user display.
        ''' </summary>
        Public Function GetConfirmationMessage(contractMonth As String) As String
            Return $"Calculate Futures Adder based on {contractMonth}?" & vbCrLf & vbCrLf &
                   $"Futures Price: {FuturesPrice:N2}/MBF" & vbCrLf &
                   $"Current Active SPF#2: {CurrentSPF:N2}/MBF" & vbCrLf &
                   $"Futures Adder Shipping: {FuturesAdderShipping:N2}/MBF" & vbCrLf &
                   $"Futures Adder/MBF: {FuturesAdderAmt:N2}" & vbCrLf &
                   $"Extended BDFT: {ExtendedBdft:N0}" & vbCrLf &
                   $"Futures Adder Total: {FuturesAdderProjTotal:C2}"
        End Function

        ''' <summary>
        ''' Generates a success message after saving.
        ''' </summary>
        Public Function GetSuccessMessage() As String
            Return $"Futures Adder values saved successfully!" & vbCrLf & vbCrLf &
                   $"Adder/MBF: {FuturesAdderAmt:N2}" & vbCrLf &
                   $"Project Total: {FuturesAdderProjTotal:C2}"
        End Function
    End Class

    ''' <summary>
    ''' Calculates futures adder values based on the given futures price and project data.
    ''' </summary>
    ''' <param name="futuresPrice">The settle price from the selected futures contract.</param>
    ''' <param name="project">The current project model.</param>
    ''' <param name="version">The current version model.</param>
    ''' <returns>A FuturesAdderResult containing calculated values or error information.</returns>
    Public Function Calculate(futuresPrice As Decimal, project As ProjectModel, version As ProjectVersionModel) As FuturesAdderResult
        Dim result As New FuturesAdderResult With {.FuturesPrice = futuresPrice}

        ' Get current active SPF#2 prices for Floor and Roof
        Dim floorSPF = LumberDataAccess.GetActiveSPFNo2ByProductType(_versionID, "Floor")
        Dim roofSPF = LumberDataAccess.GetActiveSPFNo2ByProductType(_versionID, "Roof")

        result.CurrentSPF = GetAverageSPF(floorSPF, roofSPF)
        If result.CurrentSPF <= 0 Then
            result.Success = False
            result.ErrorMessage = "No active SPF#2 price found for this version. Please update lumber prices first."
            Return result
        End If

        ' Get shipping config and calculate adder amount
        result.FuturesAdderShipping = ProjectDataAccess.GetConfigValue("FuturesAdderShipping")
        result.FuturesAdderAmt = (futuresPrice - result.CurrentSPF) + result.FuturesAdderShipping

        ' Calculate project rollup to get ExtendedBdft
        Dim rollupResult = _rollupService.CalculateProjectRollup(project, version)
        If Not rollupResult.HasData OrElse rollupResult.ExtendedBdft <= 0 Then
            result.Success = False
            result.ErrorMessage = "No BDFT data available in project rollup. Add buildings and levels first."
            Return result
        End If

        result.ExtendedBdft = rollupResult.ExtendedBdft
        result.FuturesAdderProjTotal = result.FuturesAdderAmt * (result.ExtendedBdft / 1000D)

        Return result
    End Function

    ''' <summary>
    ''' Gets the average SPF price, handling cases where one or both values may be zero.
    ''' </summary>
    Private Shared Function GetAverageSPF(floorSPF As Decimal, roofSPF As Decimal) As Decimal
        If floorSPF > 0 AndAlso roofSPF > 0 Then
            Return (floorSPF + roofSPF) / 2
        ElseIf floorSPF > 0 Then
            Return floorSPF
        ElseIf roofSPF > 0 Then
            Return roofSPF
        Else
            Return 0
        End If
    End Function
End Class
