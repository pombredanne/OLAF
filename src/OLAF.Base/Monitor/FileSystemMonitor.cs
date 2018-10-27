﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using OLAF.Win32;
namespace OLAF
{
    public abstract class FileSystemMonitor<TDetector, TDetectorMessage, TMonitorMessage> : 
        Monitor<TDetector, TDetectorMessage, TMonitorMessage>
        where TDetector : ActivityDetector<TDetectorMessage>
        where TDetectorMessage : Message
        where TMonitorMessage : Message
    {
        #region Constructors
        public FileSystemMonitor(Dictionary<string, string> paths, Profile profile)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }
            else if (paths.Count == 0)
            {
                throw new ArgumentException("No paths specified.", nameof(paths));
            }

            Paths = new Dictionary<DirectoryInfo, string>(paths.Keys.Count);
            Profile = profile;
            foreach (KeyValuePair<string, string> kv in paths)
            {
                try
                {
                    if (Directory.Exists(kv.Key))
                    {
                        DirectoryInfo dir = new DirectoryInfo(kv.Key);
                        string searchPattern = Path.Combine(dir.FullName, kv.Value);
                        var findFileData = new UnsafeNativeMethods.WIN32_FIND_DATA();
                        IntPtr hFindFile = UnsafeNativeMethods.FindFirstFile(searchPattern, ref findFileData);
                        if (hFindFile != UnsafeNativeMethods.INVALID_HANDLE_VALUE)
                        {
                            Debug("Adding {0} {1} to monitored paths.", dir.FullName, kv.Value);
                            Paths.Add(dir, kv.Value);

                        }
                        else
                        {
                            Warn("Path {0} currently has no files matching pattern {1}.", dir.FullName, kv.Value);
                            Debug("Adding {0} {1} to monitored paths.", dir.FullName, kv.Value);
                            Paths.Add(dir, kv.Value);
                        }
                    }
                    else
                    {
                        Warn("The directory {0} does not exist.", kv.Key);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Error(e, "Error occurred enumerating files in directory {0} using search pattern {1}.",
                        kv.Key, kv.Value);
                    Debug("Not adding {0} to monitored paths.", kv.Key);
                    continue;
                }
            }
            if (Paths.Count > 0)
            {
                Status = ApiStatus.Initializing;
            }
            else
            {
                Status = ApiStatus.FileNotFound;
            }
        }
        
        public FileSystemMonitor(string[] directories, string[] extensions, Profile profile)
        {
            if (directories == null)
            {
                throw new ArgumentNullException(nameof(directories));
            }
            else if (directories.Length == 0)
            {
                throw new ArgumentException("No paths specified.", nameof(directories));
            }

            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }
            else if (extensions.Length == 0)
            {
                throw new ArgumentException("No paths specified.", nameof(extensions));
            }

            Paths = new Dictionary<DirectoryInfo, string>(directories.Length * extensions.Length);
            foreach (string d in directories)
            {
                try
                {
                    if (Directory.Exists(d))
                    {
                        foreach (string ext in extensions)
                        {
                            DirectoryInfo dir = new DirectoryInfo(d);
                            try
                            {
                                string searchPattern = Path.Combine(dir.FullName, ext);
                                var findFileData = new UnsafeNativeMethods.WIN32_FIND_DATA();
                                IntPtr hFindFile = UnsafeNativeMethods.FindFirstFile(searchPattern, ref findFileData);
                                if (hFindFile != UnsafeNativeMethods.INVALID_HANDLE_VALUE)
                                {
                                    //Debug("Adding {0} {1} to monitored paths.", dir.FullName, ext);
                                    Paths.Add(dir, ext);

                                }
                                else
                                {
                                    //Warn("Path {0} currently has no files matching pattern {1}.", dir.FullName, ext);
                                    //Debug("Adding {0} {1} to monitored paths.", dir.FullName, ext);
                                    Paths.Add(dir, ext);
                                }
                            }
                            catch (Exception e)
                            {
                                Error(e, "Error occurred enumerating files in directory {0} using search pattern {1}.",
                                    d, ext);
                                Debug("Not adding {0} to monitored paths.", Path.Combine(dir.FullName, ext));
                                continue;
                            }
                        }
                    }
                    else
                    {
                        Warn("The directory {0} does not exist.", d);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Error(e, "Error occurred enumerating files in directory {0}.", d);
                    Debug("Not adding {0} to monitored paths.", d);
                    continue;
                }
            }
            Profile = profile;
            if (Paths.Count > 0)
            {
                Status = ApiStatus.Initializing;
            }
            else
            {
                Status = ApiStatus.FileNotFound;
            }
        }
        #endregion

        #region Properties
        protected Dictionary<DirectoryInfo, string> Paths;
        #endregion
    }
}
