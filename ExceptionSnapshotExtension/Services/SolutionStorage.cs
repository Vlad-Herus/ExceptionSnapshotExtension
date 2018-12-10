using ExceptionSnapshotExtension.Model;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    class SolutionStorage : IStorage
    {
        private IVsPersistSolutionOpts SolutionOptions
        {
            get
            {

            }
        }

        public IEnumerable<Snapshot> Load()
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<Snapshot> snapshots)
        {
            throw new NotImplementedException();
        }
    }
}
