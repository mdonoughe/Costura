using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;

public class ModuleWeaver : IDisposable
{
    EmbeddedResourceManager resourceManager;

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
        // Older version of Fody did not set this.
        if (ReferenceCopyLocalPaths == null)
        {
            throw new WeavingException("ReferenceCopyLocalPaths is required you may need to update to the latest version of Fody.");
        }

        var config = new Configuration(Config);

        resourceManager = new EmbeddedResourceManager(this, config);
        resourceManager.Embed();

        var templateWeaver = new TemplateWeaver(this);
        templateWeaver.CopyTemplate(config.CreateTemporaryAssemblies, resourceManager.HasUnmanagedResources);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}