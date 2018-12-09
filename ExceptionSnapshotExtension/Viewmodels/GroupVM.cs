﻿using ExceptionSnapshotExtension.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Viewmodels
{
    internal class GroupVM : IListItemVM
    {
        public bool Break { get; set; }
        public ObservableCollection<ExceptionInfoVM> Exceptions { get; private set; }

        public string Name => "Group";

        public GroupVM(bool breakFirstCHnace, IEnumerable<ExceptionInfoVM> exceptions)
        {
            Exceptions = new ObservableCollection<ExceptionInfoVM>(exceptions);
            Break = breakFirstCHnace;
        }
    }
}
