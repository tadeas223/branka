namespace Data;

using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents login credentials for the database.
/// </summary>
public class DatabaseCredentials
{
    public string Username { set; get; }
    public string Password { set; get; }
    public string DatabaseName { set; get; }
    public string Url { set;get; }

    public DatabaseCredentials()
    {
        Username = "<username>";
        Password = "<password>";
        DatabaseName = "<database>";
        Url = "<url>";
    }

    /// <summary>
    /// Prebuilds the <see cref="SqlConnectionStringBuilder"/> without setting the database name.
    /// </summary>
    [JsonIgnore]
    public SqlConnectionStringBuilder ConnectionBuilderNoDB { 
        get {
            return new SqlConnectionStringBuilder
            {
                DataSource = Url,
                UserID = Username,
                Password = Password,
                TrustServerCertificate = true,
            };
        }
    }

    /// <summary>
    /// Prebuilds the <see cref="SqlConnectionStringBuilder"/> with setting the database name.
    /// </summary>
    [JsonIgnore]
    public SqlConnectionStringBuilder ConnectionBuilder { 
        get {
            return new SqlConnectionStringBuilder
            {
                DataSource = Url,
                UserID = Username,
                Password = Password,
                InitialCatalog = DatabaseName,
                TrustServerCertificate = true,
            };
        }
    }
    
    /// <summary>
    /// Creates the credentials ands sets its variables idk.
    /// </summary>
    public DatabaseCredentials(string url, string databaseName, string username, string password) 
    {
        Username = username;
        Password = password;
        DatabaseName = databaseName;
        Url = url;
    }
    
    /// <summary>
    /// Creates a <see cref="DatabaseCredentials"/> object from json.
    /// </summary>
    /// <param name="json">Json that represents the credentials.</param>
    /// <returns>Credentials from json</returns>
    public static DatabaseCredentials FromJson(string json)
    {
        DatabaseCredentials? result = JsonSerializer.Deserialize<DatabaseCredentials>(json);
        if(result == null) throw new JsonException("Failed to parse credentials");
        return result;
    }
    
    /// <summary>
    /// Converts <see cref="DatabaseCredentials"/> to json.
    /// </summary>
    /// <returns>String with the json</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public override string ToString()
    {
        return $"DatabaseCredentials{{Url={Url}, DatabaseName={DatabaseName}, username={Username}, Password=<secret>}}";
    }
}