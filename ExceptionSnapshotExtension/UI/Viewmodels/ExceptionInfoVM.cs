using ExceptionSnapshotExtension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Viewmodels
{
    class ExceptionInfoVM : IListItemVM
    {
        private readonly ExceptionInfo m_Info;

        public string Name => m_Info.Name;

        public bool Break { get => m_Info.BreakFirstChance; set => m_Info.BreakFirstChance = value; }

        public ExceptionInfoVM(ExceptionInfo info)
        {
            m_Info = info;
        }
    }
}
