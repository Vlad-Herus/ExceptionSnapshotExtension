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
        bool SupportsConditions { get; }

        bool SessionAvailable { get; }

        void EnableAll();
        void DisableAll();

        void RestoreSnapshot(Snapshot snapshot);
        Snapshot GetCurrentExceptionSnapshot();
    }
}
