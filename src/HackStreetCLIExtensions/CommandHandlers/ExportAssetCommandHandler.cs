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
            this.Parameters = parameters?.Value;
        }
        public ExportAssetParameters Parameters { get; set; }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Renderer.RenderView(new InfoView("Export assets (START)"));
            Renderer.RenderView(new MessageView($"Input Query: {Parameters.Query}"));
            Renderer.RenderView(new MessageView($"Input Fields: {Parameters.Fields}"));
            Renderer.RenderView(new MessageView($"Input Location: {Parameters.Location}"));
            //Renderer.WriteLine(Parameters.Query);
            //Renderer.WriteLine(Parameters.Fields);
            //Renderer.WriteLine(Parameters.Location);
            // Return exit code to indicate success or failure
            IWebMClient client = Client.Value;
            var query = Parameters.Query;
            var location = Parameters.Location;
            var fields = Parameters.Fields;
            Renderer.RenderView(new InfoView("Getting M.Asset definition"));
            IEntityDefinition assetDefinition = client.EntityDefinitions.GetAsync("M.Asset").ConfigureAwait(false).GetAwaiter().GetResult();
            if (assetDefinition != null)
            {
                Renderer.RenderView(new MessageView("Found M.Asset definition"));
                //Renderer.RenderView(new MessageView("Asset Id 1"));
                //Renderer.WriteLine("Asset Id 1");
                var queryFilters = Parameters.Query.Split("&");
                IList<IPropertyDefinition> entityPropertyDefinitions = assetDefinition.GetPropertyDefinitions();
                IList<IRelationDefinition> entityRelationDefinitions = assetDefinition.GetRelationDefinitions();
                if (queryFilters.Length > 0)
                {
                    Renderer.RenderView(new InfoView("Iterating query filters"));
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
                        Renderer.RenderView(new WarningView(filterPairKey));
                        Renderer.RenderView(new WarningView(filterPairValue));
                        //Renderer.WriteLine(filterPairKey);
                        //Renderer.WriteLine(filterPairValue);
                        IEnumerable<IPropertyDefinition> filterPropertyDefinition = entityPropertyDefinitions.Where(x => x.Name == filterPairKey);
                        IEnumerable<IRelationDefinition> filterRelationDefinition = entityRelationDefinitions.Where(x => x.Name == filterPairKey);
                        if (filterPropertyDefinition.Any())
                        {
                            Renderer.RenderView(new MessageView("Property Relation"));
                            //Renderer.WriteLine("Property Relation");
                            filters.Add(new PropertyQueryFilter
                            {
                                Property = filterPairKey,
                                Value = filterPairValue,
                                DataType = FilterDataType.String
                            });
                        }
                        else if (filterRelationDefinition.Any())
                        {
                            Renderer.RenderView(new MessageView("Query Relation"));
                            //Renderer.WriteLine("Query Relation");
                            filters.Add(new RelationQueryFilter
                            {
                                Relation = filterPairKey,
                                ParentId = int.Parse(filterPairValue)
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
                        var entities = iterator.Current.Items;
                        Renderer.RenderView(new SuccessView($"Asset count: {entities.Count}"));
                        //Renderer.WriteLine("Asset Count :");
                        //Renderer.WriteLine(entities.Count.ToString());
                        foreach (var entity in entities)
                        {
                            Renderer.RenderView(new MessageView($"Asset Id: {entity.Id}"));
                            //Renderer.WriteLine("Asset Id :");
                            //Renderer.WriteLine(entity.Id.ToString());
                            var publicLink = ContentHubHelperExtension.FetchAssetPublicLink(client, entity, Renderer);
                            if (!string.IsNullOrEmpty(publicLink)){
                                Renderer.RenderView(new MessageView($"Public link: {publicLink}"));
                                //Renderer.WriteLine("Public Link :");
                                //Renderer.WriteLine(publicLink);
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
                                            Renderer.RenderView(new WarningView($"Is Multi"));
                                            //Renderer.WriteLine("is Multi");
                                            var excelfieldName = $"{fieldProp}#en-US";
                                            if (!excelKeys.Contains(excelfieldName))
                                            {
                                                excelKeys.Add(excelfieldName);
                                            }

                                            Renderer.RenderView(new MessageView($"Field prop: {fieldProp}"));
                                            //Renderer.WriteLine(fieldProp);
                                            string propertyValue = entity.GetPropertyValue<string>(fieldProp, CultureInfo.GetCultureInfo("en-US"));
                                            if (!string.IsNullOrEmpty(propertyValue))
                                            {
                                                Renderer.RenderView(new MessageView($"{propertyValue}"));
                                                //Renderer.WriteLine(propertyValue);
                                                assetEntityObj[excelfieldName] = propertyValue;
                                                Renderer.RenderView(new WarningView($"Is MultiCompleted"));
                                                //Renderer.WriteLine("is MultiCompleted");
                                            }
                                            else
                                            {
                                                assetEntityObj[excelfieldName] = string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            Renderer.RenderView(new WarningView($"Non Multi"));
                                            //Renderer.WriteLine("Non multi");
                                            var excelfieldName = $"{fieldProp}";
                                            if (!excelKeys.Contains(excelfieldName))
                                            {
                                                excelKeys.Add(excelfieldName);
                                            }
                                            string propertyValue = entity.GetPropertyValue<string>(fieldProp);
                                            if (!string.IsNullOrEmpty(propertyValue))
                                            {
                                                Renderer.RenderView(new MessageView(propertyValue));
                                                //Renderer.WriteLine(propertyValue);
                                                assetEntityObj[excelfieldName] = propertyValue;
                                                Renderer.RenderView(new WarningView($"Non multi MultiCompleted"));
                                                //Renderer.WriteLine("Non multi MultiCompleted");
                                            }
                                            else
                                            {
                                                assetEntityObj[excelfieldName] = string.Empty;
                                            }
                                        }
                                    }
                                    else if (isRelationalField != null)
                                    {
                                        Renderer.RenderView(new MessageView("Relational Id 1"));
                                        //Renderer.WriteLine("Relational Id 1");
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
                                                Renderer.RenderView(new InfoView(relationalFieldValue));
                                                //Renderer.WriteLine(string.Join("|", relationalValues));
                                                assetEntityObj[fieldProp] = relationalFieldValue;
                                            }
                                        }
                                    }
                                }
                                Renderer.RenderView(new SuccessView("Entity JSON"));
                                //Renderer.WriteLine("JSON");
                                Renderer.RenderJson(assetEntityObj);
                                assetEntities.Add(assetEntityObj);
                            }
                            else
                            {
                                Renderer.RenderView(new WarningView("Public Link does not exist"));
                                //Renderer.WriteLine("Public Link does not exist");
                            }
                        }
                        //Renderer.RenderJson(assetEntities);
                        Renderer.RenderJson(excelKeys);
                        Renderer.RenderView(new InfoView(JsonConvert.SerializeObject(assetEntities)));
                    }
                    ContentHubHelperExtension.ExportToExcel(assetEntities, excelKeys, location);
                }
            }
            //Renderer.RenderView(new InfoView("Export assets (END)"));
            return Task.FromResult(0);
        }
    }
}
