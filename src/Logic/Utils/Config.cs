namespace Logic.Utils
{
    public sealed class Config
    {
        public int NumberOfDatabaseRetries { get; set; }

        public Config(int numberOfDatabaseRetries)
        {
            NumberOfDatabaseRetries = numberOfDatabaseRetries;
        }
    }
}
