// //------------------------------------------------------------
// // Copyright (c) Microsoft Corporation.  All rights reserved.
// //------------------------------------------------------------

using System;

namespace BadMachineEntity
{
    public class Machine
    {
        public string MachineName { get; set; }
        public string SuiteName { get; set;  }
        public string RunId { get; set; }
        public string TaskId { get; set; }
        public bool IsPass { get; set; }
        public bool IsOfficial { get; set; }
        public DateTime TestStartTime { get; set; }
        public DateTime TestCompletionTime { get; set; }
    }
}
