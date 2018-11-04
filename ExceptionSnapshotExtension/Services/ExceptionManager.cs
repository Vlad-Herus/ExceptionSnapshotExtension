using EnvDTE;
using EnvDTE100;
using EnvDTE90;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    internal class CallBack : IDebugExceptionCallback2
    {
        public int QueryStopOnException(IDebugProcess2 pProcess, IDebugProgram2 pProgram, IDebugThread2 pThread, IDebugExceptionEvent2 pEvent)
        {
            return 1;
        }
    }

    internal class ExceptionManager2017
    {
        private delegate void UpdateException(ref EXCEPTION_INFO150 exception, out bool changed);

        private bool subscribed = false;

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

        private IDebugSession3 Session3
        {
            get
            {
                return Session as IDebugSession3;
            }
        }

        private Debugger Debugger
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
                return dte.Debugger as Debugger;
            }
        }

        public ExceptionManager2017()
        {

        }

        public void EnableAll()
        {
            var session = Session;
            if (session != null)
            {
                if (!subscribed)
                {
                    var info = new EXCEPTION_INFO()
                    {
                        bstrExceptionName = typeof(Exception).FullName,
                        bstrProgramName = null,
                        dwCode = 0,
                        pProgram = null,
                        guidType = VSConstants.DebugEnginesGuids.ManagedOnly_guid,
                        dwState = enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE
                            | enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE
                            | enum_EXCEPTION_STATE.EXCEPTION_JUST_MY_CODE_SUPPORTED
                            | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_FIRST_CHANCE
                            | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT
                    };

                    var res = Session3.AddExceptionCallback(new EXCEPTION_INFO[] { info }, new CallBack());
                }

                SetAll((ref EXCEPTION_INFO150 info, out bool changed) =>
                {
                    System.Diagnostics.Trace.WriteLine(info.guidType + "  :  " + info.bstrExceptionName);
                    changed = true;
                    info.dwState |= 17u;
                }, session);
            }
        }

        public void DisableAll()
        {
            var session = Session;
            if (session != null)
            {
                SetAll((ref EXCEPTION_INFO150 info, out bool changed) =>
                {
                    changed = true;
                    info.dwState &= 4294967278u;
                }, session);
            }
        }

        public void Enable(string clrExceptionType)
        {
            var session = Session;
            if (session != null)
            {
                //session.
            }
        }

        public void Go(bool addExceptionToIgnore, bool includeModule)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (InternalDebugger.InBreakMode)
            {
                Debugger.Go(false);
            }
        }

        public string GetCurrentExceptionType()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (InternalDebugger.InBreakMode)
            {
                var debugger = Debugger;
                var reason = debugger.LastBreakReason;

                if (reason == dbgEventReason.dbgEventReasonExceptionThrown || reason == dbgEventReason.dbgEventReasonExceptionNotHandled)
                {
                    var expr = debugger.GetExpression("$exception.GetType().Name", false, -1);
                    var res = expr.Value;
                }
            }

            return null;
        }

        private void SetAll(UpdateException action, IDebugSession150 session)
        {
            var topExceptions = GetExceptions(null, session);

            List<EXCEPTION_INFO150> updated = new List<EXCEPTION_INFO150>();
            for (int i = 0; i < topExceptions.Length; i++)
            {
                action(ref topExceptions[i], out bool changed);
                if (changed)
                {
                    updated.Add(topExceptions[i]);
                }
            }

            if (updated.Any())
            {
                session.SetExceptions(new ExceptionInfoEnumerator(updated.ToList()));
                updated.Clear();
            }

            List<EXCEPTION_INFO150> allChildren = new List<EXCEPTION_INFO150>();
            foreach (var topException in topExceptions)
            {
                var childExceptions = GetExceptions(topException, session);
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
                session.SetExceptions(new ExceptionInfoEnumerator(updated.ToList()));
            }
        }

        private EXCEPTION_INFO150[] GetExceptions(EXCEPTION_INFO150? parent, IDebugSession150 session)
        {
            uint num = 0u;
            EXCEPTION_INFO150[] array = (EXCEPTION_INFO150[])((!parent.HasValue) ? null : new EXCEPTION_INFO150[1]
            {
                parent.Value
            });
            IEnumDebugExceptionInfo150 val = default(IEnumDebugExceptionInfo150);
            EXCEPTION_INFO150[] array2;
            if (session.EnumDefaultExceptions(array, out val) == 0 && val != null)
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

        public static EXCEPTION_INFO Convert(EXCEPTION_INFO150 info)
        {
            return new EXCEPTION_INFO
            {
                bstrExceptionName = info.bstrExceptionName,
                bstrProgramName = info.bstrProgramName,
                dwCode = info.dwCode,
                dwState = (enum_EXCEPTION_STATE)info.dwState,
                guidType = info.guidType,
                pProgram = info.pProgram
            };
        }
    }
}
