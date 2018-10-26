using EnvDTE90;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    internal class ExceptionManager
    {
        private readonly Debugger3 m_Debugger;
        private readonly SVsShellDebugger m_ShellDebugger;
        private IDebuggerInternal11 m_Internal;

        public ExceptionManager(Debugger3 debugger, SVsShellDebugger shellDebugger)
        {
            m_Debugger = debugger;
            m_ShellDebugger = shellDebugger;

            m_Internal = (IDebuggerInternal11)shellDebugger;
        }

        public void EnableAll()
        {
            EXCEPTION_INFO[] array = null;
            uint num = 0u;
            IEnumDebugExceptionInfo2 val = default(IEnumDebugExceptionInfo2);
            EXCEPTION_INFO[] array2;
            if (m_Internal.CurrentSession.EnumDefaultExceptions(array, out val) == 0 && val != null)
            {
                uint num2 = default(uint);
                val.GetCount(out num2);
                array2 = (EXCEPTION_INFO[])new EXCEPTION_INFO[num2];
                val.Next(num2, array2, ref num);
            }



            SetAll(true);
        }

        public void DisableAll()
        {
            var guid = Guid.Empty;
            m_Internal.CurrentSession.RemoveAllSetExceptions(ref guid);
        }

        private void SetAll(bool value)
        {
            Observable.Start(() =>
            {
                if (m_Debugger.ExceptionGroups != null)
                {
                    ExceptionSettings group = m_Debugger.ExceptionGroups.Item("Common Language Runtime Exceptions");

                    foreach (ExceptionSetting exception in group)
                    {
                        if (exception.BreakWhenThrown != value)
                        {
                            group.SetBreakWhenThrown(value, exception);
                        }
                    }
                }
            });
        }
    }
}
