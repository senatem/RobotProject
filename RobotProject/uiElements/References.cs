using System;
using System.Windows.Forms;

namespace RobotProject.uiElements
{
    public static class References
    {
        public static string ProjectPath
        {
            get
            {
                return  Application.ExecutablePath.Split(new[] { "bin" },StringSplitOptions.None)[0];
            }
        }

    }
}