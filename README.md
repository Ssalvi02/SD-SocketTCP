Faça um servidor para processar as seguintes mensagens dos clientes. O servidor deve suportar mensagens
de múltiplos clientes. Use o TCP. As mensagens de solicitação estão no formato String UTF:

CONNECT user, password
* Conecta o usuário user a sua área no servidor se a senha password conferir e o usuário user existir. Em caso de
sucesso, devolver SUCCESS como String UTF. Em caso de falha, devolver ERROR. Obs: password deve ser enviado
o hash em SHA-512.

PWD
* Devolve o caminho corrente (PATH) usando String UTF separando os diretórios por barra(/).
CHDIR path
* Altera o diretório corrente para path, retornado uma String UTF, SUCCESS, no caso de sucesso, e ERROR, no caso
de erro.

GETFILES
* Devolve os arquivos do diretório corrente no servidor.
Formato:
* devolve um inteiro indicando o número de arquivos como String UTF.
* devolve cada arquivo como uma String UTF.

GETDIRS
* Devolve os diretórios do diretório corrente no servidor.
Formato:
* devolve um inteiro indicando o número de diretórios como String UTF.
* devolve cada diretório como uma String UTF.

EXIT
* Finaliza a conexão