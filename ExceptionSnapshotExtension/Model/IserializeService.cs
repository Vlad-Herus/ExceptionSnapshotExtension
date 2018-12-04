using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Model
{
    interface IserializeService
    {
        string SerializeSnapshot(Snapshot snapshot);
        Snapshot RestoreSnapshot(string snapshot);
    }
}
