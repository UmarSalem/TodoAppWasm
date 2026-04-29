// Makes xUnit attributes and assertions available project-wide without explicit imports
global using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
