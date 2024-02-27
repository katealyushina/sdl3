using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

public class Program
{
    static async Task Main(string[] args)
    {
        string logFilePath = Environment.GetEnvironmentVariable("LOG_FILE_PATH") ?? "defaultLogFile.txt";
        using (StreamReader sr = new StreamReader("sdl3.conf"))
        {
            Console.WriteLine("Введите имя пользователя:");
            var userName = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            var password = Console.ReadLine();

            Console.WriteLine($"Добро пожаловать, {userName}!");

            string connectionStringFromConf = await sr.ReadLineAsync();
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(connectionStringFromConf);
            csb.Username = userName;
            csb.Password = password;

            string connectionString = csb.ToString();

            EntityCommander commander = new EntityCommander(logFilePath);
            string action = "";
            while (action != "/exit")
            {
                try
                {
                    Console.WriteLine("Пожалуйста, введите команду для доступа к системе (для получения справки введите /help)\n\n");
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        commander.command = Console.ReadLine();
                        if (commander.command == "/help")
                        {
                            commander.HelpPole();
                        }
                        if (commander.command == "/lookall")
                        {
                            await commander.LookPole(connection, logFilePath);
                        }

                        if (commander.command == "/create")
                        {
                            await commander.CreatePole(connection);
                        }
                        if (commander.command == "/open")
                        {
                            await commander.OpenTool(connection, logFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                    {
                        logWriter.WriteLine($"{DateTime.Now}: Ошибка: " + ex.Message);
                    }
                    Console.Error.WriteLine($"{DateTime.Now}: Ошибка: " + ex.Message + "\nПожалуйста, повторите ввод...");
                    break;
                }
            }
        }
    }
}

public class EntityCommander
{
    public string? command;
    private string logFilePath;

    public EntityCommander(string logFilePath)
    {
        this.logFilePath = logFilePath;
    }

    public void HelpPole()
    {
        Console.WriteLine("\t\tКоманды");
        Console.WriteLine("/lookall - вывод выбранной таблицы\n");
        Console.WriteLine("/create - создать сущность в базе данных\n");
        Console.WriteLine("/open - войти в сущность для манипуляций\n");
        Console.WriteLine("Список таблиц: Projects, Materials, Classes, Tests\n");
    }

    public async Task LookPole(NpgsqlConnection connection, string logFilePath)
    {
        Console.WriteLine("Введите название таблицы, используя caps lock: ");
        string tableName = Console.ReadLine();
        string query = $"SELECT * FROM {tableName}";
        await using (var command = new NpgsqlCommand(query, connection))
        {
            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i].ToString() + " ");
                        using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                        {
                            await logWriter.WriteAsync(reader[i].ToString() + " ");
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

         }
    }

    public async Task CreatePole(NpgsqlConnection connection)
    {
        Console.WriteLine("Введите имя проекта: ");
        var name = Console.ReadLine();
        Console.WriteLine("Введите код проекта: ");
        var code = Console.ReadLine();

        using (var command = new NpgsqlCommand("INSERT INTO PROJECTS (name, code) VALUES (@name, @code)", connection))
        {
            command.Parameters.AddWithValue("name", name);
            command.Parameters.AddWithValue("code", code);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task OpenTool(NpgsqlConnection connection, string logFilePath)
    {
        var action = "";
        Console.WriteLine("Введите id проекта: ");
        var idProject = Console.ReadLine();

        while (action != "/exit")
        {
            Console.WriteLine("Введите команды: ");
            Console.WriteLine("/show - Показать материалы");
            Console.WriteLine("/show +f - Показать материалы с фильтром");
            Console.WriteLine("/add - Добавить материал");
            Console.WriteLine("/update - Обновить материал");
            Console.WriteLine("Для возврата в главное меню введите '/exit'");
            action = Console.ReadLine();


            if (action == "/show") // Показать компоненты
            {
                await using var command = new NpgsqlCommand($"SELECT id, name, class, weight, length FROM Materials WHERE project_id = {idProject}", connection);
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i].ToString() + " ");
                        using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                        {
                            logWriter.Write(reader[i].ToString() + " ");
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            if (action == "/show +f") // Показать компоненты с фильтром
            {
                Console.WriteLine("Введите фильтр (например, weight, length): ");
                var field = Console.ReadLine();
                Console.WriteLine("Введите оператор (=, >, <): ");
                var operand = Console.ReadLine();
                Console.WriteLine("Введите значение: ");
                var value = Console.ReadLine();

                await using var command = new NpgsqlCommand($"SELECT id, name, class, weight, length FROM Materials WHERE {field} {operand} '{value}' AND project_id = {idProject}", connection);
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i].ToString() + " ");
                        using (StreamWriter logWriter = new StreamWriter(logFilePath, true))
                        {
                            logWriter.Write(reader[i].ToString() + " ");
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            if (action == "/add") // Добавить компонент
            {
                Console.WriteLine("Введите название материала:");
                var name = Console.ReadLine();
                Console.WriteLine("Введите тип:");
                var type = Console.ReadLine();
                Console.WriteLine("Введите вес: ");
                var weight = Console.ReadLine();
                Console.WriteLine("Введите длину: ");
                var length = Console.ReadLine();

                using var command = new NpgsqlCommand($"INSERT INTO Materials (project_id, name, class, weight, length) VALUES ({idProject}, '{name}', '{type}', {weight}, {length})", connection);
                await command.ExecuteNonQueryAsync();
            }

            if (action == "/update") // Обновить компонент
            {
                Console.WriteLine("Введите название материала:");
                var materialName = Console.ReadLine();
                Console.WriteLine("Введите название столбца: ");
                var column = Console.ReadLine();
                Console.WriteLine("Введите значение: ");
                var value = Console.ReadLine();

                using var command = new NpgsqlCommand($"UPDATE Materials SET {column} = {value} WHERE name = '{materialName}'", connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}