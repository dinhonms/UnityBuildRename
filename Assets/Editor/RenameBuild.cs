using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class RenameBuild
{
	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		string builtDirectoryPath = Path.GetDirectoryName(pathToBuiltProject);

		if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
		{
			foreach (string file in Directory.GetFiles(builtDirectoryPath, "*.pdb"))
			{
				Debug.Log(file + " deleted!");
				File.Delete(file);
			}
		}

		CreateVersionTxtFile(target);


		switch (target)
		{
			case BuildTarget.Android:
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSX:
				RenameRecentBuild(target, pathToBuiltProject, builtDirectoryPath);
				break;
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				RenameWindowsBuild(target, pathToBuiltProject, builtDirectoryPath);
				break;
			case BuildTarget.WebGL:
				RenameWebGlBuild(target, pathToBuiltProject, builtDirectoryPath);
				break;
		}
	}

	private static void CreateVersionTxtFile(BuildTarget buildTarget)
	{
		string bundleVersionCodePath = Application.dataPath + "/../bundleVersionCode.txt";
		string versionPath = Application.dataPath + "/../version.txt";
		int bundleVersionCode = 0;

		switch(buildTarget)
		{
			case BuildTarget.Android:
				bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
				break;

			case BuildTarget.iOS:
				bundleVersionCode = System.Convert.ToInt32(PlayerSettings.iOS.buildNumber);
				break;
		}

        if (File.Exists(bundleVersionCodePath))
            File.Delete(bundleVersionCodePath);

        if (File.Exists(versionPath))
            File.Delete(versionPath);

		if (!File.Exists(bundleVersionCodePath))
		{
			File.Create(bundleVersionCodePath).Close();
			File.WriteAllText(bundleVersionCodePath, bundleVersionCode.ToString());
        }

        if (!File.Exists(versionPath))
        {
            File.Create(versionPath).Close();
            File.WriteAllText(versionPath, Application.version);
        }
    }

	private static void RenameRecentBuild(BuildTarget target, string builtExecutablePath, string builtDirectoryPath)
	{
		string destinationPath = builtDirectoryPath + "/" + GetNewVersionName(target) + GetPlatformExtension(target);

		File.Move(builtExecutablePath, destinationPath);
		Debug.Log("Renamed the build successfully!");
	}


	private static void RenameWindowsBuild(BuildTarget target, string builtExecutablePath, string builtDirectoryPath)
	{
		string destinationPath = builtDirectoryPath + "/" + GetNewVersionName(target);

		Debug.Log("built executable path: " + builtExecutablePath);
		Debug.Log("destination path: " + destinationPath);

		Directory.CreateDirectory(destinationPath);

		File.Move(builtExecutablePath, destinationPath + "/" + PlayerSettings.productName + ".exe");
		Directory.Move(builtExecutablePath.Substring(0, builtExecutablePath.Length - 4) + "_Data", destinationPath + "/" + PlayerSettings.productName + "_Data");
		Debug.Log("Renamed the build directory successfully!");
	}


	private static void RenameWebGlBuild(BuildTarget target, string builtExecutablePath, string builtDirectoryPath)
	{
		string destinationPath = builtExecutablePath + "/" + GetNewVersionName(target);

		Debug.Log("built executable path: " + builtExecutablePath);
		Debug.Log("destination path: " + destinationPath);

		Directory.CreateDirectory(destinationPath);
		File.Move(builtExecutablePath + "/index.html", destinationPath + "/index.html");
		if (Directory.Exists(builtExecutablePath + "/TemplateData"))
		{
			Directory.Move(builtExecutablePath + "/TemplateData", destinationPath + "/TemplateData");
		}
		if (Directory.Exists(builtExecutablePath + "/Release"))
		{
			Directory.Move(builtExecutablePath + "/Release", destinationPath + "/Release");
		}
		if (Directory.Exists(builtExecutablePath + "/Debug"))
		{
			Directory.Move(builtExecutablePath + "/Debug", destinationPath + "/Debug");
		}

		Debug.Log("Build directory created successfully!");
	}


	private static string GetNewVersionName(BuildTarget target)
	{
		return PlayerSettings.productName + "_" + Application.version + "_" + GetCurrentVersion();
	}
    
	private static string GetPlatformExtension(BuildTarget target)
	{
		switch (target)
		{
			case BuildTarget.Android:
				return ".apk";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSX:
				return ".app";
			default:
				return "";
		}
	}
    
	private static string GetCurrentVersion()
	{
		string versionPath = Application.dataPath + "/../bundleVersionCode.txt";
		return File.ReadAllText(versionPath);
	}
}