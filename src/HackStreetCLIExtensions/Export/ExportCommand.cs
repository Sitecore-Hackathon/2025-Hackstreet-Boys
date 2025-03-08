﻿using HackStreetCLIExtensions.Extensions;
using Microsoft.Extensions.Options;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using SitecoreCHCLIExtensions.Models;
using Stylelabs.M.Base.Querying;
using Stylelabs.M.Base.Querying.Filters;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Sdk.Contracts.Querying;
using Stylelabs.M.Sdk.WebClient;
using System.CommandLine.Invocation;
using System.Dynamic;

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
            Renderer.WriteLine(this.Parameters.Query);
            Renderer.WriteLine(this.Parameters.Fields);
            Renderer.WriteLine(this.Parameters.Location);
            // Return exit code to indicate success or failure
            var client = this.Client.Value;
            var query = this.Parameters.Query;
            var location = this.Parameters.Location;
            var assetDefinition = client.EntityDefinitions.GetAsync("M.Asset").ConfigureAwait(false).GetAwaiter().GetResult();
            if (assetDefinition != null)
            {
                Renderer.WriteLine("Asset Id 1");
                var queryFilters = this.Parameters.Query.Split(",");
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
                        var filterPropertyDefintion = assetDefinition.GetPropertyDefinitions().Where(x => x.Name == filterPairKey);
                        var filterRelationDefintion = assetDefinition.GetRelationDefinitions().Where(x => x.Name == filterPairKey);
                        if (filterPropertyDefintion.Any())
                        {
                            Renderer.WriteLine("Property Relation");
                            filters.Add(new PropertyQueryFilter
                            {
                                Property = filterPairKey,
                                Value = filterPairValue,
                                DataType = FilterDataType.String
                            });
                        }
                        else if (filterRelationDefintion.Any())
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
                    List<dynamic> assetEntities = new List<dynamic>();
                    while (iterator.MoveNextAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                    {
                        var entities = iterator.Current.Items;
                        Renderer.WriteLine("Asset Count :");
                        Renderer.WriteLine(entities.Count.ToString());
                        foreach (var entity in entities)
                        {
                            Renderer.WriteLine("Asset Id :");
                            Renderer.WriteLine(entity.Id.ToString());

                            var entityTitle = entity.GetPropertyValue("Title");
                            var entityIdentifier = entity.Identifier;
                            var publicLink = ContentHubHelperExtension.FetchAssetPublicLink(client, entity, Renderer);
                            Renderer.WriteLine("Public Link :");
                            Renderer.WriteLine(publicLink);
                            dynamic assetEntityObj = new ExpandoObject();
                            assetEntityObj.Title = entityTitle;
                            assetEntityObj.Identifier = entityIdentifier;   
                            assetEntityObj.PublicLink = publicLink;
                            assetEntities.Add(assetEntityObj);
                        }
                    }
                    ContentHubHelperExtension.ExportToExcel(assetEntities, location);
                }
            }
            return Task.FromResult(0);
        }
    }
}
