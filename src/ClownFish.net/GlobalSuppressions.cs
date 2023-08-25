// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions")]
[assembly: SuppressMessage("Critical Code Smell", "S2365:Properties should not make collection or array copies")]
[assembly: SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced")]
[assembly: SuppressMessage("Major Code Smell", "S3928:Parameter names used into ArgumentException constructors should match an existing one ")]
[assembly: SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed")]
[assembly: SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous")]
[assembly: SuppressMessage("Critical Code Smell", "S1186:Methods should not be empty")]
[assembly: SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly")]
[assembly: SuppressMessage("Critical Code Smell", "S3871:Exception types should be \"public\"")]
[assembly: SuppressMessage("Style", "IDE0070:使用 \"System.HashCode\"")]
[assembly: SuppressMessage("Major Bug", "S3249:Classes directly extending \"object\" should not call \"base\" in \"GetHashCode\" or \"Equals\"")]
[assembly: SuppressMessage("Minor Code Smell", "S3260:Non-derived \"private\" classes and records should be \"sealed\"")]
[assembly: SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-value members should be named \"None\"")]
[assembly: SuppressMessage("Major Bug", "S2225:\"ToString()\" method should not return null")]
[assembly: SuppressMessage("Blocker Bug", "S2190:Recursion should not be infinite")]
[assembly: SuppressMessage("Major Code Smell", "S108:Nested blocks of code should not be left empty")]
[assembly: SuppressMessage("Minor Code Smell", "S2219:Runtime type checking should be simplified")]
[assembly: SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed")]
[assembly: SuppressMessage("Critical Code Smell", "S927:Parameter names should match base declaration and other partial definitions")]
[assembly: SuppressMessage("Major Code Smell", "S2327:\"try\" statements with identical \"catch\" and/or \"finally\" blocks should be merged")]
[assembly: SuppressMessage("Major Code Smell", "S2372:Exceptions should not be thrown from property getters")]
[assembly: SuppressMessage("Blocker Code Smell", "S3875:\"operator==\" should not be overloaded on reference types")]
[assembly: SuppressMessage("Major Code Smell", "S2376:Write-only properties should not be used")]
[assembly: SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested")]
[assembly: SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed")]
[assembly: SuppressMessage("Critical Code Smell", "S1215:\"GC.Collect\" should not be called")]
[assembly: SuppressMessage("Critical Code Smell", "S2696:Instance members should not write to \"static\" fields")]
[assembly: SuppressMessage("Major Code Smell", "S3010:Static fields should not be updated in constructors")]
[assembly: SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[assembly: SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant")]
[assembly: SuppressMessage("Major Code Smell", "S1066:Collapsible \"if\" statements should be merged")]
