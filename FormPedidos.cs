using ServiceTrack.Client.Models;
using ServiceTrack.Client.Services;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            ConfigurarLayout();
            AplicarEstilo();
        }
        private void ConfigurarLayout()
        {
            SuspendLayout();

            // Painel dos campos: labels à esquerda e campos à direita.
            var painelCampos = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 7,
                Margin = Padding.Empty
            };

            painelCampos.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            painelCampos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label[] etiquetas =
            {
                label1, label2, label3, label4,label5, label6, label7
            };

            Control[] campos =
            {
                txtTitulo, txtDescricao, txtNomeContato,
                txtEmailContato, txtCategoria,
                cmbPrioridade, cmbEstado
               };

            for (int i = 0; i < campos.Length; i++)
            {
                painelCampos.RowStyles.Add(
                    new RowStyle(SizeType.AutoSize));

                etiquetas[i].AutoSize = true;
                etiquetas[i].Anchor = AnchorStyles.Left;
                etiquetas[i].Margin = new Padding(0, 6, 12, 6);

                campos[i].Dock = DockStyle.Fill;
                campos[i].Margin = new Padding(0, 3, 0, 3);

                // Move os controlos existentes para as respetivas células.
                painelCampos.Controls.Add(etiquetas[i], 0, i);
                painelCampos.Controls.Add(campos[i], 1, i);
            }

            txtDescricao.Multiline = true;
            txtDescricao.Height = 70;
            txtDescricao.ScrollBars = ScrollBars.Vertical;

            // Painel que organiza os botões e permite mudar de linha.
            var flpBotoes = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,

                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Margin = Padding.Empty,
                Padding = new Padding(0, 6, 0, 0)
            };

            Button[] botoes =
            {
                btnLimpar, btnGuardar, btnAtualizar,btnEliminar, btnRecarregar
            };

            foreach (Button botao in botoes)
            {
                botao.Dock = DockStyle.None;
                botao.Anchor = AnchorStyles.Top | AnchorStyles.Left;

                flpBotoes.Controls.Add(botao);
            }

            // A grelha ocupa a sua própria área.
            dgvPedidos.Dock = DockStyle.Fill;
            dgvPedidos.Margin = new Padding(0, 8, 0, 0);
            // Reserva altura para apresentar os pedidos.
            dgvPedidos.MinimumSize = new Size(0, 150);

            var tlpPrincipal = new TableLayoutPanel
            {
                Name = "tlpPrincipal"
            };

            tlpPrincipal.AutoSize = true;
            tlpPrincipal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlpPrincipal.ColumnCount = 1;
            tlpPrincipal.RowCount = 3;
            tlpPrincipal.Padding = new Padding(12);
            tlpPrincipal.Dock = DockStyle.Top;
            tlpPrincipal.AutoScroll = false;

            tlpPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlpPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 158));
            tlpPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tlpPrincipal.Controls.Add(painelCampos, 0, 0);
            tlpPrincipal.Controls.Add(dgvPedidos, 0, 1);
            tlpPrincipal.Controls.Add(flpBotoes, 0, 2);

            // O painel exterior disponibiliza o deslocamento de todo o conteúdo.
            var painelDeslocamento = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            painelDeslocamento.Controls.Add(tlpPrincipal);
            Controls.Add(painelDeslocamento);
            painelDeslocamento.BringToFront();

            ResumeLayout(true);
        }
        private void AplicarEstilo()
        {
            var fonteInterface = new Font("Segoe UI", 9F);
            BackColor = Color.FromArgb(243, 244, 246);
            ForeColor = Color.FromArgb(31, 41, 55);

            // Aplica a fonte aos controlos, sem alterar a fonte do formulário.

            Control[] controlos =
            {
                label1, label2, label3, label4, label5, label6, label7,
                txtTitulo, txtDescricao, txtNomeContato, txtEmailContato,
                txtCategoria, cmbPrioridade, cmbEstado, dgvPedidos
            };

            foreach (Control controlo in controlos)
            {
                controlo.Font = fonteInterface;
            }

            Button[] botoes =
            {
                btnLimpar, btnGuardar, btnAtualizar,
                btnEliminar, btnRecarregar
            };

            foreach (Button botao in botoes)
            {
                botao.Font = fonteInterface;

                botao.FlatStyle = FlatStyle.Flat;
                botao.UseVisualStyleBackColor = false;

                botao.AutoSize = true;
                botao.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                botao.MinimumSize = new Size(96, 36);
                botao.Padding = new Padding(10, 4, 10, 4);
                botao.Margin = new Padding(0, 0, 8, 8);

                botao.BackColor = Color.White;
                botao.ForeColor = Color.FromArgb(55, 65, 81);

                botao.FlatAppearance.BorderSize = 1;
                botao.FlatAppearance.BorderColor =
                    Color.FromArgb(209, 213, 219);

                botao.FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(243, 244, 246);

                botao.FlatAppearance.MouseDownBackColor =
                    Color.FromArgb(229, 231, 235);

                botao.Cursor = Cursors.Hand;
            }

            // Ação principal.
            btnGuardar.BackColor = Color.FromArgb(37, 99, 235);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.FlatAppearance.BorderColor = Color.FromArgb(37, 99, 235);
            btnGuardar.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
            btnGuardar.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);

            // Eliminação: destaque discreto em vermelho.
            btnEliminar.ForeColor = Color.FromArgb(185, 28, 28);
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnEliminar.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 242, 242);
            btnEliminar.FlatAppearance.MouseDownBackColor = Color.FromArgb(254, 226, 226);

            // Aspeto geral da grelha.
            dgvPedidos.BackgroundColor = Color.White;
            dgvPedidos.BorderStyle = BorderStyle.None;
            dgvPedidos.RowHeadersVisible = false;

            dgvPedidos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgvPedidos.GridColor = Color.FromArgb(229, 231, 235);

            // Cabeçalhos.
            dgvPedidos.EnableHeadersVisualStyles = false;

            dgvPedidos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(229, 231, 235);

            dgvPedidos.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);

            dgvPedidos.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(229, 231, 235);

            dgvPedidos.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(31, 41, 55);

            dgvPedidos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Linhas e seleção.
            dgvPedidos.DefaultCellStyle.BackColor = Color.White;
            dgvPedidos.DefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);

            dgvPedidos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);

            dgvPedidos.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);

            dgvPedidos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 251);

            dgvPedidos.DefaultCellStyle.Padding = new Padding(4);
            dgvPedidos.RowTemplate.Height = 34;

            // Aplica os títulos quando as colunas forem geradas pelos dados.
            dgvPedidos.DataBindingComplete += (sender, e) => FormatarColunas();
        }
        private void FormatarColunas()
        {
            // Protege o método durante a limpeza do DataSource.
            if (!dgvPedidos.Columns.Contains("Id"))
            {
                return;
            }

            dgvPedidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dgvPedidos.Columns["Id"].HeaderText = "ID";
            dgvPedidos.Columns["Titulo"].HeaderText = "Título";
            dgvPedidos.Columns["Descricao"].HeaderText = "Descrição";
            dgvPedidos.Columns["NomeContato"].HeaderText = "Nome do contacto";
            dgvPedidos.Columns["EmailContato"].HeaderText = "Email do contacto";
            dgvPedidos.Columns["Categoria"].HeaderText = "Categoria";
            dgvPedidos.Columns["Prioridade"].HeaderText = "Prioridade";
            dgvPedidos.Columns["Estado"].HeaderText = "Estado";

            string[] datas = { "CriadoEm", "AtualizadoEm", "ResolvidoEm" };
            string[] titulos = { "Criado em", "Atualizado em", "Resolvido em" };

            for (int i = 0; i < datas.Length; i++)
            {
                var coluna = dgvPedidos.Columns[datas[i]];

                coluna.HeaderText = titulos[i];
                coluna.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                coluna.DefaultCellStyle.NullValue = "—";
            }

            // Evita que descrições longas produzam colunas enormes.
            dgvPedidos.Columns["Titulo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPedidos.Columns["Titulo"].Width = 220;

            dgvPedidos.Columns["Descricao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPedidos.Columns["Descricao"].Width = 280;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
                MessageBox.Show(ex.Message,
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
        private async void btnAtualizar_Click(object sender, EventArgs e)
        {
            // Exige a seleção de um pedido antes da atualização.
            if (_pedidoIdSelecionado == 0)
            {
                MessageBox.Show(
                    "Selecione um pedido na grelha antes de atualizar!",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Verifica os campos obrigatórios.
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

            var pedidoAtualizado = new Pedido
            {
                Id = _pedidoIdSelecionado,
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
                bool sucesso = await ApiService.AtualizarPedidoAsync(
                    _pedidoIdSelecionado, pedidoAtualizado);

                if (sucesso)
                {
                    MessageBox.Show(
                        "Pedido atualizado com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LimparCampos();
                    await CarregarListaPedidos();
                }
                else
                {
                    MessageBox.Show(
                        "Não foi possível atualizar o pedido. Verifique os dados e se o " +
                        "pedido ainda existe.",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Erro de comunicação com a API",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_pedidoIdSelecionado == 0)
            {
                MessageBox.Show
                (
                    "Selecione um pedido na grelha antes de eliminar!",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // Guarda o ID do pedido cuja eliminação será confirmada.
            int idParaEliminar = _pedidoIdSelecionado;

            var confirmar = MessageBox.Show(
                $"Deseja eliminar o pedido '{txtTitulo.Text}'?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool sucesso = await ApiService.EliminarPedidoAsync(idParaEliminar);

                if (sucesso)
                {
                    MessageBox.Show(
                        "Pedido eliminado com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LimparCampos();
                    await CarregarListaPedidos();
                }
                else
                {
                    MessageBox.Show(
                        "Não foi possível eliminar o pedido. Verifique se ainda existe.",
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
        private async void btnRecarregar_Click(object sender, EventArgs e)
        {
            // Remove os dados da seleção anterior.
            LimparCampos();

            // Obtém novamente os pedidos da API.
            await CarregarListaPedidos();
        }
        private void txtDescricao_TextChanged(object sender, EventArgs e)
        {
        }
        private void FormPedidos_ResizeBegin(object sender, EventArgs e)
        {
        }
    }
}
