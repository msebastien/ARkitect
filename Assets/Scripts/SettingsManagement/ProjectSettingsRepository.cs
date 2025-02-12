using System;
using PathUtils = System.IO.Path;

namespace ARKitect.SettingsManagement
{
    /// <summary>
    /// A settings repository that stores on-device settings persistent data relative to a ARkitect project.
    /// The settings data is serialized to a JSON file.
    /// </summary>
    [Serializable]
    public sealed class ProjectSettingsRepository : FileSettingsRepository
    {
        /// <summary>
        /// Initializes and returns an instance of the ProjectSettingsRepository with the
        /// serialized data location set to a path defined by the specified `project` and
        /// `fileName` values relative to the `ProjectSettings` directory. For example:
        /// `{Application persistent data directory}/ProjectSettings/MyTestProject/Settings.json`.
        /// </summary>
        /// <param name="project">The name of the project to store the serialized data under.</param>
        /// <param name="fileName">The base filename to use for the serialized data location.</param>
        public ProjectSettingsRepository(string project, string fileName) : base(GetSettingsPath(project, fileName))
        {
        }

        /// <summary>
        /// Initializes and returns an instance of the ProjectSettingsRepository with the
        /// serialized data location set to a path defined by the specified `project`.
        /// For example:
        /// `{Application persistent data directory}/ProjectSettings/MyTestProject/Settings.json`.
        /// </summary>
        /// <param name="project">The name of the project to store the serialized data under.</param>
        public ProjectSettingsRepository(string project) : base(GetSettingsPath(project))
        {
        }


        /// <summary>
        /// Builds and returns a path for a settings file relative to the application persistent data directory.
        /// This method constructs the location from the specified `project` and `fileName` under the `ProjectSettings` folder.
        /// </summary>
        /// <param name="projectName">The name of the project requesting this setting.</param>
        /// <param name="fileName">An optional name for the settings file. Default is "Settings."</param>
        /// <returns>A path to the settings file inside the application's `ProjectSettings` directory.</returns>
        public static string GetSettingsPath(string projectName, string fileName = "Settings")
        {
            var separator = PathUtils.DirectorySeparatorChar;
            return $"{ProjectSettingsPath}{separator}{projectName}{separator}{fileName}.json";
        }
    }

}
