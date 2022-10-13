using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.Util
{
    public class ConfigureHelper
    {
        public static IConfigurationSection GetConfiguration(string serviceName)
        {
            var env = Environment.GetEnvironmentVariable("ENV");
            var configFolder = Environment.GetEnvironmentVariable("ENV_CONFIG_FOLDER");
            var config = ConfigureHelper.LoadAppSettings(env, configFolder);
            var configuration = config.GetSection(serviceName);
            return configuration;
        }

        public static IConfigurationSection GetConfiguration(string env, string configFolder, string key)
        {
            var config = ConfigureHelper.LoadAppSettings(env, configFolder);
            var configuration = config.GetSection(key);
            return configuration;
        }

        public static IConfigurationRoot LoadAppSettings(string env, string configFolder)
        {
            //var env = Environment.GetEnvironmentVariable("ENV");
            var baseconfigfile = "appsettings.json";
            var configfile = string.Empty;
            if (env == null || env == "DEV")
                configfile = "appsettings.DEV.json";
            else if (env == "PRD")
                configfile = "appsettings.PRD.json";
            else if (env == "DKR")
                configfile = "appsettings.DKR.json";

            //var configFolder = Environment.GetEnvironmentVariable("ENV_CONFIG_FOLDER");
            //search upwards for folder "appsettings"
            if (string.IsNullOrEmpty(configFolder))
            {
                int count = 0;
                configFolder = "appsettings";
                while (!File.Exists(Path.Combine(configFolder, baseconfigfile)) && count++ < 10)
                    configFolder = Path.Combine("..", configFolder);
            }

            //if (!Directory.Exists(configFolder)) 
            //    throw new MyException(DataType.ApiStatusCode.Error_NotFound, $"folder {configFolder} not found!");
            baseconfigfile = Path.GetFullPath(Path.Combine(configFolder, baseconfigfile));
            configfile = Path.GetFullPath(Path.Combine(configFolder, configfile));
            if (!File.Exists(baseconfigfile))
                throw new MyException("BaseConfigFileNotFound", $"{baseconfigfile} not found!");
            if (!File.Exists(configfile))
                throw new MyException("ConfigFileNotFound", $"{configfile} not found!");

            Console.WriteLine($"Environment = {env}");
            Console.WriteLine($"Base configuration file = {baseconfigfile}");
            Console.WriteLine($"Configuration file = {configfile}");

            var lines = File.ReadAllLines(configfile).Where(l => !l.Contains("#")).ToArray();
            var tmpConfigFile = configfile + ".tmp";
            File.WriteAllLines(tmpConfigFile, lines);

            /////////////////////////////////////////////////////////////////////////////////
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(baseconfigfile)
                .AddJsonFile(tmpConfigFile, optional: false)
                .Build();
            return config;
        }
    }

}
