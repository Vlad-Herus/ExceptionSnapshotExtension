using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

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
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Capture(object sender, RoutedEventArgs e)
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;

            var debugger = dte.Debugger as Debugger3;
            var item = debugger.ExceptionGroups.Item("Common Language Runtime Exceptions");


            int n = 22;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Restore(object sender, RoutedEventArgs e)
        {
        }
    }
}