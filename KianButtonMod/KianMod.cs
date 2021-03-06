using ICities;
using ColossalFramework;
using UnityEngine;
using System;

using Kian.UI;
using System.Diagnostics;

namespace Kian.Mod
{
    public class KianModInfo : IUserMod {
        public string Name => "kian button mod";
        public string Description => "simple test mod with a button to activate test";

        public void OnEnabled() {
            System.IO.File.WriteAllText(ShortCuts.filename, ""); //clear file
            ShortCuts.Log("IUserMod.OnEnabled"); 

            if (ShortCuts.InGame)
                LoadTool.Load();
        }

        public void OnDisabled() {
            ShortCuts.Log("IUserMod.OnDisabled");
            LoadTool.Release();
        }
    }

    public static class LoadTool {
        public static void Load() {
            ToolButton.Create();
            ShortCuts.Log("LoadTool:Created kian tool.");
        }
        public static void Release() {
            ToolButton.Release();
            ShortCuts.Log("LoadTool:Removed kian tool.");
        }
    }

    public class LoadingExtention : LoadingExtensionBase {

        public override void OnLevelLoaded(LoadMode mode) {
            ShortCuts.Log("LoadingExtention.OnLevelLoaded");
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                LoadTool.Load();
        }

        public override void OnLevelUnloading() {
            ShortCuts.Log("LoadingExtention.OnLevelUnloading");
            LoadTool.Release();
        }
    }

    public static class ShortCuts {
        public static string filename = "mod.debug.log";
        internal static ref NetNode ToNode(this ushort ID) => ref Singleton<NetManager>.instance.m_nodes.m_buffer[ID];
        internal static ref NetSegment ToSegment(this ushort ID) => ref Singleton<NetManager>.instance.m_segments.m_buffer[ID];
        internal static NetManager netMan => Singleton<NetManager>.instance;

        internal static void Log(string m) {
            //var st = System.Environment.StackTrace;
            //m  = st + " : \n" + m;
            UnityEngine.Debug.Log(m);
            System.IO.File.AppendAllText(filename, m + "\n\n");
        }

        static Stopwatch ticks = null;
        internal static void LogWait(string m) {
            if (ticks == null) {
                Log(m);
                ticks = Stopwatch.StartNew();
            }
            else if (ticks.Elapsed.TotalSeconds > .5) {
                Log(m);
                ticks.Reset();
                ticks.Start();
            }
        }


        internal static AppMode currentMode => SimulationManager.instance.m_ManagersWrapper.loading.currentMode;
        internal static bool CheckGameMode(AppMode mode) => CheckGameMode(new[] { mode });
        internal static bool CheckGameMode(AppMode[] modes) {
            try {
                foreach (var mode in modes) {
                    if (currentMode == mode)
                        return true;
                }
            }
            catch { }
            return false;
        }
        internal static bool InGame => CheckGameMode(AppMode.Game);
        internal static bool InAssetEditor => CheckGameMode(AppMode.AssetEditor);
    }
} // end namesapce
