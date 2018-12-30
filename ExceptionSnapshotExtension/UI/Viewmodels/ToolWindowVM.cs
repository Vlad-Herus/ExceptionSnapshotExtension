using ExceptionSnapshotExtension.Model;
using ExceptionSnapshotExtension.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Viewmodels
{
    internal class ToolWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IExceptionManager m_ExceptionManager;
        private RelayCommand m_EnableAllCommand;
        private RelayCommand m_DisableAllCommand;
        private RelayCommand m_SaveSnapshotCommand;
        private RelayCommand m_DeleteSnapshotCommand;
        private RelayCommand m_ActivateSnapshotCommand;

        public ObservableCollection<SnapshotVM> SnapshotVms { get; private set; }

        public IEnumerable<Snapshot> Snapshots
        {
            get
            {
                return SnapshotVms.Select(vm => vm.Snapshot);
            }
            set
            {
                SnapshotVms.Clear();
                foreach (var snapshot in value ?? new Snapshot[] { })
                {
                    SnapshotVms.Add(new SnapshotVM(snapshot));
                }
            }
        }
        public string NewSnapshotName { get; set; }
        
        /// <summary>
        /// One way to source binding
        /// </summary>
        public SnapshotVM SelectedSnapshot { get; set; }
        
        /// <summary>
        /// Two way binding
        /// </summary>
        public int SelectedSnapshotIndex { get; set; }

        public RelayCommand EnableAllCommand
        {
            get
            {
                if (m_EnableAllCommand == null)
                {
                    m_EnableAllCommand = new RelayCommand(p => true, p =>
                    {
                        m_ExceptionManager.EnableAll();
                    });
                }

                return m_EnableAllCommand;
            }
        }

        public RelayCommand DisableAllCommand
        {
            get
            {
                if (m_DisableAllCommand == null)
                {
                    m_DisableAllCommand = new RelayCommand(p => true, p =>
                    {
                        m_ExceptionManager.DisableAll();
                    });
                }

                return m_DisableAllCommand;
            }
        }

        public RelayCommand SaveSnapshotCommand
        {
            get
            {
                if (m_SaveSnapshotCommand == null)
                {
                    m_SaveSnapshotCommand = new RelayCommand(p => true, p =>
                    {
                        var snapshot = m_ExceptionManager.GetCurrentExceptionSnapshot();
                        snapshot.Name = NewSnapshotName;
                        SnapshotVms.Add(new SnapshotVM(snapshot));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SnapshotVms)));
                    });
                }

                return m_SaveSnapshotCommand;
            }
        }

        public RelayCommand DeleteSnapshotCommand
        {
            get
            {
                if (m_DeleteSnapshotCommand == null)
                {
                    m_DeleteSnapshotCommand = new RelayCommand(p => true, p =>
                    {
                        if (SelectedSnapshot != null)
                        {
                            Debug.Assert(SnapshotVms.Contains(SelectedSnapshot));

                            int selectedId = SelectedSnapshotIndex;
                            SnapshotVms.Remove(SelectedSnapshot);
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SnapshotVms)));

                            SelectedSnapshotIndex = SnapshotVms.Count > selectedId ? selectedId : SnapshotVms.Count - 1;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSnapshotIndex)));
                        }
                    });
                }

                return m_DeleteSnapshotCommand;
            }
        }

        public RelayCommand ActivateSnapshotCommand
        {
            get
            {
                if (m_ActivateSnapshotCommand == null)
                {
                    m_ActivateSnapshotCommand = new RelayCommand(p => true, p =>
                    {
                        if (p is SnapshotVM snapshotVM)
                        {
                            m_ExceptionManager.RestoreSnapshot(snapshotVM.Snapshot);
                        }
                    });
                }

                return m_ActivateSnapshotCommand;
            }
        }

        public ToolWindowVM() // For designer
        {
            SnapshotVms = new ObservableCollection<SnapshotVM>
            (
                new SnapshotVM[]
                {
                    new SnapshotVM(new Snapshot
                    {
                        Name = "TEst Shot"
                    })
                }
            );
        }

        public ToolWindowVM(IExceptionManager exceptionManager)
        {
            m_ExceptionManager = exceptionManager;
            SnapshotVms = new ObservableCollection<SnapshotVM>();
        }
    }
}
