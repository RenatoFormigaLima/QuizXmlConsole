using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Biblioteca para o tratamento de arquivos xml, utiliza o Linq -> Query integrada a linguagem (similar a sintaxe do SQL)
using System.Xml.Linq;


namespace QuizXmlConsole
{
    //Classe responsável por fornecer meios de captura de dados no arquivos xml (Deserialização) e fornecer meios de escrita de dados no arquivo xml(Serialização)
    //Possui uma aplicação que não trata erros e além disso a autenticação e validação para as alterações é fraca
    class QuestOperation
    {
        private readonly static string QuestDB = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\DataBase\QuestsDB.xml"; //Local do Banco de Dados

        #region AdicaoDeElementosNoBD
        //Função responsável por tratar da adição de novos assuntos no Banco de Dados Local (No Arquivo xml)
        public static void AddAssunto(string idAssunto, User u)
        {
            //Autenticação de usuário
            if(User.AutenticarLogin(u.Login,u.Senha) && u.Lv_acesso == "admin") //Apenas o adm pode fazer alterações do BD
            {
                XElement root = XElement.Load(QuestDB); //Elemento raiz do banco de dados

                XElement assuntoNew = new XElement("Assunto"); //Criação do nó assunto
                assuntoNew.Add(new XAttribute("id", idAssunto)); //Adição do id no nó assunto
                assuntoNew.Add(new XElement("Questoes")); //Adição do nó questoes para adicionar posteriormente as questões relacionadas no assunto

                root.Add(assuntoNew); //Adiciona o assunto na raiz do banco de dados
                root.Save(@".\DataBase\QuestsDB.xml"); //Salva as alterações
            }
            else
            {
                //Tratamento caso a autenticação falhe
                Console.WriteLine("Erro ao autenticar o login!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        //Função responsável por adicionar novas questões ao Banco de Dados
        public static void AddQuest(string idAssunto, QuestData qd, User u)
        {
            //Autenticação de usuário
            if (User.AutenticarLogin(u.Login, u.Senha) && u.Lv_acesso == "admin") { //Apenas o adm pode realizar as alterações

                int cont = 0; //Contador auxiliar para a adição de alternativas
                string[] assuntos = GetAssuntos();  //Listagem dos assuntos para a adição ao nó assunto correspondente

                XElement root = XElement.Load(QuestDB); //Carrega a raiz do Banco de Dados para realizar as adições
                
                XElement newData = new XElement("Questao"); //Cria o nó Questao que irá conter a questão propriamente dito
                XElement texto = new XElement("texto"); //Cria o nó texto que irá conter os dados de texto/imagem da questao
                XElement alternativas = new XElement("alternativas"); //Cria o nó alternativas que está dentro do nó texto e nela irá conter o nó alternativa
                List<XElement> alternativaContent = new List<XElement>(); //Cria a lista de nós que estarão no nó alternativas, nele irá conter o texto propriamente dito das questões

                newData.Add(new XAttribute("qid",qd.Id)); //Define o id da questao, *alterar para fazer uma listagem automática para não conter id duplicado

                texto.Add(new XElement("pergunta", qd.Pergunta));//Adiciona o nó pergunta dentro do nó texto e define o conteúdo
                texto.Add(new XElement("img","n")); //Adiciona o nó imagem ao nó texto e define o seu conteúdo, *Adicionar no futuro urls das imagens

                //Laço de repetição que irá: criar o nó alternativa, adicionar o texto das alternativas e adicionar no nó alternativas 
                foreach(string txt in qd.Alternativas)
                {
                    alternativaContent.Add(new XElement("alternativa",txt));
                }
                //Laço de repetição que irá adicionar os aids(Id de alternativa) nas alternativas
                foreach(XElement xe in alternativaContent)
                {
                    xe.Add(new XAttribute("aid",cont.ToString()));
                    alternativas.Add(xe);
                    cont++;
                }

                //Após construir o nó alternativas o mesmo é adicionado ao nó pai, no caso o nó alternativas
                texto.Add(alternativas);
                //Após a construção do nó texto ele é adicionado ao nó pai, no caso o  nó Questao
                newData.Add(new XElement("resposta", qd.Resposta)); //Adiciona o nó resposta e seu conteúdo
                newData.Add(new XElement("score", qd.Score.ToString())); //Adiciona o nó Score e seu conteúdo
                newData.Add(texto);

                //Filtro para a adição do nó Questão ao nó pai correspondente ao assunto
                IEnumerable<XElement> quests = FiltroQuestao(root,idAssunto);

                //Após o filtro, o nó questão é adicionado ao nó assunto correspondente. *Essa não é a melhor mandeira de se fazer isso!
                foreach (XElement el in quests)
                {
                    el.Add(newData);
                }

                //Salva as alterações
                root.Save(@".\DataBase\QuestsDB.xml");

            }
            else
            {
                //Tratamento de erro de autenticação
                Console.WriteLine("Erro ao autenticar o login!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        }
        #endregion
        //Filtro para o nó questão pertencente ao assunto escolhido, essa função retorna uma Lista de elementos da questão requerida
        private static IEnumerable<XElement> FiltroQuestao(XElement root, string idAssunto)
        {
            //Listagem para o primeiro filtro, ele irá para o nó assunto correspondente
            IEnumerable<XElement> questRoot =
                from el in root.Elements("Assunto")
                where (string)el.Attribute("id") == idAssunto
                select el;

            //Listagem para o segundo filtro, ele irá selecionar o nó questoes
            IEnumerable<XElement> quests =
            from el in questRoot.Elements("Questoes")
            select el;

            //Retorna o resultado do filtro
            return quests;
        }

        #region MetodosDeObtencaoDeDados
        //Função responsável por capturar todas as questões em um determinado assunto
        public static List<QuestData> GetALLQuests(string idAssunto)
        {
            //Lista de objetos QuestData, ou seja, das representações dos dados de todas as questões
            List<QuestData> quest = new List<QuestData>();
            //Carrega o Banco de Dados do arquivo
            XElement root = XElement.Load(QuestDB);

            //Filtro por questão do assunto escolhido
            IEnumerable<XElement> quests = FiltroQuestao(root,idAssunto);
            
            //Coleta dos dados da questão e armazenamento na lista quest
            foreach (XElement el in quests.Elements())
            {
                //Criação do dado de uma questão
                QuestData qd = new QuestData()
                {
                    Id = (String)el.Attribute("qid"), //Captura e armazena o id 
                    Pergunta = (string)el.Element("texto").Element("pergunta"), //Captura e armazena a pergunta
                    Alternativas = GetAlternativas(el), // Captura e armazena as alternativas
                    Resposta = (String)el.Element("resposta"), //Captura e armazena a resposta
                    Score = int.Parse((string)el.Element("score")) //Capura e armazena o score
                };

                //Adição do dado questao a lista de questões
                quest.Add(qd);
            }

            //retorna as questões
            return quest;
        }

        //Caputura as alternativas e as retorna, *Alterar dps para um array, pois é desnecessário uma lista para armazenar apenas strings
        private static List<string> GetAlternativas(XElement xe) //Nó específico de uma questão, é necessário fazer uma filtragem prévia!
        {
            //Conteúdo textual das alternativas
            List<string> alternativa = new List<string>();

            //Filtro para a captura das alternativas do nó da questao em específico
            IEnumerable<XElement> txt =
                from el in xe.Descendants("alternativas")
                select el;

            //Converte o conteúdo do nó em texto manipulável
            foreach (XElement el in txt.Elements())
            {
                alternativa.Add((string)el.Value);
            }

            //retorna uma lista de alternativas de forma textual
            return alternativa;

        }
        //Função responsável por fazer uma listagem dos assuntos e retronar o id
        public static string[] GetAssuntos()
        {
            int index_aux = 0; //Variável auxiliar para percorrer o vetor
            string[] assuntos; //Vetor dos textos dos assuntos
            XElement root = XElement.Load(QuestDB); //Carrega o Banco de Dados

            assuntos = new string[root.Elements("Assunto").Attributes("id").Count()]; //Instancia o Vetor para conter apenas a quantidade
                                                                                      //de assuntos que existem de forma automática

            //Laço de repetição que lista os elementos dos nos Assunto e os armazena no vetor 
            foreach (XElement el in root.Elements())
            {
                assuntos[index_aux] = (string)el.Attribute("id");
                index_aux++;
            };
            //Retorna o Vetor Dos assuntos
            return assuntos;
        }
        //Função responsável por pegar o último Qid em qualquer assunto cadastrado na Banco de Dados
        private static int GetLastQid(string aid)
        {
            int last_Qid; //Variável que armazena o valor numérico do id
            XElement root = XElement.Load(QuestDB); //Carrega o Banco de Dados
            IEnumerable<XElement> quest = FiltroQuestao(root, aid); //Filtra para o acesso dos ids do tópico selecionado

            //teste para verificar se o filtro retornou a lista de questões
            try {
                //Atribuição do valor do id
                last_Qid = Int32.Parse(quest.Elements("Questao").Attributes("qid").Last().Value);

            } 
            catch (Exception){//Caso não encontre (quest == vazio) o id é negativo 

                last_Qid = -1;

            }
            //Retorna o valor do último id, ou, -1 caso não tenha questão ainda no tópico selecionado, ou não existe o tópico
            return last_Qid;

        }
        #endregion

        #region BlocoDeNotas_console
        //Função que realiza a captura de dados em formato similar ao bloco de notas, porém muito limitado
        public static string CaputuraTextoMaior_Console()
        {
            //Menu do bloco de notas
            Console.Clear();
            Console.WriteLine("                   Bloco de notas                     ");
            Console.WriteLine("Digite o texto: \nComando:\n[\\] -> Terminar a leitura dos dados (Acrecentar como se fosse uma última linha)");
            //Variáveis de controle
            string inputFormTxt = ""; string texto = "";
            while (inputFormTxt != "\\") // o \ siginifica para a função que é para encerrar a capitura de dados
            {
                inputFormTxt = Console.ReadLine(); //lê a linha
                if (inputFormTxt != "\\") //verifica se foi digitado a \, ela não é para ser armazenada
                {
                    texto += inputFormTxt + "\n";
                }
            }
            //retorna o texto lido pelo bloco de notas
            return texto;
        }
        #endregion

        #region FormularioViaConsole
        //Função responsável por gerar o formulário para obtenção dos dados de questão e retornar um objeto de dados 
        public static QuestData GetFormComplQ_Console()
        {
            //Objeto de dados de questão
            QuestData dadosFormulario = new QuestData();
            //Variáveis auxiliares
            int listAltRange; int aux = 0; 
            //Variáveis para a captura e comparação dos dados
            string[] assuntos = GetAssuntos(); string assunto_input;

            //Inicio do processo, exibindo informações iniciais
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Digite o assunto em que a questao pertence: ");

            //Leitura do tópico que será adicionado a questão, verifica se existe
            assunto_input = Console.ReadLine();
            for(int i = 0; i < assuntos.Length; i++) { 
                //Se existe então a variável auxiliar vai receber o valor 1
                if(assuntos[i] == assunto_input)
                {
                    aux = 1;
                    break;
                }
            }

            if (aux == 1) //Variável auxiliar vai ser a responsável por verificar se existe ou não o tópico
            {
                //Input de texto da pergunta
                dadosFormulario.Assunto = assunto_input;
                Console.WriteLine("Digite o texto da pergunta:\n");

                dadosFormulario.Pergunta = CaputuraTextoMaior_Console();
                //Input de texto para as alternativas
                Console.WriteLine("Digite a quantidade de alternativas: "); //Captura da quantidade requerida para a criação de alternativas
                listAltRange = Int32.Parse(Console.ReadLine());
                //Captura dos dados de texto efeitivamente
                Console.WriteLine("Digite as alternativas em ordem: ");

                for (int i = 0; i < listAltRange; i++)
                {
                    Console.Write((char)(i + 0x61)); //Imprime a formatação a), b) e assim por diante
                    dadosFormulario.Alternativas.Add($"{(char)(i + 0x61)})" + CaputuraTextoMaior_Console()); //Armazena o texto junto com a letra da alternativa
                }
                //Input da alternativa
                Console.Write("Digite a resposta da questao: ");
                dadosFormulario.Resposta = Console.ReadLine();
                //Input do score
                Console.Write("Digite o score da questao: ");
                dadosFormulario.Score = Int32.Parse(Console.ReadLine());

                //Auto numeração do id da questão
                aux = GetLastQid(assunto_input) + 1; //reaproveitamento da variável aux
                dadosFormulario.Id = aux.ToString();

                //retorna o objeto de dados de questão
                return dadosFormulario;
            }
            else
            {
                //Tratamento caso ocorre um erro de validação 
                Console.WriteLine("Erro, assunto nao encontrado no Banco de Dados!");

                //retorna um objeto com o valor nulo
                return null;
            }
            
        }
        #endregion
    }

}
