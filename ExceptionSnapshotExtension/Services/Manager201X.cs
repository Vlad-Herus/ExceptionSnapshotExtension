using ExceptionSnapshotExtension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    abstract class Manager201X<ExceptionType> : IExceptionManager
    {
        public abstract bool SupportsConditions { get; }

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
