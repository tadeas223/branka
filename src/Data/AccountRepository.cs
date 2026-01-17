namespace Data;

using Application.Models;
using Microsoft.Data.SqlClient;
using Utils;

public class AccountRepository
{
    private Database database;

    
    public int Count
    {
        get
        {
            using var command = new SqlCommand("SELECT COUNT(account_id) as num FROM account", database.Connection);
            using var reader = command.ExecuteReader();
            if(reader.Read())
            {
                return reader.GetInt32(0);
            }
            throw new DatabaseException("sql command failed execution");
        }
    }

    public AccountRepository(Database database)
    {
        this.database = database;
    }

    public void Insert(Account account)
    {
        using var command = new SqlCommand(
            "INSERT INTO account_table(balance) VALUES (@balance); SELECT CAST(SCOPE_IDENTITY() AS INT)",
            database.Connection
        );
        command.Parameters.AddWithValue("@balance", account.Balance);
        account.Id = (int)command.ExecuteScalar();

        Log.Info($"created account {account}");
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
        throw new DatabaseException($"Account with id {id} not found"); 
    }

}