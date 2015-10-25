﻿using NUnit.Framework;

[TestFixture]
public class InMemoryTests : BaseCosturaTest
{
    protected override string Suffix => "InMemory";

    [SetUp]
    public void Setup()
    {
        if (AppDomainRunner.IsNotInTestAppDomain)
            CreateIsolatedAssemblyCopy("<Costura />");

        if (AppDomainRunner.IsInTestAppDomain)
            LoadAssemblyIntoAppDomain();
    }
}