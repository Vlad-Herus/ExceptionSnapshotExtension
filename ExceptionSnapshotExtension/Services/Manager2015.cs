using ExceptionSnapshotExtension.Model;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    class Manager2015 : IExceptionManager
    {
        private IDebuggerInternal11 InternalDebugger
        {
            get
            {
                var debugger = Package.GetGlobalService(typeof(SVsShellDebugger)) as IDebuggerInternal11;
                return debugger;
            }
        }

        private IDebugSession3 Session
        {
            get
            {
                return InternalDebugger?.CurrentSession;
            }
        }

        private bool SessionAvailable => Session != null;

        public bool SupportsConditions => false;

        public void DisableAll()
        {
            throw new NotImplementedException();
        }

        public void EnableAll()
        {
            throw new NotImplementedException();
        }

        public Snapshot GetCurrentExceptionSnapshot()
        {
            throw new NotImplementedException();
        }

        public void RestoreSnapshot(Snapshot snapshot)
        {
            throw new NotImplementedException();
        }
    }
}
