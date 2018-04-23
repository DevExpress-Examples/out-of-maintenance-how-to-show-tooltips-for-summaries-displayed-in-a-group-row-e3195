Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.Utils
Imports System.Collections
Imports DevExpress.XtraGrid

Namespace AlignSummaries
	Public Class GridViewGroupToolTipController
		Private view_Renamed As GridView
		Private tooltip_Renamed As ToolTipController
		Private groupRowSummary As GroupRowSummaryHelper
		Private hitInfo As GridHitInfo

		'Use Default Tooltip controller
		Public Sub New(ByVal view As GridView)
			Me.New(view, Nothing, Nothing)
		End Sub
		Public Sub New(ByVal view As GridView, ByVal grRowSummary As GroupRowSummaryHelper, ByVal tooltip As ToolTipController)
			Me.view_Renamed = view
			Me.tooltip_Renamed = tooltip
			groupRowSummary = grRowSummary
			AddHandler Me.View.MouseLeave, AddressOf OnView_MouseLeave
			AddHandler Me.View.MouseMove, AddressOf OnView_MouseMove
		End Sub

		Private Sub OnView_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			hitInfo = View.CalcHitInfo(e.X, e.Y)
			If (Not View.IsGroupRow(hitInfo.RowHandle)) Then
				ToolTip.HideHint()
			Else
				Dim hoveredColumn As GridColumn = GetHoveredColumn()
				Dim toolTipText As String = GetToolTipText(hoveredColumn)
				ToolTip.ShowHint(New DevExpress.Utils.ToolTipControlInfo(toolTipText, toolTipText))
			End If
		End Sub

		Private Function GetToolTipText(ByVal column As GridColumn) As String
			Dim text As String = ""
			If column Is Nothing Then
				Return text
			End If
			Dim summaryValues As Hashtable = view_Renamed.GetGroupSummaryValues(hitInfo.RowHandle)
			For Each itemObject As DictionaryEntry In summaryValues
				Dim item As GridSummaryItem = TryCast(itemObject.Key, GridSummaryItem)
				If item.FieldName = column.FieldName Then
					text = item.GetDisplayText(summaryValues(item), False)
					Exit For
				End If
			Next itemObject
			Return text
		End Function

		Private Function GetHoveredColumn() As GridColumn
			Dim columnBounds As Rectangle
			Dim column As GridColumn = Nothing
			If groupRowSummary Is Nothing Then
				Return column
			End If
			For Each gc As GridColumn In view_Renamed.Columns
				columnBounds = groupRowSummary.GetColumnBounds(gc)
				If hitInfo.HitPoint.X >= columnBounds.X AndAlso hitInfo.HitPoint.X <= columnBounds.X + columnBounds.Width Then
					column = gc
					Exit For
				End If
			Next gc
			Return column
		End Function

		Private Sub OnView_MouseLeave(ByVal sender As Object, ByVal e As EventArgs)
			ToolTip.HideHint()
		End Sub
		Public ReadOnly Property View() As GridView
			Get
				Return view_Renamed
			End Get
		End Property
		Protected ReadOnly Property ToolTip() As ToolTipController
			Get
				If tooltip_Renamed IsNot Nothing Then
					Return tooltip_Renamed
				Else
					Return ToolTipController.DefaultController
				End If
			End Get
		End Property
	End Class
End Namespace
