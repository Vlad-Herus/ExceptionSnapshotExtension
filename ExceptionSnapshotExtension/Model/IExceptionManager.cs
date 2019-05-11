using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Model
{
    internal delegate void OnExceptionDelegade(ExceptionInfo info, string modlueName, ref bool continueExecution);

    internal interface IExceptionManager
    {
        bool SessionAvailable { get; }

        void EnableAll();
        void DisableAll();

        void RestoreSnapshot(Snapshot snapshot);
        Snapshot GetCurrentExceptionSnapshot();

        /// <summary>
        /// Checks if Break first chance for snapshot matches current exception settings
        /// </summary>
        bool VerifySnapshot(Snapshot snapshot);
    }
}
