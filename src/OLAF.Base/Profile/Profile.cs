﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLAF
{
    public abstract class Profile : OLAFApi<Profile, Message>
    {
        #region Constructors
        public Profile()
        {
            string artifactsdirName = GetCurrentDirectoryPathTo("data", "artifacts", GetArtifactsDirectoryName());
            if (!Directory.Exists(artifactsdirName))
            {
                ArtifactsDirectory = Directory.CreateDirectory(artifactsdirName);
            }
            else ArtifactsDirectory = new DirectoryInfo(artifactsdirName);
       
            Monitors = new List<IMonitor>();
            Status = ApiStatus.Initializing;
        }
        #endregion

        #region Properties
        public static string[] ImageWildcardExtensions = {"*.bmp", "*.dib", "*.rle", "*.jpg", "*.jpeg", "*.jpe", "*.jfif", "*.gif", "*.tif",
            "*.tiff", "*.png"};

        public DirectoryInfo ArtifactsDirectory { get; }

        public Pipeline Pipeline { get; protected set; }

        public List<IMonitor> Monitors { get; protected set; }
        #endregion

        #region Methods
        public virtual ApiResult Init()
        {
            if (Status != ApiStatus.Initializing) return ApiResult.Failure;

            if (Monitors == null || Monitors.Count == 0)
            {
                throw new InvalidOperationException("No monitors were created.");
            }

            foreach (IMonitor monitor in Monitors)
            {
                if (monitor.Init() != ApiResult.Success)
                {
                    Error("Monitor {0} did not initialize.", monitor.Name);
                    return SetErrorStatusAndReturnFailure();
                }
            }

            if (Pipeline.Init() != ApiResult.Success)
            {
                Error("Pipeline {0} did not initialize.", Pipeline.Name);
                return SetErrorStatusAndReturnFailure();
            }
            else
            {
                return SetInitializedStatusAndReturnSucces();
            }
        }

        public virtual ApiResult Start()
        {
            foreach (IMonitor monitor in Monitors)
            {
                if (monitor.Start() != ApiResult.Success)
                {
                    Error("Monitor {0} did not start.", monitor.Name);
                    return SetErrorStatusAndReturnFailure();
                }
            }

            if (Pipeline.Start() != ApiResult.Success)
            {
                Error("Pipeline {0} did not initialize.", Pipeline.Name);
                return SetErrorStatusAndReturnFailure();
            }
            else
            {
                return SetOkStatusAndReturnSucces();
            }
        }
        public virtual ApiResult Shutdown()
        {
            ThrowIfNotOk();
            Parallel.ForEach(Monitors, m => m.Shutdown());
            if (Monitors.All(m => m.ShutdownCompleted))
            {
                Status = ApiStatus.Ok;
                return ApiResult.Success;
            }
            else
            {
                Error("One or more of the monitors did not shutdown.");
                Status = ApiStatus.Error;
                return ApiResult.Failure;
            }
        }

        [DebuggerStepThrough]
        public string GetArtifactsDirectoryPathTo(params string[] paths) =>
            Path.Combine(ArtifactsDirectory.FullName, Path.Combine(paths));

        [DebuggerStepThrough]
        protected string GetArtifactsDirectoryName() =>
            string.Format("{0:D4}{1:D2}{2:D2}_{3}_{4}", DateTime.Today.Year, DateTime.Today.Month,
                DateTime.Today.Day, Name, DateTime.UtcNow.Ticks);

        #endregion
    }
}
