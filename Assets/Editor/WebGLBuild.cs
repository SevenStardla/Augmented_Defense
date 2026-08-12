using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuild
{
    private const string OutputPath = "Builds/WebGL";

    [MenuItem("Build/Augmented Defense WebGL")]
    public static void BuildFromMenu()
    {
        Build();
    }

    public static void Build()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new InvalidOperationException("No enabled scenes were found in Build Settings.");
        }

        Directory.CreateDirectory(OutputPath);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.CleanBuildCache
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException($"WebGL build failed: {summary.result}, {summary.totalErrors} errors.");
        }

        Debug.Log($"WebGL build completed: {Path.GetFullPath(OutputPath)} ({summary.totalSize} bytes, {summary.totalTime})");
    }
}
