using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.All)] // allow each fixture (feature) to run in parallel
[assembly: LevelOfParallelism(4)]                  // number of parallel workers, tweak as needed
