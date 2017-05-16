using System;

namespace CloudBedlam.Config
{
    public enum Orchestration
    {
        Unknown,
        // Run tests in the order specified in the config...
        Sequential,
        // Run all specified tests at the same time (bedlam...)
        Concurrent,
        // Randomly choose which test runs when (random order, one run per test...)
        Random
    }
}
