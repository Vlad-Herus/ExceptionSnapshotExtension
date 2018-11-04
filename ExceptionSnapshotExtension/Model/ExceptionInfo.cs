using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Model
{
    class ExceptionInfo
    {
        public string Name { get; }
        public string GroupName { get; }
        public bool BreakFirstChance { get; set; }
        public bool SupportsConditions { get; }
        public List<Condition> Conditions { get; set; }
    }

    class Condition
    {
        public string Pattern { get; set; }
        public bool PositiveComparison { get; set; }
    }

    class Spapshot
    {
        IEnumerable<ExceptionInfo> Exceptions { get; }
    }

    interface IExceptionManager
    {
        event Action ExceptionCaught;

        void EnableAll();
        void DisableAll();

        void AddIgnoreCurrentException(bool respectModule, bool continueExecution);

        void RestoreSnapshot(Spapshot snapshot);
    }
}
