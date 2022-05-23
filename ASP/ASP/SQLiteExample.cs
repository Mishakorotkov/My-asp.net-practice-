using System.Data;
using System.Data.SQLite;
namespace SQLiteExample
{
    class Sq
    {
        SQLiteConnection connection;
        SQLiteCommand command;
        public bool Connect(string fileName)
        {
            try
            {
                connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3; FailIfMissing=False");
                connection.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
                return false;
            }
        }
        public string db_selector(string log1, string passw)
        {
            command.CommandText = $"SELECT * FROM Reg WHERE login=\"{ log1 }\"";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);
            if (data.Rows.Count > 0)
            {
                Console.WriteLine("Такой логин уже есть,выберите другой");
                return "Такой логин уже есть,выберите другой";
            }
            else if (log1 == "" | log1.Contains(" "))
            {
                Console.WriteLine("Вы не ввели символы или написали логин или пароль раздельно");
                return "Вы не ввели символы или написали логин или пароль раздельно";
            }
            else if (log1.Length < 6 | passw.Length < 6)
            {
                Console.WriteLine("Вы ввели слишком короткий логин или пароль, введите хотя-бы 6 символов");
                return "Вы ввели слишком короткий логин или пароль, введите хотя-бы 6 символов";
            }
            return "true";
        }
        public string Delete_acc(string loggg, string passs)
        {
            command.CommandText = $"SELECT * FROM Reg WHERE login=\"{ loggg }\"";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);
            if (data.Rows.Count > 0)
            {
                DataRow row = data.Rows[0];
                return loggg;
            }
            return "no";
        }
        public bool Vhod(string login,string pass)
        {
            command.CommandText = $"SELECT * FROM Reg WHERE login=\"{ login }\"";
            DataTable data = new DataTable();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);
            if (data.Rows.Count > 0)
            {
                DataRow row = data.Rows[0];
                if (row.Field<string>("password") == pass)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        public bool createTable(string sql)
        {
            try
            {
                Console.WriteLine("Connected");
                command = new SQLiteCommand(connection)
                {
                    CommandText = sql
                };
                command.ExecuteNonQuery();
                Console.WriteLine("Запрос выполнен");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}