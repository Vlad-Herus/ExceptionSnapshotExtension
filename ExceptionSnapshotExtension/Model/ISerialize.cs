using ExceptionSnapshotExtension.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Model
{
    internal interface ISerialize
    {
        void Serialize(IEnumerable<Snapshot> snapshots, Stream targetStream);
        IEnumerable<Snapshot> Deserialize(Stream snapshots);
    }
}
