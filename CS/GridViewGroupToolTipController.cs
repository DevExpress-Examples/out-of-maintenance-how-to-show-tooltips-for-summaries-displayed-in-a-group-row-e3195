using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using System.Collections;
using DevExpress.XtraGrid;

namespace AlignSummaries
{
    public class GridViewGroupToolTipController
    {
        GridView view;
        ToolTipController tooltip;
        GroupRowSummaryHelper groupRowSummary;
        GridHitInfo hitInfo;

        //Use Default Tooltip controller
        public GridViewGroupToolTipController(GridView view) : this(view, null, null) { }
        public GridViewGroupToolTipController(GridView view, GroupRowSummaryHelper grRowSummary, ToolTipController tooltip)
        {
            this.view = view;
            this.tooltip = tooltip;
            groupRowSummary = grRowSummary;
            View.MouseLeave += new EventHandler(OnView_MouseLeave);
            View.MouseMove += new MouseEventHandler(OnView_MouseMove);
        }

        void OnView_MouseMove(object sender, MouseEventArgs e)
        {
            hitInfo = View.CalcHitInfo(e.X, e.Y);
            if (!View.IsGroupRow(hitInfo.RowHandle))
                ToolTip.HideHint();
            else
            {
                GridColumn hoveredColumn = GetHoveredColumn();
                string toolTipText = GetToolTipText(hoveredColumn);
                ToolTip.ShowHint(new DevExpress.Utils.ToolTipControlInfo(toolTipText, toolTipText));
            }
        }

        string GetToolTipText(GridColumn column)
        {
            string text = "";
            if (column == null) return text;
            Hashtable summaryValues = view.GetGroupSummaryValues(hitInfo.RowHandle);
            foreach (DictionaryEntry itemObject in summaryValues)
            {
                GridSummaryItem item = itemObject.Key as GridSummaryItem;
                if (item.FieldName == column.FieldName)
                {
                    text = item.GetDisplayText(summaryValues[item], false);
                    break;
                }
            }
            return text;
        }

        GridColumn GetHoveredColumn()
        {
            Rectangle columnBounds;
            GridColumn column = null;
            if (groupRowSummary == null) return column;
            foreach (GridColumn gc in view.Columns)
            {
                columnBounds = groupRowSummary.GetColumnBounds(gc);
                if (hitInfo.HitPoint.X >= columnBounds.X && hitInfo.HitPoint.X <= columnBounds.X + columnBounds.Width)
                {
                    column = gc;
                    break;
                }
            }
            return column;
        }

        void OnView_MouseLeave(object sender, EventArgs e)
        {
            ToolTip.HideHint();
        }
        public GridView View { get { return view; } }
        protected ToolTipController ToolTip { get { return tooltip != null ? tooltip : ToolTipController.DefaultController; } }
    }
}
