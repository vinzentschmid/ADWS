namespace DAL
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HyperTableAttributes : Attribute
    {
        public int ChunkSize { get; set; } = 12;
        public int RetentionPeriod { get; set; } = -1;

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class HyperTableColumnAttribute : Attribute
    {

    }


}
