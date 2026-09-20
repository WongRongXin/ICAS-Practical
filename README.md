## How to run the ICAS test on a clean machine

The clean machine needs:

* **.NET 10 SDK** — required to build/run from source
* **SQL Server** — with the `ICAS_Test` database
* **Git** — only if cloning from GitHub

### 1. Get the project

```cmd
git clone https://github.com/WongRongXin/ICAS-Practical.git
cd ICAS-Practical
```

### 2. Prepare SQL Server

Run the provided database setup script in **SQL Server Management Studio**.

It should create:

```text
ICAS_Test
└── dbo.t_DeviceCfg
```

and insert the 10 test records.

### 3. Restore dependencies

Inside the project folder:

```cmd
dotnet restore
```

This downloads `Microsoft.Data.SqlClient` from NuGet.

### 4. Build

```cmd
dotnet build
```

### 5. Run all devices

```cmd
dotnet run
```

### 6. Run a specific device

For example:

```cmd
dotnet run -- 3
```

### 7. Test error handling

```cmd
dotnet run -- 999
dotnet run -- abc
dotnet run -- 4
dotnet run -- 7
```

These test:

* Device not found
* Invalid ID
* Missing IP address
* Invalid scan rate

### Important: one thing is missing from the current GitHub commit

Your last Git commit showed **6 files**:

```text
.gitignore
DeviceConfig.cs
DeviceConfigService.cs
ICAS-Practical.csproj
Program.cs
README.md
```

The **SQL setup script was not committed**, so a clean machine currently cannot recreate `ICAS_Test` from the repository alone.

For a truly self-contained submission, add the original SQL script as:

```text
setup.sql
```

Then:

```cmd
git add setup.sql
git commit -m "Add database setup script"
git push
```

Your final clean-machine workflow would then be:

```text
Clone GitHub
   ↓
Install .NET 10 SDK + SQL Server
   ↓
Run setup.sql
   ↓
dotnet restore
   ↓
dotnet build
   ↓
dotnet run
```
