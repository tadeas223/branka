namespace P2PBank.Data.Interface;

using Application.Interface.Models;

public interface IAccountRepository
{
    public int ClientCount {get;}
    public long TotalBalance {get;}
    public void Insert(Account account);
    public void Update(Account oldAccount, Account newAccount);
    public Account SelectById(int id);
    public void Delete(Account account);
}