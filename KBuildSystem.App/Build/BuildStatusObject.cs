using System;
using System.Collections.Generic;

namespace KBuildSystem.App.Build
{
    public enum BuildStatus
    {
        Unknown,
        InProgress,
        Done
    }
    public interface IBuildStatus
    {
        string ID { get; set; }
        BuildStatus Status { get; set; }
        long Timestamp { get; set; }
    }
    public struct SBuildStatus : IBuildStatus
    {
        public string ID { get; set; }
        public BuildStatus Status { get; set; }
        public long Timestamp { get; set; }
    }
    public class BuildStatusObject : IBuildStatus
    {
        public BuildStatusObject(BuildHistoryObject _parent)
        {
            SetParent(_parent);
            Status = BuildStatus.Unknown;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        private BuildHistoryObject parent;
        public BuildHistoryObject GetParent()
        {
            return parent;
        }
        public void SetParent(BuildHistoryObject _parent)
        {
            parent = _parent;
            ID = _parent.ID;
        }
        public string ID { get; set; }
        public BuildStatus Status { get; set; }
        public long Timestamp { get; set; }

    }
}
