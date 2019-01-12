using Microsoft.VisualStudio.Debugger.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    internal class ExceptionInfoEnumerator2015 : IEnumDebugExceptionInfo2
    {
        private readonly IList<EXCEPTION_INFO> _data;

        private uint _position;

        private object _syncObj = new object();

        internal ExceptionInfoEnumerator2015(IEnumerable<EXCEPTION_INFO> data)
        {
            _data = data.ToList();
        }

        public int Clone(out IEnumDebugExceptionInfo2 ppEnum)
        {
            ppEnum = null;
            return -2147467263;
        }

        public int GetCount(out uint pcelt)
        {
            pcelt = (uint)_data.Count;
            return 0;
        }

        public int Next(uint celt, EXCEPTION_INFO[] rgelt, ref uint pceltFetched)
        {
            return Move(celt, rgelt, out pceltFetched);
        }

        public int Reset()
        {
            lock (_syncObj)
            {
                _position = 0u;
            }
            return 0;
        }

        public int Skip(uint celt)
        {
            uint celtFetched;
            return Move(celt, null, out celtFetched);
        }

        private int Move(uint celt, EXCEPTION_INFO[] rgelt, out uint celtFetched)
        {
            //IL_004a: Unknown result type (might be due to invalid IL or missing references)
            //IL_004f: Unknown result type (might be due to invalid IL or missing references)
            lock (this)
            {
                int result = 0;
                celtFetched = (uint)(_data.Count - (int)_position);
                if (celt > celtFetched)
                {
                    result = 1;
                }
                else if (celt < celtFetched)
                {
                    celtFetched = celt;
                }
                if (rgelt != null)
                {
                    for (int i = 0; i < celtFetched; i++)
                    {
                        rgelt[i] = _data[(int)_position + i];
                    }
                }
                _position += celtFetched;
                return result;
            }
        }
    }

    public static class IEnumDebugExceptionInfoExtension
    {
        public static EXCEPTION_INFO[] ToArray(this IEnumDebugExceptionInfo2 enumerator)
        {
            uint num = 0;
            uint count = default(uint);
            enumerator.GetCount(out count);
            var array = (EXCEPTION_INFO[])new EXCEPTION_INFO[count];
            enumerator.Next(count, array, ref num);
            return array;
        }
    }
}
