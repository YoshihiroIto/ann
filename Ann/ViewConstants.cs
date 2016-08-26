using System.Windows;
using Ann.Core;

namespace Ann
{
    // ReSharper disable InconsistentNaming
    public static class ViewConstants
    {
        public const int MaxExecutableUnitPanelCount = 10;

        public const double MainWindowWidth = 600;
        public const double MainWindowBorderThicknessUnit = 1;
        public static Thickness MainWindowBorderThickness = new Thickness(MainWindowBorderThicknessUnit);

        public const double BaseMarginUnit = 16;
        public static Thickness BaseMargin = new Thickness(BaseMarginUnit);

        public const double StatusBarWidth = MainWindowWidth - MainWindowBorderThicknessUnit*2;
        public const double InputLineWidth = MainWindowWidth - BaseMarginUnit*2;

        //
        public const double ExecutableUnitPanelWidth = MainWindowWidth - BaseMarginUnit*2;
        public const double ExecutableUnitPanelHeight = 64;

        public const double ExecutableUnitPanel_BaseMarginUnit = 8;

        public const double ExecutableUnitPanel_Star_Right = 4;
        public const double ExecutableUnitPanel_Star_Bottom = 0;

        public const double ExecutableUnitPanel_Menu_Right = -1;
        public const double ExecutableUnitPanel_Menu_Top = 8;

        public const double ExecutableUnitPanel_Image_Left = ExecutableUnitPanel_BaseMarginUnit;
        public const double ExecutableUnitPanel_Image_Top = ExecutableUnitPanel_BaseMarginUnit;

        public const double ExecutableUnitPanel_Name_Left = Constants.IconSize + ExecutableUnitPanel_BaseMarginUnit*2;
        public const double ExecutableUnitPanel_Name_Top = ExecutableUnitPanel_BaseMarginUnit;
        public const double ExecutableUnitPanel_Name_Width = ExecutableUnitPanelWidth - 96;

        public const double ExecutableUnitPanel_Path_Left = ExecutableUnitPanel_Name_Left;
        public const double ExecutableUnitPanel_Path_Top = 42;
        public const double ExecutableUnitPanel_Path_Width = ExecutableUnitPanel_Name_Width;

    }
    // ReSharper restore InconsistentNaming
}