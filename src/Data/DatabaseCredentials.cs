using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Represents login credentials for the database.
/// </summary>
public class DatabaseCredentials
{
    private string username;
    private string password;
    private string databaseName;
    private string url;
    
    public string Username { get => username; }
    public string Password { get => password; }
    public string DatabaseName { get => databaseName; }
    public string Url { get => url; }

    /// <summary>
    /// Prebuilds the <see cref="SqlConnectionStringBuilder"/> without setting the database name.
    /// </summary>
    [JsonIgnore]
    public SqlConnectionStringBuilder ConnectionBuilderNoDB { 
        get {
            return new SqlConnectionStringBuilder
            {
                DataSource = url,
                UserID = username,
                Password = password,
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
                DataSource = url,
                UserID = username,
                Password = password,
                InitialCatalog = databaseName,
                TrustServerCertificate = true,
            };
        }
    }
    
    /// <summary>
    /// Creates the credentials ands sets its variables idk.
    /// </summary>
    public DatabaseCredentials(string url, string databaseName, string username, string password) 
    {
        this.username = username;
        this.password = password;
        this.databaseName = databaseName;
        this.url = url;
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
        return $"DatabaseCredentials{{url={url}, databaseName={databaseName}, username={username}, password={password}}}";
    }
}