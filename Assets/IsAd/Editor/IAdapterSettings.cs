using UnityEditor;

namespace IsAd.Editor
{
	public interface IAdapterSettings
	{
		void updateProject(BuildTarget buildTarget, string projectPath);
		void updateProjectPlist(BuildTarget buildTarget, string plistPath);
	}
}