using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using AIServices.Models;

namespace AIServices.APIs;

public class CustomVision
{
    private const string Key = "YOUR_CUSTOM_VISION_KEY";
    private const string Endpoint = "YOUR_ENDPOINT";
    private const string PublishedModelName = "YOUR_MODEL_NAME";
    private const string ProjectId = "YOUR_PROJECT_ID";
    private const double MinProbability = 0.75;

    private readonly Project _project;

    private readonly CustomVisionTrainingClient _trainingApi;
    private readonly CustomVisionPredictionClient _predictionApi;

    public CustomVision() 
    {
        _trainingApi = AuthenticateTraining(Endpoint, Key);
        _predictionApi = AuthenticatePrediction(Endpoint, Key);

        _project = _trainingApi.GetProject(new Guid(ProjectId));
    }

    private CustomVisionTrainingClient AuthenticateTraining(string endpoint, string trainingKey)
    {
        // Create the Api, passing in the training key
        return new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(trainingKey))
        {
            Endpoint = endpoint
        };
    }

    private CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
    {
        // Create a prediction endpoint, passing in the obtained prediction key
        return new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };
    }

    public async Task<List<CustomVisionModel>> DetectObjectsAsync(string imageFile, string imageFileOut)
    {

        // Make a prediction with the project specified
        using (var stream = File.Open(imageFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            var result = await _predictionApi.DetectImageAsync(_project.Id, PublishedModelName, stream);

            // Loop over each prediction and write out the results
            var img = System.Drawing.Image.FromFile(imageFile);
            var g = Graphics.FromImage(img);

            var BottleColor = new SolidBrush(Color.DarkBlue);
            var FaceColor = new SolidBrush(Color.Brown);
            var GuitarColor = new SolidBrush(Color.LightYellow);
            var HatColor = new SolidBrush(Color.DarkRed);

            var list = new List<CustomVisionModel>();

            foreach (var c in result.Predictions)
            {
                if (!(c.Probability > MinProbability)) continue;
                var Model = new CustomVisionModel { TagName = c.TagName, Probability = c.Probability*100 };

                switch (c.TagName)
                {
                    case "Bottle":
                        g.FillRectangle(BottleColor, (int)(c.BoundingBox.Left * img.Width), (int)(c.BoundingBox.Top * img.Height), (int)(c.BoundingBox.Width * img.Width), (int)(c.BoundingBox.Height * img.Height));
                        break;
                    case "Face":
                        g.FillRectangle(FaceColor, (int)(c.BoundingBox.Left * img.Width), (int)(c.BoundingBox.Top * img.Height), (int)(c.BoundingBox.Width * img.Width), (int)(c.BoundingBox.Height * img.Height));
                        break;
                    case "Guitar":
                        g.FillRectangle(GuitarColor, (int)(c.BoundingBox.Left * img.Width), (int)(c.BoundingBox.Top * img.Height), (int)(c.BoundingBox.Width * img.Width), (int)(c.BoundingBox.Height * img.Height));
                        break;
                    case "Hat":
                        g.FillRectangle(HatColor, (int)(c.BoundingBox.Left * img.Width), (int)(c.BoundingBox.Top * img.Height), (int)(c.BoundingBox.Width * img.Width), (int)(c.BoundingBox.Height * img.Height));
                        break;
                }

                list.Add(Model);
            }

            img.Save(imageFileOut);
            return list;
        }
    }
}