using System;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Unity.Editor.Example
{
    static class PackageInstaller
    {
        static AddRequest addRequest;

        //[MenuItem("Window/Add Package Example")]
        static void Add(string packageName)
        {
            // Add a package to the project
            addRequest = Client.Add(packageName);
            EditorApplication.update += AddRequestProgress;
        }
        static void AddRequestProgress()
        {
            if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                    Debug.Log("Installed: " + addRequest.Result.packageId);
                else if (addRequest.Status >= StatusCode.Failure)
                    Debug.Log(addRequest.Error.message);

                EditorApplication.update -= AddRequestProgress;
            }
        }

        static ListRequest listRequest;
        static Dictionary<string, string> installedPackages = new Dictionary<string, string>();

        [MenuItem("Window/List Package Example")]
        static void List()
        {
            listRequest = Client.List();    // List packages installed for the project
            EditorApplication.update += ListRequestProgress;
        }

        static void ListRequestProgress()
        {
            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Success)
                { 
                    installedPackages.Clear();
                    foreach (var package in listRequest.Result)
                    {
                        //Debug.Log("Package name: " + package.name);
                        installedPackages.Add(package.name, package.version);
                    }
                    GetManifestData();
                }
                else if (listRequest.Status >= StatusCode.Failure)
                {
                    Debug.Log(listRequest.Error.message);
                }

                EditorApplication.update -= ListRequestProgress;
            }
        }

        [DidReloadScripts]
        public static void OnCompileScripts()
        {
            Debug.Log("Bla-bla-bla");
            GetManifestData();
        }

        private static void GetManifestData()
        {
            string npmManifestJson = ReadFromFile("Packages_npm", "manifest.json");
            string projectManifestJson = ReadFromFile("Packages", "manifest.json");
            Debug.Log("NPM Json string : " + npmManifestJson + "ProjectJson string : "+ projectManifestJson);
            if (npmManifestJson != "" && projectManifestJson != "")
            {
                CheckAndInstallMissingPackages(npmManifestJson, projectManifestJson);
            }
        }

        private static void CheckAndInstallMissingPackages(string npmManifestJson, string projectManifestJson)
        {
            Dictionary<string, Dictionary<string, string>> npmJsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(npmManifestJson);
            Dictionary<string, string> npmManifestPackages = npmJsonDict["dependencies"];
            Dictionary<string, Dictionary<string, string>> projectJsonDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(projectManifestJson);
            Dictionary<string, string> installedPackagesInProject = projectJsonDict["dependencies"];
            foreach (var key in npmManifestPackages.Keys)
            {
                if (installedPackagesInProject.ContainsKey(key))
                {
                    if (installedPackagesInProject[key] != npmManifestPackages[key])
                    {
                        Debug.Log("Version is not same : Installed -> " + installedPackagesInProject[key] + "Package -> " + npmManifestPackages[key]);
                    }
                }
                else
                {
                    Debug.Log("Install new package : " + key);
                    Add(key);
                }
            }
        }

        //This Method will read a file from the given folder path and return the string content.
        public static string ReadFromFile(string folderPath, string fileName)
        {
            string filePath = folderPath + "/" + fileName;
            string fileContent = "";
            if (File.Exists(filePath))
            {
                StreamReader reader = new StreamReader(filePath);
                fileContent = reader.ReadToEnd();
                reader.Close();
            }
            return fileContent;
        }
    }
}

