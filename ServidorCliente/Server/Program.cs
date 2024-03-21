using System.Security.Cryptography;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TCPServer
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        // Configurar a linha de comando
        var rootCommand = new RootCommand
        {
            new Option<int>(
                "--port",
                description: "Porta do servidor"),
            new Option<string>(
                "--ip",
                description: "Endereço IP do servidor")
        };

        rootCommand.Handler = CommandHandler.Create<int, string>(async (port, ip) =>
        {
            await StartServer(port, ip);
        });

        // Analisar os argumentos de linha de comando e iniciar o servidor
        await rootCommand.InvokeAsync(args);
    }

    static async Task StartServer(int port, string ip)
    {
        // Verificar se a porta e o endereço IP foram fornecidos
        if (port == 0 || string.IsNullOrWhiteSpace(ip))
        {
            Console.WriteLine("Por favor, forneça a porta e o endereço IP.");
            return;
        }

        // Crie um objeto TcpListener
        IPAddress ipAddress = IPAddress.Parse(ip);
        TcpListener server = new TcpListener(ipAddress, port);

        // Comece a ouvir por conexões
        server.Start();
        Console.WriteLine($"Servidor TCP iniciado em {ip}:{port}...");

        while (true)
        {
            // Aceite a conexão de um cliente
            TcpClient client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Cliente conectado...");

            // Lide com a conexão do cliente em uma tarefa separada
            _ = HandleClientAsync(client);
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            while (true){
                // Obtenha o fluxo de entrada e saída da conexão
                NetworkStream stream = client.GetStream();

                // Leia os dados do cliente
                byte[] data = new byte[256];
                int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                string message = Encoding.ASCII.GetString(data, 0, bytesRead);
                Console.WriteLine("Mensagem recebida do cliente: {0}", message);

                if(string.Equals(CheckCommand(message.ToUpper()), "CONNECT"))
                {
                    var resultauth = ProcessMessage(message);
                    Console.WriteLine(resultauth);
                    if(resultauth == "SUCCESS")
                    {
                        Console.WriteLine("Cliente autenticado.");
                    }
                    else
                    {
                        Console.WriteLine("Cliente não autenticado.");
                    }
                }

                // Feche a conexão
                if(string.Equals(message.ToUpper(), "EXIT"))
                {
                    client.Close();
                    Console.WriteLine("Conexão fechada com o cliente.");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao lidar com a conexão do cliente: " + ex.Message);
        }
    }

    static string CheckCommand(string message)
    {
        string[] cmd = message.Split(' ');

        return cmd[0];
    }

    static string ProcessMessage(string message)
    {
        // Divide a mensagem em partes
        string[] parts = message.Split(' ');

        // Verifica se a mensagem é no formato correto
        if (parts.Length != 3 || parts[0] != "CONNECT")
        {
            return "ERROR";
        }

        // Obtém o nome de usuário e a senha
        string username = parts[1];
        string passwordHash = parts[2];

        // Verifica o usuário e a senha (substitua isso pela lógica de autenticação real)
        if (VerifyCredentials(username, passwordHash))
        {
            return "SUCCESS";
        }
        else
        {
            return "ERROR";
        }
    }
    static bool VerifyCredentials(string username, string passwordHash)
    {
        // Lógica de verificação de credenciais (substitua isso pela sua lógica real)
        // Aqui, apenas retornamos true se o nome de usuário for "user" e a senha for "password"
        // Lembre-se de usar uma solução de hash mais segura na produção
        return CheckUsername(username) && Check512Hash(passwordHash);
    }

    static bool CheckUsername(string username)
    {
        string hashtxt = File.ReadAllText("D:/DatabaseServer/hash.txt");
        string[] splitedhashtxt = hashtxt.Split(new string[] {":", "\n"}, StringSplitOptions.None);
        
        for (int i = 0; i < splitedhashtxt.Length; i = i+2)
        {
            if(Equals(splitedhashtxt[i], username))
            {
                Console.WriteLine("ACHEI USER");
                return true;
            }
        }
        return false;
    }

    static bool Check512Hash(string input)
    {
        string hashtxt = File.ReadAllText("D:/DatabaseServer/hash.txt");
        string[] splitedhashtxt = hashtxt.Split(new string[] {":", "\n"}, StringSplitOptions.None);

        for (int i = 1; i < splitedhashtxt.Length; i += 2)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(splitedhashtxt[i]);
            splitedhashtxt[i] = Encoding.UTF8.GetString(bytes);
        

            //ESSA MERDA AQUI NÃO FAZ SENTIDO ALGUM PORRRRRRAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            if(string.Equals(splitedhashtxt[i], CalculateSHA512Hash(input), StringComparison.InvariantCulture))
            {
                Console.WriteLine("ACHEI SENHA");
                return true;
            }
        }

        return false;
    }

    static string CalculateSHA512Hash(string input)
    {
        var message = Encoding.UTF8.GetBytes(input);


        using (var shaM = SHA512.Create())
        {
            string hex = "";
            var hashValue = shaM.ComputeHash(message);

            foreach (byte x in hashValue)
            {
                hex += string.Format("{0:x2}", x);
            }

            //Console.WriteLine(hex);

            return hex;
        }
    }
}
