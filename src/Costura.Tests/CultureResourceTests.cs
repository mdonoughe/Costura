using System.Globalization;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class CultureResourceTests : BaseCosturaTest
{
    protected override string Suffix => "Culture";

    [SetUp]
    public void Setup()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr-FR");

        if (AppDomainRunner.IsNotInTestAppDomain)
            CreateIsolatedAssemblyCopy("<Costura />");

        if (AppDomainRunner.IsInTestAppDomain)
            LoadAssemblyIntoAppDomain();
    }
}