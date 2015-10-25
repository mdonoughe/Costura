﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    public XElement Config { get; set; }
    public Action<string> LogInfo { get; set; } = s => { };
    public Action<string> LogError { get; set; } = s => { };
    public ModuleDefinition ModuleDefinition { get; set; }
    public string References { get; set; }
    public List<string> ReferenceCopyLocalPaths { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public string AssemblyFilePath { get; set; }

    public void Execute()
    {
        var config = new Configuration(Config);
    }
}