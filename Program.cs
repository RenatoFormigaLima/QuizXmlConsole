using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuizXmlConsole
{
   
    class Program
    {
        
        static void Main(string[] args)
        {
            //Criação dos dados necessários para rodar a aplicação
            string log, passw;
            string sysResp = "";
            //Obj_usuario 
            User user;
            
            //Login inicial
            Inicio:
            Console.Write("Digite o seu login: ");
            log = Console.ReadLine();
            Console.Write("Digite a sua senha: ");
            passw = Console.ReadLine();
            bool autenticado = User.AutenticarLogin(log,passw);

            if (autenticado)
            {
                user = new User(log,passw);
            }
            else
            {
                Console.WriteLine("Login ou senha inválidos!");
                Console.ReadKey();
                Console.Clear();
                goto Inicio;
            }
           
            while(sysResp != "Sair" && autenticado)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Clear();
                printMenu(user);

                sysResp = Console.ReadLine();

                switch (sysResp.ToLower())
                {
                    case "jogar": //Comando dado para ir para o menu de seleção de quiz
                        SysQuiz.Quiz();
                        break;
                    case "sair":
                        Console.WriteLine("Até logo!...");
                        break;
                    case "ca": //Comando dado para cadastrar assunto(type = 0)
                        CadastroDado(0, user);
                        break;
                    case "cq": //Comando dado para cadastrar questao(type = 1)
                        CadastroDado(1, user); 
                        break;
                    case "aa": //Comando dado para alterar assunto(type = 0)
                        alterarDado(0,user);
                        break;
                    case "aq": //Comando dado para alterar questao(type = 1)
                        alterarDado(1,user);
                        break;
                    default: 
                        Console.WriteLine("Comando não reconhecido!");
                        break;
                }

            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        #region MetodosDeOperacaoNoDB
        private static void CadastroDado(int tipo, User user) {
            switch (tipo)
            {
                case 0:
                    string[] assuntos = QuestOperation.GetAssuntos(); string assunto; bool assuntoJaExiste = false;

                    Console.Write("Digite o nome do novo assunto:");
                    assunto = Console.ReadLine();

                    for (int i = 0; i < assuntos.Length; i++)
                    {
                        if (assuntos[i] == assunto)
                        {
                            assuntoJaExiste = true;
                            break;
                        }
                    }
                    if (!assuntoJaExiste)
                    {
                        QuestOperation.AddAssunto(assunto, user);
                    }
                    else
                    {
                        Console.WriteLine("O assunto já exite no banco de dados!");
                    }

                    break;
                case 1:
                    QuestData aux_qd = QuestOperation.GetFormComplQ_Console();//Obj para a troca de informações para cadastro e alteração dos dados do BD
                    if (aux_qd != (null))
                    {
                        QuestOperation.AddQuest(aux_qd.Assunto, aux_qd, user);
                    }

                    break;
                default:
                    Console.WriteLine("ERRO, atribuição inválida");
                    break;
            }
        }
        
        private static void alterarDado(int tipo, User user)
        {
            
        }
        #endregion

        #region MetodosUIConsole
        //Exibe o menu principal
        private static void printMenu(User u) {

            
            Console.WriteLine("|+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|");
            Console.WriteLine("|+-+-+-+-+ Xml Quiz -+-+-+-+-+-+-|");
            Console.WriteLine("|+-+-+-+-+-+-V1.1+-+-+-+-+-+-+-+-|");
            Console.WriteLine("|+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"\n->Usuario: {u.Login}<-\n");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n->Menu<-\n");
            Console.WriteLine("-> [ca] Cadastrar assunto");
            Console.WriteLine("-> [cq] Cadastrar questão ");
           // Console.WriteLine("-> [aa] Alterar assunto");
           // Console.WriteLine("-> [aq] Alterar questao");
            Console.WriteLine("-> [jogar] Jogar o quiz");
            Console.WriteLine("-> [sair] Sair da aplicação\n");

            Console.WriteLine("==================================");
        }

        #endregion
    }
}
