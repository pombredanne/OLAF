﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;

namespace OLAF.Services.Classifiers
{
    [Description("Detect faces")]
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
            return SetInitializedStatusAndReturnSucces(); ;
        }

        protected override ApiResult ProcessClientQueueMessage(ImageArtifact artifact)
        {
            if (artifact.HasOCRText)
            {
                Info("Not using face detector on text-rich image artifact {0}.", artifact.Id);
                return ApiResult.Success;
            }

            Bitmap image = artifact.Image;
            using (var op = Begin("Viola-Jones face detection on image artifact {0}.", artifact.Id))
            {
                Rectangle[] objects = Detector.ProcessFrame(image);
                if (objects.Length > 0)
                {
                    if (!artifact.DetectedObjects.ContainsKey(ImageObjectKinds.FaceCandidate))
                    {
                        artifact.DetectedObjects.Add(ImageObjectKinds.FaceCandidate, objects.ToList());
                    }
                    else
                    {
                        artifact.DetectedObjects[ImageObjectKinds.FaceCandidate].AddRange(objects); 
                    }
                }
                Info("Found {0} candidate face object(s) in artifact {1}.", objects.Length, artifact.Id);
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
