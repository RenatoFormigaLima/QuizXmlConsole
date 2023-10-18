using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizXmlConsole
{
    //Função responsável por realizar o Quiz, utiliza as funções de captura de dados de QuestOperation.
    class SysQuiz
    {
        private static List<QuestData> questoes; //Questões
        
        public static void Quiz()
        {
            string systemResp="";

            while (systemResp.ToLower() != "sair")
            {
                PrintMenuQuiz();
                systemResp = Console.ReadLine();
                int outputSys = OutputResp(systemResp);

                if (outputSys < 0) {
                    Console.WriteLine("Comando nao reconhecido!");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            
            

        }

        private static int DoQuiz(string QPer, List<string> Qalter, string Qresp, int Qscore, int Qrange, string id)
        {
            string txt_aux;

            Console.WriteLine($"{Qscore}pts\n{int.Parse(id)+1}){QPer}");

            foreach(string text in Qalter)
            {
                txt_aux = text.Replace("\\n","\n");
                Console.WriteLine($"{txt_aux}");
            }
            
            Console.WriteLine("==================================");
            Console.Write("Resposta:");
            txt_aux = Console.ReadLine();

            if(Qresp == txt_aux.ToLower())
            {
                return Qscore;
            }
            else
            {
                return 0;
            }

            

        }

        private static int OutputResp(string inputResp)
        {
            string[] assuntos = QuestOperation.GetAssuntos();
            questoes = QuestOperation.GetALLQuests(inputResp);
            int sc = 0; int detecAssunto = -1;
            if(inputResp.ToLower() == "sair")
            {
                detecAssunto = 0;
            }
            else
            {
                for (int i = 0; i < assuntos.Length; i++)
                {
                    if (assuntos[i] == inputResp)
                    {
                        detecAssunto = 0;
                        Console.WriteLine("==================================");
                        for (int j = 0; j < questoes.Count; j++)
                        {
                            questoes[j].Pergunta = questoes[j].Pergunta.Replace("\\n", "\n");
                            sc += DoQuiz(questoes[j].Pergunta, questoes[j].Alternativas, questoes[j].Resposta, questoes[j].Score, 2, questoes[j].Id);
                        }
                        Console.WriteLine("==================================");
                        Console.WriteLine($"-->Sua pontuacao:{sc}");
                        break;
                    }
                }
            }

            return detecAssunto;
        }
        private static void PrintMenuQuiz() {

            string[] assuntos = QuestOperation.GetAssuntos();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("    >----------------------<");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(">---  Bem vindo ao Quiz!  ---<");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(">---  Selecione o topico  ---<");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("    >----------------------<");
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine("\n->Menu<-\n");
            foreach (string text in assuntos)
            {
                Console.WriteLine($"[{text}] Unidade {text[1]} Secao {text[3]}");
            }
            Console.WriteLine("[Sair] Voltar para o menu principal");

            Console.WriteLine("==================================");
        }


    }
}
