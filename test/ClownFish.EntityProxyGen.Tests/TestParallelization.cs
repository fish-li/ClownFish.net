using Microsoft.VisualStudio.TestTools.UnitTesting;

// Explicitly disable test parallelization to satisfy MSTEST0001 analyzer.
[assembly: Parallelize(Workers = 0)]
