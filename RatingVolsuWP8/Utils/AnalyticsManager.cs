using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleAnalytics;

namespace RatingVolsuWP8
{
    public static class AnalyticsManager
    {
        public static void SendView(string screenName)
        {
            EasyTracker.GetTracker().SendView(screenName);
            //EasyTracker.Current.Dispatch();
        }
        public static void StartSession()
        {
            EasyTracker.GetTracker().SetStartSession(true);
        }

        public static void EndSession()
        {
            EasyTracker.GetTracker().SetEndSession(true);
        }

        public static void SendEvent(string category, string action, string label, long value)
        {
            EasyTracker.GetTracker().SendEvent(category, action, label, value);
        }

        public static void SendException(string description, bool isFatal)
        {
            EasyTracker.GetTracker().SendException(description,isFatal);
        }

    }
}
