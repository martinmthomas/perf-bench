namespace Analyzer.Models.Configs
{
    public class CosmosOptions
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public ContainerOptions AnalysesContainer { get; set; }

        public ContainerOptions PlatformsContainer { get; set; }
    }
}
