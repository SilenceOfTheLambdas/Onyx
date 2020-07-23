using System;

namespace Unity.Platforms
{
    public struct SuspendResumeEvent
    {
        public bool Suspend { get; }
        public SuspendResumeEvent(bool suspend)
        {
            Suspend = suspend;
        }
    }

    public struct QuitEvent
    {
    }

    public static class PlatformEvents
    {
        public delegate void SuspendResumeEventHandler(object sender, SuspendResumeEvent evt);
        public delegate void QuitEventHandler(object sender, QuitEvent evt);

        public static void SendSuspendResumeEvent(object sender, SuspendResumeEvent evt)
        {
            var handler = OnSuspendResume;
            handler?.Invoke(sender, evt);
        }

        public static void SendQuitEvent(object sender, QuitEvent evt)
        {
            var handler = OnQuit;
            handler?.Invoke(sender, evt);
        }

        public static event SuspendResumeEventHandler OnSuspendResume;
        public static event QuitEventHandler OnQuit;
    }
}
