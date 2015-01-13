using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesquisaDeEfetividade
{
    public class ConsultaDeAtendimento
    {
        private readonly string _stringDeConexao;

        public ConsultaDeAtendimento(string stringDeConexao)
        {
            _stringDeConexao = stringDeConexao;
        }

        public int? PorCnpj(string cnpj)
        {
            using (var conexao = new SqlConnection(_stringDeConexao))
            {
                conexao.Open();

                SqlCommand sqlCommand = conexao.CreateCommand();
                var stringBuilder = new StringBuilder();
                stringBuilder
                    .AppendLine("SELECT AT.IdAtendimento ")
                    .AppendLine("FROM NAN_ATENDIMENTO AT INNER JOIN NAN_EMPRESAREFERENCIA ER")
                    .AppendLine("ON AT.IDEmpresaReferencia = ER.IDEmpresaReferencia")
                    .AppendLine("INNER JOIN PES_PJ PJ")
                    .AppendLine("ON PJ.IDPESSOA = ER.idpessoa")
                    .AppendLine("WHERE AT.TIPOATENDIMENTO = 12")
                    .AppendLine("AND CNPJ = REPLACE(REPLACE(REPLACE(@cnpj,'.',''),'/',''),'-','')");

                sqlCommand.CommandText = stringBuilder.ToString();
                sqlCommand.Parameters.AddWithValue("@cnpj", cnpj);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                sqlDataReader.Read();

                int? idAtendimento = sqlDataReader.HasRows ? sqlDataReader.GetInt32(0) : (int?) null;

                return idAtendimento;
            }
        }
    }

}
