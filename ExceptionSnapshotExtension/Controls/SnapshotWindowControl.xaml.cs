using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using ExceptionSnapshotExtension.Services;
using ExceptionSnapshotExtension.Viewmodels;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotWindowControl"/> class.
        /// </summary>
        public SnapshotWindowControl()
        {
            this.InitializeComponent();
            Dispatcher.VerifyAccess();
            var debugger = (Package.GetGlobalService(typeof(DTE)) as DTE).Debugger as Debugger3;
            var shellDebugger = Package.GetGlobalService(typeof(SVsShellDebugger)) as SVsShellDebugger;

            this.DataContext = new ToolWindowVM(ExceptionManagerProto.Instance);
        }
    }
}