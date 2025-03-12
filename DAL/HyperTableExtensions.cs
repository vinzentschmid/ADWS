using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DAL
{
    public static class HyperTableExtensions
    {
        public static void ApplyHypertables(this TimeScaleContext context)
        {
            //adding timescale extension to the database
            context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS timescaledb CASCADE;");
            var entityTypes = context.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                Type typ = entityType.ClrType;

                int chunkintervall = 168;
                int retention = -1;

                if (typ.GetCustomAttribute(typeof(HyperTableAttributes)) != null)
                {
                    if (typ.GetCustomAttribute<HyperTableAttributes>() != null)
                    {

                        HyperTableAttributes hta = typ.GetCustomAttribute<HyperTableAttributes>();
                        retention = hta.RetentionPeriod;
                        chunkintervall = hta.ChunkSize;
                    }
                }

                foreach (var property in entityType.GetProperties())
                {
                    if (property.PropertyInfo.GetCustomAttribute(typeof(HyperTableColumnAttribute)) != null)
                    {

                        var tableName = entityType.GetTableName();
                        var columnName = property.GetColumnName();

                        if (!String.IsNullOrEmpty(tableName))
                        {

                            String query = $"SELECT COUNT(hypertable_name) FROM timescaledb_information.hypertables WHERE hypertable_name = '{tableName}';";

                            IQueryable<long> result = context.Database.SqlQueryRaw<long>(query);
                            long counter = 0;



                            foreach (var res in result)
                            {
                                counter += res;
                            }

                            if (counter == 0)
                            {
                                context.Database.ExecuteSqlRaw($"SELECT create_hypertable('\"{tableName}\"', '{columnName}');");

                                context.Database.ExecuteSqlRaw($"SELECT set_chunk_time_interval('\"{tableName}\"', INTERVAL '" + chunkintervall + " hours');");

                                if (retention > 0)
                                {
                                    context.Database.ExecuteSqlRaw($"SELECT add_retention_policy('\"{tableName}\"', INTERVAL '" + retention + " hours',   if_not_exists => TRUE);");
                                }
                            }
                            else
                            {
                                context.Database.ExecuteSqlRaw($"SELECT set_chunk_time_interval('\"{tableName}\"', INTERVAL '" + chunkintervall + " hours');");

                                if (retention > 0)
                                {
                                    context.Database.ExecuteSqlRaw($"SELECT add_retention_policy('\"{tableName}\"', INTERVAL '" + retention + " hours',   if_not_exists => TRUE);");
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
