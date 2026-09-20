using System.Net;
using Microsoft.Data.SqlClient;

public class DeviceConfigService
{
    private readonly string _connectionString;

    public DeviceConfigService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<DeviceConfig> GetAllDevices()
    {
        var devices = new List<DeviceConfig>();

        const string sql = @"
            SELECT
                EquipId,
                TaskID,
                EquipType,
                EquipName,
                IpAddress,
                Port,
                SlotIndex,
                Enable,
                Version,
                [Trigger],
                ScanRate,
                LastUpdate
            FROM dbo.t_DeviceCfg
            ORDER BY EquipId;";

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            devices.Add(new DeviceConfig
            {
                EquipId = reader.GetInt32(reader.GetOrdinal("EquipId")),
                TaskId = reader.GetInt32(reader.GetOrdinal("TaskID")),
                EquipType = reader.GetString(reader.GetOrdinal("EquipType")),
                EquipName = reader.GetString(reader.GetOrdinal("EquipName")),

                IpAddress = reader["IpAddress"] == DBNull.Value
                    ? null
                    : reader["IpAddress"].ToString(),

                Port = reader["Port"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["Port"]),

                SlotIndex = reader["SlotIndex"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["SlotIndex"]),

                Enable = reader.GetInt16(reader.GetOrdinal("Enable")),

                Version = reader["Version"] == DBNull.Value
                    ? null
                    : Convert.ToDouble(reader["Version"]),

                Trigger = reader["Trigger"] == DBNull.Value
                    ? null
                    : reader["Trigger"].ToString(),

                ScanRate = reader["ScanRate"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["ScanRate"]),

                LastUpdate = reader["LastUpdate"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["LastUpdate"])
            });
        }

        return devices;
    }

    public static List<string> Validate(DeviceConfig device)
    {
        var errors = new List<string>();

        if (device.EquipId <= 0)
            errors.Add("EquipId must be greater than 0.");

        if (device.TaskId <= 0)
            errors.Add("TaskID must be greater than 0.");

        if (string.IsNullOrWhiteSpace(device.EquipType))
            errors.Add("EquipType is missing.");

        if (string.IsNullOrWhiteSpace(device.EquipName))
            errors.Add("EquipName is missing.");

        if (device.Enable != 0 && device.Enable != 1)
            errors.Add("Enable must be 0 or 1.");

        if (device.IpAddress == null)
        {
            errors.Add("IpAddress is missing.");
        }
        else if (!IPAddress.TryParse(device.IpAddress, out var address)
                 || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
        {
            errors.Add($"IpAddress '{device.IpAddress}' is not a valid IPv4 address.");
        }

        if (device.Port.HasValue &&
            (device.Port.Value < 1 || device.Port.Value > 65535))
        {
            errors.Add($"Port '{device.Port.Value}' must be between 1 and 65535.");
        }

        if (device.Version.HasValue && device.Version.Value <= 0)
        {
            errors.Add($"Version '{device.Version.Value}' must be greater than 0.");
        }

        if (device.ScanRate.HasValue && device.ScanRate.Value <= 0)
        {
            errors.Add($"ScanRate '{device.ScanRate.Value}' must be greater than 0.");
        }

        if (device.Trigger != null &&
            string.IsNullOrWhiteSpace(device.Trigger))
        {
            errors.Add("Trigger cannot be empty.");
        }

        // SlotIndex = -1 is intentionally accepted as "unassigned".
        if (device.SlotIndex.HasValue && device.SlotIndex.Value < -1)
        {
            errors.Add($"SlotIndex '{device.SlotIndex.Value}' is invalid.");
        }

        // LastUpdate is allowed to be NULL.
        return errors;
    }
}