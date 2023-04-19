using System.Data.SqlClient;

namespace hmwk51.data
{
    public class Image
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public int Views { get; set; }
        public string FileName { get; set; }
    }
    public class Manager
    {
        public string _connectionString;
        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Add(string fileName, string password)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO Images(FileName, Password, Views)
VALUES(@fileName,@password,@views); SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@fileName", fileName);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@views", 1);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();
        }
        public Image GetImage(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT * FROM Images 
WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            Image image = new()
            {
                Id = (int)reader["Id"],
                FileName = (string)reader["FileName"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]
            };

            return image;
        }
        public void UpdateViews(int id, int views)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"UPDATE Images
SET Views = @views
WHERE Id = @id";
            command.Parameters.AddWithValue("@views", views);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}