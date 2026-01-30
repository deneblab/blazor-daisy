using Deneblab.SimpleEnv;
using Deneblab.SimpleVersion;
#pragma warning disable IDE0130

namespace Deneblab.BlazorDaisy.Areas.Template.App;

public class DaisyAppRegistry
{
    public DaisyAppRegistry(SimpleEnvResult env, DaisyAppConfig config, SimpleVersionInfo version)
    {
        Env = env;
        Config = config;
        Version = version;
        Work = Path.Combine(env.AppRoot, "work");
        ConfigDir = Path.Combine(env.AppRoot, "config");
        Io.CreateDirIfNotExist(Work);
    }

    public SimpleEnvResult Env { get; }
    public DaisyAppConfig Config { get; }
    public SimpleVersionInfo Version { get; }
    public string Work { get; }
    public string ConfigDir { get; }
}

public class DaisyAppConfig
{
    public string AppName { get; set; } = "Daisy App";

    public static DaisyAppConfig Load(SimpleEnvResult env)
    {
        return new DaisyAppConfig();
    }
}