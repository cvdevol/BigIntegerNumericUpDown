Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Numerics
Imports System.Globalization
Imports System.Windows.Media.TextFormatting
Imports System.Text.RegularExpressions

Public Class BigIntegerUpDown
	Inherits UserControl
	Implements INotifyPropertyChanged

#Region "Fields"

	Public Delegate Sub ValueChangedEventHandler(sender As Object, e As ValueChangedEventArgs)
	Public Event ValueChanged As ValueChangedEventHandler

	Public Custom Event ValChanged As ValueChangedEventHandler
		AddHandler(ByVal value As ValueChangedEventHandler)

		End AddHandler

		RemoveHandler(ByVal value As ValueChangedEventHandler)

		End RemoveHandler

		RaiseEvent(ByVal sender As Object, ByVal e As ValueChangedEventArgs)

		End RaiseEvent
	End Event

	Protected Overridable Sub OnValChanged(ByVal e As ValueChangedEventArgs)
		RaiseEvent ValChanged(Me, e)
	End Sub



	Private Event INotifyPropertyChanged_PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

	Public Sub NotifyPropertyChanged(ByVal info As String)
		Select Case info
			Case "Value"
				RaiseEvent ValueChanged(Me, New ValueChangedEventArgs(Value))
				TB.Tag = Value
				TB.Text = Value.ToString("N0")

		End Select

	End Sub

#End Region

	Public Sub New()
		InitializeComponent()
		Me.DataContext = Me

	End Sub

#Region "ValueProperty"

	Public Shared ValueProperty As DependencyProperty = DependencyProperty.RegisterAttached("Value", GetType(BigInteger), GetType(BigIntegerUpDown))

	Public Property Value() As BigInteger
		Get
			Return GetValue(ValueProperty)
		End Get
		Set(ByVal value As BigInteger)
			If value < MinValue Then
				value = MinValue
			End If
			If value > MaxValue Then
				value = MaxValue
			End If
			SetValue(ValueProperty, value)
			NotifyPropertyChanged("Value")

		End Set
	End Property


#End Region

#Region "StepProperty"

	Public Shared SmallStepProperty As DependencyProperty = DependencyProperty.RegisterAttached("SmallStep", GetType(BigInteger), GetType(BigIntegerUpDown), New PropertyMetadata(BigInteger.One))

	Public Property SmallStep() As BigInteger
		Get
			Return GetValue(SmallStepProperty) 'Dec(GetValue(StepProperty))
		End Get
		Set(ByVal value As BigInteger)
			SetValue(SmallStepProperty, value)
		End Set
	End Property

	Public Shared BigStepProperty As DependencyProperty = DependencyProperty.RegisterAttached("BigStep", GetType(BigInteger), GetType(BigIntegerUpDown), New PropertyMetadata(BigInteger.Parse("100")))

	Public Property BigStep() As BigInteger
		Get
			Return GetValue(BigStepProperty) 'Dec(GetValue(BigStepProperty))
		End Get
		Set(ByVal value As BigInteger)
			SetValue(BigStepProperty, value)
		End Set
	End Property

#End Region

#Region "MinValueProperty"

	Public Shared MinValueProperty As DependencyProperty = DependencyProperty.RegisterAttached("MinValue", GetType(BigInteger), GetType(BigIntegerUpDown), New PropertyMetadata(BigInteger.Zero))

	Public Property MinValue() As BigInteger
		Get
			Return GetValue(MinValueProperty) 'CDec(GetValue(MinValueProperty))
		End Get
		Set(ByVal value As BigInteger)
			If value > MaxValue Then
				MaxValue = value
			End If
			SetValue(MinValueProperty, value)
		End Set
	End Property

#End Region

#Region "MaxValueProperty"

	Public Shared MaxValueProperty As DependencyProperty = DependencyProperty.RegisterAttached("MaxValue", GetType(BigInteger), GetType(BigIntegerUpDown), New PropertyMetadata(BigInteger.Parse("1000000000000000000000000000000")))

	Public Property MaxValue() As BigInteger
		Get
			Return GetValue(MaxValueProperty) 'CDec(GetValue(MaxValueProperty))
		End Get
		Set(ByVal value As BigInteger)
			If value < MinValue Then
				value = MinValue
			End If
			SetValue(MaxValueProperty, value)
		End Set
	End Property

#End Region

#Region "Up/Down"

	Private Sub cmdUp_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		If Keyboard.Modifiers = ModifierKeys.Control Then
			Value += BigStep
		Else
			Value += SmallStep
		End If
		NotifyPropertyChanged("Value")
	End Sub

	Private Sub cmdDown_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		If Keyboard.Modifiers = ModifierKeys.Control Then
			Value -= BigStep
		Else
			Value -= SmallStep
		End If
		NotifyPropertyChanged("Value")
	End Sub



	Private Sub TB_MouseWheel(sender As Object, e As MouseWheelEventArgs)
		If e.Delta > 0 Then
			If Keyboard.Modifiers = ModifierKeys.Control Then
				Value += BigStep
			Else
				Value += SmallStep
			End If
			NotifyPropertyChanged("Value")
		ElseIf e.Delta < 0 Then
			If Keyboard.Modifiers = ModifierKeys.Control Then
				Value -= BigStep
			Else
				Value -= SmallStep
			End If
			NotifyPropertyChanged("Value")
		End If
	End Sub

#End Region




	Private Sub TB_TextChanged(sender As Object, e As TextChangedEventArgs)
		NotifyPropertyChanged("Value")
	End Sub

End Class

Public Class ValueChangedEventArgs
	Inherits EventArgs

	Public Property NewValue As BigInteger

	Public Sub New(ByVal value As BigInteger)
		NewValue = value
	End Sub

	Public Overrides Function ToString() As String
		Return NewValue.ToString("N0")
	End Function

End Class

Public Class BigIntToStringConverter
	Implements IValueConverter

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If TypeOf value Is BigInteger Then
			Return Strings.FormatNumber(value.ToString(), 0, TriState.UseDefault, TriState.False, TriState.True)
		End If
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Return BigInteger.Parse(value, NumberStyles.AllowThousands Or NumberStyles.Integer Or NumberStyles.AllowLeadingSign)
	End Function



End Class

