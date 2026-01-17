namespace P2PBank.Application.Interface;

public interface ICommandFactory
{
    public Command Create(string commandStr);
}