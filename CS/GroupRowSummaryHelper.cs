using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid;
using System.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;
using DevExpress.Utils.Text;

namespace AlignSummaries
{
    public class GroupRowSummaryHelper
    {
        public ArrayList ExtractSummaryItems(GridView view)
        {
            ArrayList items = new ArrayList();
            foreach (GridSummaryItem si in view.GroupSummary)
                if (si is GridGroupSummaryItem && si.SummaryType != SummaryItemType.None)
                    items.Add(si);
            return items;
        }

        public void DrawBackground(RowObjectCustomDrawEventArgs e, GridView view)
        {
            GridGroupRowPainter painter;
            GridGroupRowInfo info;
            painter = e.Painter as GridGroupRowPainter;
            info = e.Info as GridGroupRowInfo;
            int level = view.GetRowLevel(e.RowHandle);
            int row = view.GetDataRowHandleByGroupRowHandle(e.RowHandle);
            info.GroupText = string.Format("{0}: {1}", view.GroupedColumns[level].Caption,
                view.GetRowCellDisplayText(row, view.GroupedColumns[level]));
            e.Appearance.DrawBackground(e.Cache, info.Bounds);
            painter.ElementsPainter.GroupRow.DrawObject(info);
        }

        public void DrawSummaryValues(RowObjectCustomDrawEventArgs e, GridView view, ArrayList items)
        {
            Hashtable values = view.GetGroupSummaryValues(e.RowHandle);
            foreach (GridGroupSummaryItem item in items)
            {
                Rectangle rect = GetColumnBounds(view, item);
                if (rect.IsEmpty) continue;
                string text = item.GetDisplayText(values[item], false);
                rect = CalcSummaryRect(text, e, view.Columns[item.FieldName]);
                e.Appearance.DrawString(e.Cache, text, rect);
            }
        }

        private Rectangle GetColumnBounds(GridView view, GridGroupSummaryItem item)
        {
            GridColumn column = view.Columns[item.FieldName];
            return GetColumnBounds(column);
        }

        public Rectangle GetColumnBounds(GridColumn column)
        {
            GridViewInfo gridInfo = (GridViewInfo)column.View.GetViewInfo();
            GridColumnInfoArgs colInfo = gridInfo.ColumnsInfo[column];

            if (colInfo != null)
                return colInfo.Bounds;
            else
                return Rectangle.Empty;
        }

        private Rectangle CalcSummaryRect(string text, RowObjectCustomDrawEventArgs e, GridColumn column)
        {
            SizeF sz = TextUtils.GetStringSize(e.Graphics, text, e.Appearance.Font);
            int width = Convert.ToInt32(sz.Width) + 1;
            Rectangle result = GetColumnBounds(column);
            result = FixLeftEdge(width, result);
            result.Width = result.Width;
            result.Y = e.Bounds.Y;
            result.Height = e.Bounds.Height - 2;
            return PreventSummaryTextOverlapping(e, result);
        }

        private Rectangle FixLeftEdge(int width, Rectangle result)
        {
            int delta = result.Width - width - 2;
            if (delta > 0)
            {
                result.X += delta;
                result.Width -= delta;
            }
            return result;
        }

        private Rectangle PreventSummaryTextOverlapping(RowObjectCustomDrawEventArgs e, Rectangle rect)
        {
            GridGroupRowInfo gInfo = (GridGroupRowInfo)e.Info;
            int groupTextLocation = gInfo.ButtonBounds.Right + 10;
            int groupTextWidth = TextUtils.GetStringSize(e.Graphics, gInfo.GroupText, e.Appearance.Font).Width;
            Rectangle r = new Rectangle(groupTextLocation, 0, groupTextWidth, e.Info.Bounds.Height);
            if (r.Right > rect.X)
            {
                if (r.Right > rect.Right)
                    rect.Width = 0;
                else
                {
                    rect.Width -= r.Right - rect.X;
                    rect.X = r.Right;
                }
            }
            return rect;
        }
    }
}
