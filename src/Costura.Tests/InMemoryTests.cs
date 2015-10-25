using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class InMemoryTests
{
    string isolatedPath;
    Assembly assembly;

    [SetUp]
    public void Setup()
    {
        if (AppDomainRunner.IsNotInTestAppDomain)
            CreateIsolatedAssemblyCopy("InMemory");

        if (AppDomainRunner.IsInTestAppDomain)
            LoadAssemblyIntoAppDomain("InMemory");
    }

    private void CreateIsolatedAssemblyCopy(string prefix)
    {
        var processingDirectory = Path.GetFullPath(@"..\..\..\ExeToProcess\bin\Debug");
#if (!DEBUG)
        processingDirectory = processingDirectory.Replace("Debug", "Release");
#endif

        var beforeAssemblyPath = Path.Combine(processingDirectory, "ExeToProcess.exe");

        var afterAssemblyPath = beforeAssemblyPath.Replace(".exe", $"{prefix}.exe");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);
        File.Copy(beforeAssemblyPath.Replace(".exe", ".pdb"), afterAssemblyPath.Replace(".exe", ".pdb"), true);

        var readerParams = new ReaderParameters { ReadSymbols = true };

        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath, readerParams);

        var references = new List<string>
        {
            Path.Combine(processingDirectory, "AssemblyToReference.dll")
        };

        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition,
            AssemblyResolver = new MockAssemblyResolver(),
            Config = XElement.Parse("<Costura Unmanaged32Assemblies='AssemblyToReferenceMixed' PreloadOrder='AssemblyToReferenceNative' />"),
            ReferenceCopyLocalPaths = references,
            AssemblyFilePath = beforeAssemblyPath
        };
        {
            weavingTask.Execute();
            var writerParams = new WriterParameters { WriteSymbols = true };
            moduleDefinition.Write(afterAssemblyPath, writerParams);
        }

        isolatedPath = Path.Combine(Path.GetTempPath(), $"Costura{prefix}.exe");
        File.Copy(afterAssemblyPath, isolatedPath, true);
        File.Copy(afterAssemblyPath.Replace(".exe", ".pdb"), isolatedPath.Replace(".exe", ".pdb"), true);
    }

    private void LoadAssemblyIntoAppDomain(string prefix)
    {
        var isolatedPath = Path.Combine(Path.GetTempPath(), $"Costura{prefix}.exe");

        assembly = Assembly.LoadFile(isolatedPath);
    }

    [Test, RunInApplicationDomain]
    public void Simple()
    {
        var instance2 = assembly.GetInstance("ClassToTest");
        Assert.AreEqual("Hello", instance2.Foo());
    }
}