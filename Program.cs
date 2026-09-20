using System.Text;
using Microsoft.Data.SqlClient;

Console.OutputEncoding = Encoding.UTF8;

const string connectionString =
    "Server=localhost;Database=ICAS_Test;Integrated Security=True;TrustServerCertificate=True;";

var service = new DeviceConfigService(connectionString);

try
{
    // ------------------------------------------------------------
    // 1. Read all device configurations
    // ------------------------------------------------------------
    List<DeviceConfig> devices = service.GetAllDevices();

    Console.WriteLine();
    Console.WriteLine("ICAS Device Configuration");
    Console.WriteLine("=========================");
    Console.WriteLine();

    // ------------------------------------------------------------
    // 2. Check whether a device ID was supplied
    // ------------------------------------------------------------
    int? requestedId = null;

    if (args.Length > 1)
    {
        Console.Error.WriteLine(
            "Usage: ICAS-Practical.exe [DeviceId]");
        Environment.ExitCode = 2;
        return;
    }

    if (args.Length == 1)
    {
        if (!int.TryParse(args[0], out int parsedId))
        {
            Console.Error.WriteLine(
                $"Invalid device ID '{args[0]}'. Device ID must be an integer.");

            Environment.ExitCode = 2;
            return;
        }

        if (parsedId <= 0)
        {
            Console.Error.WriteLine(
                $"Invalid device ID '{parsedId}'. Device ID must be greater than 0.");

            Environment.ExitCode = 2;
            return;
        }

        requestedId = parsedId;
    }

    // ------------------------------------------------------------
    // 3. Print all devices if no ID was supplied
    // ------------------------------------------------------------
    if (!requestedId.HasValue)
    {
        PrintTable(devices);

        Console.WriteLine();
        Console.WriteLine("Validation:");

        bool hasErrors = false;

        foreach (var device in devices)
        {
            List<string> errors = DeviceConfigService.Validate(device);

            if (errors.Count == 0)
                continue;

            hasErrors = true;

            Console.WriteLine();
            Console.WriteLine($"Device {device.EquipId} ({device.EquipName}) has problems:");

            foreach (string error in errors)
            {
                Console.WriteLine($"  - {error}");
            }
        }

        if (hasErrors)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine(
                "The database contains invalid or incomplete device configuration.");

            Environment.ExitCode = 5;
            return;
        }

        Console.WriteLine("All device configurations are valid.");
        return;
    }

    // ------------------------------------------------------------
    // 4. Find requested device
    // ------------------------------------------------------------
    DeviceConfig? selectedDevice =
        devices.FirstOrDefault(x => x.EquipId == requestedId.Value);

    if (selectedDevice == null)
    {
        Console.Error.WriteLine(
            $"Device ID {requestedId.Value} was not found.");

        Environment.ExitCode = 4;
        return;
    }

    // ------------------------------------------------------------
    // 5. Validate requested device
    // ------------------------------------------------------------
    List<string> selectedErrors =
        DeviceConfigService.Validate(selectedDevice);

    if (selectedErrors.Count > 0)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine(
            $"Device ID {selectedDevice.EquipId} contains invalid or missing data:");

        foreach (string error in selectedErrors)
        {
            Console.Error.WriteLine($"  - {error}");
        }

        Environment.ExitCode = 5;
        return;
    }

    // ------------------------------------------------------------
    // 6. Print requested device
    // ------------------------------------------------------------
    PrintDevice(selectedDevice);
}
catch (SqlException ex)
{
    Console.Error.WriteLine("Unable to connect to or read from SQL Server.");
    Console.Error.WriteLine($"Database error: {ex.Message}");

    Environment.ExitCode = 3;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Unexpected application error.");
    Console.Error.WriteLine($"Error: {ex.Message}");

    Environment.ExitCode = 10;
}


// ================================================================
// Local functions
// ================================================================

static void PrintTable(List<DeviceConfig> devices)
{
    Console.WriteLine(
        "{0,-5} {1,-5} {2,-6} {3,-28} {4,-16} {5,6} {6,6} {7,7} {8,8}",
        "ID",
        "Task",
        "Type",
        "Name",
        "IP",
        "Port",
        "Slot",
        "Enable",
        "Rate");

    Console.WriteLine(
        new string('-', 100));

    foreach (DeviceConfig device in devices)
    {
        Console.WriteLine(
            "{0,-5} {1,-5} {2,-6} {3,-28} {4,-16} {5,6} {6,6} {7,7} {8,8}",
            device.EquipId,
            device.TaskId,
            device.EquipType,
            Shorten(device.EquipName, 28),
            device.IpAddress ?? "<NULL>",
            device.Port?.ToString() ?? "-",
            device.SlotIndex?.ToString() ?? "-",
            device.Enable,
            device.ScanRate?.ToString() ?? "-");
    }
}

static void PrintDevice(DeviceConfig device)
{
    Console.WriteLine();
    Console.WriteLine("Device Details");
    Console.WriteLine("==============");

    Console.WriteLine($"EquipId    : {device.EquipId}");
    Console.WriteLine($"TaskID     : {device.TaskId}");
    Console.WriteLine($"EquipType  : {device.EquipType}");
    Console.WriteLine($"EquipName  : {device.EquipName}");
    Console.WriteLine($"IpAddress  : {device.IpAddress ?? "<NULL>"}");
    Console.WriteLine($"Port       : {device.Port?.ToString() ?? "<NULL>"}");
    Console.WriteLine($"SlotIndex  : {device.SlotIndex?.ToString() ?? "<NULL>"}");
    Console.WriteLine($"Enable     : {device.Enable}");
    Console.WriteLine($"Version    : {device.Version?.ToString() ?? "<NULL>"}");
    Console.WriteLine($"Trigger    : {device.Trigger ?? "<NULL>"}");
    Console.WriteLine($"ScanRate   : {device.ScanRate?.ToString() ?? "<NULL>"}");
    Console.WriteLine(
        $"LastUpdate : {device.LastUpdate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "<NULL>"}");
}

static string Shorten(string value, int maxLength)
{
    if (value.Length <= maxLength)
        return value;

    return value[..(maxLength - 3)] + "...";
}