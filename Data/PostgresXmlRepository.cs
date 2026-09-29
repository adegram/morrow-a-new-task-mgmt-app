using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Npgsql;

namespace Taskflow.Data;

/// <summary>Shares ASP.NET Core Data Protection keys across serverless instances.</summary>
public sealed class PostgresXmlRepository(string connectionString) : IXmlRepository
{
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        var elements = new List<XElement>();
        using var db = new NpgsqlConnection(connectionString);
        db.Open();
        using var command = db.CreateCommand();
        command.CommandText = "SELECT xml FROM data_protection_keys ORDER BY id";
        using var reader = command.ExecuteReader();
        while (reader.Read())
            elements.Add(XElement.Parse(reader.GetString(0)));
        return elements;
    }

    public void StoreElement(XElement element, string? friendlyName)
    {
        var idText = element.Attribute("id")?.Value
            ?? throw new InvalidOperationException("Data Protection key XML must contain an id attribute.");
        using var db = new NpgsqlConnection(connectionString);
        db.Open();
        using var command = db.CreateCommand();
        command.CommandText = "INSERT INTO data_protection_keys (id, xml) VALUES (@id, @xml) ON CONFLICT (id) DO NOTHING";
        command.Parameters.AddWithValue("id", Guid.Parse(idText));
        command.Parameters.AddWithValue("xml", element.ToString(SaveOptions.DisableFormatting));
        command.ExecuteNonQuery();
    }
}
