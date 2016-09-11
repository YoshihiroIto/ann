﻿using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ann.Core;

namespace Ann.MainWindow
{
    public class CandidatePanelCanvas : Canvas
    {
        private static readonly Typeface Dummy = new Typeface(string.Empty);

        private static readonly Pen PanelBorderLinePen;
        private static readonly Brush CaptionBrush;
        private static readonly Brush CommentBrush;

        private static readonly Brush IsSelectedBackgroundBrush;
        private static readonly Brush IsMouseOverBackgroundBrush;
        private static readonly Brush TransparentBrush;

        static CandidatePanelCanvas()
        {
            PanelBorderLinePen = new Pen(Application.Current.Resources["PanelBorderLineBrush"] as Brush, 1);
            if (PanelBorderLinePen.CanFreeze)
                PanelBorderLinePen.Freeze();

            CaptionBrush = Application.Current.Resources["CaptionBrush"] as Brush;
            CommentBrush = Application.Current.Resources["CommentBrush"] as Brush;
            IsSelectedBackgroundBrush = Application.Current.Resources["Item.SelectedActive.Background"] as Brush;
            IsMouseOverBackgroundBrush = Application.Current.Resources["Item.MouseOver.Background"] as Brush;
            TransparentBrush = Application.Current.Resources["TransparentBrush"] as Brush;
        }

        protected override void OnRender(DrawingContext dc)
        {
            //base.OnRender(dc);

            var d = DataContext as CandidatePanelViewModel;
            if (d == null)
                return;

            var backgroundRect = new Rect
            {
                X = 0,
                Y = 0,
                Width = ViewConstants.CandidatePanelWidth,
                Height = ViewConstants.CandidatePanelHeight
            };

            if (d.IsSelected)
                dc.DrawRectangle(IsSelectedBackgroundBrush, null, backgroundRect);
            else if (IsMouseOver)
                dc.DrawRectangle(IsMouseOverBackgroundBrush, null, backgroundRect);
            else
                dc.DrawRectangle(TransparentBrush, null, backgroundRect);

            var p0 = new Point(0, ViewConstants.CandidatePanelHeight);
            var p1 = new Point(ViewConstants.CandidatePanelWidth, ViewConstants.CandidatePanelHeight);
            dc.DrawLine(PanelBorderLinePen, p0, p1);


            if (d.Name != null)
            {
                var text = new FormattedText(
                    d.Name,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    Dummy,
                    22,
                    CaptionBrush)
                {
                    MaxTextWidth = ViewConstants.CandidatePanel_Name_Width,
                    MaxTextHeight = 30,
                    Trimming = TextTrimming.CharacterEllipsis
                };

                text.SetFontFamily(Application.Current.MainWindow.FontFamily);

                var p = new Point(ViewConstants.CandidatePanel_Name_Left, ViewConstants.CandidatePanel_Name_Top);
                dc.DrawText(text, p);
            }

            if (d.Comment != null)
            {
                var text = new FormattedText(
                    d.Comment,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    Dummy,
                    11,
                    CommentBrush)
                {
                    MaxTextWidth = ViewConstants.CandidatePanel_Comment_Width,
                    MaxTextHeight = 16,
                    Trimming = TextTrimming.CharacterEllipsis
                };

                text.SetFontFamily(Application.Current.MainWindow.FontFamily);

                var p = new Point(ViewConstants.CandidatePanel_Comment_Left, ViewConstants.CandidatePanel_Comment_Top);
                dc.DrawText(text, p);
            }

            if (d.Icon != null)
            {
                var r = new Rect
                {
                    X = ViewConstants.CandidatePanel_Image_Left,
                    Y = ViewConstants.CandidatePanel_Image_Top,
                    Width = Constants.IconSize,
                    Height = Constants.IconSize
                };
                dc.DrawRectangle(d.Icon, null, r);
            }
        }
    }
}