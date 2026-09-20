public class DeviceConfig
{
    public int EquipId { get; set; }
    public int TaskId { get; set; }
    public string EquipType { get; set; } = "";
    public string EquipName { get; set; } = "";

    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public int? SlotIndex { get; set; }

    public short Enable { get; set; }

    public double? Version { get; set; }
    public string? Trigger { get; set; }
    public int? ScanRate { get; set; }
    public DateTime? LastUpdate { get; set; }
}