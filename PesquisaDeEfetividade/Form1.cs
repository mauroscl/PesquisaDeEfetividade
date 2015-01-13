using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PesquisaDeEfetividade
{
    public partial class Form1 : Form
    {
        private readonly IList<ComboBox> _combosDeRespostas ;

        private void InicializarCombos()
        {
            foreach (var comboBox in _combosDeRespostas)
            {
                comboBox.SelectedIndexChanged += comboBoxX_SelectedIndexChanged;
            }
        }

        private void CarregarOpcoesDosCombos()
        {
            var consultaDeResposta = new ConsultaDeResposta(ConfigurationManager.ConnectionStrings["SQL_SEBRAE"].ConnectionString);
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


        public Form1()
        {
            InitializeComponent();
            
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
            CarregarOpcoesDosCombos();
            InicializarCombos();

        }

    }
}
