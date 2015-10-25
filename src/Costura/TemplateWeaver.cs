using System.IO;
using System.Linq;
using Mono.Cecil;

class TemplateWeaver
{
    private ModuleWeaver weaver;

    public TemplateWeaver(ModuleWeaver weaver)
    {
        this.weaver = weaver;
    }

    public void CopyTemplate(bool createTemporaryAssemblies, bool hasUnmanaged)
    {
        var moduleDefinition = GetTemplateModuleDefinition();

        TypeDefinition sourceType;

        if (createTemporaryAssemblies)
        {
            sourceType = moduleDefinition.Types.First(x => x.Name == "ILTemplateWithTempAssembly");
            DumpSource("ILTemplateWithTempAssembly");
        }
        else if (hasUnmanaged)
        {
            sourceType = moduleDefinition.Types.First(x => x.Name == "ILTemplateWithUnmanagedHandler");
            DumpSource("ILTemplateWithUnmanagedHandler");
        }
        else
        {
            sourceType = moduleDefinition.Types.First(x => x.Name == "ILTemplate");
            DumpSource("ILTemplate");
        }
    }

    ModuleDefinition GetTemplateModuleDefinition()
    {
        var readerParameters = new ReaderParameters
        {
            AssemblyResolver = weaver.AssemblyResolver,
            ReadSymbols = true,
            SymbolStream = GetType().Assembly.GetManifestResourceStream("Costura.Template.bin.Template.pdb"),
        };

        using (var resourceStream = GetType().Assembly.GetManifestResourceStream("Costura.Template.bin.Template.dll"))
        {
            return ModuleDefinition.ReadModule(resourceStream, readerParameters);
        }
    }

    void DumpSource(string file)
    {
        var localFile = Path.Combine(Path.GetDirectoryName(weaver.AssemblyFilePath), file + ".cs");

        if (File.Exists(localFile))
            return;

        using (var stream = GetType().Assembly.GetManifestResourceStream($"Costura.Template.src.{file}.cs"))
        using (var outStream = new FileStream(localFile, FileMode.Create))
        {
            stream.CopyTo(outStream);
        }
    }
}