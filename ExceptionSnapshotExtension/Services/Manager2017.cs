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
    internal class Manager2017 : Manager201X<EXCEPTION_INFO150>
    {
        #region Services

        private IDebuggerInternal15 InternalDebugger
        {
            get
            {
                var debugger = Package.GetGlobalService(typeof(SVsShellDebugger)) as IDebuggerInternal15;
                return debugger;
            }
        }

        private IDebugSession150 Session
        {
            get
            {
                return InternalDebugger?.CurrentSession as IDebugSession150;
            }
        }

        #endregion

        public override bool SupportsConditions => true;

        public override bool SessionAvailable => Session != null;

        protected override EXCEPTION_INFO150[] GetDefaultExceptions(EXCEPTION_INFO150? parent)
        {
            var parentArray = (!parent.HasValue) ? null : new EXCEPTION_INFO150[]
            {
                parent.Value
            };

            IEnumDebugExceptionInfo150 val = default(IEnumDebugExceptionInfo150);
            if (Session.EnumDefaultExceptions(parentArray, out val) == 0 && val != null)
            {
                return val.ToArray();
            }
            else
            {
                return new EXCEPTION_INFO150[] { };
            }
        }

        protected override EXCEPTION_INFO150[] GetSetExceptions(EXCEPTION_INFO150 parent)
        {
            IEnumDebugExceptionInfo150 enumerator = default(IEnumDebugExceptionInfo150);
            if (Session.EnumSetExceptions(parent.guidType, out enumerator) == 0 && enumerator != null)
            {
                return enumerator.ToArray();
            }
            else
            {
                return new EXCEPTION_INFO150[] { };
            }
        }

        protected override bool IsExceptionTopException(EXCEPTION_INFO150 exception)
        {
            return TopExceptions.Any(top => top.bstrExceptionName == exception.bstrExceptionName);
        }

        protected override void RemoveAllSetExceptions()
        {
            Guid guidType = Guid.Empty;
            Marshal.ThrowExceptionForHR(
                      InternalDebugger.CurrentSession.RemoveAllSetExceptions(ref guidType));
        }

        protected override void SetBreakFirstChance(ref EXCEPTION_INFO150 exception, bool breakFirstChance)
        {
            if (breakFirstChance)
            {
                Constants.EnableException(ref exception.dwState);
            }
            else
            {
                Constants.DisableException(ref exception.dwState);
            }
        }

        protected override void SetExceptions(IEnumerable<EXCEPTION_INFO150> exceptions)
        {
            ExceptionInfoEnumerator2017 enumerator =
                    new ExceptionInfoEnumerator2017(exceptions);
            Marshal.ThrowExceptionForHR(
                Session.SetExceptions(enumerator));
        }

        protected override EXCEPTION_INFO150 ConvertFromGeneric(ExceptionInfo exception)
        {
            return new EXCEPTION_INFO150
            {
                bstrExceptionName = exception.Name,
                guidType = TopExceptions.First(ex => ex.bstrExceptionName == exception.GroupName).guidType,
                dwState = exception.State,
                dwCode = exception.Code,
                pConditions = new ConditionEnumerator(exception.Conditions ?? new Condition[] { })
            };
        }

        protected override ExceptionInfo ConvertToGeneric(EXCEPTION_INFO150 exception)
        {
            return new ExceptionInfo(exception.bstrExceptionName, TopExceptions.First(ex => ex.guidType == exception.guidType).bstrExceptionName)
            {
                State = exception.dwState,
                Code = exception.dwCode,
                Conditions = exception.pConditions.ToArray()
            };
        }
    }
}
