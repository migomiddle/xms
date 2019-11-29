namespace Xms.ServerHostManage.Domain
{
    public class SystemInfomation
    {
        /// <summary>
        /// 系统架构
        /// </summary>
        /// <returns></returns>
        public string OSArchitecture { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        /// <returns></returns>
        public string OSDescription { get; set; }

        /// <summary>
        /// 进程架构
        /// </summary>
        /// <returns></returns>
        public string ProcessArchitecture { get; set; }

        /// <summary>
        /// 架构描述
        /// </summary>
        /// <returns></returns>
        public string FrameworkDescription { get; set; }

        /// <summary>
        /// 主机名称
        /// </summary>
        /// <returns></returns>
        public string MachineName { get; set; }

        /// <summary>
        /// 当前工作目录的完全限定路径。
        /// </summary>
        /// <returns></returns>
        public string CurrentDirectory { get; set; }

        ///<summary>
        ///当前计算机上的处理器数。
        /// </summary>
        /// <returns></returns>
        public int ProcessorCount { get; set; }

        /// <summary>
        /// 系统目录的完全限定路径。
        /// </summary>
        /// <returns></returns>
        public string SystemDirectory { get; set; }

        /// <summary>
        /// 操作系统的内存页的字节数。
        /// </summary>
        /// <returns></returns>
        public int SystemPageSize { get; set; }

        /// <summary>
        /// 系统启动后经过的毫秒数。
        /// </summary>
        /// <returns></returns>
        public int TickCount { get; set; }

        /// <summary>
        /// 与当前用户关联的网络域名。
        /// </summary>
        /// <returns></returns>
        public string UserDomainName { get; set; }

        /// <summary>
        /// 映射到进程上下文的物理内存量。
        /// </summary>
        /// <returns></returns>
        public long WorkingSet { get; set; }

        /// <summary>
        /// 逻辑驱动器名称的字符串数组。
        /// </summary>
        /// <returns></returns>
        public string[] LogicalDrives { get; set; }

        /// <summary>
        /// 磁盘信息
        /// </summary>
        /// <returns></returns>
        public DiskInfo[] DiskInfos { get; set; }

        /// <summary>
        /// 已使用CPU
        /// </summary>
        public float CPUCounter { get; set; }

        /// <summary>
        /// 已使用内存
        /// </summary>
        public float RAMCounter { get; set; }

        /// <summary>
        /// CPU名称
        /// </summary>
        public string CPUName { get; set; }
    }

    public class DiskInfo
    {
        public string Name { get; set; }
        public long TotalSize { get; set; }
        public long AvailableFreeSpace { get; set; }
        public long TotalFreeSpace { get; set; }
        public string DriveFormat { get; set; }
        public string DriveType { get; set; }
        public bool IsReady { get; set; }
        public string RootDirectory { get; set; }
        public string VolumeLabel { get; set; }
    }
}