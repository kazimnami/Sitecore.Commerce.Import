using Microsoft.AspNetCore.Hosting;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.SQL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Catalog.Engine
{
    public class GetMediaItemsCommand : SQLCommerceCommand
    {
        private readonly IHostingEnvironment HostingEnvironment;

        public GetMediaItemsCommand(IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment) : base(serviceProvider)
        {
            HostingEnvironment = hostingEnvironment;
        }

        public async Task<Dictionary<string, string>> Process(CommerceContext commerceContext, IEnumerable<string> listOfImageNames)
        {
            using (CommandActivity.Start(commerceContext, this))
            {

                var mediaItemList = new Dictionary<string, string>();
                var connectionString = commerceContext.GetPolicy<SitecoreMasterSqlPolicy>().ReadOnlyConnectionString(commerceContext);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string[] paramNames = listOfImageNames.Select((s, i) => "@tag" + i.ToString()).ToArray();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT DISTINCT TOP (1000) [Name], [ID] FROM [dbo].[Items] WHERE [Name] in ({string.Join(", ", paramNames)})";
                        for (int i = 0; i < paramNames.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(paramNames[i], listOfImageNames.ElementAt(i));
                        }

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                mediaItemList.Add(reader["Name"].ToString(), reader["ID"].ToString());
                            }
                        }
                    }
                }

                return mediaItemList;
            }
        }
    }
}
