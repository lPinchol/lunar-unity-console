﻿using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

using LunarConsole;

namespace LunarConsoleInternal
{
    static class AndroidPlugin
    {
        internal static void ForceUpdateFiles()
        {
            RemovePlugin(EditorConstants.PluginAndroidPath);
            UpdateFiles();
        }

        internal static void UpdateFiles()
        {
            string androidDir = EditorConstants.EditorPathAndroid;
            bool androidDirExists = Directory.Exists(androidDir);
            
            if (LunarConsoleSettings.consoleEnabled)
            {
                if (!androidDirExists)
                {
                    Debug.LogWarning("Can't enable " + Constants.PluginName + ". Directory doesn't exist: " + androidDir);
                    return;
                }
                
                AddPlugin(androidDir, EditorConstants.PluginAndroidPath);
            }
            else
            {
                RemovePlugin(EditorConstants.PluginAndroidPath);
            }
        }
        
        static void AddPlugin(string editorDir, string pluginDir)
        {
            if (Directory.Exists(pluginDir))
            {
                return;
            }

            // create plugin directory structure
            Debug.Log("Creating plugin dir: " + pluginDir);

            string[] components = pluginDir.Split('/'); // get path components
            if (components.Length == 0 || components[0] != "Assets")
            {
                Debug.LogError("Can't add " + Constants.PluginName + ". Unexpected plugin path: " + pluginDir);
                return;
            }

            string path = "Assets";
            for (int i = 1; i < components.Length; ++i)
            {
                string subpath = path + "/" + components[i]; // can't use Path.Combine since it will screw it up on Windows
                if (!Directory.Exists(subpath))
                {
                    AssetDatabase.CreateFolder(path, components[i]);
                }

                path = subpath;
            }

            // copy plugin files
            string[] pluginFiles =
            {
                "AndroidManifest.xml",
                "project.properties",
                "libs",
                "res"
            };

            foreach (string pluginFile in pluginFiles)
            {
                string srcPath = editorDir + "/" + pluginFile;
                string dstPath = pluginDir + "/" + pluginFile;

                AssetDatabase.CopyAsset(srcPath, dstPath);
            }

            AssetDatabase.ImportAsset(pluginDir, ImportAssetOptions.ImportRecursive);
        }

        static void RemovePlugin(string pluginDir)
        {
            if (!Directory.Exists(pluginDir))
            {
                return;
            }

            Log.d("Removing plugin dir: " + pluginDir);

            // delete plugin folder
            AssetDatabase.DeleteAsset(pluginDir);

            // delete all non-empty asset directories up to Assets
            string parentPath = pluginDir;
            while ((parentPath = GetParentPath(parentPath)) != null && parentPath != "Assets")
            {
                if (!IsEmptyDir(parentPath))
                {
                    break;
                }

                AssetDatabase.DeleteAsset(parentPath);
            }
        }

        #region Helpers

        static string GetParentPath(string assetPath)
        {
            int index = assetPath.LastIndexOf('/');
            return index != -1 ? assetPath.Substring(0, index) : null;
        }

        static bool IsEmptyDir(string path)
        {
            if (Directory.GetDirectories(path).Length != 0) return false; // has sub directories

            string[] files = Directory.GetFiles(path);
            if (files.Length == 1 && Path.GetFileName(files[0]) == ".DS_Store") return true; // only hidden .DS_Store file

            return files.Length == 0; // no files?
        }

        #endregion
    }
}