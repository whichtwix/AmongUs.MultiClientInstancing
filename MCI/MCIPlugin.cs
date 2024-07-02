using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MCI.Components;
using System;
using UnityEngine.SceneManagement;

namespace MCI;

[BepInAutoPlugin("dragonbreath.au.mci", "MCI", VersionString)]
[BepInProcess("Among Us.exe")]
[BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
public partial class MCIPlugin : BasePlugin
{
    public const string VersionString = "0.0.6";
    public static Version vVersion = new(VersionString);
    public Harmony Harmony { get; } = new(Id);

    public static MCIPlugin Singleton { get; private set; } = null;

    public static string RobotName { get; set; } = "Bot";

    public static bool Enabled { get; set; } = true;
    public static bool IKnowWhatImDoing { get; set; } = false;
    public static bool Persistence { get; set; } = true;

    public static Debugger Debugger { get; set; } = null;

    public override void Load()
    {
        if (Singleton != null)
            return;

        Singleton = this;
        Harmony.PatchAll();
        UpdateChecker.CheckForUpdate();
        SubmergedCompatibility.Initialize();

        ClassInjector.RegisterTypeInIl2Cpp<Debugger>();
        ClassInjector.RegisterTypeInIl2Cpp<Embedded.ReactorCoroutines.Coroutines.Component>();
        Debugger = this.AddComponent<Debugger>();
        this.AddComponent<Embedded.ReactorCoroutines.Coroutines.Component>();

        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (scene.name == "MainMenu")
                ModManager.Instance.ShowModStamp();
        }));
    }
}
