using System;
using System.Runtime.InteropServices;

namespace Princess.Tray.App.Helpers
{
    internal sealed class ShutDownApi
    {
        internal const string ADVAPI32 = "advapi32.dll";
        internal const string KERNEL32 = "kernel32.dll";
        internal const string USER32 = "user32.dll";

        internal const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        internal const uint TOKEN_QUERY = 0x0008;

        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        internal const uint EWX_SHUTDOWN = 0x00000001;
        internal const uint EWX_FORCE = 0x00000004;

        internal const uint SE_PRIVILEGE_DISABLED = 0x00000000;
        internal const uint SE_PRIVILEGE_ENABLED = 0x00000002;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LUID
        {
            internal uint LowPart;
            internal uint HighPart;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LUID_AND_ATTRIBUTES
        {
            internal LUID Luid;
            internal uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct TOKEN_PRIVILEGE
        {
            internal uint PrivilegeCount;
            internal LUID_AND_ATTRIBUTES Privilege;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            public LUID_AND_ATTRIBUTES Privileges;
        }

        [DllImport(KERNEL32)]
        static extern IntPtr GetCurrentProcess();

        [DllImport(ADVAPI32)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport(ADVAPI32)]
        static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        [DllImport(USER32)]
        static extern int ExitWindowsEx(uint uFlags, int dwReason);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint Zero, IntPtr Null1, IntPtr Null2);

        public static void ShutDown()
        {
            SetupPrivilages();
            ExitWindowsEx(EWX_SHUTDOWN, 0);
        }

        private static void SetupPrivilages()
        {
            TOKEN_PRIVILEGES tokenPrivilages;

            if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out var tokenHandle))
            {
                Console.WriteLine("Failed to open process token!");
            }

            if (!LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, out tokenPrivilages.Privileges.Luid))
            {
                Console.WriteLine("Failed to look up privilage!");
            }

            tokenPrivilages.PrivilegeCount = 1;
            tokenPrivilages.Privileges.Attributes = SE_PRIVILEGE_ENABLED;

            if (!AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivilages, 0U, IntPtr.Zero, IntPtr.Zero))
            {
                Console.WriteLine("Failed to adjust token priovilages!");
            }
        }
    }
}
