﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OLAF
{
    public class FileArtifact : Artifact
    {
        #region Constructors
        public FileArtifact(long id, string artifactPath) : base(id)
        {
            Path = artifactPath;
        }

        public FileArtifact(long id, string artifactPath, byte[] artifactFileData) : this(id, artifactPath)
        {
            Data = artifactFileData;
        }
        #endregion

        #region Properties
        public string Path { get; }
        public string Name => Path?.GetPathFilename();
        public bool FileLocked { get; set; } = false;
        public int FileOpenAttempts { get; set; } = 0;
        public byte[] Data { get; set; }
        public bool HasData => Data != null && Data.Length > 0;
        #endregion
    }
}
