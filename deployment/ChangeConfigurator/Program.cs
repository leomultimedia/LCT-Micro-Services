using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var environmentOption = new Option<string>(
    name: "--environment",
    description: "The environment to configure (dev, staging, prod)",
    getDefaultValue: () => "dev");

var configFileOption = new Option<string>(
    name: "--config-file",
    description: "Path to the configuration file",
    getDefaultValue: () => "appsettings.json");

var rootCommand = new RootCommand("Change Configurator for E-Commerce Platform");
rootCommand.AddOption(environmentOption);
rootCommand.AddOption(configFileOption);

rootCommand.SetHandler(async (string environment, string configFile) =>
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(configFile, optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddUserSecrets<Program>()
        .AddEnvironmentVariables()
        .Build();

    var serviceCollection = new ServiceCollection();
    ConfigureServices(serviceCollection, configuration);

    var serviceProvider = serviceCollection.BuildServiceProvider();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Starting configuration for environment: {Environment}", environment);
        
        // Load current configuration
        var configManager = serviceProvider.GetRequiredService<IConfigurationManager>();
        var currentConfig = await configManager.LoadConfigurationAsync(environment);
        
        // Display current configuration
        Console.WriteLine("\nCurrent Configuration:");
        DisplayConfiguration(currentConfig);
        
        // Get user input for changes
        var changes = GetUserChanges(currentConfig);
        
        // Validate changes
        var validationResult = await configManager.ValidateChangesAsync(changes);
        if (!validationResult.IsValid)
        {
            logger.LogError("Configuration validation failed: {Errors}", validationResult.Errors);
            return;
        }
        
        // Apply changes
        await configManager.ApplyChangesAsync(changes);
        
        // Generate release notes
        var releaseNotes = await configManager.GenerateReleaseNotesAsync(changes);
        Console.WriteLine("\nRelease Notes Generated:");
        Console.WriteLine(releaseNotes);
        
        // Generate report
        var report = await configManager.GenerateReportAsync(changes);
        Console.WriteLine("\nDeployment Report Generated:");
        Console.WriteLine(report);
        
        logger.LogInformation("Configuration completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during configuration");
    }
});

await rootCommand.InvokeAsync(args);

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddLogging(configure => configure.AddConsole());
    services.AddSingleton<IConfigurationManager, ConfigurationManager>();
    services.AddSingleton<IConfiguration>(configuration);
    services.AddAzureClients(builder =>
    {
        builder.AddSecretClient(new Uri(configuration["KeyVault:Url"]));
    });
}

void DisplayConfiguration(Dictionary<string, string> config)
{
    foreach (var (key, value) in config)
    {
        Console.WriteLine($"{key}: {value}");
    }
}

Dictionary<string, string> GetUserChanges(Dictionary<string, string> currentConfig)
{
    var changes = new Dictionary<string, string>();
    Console.WriteLine("\nEnter new values (press Enter to keep current value):");
    
    foreach (var (key, currentValue) in currentConfig)
    {
        Console.Write($"{key} [{currentValue}]: ");
        var newValue = Console.ReadLine();
        
        if (!string.IsNullOrEmpty(newValue) && newValue != currentValue)
        {
            changes[key] = newValue;
        }
    }
    
    return changes;
} 