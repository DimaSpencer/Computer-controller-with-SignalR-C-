using System.Collections.Generic;

namespace VirusControllerLibrary
{
    public interface IHaveManyTargetsCommand
    {
        IReadOnlyCollection<string> TargetIds { get; }
    }
}
