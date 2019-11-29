using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using Xms.ServerHostManage.Domain;

namespace Xms.ServerHostManage.Utility
{
    /// <summary>
    /// 系统信息帮助类
    /// </summary>
    public static class SystemInfomationHelper
    {
        private static PerformanceCounter cpu;
        private static PerformanceCounter ram;

        /// <summary>
        /// 获取系统架构
        /// </summary>
        /// <returns></returns>
        public static Architecture GetOSArchitecture()
        {
            return RuntimeInformation.OSArchitecture;
        }

        /// <summary>
        /// 获取系统名称
        /// </summary>
        /// <returns></returns>
        public static string GetOSDescription()
        {
            return RuntimeInformation.OSDescription;
        }

        /// <summary>
        /// 获取进程架构
        /// </summary>
        /// <returns></returns>
        public static Architecture GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture;
        }

        /// <summary>
        /// 获取架构描述
        /// </summary>
        /// <returns></returns>
        public static string GetFrameworkDescription()
        {
            return RuntimeInformation.FrameworkDescription;
        }

        /// <summary>
        /// 获取主机名称
        /// </summary>
        /// <returns></returns>
        public static string GetMachineName()
        {
            return System.Environment.MachineName;
        }

        /// <summary>
        /// 获取当前工作目录的完全限定路径。
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory()
        {
            return System.Environment.CurrentDirectory;
        }

        ///<summary>
        ///获取当前计算机上的处理器数。
        /// </summary>
        /// <returns></returns>
        public static int GetProcessorCount()
        {
            return System.Environment.ProcessorCount;
        }

        /// <summary>
        /// 获取系统目录的完全限定路径。
        /// </summary>
        /// <returns></returns>
        public static string GetSystemDirectory()
        {
            return System.Environment.SystemDirectory;
        }

        /// <summary>
        /// 获取操作系统的内存页的字节数。
        /// </summary>
        /// <returns></returns>
        public static int GetSystemPageSize()
        {
            return System.Environment.SystemPageSize;
        }

        /// <summary>
        /// 获取系统启动后经过的毫秒数。
        /// </summary>
        /// <returns></returns>
        public static int GetTickCount()
        {
            return System.Environment.TickCount;
        }

        /// <summary>
        /// 获取与当前用户关联的网络域名。
        /// </summary>
        /// <returns></returns>
        public static string GetUserDomainName()
        {
            return System.Environment.UserDomainName;
        }

        /// <summary>
        /// 获取映射到进程上下文的物理内存量。
        /// </summary>
        /// <returns></returns>
        public static long GetWorkingSet()
        {
            return System.Environment.WorkingSet;
        }

        /// <summary>
        /// 获取逻辑驱动器名称的字符串数组。
        /// </summary>
        /// <returns></returns>
        public static string[] GetLogicalDrives()
        {
            return System.Environment.GetLogicalDrives();
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public static DiskInfo[] GetDiskInfos()
        {
            return ConvertToDiskInfo(DriveInfo.GetDrives());
        }

        /// <summary>
        /// 获取磁盘详情
        /// </summary>
        /// <param name="driveInfos"></param>
        /// <returns></returns>
        public static DiskInfo[] ConvertToDiskInfo(DriveInfo[] driveInfos)
        {
            DiskInfo[] diskInfos = new DiskInfo[driveInfos.Length];
            for (int i = 0; i < driveInfos.Length; i++)
            {
                if (driveInfos[i].IsReady)
                {
                    diskInfos[i] = new DiskInfo()
                    {
                        Name = driveInfos[i].Name,
                        TotalSize = driveInfos[i].TotalSize,
                        AvailableFreeSpace = driveInfos[i].AvailableFreeSpace,
                        TotalFreeSpace = driveInfos[i].TotalFreeSpace,
                        DriveFormat = driveInfos[i].DriveFormat,
                        DriveType = driveInfos[i].DriveType.ToString(),
                        IsReady = driveInfos[i].IsReady,
                        RootDirectory = driveInfos[i].RootDirectory.Name,
                        VolumeLabel = driveInfos[i].VolumeLabel
                    };
                }
                else
                {
                    diskInfos[i] = new DiskInfo()
                    {
                        Name = driveInfos[i].Name
                    };
                }
            }
            return diskInfos;
        }

        /// <summary>
        /// 获取CPU使用情况
        /// </summary>
        /// <returns></returns>
        public static float GetCPUCounter()
        {
            try
            {
                SystemInfomationHelper.InitCounter();
                return cpu.NextValue();
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取内存使用情况
        /// </summary>
        /// <returns></returns>
        public static float GetRAMCounter()
        {
            try
            {
                SystemInfomationHelper.InitCounter();
                return ram.NextValue() / 1024 / 1024;
            }
            catch
            {
                return 0;
            }
        }

        public static void InitCounter()
        {
            bool isInit = false;
            if (cpu == null)
            {
                isInit = true;
                cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
            if (ram == null)
            {
                isInit = true;
                ram = new PerformanceCounter("Process", "Working Set", "_Total");
            }
            if (isInit)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 获取CPU序列号代码
        /// </summary>
        /// <returns></returns>
        public static string GetCPUName()
        {
            try
            {
                string name = "";
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    name = mo.Properties["Name"].Value.ToString();
                }
                moc = null;
                mc = null;
                return name;
            }
            catch
            {
                return "";
            }
            finally
            {
            }
        }
    }
}