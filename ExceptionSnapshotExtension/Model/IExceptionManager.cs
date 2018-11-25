using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Model
{
    internal delegate void OnExceptionDelegade(string exceptionName, string exceptionModule, ref bool continueExecution);

    internal interface IExceptionManager
    {
        event OnExceptionDelegade ExceptionCaught;

        /// <summary>
        /// Null if debugger is not in break mode because of exception
        /// </summary>
        ExceptionInfo CurrentException { get; }

        void ApplyException(ExceptionInfo info);

        void EnableAll();
        void DisableAll();

        void RestoreSnapshot(Snapshot snapshot);
        Snapshot GetCurrentExceptionSnapshot();
    }
}
