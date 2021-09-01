/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class PXR_ADBTool
{
    private const string ADB_NO_READ_STR = "Failed to initialize ADB Tool. Please check Android SDK path in Preferences -> External Tools";
    private const string ADB_NO_DEVICE_STR = "Device not connected via ADB. Please connect device and allow debugging.";
    private const string ADB_MULTIPLE_DEVICE_STR = "Multiple ADB devices connected. Cannot perform Build and Run.";

    private static bool isReady;

    public string androidSdkRoot;
    public string androidPlatformToolsPath;
    public string adbPath;

    private static PXR_ADBTool instance = null;
    private static readonly object mlock = new object();

    public delegate void WaitingProcessToExitCallback();

    private StringBuilder outputStringBuilder = null;
    private StringBuilder errorStringBuilder = null;

    public static PXR_ADBTool GetInstance()
    {
        if(instance == null)
        {
            lock (mlock)
            {
                if (instance == null)
                {
                    instance = new PXR_ADBTool();
                    return instance;
                }
                else
                {
                    return instance;
                }
            }
        }

        return instance;
    }

    public PXR_ADBTool()
    {
        this.androidSdkRoot = GetAndroidSDKPath();

        if (this.androidSdkRoot.EndsWith("\\") || this.androidSdkRoot.EndsWith("/"))
        {
            this.androidSdkRoot = this.androidSdkRoot.Remove(this.androidSdkRoot.Length - 1);
        }

        androidPlatformToolsPath = Path.Combine(this.androidSdkRoot, "platform-tools");
        adbPath = Path.Combine(androidPlatformToolsPath, "adb.exe");
        isReady = File.Exists(adbPath);
    }

    public bool IsReady()
    {
        if (!isReady)
        {
            Debug.LogError(ADB_NO_READ_STR);
        }
        return isReady;
    }

    public bool CheckADBDevices(Action<string> action)
    {
        if (!GetInstance().IsReady())
        {
            action.Invoke(ADB_NO_READ_STR);
            return false;
        }
        List<string> devices = GetDevices();
        if (devices.Count == 0)
        {
            Debug.LogError(ADB_NO_DEVICE_STR);
            action.Invoke(ADB_NO_DEVICE_STR);
            return false;
        }
        else if (devices.Count > 1)
        {
            Debug.LogError(ADB_MULTIPLE_DEVICE_STR);
            action.Invoke(ADB_MULTIPLE_DEVICE_STR);
            return false;
        }

        return true;
    }

    public int RunCommand(string[] arguments, WaitingProcessToExitCallback waitingProcessToExitCallback, out string outputString, out string errorString)
    {
        int exitCode = -1;
        if (!isReady)
        {
            Debug.LogWarning("PXR ADB Tool not ready");
            outputString = string.Empty;
            errorString = "PXR ADB Tool not ready";
            return exitCode;
        }

        string args = string.Join(" ", arguments);

        ProcessStartInfo startInfo = new ProcessStartInfo(adbPath, args);
        startInfo.WorkingDirectory = androidSdkRoot;
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        outputStringBuilder = new StringBuilder("");
        errorStringBuilder = new StringBuilder("");

        Process process = Process.Start(startInfo);
        process.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceivedHandler);
        process.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataReceivedHandler);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            do
            {
                if (waitingProcessToExitCallback != null)
                {
                    waitingProcessToExitCallback();
                }
            } while (!process.WaitForExit(100));

            process.WaitForExit();
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("[PXR_ADBTool.RunCommand] exception {0}", e.Message);
        }

        exitCode = process.ExitCode;

        process.Close();

        outputString = outputStringBuilder.ToString();
        errorString = errorStringBuilder.ToString();

        outputStringBuilder = null;
        errorStringBuilder = null;

        if (!string.IsNullOrEmpty(errorString))
        {
            if (errorString.Contains("Warning"))
            {
                Debug.LogWarning("PXR ADB Tool " + errorString);
            }
            else
            {
                Debug.LogError("PXR ADB Tool " + errorString);
            }
        }

        return exitCode;
    }

    public Process RunCommandAsync(string[] arguments, DataReceivedEventHandler outputDataRecievedHandler)
    {
        if (!isReady)
        {
            Debug.LogWarning("PXR ADB Tool not ready");
            return null;
        }

        string args = string.Join(" ", arguments);

        ProcessStartInfo startInfo = new ProcessStartInfo(adbPath, args);
        startInfo.WorkingDirectory = androidSdkRoot;
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        Process process = Process.Start(startInfo);
        if (outputDataRecievedHandler != null)
        {
            process.OutputDataReceived += new DataReceivedEventHandler(outputDataRecievedHandler);
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return process;
    }

    public string GetAndroidSDKPath(bool throwError = true)
    {
        string androidSDKPath = "";

#if UNITY_2019_1_OR_NEWER
		// Check for use of embedded path or user defined 
		bool useEmbedded = EditorPrefs.GetBool("SdkUseEmbedded") || string.IsNullOrEmpty(EditorPrefs.GetString("AndroidSdkRoot"));
		if (useEmbedded)
		{
			androidSDKPath = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "SDK");
		}
		else
#endif
        {
            androidSDKPath = EditorPrefs.GetString("AndroidSdkRoot");
        }

        androidSDKPath = androidSDKPath.Replace("/", "\\");
        // Validate sdk path and notify user if path does not exist.
        if (!Directory.Exists(androidSDKPath))
        {
            androidSDKPath = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            if (!string.IsNullOrEmpty(androidSDKPath))
            {
                return androidSDKPath;
            }

            if (throwError)
            {
                EditorUtility.DisplayDialog("Android SDK not Found",
                        "Android SDK not found. Please ensure that the path is set correctly in (Edit -> Preferences -> External Tools) or that the Unity Android module is installed correctly.",
                        "Ok");
            }
            return string.Empty;
        }

        return androidSDKPath;
    }

    // Returns the path to the gradle-launcher-*.jar
    public string GetGradlePath(bool throwError = true)
    {
        string gradlePath = "";
        string libPath = "";
#if UNITY_2019_1_OR_NEWER
		// Check for use of embedded path or user defined 
		bool useEmbedded = EditorPrefs.GetBool("GradleUseEmbedded") || string.IsNullOrEmpty(EditorPrefs.GetString("GradlePath"));

		if (useEmbedded)
		{
			libPath = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "Tools\\gradle\\lib");
		}
		else
		{
			libPath = Path.Combine(EditorPrefs.GetString("GradlePath"), "lib");
		}
#else
        libPath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines\\AndroidPlayer\\Tools\\gradle\\lib");
#endif

        libPath = libPath.Replace("/", "\\");
        if (!string.IsNullOrEmpty(libPath) && Directory.Exists(libPath))
        {
            string[] gradleJar = Directory.GetFiles(libPath, "gradle-launcher-*.jar");
            if (gradleJar.Length == 1)
            {
                gradlePath = gradleJar[0];
            }
        }

        // Validate gradle path and notify user if path does not exist.
        if (!File.Exists(gradlePath))
        {
            if (throwError)
            {
                EditorUtility.DisplayDialog("Gradle not Found",
                        "Gradle not found. Please ensure that the path is set correctly in (Edit -> Preferences -> External Tools) or that the Unity Android module is installed correctly.",
                        "Ok");
            }
            return string.Empty;
        }

        return gradlePath;
    }

    // Returns path to the Java executable in the JDK
    public string GetJDKPath(bool throwError = true)
    {
        string jdkPath = "";
#if UNITY_EDITOR_WIN
        // Check for use of embedded path or user defined 
        bool useEmbedded = EditorPrefs.GetBool("JdkUseEmbedded") || string.IsNullOrEmpty(EditorPrefs.GetString("JdkPath"));

        string exePath = "";
        if (useEmbedded)
        {
#if UNITY_2019_1_OR_NEWER
			exePath = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "Tools\\OpenJDK\\Windows\\bin");
#else
            exePath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines\\AndroidPlayer\\Tools\\OpenJDK\\Windows\\bin");
#endif
        }
        else
        {
            exePath = Path.Combine(EditorPrefs.GetString("JdkPath"), "bin");
        }

        jdkPath = Path.Combine(exePath, "java.exe");
        jdkPath = jdkPath.Replace("/", "\\");

        // Validate gradle path and notify user if path does not exist.
        if (!File.Exists(jdkPath))
        {
            // Check the enviornment variable as a backup to see if the JDK is there.
            string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(javaHome))
            {
                jdkPath = Path.Combine(javaHome, "bin\\java.exe");
                if (File.Exists(jdkPath))
                {
                    return jdkPath;
                }
            }

            if (throwError)
            {
                EditorUtility.DisplayDialog("JDK not Found",
                    "JDK not found. Please ensure that the path is set correctly in (Edit -> Preferences -> External Tools) or that the Unity Android module is installed correctly.",
                    "Ok");
            }
            return string.Empty;
        }
#endif
        return jdkPath;
    }

    private List<string> GetDevices()
    {
        string outputString;
        string errorString;

        RunCommand(new string[] { "devices" }, null, out outputString, out errorString);
        string[] devices = outputString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        List<string> deviceList = new List<string>(devices);
        deviceList.RemoveAt(0);

        for (int i = 0; i < deviceList.Count; i++)
        {
            string deviceName = deviceList[i];
            int index = deviceName.IndexOf('\t');
            if (index >= 0)
            {
                deviceList[i] = deviceName.Substring(0, index);
            }
            else
            {
                deviceList[i] = "";
                deviceList.RemoveAt(i);
            }
        }

        return deviceList;
    }

    private void OutputDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            outputStringBuilder.Append(args.Data);
            outputStringBuilder.Append(Environment.NewLine);
        }
    }

    private void ErrorDataReceivedHandler(object sendingProcess, DataReceivedEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Data))
        {
            errorStringBuilder.Append(args.Data);
            errorStringBuilder.Append(Environment.NewLine);
        }
    }

}
