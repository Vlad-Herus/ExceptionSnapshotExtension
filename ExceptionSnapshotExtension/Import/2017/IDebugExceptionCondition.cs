using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Debugger.Interop
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("FBFD0196-B9A4-48FC-AB5E-77E4A2EFD887")]
    public interface IDebugExceptionCondition
    {
        [DispId(1610678272)]
        EXCEPTION_CONDITION_TYPE Type
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        [DispId(1610678273)]
        EXCEPTION_CONDITION_CALLSTACK_BEHAVIOR CallStackBehavior
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        [DispId(1610678274)]
        EXCEPTION_CONDITION_OPERATOR Operator
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        [DispId(1610678275)]
        string Value
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }
    }
}