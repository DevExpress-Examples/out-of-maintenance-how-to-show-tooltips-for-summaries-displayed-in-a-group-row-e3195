Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.XtraGrid.Views.Grid
Imports System.Collections
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid
Imports System.Drawing
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Drawing
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.Data
Imports DevExpress.Utils.Text

Namespace AlignSummaries
	Public Class GroupRowSummaryHelper
		Public Function ExtractSummaryItems(ByVal view As GridView) As ArrayList
			Dim items As New ArrayList()
			For Each si As GridSummaryItem In view.GroupSummary
				If TypeOf si Is GridGroupSummaryItem AndAlso si.SummaryType <> SummaryItemType.None Then
					items.Add(si)
				End If
			Next si
			Return items
		End Function

		Public Sub DrawBackground(ByVal e As RowObjectCustomDrawEventArgs, ByVal view As GridView)
			Dim painter As GridGroupRowPainter
			Dim info As GridGroupRowInfo
			painter = TryCast(e.Painter, GridGroupRowPainter)
			info = TryCast(e.Info, GridGroupRowInfo)
			Dim level As Integer = view.GetRowLevel(e.RowHandle)
			Dim row As Integer = view.GetDataRowHandleByGroupRowHandle(e.RowHandle)
			info.GroupText = String.Format("{0}: {1}", view.GroupedColumns(level).Caption, view.GetRowCellDisplayText(row, view.GroupedColumns(level)))
			e.Appearance.DrawBackground(e.Cache, info.Bounds)
			painter.ElementsPainter.GroupRow.DrawObject(info)
		End Sub

		Public Sub DrawSummaryValues(ByVal e As RowObjectCustomDrawEventArgs, ByVal view As GridView, ByVal items As ArrayList)
			Dim values As Hashtable = view.GetGroupSummaryValues(e.RowHandle)
			For Each item As GridGroupSummaryItem In items
				Dim rect As Rectangle = GetColumnBounds(view, item)
				If rect.IsEmpty Then
					Continue For
				End If
				Dim text As String = item.GetDisplayText(values(item), False)
				rect = CalcSummaryRect(text, e, view.Columns(item.FieldName))
				e.Appearance.DrawString(e.Cache, text, rect)
			Next item
		End Sub

		Private Function GetColumnBounds(ByVal view As GridView, ByVal item As GridGroupSummaryItem) As Rectangle
			Dim column As GridColumn = view.Columns(item.FieldName)
			Return GetColumnBounds(column)
		End Function

		Public Function GetColumnBounds(ByVal column As GridColumn) As Rectangle
			Dim gridInfo As GridViewInfo = CType(column.View.GetViewInfo(), GridViewInfo)
			Dim colInfo As GridColumnInfoArgs = gridInfo.ColumnsInfo(column)

			If colInfo IsNot Nothing Then
				Return colInfo.Bounds
			Else
				Return Rectangle.Empty
			End If
		End Function

		Private Function CalcSummaryRect(ByVal text As String, ByVal e As RowObjectCustomDrawEventArgs, ByVal column As GridColumn) As Rectangle
			Dim sz As SizeF = TextUtils.GetStringSize(e.Cache.Graphics, text, e.Appearance.Font)
			Dim width As Integer = Convert.ToInt32(sz.Width) + 1
			Dim result As Rectangle = GetColumnBounds(column)
			result = FixLeftEdge(width, result)
			result.Width = result.Width
			result.Y = e.Bounds.Y
			result.Height = e.Bounds.Height - 2
			Return PreventSummaryTextOverlapping(e, result)
		End Function

		Private Function FixLeftEdge(ByVal width As Integer, ByVal result As Rectangle) As Rectangle
			Dim delta As Integer = result.Width - width - 2
			If delta > 0 Then
				result.X += delta
				result.Width -= delta
			End If
			Return result
		End Function

		Private Function PreventSummaryTextOverlapping(ByVal e As RowObjectCustomDrawEventArgs, ByVal rect As Rectangle) As Rectangle
			Dim gInfo As GridGroupRowInfo = CType(e.Info, GridGroupRowInfo)
			Dim groupTextLocation As Integer = gInfo.ButtonBounds.Right + 10
			Dim groupTextWidth As Integer = TextUtils.GetStringSize(e.Cache.Graphics, gInfo.GroupText, e.Appearance.Font).Width
			Dim r As New Rectangle(groupTextLocation, 0, groupTextWidth, e.Info.Bounds.Height)
			If r.Right > rect.X Then
				If r.Right > rect.Right Then
					rect.Width = 0
				Else
					rect.Width -= r.Right - rect.X
					rect.X = r.Right
				End If
			End If
			Return rect
		End Function
	End Class
End Namespace
