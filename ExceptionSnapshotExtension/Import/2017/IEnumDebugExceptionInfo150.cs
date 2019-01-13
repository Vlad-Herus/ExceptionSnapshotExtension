using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Debugger.Interop
{
    [ComImport]
    [Guid("418C94A5-23B8-461E-A117-69543CE0DB36")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumDebugExceptionInfo150
    {
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int Next([In] uint celt, [Out] [ComAliasName("Microsoft.VisualStudio.Debugger.Interop.EXCEPTION_INFO150")] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] EXCEPTION_INFO150[] rgelt, [In] [Out] ref uint pceltFetched);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int Skip([In] uint celt);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int Reset();

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int Clone([MarshalAs(UnmanagedType.Interface)] out IEnumDebugExceptionInfo150 ppEnum);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int GetCount(out uint pcelt);
    }
}