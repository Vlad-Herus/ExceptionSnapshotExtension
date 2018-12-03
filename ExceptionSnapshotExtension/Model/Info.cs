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
        public Condition[] Conditions { get; set; }

        public ExceptionInfo(string name, string groupName)
        {
            Name = name;
            GroupName = groupName;
        }
    }

    internal class Condition
    {
        public string Pattern { get; set; }
        public bool PositiveComparison { get; set; }
    }

    internal class Snapshot
    {
        public ExceptionInfo[] Exceptions { get; set; }
    }
}
