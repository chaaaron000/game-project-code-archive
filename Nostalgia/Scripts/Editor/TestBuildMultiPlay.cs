using UnityEditor;

namespace _Scripts.Editor
{
    public class TestBuildMultiPlay
    {
        [MenuItem("Tools/Build Test/Win64")]
        private static void MultiPlayBuildForWin64()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone,
                BuildTarget.StandaloneWindows64);

            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes           = GetScenesPath();
            options.locationPathName = "Build/Win64/Test.exe";
            options.target           = BuildTarget.StandaloneWindows64;
            options.options          = BuildOptions.AutoRunPlayer;
            
            for (int i = 0; i < 2; i++)
                BuildPipeline.BuildPlayer(options);
        }

        /// <summary>
        /// 씬 경로 가져오기
        /// </summary>
        /// <returns></returns>
        private static string[] GetScenesPath()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            string[] scenes_path = new string[scenes.Length];
            
            for (int n = 0; n < scenes.Length; n++)
            {
                scenes_path[n] = scenes[n].path;
            }

            return scenes_path;
        }
    }
}