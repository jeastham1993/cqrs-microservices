using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeasthamdev.Cqrs.Read.Domain.Common;
using Jeasthamdev.Cqrs.Read.Domain.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Jeasthamdev.Cqrs.Read.Domain.Settings;
using Newtonsoft.Json;
using System.Linq;

namespace Jeasthamdev.Cqrs.Read.Infrastructure
{
    public class OrderRepository
    {
        private readonly IDistributedCache _distributedCache;

        public OrderRepository(IDistributedCache distributedCache)
        {
            if (string.IsNullOrEmpty(DbSettings.ConnectionString))
            {
                throw new ArgumentException("Database connection string cannot be null or empty", nameof(DbSettings.ConnectionString));
            }

            this._distributedCache = distributedCache;
        }

        public async Task<IEnumerable<Order>> GetAndStoreInCacheIfEmpty()
        {
            IEnumerable<Order> orderResponse = null;

            var cachedOrderData = await this._distributedCache.GetStringAsync(InstanceSettings.InstanceIdentifier);

            if (cachedOrderData == null)
            {
                using (var sqlConnection = new SqlConnection(DbSettings.ConnectionString))
                {
                    await sqlConnection.OpenAsync();

                    orderResponse = await sqlConnection.QueryAsync<Order>(GET_QUERY);
                }

                await this._distributedCache.SetStringAsync(InstanceSettings.InstanceIdentifier, JsonConvert.SerializeObject(orderResponse));
            }
            else
            {
                orderResponse = JsonConvert.DeserializeObject<IEnumerable<Order>>(cachedOrderData);
            }

            if (orderResponse == null)
            {
                orderResponse = new List<Order>(0);
            }

            return orderResponse;
        }

        public async Task AddSpecificOrderToCache(int newOrderId)
        {
            List<Order> orderResponse = null;

            var existingCacheDate = await this._distributedCache.GetStringAsync(InstanceSettings.InstanceIdentifier);

            using (var sqlConnection = new SqlConnection(DbSettings.ConnectionString))
            {
                if (existingCacheDate == null)
                {
                    await sqlConnection.OpenAsync();

                    orderResponse = (await sqlConnection.QueryAsync<Order>(GET_QUERY)).ToList();

                    await this._distributedCache.SetStringAsync(InstanceSettings.InstanceIdentifier, JsonConvert.SerializeObject(orderResponse));
                }
                else
                {
                    orderResponse = JsonConvert.DeserializeObject<List<Order>>(existingCacheDate);

                    var newOrder = await sqlConnection.QueryFirstOrDefaultAsync<Order>(GET_SPECIFIC_QUERY, new {orderId = newOrderId});

                    if (newOrder != null)
                    {
                        orderResponse.Add(newOrder);
                    }

                    await this._distributedCache.SetStringAsync(InstanceSettings.InstanceIdentifier, JsonConvert.SerializeObject(orderResponse));
                }
            }

            if (orderResponse == null)
            {
                orderResponse = new List<Order>(0);
            }
        }

        private const string GET_QUERY = @"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
        
        SELECT [Orders].Id
    ,[Orders].OrderNumber
    ,[Orders].OrderDate
    ,COUNT([OrderLines].Id) as LineCount
    ,SUM([OrderLines].Value) as TotalValue
FROM [Orders]
LEFT JOIN [OrderLines] on [OrderLines].OrderId = [Orders].Id
GROUP BY [Orders].Id
    ,[Orders].OrderNumber
    ,[Orders].OrderDate";

        private const string GET_SPECIFIC_QUERY = @"SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
        
        SELECT [Orders].Id
    ,[Orders].OrderNumber
    ,[Orders].OrderDate
    ,COUNT([OrderLines].Id) as LineCount
    ,SUM([OrderLines].Value) as TotalValue
FROM [Orders]
LEFT JOIN [OrderLines] on [OrderLines].OrderId = [Orders].Id
WHERE [Orders].Id = @orderId
GROUP BY [Orders].Id
    ,[Orders].OrderNumber
    ,[Orders].OrderDate";
    }
}