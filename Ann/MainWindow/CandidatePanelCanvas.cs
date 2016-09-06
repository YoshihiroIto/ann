using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ann.MainWindow
{
    public class CandidatePanelCanvas : Canvas
    {
        private static readonly Pen PanelBorderLinePen;

        static CandidatePanelCanvas()
        {
            PanelBorderLinePen = new Pen(Application.Current.Resources["PanelBorderLineBrush"] as Brush, 1);
            if (PanelBorderLinePen.CanFreeze)
                PanelBorderLinePen.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var p0 = new Point(0, ViewConstants.CandidatePanelHeight);
            var p1 = new Point(ViewConstants.CandidatePanelWidth, ViewConstants.CandidatePanelHeight);
            dc.DrawLine(PanelBorderLinePen, p0, p1);
        }
    }
}