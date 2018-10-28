﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;


namespace OLAF.Services.Classifiers
{
    public class ViolaJonesFaceDetector : Service<ImageArtifact, ImageArtifact>
    {
        #region Constructors
        public ViolaJonesFaceDetector(Profile profile, params Type[] clients) : base(profile, clients)
        {
            Cascade = new FaceHaarCascade();
            Detector = new HaarObjectDetector(Cascade, 30);
            Status = ApiStatus.Initializing;
        }
        #endregion

        #region Overridden members
        public override ApiResult Init()
        {
            Detector.SearchMode = ObjectDetectorSearchMode.NoOverlap;
            Detector.ScalingMode = ObjectDetectorScalingMode.SmallerToGreater;
            Detector.ScalingFactor = 1.5f;
            Detector.UseParallelProcessing = true;
            Detector.Suppression = 2;
            Status = ApiStatus.Initialized;
            Info("Accord.NET Viola-Jones face detector initialized.");
            return ApiResult.Success;
        }

        protected override ApiResult ProcessClientQueueMessage(ImageArtifact artifact)
        {
            Bitmap image = artifact.Image;
            using (var op = Begin("Viola-Jones face detection"))
            {
                Rectangle[] objects = Detector.ProcessFrame(image);
                if (objects.Length > 0)
                {
                    if (!artifact.DetectedObjects.ContainsKey(ImageObjectKinds.Face))
                    {
                        artifact.DetectedObjects.Add(ImageObjectKinds.Face, objects.ToList());
                    }
                    else
                    {
                        artifact.DetectedObjects[ImageObjectKinds.Face].AddRange(objects); 
                    }
                }
                Info("Found {0} objects.", objects.Length);
                Global.MessageQueue.Enqueue<ViolaJonesFaceDetector>(artifact);
                op.Complete();
                
            }
            return ApiResult.Success;
        }
        #endregion

        #region Properties
        HaarCascade Cascade { get; }
        HaarObjectDetector Detector { get; }
        #endregion'
    }
}
