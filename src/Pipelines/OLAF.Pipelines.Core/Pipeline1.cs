﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OLAF.Services.Classifiers;
using OLAF.Services.Extractors;
namespace OLAF.Pipelines
{
    public class Pipeline1 : Pipeline
    {
        public Pipeline1(Profile profile) : base(profile)
        {
            Services.Add(0, new Images(profile, MonitorClients));
            Services.Add(1, new ViolaJonesFaceDetector(profile, typeof(Images)));
            if (Services.All(s => s.Value.Status == ApiStatus.Initializing))
            {
                this.Status = ApiStatus.Initializing;
            }
            else
            {
                this.Status = ApiStatus.Error;
            }
        }

        public override string Description { get; } = "First pipeline";

    }
}
