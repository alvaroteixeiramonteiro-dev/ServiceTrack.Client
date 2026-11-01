using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceTrack.Client.Services;
using ServiceTrack.Client.Models;

namespace ServiceTrack.Client
{
    public partial class FormPedidos : Form
    {
        // Guarda o ID do pedido selecionado para posterior edição ou eliminação.
        private int _pedidoIdSelecionado = 0;
        public FormPedidos()
        {
            InitializeComponent();
            
            // Define os valores iniciais para um novo pedido.
            cmbPrioridade.SelectedItem = "Normal";
            cmbEstado.SelectedItem = "Aberto";
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void CarregarDadosSelecionados(int rowIndex)
        {
            // Ignora cliques nos cabeçalhos ou fora das linhas de dados.
            if (rowIndex < 0 || rowIndex >= dgvPedidos.Rows.Count)
            {
                return;
            }

            DataGridViewRow linha = dgvPedidos.Rows[rowIndex];

            // Obtém o objeto Pedido associado à linha.
            if (linha.DataBoundItem is Pedido pedido)
            {
                _pedidoIdSelecionado = pedido.Id;

                txtTitulo.Text = pedido.Titulo;
                txtDescricao.Text = pedido.Descricao;
                txtNomeContato.Text = pedido.NomeContato;
                txtEmailContato.Text = pedido.EmailContato;
                txtCategoria.Text = pedido.Categoria;
                cmbPrioridade.SelectedItem = pedido.Prioridade;
                cmbEstado.SelectedItem = pedido.Estado;
            }
        }

        private void dgvPedidos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CarregarDadosSelecionados(e.RowIndex);
        }

        // Limpa os campos e prepara o formulário para um novo pedido.
        private void LimparCampos()
        {
            _pedidoIdSelecionado = 0;

            txtTitulo.Clear();
            txtDescricao.Clear();
            txtNomeContato.Clear();
            txtEmailContato.Clear();
            txtCategoria.Clear();

            cmbPrioridade.SelectedItem = "Normal";
            cmbEstado.SelectedItem = "Aberto";

            dgvPedidos.ClearSelection();
            txtTitulo.Focus();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            // Verifica os campos obrigatórios antes de chamar a API.
            if (string.IsNullOrWhiteSpace(txtTitulo.Text) ||
                string.IsNullOrWhiteSpace(txtDescricao.Text) ||
                string.IsNullOrWhiteSpace(txtNomeContato.Text) ||
                string.IsNullOrWhiteSpace(txtEmailContato.Text) ||
                string.IsNullOrWhiteSpace(txtCategoria.Text) ||
                cmbPrioridade.SelectedItem == null ||
                cmbEstado.SelectedItem == null)
            {
                MessageBox.Show(
                    "Preencha todos os campos!",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var novoPedido = new Pedido
            {
                Titulo = txtTitulo.Text.Trim(),
                Descricao = txtDescricao.Text.Trim(),
                NomeContato = txtNomeContato.Text.Trim(),
                EmailContato = txtEmailContato.Text.Trim(),
                Categoria = txtCategoria.Text.Trim(),
                Prioridade = cmbPrioridade.SelectedItem.ToString(),
                Estado = cmbEstado.SelectedItem.ToString()
            };

            try
            {
                bool sucesso = await ApiService.CriarPedidoAsync(novoPedido);

                if (sucesso)
                {
                    MessageBox.Show(
                        "Pedido inserido com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LimparCampos();
                    await CarregarListaPedidos();
                }
                else
                {
                    MessageBox.Show(
                        "Erro ao inserir o pedido. Verifique os dados introduzidos.",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Erro de comunicação com a API",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
