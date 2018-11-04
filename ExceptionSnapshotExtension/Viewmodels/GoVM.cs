using ExceptionSnapshotExtension.Model;
using ExceptionSnapshotExtension.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Viewmodels
{
    internal class GoVM
    {
        private readonly ExceptionManager2017 m_ExceptionManager;

        public RelayCommand GoCommand => new RelayCommand(p => true, p =>
           {
               m_ExceptionManager.Go(true, true);
           });

        public bool AddToVSIgnore { get; set; }

        public GoVM(ExceptionManager2017 exceptionManager)
        {
            m_ExceptionManager = exceptionManager;
        }
    }
}
