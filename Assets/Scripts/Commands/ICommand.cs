namespace ARKitect.Commands
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
