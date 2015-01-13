using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesquisaDeEfetividade
{
    public class ConsultaDeResposta
    {
        public readonly string _stringDeConexao;

        public ConsultaDeResposta(string stringDeConexao)
        {
            _stringDeConexao = stringDeConexao;
        }

        public IList<RespostaPossivel> ListarTodas()
        {
            var respostasPossiveis = new List<RespostaPossivel>();

            using (var conexao = new SqlConnection(_stringDeConexao))
            {
                try
                {
                    conexao.Open();

                    SqlCommand sqlCommand = conexao.CreateCommand();
                    var stringBuilder = new StringBuilder();
                    stringBuilder
                        .AppendLine("select qp.id as idpergunta, qrp.id as idresposta, convert(varchar,qrp.id - menorresposta.idresposta + 1) + '-' + qrp.dscresposta")
                        .AppendLine("from nan_questionariopergunta qp inner join nan_questionariorespostapossivel qrp on qp.id = qrp.idpergunta")
                        .AppendLine("inner join")
                        .AppendLine("(")
                        .AppendLine("\tselect qp.id as idpergunta, min(qrp.id) as idresposta")
                        .AppendLine("\tfrom nan_questionariopergunta qp inner join nan_questionariorespostapossivel qrp on qp.id = qrp.idpergunta")
                        .AppendLine("\tgroup by qp.id")
                        .AppendLine(") as MenorResposta on qrp.idpergunta = menorresposta.idpergunta")
                        .AppendLine("where qp.tipoquestionario = 7");

                    sqlCommand.CommandText = stringBuilder.ToString();

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        respostasPossiveis.Add(new RespostaPossivel
                        {
                            IdPergunta = sqlDataReader.GetInt32(0),
                            IdResposta = sqlDataReader.GetInt32(1),
                            Descricao = sqlDataReader.GetString(2)
                        });
                    }
                }
                finally
                {
                    if (conexao.State != ConnectionState.Closed)
                    {
                        conexao.Close();
                    }
                }
            }

            return respostasPossiveis;
        }
    }

}
