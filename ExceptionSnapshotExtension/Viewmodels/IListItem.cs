using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Viewmodels
{
    public interface IListItemVM
    {
       string Name { get; }
       bool Break { get; set; }
    }
}
