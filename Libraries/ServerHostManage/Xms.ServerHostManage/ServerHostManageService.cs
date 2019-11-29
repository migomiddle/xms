using Xms.ServerHostManage.Domain;

namespace Xms.ServerHostManage
{
    /// <summary>
    /// 服务器主机管理服务
    /// </summary>
    public class ServerHostManageService : IServerHostManageService
    {
        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public SystemInfomation GetSystemInfomation()
        {
            return new SystemInfomation()
            {
                OSArchitecture = Utility.SystemInfomationHelper.GetOSArchitecture().ToString(),
                OSDescription = Utility.SystemInfomationHelper.GetOSDescription(),
                ProcessArchitecture = Utility.SystemInfomationHelper.GetProcessArchitecture().ToString(),
                FrameworkDescription = Utility.SystemInfomationHelper.GetFrameworkDescription(),
                MachineName = Utility.SystemInfomationHelper.GetMachineName(),
                CurrentDirectory = Utility.SystemInfomationHelper.GetCurrentDirectory(),
                ProcessorCount = Utility.SystemInfomationHelper.GetProcessorCount(),
                SystemDirectory = Utility.SystemInfomationHelper.GetSystemDirectory(),
                SystemPageSize = Utility.SystemInfomationHelper.GetSystemPageSize(),
                TickCount = Utility.SystemInfomationHelper.GetTickCount(),
                UserDomainName = Utility.SystemInfomationHelper.GetUserDomainName(),
                WorkingSet = Utility.SystemInfomationHelper.GetWorkingSet(),
                LogicalDrives = Utility.SystemInfomationHelper.GetLogicalDrives(),
                DiskInfos = Utility.SystemInfomationHelper.GetDiskInfos(),
                CPUCounter = Utility.SystemInfomationHelper.GetCPUCounter(),
                RAMCounter = Utility.SystemInfomationHelper.GetRAMCounter(),
                CPUName = Utility.SystemInfomationHelper.GetCPUName()
            };
        }
    }
}