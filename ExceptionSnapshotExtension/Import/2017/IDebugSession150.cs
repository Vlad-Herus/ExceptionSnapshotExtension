using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Debugger.Interop
{
    [ComImport]
    [Guid("6B762667-EB09-4B7E-AC1A-5BAABCCC412A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugSession150
    {
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int SetExceptions([In] [MarshalAs(UnmanagedType.Interface)] IEnumDebugExceptionInfo150 pExceptionList);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int RemoveSetExceptions([In] [MarshalAs(UnmanagedType.Interface)] IEnumDebugExceptionInfo150 pExceptionList);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int EnumDefaultExceptions([In] [ComAliasName("Microsoft.VisualStudio.Debugger.Interop.EXCEPTION_INFO150")] [MarshalAs(UnmanagedType.LPArray)] EXCEPTION_INFO150[] pParentException, [MarshalAs(UnmanagedType.Interface)] out IEnumDebugExceptionInfo150 ppEnum);

        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
        int EnumSetExceptions([In] ref Guid guidType, [MarshalAs(UnmanagedType.Interface)] out IEnumDebugExceptionInfo150 ppEnum);
    }
}