using AskDelphi.Adapter.AzureNuggets.DTO;
using AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public class NuggetTopicFactory
    {
        private readonly IOperationContext operationContext;
        private readonly Nugget nugget;
        private readonly string nuggetTopicId;
        private readonly string resourceBaseId;
        private readonly List<TopicContent> contents = new List<TopicContent>();
        private readonly TopicContent rootTopic;
        private NuggetTopic rootNuggetTopicContents;
        private readonly FileExtensionContentTypeProvider fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
        private readonly string nuggetTopicType;
        private readonly string imageTopicType;
        private readonly string videoTopicType;

        public NuggetTopicFactory(IOperationContext operationContext, IConfiguration configuration, Nugget nugget, string nuggetTopicId, string resourceBaseId)
        {
            this.operationContext = operationContext;
            this.nugget = nugget;
            this.nuggetTopicId = nuggetTopicId;
            this.resourceBaseId = resourceBaseId;
            rootTopic = new TopicContent();
            contents.Add(rootTopic);


            rootTopic.Guid = new Guid(nugget.Uuid);
            rootTopic.TopicId = this.nuggetTopicId;
            rootTopic.BasicData = new TopicContentBasicData();
            rootTopic.Relations = new TopicContentRelationData();

            nuggetTopicType = configuration.GetValue<string>("Configuration:NuggetTopicType");
            imageTopicType = configuration.GetValue<string>("Configuration:ImageTopicType");
            videoTopicType = configuration.GetValue<string>("Configuration:VideoTopicType");
        }

        public async Task<List<TopicContent>> Build()
        {
            rootNuggetTopicContents = new NuggetTopic();
            rootTopic.BasicData = new TopicContentBasicData();
            rootTopic.Relations = new TopicContentRelationData();

            InitializeRootTopicBasicData();
            CalculateRootTopicContents();

            string contentJson = SerializeContent(rootNuggetTopicContents);
            rootTopic.Content = contentJson;

            return await Task.FromResult<List<TopicContent>>(contents);
        }

        private void CalculateRootTopicContents()
        {
            rootNuggetTopicContents.Description = CleanupHtmlTags(nugget.Description);
            rootNuggetTopicContents.Guid = new Guid(nugget.Uuid);
            rootNuggetTopicContents.IsBodyEmpty = false;
            rootNuggetTopicContents.IsPublished = true;
            rootNuggetTopicContents.Keywords = string.Join(",", (nugget.Keywords ?? new List<Nugget.NuggetTextAndImage>()).Select(x => x.Text));
            rootNuggetTopicContents.Name = nugget.Identifier;
            
            rootNuggetTopicContents.Steps = new List<NuggetStep>();
            foreach (var step in nugget.Steps ?? new List<Nugget.NuggetStep>())
            {
                rootNuggetTopicContents.Steps.Add(CalculateStepContents(step));
            }

            if (null != nugget.Tip)
            {
                rootNuggetTopicContents.TipHTML = $"<html><body>{nugget.Tip.Text}</body></html>";
                rootNuggetTopicContents.TipImageTopicGuid = CreateImageTopicGuidForHref(nugget.Tip.Href, nugget.Title);
            }
            else
            {
                rootNuggetTopicContents.TipHTML = string.Empty;
            }

            rootNuggetTopicContents.Title = CleanupHtmlTags(nugget.Title);
            rootNuggetTopicContents.TitleMarkup = CleanupHtmlTags(nugget.Title);
            rootNuggetTopicContents.TopicLearning = new TopicLearning { DescriptionHTML = $"<html><body>{nugget.Description ?? ""}</body></html>" };
            rootNuggetTopicContents.TopicType = nuggetTopicType;
            rootNuggetTopicContents.Version = new TopicVersion { MajorVersion = 1, MinorVersion = 0 };
            if (nugget.Video?.Sources != null && nugget.Video.Sources.Count >= 1)
            {
                rootNuggetTopicContents.VideoTopicGuid = CreateVideoTopic(nugget.Video);
            }
        }

        private Guid CreateVideoTopic(Nugget.NuggetVideo video)
        {
            // TODO: TOPIC GUIDS SHOULD BE REGISTERED IF WE NEED OT RETRIEVE THEM VIA SEARCH LATER
            // FOR IMAGES, VIDEO'S AND CAPTIONS THIS IS NOT THE CASE THOUGH
            Guid topicGuid = Guid.NewGuid();

            var source = video.Sources.First();
            string resourcePath = $"remote-resource://{operationContext.GetAskDelphiSystemID()}{resourceBaseId}/{source.Href}";

            if(!fileExtensionContentTypeProvider.TryGetContentType(System.IO.Path.GetFileName(source.Href), out string mimeType))
            {
                mimeType = "video/mp4";
            }

            var topicContent = new VideoTopic
            {
                Description = string.Empty,
                Guid = topicGuid,
                IsPublished = true,
                Title = resourcePath,
                TitleMarkup = resourcePath,
                TopicLearning = null,
                TopicType = videoTopicType,
                Version = new TopicVersion { MajorVersion = 1, MinorVersion = 0 },
                ThumbnailTopicGuid = CreateImageTopicGuidForHref(video.Poster, source.Href),
                Alt = source.Href,
                IsStored = true,
                BodyHTML = "<body></body>",
                IsBodyEmpty = true,
                EmbedCodeHTML = null,
                EnableAutoPlay = false,
                Keywords = string.Empty,
                LocalResourceFilePath = resourcePath,
                AlternativeText = "Video",
                ContentType = mimeType,
                MimeType = mimeType,                
            };
            topicContent.Captions = (video.Tracks ?? new List<Nugget.VideoTrack>()).Select(track =>
            {
                string captionResourcePath = $"remote-resource://{operationContext.GetAskDelphiSystemID()}{resourceBaseId}/{track.Href}";
                return new Caption
                {
                    BlobstoreGuid = Guid.NewGuid(),
                    LocalResourceFilePath = captionResourcePath,
                    Filename = System.IO.Path.GetFileName(captionResourcePath),
                    Label = track.Label,
                    Language = track.Language,
                    ResourceExternalContentUri = captionResourcePath,
                    ResourceIsExternal = true,
                };
            }).ToList();
            TopicContent videoTopic = new TopicContent
            {
                Guid = topicGuid,
                Relations = new TopicContentRelationData { References = new List<TopicContentRelationReference>() },
                TopicId = $"{resourceBaseId}/{source.Href}",
                Content = SerializeContent(topicContent),
                BasicData = new TopicContentBasicData
                {
                    Description = string.Empty,
                    Enabled = true,
                    IsDescriptionCalculated = false,
                    IsEmpty = false,
                    IsPublished = true,
                    MetricsTags = null,
                    ModificationDate = DateTime.UtcNow.ToString("o"),
                    Namespace = Namespaces.NamespaceVideo,
                    TopicTitle = resourcePath,
                    TopicTitleMarkup = resourcePath,
                    TopicType = videoTopicType,
                    Version = "1.0"
                }
            };

            contents.Add(videoTopic);
            return videoTopic.Guid;
        }

        private Guid CreateImageTopicGuidForHref(string href, string alt = null)
        {
            // TODO: TOPIC GUIDS SHOULD BE REGISTERED IF WE NEED OT RETRIEVE THEM VIA SEARCH LATER
            // FOR IMAGES, VIDEO'S AND CAPTIONS THIS IS NOT THE CASE THOUGH
            Guid topicGuid = Guid.NewGuid();

            string resourcePath = $"remote-resource://{operationContext.GetAskDelphiSystemID()}{resourceBaseId}/{href}";
            
            // TODO -> Service for portability
            if (!fileExtensionContentTypeProvider.TryGetContentType(System.IO.Path.GetFileName(href), out string mimeType))
            {
                mimeType = "application/octet-stream";
            }

            var topicContent = new ImageTopic
            {
                LocalPathExtraLarge = resourcePath,
                LocalPathLarge= resourcePath,
                LocalPathMedium = resourcePath,
                LocalPathRaw = resourcePath,
                LocalPathSmall = resourcePath,
                Description = string.Empty,
                Guid = topicGuid,
                IsPublished = true,
                MimeType = mimeType,
                Title = resourcePath,
                TitleMarkup = resourcePath,
                TopicLearning = null,
                TopicType = imageTopicType,
                Version = new TopicVersion {  MajorVersion = 1, MinorVersion = 0 }
            };
            TopicContent imageTopic = new TopicContent {
                Guid = topicGuid,
                Relations = new TopicContentRelationData { References = new List<TopicContentRelationReference>() },
                TopicId = resourceBaseId + "/" + href,
                Content = SerializeContent(topicContent),
                BasicData = new TopicContentBasicData
                {
                    Description = string.Empty,
                    Enabled = true,
                    IsDescriptionCalculated = false,
                    IsEmpty = false,
                    IsPublished = true,
                    MetricsTags = null,
                    ModificationDate = DateTime.UtcNow.ToString("o"),
                    Namespace = Namespaces.NamespaceImage,
                    TopicTitle = resourcePath,
                    TopicTitleMarkup = resourcePath,
                    TopicType = imageTopicType,
                    Version = "1.0"
                }
            };

            contents.Add(imageTopic);
            return imageTopic.Guid;
        }

        private NuggetStep CalculateStepContents(Nugget.NuggetStep step)
        {
            NuggetStep result = new NuggetStep
            {
                ClickInstructionHTML = $"<html><body>{step.Clickinstruction}</body></html>",
            };
            if (!string.IsNullOrWhiteSpace(step.Href))
            {
                result.ImageGuid = CreateImageTopicGuidForHref(step.Href, step.Alt);
            }
            if (!string.IsNullOrWhiteSpace(step.Text))
            {
                result.TextHTML = $"<html><body>{step.Text}</body></html>";
            }
            return result;
        }

        private void InitializeRootTopicBasicData()
        {
            rootTopic.BasicData.Description = CleanupHtmlTags(nugget.Description);
            rootTopic.BasicData.Enabled = true;
            rootTopic.BasicData.IsDescriptionCalculated = false;
            rootTopic.BasicData.IsEmpty = false;
            rootTopic.BasicData.IsPublished = true;
            rootTopic.BasicData.MetricsTags = new string[] { };
            rootTopic.BasicData.ModificationDate = DateTime.Now.ToString("o");
            rootTopic.BasicData.Namespace = Namespaces.NamespaceNugget;
            rootTopic.BasicData.TopicTitle = CleanupHtmlTags(nugget.Title);
            rootTopic.BasicData.TopicTitleMarkup = rootTopic.BasicData.TopicTitle;
            rootTopic.BasicData.TopicType = nuggetTopicType;
            rootTopic.BasicData.Version = "1.0";
        }

        private string SerializeContent(object contentObject)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,

            };
            string contentJson = JsonConvert.SerializeObject(contentObject, settings);
            return contentJson;
        }

        private string CleanupHtmlTags(string str)
        {
            string result = str ?? "";
            result = Regex.Replace(result, "&#([0-9]{1,});", (x) => $"{(char)int.Parse(x.Groups[1].Value)}");
            result = Regex.Replace(result, "<b>|</b>|<i>|</i>", "", RegexOptions.IgnoreCase);
            result = System.Web.HttpUtility.HtmlDecode(result);
            return result;
        }
    }
}
