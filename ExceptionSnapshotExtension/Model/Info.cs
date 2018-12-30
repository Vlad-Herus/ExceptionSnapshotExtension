using ExceptionSnapshotExtension.Services;
using Microsoft.VisualStudio.Debugger.Interop;
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
        public uint State { get; set; }
        public uint Code { get; set; }
        public bool BreakFirstChance
        {
            get
            {
                return Constants.SetToBreakFirstChance(State);
            }
            set
            {
                uint state = State;

                if (value)
                {
                    Constants.EnableException(ref state);
                }
                else
                {
                    Constants.EnableException(ref state);
                }

                State = state;
            }
        }
        public Condition[] Conditions { get; set; }

        public ExceptionInfo(string name, string groupName)
        {
            Name = name;
            GroupName = groupName;
        }
    }

    internal class Condition : IDebugExceptionCondition
    {
        public EXCEPTION_CONDITION_TYPE Type { get; set; }

        public EXCEPTION_CONDITION_CALLSTACK_BEHAVIOR CallStackBehavior { get; set; }

        public EXCEPTION_CONDITION_OPERATOR Operator { get; set; }

        public string Value { get; set; }
    }

    internal class Snapshot
    {
        public string Name { get; set; }
        public ExceptionInfo[] Exceptions { get; set; }
    }
}
