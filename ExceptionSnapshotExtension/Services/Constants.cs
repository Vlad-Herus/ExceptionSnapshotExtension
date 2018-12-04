using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    static class Constants
    {
        public const uint EXCEPTION_CODE_ENABLE = 17u;
        public const uint EXCEPTION_CODE_DISABLE = 4294967278u;

        public static void EnableException(ref uint exceptioncode)
        {
            exceptioncode |= EXCEPTION_CODE_ENABLE;
        }

        public static void DisableException(ref uint exceptioncode)
        {
            exceptioncode &= EXCEPTION_CODE_DISABLE;
        }

        public static bool SetToBreakFirstChance(uint exceptioncode)
        {
            return (exceptioncode & EXCEPTION_CODE_ENABLE) == EXCEPTION_CODE_ENABLE;
        }
    }
}
