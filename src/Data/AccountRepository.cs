namespace Data;

using Application.Commands;
using Application.Models;
using Microsoft.Data.SqlClient;

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
        using var command = new SqlCommand("INSERT INTO account(balance) VALUCES (@balance)", database.Connection);
        command.Parameters.AddWithValue("@balance", account.Balace);
        
        using var command2 = new SqlCommand("SELECT TOP 1 account_id FROM account ORDER BY account_id DESC", database.Connection);
        var result = command2.ExecuteScalar();
        int id = Convert.ToInt32(result); 

        account.Id = id;
    }
    
    public void Update(Account oldAccount, Account newAccount)
    {
        using var command = new SqlCommand("UPDATE account SET balance=@balance WHERE account_id = @id");
        command.Parameters.AddWithValue("@id", oldAccount.Id);
        command.Parameters.AddWithValue("@balance", newAccount.Id);

        command.ExecuteNonQuery();
    }
    
    public Account SelectById(int id)
    {
        using var command = new SqlCommand("SELECT account_id, balance FROM account WHERE account_id = @id", database.Connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader(); 
        if(reader.Read())
        {
            return new Account(reader.GetInt32(0), reader.GetString(1));
        }
        throw new DatabaseException($"Account with id {id} not found"); 
    }

}