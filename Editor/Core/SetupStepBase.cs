using System;
using UnityEngine;

namespace Prasanna.MobileSetup.Editor
{
    public enum StepStatus
    {
        Pending,
        Running,
        Success,
        Warning,
        Failed
    }

    /// <summary>
    /// Abstract base class for every setup step.
    /// Subclasses only need to implement <see cref="Run"/>.
    /// Status, logging, and exception safety are handled here.
    /// </summary>
    public abstract class SetupStepBase
    {
        // ── Public API ────────────────────────────────────────────────────────────
        public string     Name      { get; protected set; }
        public string     Description { get; protected set; }
        public StepStatus Status    { get; private   set; } = StepStatus.Pending;
        public string     StatusLog { get; private   set; } = string.Empty;

        /// <summary>
        /// Called by the wizard. Wraps <see cref="Run"/> with try/catch and status tracking.
        /// </summary>
        public void Execute()
        {
            Status    = StepStatus.Running;
            StatusLog = string.Empty;

            try
            {
                Run();

                // If Run() didn't call Warn/Succeed explicitly, default to success.
                if (Status == StepStatus.Running)
                    Succeed();
            }
            catch (Exception e)
            {
                Status    = StepStatus.Failed;
                StatusLog = e.Message;
                Debug.LogError($"[MobileSetup] ❌ {Name} failed:\n{e}");
            }
        }

        // ── Abstract ──────────────────────────────────────────────────────────────

        /// <summary>Override this to implement the step logic.</summary>
        protected abstract void Run();

        // ── Status helpers ────────────────────────────────────────────────────────

        protected void Succeed(string message = "")
        {
            Status    = StepStatus.Success;
            StatusLog = message;
        }

        protected void Warn(string message)
        {
            Status    = StepStatus.Warning;
            StatusLog = message;
            Debug.LogWarning($"[MobileSetup] ⚠️ {Name}: {message}");
        }
    }
}
