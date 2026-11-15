using System;
using System.Drawing;
using System.Windows.Forms;

namespace ServiceTrack.Client
{
    public partial class principal : Form
    {
        private int childFormNumber = 0;

        public principal()
        {
            InitializeComponent();
            AplicarEstiloPrincipal();
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.CenterScreen;

            // Garante uma única associação ao evento.
            Resize -= Principal_Resize;

            // Ajusta os formulários quando a área MDI muda de tamanho.
            foreach (Control controlo in Controls)
            {
                if (controlo is MdiClient areaMdi)
                {
                    areaMdi.SizeChanged += (sender, e) => AjustarJanelasMdi();
                }
            }

            Shown += (sender, e) => AjustarJanelasMdi();
        }
        // Uniformiza as cores e fontes da janela principal.
        private void AplicarEstiloPrincipal()
        {
            var fonteInterface = new Font("Segoe UI", 9F);
            Color fundo = Color.FromArgb(243, 244, 246);
            Color texto = Color.FromArgb(31, 41, 55);

            menuStrip.Font = fonteInterface;
            menuStrip.BackColor = Color.White;
            menuStrip.ForeColor = texto;

            toolStrip.Font = fonteInterface;
            toolStrip.BackColor = fundo;
            toolStrip.ForeColor = texto;

            statusStrip.Font = fonteInterface;
            statusStrip.BackColor = fundo;
            statusStrip.ForeColor = texto;

            foreach (Control controlo in Controls)
            {
                if (controlo is MdiClient areaMdi)
                {
                    areaMdi.BackColor = Color.FromArgb(229, 231, 235);
                }
            }
        }

        private void AjustarJanelasMdi()
        {
            foreach (Control controlo in Controls)
            {
                if (controlo is MdiClient areaMdi)
                {
                    // Não ajusta enquanto a área não tiver dimensões válidas.
                    if (areaMdi.ClientSize.Width <= 0 ||
                        areaMdi.ClientSize.Height <= 0)
                    {
                        return;
                    }

                    foreach (Form janela in MdiChildren)
                    {
                        if (janela is FormPedidos &&
                            janela.WindowState == FormWindowState.Normal)
                        {
                            janela.MinimumSize = Size.Empty;

                            janela.SetBounds(
                                0,
                                0,
                                Math.Min(819, areaMdi.ClientSize.Width),
                                Math.Min(611, areaMdi.ClientSize.Height));
                        }
                    }
                }
            }
        }
        private void Principal_Resize(object sender, EventArgs e)
        {
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Janela " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt|Todos os arquivos (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt|Todos os arquivos (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void toolStripStatusLabel_Click(object sender, EventArgs e)
        {

        }

        private void gerirPedidosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPedidos formPedidos = new FormPedidos();
            formPedidos.MdiParent = this;
            formPedidos.Show();
            AjustarJanelasMdi();
        }
    }
}
