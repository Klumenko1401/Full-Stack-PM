using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;

class Program
{
    static async Task Main(string[] args)
    {
        string name = "" ;

        while (string.IsNullOrEmpty(name))
        {
            Console.Write("Введiть iм'я: ");
            name = Console.ReadLine();

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Iм'я не може бути порожнiм! Спробуйте ще раз.");
            }
        }

        string url = $"https://api.agify.io?name={name}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\nОтримана вiдповiдь з API:");
                Console.WriteLine(result);

                SaveToDatabase(name, result);
                Console.WriteLine("\nДанi збережено в базу.");

                Console.WriteLine("\nДанi з бази:");
                GetFromDatabase();
            }
            else
            {
                Console.WriteLine("Помилка запиту до API");
            }
        }
    }

    static void SaveToDatabase(string name, string data)
    {
        using (var con = new SQLiteConnection("Data Source=agify.db;Version=3;"))
        {
            con.Open();
            new SQLiteCommand("CREATE TABLE IF NOT EXISTS AgePrediction (Id INTEGER PRIMARY KEY, Name TEXT, Data TEXT)", con).ExecuteNonQuery();
            var cmd = new SQLiteCommand("INSERT INTO AgePrediction (Name, Data) VALUES (@Name, @Data)", con);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Data", data);
            cmd.ExecuteNonQuery();
        }
    }

    static void GetFromDatabase()
    {
        using (var con = new SQLiteConnection("Data Source=agify.db;Version=3;"))
        {
            con.Open();
            var cmd = new SQLiteCommand("SELECT Name, Data FROM AgePrediction", con);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read()) Console.WriteLine($"Iм'я: {reader["Name"]}, Данi: {reader["Data"]}");
            }
        }
    }
}
