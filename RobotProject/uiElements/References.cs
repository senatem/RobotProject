namespace RobotProject.uiElements
{
    public static class References
    {
        public static string projectPath
        {
            get
            {
                return  System.Windows.Forms.Application.ExecutablePath.Split(new string[] { "bin" },System.StringSplitOptions.None)[0];
            }
        }

    }
}