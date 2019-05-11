﻿using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using ExceptionSnapshotExtension.Services;
using ExceptionSnapshotExtension.Viewmodels;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExceptionSnapshotExtension
{
    /// <summary>
    /// Interaction logic for SnapshotWindowControl.
    /// </summary>
    public partial class SnapshotWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotWindowControl"/> class.
        /// </summary>
        public SnapshotWindowControl()
        {
            this.InitializeComponent();
            Dispatcher.VerifyAccess();
            this.DataContext = ExceptionPackage.MasterViewModel;
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (listView.ActualWidth == Double.NaN)
            {
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            }

            double remainingSpace = listView.ActualWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
            {
                if (i != autoFillColumnIndex)
                {
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
                }
            }

            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
        }
    }
}