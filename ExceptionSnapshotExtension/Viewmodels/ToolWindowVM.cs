using ExceptionSnapshotExtension.Model;
using ExceptionSnapshotExtension.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Viewmodels
{
    internal class ToolWindowVM
    {
        private readonly ExceptionManager2017 m_ExceptionManager;
        private RelayCommand m_GoCommand;
        private RelayCommand m_EnableAllCommand;
        private RelayCommand m_DisableAllCommand;

        public bool AutoSkip
        {
            get
            {
                return m_ExceptionManager.AutoSkipExceptions;
            }
            set
            {
                if (m_ExceptionManager.AutoSkipExceptions != value)
                {
                    m_ExceptionManager.AutoSkipExceptions = value;
                }
            }
        }

        public bool AddToIgnoreList
        {
            get
            {
                return m_ExceptionManager.AddExceptionsToIgnoreList;
            }
            set
            {
                if (m_ExceptionManager.AddExceptionsToIgnoreList != value)
                {
                    m_ExceptionManager.AddExceptionsToIgnoreList = value;
                }
            }
        }

        public bool RespectModuleName
        {
            get
            {
                return m_ExceptionManager.RespectModuleName;
            }
            set
            {
                if (m_ExceptionManager.RespectModuleName != value)
                {
                    m_ExceptionManager.RespectModuleName = value;
                }
            }
        }



        public RelayCommand GoCommand
        {
            get
            {
                if (m_GoCommand == null)
                {
                    m_GoCommand = new RelayCommand(p => true, p =>
                    {
                        m_ExceptionManager.Go();
                    });
                }

                return m_GoCommand;
            }
        }

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

        public ToolWindowVM() // For designer
        { }

        public ToolWindowVM(ExceptionManager2017 exceptionManager)
        {
            m_ExceptionManager = exceptionManager;
        }
    }
}
