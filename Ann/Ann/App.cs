﻿using System.Collections.Generic;
using Livet;

namespace Ann
{
    public class App : NotificationObject
    {
        public static App Instance { get; } = new App();

        private readonly HashSet<string> _highPriorities = new HashSet<string>();

        public static void Initialize()
        {
            // test
            Instance.AddHighPriorityPath(@"C:\Program Files\vim\gvim.exe");
        }

        public static void Destory()
        {
            Instance.Dispose();
        }

        public bool IsHighPriority(string path) => _highPriorities.Contains(path);
        public bool AddHighPriorityPath(string path) => _highPriorities.Add(path);
        public bool RemoveHighPriorityPath(string path) => _highPriorities.Remove(path);

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        private readonly LivetCompositeDisposable _compositeDisposable = new LivetCompositeDisposable();

        private App()
        {
        }
    }
}