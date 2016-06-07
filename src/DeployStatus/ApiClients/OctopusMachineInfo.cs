namespace DeployStatus.ApiClients
{
    public class OctopusMachineInfo
    {
        public string Id { get; }
        public string Name { get; }
        public bool IsDisabled { get; }

        public OctopusMachineInfo(string id, string name, bool isDisabled)
        {
            Id = id;
            Name = name;
            IsDisabled = isDisabled;
        }
    }
}