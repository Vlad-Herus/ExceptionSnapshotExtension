using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using ExceptionSnapshotExtension.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
        ExceptionManager m_Manager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotWindowControl"/> class.
        /// </summary>
        public SnapshotWindowControl()
        {
            this.InitializeComponent();
            Dispatcher.VerifyAccess();
            var debugger = (Package.GetGlobalService(typeof(DTE)) as DTE).Debugger as Debugger3;
            var shellDebugger = Package.GetGlobalService(typeof(SVsShellDebugger)) as SVsShellDebugger;
            m_Manager = new ExceptionManager(debugger, shellDebugger);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void EnableAll(object sender, RoutedEventArgs e)
        {
            m_Manager.EnableAll();
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void DisableAll(object sender, RoutedEventArgs e)
        {
            m_Manager.DisableAll();

        }
    }
}