namespace P2PBank.Data.MicrosoftSql;

using Microsoft.Data.SqlClient;

using P2PBank.Data.Interface;
using Application.Interface.Models;
using P2PBank.Utils;

public class MicrosoftSqlAccountRepository: IAccountRepository
{
    private MicrosoftSqlDatabase database;
    private Log log;
    
    public int ClientCount
    {
        get
        {
            using var command = new SqlCommand("SELECT COUNT(id) as num FROM account_table", database.Connection);
            using var reader = command.ExecuteReader();
            if(reader.Read())
            {
                return reader.GetInt32(0);
            }
            throw new Exception("sql command failed execution");
        }
    }

    public long TotalBalance
    {
        get {
            using var command = new SqlCommand("SELECT SUM(balance) as num FROM account_table", database.Connection);
            using var reader = command.ExecuteReader();
            if(reader.Read())
            {
                return reader.GetInt64(0);
            }
            throw new Exception("failed to calculate total balance");
        }
    }

    public MicrosoftSqlAccountRepository(MicrosoftSqlDatabase database, Log log)
    {
        this.database = database;
        this.log = log;
    }

    public void Insert(Account account)
    {
        log.Info($"creating account");
        using var command = new SqlCommand(
            "INSERT INTO account_table(balance) VALUES (@balance); SELECT CAST(SCOPE_IDENTITY() AS INT)",
            database.Connection
        );
        command.Parameters.AddWithValue("@balance", account.Balance);
        account.Id = (int)command.ExecuteScalar();

        log.Info($"created account {account}");
    }
    
    public void Update(Account oldAccount, Account newAccount)
    {
        using var command = new SqlCommand("UPDATE account_table SET balance=@balance WHERE id = @id", database.Connection);
        command.Parameters.AddWithValue("@id", oldAccount.Id);
        command.Parameters.AddWithValue("@balance", newAccount.Balance);

        command.ExecuteNonQuery();
    }
    
    public Account SelectById(int id)
    {
        using var command = new SqlCommand("SELECT id, balance FROM account_table WHERE id = @id", database.Connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader(); 
        if(reader.Read())
        {
            return new Account
            {
                Id = reader.GetInt32(0),
                Balance = reader.GetInt64(1)
            };
        }
        throw new Exception($"Account with id {id} not found"); 
    }

    public void Delete(Account account)
    {
        using var command = new SqlCommand("DELETE FROM account_table WHERE id = @id", database.Connection);
        command.Parameters.AddWithValue("@id", account.Id);

        command.ExecuteNonQuery();
    }

}