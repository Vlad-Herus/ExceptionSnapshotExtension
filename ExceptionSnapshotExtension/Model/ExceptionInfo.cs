using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Model
{
    internal class ExceptionInfo
    {
        public string Name { get; }
        public string GroupName { get; }
        public bool BreakFirstChance { get; set; }
        public List<Condition> Conditions { get; set; }
    }

    internal class Condition
    {
        public string Pattern { get; set; }
        public bool PositiveComparison { get; set; }
    }

    internal class Spapshot
    {
        private IEnumerable<ExceptionInfo> Exceptions { get; }
    }

    internal interface IExceptionManager
    {
        ExceptionInfo CurrentException { get; }

        void EnableAll();
        void DisableAll();

        void AddExceptionToIgnoreList(ExceptionInfo info);

        void RestoreSnapshot(Spapshot snapshot);
    }
}
