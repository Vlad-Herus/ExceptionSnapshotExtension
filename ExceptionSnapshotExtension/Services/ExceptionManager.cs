using EnvDTE90;
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
    internal class ExceptionManager
    {
        private delegate void UpdateException(ref EXCEPTION_INFO150 exception);

        private readonly Debugger3 m_Debugger;
        private readonly SVsShellDebugger m_ShellDebugger;
        private IDebuggerInternal15 m_Internal;
        private IDebugSession150 Session
        {
            get
            {
                return m_Internal.CurrentSession as IDebugSession150;
            }
        }


        public ExceptionManager(Debugger3 debugger, SVsShellDebugger shellDebugger)
        {
            m_Debugger = debugger;
            m_ShellDebugger = shellDebugger;

            m_Internal = (IDebuggerInternal15)shellDebugger;
        }

        public void EnableAll()
        {
            SetAll((ref EXCEPTION_INFO150 info) =>
            {
                info.dwState |= 17u;
            });
        }

        public void DisableAll()
        {
            SetAll((ref EXCEPTION_INFO150 info) => info.dwState &= 4294967278u);
        }

        private void SetAll(UpdateException action)
        {
            var topExceptions = GetExceptions(null);
            ExceptionInfoEnumerator e = null;
            int res2 = 0;

            for (int i = 0; i < topExceptions.Length; i++)
            {
                action(ref topExceptions[i]);
            }

            e = new ExceptionInfoEnumerator(topExceptions.ToList());
            res2 = Session.SetExceptions(e);

            List<EXCEPTION_INFO150> allChildren = new List<EXCEPTION_INFO150>();
            foreach (var topException in topExceptions)
            {
                var childExceptions = GetExceptions(topException);
                for (int i = 0; i < childExceptions.Count(); i++)
                {
                    action(ref childExceptions[i]);
                }
                allChildren.AddRange(childExceptions);
                
            }
            e = new ExceptionInfoEnumerator(allChildren);
            res2 = Session.SetExceptions(e);
        }

        private EXCEPTION_INFO150[] GetExceptions(EXCEPTION_INFO150? parent)
        {
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            //IL_002e: Unknown result type (might be due to invalid IL or missing references)
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
    }
}
