﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLAF
{
    public abstract class Detector : OLAFApi<Detector>
    {
        #region Constructors
        static Detector()
        {
            if (Global.Logger == null)
            {
                Global.SetupLogger(() => new SimpleConsoleLogger());
            }
        }
        #endregion

        #region Properties
        public Type MonitorType { get; protected set; }
        public int ProcessId { get; protected set; }
        #endregion
    }
}
