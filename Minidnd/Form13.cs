using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto_Dnd
{
    public partial class Form13 : Form
    {
        private int jugadorId;
        private Panel panelEstadisticas;

        public Form13(int idJugador)
        {
            InitializeComponent();
            jugadorId = idJugador;
            ConfigurarUI();
            CargarMetricas();
        }

        private void Form13_Load(object sender, EventArgs e)
        {
        }

        private void ConfigurarUI()
        {
            this.Text = "Estadísticas del Aventurero";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(40, 30, 20);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitulo = new Label
            {
                Text = "REGISTRO DE AVENTURAS",
                Font = new Font("Papyrus", 18, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = false,
                Size = new Size(660, 50),
                Location = new Point(20, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelEstadisticas = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(660, 430),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            Button btnCerrar = new Button
            {
                Text = "CERRAR",
                Location = new Point(260, 525),
                Size = new Size(180, 40),
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCerrar.FlatAppearance.BorderColor = Color.Gold;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitulo);
            this.Controls.Add(panelEstadisticas);
            this.Controls.Add(btnCerrar);
        }

        private void CargarMetricas()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmdInicializar = new MySqlCommand("CALL InicializarMetricas(@id)", conexion))
                    {
                        cmdInicializar.Parameters.AddWithValue("@id", jugadorId);
                        cmdInicializar.ExecuteNonQuery();
                    }

                    using (MySqlCommand cmd = new MySqlCommand("CALL ObtenerMetricas(@id)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", jugadorId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int zombis = reader.GetInt32("zombis_derrotados");
                                int enemigos = reader.GetInt32("enemigos_totales");
                                int zonas = reader.GetInt32("zonas_visitadas");
                                int compras = reader.GetInt32("compras_realizadas");
                                int pociones = reader.GetInt32("pociones_usadas");
                                int oroGastado = reader.GetInt32("oro_gastado");
                                int tiempo = reader.GetInt32("tiempo_jugado");
                                int muertes = reader.GetInt32("muertes");

                                MostrarEstadisticas(zombis, enemigos, zonas, compras, pociones, oroGastado, tiempo, muertes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar métricas: {ex.Message}\n\nDetalles: {ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarEstadisticas(int zombis, int enemigos, int zonas, int compras, int pociones, int oroGastado, int tiempo, int muertes)
        {
            panelEstadisticas.Controls.Clear(); // Limpiar controles previos

            int yPos = 20;

            CrearEtiquetaMetrica("⚔️ COMBATE", yPos, true);
            yPos += 40;
            CrearEtiquetaMetrica($"   Enemigos derrotados: {enemigos}", yPos);
            yPos += 30;
            CrearEtiquetaMetrica($"   Zombis eliminados: {zombis}", yPos);
            yPos += 30;
            CrearEtiquetaMetrica($"   Veces caído en combate: {muertes}", yPos);
            yPos += 50;

            CrearEtiquetaMetrica("🗺️ EXPLORACIÓN", yPos, true);
            yPos += 40;
            CrearEtiquetaMetrica($"   Zonas descubiertas: {zonas}", yPos);
            yPos += 30;
            CrearEtiquetaMetrica($"   Tiempo de aventura: {tiempo} minutos", yPos);
            yPos += 50;

            CrearEtiquetaMetrica("💰 ECONOMÍA", yPos, true);
            yPos += 40;
            CrearEtiquetaMetrica($"   Compras realizadas: {compras}", yPos);
            yPos += 30;
            CrearEtiquetaMetrica($"   Oro gastado: {oroGastado}", yPos);
            yPos += 30;
            CrearEtiquetaMetrica($"   Pociones consumidas: {pociones}", yPos);
        }

        private void CrearEtiquetaMetrica(string texto, int y, bool esSeccion = false)
        {
            Label lbl = new Label
            {
                Text = texto,
                Location = new Point(20, y),
                Size = new Size(620, 25),
                Font = esSeccion ? new Font("Papyrus", 12, FontStyle.Bold) : new Font("Segoe UI", 10),
                ForeColor = esSeccion ? Color.Gold : Color.White,
                BackColor = Color.Transparent,
                AutoSize = false
            };

            panelEstadisticas.Controls.Add(lbl);
        }
    }
}