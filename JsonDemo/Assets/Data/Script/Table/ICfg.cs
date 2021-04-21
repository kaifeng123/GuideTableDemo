public interface ICfg
{
	string GetKey();

	void Init();

	void AutoParse(string[] source);
}