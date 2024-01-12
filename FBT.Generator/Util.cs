namespace FBT;

public class Util
{
	private const uint FB_FNV_BASE = 0x1505;
	private const uint FB_FNV_PRIME = 0x21;

	public static uint Hash(string p_String)
	{
		var s_Hash = FB_FNV_BASE;
		for (var i = 0; i < p_String.Length; ++i)
			s_Hash = (s_Hash * FB_FNV_PRIME) ^ p_String[i];
		return s_Hash;
	}
}