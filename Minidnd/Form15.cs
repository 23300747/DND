using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto_Dnd
{
    public partial class Form15 : Form
    {
        private int jugadorId;
        private int hpActual;
        private int hpMax;
        private Label lblEstado;
        private Button btnDescansar;

        public Form15(int idJugador, int hp, int hpMaximo)
        {
            InitializeComponent();
            jugadorId = idJugador;
            hpActual = hp;
            hpMax = hpMaximo;
            ConfigurarUI();
        }
        private void Form15_Load(object sender, EventArgs e)
        {
        }
        private void ConfigurarUI()
        {
            this.Text = "Descanso Largo";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 30, 20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel panelPrincipal = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(460, 340),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitulo = new Label
            {
                Text = "LUGAR DE DESCANSO",
                Font = new Font("Papyrus", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(20, 20),
                Size = new Size(420, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblDescripcion = new Label
            {
                Text = "Encuentras un lugar seguro para descansar.\n\n" +
                       "Un descanso largo restaura completamente\n" +
                       "tus puntos de vida y te prepara para\n" +
                       "los desafíos venideros.\n\n" +
                       "¿Deseas acampar aquí?",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(20, 80),
                Size = new Size(420, 120),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblEstado = new Label
            {
                Text = $"Vida actual: {hpActual}/{hpMax} HP",
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                ForeColor = hpActual < hpMax ? Color.Orange : Color.LightGreen,
                Location = new Point(20, 210),
                Size = new Size(420, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnDescansar = new Button
            {
                Text = "DESCANSAR (Restaurar HP)",
                Location = new Point(90, 260),
                Size = new Size(280, 40),
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = hpActual < hpMax
            };
            btnDescansar.FlatAppearance.BorderColor = Color.Gold;
            btnDescansar.Click += BtnDescansar_Click;

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(190, 310),
                Size = new Size(120, 35),
                Font = new Font("Papyrus", 10),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancelar.FlatAppearance.BorderColor = Color.Gold;
            btnCancelar.Click += (s, e) => this.Close();

            panelPrincipal.Controls.Add(lblTitulo);
            panelPrincipal.Controls.Add(lblDescripcion);
            panelPrincipal.Controls.Add(lblEstado);
            panelPrincipal.Controls.Add(btnDescansar);
            panelPrincipal.Controls.Add(btnCancelar);

            this.Controls.Add(panelPrincipal);
        }

        private void BtnDescansar_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL DescansarLargo(@id, @resultado)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);
                        cmd.Parameters.Add("@resultado", MySqlDbType.VarChar, 100).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        string resultado = cmd.Parameters["@resultado"].Value.ToString();

                        MessageBox.Show(
                            "Has descansado profundamente.\n\n" +
                            "Tus heridas se han curado.\n" +
                            "Te sientes renovado y listo para continuar.",
                            "Descanso Completado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al descansar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}