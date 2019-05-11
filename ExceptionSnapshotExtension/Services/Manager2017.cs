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
    internal class Manager2017 : IExceptionManager
    {
        private delegate void UpdateException(ref EXCEPTION_INFO150 exception, out bool changed);

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

        private EXCEPTION_INFO150[] m_TopExceptions;

        public bool SessionAvailable => Session != null;

        EXCEPTION_INFO150[] TopExceptions
        {
            get
            {
                if (m_TopExceptions == null)
                {
                    m_TopExceptions = GetDefaultExceptions(null);
                }

                return m_TopExceptions;
            }
        }

        #region  public

        public void DisableAll()
        {
            ThrowIfNoSession();

            SetAll((ref EXCEPTION_INFO150 info, out bool changed) =>
            {
                changed = true;
                SetBreakFirstChance(ref info, false);
            });
        }

        public void EnableAll()
        {
            ThrowIfNoSession();

            SetAll((ref EXCEPTION_INFO150 info, out bool changed) =>
            {
                changed = true;
                SetBreakFirstChance(ref info, true);
            });
        }

        public Snapshot GetCurrentExceptionSnapshot()
        {
            ThrowIfNoSession();

            var exceptions = TopExceptions.SelectMany(top => GetSetExceptions(top));

            return new Snapshot
            {
                Exceptions = exceptions.Select(ex => ConvertToGeneric(ex)).ToArray()
            };
        }

        public void RestoreSnapshot(Snapshot snapshot)
        {
            ThrowIfNoSession();

            RemoveAllSetExceptions();

            var nativeExceptions = snapshot.
                Exceptions.
                Select(ex => ConvertFromGeneric(ex));
            var topExceptions = nativeExceptions.Where(ex => IsExceptionTopException(ex));

            SetExceptions(topExceptions);
            SetExceptions(nativeExceptions.Except(topExceptions));
        }

        public bool VerifySnapshot(Snapshot snapshot)
        {
            var current = GetCurrentExceptionSnapshot();

            foreach (var ex in snapshot.Exceptions)
            {
                var corresponding = current.Exceptions.SingleOrDefault(corEx =>
                corEx.Name == ex.Name &&
                corEx.Code == ex.Code &&
                corEx.GroupName == ex.GroupName);

                if (ex.BreakFirstChance != corresponding.BreakFirstChance)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Private

        EXCEPTION_INFO150[] GetDefaultExceptions(EXCEPTION_INFO150? parent)
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

        EXCEPTION_INFO150[] GetSetExceptions(EXCEPTION_INFO150 parent)
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

        bool IsExceptionTopException(EXCEPTION_INFO150 exception)
        {
            return TopExceptions.Any(top => top.bstrExceptionName == exception.bstrExceptionName);
        }

        void RemoveAllSetExceptions()
        {
            Guid guidType = Guid.Empty;
            Marshal.ThrowExceptionForHR(
                      InternalDebugger.CurrentSession.RemoveAllSetExceptions(ref guidType));
        }

        void SetBreakFirstChance(ref EXCEPTION_INFO150 exception, bool breakFirstChance)
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

        void SetExceptions(IEnumerable<EXCEPTION_INFO150> exceptions)
        {
            ExceptionInfoEnumerator2017 enumerator =
                    new ExceptionInfoEnumerator2017(exceptions);
            Marshal.ThrowExceptionForHR(
                Session.SetExceptions(enumerator));
        }

        EXCEPTION_INFO150 ConvertFromGeneric(ExceptionInfo exception)
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

        ExceptionInfo ConvertToGeneric(EXCEPTION_INFO150 exception)
        {
            return new ExceptionInfo(exception.bstrExceptionName, TopExceptions.First(ex => ex.guidType == exception.guidType).bstrExceptionName)
            {
                State = exception.dwState,
                Code = exception.dwCode,
                Conditions = exception.pConditions.ToArray()
            };
        }

        #endregion

        void SetAll(UpdateException action)
        {
            List<EXCEPTION_INFO150> updated = new List<EXCEPTION_INFO150>();
            for (int i = 0; i < TopExceptions.Length; i++)
            {
                action(ref TopExceptions[i], out bool changed);
                if (changed)
                {
                    updated.Add(TopExceptions[i]);
                }
            }

            if (updated.Any())
            {
                SetExceptions(updated);
                updated.Clear();
            }

            // For some reason top level exceptions (groups) must be passed to IDebugSession150::SetExceptions() separately from normal exceptions
            List<EXCEPTION_INFO150> allChildren = new List<EXCEPTION_INFO150>();
            foreach (var topException in TopExceptions)
            {
                var childExceptions = GetDefaultExceptions(topException);
                for (int i = 0; i < childExceptions.Count(); i++)
                {
                    action(ref childExceptions[i], out bool changed);
                    if (changed)
                    {
                        updated.Add(childExceptions[i]);
                    }
                }
                allChildren.AddRange(childExceptions);
            }

            if (updated.Any())
            {
                SetExceptions(updated);
            }
        }

        void ThrowIfNoSession()
        {
            if (!SessionAvailable)
            {
                throw new Exception("Session not available.");
            }
        }
    }
}
