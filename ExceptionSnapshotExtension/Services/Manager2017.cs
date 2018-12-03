using ExceptionSnapshotExtension.Model;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    class Manager2017 : IExceptionManager
    {
        private delegate void UpdateException(ref EXCEPTION_INFO150 exception, out bool changed);

        public event OnExceptionDelegade ExceptionCaught;

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

        bool SessionAvailable => Session != null;

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
                    Constants.EnableException(ref info.dwState);
                });
            }
        }

        public Snapshot GetCurrentExceptionSnapshot()
        {
            throw new NotImplementedException();
        }

        public void RestoreSnapshot(Snapshot snapshot)
        {
            throw new NotImplementedException();
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
            uint num = 0u;
            EXCEPTION_INFO150[] array = (EXCEPTION_INFO150[])((!parent.HasValue) ? null : new EXCEPTION_INFO150[1]
            {
                parent.Value
            });
            IEnumDebugExceptionInfo150 val = default(IEnumDebugExceptionInfo150);
            EXCEPTION_INFO150[] array2;
            if (Session.EnumDefaultExceptions(array, out val) == 0 && val != null)
            {
                uint num2 = default(uint);
                val.GetCount(out num2);
                array2 = (EXCEPTION_INFO150[])new EXCEPTION_INFO150[num2];
                val.Next(num2, array2, ref num);
            }
            else
            {
                array2 = (EXCEPTION_INFO150[])new EXCEPTION_INFO150[0];
            }
            return array2;
        }

        private ExceptionInfo Convert(EXCEPTION_INFO150 info)
        {
            return new ExceptionInfo(info.bstrExceptionName, TopExceptions.First(ex => ex.guidType == info.guidType).bstrExceptionName)
            {
                BreakFirstChance = info.SetToBreakFirstChance(),
                // TODO: conditions
                //Conditions
            };
        }
    }
}
