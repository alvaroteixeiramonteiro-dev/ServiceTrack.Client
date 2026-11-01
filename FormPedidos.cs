using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceTrack.Client.Services;

namespace ServiceTrack.Client
{
    public partial class FormPedidos : Form
    {
        public FormPedidos()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        {

        }

        // Carrega os pedidos ao abrir o formulário.
        private async void FormPedidos_Load(object sender, EventArgs e)
        {
            await CarregarListaPedidos();
        }

        // Obtém os pedidos da API e apresenta-os na grelha.
        private async Task CarregarListaPedidos()
        {
            try
            {
                var pedidos = await ApiService.ObterPedidosAsync();

                dgvPedidos.DataSource = null;
                dgvPedidos.DataSource = pedidos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Erro ao carregar pedidos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
