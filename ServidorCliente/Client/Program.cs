using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;




class TCPClient
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

        rootCommand.Description = "Cliente TCP";

        // Define o manipulador para lidar com a chamada de linha de comando
        rootCommand.Handler = CommandHandler.Create<int, string>(async (port, ip) =>
        {
            await ConnectToServer(port, ip);
        });

        // Analisar os argumentos de linha de comando
        await rootCommand.InvokeAsync(args);
    }

    static async Task ConnectToServer(int port, string ip)
    {
        // Verificar se a porta e o endereço IP foram fornecidos
        if (port == 0 || string.IsNullOrWhiteSpace(ip))
        {
            Console.WriteLine("Por favor, forneça a porta e o endereço IP.");
            return;
        }

        try
        {

            // Conecte-se ao servidor
            using (TcpClient client = new TcpClient(ip, port))
            {
                //Auth


                Console.WriteLine("Conectado ao servidor. Digite 'EXIT' para sair.");

                while (true)
                {
                    // Obtenha o fluxo de entrada e saída da conexão
                    NetworkStream stream = client.GetStream();

                    Console.Write("$> ");
                    string input = Console.ReadLine();

                    // Envie a mensagem para o servidor
                    byte[] data = Encoding.UTF8.GetBytes(input);
                    await stream.WriteAsync(data, 0, data.Length);

                    if (input.ToUpper() == "EXIT")
                    {
                        break;
                    }

                    // Leia a resposta do servidor
                    // data = new byte[256];
                    // int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                    // string response = Encoding.UTF8.GetString(data, 0, bytesRead);
                    // Console.WriteLine("Resposta do servidor: {0}", response);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao conectar ao servidor: " + ex.Message);
        }
    }
}
