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

#pragma warning disable 67 //TODO: use or remove
        public event OnExceptionDelegade ExceptionCaught;
#pragma warning restore 67


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

        public bool SupportsConditions => true;

        private bool SessionAvailable => Session != null;

        private EXCEPTION_INFO150[] m_TopExceptions;

        private EXCEPTION_INFO150[] TopExceptions
        {
            get
            {
                if (m_TopExceptions == null)
                {
                    m_TopExceptions = GetExceptions(null);
                }

                return m_TopExceptions;
            }
        }

        public ExceptionInfo CurrentException => throw new NotImplementedException();

        public void ApplyException(ExceptionInfo info)
        {
            throw new NotImplementedException();
        }

        public void DisableAll()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (SessionAvailable)
            {
                SetAll((ref EXCEPTION_INFO150 info, out bool changed) =>
                {
                    changed = true;
                    Constants.DisableException(ref info.dwState);
                });
            }
        }

        public void EnableAll()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (SessionAvailable)
            {
                SetAll((ref EXCEPTION_INFO150 info, out bool changed) =>
                {
                    changed = true;
                    System.Diagnostics.Trace.WriteLine(info.dwCode);
                    Constants.EnableException(ref info.dwState);
                });
            }
        }

        public Snapshot GetCurrentExceptionSnapshot()
        {
            if (SessionAvailable)
            {
                var exceptions = new List<EXCEPTION_INFO150>();
                IEnumDebugExceptionInfo150 enumerator = default(IEnumDebugExceptionInfo150);
                foreach (var topException in TopExceptions)
                {
                    if (Session.EnumSetExceptions(topException.guidType, out enumerator) == 0 && enumerator != null)
                    {
                        exceptions.AddRange(enumerator.ToArray());
                    }
                }

                return new Snapshot
                {
                    Exceptions = exceptions.Select(ex => Convert(ex)).ToArray()
                };
            }
            else
            {
                return null;
            }
        }

        public void RestoreSnapshot(Snapshot snapshot)
        {
            if (SessionAvailable)
            {
                Guid guidType = Guid.Empty;
                Marshal.ThrowExceptionForHR(
                    InternalDebugger.CurrentSession.RemoveAllSetExceptions(ref guidType));

                //TODO: For exceptions that are being set to default value call RemoveSetExceptions() instead of SetExceptions

                var topExceptions = snapshot.Exceptions.Where(ex => TopExceptions.Any(top => top.bstrExceptionName == ex.Name));

                ExceptionInfoEnumerator2017 enumerator =
                    new ExceptionInfoEnumerator2017(topExceptions.Select(ex => Convert(ex)));
                //TODO: Check results of the api calls. Log errors.
                Session.SetExceptions(enumerator);

                enumerator =
                    new ExceptionInfoEnumerator2017(snapshot.Exceptions.Except(topExceptions).Select(ex => Convert(ex)));
                //TODO: Check results of the api calls. Log errors.
                Session.SetExceptions(enumerator);
            }
        }

        private void SetAll(UpdateException action)
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
                Session.SetExceptions(new ExceptionInfoEnumerator2017(updated));
                updated.Clear();
            }

            // For some reason top level exceptions (groups) must be passed to IDebugSession150::SetExceptions() separately from normal exceptions
            List<EXCEPTION_INFO150> allChildren = new List<EXCEPTION_INFO150>();
            foreach (var topException in TopExceptions)
            {
                var childExceptions = GetExceptions(topException);
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
                Session.SetExceptions(new ExceptionInfoEnumerator2017(updated));
            }
        }

        private EXCEPTION_INFO150[] GetExceptions(EXCEPTION_INFO150? parent)
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

        private ExceptionInfo Convert(EXCEPTION_INFO150 info)
        {
            return new ExceptionInfo(info.bstrExceptionName, TopExceptions.First(ex => ex.guidType == info.guidType).bstrExceptionName)
            {
                State = info.dwState,
                Code = info.dwCode,
                Conditions = info.pConditions.ToArray()
            };
        }

        private EXCEPTION_INFO150 Convert(ExceptionInfo info)
        {
            return new EXCEPTION_INFO150
            {
                bstrExceptionName = info.Name,
                guidType = TopExceptions.First(ex => ex.bstrExceptionName == info.GroupName).guidType,
                dwState = info.State,
                dwCode = info.Code,
                pConditions = new ConditionEnumerator(info.Conditions ?? new Condition[] { })
            };
        }
    }
}
