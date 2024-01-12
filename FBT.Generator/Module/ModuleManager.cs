using System.Collections.Generic;
using System.Linq;

namespace FBT.Module;

public class ModuleManager
{
	private static ModuleManager _Instance;
	private readonly List<ModuleInfo> Modules = new();
	public static ModuleManager Instance => _Instance ?? (_Instance = new ModuleManager());


	public ModuleInfo GetModule(string p_Name)
	{
		var s_Module = Modules.FirstOrDefault(x => x.Name == p_Name);

		if (s_Module == null)
		{
			s_Module = new ModuleInfo(p_Name);
			Modules.Add(s_Module);
		}

		return s_Module;
	}
}