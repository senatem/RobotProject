namespace RobotProject.uiElements
{
    public interface IUiElement
    {
        void Reorient(int? x = null, int? y = null, int? w = null, int? h = null);

        public abstract void Reorient(Geometry.Rectangle r);

    }
}