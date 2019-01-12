using ExceptionSnapshotExtension.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    internal abstract class Manager201X<ExceptionType> : IExceptionManager
        where ExceptionType : struct
    {
        private delegate void UpdateException(ref ExceptionType exception, out bool changed);

        public abstract bool SupportsConditions { get; }

        private ExceptionType[] m_TopExceptions;

        protected ExceptionType[] TopExceptions
        {
            get
            {
                if (m_TopExceptions == null)
                {
                    m_TopExceptions = GetDefaultExceptions(null);
                }

                return m_TopExceptions;
            }
        }

        #region  public

        public void DisableAll()
        {
            ThrowIfNoSession();

            SetAll((ref ExceptionType info, out bool changed) =>
            {
                changed = true;
                SetBreakFirstChance(ref info, false);
            });
        }

        public void EnableAll()
        {
            ThrowIfNoSession();

            SetAll((ref ExceptionType info, out bool changed) =>
            {
                changed = true;
                SetBreakFirstChance(ref info, true);
            });
        }

        public Snapshot GetCurrentExceptionSnapshot()
        {
            ThrowIfNoSession();

            var exceptions = TopExceptions.SelectMany(top => GetSetExceptions(top));

            return new Snapshot
            {
                Exceptions = exceptions.Select(ex => ConvertToGeneric(ex)).ToArray()
            };
        }

        public void RestoreSnapshot(Snapshot snapshot)
        {
            ThrowIfNoSession();

            RemoveAllSetExceptions();

            var nativeExceptions = snapshot.
                Exceptions.
                Select(ex => ConvertFromGeneric(ex));
            var topExceptions = nativeExceptions.Where(ex => IsExceptionTopException(ex));

            SetExceptions(topExceptions);
            SetExceptions(nativeExceptions.Except(topExceptions));
        }

        #endregion

        #region Abstract

        public abstract bool SessionAvailable { get; }

        protected abstract ExceptionType[] GetDefaultExceptions(ExceptionType? parent);

        protected abstract ExceptionType[] GetSetExceptions(ExceptionType parent);

        protected abstract void RemoveAllSetExceptions();
        protected abstract void SetExceptions(IEnumerable<ExceptionType> exceptions);

        protected abstract bool IsExceptionTopException(ExceptionType exception);

        protected abstract void SetBreakFirstChance(ref ExceptionType exception, bool breakFirstChance);

        protected abstract ExceptionInfo ConvertToGeneric(ExceptionType exception);
        protected abstract ExceptionType ConvertFromGeneric(ExceptionInfo exception);

        #endregion

        private void SetAll(UpdateException action)
        {
            List<ExceptionType> updated = new List<ExceptionType>();
            for (int i = 0; i < TopExceptions.Length; i++)
            {
                action(ref TopExceptions[i], out bool changed);
                if (changed)
                {
                    updated.Add(TopExceptions[i]);
                }
            }

            if (updated.Any())
            {
                SetExceptions(updated);
                updated.Clear();
            }

            // For some reason top level exceptions (groups) must be passed to IDebugSession150::SetExceptions() separately from normal exceptions
            List<ExceptionType> allChildren = new List<ExceptionType>();
            foreach (var topException in TopExceptions)
            {
                var childExceptions = GetDefaultExceptions(topException);
                for (int i = 0; i < childExceptions.Count(); i++)
                {
                    action(ref childExceptions[i], out bool changed);
                    if (changed)
                    {
                        updated.Add(childExceptions[i]);
                    }
                }
                allChildren.AddRange(childExceptions);
            }

            if (updated.Any())
            {
                SetExceptions(updated);
            }
        }

        private void ThrowIfNoSession()
        {
            if (!SessionAvailable)
            {
                throw new Exception("Session not available.");
            }
        }
    }
}
