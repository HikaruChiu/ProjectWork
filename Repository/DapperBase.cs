using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Repository
{
    /// <summary>
    /// 使用Dapper查詢，加強分頁、排序、條件的封裝
    /// </summary>
    public class DapperBase
    { 
        /// <summary>
        /// The connection string
        /// </summary>
        protected string ConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DapperBase(string connectionString)
        {
            ConnectionString = connectionString;
        }


        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection OpenConnection()
        {
            IDbConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 一般查詢
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<dynamic>, int>> Query(string sql, object parameters)
        {
            using (IDbConnection connection = OpenConnection()) {
                DynamicParameters dynamicParams = new DynamicParameters();
                dynamicParams.AddDynamicParams(parameters);
                SqlBuilder builder = new();

                var selector = builder.AddTemplate(sql, dynamicParams);
                List<dynamic> rows = (List<dynamic>)await connection.QueryAsync<dynamic>(selector.RawSql, dynamicParams);

                if (rows.Count == 0) {
                    return new Tuple<IEnumerable<dynamic>, int>(rows, 0);
                }

                return new Tuple<IEnumerable<dynamic>, int>(rows, rows[0].TotalCount);
            }
        }

        /// <summary>
        /// 使用 SQL 2012 之後的OFFSET FETCH的分頁技術查詢
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortings"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<dynamic>, int>> FindWithOffsetFetch(string sql
                                                , object parameters
                                                , int pageIndex
                                                , int pageSize
                                                , List<SortDescriptor> sortings)
        {
            using (IDbConnection connection = OpenConnection())
            {

                string selectQuery = $@" ;WITH _data AS (
                                            {sql}
                                        ),
                                            _count AS (
                                                SELECT COUNT(1) AS TotalCount FROM _data
                                        )
                                        SELECT * FROM _data CROSS APPLY _count /**orderby**/ OFFSET @PageIndex * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY";
                
                DynamicParameters dynamicParams = new DynamicParameters();                
                dynamicParams.AddDynamicParams(parameters);
                dynamicParams.Add("PageIndex", pageIndex - 1);
                dynamicParams.Add("PageSize", pageSize);
                
                SqlBuilder builder = new();
                
                var selector = builder.AddTemplate(selectQuery, dynamicParams);
                
                foreach (var sorting in sortings)
                {
                    if (string.IsNullOrWhiteSpace(sorting.Field))
                        continue;

                    if (sorting.Direction == SortDescriptor.SortingDirection.Ascending)
                        builder.OrderBy(sorting.Field);
                    else if (sorting.Direction == SortDescriptor.SortingDirection.Descending)
                        builder.OrderBy(sorting.Field + " desc");
                }

                List<dynamic> rows = (List<dynamic>)await connection.QueryAsync<dynamic>(selector.RawSql, dynamicParams);

                if (rows.Count == 0)
                {
                    return new Tuple<IEnumerable<dynamic>, int>(rows, 0);
                }

                return new Tuple<IEnumerable<dynamic>, int>(rows, rows[0].TotalCount);

            }
        }
        
    }

    /// <summary>
    /// appsetting.json
    /// </summary>
    public class MySectionConfig
    {
        public LoggingConfig Logging { get; set; }

        public string AllowedHosts { get; set; }

        public MyConnectionConfig ConnectionStrings { get; set; }

        public class LoggingConfig
        {
        }
    }

    /// <summary>
    /// db connection string
    /// </summary>
    public class MyConnectionConfig
    {
        public string IntranetDB { get; set; }
        public string PJW { get; set; }

    }

    public class SortDescriptor
    {
        public SortingDirection Direction { get; set; }
        public string Field { get; set; }

        public enum SortingDirection
        {
            Ascending,
            Descending
        }
    }
}
