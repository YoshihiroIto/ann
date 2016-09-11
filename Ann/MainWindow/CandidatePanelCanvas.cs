using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ann.Core;
using Jewelry.Collections;
using Application = System.Windows.Application;
using FlowDirection = System.Windows.FlowDirection;

namespace Ann.MainWindow
{
    public class CandidatePanelCanvas : Canvas
    {
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

            {
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
            }

            {
                var p0 = new Point(0, ViewConstants.CandidatePanelHeight);
                var p1 = new Point(ViewConstants.CandidatePanelWidth, ViewConstants.CandidatePanelHeight);
                dc.DrawLine(PanelBorderLinePen, p0, p1);
            }

            if (d.Name != null)
            {
                var text = MakeText(
                    d.Name,
                    ViewConstants.CandidatePanel_Name_Width,
                    30,
                    CaptionBrush,
                    22,
                    NameTextCache);

                var p = new Point(ViewConstants.CandidatePanel_Name_Left, ViewConstants.CandidatePanel_Name_Top);
                dc.DrawText(text, p);
            }

            if (d.Comment != null)
            {
                var text = MakeText(
                    d.Comment,
                    ViewConstants.CandidatePanel_Comment_Width,
                    16,
                    CommentBrush,
                    11,
                    CommentTextCache);

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

        private static FormattedText MakeText(string src, double width, double height, Brush brush, double emSize, LruCache<string, FormattedText> cache)
        {
            return cache.GetOrAdd(src, s =>
            {
                var text = new FormattedText(
                    s,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    Dummy,
                    emSize,
                    brush)
                {
                    MaxTextWidth = width,
                    MaxTextHeight = height,
                    Trimming = TextTrimming.CharacterEllipsis
                };

                text.SetFontFamily(Application.Current.MainWindow.FontFamily);

                return text;
            });
        }

        private static readonly Typeface Dummy = new Typeface(string.Empty);
        private static readonly LruCache<string, FormattedText> NameTextCache = new LruCache<string, FormattedText>(50, false);
        private static readonly LruCache<string, FormattedText> CommentTextCache = new LruCache<string, FormattedText>(50, false);
    }
}