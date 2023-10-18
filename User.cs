using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace QuizXmlConsole
{
    class User
    {
        private string login;
        private string senha;
        private string lv_acesso;

        private static string userdb_path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @".\DataBase\UserDB.xml";

        public User(string Ulogin, string Usenha) {

            if (IsUserExist(Ulogin,Usenha)) {

                this.senha = Usenha;
                this.login = Ulogin;
            }
            else
            {
                Console.WriteLine("ERRO, Falha ao criar OBJ_user, Login e/ou senha invalido(s)");
            }

        }
        
        #region Verificacao
        private bool IsUserExist(string UL, string US)
        {
            bool isuserexist = false;

            XElement root = XElement.Load(userdb_path);

            IEnumerable<XElement> user =
                from el in root.Elements("User")
                where ( (string)el.Element("login") == UL && (string)el.Element("passw") == US)
                select el;
            

            if (user.Any())
            {
                isuserexist = true;
                Lv_acesso = (String) user.Attributes("type").First();
            }
            return isuserexist;
        }

        public static bool AutenticarLogin(string dlog, string dsen) {

            bool logado = false;
            XElement root = XElement.Load(userdb_path);
            IEnumerable<XElement> UserLogin =
               from el in root.Elements("User")
               where ((string)el.Element("login") == dlog && (string)el.Element("passw") == dsen)
               select el;

            if (UserLogin.Any())
            {
                logado = true;
            }
            return logado;
        }
        #endregion

        #region Getter_Setter
        public string Login{
            get { return login;}
            set { login = value;}        
        }

        public string Senha
        {
            get { return senha; }
            set { senha = value;}

        }

        public string Lv_acesso
        {
            get { return lv_acesso; }
            set { lv_acesso = value; }
        }
        #endregion
    }
}
