using ExceptionSnapshotExtension.Services;
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
        public uint NativeCode { get; set; }
        public bool BreakFirstChance
        {
            get
            {
                return Constants.SetToBreakFirstChance(NativeCode);
            }
            set
            {
                uint code = NativeCode;

                if (value)
                {
                    Constants.EnableException(ref code);
                }
                else
                {
                    Constants.EnableException(ref code);
                }

                NativeCode = code;
            }
        }
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
