' =====================================================
' PriceLockModels.vb
' Data Model Classes for PriceLock Module
' BuildersPSE2 Extension
' =====================================================

Option Strict On
Option Explicit On

Namespace Models

#Region "Builder"

    ''' <summary>
    ''' Represents a production builder (e.g., DR Horton, Trumark)
    ''' </summary>
    Public Class PLBuilder
        Public Property BuilderID As Integer
        Public Property BuilderName As String
        Public Property BuilderCode As String
        Public Property IsActive As Boolean
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime

        Public Sub New()
            IsActive = True
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
        End Sub

        Public Overrides Function ToString() As String
            Return BuilderName
        End Function
    End Class

#End Region

#Region "Subdivision"

    ''' <summary>
    ''' Represents a subdivision belonging to a builder
    ''' </summary>
    Public Class PLSubdivision
        Public Property SubdivisionID As Integer
        Public Property BuilderID As Integer
        Public Property SubdivisionName As String
        Public Property SubdivisionCode As String
        Public Property IsActive As Boolean
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime

        ' Navigation/display properties
        Public Property BuilderName As String

        Public Sub New()
            IsActive = True
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
        End Sub

        Public Overrides Function ToString() As String
            Return SubdivisionName
        End Function

        ''' <summary>Display name with code if available</summary>
        Public ReadOnly Property DisplayName As String
            Get
                If String.IsNullOrEmpty(SubdivisionCode) Then
                    Return SubdivisionName
                Else
                    Return $"{SubdivisionName} ({SubdivisionCode})"
                End If
            End Get
        End Property
    End Class

#End Region

#Region "Plan"

    ''' <summary>
    ''' Represents a house plan (can be shared across subdivisions)
    ''' </summary>
    Public Class PLPlan
        Public Property PlanID As Integer
        Public Property PlanName As String
        Public Property PlanDescription As String
        Public Property SquareFootage As Integer?
        Public Property IsActive As Boolean
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime

        ' For subdivision assignment tracking
        Public Property SubdivisionPlanID As Integer?

        Public Sub New()
            IsActive = True
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
        End Sub

        Public Overrides Function ToString() As String
            Return PlanName
        End Function
    End Class

#End Region

#Region "Elevation"

    ''' <summary>
    ''' Represents an elevation variant of a plan
    ''' </summary>
    Public Class PLElevation
        Public Property ElevationID As Integer
        Public Property PlanID As Integer
        Public Property ElevationName As String
        Public Property IsActive As Boolean
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime

        Public Sub New()
            IsActive = True
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
        End Sub

        Public Overrides Function ToString() As String
            Return ElevationName
        End Function
    End Class

#End Region

#Region "Option"

    ''' <summary>
    ''' Represents a structural option (e.g., Oversized 2 Car, Outdoor Room)
    ''' </summary>
    Public Class PLOption
        Public Property OptionID As Integer
        Public Property OptionName As String
        Public Property OptionDescription As String
        Public Property IsActive As Boolean
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime

        Public Sub New()
            IsActive = True
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
        End Sub

        Public Overrides Function ToString() As String
            Return OptionName
        End Function
    End Class

#End Region

#Region "Product Type"

    ''' <summary>
    ''' Represents a product type (Floor, Roof, Wall)
    ''' </summary>
    Public Class PLProductType
        Public Property ProductTypeID As Integer
        Public Property ProductTypeName As String

        Public Overrides Function ToString() As String
            Return ProductTypeName
        End Function
    End Class

#End Region

#Region "Material Category"

    ''' <summary>
    ''' Represents a material category for pricing (e.g., STUDS, OSB, EWP)
    ''' </summary>
    Public Class PLMaterialCategory
        Public Property MaterialCategoryID As Integer
        Public Property CategoryName As String
        Public Property CategoryCode As String
        Public Property DisplayOrder As Integer
        Public Property IsActive As Boolean
        Public Property CreatedDate As DateTime

        Public Sub New()
            IsActive = True
            CreatedDate = DateTime.Now
        End Sub

        Public Overrides Function ToString() As String
            Return CategoryName
        End Function
    End Class

#End Region

#Region "Price Lock"

    ''' <summary>
    ''' Represents a price lock header for a subdivision at a specific date
    ''' </summary>
    Public Class PLPriceLock
        Public Property PriceLockID As Integer
        Public Property SubdivisionID As Integer
        Public Property PriceLockDate As Date
        Public Property PriceLockName As String

        ' Margin settings
        Public Property BaseMgmtMargin As Decimal?
        Public Property AdjustedMarginBaseModels As Decimal?
        Public Property OptionMargin As Decimal?

        ' Status tracking
        Public Property Status As String
        Public Property CreatedBy As String
        Public Property ApprovedBy As String
        Public Property ApprovedDate As DateTime?
        Public Property SentToBuilderDate As DateTime?

        ' Audit
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime

        ' Navigation/display properties
        Public Property SubdivisionName As String
        Public Property SubdivisionCode As String
        Public Property BuilderID As Integer
        Public Property BuilderName As String

        ' Child collections (loaded separately)
        Public Property ComponentPricing As List(Of PLComponentPricing)
        Public Property MaterialPricing As List(Of PLMaterialPricing)

        ' Random Lengths Import References
        Public Property BaselineRLImportID As Integer?
        Public Property CurrentRLImportID As Integer?

        ' Navigation properties for RL imports (loaded separately if needed)
        Public Property BaselineRLReportDate As Date?
        Public Property BaselineRLReportName As String
        Public Property CurrentRLReportDate As Date?
        Public Property CurrentRLReportName As String

        ''' <summary>Check if this price lock has both RL imports configured for calculations</summary>
        Public ReadOnly Property HasRLImportsConfigured As Boolean
            Get
                Return BaselineRLImportID.HasValue AndAlso CurrentRLImportID.HasValue
            End Get
        End Property


        Public Sub New()
            Status = "Draft"
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
            ComponentPricing = New List(Of PLComponentPricing)()
            MaterialPricing = New List(Of PLMaterialPricing)()
        End Sub

        Public Overrides Function ToString() As String
            Return $"{SubdivisionName} - {PriceLockDate:d}"
        End Function

        ''' <summary>Check if price lock can be edited</summary>
        Public ReadOnly Property CanEdit As Boolean
            Get
                Return Status = "Draft" OrElse Status = "Pending"
            End Get
        End Property

        ''' <summary>Check if price lock can be deleted</summary>
        Public ReadOnly Property CanDelete As Boolean
            Get
                Return Status = "Draft"
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Price lock status constants
    ''' </summary>
    Public NotInheritable Class PLPriceLockStatus
        Public Const Draft As String = "Draft"
        Public Const Pending As String = "Pending"
        Public Const Approved As String = "Approved"
        Public Const Sent As String = "Sent"

        Private Sub New()
        End Sub
    End Class

#End Region

#Region "Component Pricing"

    ''' <summary>
    ''' Represents component pricing for a plan/elevation/option combination
    ''' </summary>
    Public Class PLComponentPricing
        Public Property ComponentPricingID As Integer
        Public Property PriceLockID As Integer
        Public Property PlanID As Integer
        Public Property ElevationID As Integer
        Public Property OptionID As Integer?
        Public Property ProductTypeID As Integer
        Public Property Cost As Decimal?
        Public Property TrueCost As Decimal? ' For import compatibility
        Public Property MgmtSellPrice As Decimal?
        Public Property CalculatedPrice As Decimal?
        Public Property AppliedMargin As Decimal?
        Public Property FinalPrice As Decimal?
        Public Property PriceSentToSales As Decimal?
        Public Property PriceSentToBuilder As Decimal?
        Public Property IsAdder As Boolean
        Public Property BaseElevationID As Integer?
        Public Property PriceNote As String
        Public Property ModifiedBy As String
        Public Property ModifiedDate As DateTime
        Public Property CreatedDate As DateTime
        Public Property MarginName As String
        Public Property FinalMarginPct As Decimal
        Public Property BuilderMarginPct As Decimal

        ' NEW: Reference unit for adder pricing
        Public Property BaseComponentPricingID As Integer?

        ' NEW: Indicates which margin type applies to this record
        Public Property MarginSource As String = "Adjusted" ' "Adjusted" or "Option"

        ' Display properties (joined from other tables)
        Public Property PlanName As String
        Public Property ElevationName As String
        Public Property OptionName As String
        Public Property ProductTypeName As String

        ' NEW: Display name of the reference unit
        Public Property BaseComponentDescription As String

        ' NEW: Cost of the reference unit (for adder calculation display)
        Public Property BaseComponentCost As Decimal?

        ' Computed properties
        Public ReadOnly Property HasPriceDifference As Boolean
            Get
                If Not FinalPrice.HasValue OrElse Not PriceSentToBuilder.HasValue Then Return False
                Return FinalPrice.Value <> PriceSentToBuilder.Value
            End Get
        End Property

        Public ReadOnly Property FullDescription As String
            Get
                Dim desc = $"{PlanName} - {ElevationName}"
                If Not String.IsNullOrEmpty(OptionName) Then
                    desc &= $" - {OptionName}"
                End If
                desc &= $" ({ProductTypeName})"
                Return desc
            End Get
        End Property

        ''' <summary>
        ''' Determines if this record should use Option Margin
        ''' </summary>
        Public ReadOnly Property UsesOptionMargin As Boolean
            Get
                ' Options always use Option Margin
                If OptionID.HasValue AndAlso OptionID.Value > 0 Then Return True

                ' If MarginSource explicitly set to "Option", use it
                If MarginSource = "Option" Then Return True

                ' Otherwise uses Adjusted Margin
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Calculates the adder cost (difference between this unit and reference unit)
        ''' Returns Nothing if not an adder or no reference unit set
        ''' </summary>
        Public ReadOnly Property AdderCost As Decimal?
            Get
                If Not IsAdder Then Return Nothing
                If Not Cost.HasValue Then Return Nothing
                If Not BaseComponentCost.HasValue Then Return Cost ' No reference, use full cost

                Dim diff = Cost.Value - BaseComponentCost.Value
                Return If(diff > 0, diff, 0D) ' Don't allow negative adders
            End Get
        End Property
    End Class

#End Region

#Region "Reference Unit (for Adders)"

    ''' <summary>
    ''' Represents a potential reference unit for adder pricing
    ''' </summary>
    Public Class PLReferenceUnit
        Public Property ComponentPricingID As Integer
        Public Property PlanID As Integer
        Public Property ElevationID As Integer
        Public Property ProductTypeID As Integer
        Public Property Cost As Decimal?
        Public Property FinalPrice As Decimal?
        Public Property PlanName As String
        Public Property ElevationName As String
        Public Property ProductTypeName As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return Description
        End Function
    End Class

#End Region

#Region "Material Pricing"

    ''' <summary>
    ''' Represents material pricing for a category within a price lock
    ''' </summary>
    Public Class PLMaterialPricing
        Public Property MaterialPricingID As Integer
        Public Property PriceLockID As Integer
        Public Property MaterialCategoryID As Integer

        ' Random Lengths source data
        Public Property RandomLengthsDate As Date?
        Public Property RandomLengthsPrice As Decimal?

        ' Three-part tracking (CalculatedPrice = PriceSentToSales always)
        Public Property CalculatedPrice As Decimal?
        Public Property PriceSentToSales As Decimal?
        Public Property PriceSentToBuilder As Decimal?

        ' Change tracking
        Public Property PctChangeFromPrevious As Decimal?

        ' Note for price differences
        Public Property PriceNote As String

        ' =====================================================
        ' ADD TO PLMaterialPricing CLASS (around line 424, after PriceNote)
        ' =====================================================

        ' RL-based calculation fields
        Public Property BaselineRLPrice As Decimal?
        Public Property CurrentRLPrice As Decimal?
        Public Property OriginalSellPrice As Decimal?
        Public Property RLPercentChange As Decimal?

        ''' <summary>Formatted RL percent change</summary>
        Public ReadOnly Property RLPercentChangeFormatted As String
            Get
                If Not RLPercentChange.HasValue Then Return "--"
                Return RLPercentChange.Value.ToString("P2")
            End Get
        End Property

        ''' <summary>Check if this pricing has RL-based calculation data</summary>
        Public ReadOnly Property HasRLCalculation As Boolean
            Get
                Return BaselineRLPrice.HasValue AndAlso CurrentRLPrice.HasValue
            End Get
        End Property

        ''' <summary>The RL price difference (Current - Baseline)</summary>
        Public ReadOnly Property RLPriceDifference As Decimal?
            Get
                If Not BaselineRLPrice.HasValue OrElse Not CurrentRLPrice.HasValue Then Return Nothing
                Return CurrentRLPrice.Value - BaselineRLPrice.Value
            End Get
        End Property


        ' Audit
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime
        Public Property ModifiedBy As String

        ' Navigation/display properties
        Public Property CategoryName As String
        Public Property CategoryCode As String
        Public Property DisplayOrder As Integer

        Public Sub New()
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
        End Sub

        ''' <summary>Check if builder price differs from calculated price</summary>
        Public ReadOnly Property HasPriceDifference As Boolean
            Get
                If Not CalculatedPrice.HasValue OrElse Not PriceSentToBuilder.HasValue Then
                    Return False
                End If
                Return CalculatedPrice.Value <> PriceSentToBuilder.Value
            End Get
        End Property

        ''' <summary>Formatted percent change string</summary>
        Public ReadOnly Property PctChangeFormatted As String
            Get
                If Not PctChangeFromPrevious.HasValue Then
                    Return "--"
                End If
                Return PctChangeFromPrevious.Value.ToString("P1")
            End Get
        End Property
    End Class

#End Region

#Region "Price Change History"

    ''' <summary>
    ''' Represents an audit record for a price change
    ''' </summary>
    Public Class PLPriceChangeHistory
        Public Property HistoryID As Integer
        Public Property TableName As String
        Public Property RecordID As Integer
        Public Property FieldName As String
        Public Property OldValue As String
        Public Property NewValue As String
        Public Property ChangedBy As String
        Public Property ChangedDate As DateTime
        Public Property ChangeReason As String
    End Class

#End Region

#Region "Comparison Results"

    ''' <summary>
    ''' Represents a component price comparison between two locks
    ''' </summary>
    Public Class PLComponentPriceComparison
        Public Property PlanName As String
        Public Property ElevationName As String
        Public Property OptionName As String
        Public Property ProductTypeName As String
        Public Property CurrentFinalPrice As Decimal?
        Public Property CurrentBuilderPrice As Decimal?
        Public Property PreviousFinalPrice As Decimal?
        Public Property PreviousBuilderPrice As Decimal?
        Public Property FinalPriceDiff As Decimal?
        Public Property FinalPricePctChange As Decimal?
        Public Property PriceNote As String

        Public ReadOnly Property FullDescription As String
            Get
                Dim desc = $"{PlanName} {ElevationName}"
                If Not String.IsNullOrEmpty(OptionName) Then
                    desc &= $" - {OptionName}"
                End If
                Return desc
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Represents a material price comparison between two locks
    ''' </summary>
    Public Class PLMaterialPriceComparison
        Public Property CategoryName As String
        Public Property CategoryCode As String
        Public Property CurrentPrice As Decimal?
        Public Property CurrentBuilderPrice As Decimal?
        Public Property PreviousPrice As Decimal?
        Public Property PreviousBuilderPrice As Decimal?
        Public Property PriceDiff As Decimal?
        Public Property PctChangeFromPrevious As Decimal?
        Public Property PriceNote As String
    End Class

    ''' <summary>
    ''' Represents a subdivision that may need a price lock renewal
    ''' </summary>
    Public Class PLSubdivisionRenewalStatus
        Public Property SubdivisionID As Integer
        Public Property SubdivisionName As String
        Public Property SubdivisionCode As String
        Public Property BuilderID As Integer
        Public Property BuilderName As String
        Public Property PriceLockID As Integer?
        Public Property PriceLockDate As Date?
        Public Property DaysSinceLock As Integer?

        Public ReadOnly Property NeedsRenewal As Boolean
            Get
                ' Flag if more than 60 days since last lock
                Return Not DaysSinceLock.HasValue OrElse DaysSinceLock.Value > 60
            End Get
        End Property

        Public ReadOnly Property RenewalUrgency As String
            Get
                If Not DaysSinceLock.HasValue Then
                    Return "No Lock"
                ElseIf DaysSinceLock.Value > 90 Then
                    Return "Overdue"
                ElseIf DaysSinceLock.Value > 60 Then
                    Return "Due Soon"
                Else
                    Return "Current"
                End If
            End Get
        End Property
    End Class

#End Region

    ' =====================================================
    ' ALSO ADD THESE NEW CLASSES TO THE Models NAMESPACE
    ' (Add at the end of the file, before "End Namespace")
    ' =====================================================

#Region "Random Lengths Import"

    ''' <summary>
    ''' Represents a Random Lengths pricing import session.
    ''' This is GLOBAL data, not tied to any specific subdivision.
    ''' Each record represents one weekly RL pricing report.
    ''' </summary>
    Public Class PLRandomLengthsImport
        Public Property RandomLengthsImportID As Integer
        Public Property ReportDate As Date
        Public Property ReportName As String
        Public Property SourceFileName As String
        Public Property ImportMethod As String  ' "Manual", "Excel", "PDF"
        Public Property ImportedBy As String
        Public Property ImportedDate As DateTime
        Public Property Notes As String
        Public Property IsActive As Boolean

        ' Audit
        Public Property CreatedDate As DateTime
        Public Property ModifiedDate As DateTime
        Public Property ModifiedBy As String

        ' Calculated/loaded separately
        Public Property PriceCount As Integer
        Public Property Pricing As List(Of PLRandomLengthsPricing)

        Public Sub New()
            ReportDate = Date.Today
            ImportMethod = "Manual"
            ImportedBy = Environment.UserName
            ImportedDate = DateTime.Now
            IsActive = True
            CreatedDate = DateTime.Now
            ModifiedDate = DateTime.Now
            Pricing = New List(Of PLRandomLengthsPricing)()
        End Sub

        ''' <summary>Display name for combo boxes and lists</summary>
        Public ReadOnly Property DisplayName As String
            Get
                Dim name = ReportDate.ToString("MM/dd/yyyy")
                If Not String.IsNullOrEmpty(ReportName) Then
                    name &= $" - {ReportName}"
                End If
                If PriceCount > 0 Then
                    name &= $" ({PriceCount} prices)"
                End If
                Return name
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return DisplayName
        End Function
    End Class

#End Region

#Region "Random Lengths Pricing Type"

    ''' <summary>
    ''' Represents a specific pricing field from the Random Lengths report.
    ''' Examples: "2x4 #2 SPF Denver", "7/16 OSB Denver", etc.
    ''' This is lookup/reference data that rarely changes.
    ''' </summary>
    Public Class PLRandomLengthsPricingType
        Public Property RLPricingTypeID As Integer
        Public Property TypeCode As String          ' Short code like "SPF_2X4_DEN"
        Public Property TypeName As String          ' Display name like "#2 SPF Denver 2x4"
        Public Property TypeDescription As String   ' Detailed description
        Public Property CategoryGroup As String     ' High-level group: "Framing", "OSB", "Studs", etc.
        Public Property DisplayOrder As Integer
        Public Property IsActive As Boolean

        Public Sub New()
            IsActive = True
            DisplayOrder = 0
        End Sub

        Public Overrides Function ToString() As String
            Return TypeName
        End Function

        ''' <summary>Display name with category group</summary>
        Public ReadOnly Property FullDisplayName As String
            Get
                Return $"{CategoryGroup}: {TypeName}"
            End Get
        End Property
    End Class

#End Region

#Region "Random Lengths Pricing"

    ''' <summary>
    ''' Represents an actual price from a Random Lengths import.
    ''' One record per pricing type per import.
    ''' </summary>
    Public Class PLRandomLengthsPricing
        Public Property RandomLengthsPricingID As Integer
        Public Property RandomLengthsImportID As Integer
        Public Property RLPricingTypeID As Integer
        Public Property Price As Decimal
        Public Property PriceSource As String       ' Where in the RL report this came from
        Public Property Notes As String

        ' Navigation properties (loaded via JOIN)
        Public Property TypeCode As String
        Public Property TypeName As String
        Public Property CategoryGroup As String
        Public Property DisplayOrder As Integer

        Public Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return $"{TypeName}: {Price:C2}"
        End Function
    End Class

#End Region

#Region "Material Category RL Mapping"

    ''' <summary>
    ''' Maps a Material Category to one or more Random Lengths pricing types.
    ''' This defines which RL prices drive the % change for each material category.
    ''' </summary>
    Public Class PLMaterialCategoryRLMapping

        Public Property MappingID As Integer
        Public Property MaterialCategoryID As Integer
        Public Property RLPricingTypeID As Integer
        Public Property WeightFactor As Decimal
        Public Property IsPrimary As Boolean
        Public Property IsActive As Boolean

        ' Navigation properties (optional, for display purposes)
        Public Property CategoryName As String
        Public Property RLTypeCode As String
        Public Property RLTypeName As String

    End Class

#End Region

#Region "RL Price Calculation Results"

    ''' <summary>
    ''' Result of calculating % change between two RL imports for a material category
    ''' </summary>
    Public Class PLRLPriceChangeResult
        Public Property MaterialCategoryID As Integer
        Public Property CategoryName As String
        Public Property BaselineRLPrice As Decimal
        Public Property CurrentRLPrice As Decimal
        Public Property PercentChange As Decimal
        Public Property OriginalSellPrice As Decimal
        Public Property NewCalculatedPrice As Decimal

        ''' <summary>Formatted percent change string</summary>
        Public ReadOnly Property PercentChangeFormatted As String
            Get
                Return PercentChange.ToString("P2")
            End Get
        End Property

        ''' <summary>Price difference (absolute)</summary>
        Public ReadOnly Property PriceDifference As Decimal
            Get
                Return CurrentRLPrice - BaselineRLPrice
            End Get
        End Property

        ''' <summary>Sell price difference</summary>
        Public ReadOnly Property SellPriceDifference As Decimal
            Get
                Return NewCalculatedPrice - OriginalSellPrice
            End Get
        End Property
    End Class

#End Region

End Namespace