using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesquisaDeEfetividade
{
    public class GeradorDeArquivo
    {
        public void Gerar(int idDoAtendimento, IEnumerable<int> respostas)
        {
            var linhas = new List<string>();
            linhas.Add(string.Format("IF dbo.NaN_ContarRespostasRealizadasNaPesquisaDeEfetividade({0}) = 0",idDoAtendimento));
            linhas.Add("BEGIN ");

            foreach (var resposta in respostas)
            {
                linhas.Add(string.Format("\tEXEC NAN_InserirRespostaDaPesquisaDeEfetividade {0}, {1}",idDoAtendimento, resposta));
            }

            linhas.Add("END");
            linhas.Add(Environment.NewLine);

            File.AppendAllLines("PesquisaDeEfetividade.sql", linhas);

        }
    }
}
