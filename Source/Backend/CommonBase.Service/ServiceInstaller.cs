using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.ServiceProcess;

using CommonBase.Application;

namespace CommonBase.Service
{
    [RunInstaller(true)]
    public abstract class ServiceInstaller : Installer
    {
        private const int ManagerAllAccessFlag = 0xF003F;
        private const int ServiceAllAccessFlag = 0xF01FF;
        private const int RecoveryOptionsFlag = 0x2;
        private IApplicationInfo _serviceInfo;

        protected ServiceInstaller(IApplicationInfo serviceInfo)
        {
            _serviceInfo = serviceInfo;

            ServiceProcessInstaller process = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };

            System.ServiceProcess.ServiceInstaller installer = new System.ServiceProcess.ServiceInstaller();
            installer.ServiceName = _serviceInfo.Name;
            installer.DisplayName = _serviceInfo.LongName;
            installer.Description = _serviceInfo.Description;
            installer.StartType = ServiceStartMode.Automatic;
            installer.Committed += OnCommitted;

            Installers.Add(process);
            Installers.Add(installer);
        }

        private void OnCommitted(object sender, InstallEventArgs args)
        {
            IntPtr handleManager = IntPtr.Zero;
            IntPtr handleService = IntPtr.Zero;
            IntPtr handleDatabase = IntPtr.Zero;
            IntPtr buffer = IntPtr.Zero;

            try
            {
                // Open the service control manager
                handleManager = OpenSCManager(null, null, ManagerAllAccessFlag);
                if (handleManager.ToInt64() <= 0)
                {
                    throw new InvalidOperationException("Error accessing service control manager.");
                }

                // Lock the Service Database
                handleDatabase = LockServiceDatabase(handleManager);
                if (handleDatabase.ToInt64() <= 0)
                {
                    throw new InvalidOperationException("Error locking service database.");
                }

                // Open the service
                handleService = OpenService(handleManager, _serviceInfo.Name, ServiceAllAccessFlag);
                if (handleService.ToInt64() <= 0)
                {
                    throw new InvalidOperationException("Error opening service '" + _serviceInfo.Name + "'");
                }

                // define actions
                int countActions = 3;
                int[] actions = new int[countActions * 2];
                // first failure
                actions[0] = (int)RecoverAction.Restart;
                actions[1] = 0;
                // second failure
                actions[2] = (int)RecoverAction.Restart;
                actions[3] = 0;
                // any subsequent failure
                actions[4] = (int)RecoverAction.Restart;
                actions[5] = 0;
                // 8 bytes per struct
                buffer = Marshal.AllocHGlobal(countActions * 8);
                Marshal.Copy(actions, 0, buffer, countActions * 2);

                // create the unmanaged struct
                ServiceFailureActions unmanagedActions = new ServiceFailureActions();
                unmanagedActions.cActions = countActions;
                unmanagedActions.dwResetPeriod = 0;
                unmanagedActions.lpCommand = null;
                unmanagedActions.lpRebootMsg = null;
                unmanagedActions.lpsaActions = buffer;

                if (!ChangeServiceFailureActions(handleService, RecoveryOptionsFlag, ref unmanagedActions))
                {
                    throw new Exception("Error while setting service recovery options.");
                }
            }
            finally
            {
                if (handleService != IntPtr.Zero)
                {
                    CloseServiceHandle(handleService);
                }
                if (handleDatabase != IntPtr.Zero)
                {
                    UnlockServiceDatabase(handleDatabase);
                }
                if (handleManager != IntPtr.Zero)
                {
                    CloseServiceHandle(handleManager);
                }
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        #region Native imports

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Native import.")]
        [StructLayout(LayoutKind.Sequential)]
        private struct ServiceFailureActions
        {
            public int dwResetPeriod;
            public string lpRebootMsg;
            public string lpCommand;
            public int cActions;
            public IntPtr lpsaActions;
        }

        private enum RecoverAction
        {
            None = 0,
            Restart = 1,
            Reboot = 2,
            RunCommand = 3
        }

        // Open the service control manager
        [DllImport("advapi32.dll")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Native import.")]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, int dwDesiredAccess);

        // Open a service instance
        [DllImport("advapi32.dll")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Native import.")]
        private static extern IntPtr OpenService(IntPtr hScManager, string lpServiceName, int dwDesiredAccess);

        // Close a service related handle.
        [DllImport("advapi32.dll")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Native import.")]
        public static extern bool CloseServiceHandle(IntPtr handle);

        // Lock the service database to perform write operations.
        [DllImport("advapi32.dll")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Native import.")]
        private static extern IntPtr LockServiceDatabase(IntPtr hScManager);

        // Unlock the service database.
        [DllImport("advapi32.dll")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Native import.")]
        public static extern bool UnlockServiceDatabase(IntPtr hSCManager);

        // Change the service config for the failure actions.
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Native import.")]
        private static extern bool ChangeServiceFailureActions(IntPtr hService, int dwInfoLevel, [MarshalAs(UnmanagedType.Struct)] ref ServiceFailureActions lpInfo);

        #endregion
    }
}