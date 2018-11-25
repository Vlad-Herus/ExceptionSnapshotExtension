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
    internal class ExceptionManager2017 : IDebugEventCallback2
    {
        private static ExceptionManager2017 m_Isntance = null;
        public static ExceptionManager2017 Instance
        {
            get
            {
                if (m_Isntance == null)
                {
                    m_Isntance = new ExceptionManager2017();
                }

                return m_Isntance;
            }
        }

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

        private IVsDebugger VsDebugger
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return InternalDebugger as IVsDebugger;
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
            ThreadHelper.ThrowIfNotOnUIThread();
            var session = Session;
            if (session != null)
            {
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

        public void Go()
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
                    var expr = debugger.GetExpression("$exception.GetType().FullName", false, -1);
                    var res = expr.Value;
                    return res;
                }
            }

            return null;
        }

        public void AttachEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (!subscribed)
            {
                subscribed = true;

                var res = VsDebugger.AdviseDebugEventCallback(this);
            }
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

        public int Event(IDebugEngine2 pEngine, IDebugProcess2 pProcess, IDebugProgram2 pProgram, IDebugThread2 pThread, IDebugEvent2 pEvent, ref Guid riidEvent, uint dwAttrib)
        {
            // 51a94113-8788-4a54-ae15-08b74ff922d0 IDebugExceptionEvent2 IDebugExceptionEvent150 AD7StoppingEvent
            try
            {
                if (typeof(IDebugExceptionEvent2).GUID == riidEvent)
                {
                    //var e2 = pEvent as IDebugExceptionEvent2;
                    //var e = pEvent as IDebugExceptionEvent150;
                    //var cp = e2.CanPassToDebuggee();
                    //EXCEPTION_INFO150 info = new EXCEPTION_INFO150();
                    //var arr = new EXCEPTION_INFO150[] { info };
                    //var res = e.GetException(arr);
                    //res = e2.PassToDebuggee(1);

                    //e.GetExceptionDetails(out IDebugExceptionDetails details);
                    //details.GetTypeName(1, out string typeName);
                    //details.GetSource(out string sourceName);

                    //var engine150 = pEngine as IDebugEngine150;


                    var session = Session;
                    System.Diagnostics.Trace.WriteLine("IDebugExceptionEvent2");
                    //var topExceptions = GetExceptions(null, session);

                    //List<EXCEPTION_INFO150> updated = new List<EXCEPTION_INFO150>();
                    //List<EXCEPTION_INFO150> allChildren = new List<EXCEPTION_INFO150>();
                    //foreach (var topException in topExceptions)
                    //{
                    //    var childExceptions = GetExceptions(topException, session);
                    //    for (int i = 0; i < childExceptions.Count(); i++)
                    //    {
                    //        childExceptions[i].dwState &= 4294967278u;
                    //        updated.Add(childExceptions[i]);

                    //        if (childExceptions[i].bstrExceptionName == typeName.Trim('\"'))
                    //        {
                    //            pEngine.SetException(new EXCEPTION_INFO[] { Convert(childExceptions[i]) });
                    //        }
                    //    }
                    //    allChildren.AddRange(childExceptions);
                    //}

                    //if (updated.Any())
                    //{
                    //    engine150.SetExceptions(new ExceptionInfoEnumerator(updated.ToList()));
                    //}
                }
                else if (Guid.Parse("04bcb310-5e1a-469c-87c6-4971e6c8483a") == riidEvent)
                {
                    System.Diagnostics.Trace.WriteLine("Try 1");
                    //var ex = new ExceptionManager2017().GetCurrentExceptionType();
                    //if (ex != null)
                    //{
                    //pProgram.Continue(pThread);
                    //}
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine(riidEvent);
                }
            }
            catch
            {

            }
            return 0;
        }
    }
}
