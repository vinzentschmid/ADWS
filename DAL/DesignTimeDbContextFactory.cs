using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Design;
using Utilities.Logging;
using Utils;
using Utils.Consul;

namespace DAL
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TimeScaleContext>
    {
        public TimeScaleContext CreateDbContext(string[] args)
        {
            DataSettings settings = new DataSettings();


            ConsulSettingsHandler consul = new ConsulSettingsHandler(settings);
            Task load = consul.Load();
            load.Wait();


            AquariumLogger logger = new AquariumLogger(settings);
            Task log = logger.Init();
            log.Wait();

            TimeScaleContext context = new TimeScaleContext(settings, logger);

            //var builder = new DbContextOptionsBuilder<TimeScaleContext>();
            //var connectionString = configuration.GetConnectionString("MyDbContext");

            //builder.UseNpgsql(con, b => b.CommandTimeout(120)).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            //  builder.UseSqlServer(connectionString);

            return context;
        }
    }
}
