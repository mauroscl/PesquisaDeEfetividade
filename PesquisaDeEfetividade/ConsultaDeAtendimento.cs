using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PesquisaDeEfetividade
{
    public class ConsultaDeAtendimento
    {
        public readonly string _stringDeConexao;

        public ConsultaDeAtendimento(string stringDeConexao)
        {
            _stringDeConexao = stringDeConexao;
        }

        //public int PorCnpj(string cnpj)
        //{
        //    using (var conexao = new SqlConnection(_stringDeConexao))
        //    {
        //        SqlCommand sqlCommand = conexao.CreateCommand();
        //        sqlCommand.CommandText = 
        //    }
        //}
    }

}
