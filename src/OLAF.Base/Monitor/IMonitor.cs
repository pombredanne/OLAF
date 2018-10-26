﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLAF
{
    public interface IMonitor
    {
        ApiResult Init();
        ApiResult Start();
        ApiResult Shutdown();
        ApiStatus Status { get; }
        bool ShutdownRequested { get; }
        bool ShutdownCompleted { get; }
    }
}
