using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PesquisaDeEfetividade
{
    public partial class Form1 : Form
    {
        private readonly IList<ComboBox> _combosDeRespostas ;
        private readonly string _connectionString;
        private int? _idDoAtendimento;
        private int _quantidadeDeRegistrosInseridos;

        private void InicializarCombos()
        {
            foreach (var comboBox in _combosDeRespostas)
            {
                comboBox.SelectedIndexChanged += comboBoxX_SelectedIndexChanged;
            }
        }

        private void ReiniciarRespostas()
        {
            foreach (var comboBox in _combosDeRespostas)
            {
                comboBox.SelectedItem = null;
            }
        }


        private void CarregarOpcoesDosCombos()
        {
            var consultaDeResposta = new ConsultaDeResposta(_connectionString);
            try
            {
                IList<RespostaPossivel> respostasPossiveis = consultaDeResposta.ListarTodas();

                foreach (var comboBox in _combosDeRespostas)
                {
                    ComboBox box = comboBox;
                    IEnumerable<RespostaPossivel> respostasDaPergunta = respostasPossiveis.Where(rp => rp.IdPergunta == Convert.ToInt32(box.Tag));

                    foreach (var resposta in respostasDaPergunta)
                    {
                        box.Items.Add(resposta);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public Form1()
        {
            InitializeComponent();

            _connectionString = ConfigurationManager.ConnectionStrings["SQL_SEBRAE"].ConnectionString;
            
            _combosDeRespostas = new List<ComboBox>();

            foreach (var control in this.Controls)
            {
                var comboBox = control as ComboBox;
                if (comboBox != null)
                {
                    _combosDeRespostas.Add(comboBox);
                }
            }

        }

        private void comboBoxX_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendKeys.Send("{TAB}");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResultadoDaBuscaDoCnpj.Text = "";
        }

        private void Salvar_Click(object sender, EventArgs e)
        {
            if (!this._idDoAtendimento.HasValue)
            {
                MessageBox.Show("Atendimento não encontrado");
                return;
            }

            if (_combosDeRespostas.Any(combo => combo.SelectedItem == null))
            {
                MessageBox.Show("Existem perguntas não respondidas");
                return;
            }

            var geradorDeArquivo = new GeradorDeArquivo();
            IEnumerable<int> respostas = _combosDeRespostas
                .OrderBy(combo => Convert.ToInt32(combo.Tag))
                .Select(combo => ((RespostaPossivel) combo.SelectedItem).IdResposta);

            geradorDeArquivo.Gerar(this._idDoAtendimento.Value,respostas);

            QuantidadeDeRegistrosAdicionados.Text = Convert.ToString(++_quantidadeDeRegistrosInseridos);

            ReiniciarRespostas();

            this.Cnpj.Text = "";
            this.ResultadoDaBuscaDoCnpj.Text = "";

            this._idDoAtendimento = null;

            SendKeys.Send("{TAB}{TAB}{TAB}");
        }

        private void Cnpj_Validating(object sender, CancelEventArgs e)
        {
            string cnpj = Cnpj.Text.Trim();
            if (cnpj.Length != 14)
            {
                ResultadoDaBuscaDoCnpj.ForeColor = Color.Red;
                ResultadoDaBuscaDoCnpj.Text = "CNPJ inválido";
                this._idDoAtendimento = null;
                return;
            }

            var consultaDeAtendimento = new ConsultaDeAtendimento(_connectionString);

            try
            {
                this._idDoAtendimento = consultaDeAtendimento.PorCnpj(cnpj);

                if (this._idDoAtendimento.HasValue)
                {
                    int quantidadeDeRespostas = consultaDeAtendimento.ContarRespostas(this._idDoAtendimento.Value);

                    if (quantidadeDeRespostas == 0)
                    {
                        ResultadoDaBuscaDoCnpj.Text = string.Format("Atendimento {0}", this._idDoAtendimento);
                        ResultadoDaBuscaDoCnpj.ForeColor = Color.ForestGreen;
                    }
                    else
                    {
                        ResultadoDaBuscaDoCnpj.Text = string.Format("Atendimento {0} já possui respostas",
                            this._idDoAtendimento);
                        ResultadoDaBuscaDoCnpj.ForeColor = Color.DarkOrange;

                    }
                }
                else
                {
                    ResultadoDaBuscaDoCnpj.Text = "Atendimento não encontrado";
                    ResultadoDaBuscaDoCnpj.ForeColor = Color.Red;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Cnpj_Enter(object sender, EventArgs e)
        {
            ResultadoDaBuscaDoCnpj.Text = "";
        }

        private void ReiniciarContador_Click(object sender, EventArgs e)
        {
            this._quantidadeDeRegistrosInseridos = 0;
            this.QuantidadeDeRegistrosAdicionados.Text = "0";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            CarregarOpcoesDosCombos();
            InicializarCombos();

        }
    }
}
