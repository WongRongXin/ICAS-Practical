\# ICAS Device Configuration – Practical Assessment



\## Overview



This application is a C# console application that reads device configuration data from SQL Server (`dbo.t\_DeviceCfg`).



The application supports:



\* Reading all device configurations from the database

\* Displaying the configurations in a readable console table

\* Looking up a specific device by `EquipId`

\* Validating device configuration values

\* Reporting missing or invalid data clearly

\* Handling database and input errors without crashing



\## Running the application



\### Display all devices



```text

dotnet run

```



\### Display one device



```text

dotnet run -- 3

```



The number after `--` is the device `EquipId`.



\## Validation assumptions



The following validation rules are used by the application:



\* `EquipId` must be greater than 0.

\* `TaskID` must be greater than 0.

\* `EquipType` is required.

\* `EquipName` is required.

\* `Enable` must be either `0` or `1`.

\* `IpAddress` is required and must be a valid IPv4 address.

\* `Port`, when present, must be between `1` and `65535`.

\* `Version`, when present, must be greater than `0`.

\* `ScanRate`, when present, must be greater than `0`.

\* `Trigger`, when supplied, must not be empty.

\* `SlotIndex = -1` is treated as an unassigned slot and is therefore allowed.

\* `LastUpdate` may be `NULL` because the database column allows `NULL`.



These rules are assumptions made by the application based on the supplied database schema and sample data.



\## Handling unexpected data



The sample database intentionally contains incomplete or unusual values.



Examples:



\* Device 4 has a missing IP address.

\* Device 7 has a `ScanRate` of `0`.

\* Device 8 has a `NULL` `LastUpdate`.

\* Device 6 has a `SlotIndex` of `-1`.



The application does not silently ignore these conditions. It reports validation problems clearly to the user.



\## Error handling



The application handles several failure cases:



\* Invalid command-line device ID

\* Device ID not found

\* Missing or invalid configuration values

\* SQL Server connection or database errors

\* Unexpected application errors



The application also uses non-zero exit codes when an operation fails.



