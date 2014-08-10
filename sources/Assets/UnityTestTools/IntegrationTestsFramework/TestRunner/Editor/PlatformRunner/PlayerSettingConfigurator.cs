using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace UnityTest
{
	class PlayerSettingConfigurator
	{
		private string resourcesPath {
			get { return temp ? tempPath : projectResourcesPath; }
		}

		private string projectResourcesPath = Path.Combine("Assets", "Resources");
		private string tempPath = "Temp";
		private bool temp;

		private ResolutionDialogSetting displayResolutionDialog;
		private bool runInBackground;
		private bool fullScreen;
		private bool resizableWindow;
		private List<string> tempFileList = new List<string> ();

		public PlayerSettingConfigurator (bool saveInTempFolder)
		{
			temp = saveInTempFolder;
		}

		public void ChangeSettingsForIntegrationTests ()
		{
			displayResolutionDialog = PlayerSettings.displayResolutionDialog;
			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

			runInBackground = PlayerSettings.runInBackground;
			PlayerSettings.runInBackground = true;

			fullScreen = PlayerSettings.defaultIsFullScreen;
			PlayerSettings.defaultIsFullScreen = false;

			resizableWindow = PlayerSettings.resizableWindow;
			PlayerSettings.resizableWindow = true;
		}

		public void RevertSettingsChanges ()
		{
			PlayerSettings.defaultIsFullScreen = fullScreen;
			PlayerSettings.runInBackground = runInBackground;
			PlayerSettings.displayResolutionDialog = displayResolutionDialog;
			PlayerSettings.resizableWindow = resizableWindow;
		}

		public void AddConfigurationFile (string fileName, string content)
		{
			var resourcesPathExists = Directory.Exists (resourcesPath);
			if (!resourcesPathExists) AssetDatabase.CreateFolder ("Assets", "Resources");

			var filePath = Path.Combine (resourcesPath, fileName);
			File.WriteAllText (filePath, content);
			
			tempFileList.Add (filePath);
		}

		public void RemoveAllConfigurationFiles (  )
		{
			foreach (var filePath in tempFileList)
				AssetDatabase.DeleteAsset (filePath);
			if (Directory.Exists(resourcesPath) 
				&& Directory.GetFiles (resourcesPath).Length == 0)
				AssetDatabase.DeleteAsset (resourcesPath);
		}
	}
}
