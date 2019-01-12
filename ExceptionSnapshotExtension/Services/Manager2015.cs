using ExceptionSnapshotExtension.Model;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    class Manager2015 : Manager201X<EXCEPTION_INFO>
    {
        private IDebuggerInternal11 InternalDebugger
        {
            get
            {
                var debugger = Package.GetGlobalService(typeof(SVsShellDebugger)) as IDebuggerInternal11;
                return debugger;
            }
        }

        private IDebugSession3 Session
        {
            get
            {
                return InternalDebugger?.CurrentSession;
            }
        }

        public override bool SessionAvailable => Session != null;

        public override bool SupportsConditions => false;

        protected override EXCEPTION_INFO[] GetDefaultExceptions(EXCEPTION_INFO? parent)
        {
            var parentArray = (!parent.HasValue) ? null : new EXCEPTION_INFO[]
            {
                parent.Value
            };

            IEnumDebugExceptionInfo2 val = default(IEnumDebugExceptionInfo2);
            if (Session.EnumDefaultExceptions(parentArray, out val) == 0 && val != null)
            {
                return val.ToArray();
            }
            else
            {
                return new EXCEPTION_INFO[] { };
            }
        }

        protected override EXCEPTION_INFO[] GetSetExceptions(EXCEPTION_INFO parent)
        {
            IEnumDebugExceptionInfo2 enumerator = default(IEnumDebugExceptionInfo2);
            if (Session.EnumSetExceptions(null, null, parent.guidType, out enumerator) == 0 && enumerator != null)
            {
                return enumerator.ToArray();
            }
            else
            {
                return new EXCEPTION_INFO[] { };
            }
        }

        protected override bool IsExceptionTopException(EXCEPTION_INFO exception)
        {
            return TopExceptions.Any(top => top.bstrExceptionName == exception.bstrExceptionName);
        }

        protected override void RemoveAllSetExceptions()
        {
            Guid guidType = Guid.Empty;
            Marshal.ThrowExceptionForHR(
                      Session.RemoveAllSetExceptions(ref guidType));
        }

        protected override void SetBreakFirstChance(ref EXCEPTION_INFO exception, bool breakFirstChance)
        {
            if (breakFirstChance)
            {
                var state = (uint)exception.dwState;
                Constants.EnableException(ref state);
                exception.dwState = (enum_EXCEPTION_STATE)state;
            }
            else
            {
                var state = (uint)exception.dwState;
                Constants.DisableException(ref state);
                exception.dwState = (enum_EXCEPTION_STATE)state;
            }
        }

        protected override void SetExceptions(IEnumerable<EXCEPTION_INFO> exceptions)
        {
            foreach (var ex in exceptions)
            {
                Marshal.ThrowExceptionForHR(
                    Session.SetException(new EXCEPTION_INFO[] { ex }));
            }
        }

        protected override EXCEPTION_INFO ConvertFromGeneric(ExceptionInfo exception)
        {
            return new EXCEPTION_INFO
            {
                bstrExceptionName = exception.Name,
                guidType = TopExceptions.First(ex => ex.bstrExceptionName == exception.GroupName).guidType,
                dwState = (enum_EXCEPTION_STATE)exception.State,
                dwCode = exception.Code,
            };
        }

        protected override ExceptionInfo ConvertToGeneric(EXCEPTION_INFO exception)
        {
            return new ExceptionInfo(exception.bstrExceptionName, TopExceptions.First(ex => ex.guidType == exception.guidType).bstrExceptionName)
            {
                State = (uint)exception.dwState,
                Code = exception.dwCode,
            };
        }
    }
}
