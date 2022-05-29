﻿namespace OrchardCore.Commerce.Tests.UI;

public static class Config
{
    public static string GetAbsoluteApplicationAssemblyPath()
    {
        // The test assembly can be in a folder below the src and test folders (those should be in the repo root).
        var baseDirectory = File.Exists("SnowMountain.Web.dll")
            ? AppContext.BaseDirectory
            : Path.Combine(
                AppContext.BaseDirectory.Split(new[] { "src", "test" }, StringSplitOptions.RemoveEmptyEntries)[0],
                "SampleWebApp",
                "bin",
                "Debug",
                "net6.0");

        return Path.Combine(baseDirectory, "SnowMountain.Web.dll");
    }
}
