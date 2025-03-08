using HackStreetCLIExtensions.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using SitecoreCHCLIExtensions.Models;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Filters;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.Contracts.Querying;
using Stylelabs.M.Sdk.WebClient;
using System.CommandLine.Invocation;
using System.Dynamic;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SitecoreCHCLIExtensions.Export
{
    public class ExportCommand : BaseCommand<ExportCommandHandler>
    {
        public ExportCommand() : base("export", "Export Assets")
        {
            AddOption<string>(ExportMessages.ExportCommandQuery, false, new string[2]
            {
                "--query",
                "-q"
            });
            AddOption<string>(ExportMessages.ExportCommandLocation, false, new string[2]
            {
                "--location",
                "-l"
            });
            AddOption<string>(ExportMessages.ExportCommandFields, false, new string[2]
            {
                "--fields",
                "-f"
            });
        }
    }
    public class ExportCommandHandler : BaseCommandHandler
    {
        public ExportCommandHandler(Lazy<IWebMClient> client, IOutputRenderer renderer, IOptions<ExportParameters> parameters) : base(client, renderer)
        {
            this.Parameters = parameters?.Value;
        }
        public ExportParameters Parameters { get; set; }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Renderer.WriteLine(Parameters.Query);
            Renderer.WriteLine(Parameters.Fields);
            Renderer.WriteLine(Parameters.Location);
            // Return exit code to indicate success or failure
            var client = Client.Value;
            var query = Parameters.Query;
            var location = Parameters.Location;
            var fields = Parameters.Fields;
            var assetDefinition = client.EntityDefinitions.GetAsync("M.Asset").ConfigureAwait(false).GetAwaiter().GetResult();
            if (assetDefinition != null)
            {
                Renderer.WriteLine("Asset Id 1");
                var queryFilters = Parameters.Query.Split(",");
                var entityPropertyDefinitions = assetDefinition.GetPropertyDefinitions();
                var entityRelationDefinitions = assetDefinition.GetRelationDefinitions();
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
                        Renderer.WriteLine(filterPairKey);
                        Renderer.WriteLine(filterPairValue);
                        var filterPropertyDefinition = entityPropertyDefinitions.Where(x => x.Name == filterPairKey);
                        var filterRelationDefinition = entityRelationDefinitions.Where(x => x.Name == filterPairKey);
                        if (filterPropertyDefinition.Any())
                        {
                            Renderer.WriteLine("Property Relation");
                            filters.Add(new PropertyQueryFilter
                            {
                                Property = filterPairKey,
                                Value = filterPairValue,
                                DataType = FilterDataType.String
                            });
                        }
                        else if (filterRelationDefinition.Any())
                        {
                            Renderer.WriteLine("Query Relation");
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
                    var fieldsArray = fields.Split(',');
                    HashSet<string> excelKeys = new HashSet<string>();
                    excelKeys.Add("Identifier");
                    excelKeys.Add("File");
                    while (iterator.MoveNextAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                    {
                        var entities = iterator.Current.Items;
                        Renderer.WriteLine("Asset Count :");
                        Renderer.WriteLine(entities.Count.ToString());
                        foreach (var entity in entities)
                        {
                            Renderer.WriteLine("Asset Id :");
                            Renderer.WriteLine(entity.Id.ToString());
                            JObject assetEntityObj = new JObject();
                            var entityIdentifier = entity.Identifier;
                            var publicLink = ContentHubHelperExtension.FetchAssetPublicLink(client, entity, Renderer);
                            Renderer.WriteLine("Public Link :");
                            Renderer.WriteLine(publicLink);
                            assetEntityObj["Identifier"] = entityIdentifier;
                            assetEntityObj["File"] = publicLink;
                            foreach (var fieldProp in fieldsArray)
                            {
                                var isPropertyField = entityPropertyDefinitions.Where(x => x.Name == fieldProp)?.FirstOrDefault();
                                var isRelationalField = entityRelationDefinitions.Where(x => x.Name == fieldProp)?.FirstOrDefault();

                                if (isPropertyField != null)
                                {
                                    if (isPropertyField.IsMultiLanguage)
                                    {
                                        Renderer.WriteLine("is Multi");
                                        var excelfieldName = $"{fieldProp}#en-US";
                                        if (!excelKeys.Contains(excelfieldName))
                                        {
                                            excelKeys.Add(excelfieldName);
                                        }
                                       
                                        Renderer.WriteLine(fieldProp);
                                        string propertyValue = entity.GetPropertyValue<string>(fieldProp, CultureInfo.GetCultureInfo("en-US"));
                                        Renderer.WriteLine(propertyValue);
                                        assetEntityObj[excelfieldName] = propertyValue;
                                        Renderer.WriteLine("is MultiCompleted");
                                    }
                                    else
                                    {
                                        Renderer.WriteLine("Non multi");
                                        var excelfieldName = $"{fieldProp}";
                                        if (!excelKeys.Contains(excelfieldName))
                                        {
                                            excelKeys.Add(excelfieldName);
                                        }
                                        string propertyValue = entity.GetPropertyValue<string>(fieldProp);
                                        Renderer.WriteLine(propertyValue);
                                        assetEntityObj[fieldProp] = propertyValue;
                                        Renderer.WriteLine("Non multi MultiCompleted");
                                    }
                                }
                                else if (isRelationalField != null)
                                {
                                    Renderer.WriteLine("Relational Id 1");
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
                                            Renderer.WriteLine(string.Join("|", relationalValues));
                                            assetEntityObj[fieldProp] = string.Join("|", relationalValues);
                                        }
                                    }
                                }
                            }
                            Renderer.WriteLine("JSON");
                            Renderer.RenderJson(assetEntityObj);
                            assetEntities.Add(assetEntityObj);
                        }
                        Renderer.RenderJson(assetEntities);
                        Renderer.RenderJson(excelKeys);
                        Renderer.WriteLine(JsonConvert.SerializeObject(assetEntities));
                    }
                    ContentHubHelperExtension.ExportToExcel(assetEntities, excelKeys, location);
                }
            }
            return Task.FromResult(0);
        }
    }
}