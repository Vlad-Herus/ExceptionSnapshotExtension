using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Debugger.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct EXCEPTION_INFO150
    {
        [MarshalAs(UnmanagedType.Interface)]
        public IDebugProgram2 pProgram;

        [MarshalAs(UnmanagedType.BStr)]
        public string bstrProgramName;

        [MarshalAs(UnmanagedType.BStr)]
        public string bstrExceptionName;

        public uint dwCode;

        [ComAliasName("AD7InteropA.EXCEPTION_STATE")]
        public uint dwState;

        public Guid guidType;

        [MarshalAs(UnmanagedType.Interface)]
        public IDebugExceptionConditionList pConditions;
    }
}
