using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuizXmlConsole
{
    //Classe responsável por encapsular os dados de uma questao, representa a questao.
    class QuestData
    {

        private string pergunta;
        private List<string> alternativas = new List<string>();
        private string resposta;
        private string id;
        private string assunto;
        private int score;
        
         

        #region Getter_Setter

        public string Resposta
        {
            get { return resposta; } set { resposta = value; }
        }

        public string Id
        {
            get { return id; } set { id = value; }
        }

        public int Score
        {
            get { return score; } set { score = value; }
        }

        public string Pergunta { get => pergunta; set => pergunta = value; }
        public List<string> Alternativas { get => alternativas; set => alternativas = value; }
        public string Assunto { get => assunto; set => assunto = value; }
        #endregion
    }
}
