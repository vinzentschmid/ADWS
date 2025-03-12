namespace Tests
{
    public class TimeScaleDBTests
    {
        public TimeScaleDBTests()
        {
        }

        //private ConcurrentBag<HyperEntity> DummyData()
        //{
        //    ConcurrentBag<NumericSample> data = new ConcurrentBag<NumericSample>();
        //    DateTime dateTime = DateTime.Now;
        //    DateTime last = DateTime.Now.AddDays(-2);

        //    TimeSpan spn = dateTime - last;

        //    Random rnd = new Random();


        //    long minutes = (long)spn.TotalMinutes;

        //    for (int i = 1; i <= minutes; i++)
        //    {



        //        NumericSample influx = new NumericSample();
        //        influx.Tag = "TestTag";
        //        influx.Value = (float)rnd.NextDouble() * 1000;
        //        influx.TimeStamp = DateTime.Now.AddMinutes(i * -1);

        //        data.Add(influx);
        //    }

        //    return data;
        //}


        //[Test]
        //public async Task CreateEntry()
        //{
        //    NumericSample influx = new NumericSample();
        //    influx.Tag = "TestTag";
        //    influx.Value = 7214;
        //    influx.TimeStamp = DateTime.Now;

        //    await InfluxUoW.Influx.InsertOneAsync("Test", influx);

        //    Assert.NotNull(influx);
        //}


        //[Test]
        //public async Task CreateEntries()
        //{
        //    ConcurrentBag<Sample> data = DummyData();

        //    await InfluxUoW.Influx.InsertManyAsync("Test", data);

        //    Assert.NotNull(data);
        //}

        //[Test]
        //public async Task CreateBucket()
        //{
        //    await InfluxUoW.Influx.CreateBucket("Test");
        //}




    }
}
