namespace DefaultNamespace
{
    public interface IInterfaceController
    {
        void Setup();
        void Refresh();
        void AddDependantController(IInterfaceController dependantController);
    }
}