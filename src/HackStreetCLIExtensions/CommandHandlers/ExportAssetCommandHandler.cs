using HackStreetCLIExtensions.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using HackStreetCLIExtensions.Models;
using Stylelabs.M.Base.Querying.Filters;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.Contracts.Querying;
using Stylelabs.M.Sdk.WebClient;
using System.CommandLine.Invocation;
using System.Globalization;
using Stylelabs.M.Base.Querying;
using HackStreetCLIExtensions.Views;

namespace HackStreetCLIExtensions.CommandHandlers
{
    public class ExportAssetCommandHandler : BaseCommandHandler
    {
        public ExportAssetCommandHandler(Lazy<IWebMClient> client, IOutputRenderer renderer, IOptions<ExportAssetParameters> parameters) : base(client, renderer)
        {
            Parameters = parameters?.Value!;
        }
        public ExportAssetParameters Parameters { get; set; }
        /// <summary>
        /// This command exports assets based on given query, fields and location in Excel File.
        /// </summary>
        /// <param name="context">The command invocation context.</param>
        /// <returns>An Excel File if the command is executed successfully.</returns>
        public override Task<int> InvokeAsync(InvocationContext context)
        {
            try
            {
                Renderer.RenderView(new InfoView("Starting Asset Export from Sitecore Content Hub to Excel"));
                Renderer.RenderView(new MessageView($"Input Query: {Parameters.Query}"));
                Renderer.RenderView(new MessageView($"Input Fields: {Parameters.Fields}"));
                Renderer.RenderView(new MessageView($"Input Location: {Parameters.Location}"));                
                IWebMClient client = Client.Value;
                var query = Parameters.Query;
                var location = Parameters.Location;
                var fields = Parameters.Fields;
                Renderer.RenderView(new InfoView("Getting the definition for M.Asset"));
                IEntityDefinition assetDefinition = client.EntityDefinitions.GetAsync("M.Asset").ConfigureAwait(false).GetAwaiter().GetResult();
                if (assetDefinition != null)
                {
                    Renderer.RenderView(new MessageView("The M.Asset definition is found"));                    
                    var queryFilters = Parameters.Query.Split("&");
                    IList<IPropertyDefinition> entityPropertyDefinitions = assetDefinition.GetPropertyDefinitions();
                    IList<IRelationDefinition> entityRelationDefinitions = assetDefinition.GetRelationDefinitions();
                    if (queryFilters.Length > 0)
                    {
                        List<QueryFilter> filters = new List<QueryFilter>();
                        filters.Add(new DefinitionQueryFilter()
                        {
                            Name = "M.Asset"
                        });
                        foreach (var filter in queryFilters)
                        {
                            var filterPair = filter.Split("=");
                            var filterPairKey = filterPair[0];
                            var filterPairValue = filterPair[1];                     
                            IEnumerable<IPropertyDefinition> filterPropertyDefinition = entityPropertyDefinitions.Where(x => x.Name == filterPairKey);
                            IEnumerable<IRelationDefinition> filterRelationDefinition = entityRelationDefinitions.Where(x => x.Name == filterPairKey);
                            if (filterPropertyDefinition.Any())
                            {                                
                                filters.Add(new PropertyQueryFilter
                                {
                                    Property = filterPairKey,
                                    Value = filterPairValue,
                                    DataType = FilterDataType.String
                                });
                            }
                            else if (filterRelationDefinition.Any())
                            {                             
                                var entityId = client.Entities.GetAsync(filterPairValue).ConfigureAwait(false).GetAwaiter().GetResult();   
                                filters.Add(new RelationQueryFilter
                                {
                                    Relation = filterPairKey,
                                    ParentId = entityId.Id
                                });
                            }
                        }
                        var assetQuery = new Query
                        {
                            Filter = new CompositeQueryFilter()
                            {
                                Children = filters,
                                CombineMethod = CompositeFilterOperator.And
                            }
                        };
                        IEntityIterator iterator = client.Querying.CreateEntityIterator(assetQuery, EntityLoadConfiguration.Full);
                        List<JObject> assetEntities = new List<JObject>();
                        var fieldsArray = fields.Split('|');
                        HashSet<string> excelKeys = new HashSet<string>();
                        excelKeys.Add("Identifier");
                        excelKeys.Add("File");
                        excelKeys.Add("FinalLifeCycleStatusToAsset");
                        
                        while (iterator.MoveNextAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                        {
                            Renderer.RenderView(new SuccessView($"Total assets satisfying the query in Sitecore Content Hub: {iterator.Current.TotalNumberOfResults}"));
                            var entities = iterator.Current.Items;                            
                            foreach (var entity in entities)
                            {                              
                                var publicLink = ContentHubHelperExtension.FetchAssetPublicLink(client, entity, Renderer);
                                if (!string.IsNullOrEmpty(publicLink))
                                {
                                    Renderer.RenderView(new SuccessView($"Asset Id: {entity.Id}"));                             
                                    var entityIdentifier = entity.Identifier;
                                    JObject assetEntityObj = new JObject();
                                    assetEntityObj["Identifier"] = entityIdentifier;
                                    assetEntityObj["File"] = publicLink;
                                    assetEntityObj["FinalLifeCycleStatusToAsset"] = "M.Final.LifeCycle.Status.Approved";
                                    foreach (var fieldProp in fieldsArray)
                                    {
                                        var isPropertyField = entityPropertyDefinitions.Where(x => x.Name == fieldProp)?.FirstOrDefault();
                                        var isRelationalField = entityRelationDefinitions.Where(x => x.Name == fieldProp)?.FirstOrDefault();

                                        if (isPropertyField != null)
                                        {
                                            if (isPropertyField.IsMultiLanguage)
                                            {                                                
                                                var excelfieldName = $"{fieldProp}#en-US";
                                                if (!excelKeys.Contains(excelfieldName))
                                                {
                                                    excelKeys.Add(excelfieldName);
                                                }

                                                string propertyValue = entity.GetPropertyValue<string>(fieldProp, CultureInfo.GetCultureInfo("en-US"));
                                                if (!string.IsNullOrEmpty(propertyValue))
                                                {
                                                    assetEntityObj[excelfieldName] = propertyValue;
                                                }
                                                else
                                                {
                                                    assetEntityObj[excelfieldName] = string.Empty;
                                                }
                                            }
                                            else
                                            {
                                                var excelfieldName = $"{fieldProp}";
                                                if (!excelKeys.Contains(excelfieldName))
                                                {
                                                    excelKeys.Add(excelfieldName);
                                                }
                                                string propertyValue = entity.GetPropertyValue<string>(fieldProp);
                                                if (!string.IsNullOrEmpty(propertyValue))
                                                {
                                                    assetEntityObj[excelfieldName] = propertyValue;
                                                }
                                                else
                                                {
                                                    assetEntityObj[excelfieldName] = string.Empty;
                                                }
                                            }
                                        }
                                        else if (isRelationalField != null)
                                        {
                                            var excelfieldName = $"{fieldProp}";
                                            if (!excelKeys.Contains(excelfieldName))
                                            {
                                                excelKeys.Add(excelfieldName);
                                            }
                                            IRelation entityRelation = entity.GetRelation(fieldProp);
                                            List<string> relationalValues = new List<string>();
                                            if (entityRelation != null)
                                            {
                                                var relationalIds = entityRelation.GetIds();
                                                if (relationalIds != null && relationalIds.Any())
                                                {
                                                    foreach (var relationalId in relationalIds)
                                                    {
                                                        var relationalEntity = client.Entities.GetAsync(relationalId).ConfigureAwait(false).GetAwaiter().GetResult();
                                                        if (relationalEntity != null)
                                                        {
                                                            relationalValues.Add(relationalEntity.Identifier);
                                                        }
                                                    }
                                                    var relationalFieldValue = string.Join("|", relationalValues);
                                                    assetEntityObj[fieldProp] = relationalFieldValue;
                                                }
                                            }
                                        }
                                    }
                                    Renderer.RenderView(new SuccessView(JsonConvert.SerializeObject(assetEntityObj)));
                                    assetEntities.Add(assetEntityObj);
                                }
                                else
                                {
                                    Renderer.RenderView(new ErrorView($"Asset Id: {entity.Id}"));
                                    Renderer.RenderView(new ErrorView("Public Link for the entity does not exist. It will be skipped from the export."));
                                }
                            }                            
                        }
                        ContentHubHelperExtension.ExportToExcel(assetEntities, excelKeys, Renderer, location);
                    }
                }                
                return Task.FromResult(0);
            }
            catch (Exception)
            {
                Renderer.RenderView(new ErrorView("Some error occuring while exporting assets"));
                return Task.FromResult(0);
            }
        }
    }
}