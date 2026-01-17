namespace Data;

using Application.Models;
using Microsoft.Data.SqlClient;
using Utils;

public class AccountRepository
{
    private Database database;

    
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
            throw new DatabaseException("sql command failed execution");
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
            throw new DatabaseException("failed to calculate total balance");
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

    public void Delete(Account account)
    {
        using var command = new SqlCommand("DELETE FROM account_table WHERE id = @id", database.Connection);
        command.Parameters.AddWithValue("@id", account.Id);

        command.ExecuteNonQuery();
    }

}