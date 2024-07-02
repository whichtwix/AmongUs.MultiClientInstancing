using BepInEx.Unity.IL2CPP.Utils;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MCI.Embedded.ReactorCoroutines;

public static class Coroutines
{
    internal sealed class Component : MonoBehaviour
    {
        internal static Component? Instance { get; set; }

        public Component(IntPtr ptr) : base(ptr)
        {
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }

    private static readonly ConditionalWeakTable<IEnumerator, Coroutine> _ourCoroutineStore = new();

    [return: NotNullIfNotNull("coroutine")]
    public static IEnumerator? Start(IEnumerator? coroutine)
    {
        if (coroutine != null)
        {
            _ourCoroutineStore.AddOrUpdate(coroutine, Component.Instance!.StartCoroutine(coroutine));
        }

        return coroutine;
    }

    public static void Stop(IEnumerator? coroutine)
    {
        if (coroutine != null && _ourCoroutineStore.TryGetValue(coroutine, out var routine))
        {
            Component.Instance!.StopCoroutine(routine);
        }
    }
}