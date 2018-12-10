using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceptionSnapshotExtension.Model;

namespace ExceptionSnapshotExtension.Services
{
    class JsonSerialize : ISerialize
    {
        public IEnumerable<Snapshot> Deserialize(Stream snapshots)
        {
            throw new NotImplementedException();
        }

        public Stream Serialize(IEnumerable<Snapshot> snapshots)
        {
            throw new NotImplementedException();
        }
    }
}
