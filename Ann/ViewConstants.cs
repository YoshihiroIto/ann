﻿using System.Windows;
using Ann.Core;

namespace Ann
{
    // ReSharper disable InconsistentNaming
    public static class ViewConstants
    {
        public const int MaxCandidateCount = 10;

        public const double ShadowSize = 10;
        public static Thickness MainWindowShadowMargin = new Thickness(ShadowSize);

        public const double MainWindowWithShadowWidth = 600+ShadowSize*2;

        public const double MainWindowCournerCornerRadiusUnit = 6;
        public static CornerRadius MainWindowCornerRadius = new CornerRadius(MainWindowCournerCornerRadiusUnit);

        public const double MainWindowWidth = 600;
        public const double MainWindowBorderThicknessUnit = 0;
        public static Thickness MainWindowBorderThickness = new Thickness(MainWindowBorderThicknessUnit);

        public const double BaseMarginUnit = 16;
        public static Thickness BaseMargin = new Thickness(BaseMarginUnit);

        public const double StatusBarWidth = MainWindowWidth - MainWindowBorderThicknessUnit*2;
        public const double InputLineWidth = MainWindowWidth - BaseMarginUnit*2;

        //
        public const double CandidatePanelWidth = MainWindowWidth - BaseMarginUnit*2;
        public const double CandidatePanelHeight = 64;

        public const double CandidatePanel_BaseMarginUnit = 8;

        public const double CandidatePanel_Star_Right = 4;
        public const double CandidatePanel_Star_Bottom = 0;

        public const double CandidatePanel_Menu_Right = -1;
        public const double CandidatePanel_Menu_Top = 8;

        public const double CandidatePanel_Image_Left = CandidatePanel_BaseMarginUnit;
        public const double CandidatePanel_Image_Top = CandidatePanel_BaseMarginUnit;

        public const double CandidatePanel_Name_Left = Constants.IconSize + CandidatePanel_BaseMarginUnit*2;
        public const double CandidatePanel_Name_Top = CandidatePanel_BaseMarginUnit;
        public const double CandidatePanel_Name_Width = CandidatePanelWidth - 96;

        public const double CandidatePanel_Comment_Left = CandidatePanel_Name_Left;
        public const double CandidatePanel_Comment_Top = 42;
        public const double CandidatePanel_Comment_Width = CandidatePanel_Name_Width;

    }
    // ReSharper restore InconsistentNaming
}