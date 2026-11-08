using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using PolicySearchService.Domain;

namespace PolicySearchService.DataAccess.ElasticSearch;

public class PolicyRepository : IPolicyRepository
{
    private readonly ElasticsearchClient elasticClient;

    public PolicyRepository(ElasticsearchClient elasticClient)
    {
        this.elasticClient = elasticClient;
    }

    public async Task Add(Policy policy)
    {
        var result = await elasticClient.IndexAsync(policy);
        if (!result.IsValidResponse)
            throw new InvalidOperationException($"Failed to index policy {policy.PolicyNumber}: {result.DebugInformation}");
    }

    public async Task<List<Policy>> Find(string queryText)
    {
        var result = await elasticClient.SearchAsync<Policy>(search =>
        {
            search.Size(10);
            if (string.IsNullOrWhiteSpace(queryText))
                search.Sort(sort => sort.Field(p => p.PolicyStartDate, field => field.Order(SortOrder.Desc)));
            else
                search.Query(query => query.MultiMatch(match => match
                    .Query(queryText)
                    .Fields(Infer.Fields<Policy>(p => p.PolicyNumber, p => p.PolicyHolder))
                    .Type(TextQueryType.BestFields)
                    .Fuzziness(new Fuzziness("AUTO"))));
        });

        if (result.ApiCallDetails?.HttpStatusCode == (int)HttpStatusCode.NotFound)
            return new List<Policy>();

        if (!result.IsValidResponse)
            throw new InvalidOperationException($"Failed to search policies: {result.DebugInformation}");

        return result.Documents.ToList();
    }
}