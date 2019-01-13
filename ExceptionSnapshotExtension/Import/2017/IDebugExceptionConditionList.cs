using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Debugger.Interop
{
    [ComImport]
    [Guid("14FB02FF-2D1B-496A-96C7-2F565BBFCEA4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDebugExceptionConditionList
    {
        [DispId(1610678272)]
        int Count
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        [DispId(0)]
        IDebugExceptionCondition this[[In] int lIndex]
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }
    }
}