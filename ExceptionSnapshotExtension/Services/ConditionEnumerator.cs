﻿using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    class ConditionEnumerator : IDebugExceptionConditionList
    {
        private readonly IEnumerable<IDebugExceptionCondition> m_Conditions;

        public int Count => m_Conditions.Count();

        public IDebugExceptionCondition this[int index] => m_Conditions.ElementAt(index);

        public ConditionEnumerator(IEnumerable<IDebugExceptionCondition> conditions)
        {
            m_Conditions = conditions;
        }
    }
}
