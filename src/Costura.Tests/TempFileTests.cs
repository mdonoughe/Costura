using NUnit.Framework;

[TestFixture]
public class TempFileTests : BaseCosturaTest
{
    protected override string Suffix => "TempFile";

    [SetUp]
    public void Setup()
    {
        if (AppDomainRunner.IsNotInTestAppDomain)
            CreateIsolatedAssemblyCopy("<Costura CreateTemporaryAssemblies='true' />");

        if (AppDomainRunner.IsInTestAppDomain)
            LoadAssemblyIntoAppDomain();
    }
}